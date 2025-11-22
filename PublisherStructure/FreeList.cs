using System;
using System.Collections.Generic;
using System.Threading;

namespace PublishStructure.Internal;

#nullable enable
#pragma warning disable CS8618

//Subscribeの内部


internal sealed class  FreeList<T> : IDisposable where T : class
{
    const int InitialCapacity = 4;
    const int MinShrinkStart = 8;

    T?[] values;
    int count;
    // 空いているインデックスのキュー
    //使われていないインデックスの記録
    //INitializeでサイズ [InitialCapacity] のint型FastQueueを生成し、その全てを満たすようにEnqueueしている。
    FastQueue<int> freeIndex;
    bool isDisposed;
    readonly object gate = new object();

    public FreeList()
    {
        Initialize();
    }

    //呼び出しに対してvaluesを返している。
    public T?[] GetValues() => values;

    public int GetCount()
    {
        lock (gate)
        {
            return count;
        }
    }

    public int Add(T value)
    {
        lock (gate)
        {
            if(isDisposed)
                throw new ObjectDisposedException(nameof(FreeList<T>));

            //freeIndex.CountはFastQueueのsizeを返す。
            //sizeは初期化で0に設定され、Enqueueで+1、Dequeueで-1される。
            //初期化時点で全ての要素が満たされている(使われていないインデックスとして記録。この時sizeは最大)
            //このifがtrueならば、使われていないインデックスが存在するということ。
            if (freeIndex.Count != 0)
            {
                var index = freeIndex.Dequeue();
                values[index] = value;
                count++;
                return index;
            }
            else
            {
                var newValues = new T[values.Length * 2];
                Array.Copy(values, 0, newValues, 0, values.Length);
                freeIndex.EnsureNewCapacity(newValues.Length);
                for (int i = values.Length; i < newValues.Length; i++)
                {
                    freeIndex.Enqueue(i);
                }

                var index = freeIndex.Dequeue();
                newValues[values.Length] = value;
                count++;
                //多スレッド環境で、安全に参照を書き換えるためのメモリバリア付き代入です。
                Volatile.Write(ref values, newValues);
                return index;
            }
        }
    }

    public void Remove(int index, bool shrinkWhenEmpty)
    {
        lock (gate)
        {
            if (isDisposed) return; // do nothing

            //Tの配列
            ref var v = ref values[index];
            if (v == null) throw new KeyNotFoundException($"key index {index} is not found.");

            v = null;
            freeIndex.Enqueue(index);
            count--;

            // 空になったら縮小する
            if (shrinkWhenEmpty && count == 0 && values.Length > MinShrinkStart)
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
            values = Array.Empty<T?>();
            count = 0;
        }
    }

    void Initialize()
    {
        //初期状態では全てのインデックスが使われていない状態なので、0からInitialCapacity-1までをEnqueueしている。
        freeIndex = new FastQueue<int>(InitialCapacity);
        for (int i = 0; i < InitialCapacity; i++)
        {
            freeIndex.Enqueue(i);
        }
        count = 0;

        var v = new T?[InitialCapacity];
        Volatile.Write(ref values, v);
    }
}