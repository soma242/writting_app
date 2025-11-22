#pragma warning disable CS8618

using System;
using System.Runtime.CompilerServices;

namespace PublishStructure.Internal;

// fixed size queue.
internal class FastQueue<T>
{
    //指定した型の配列
    T[] array;
    //現在入っている中で最初に入った要素の番号
    int head;
    //現在入っている中で最後に入った要素の次の番号
    int tail;
    //現在入っている要素数
    int size;

    public FastQueue(int capacity)
    {
        if (capacity < 0) throw new ArgumentOutOfRangeException("capacity");
        array = new T[capacity];
        //初期化なので全て0
        head = tail = size = 0;
    }

    public int Count
    {
        //軽い処理をインライン化するように要請。必ずではない。
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get { return size; }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Enqueue(T item)
    {
        if (size == array.Length)
        {
            //満杯。例外を投げる関数を呼んでいる。
            ThrowForFullQueue();
        }

        //tailの位置に要素を追加。
        //freeListからの呼び出しではindex(引数として取ってきたsubscriptionKey)を持ってきてintを入れている。
        //最後に入れた要素の次の位置にこれを入れているので、使用可能なIndex表の最後列に解放した要素番号を入れていることになる。
        array[tail] = item;
        //tailを円環の中で次に進める
        MoveNext(ref tail);
        size++;
    }

    //最初に入った要素を取り出す
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Dequeue()
    {
        //空。例外を投げる関数を呼んでいる。
        if (size == 0) ThrowForEmptyQueue();

        int head = this.head;
        T[] array = this.array;
        T removed = array[head];
        //配列要素を「既定値（デフォルト値）」に戻す
        array[head] = default!;

        //最初に入った要素の番号を次に進める
        MoveNext(ref this.head);
        size--;
        return removed;
    }

    //arrayのリサイズ。
    public void EnsureNewCapacity(int capacity)
    {
        T[] newarray = new T[capacity];
        if (size > 0)
        {
            if (head < tail)
            {
                Array.Copy(array, head, newarray, 0, size);
            }
            else
            {
                Array.Copy(array, head, newarray, 0, array.Length - head);
                Array.Copy(array, 0, newarray, array.Length - head, tail);
            }
        }

        array = newarray;
        head = 0;
        //size=元のarrayの最後の要素
        tail = (size == capacity) ? 0 : size;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void MoveNext(ref int index)
    {
        int tmp = index + 1;
        if (tmp == array.Length)
        {
            tmp = 0;
        }
        index = tmp;
    }

    void ThrowForEmptyQueue()
    {
        throw new InvalidOperationException("Queue is empty.");
    }

    void ThrowForFullQueue()
    {
        throw new InvalidOperationException("Queue is full.");
    }
}
