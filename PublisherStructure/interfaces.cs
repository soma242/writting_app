using System;

namespace PublishStructure;


public interface IPublisher<TMessage>
{
    void Publish(TMessage message);
}

public interface ISubscriber<TMessage>
{
    IDisposable Subscribe(IMessageHandler<TMessage> handler);
        //, params MessageHandlerFilter<TMessage>[] filters);因数をオミット
}

public interface IPublisher<TKey, TMessage>
    where TKey : notnull
{
    void Publish(TKey key, TMessage message);
}

public interface ISubscriber<TKey, TMessage>
    where TKey : notnull
{
    IDisposable Subscribe(TKey key, IMessageHandler<TMessage> handler);
        //, params MessageHandlerFilter<TMessage>[] filters);
}

public interface IMessageHandler<TMessage>
{
    void Handle(TMessage message);
}

