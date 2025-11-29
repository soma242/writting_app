using System;
using System.Collections.Generic;
using System.Threading;

using PublishStructure.Internal;

namespace writting_app;

#nullable enable
#pragma warning disable CS8618

//Subscribeの内部

//空いたインデックスをQueueに入れてFIFOで空いているインデックスを受け取っている。
//FreeListをいじったもの。FastQueueに依存している。

//必要なのはindex(MessagePipeのKey)の管理だけなので内部にValueを持つ必要はない。

internal sealed class IndexList
    //<T> : IDisposable where T : class
{
    const int InitialCapacity = 4;
    const int MinShrinkStart = 8;

    int capacity;

    int count;
    // 空いているインデックスのキュー
    //使われていないインデックスの記録
    //INitializeでサイズ [InitialCapacity] のint型FastQueueを生成し、その全てを満たすようにEnqueueしている。
    FastQueue<int> freeIndex;
    bool isDisposed;
    readonly object gate = new object();

    public IndexList()
    {
        Initialize();
    }

    //呼び出しに対してvaluesを返している。

    public int GetCount()
    {
        lock (gate)
        {
            return count;
        }
    }

    public int Add()
    {
        lock (gate)
        {
            if (isDisposed)
                throw new ObjectDisposedException("既にDisposeされています");

            //freeIndex.CountはFastQueueのsizeを返す。
            //sizeは初期化で0に設定され、Enqueueで+1(Initで最大値)、Dequeueで-1(Addするごとに減る)される。
            //初期化時点で全ての要素が満たされている(使われていないインデックスとして記録。この時sizeは最大)
            //このifがtrueならば、使われていないインデックスが存在するということ。
            if (freeIndex.Count != 0)
            {
                int index = freeIndex.Dequeue();
                count++;
                return index;
            }
            else
            {
                //新しいTの配列（元の二倍の大きさ）
                int newCapacity = capacity * 2;
                freeIndex.EnsureNewCapacity(newCapacity);
                for (int i = capacity; i < newCapacity; i++)
                {
                    freeIndex.Enqueue(i);
                }

                int index = freeIndex.Dequeue();
                count++;
                return index;
            }
        }
    }

    //Alignmentを増やすことは多くないのでshrinkWhenEmptyをオミット
    public void Remove(int index)
    {
        lock (gate)
        {
            if (isDisposed) return; // do nothing

            //Queueに最後に入れられた次の場所に新しく空いたインデックスの番号をintで渡している。
            freeIndex.Enqueue(index);
            count--;

            // 空になったら縮小する
            if (count == 0 && capacity > MinShrinkStart)
            {
                Initialize(); // re-init.
            }
        }
    }

    //outなので、呼び出し元で変数を用意してそれを操作している
    public bool TryDispose(out int clearedCount)
    {
        lock (gate)
        {
            if (isDisposed)
            {
                clearedCount = 0;
                return false;
            }

            clearedCount = count;
            Dispose();
            return true;
        }
    }


    public void Dispose()
    {
        lock (gate)
        {
            if (isDisposed) return;

            isDisposed = true;

            freeIndex = null!;
            count = 0;
        }
    }

    void Initialize()
    {
        isDisposed = false;
        //初期状態では全てのインデックスが使われていない状態なので、0からInitialCapacity-1までをEnqueueしている。
        freeIndex = new FastQueue<int>(InitialCapacity);
        for (int i = 0; i < InitialCapacity; i++)
        {
            freeIndex.Enqueue(i);
        }
        count = 0;
        capacity = InitialCapacity;
    }
}