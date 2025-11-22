using PublishStructure;
using PublishStructure.Internal;
using System;
using System.Runtime.CompilerServices;

namespace PublishStructure;

//MessagePIpeを分解して最小単位のイベントを作成したい。
//PublisherStructureファイル内に収める。それ以外は排除する。

//preserveはunityでないので不要。


//BuiltInContainers.cs内の
//ServiceProviderTypeクラスのInstantiateメソッドで生成されている。

//実際にSubPubを仲介するインスタンス
public class MessageBroker<TMessage> : IPublisher<TMessage>, ISubscriber<TMessage>
{
    readonly MessageBrokerCore<TMessage> core;
    //readonly FilterAttachedMessageHandlerFactory handlerFactory;


    public MessageBroker(MessageBrokerCore<TMessage> core)
    //, FilterAttachedMessageHandlerFactory handlerFactory) 引数をオミット
    {
        this.core = core;
        //Subscribeの引数であるIMessageHandler<TMessage>をfilter込みで再生成するために使っている。
        //this.handlerFactory = handlerFactory;
    }

    public void Publish(TMessage message)
    {
        core.Publish(message);
    }

    //
    public IDisposable Subscribe(IMessageHandler<TMessage> handler)
        //(IMessageHandler<TMessage> handler, params MessageHandlerFilter<TMessage>[] filters)
    {
        //ここもfiltersをオミットしているので注意

        //(FilterとOptionなし)受け取ったhandlerをcore.Subscribeに渡して返ってきたSubscriptionをそのまま返している。
        //return core.Subscribe(handlerFactory.CreateMessageHandler(handler));
        return core.Subscribe(handler);
    }

    //IHandlerHolderMarker
    //=>MessagePipeDiagnosticsInfo.csに記載。一旦オミット

    //Pub, Subのコア部分



        //BufferedMessageBrokerCore
        //SingletonMessageBroker, ScopedMessageBrokerおよびその二つのCoreはオミット



}

public class MessageBrokerCore<TMessage> : IDisposable
{
    readonly FreeList<IMessageHandler<TMessage>> handlers;
    //readonly MessagePipeDiagnosticsInfo diagnostics;　は一旦オミット。
    // =>Sub情報を表示するやつ
    //readonly HandlingSubscribeDisposedPolicy handlingSubscribeDisposedPolicy;もオミット
    // =>　ISubscriber.SubscribeがMessageBroker（パブリッシャー/サブスクライバーマネージャー）の破棄後（例えばスコープが破棄された後）に呼び出された場合の処理を左右するenum。デフォルトでignore。
    readonly object gate = new object();
    bool isDisposed;

    //引数のMessagePipeDiagnosticsInfo diagnostics, MessagePipeOptions optionsをオミット
    public MessageBrokerCore()
    {
        this.handlers = new FreeList<IMessageHandler<TMessage>>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Publish(TMessage message)
    {
        //handlers => FreeList<IMessageHandler<TMessage>>
        //GetValuesでTMessageの配列を返してもらっている。
        var array = handlers.GetValues();
        for (int i = 0; i < array.Length; i++)
        {
            //interface IMessageHandler<T>で定義されている
            //?ここで呼ばれているの(arrayの型)はAnonymousMessageHandler
            array[i]?.Handle(message);
        }
    }

    public IDisposable Subscribe(IMessageHandler<TMessage> handler)
    {
        lock (gate)
        {
            //if (isDisposed)
            //    handlingSubscribeDisposedPolicy.Handle(nameof(MessageBrokerCoreTMessage));

            //handlers(freeLis)tの中のAddメソッドで
            //freeIndx(FastQueue)のDequeueメソッドから返される使用できなくなるインデックス番号(今からsubscriptionが入る)を受け取っている。
            var subscriptionKey = handlers.Add(handler);
            //下で定義している。
            var subscription = new Subscription(this, subscriptionKey);
            //diagnostics.IncrementSubscribe(this, subscription);をオミット
            return subscription;

        }
    }

    public void Dispose()
    {
        lock (gate)
        {
            //handlers.TryDispose(out var count) => handler(freeList)のDisposeを試してdiagnosticsのためにcountを貰ってきている。
            if (!isDisposed && handlers.TryDispose(out var count))
            {
                isDisposed = true;
                //diagnostics.RemoveTargetDiagnostics(this, count);をオミット

            }
        }
    }

    sealed class Subscription : IDisposable
    {
        bool isDisposed;
        readonly MessageBrokerCore<TMessage> core;
        //自分が入ったインデックス番号
        readonly int subscriptionKey;

        public Subscription(MessageBrokerCore<TMessage> core, int subscriptionKey)
        {
            this.core = core;
            this.subscriptionKey = subscriptionKey;
        }

        public void Dispose()
        {
            if (!isDisposed)
            {
                isDisposed = true;
                lock (core.gate)
                {
                    if (!core.isDisposed)
                    {
                        //core内のhandlers(freeList)のRemoveを呼んでいる。該当するインデックス番号の要素をDispose
                        //更にfreeIndex(fastQueue)のEnqueue(index)を呼んで対応するインデックス番号を解放
                        //Enqueue
                        core.handlers.Remove(subscriptionKey, true);
                        //core.diagnostics.DecrementSubscribe(core, this);をオミット
                    }
                }
            }
        }
    }
}