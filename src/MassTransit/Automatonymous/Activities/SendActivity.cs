namespace Automatonymous.Activities
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit;


    public class SendActivity<TInstance, TMessage> :
        Activity<TInstance>
        where TInstance : SagaStateMachineInstance
        where TMessage : class
    {
        readonly AsyncEventMessageFactory<TInstance, TMessage> _asyncMessageFactory;
        readonly SendContextCallback<TInstance, TMessage> _contextCallback;
        readonly DestinationAddressProvider<TInstance> _destinationAddressProvider;
        readonly EventMessageFactory<TInstance, TMessage> _messageFactory;

        public SendActivity(DestinationAddressProvider<TInstance> destinationAddressProvider, EventMessageFactory<TInstance, TMessage> messageFactory,
            Action<SendContext<TMessage>> contextCallback)
            : this(messageFactory, Convert(contextCallback))
        {
            _destinationAddressProvider = destinationAddressProvider;
        }

        public SendActivity(DestinationAddressProvider<TInstance> destinationAddressProvider, AsyncEventMessageFactory<TInstance, TMessage> messageFactory,
            Action<SendContext<TMessage>> contextCallback)
            : this(messageFactory, Convert(contextCallback))
        {
            _destinationAddressProvider = destinationAddressProvider;
        }

        public SendActivity(EventMessageFactory<TInstance, TMessage> messageFactory, Action<SendContext<TMessage>> contextCallback)
            : this(Convert(contextCallback))
        {
            _messageFactory = messageFactory;
        }

        public SendActivity(AsyncEventMessageFactory<TInstance, TMessage> messageFactory, Action<SendContext<TMessage>> contextCallback)
            : this(Convert(contextCallback))
        {
            _asyncMessageFactory = messageFactory;
        }

        public SendActivity(DestinationAddressProvider<TInstance> destinationAddressProvider, EventMessageFactory<TInstance, TMessage> messageFactory,
            SendContextCallback<TInstance, TMessage> contextCallback)
            : this(messageFactory, contextCallback)
        {
            _destinationAddressProvider = destinationAddressProvider;
        }

        public SendActivity(DestinationAddressProvider<TInstance> destinationAddressProvider, AsyncEventMessageFactory<TInstance, TMessage> messageFactory,
            SendContextCallback<TInstance, TMessage> contextCallback)
            : this(messageFactory, contextCallback)
        {
            _destinationAddressProvider = destinationAddressProvider;
        }

        public SendActivity(EventMessageFactory<TInstance, TMessage> messageFactory, SendContextCallback<TInstance, TMessage> contextCallback)
            : this(contextCallback)
        {
            _messageFactory = messageFactory;
        }

        public SendActivity(AsyncEventMessageFactory<TInstance, TMessage> messageFactory, SendContextCallback<TInstance, TMessage> contextCallback)
            : this(contextCallback)
        {
            _asyncMessageFactory = messageFactory;
        }

        SendActivity(SendContextCallback<TInstance, TMessage> contextCallback)
        {
            _contextCallback = contextCallback;
        }

        public void Accept(StateMachineVisitor inspector)
        {
            inspector.Visit(this);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("send");
        }

        async Task Activity<TInstance>.Execute(BehaviorContext<TInstance> context, Behavior<TInstance> next)
        {
            await Execute(context).ConfigureAwait(false);

            await next.Execute(context).ConfigureAwait(false);
        }

        async Task Activity<TInstance>.Execute<T>(BehaviorContext<TInstance, T> context, Behavior<TInstance, T> next)
        {
            await Execute(context).ConfigureAwait(false);

            await next.Execute(context).ConfigureAwait(false);
        }

        Task Activity<TInstance>.Faulted<TException>(BehaviorExceptionContext<TInstance, TException> context, Behavior<TInstance> next)
        {
            return next.Faulted(context);
        }

        Task Activity<TInstance>.Faulted<T, TException>(BehaviorExceptionContext<TInstance, T, TException> context, Behavior<TInstance, T> next)
        {
            return next.Faulted(context);
        }

        async Task Execute(BehaviorContext<TInstance> context)
        {
            ConsumeEventContext<TInstance> consumeContext = context.CreateConsumeContext();

            var message = _messageFactory?.Invoke(consumeContext) ?? await _asyncMessageFactory(consumeContext).ConfigureAwait(false);

            IPipe<SendContext<TMessage>> sendPipe = _contextCallback != null
                ? Pipe.Execute<SendContext<TMessage>>(sendContext =>
                {
                    _contextCallback(consumeContext, sendContext);
                })
                : Pipe.Empty<SendContext<TMessage>>();

            if (_destinationAddressProvider != null)
            {
                var destinationAddress = _destinationAddressProvider(consumeContext);

                var endpoint = await consumeContext.GetSendEndpoint(destinationAddress).ConfigureAwait(false);


                await endpoint.Send(message, sendPipe).ConfigureAwait(false);
            }
            else
                await consumeContext.Send(message, sendPipe).ConfigureAwait(false);
        }

        static SendContextCallback<TInstance, TMessage> Convert(Action<SendContext<TMessage>> contextCallback)
        {
            if (contextCallback == null)
                return null;

            return (_, sendContext) => contextCallback(sendContext);
        }
    }


    public class SendActivity<TInstance, TData, TMessage> :
        Activity<TInstance, TData>
        where TInstance : SagaStateMachineInstance
        where TData : class
        where TMessage : class
    {
        readonly AsyncEventMessageFactory<TInstance, TData, TMessage> _asyncMessageFactory;
        readonly SendContextCallback<TInstance, TData, TMessage> _contextCallback;
        readonly DestinationAddressProvider<TInstance, TData> _destinationAddressProvider;
        readonly EventMessageFactory<TInstance, TData, TMessage> _messageFactory;

        public SendActivity(DestinationAddressProvider<TInstance, TData> destinationAddressProvider,
            EventMessageFactory<TInstance, TData, TMessage> messageFactory, Action<SendContext<TMessage>> contextCallback)
            : this(messageFactory, Convert(contextCallback))
        {
            _destinationAddressProvider = destinationAddressProvider;
        }

        public SendActivity(DestinationAddressProvider<TInstance, TData> destinationAddressProvider,
            AsyncEventMessageFactory<TInstance, TData, TMessage> messageFactory, Action<SendContext<TMessage>> contextCallback)
            : this(messageFactory, Convert(contextCallback))
        {
            _destinationAddressProvider = destinationAddressProvider;
        }

        public SendActivity(EventMessageFactory<TInstance, TData, TMessage> messageFactory, Action<SendContext<TMessage>> contextCallback)
            : this(Convert(contextCallback))
        {
            _messageFactory = messageFactory;
        }

        public SendActivity(AsyncEventMessageFactory<TInstance, TData, TMessage> messageFactory, Action<SendContext<TMessage>> contextCallback)
            : this(Convert(contextCallback))
        {
            _asyncMessageFactory = messageFactory;
        }

        public SendActivity(DestinationAddressProvider<TInstance, TData> destinationAddressProvider,
            EventMessageFactory<TInstance, TData, TMessage> messageFactory, SendContextCallback<TInstance, TData, TMessage> contextCallback)
            : this(messageFactory, contextCallback)
        {
            _destinationAddressProvider = destinationAddressProvider;
        }

        public SendActivity(DestinationAddressProvider<TInstance, TData> destinationAddressProvider,
            AsyncEventMessageFactory<TInstance, TData, TMessage> messageFactory, SendContextCallback<TInstance, TData, TMessage> contextCallback)
            : this(messageFactory, contextCallback)
        {
            _destinationAddressProvider = destinationAddressProvider;
        }

        public SendActivity(EventMessageFactory<TInstance, TData, TMessage> messageFactory, SendContextCallback<TInstance, TData, TMessage> contextCallback)
            : this(contextCallback)
        {
            _messageFactory = messageFactory;
        }

        public SendActivity(AsyncEventMessageFactory<TInstance, TData, TMessage> messageFactory,
            SendContextCallback<TInstance, TData, TMessage> contextCallback)
            : this(contextCallback)
        {
            _asyncMessageFactory = messageFactory;
        }

        SendActivity(SendContextCallback<TInstance, TData, TMessage> contextCallback)
        {
            _contextCallback = contextCallback;
        }

        void Visitable.Accept(StateMachineVisitor inspector)
        {
            inspector.Visit(this);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("send");
        }

        async Task Activity<TInstance, TData>.Execute(BehaviorContext<TInstance, TData> context, Behavior<TInstance, TData> next)
        {
            ConsumeEventContext<TInstance, TData> consumeContext = context.CreateConsumeContext();

            var message = _messageFactory?.Invoke(consumeContext) ?? await _asyncMessageFactory(consumeContext).ConfigureAwait(false);

            IPipe<SendContext<TMessage>> sendPipe = _contextCallback != null
                ? Pipe.Execute<SendContext<TMessage>>(sendContext =>
                {
                    _contextCallback(consumeContext, sendContext);
                })
                : Pipe.Empty<SendContext<TMessage>>();

            if (_destinationAddressProvider != null)
            {
                var destinationAddress = _destinationAddressProvider(consumeContext);

                var endpoint = await consumeContext.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

                await endpoint.Send(message, sendPipe).ConfigureAwait(false);
            }
            else
                await consumeContext.Send(message, sendPipe).ConfigureAwait(false);

            await next.Execute(context).ConfigureAwait(false);
        }

        Task Activity<TInstance, TData>.Faulted<TException>(BehaviorExceptionContext<TInstance, TData, TException> context,
            Behavior<TInstance, TData> next)
        {
            return next.Faulted(context);
        }

        static SendContextCallback<TInstance, TData, TMessage> Convert(Action<SendContext<TMessage>> contextCallback)
        {
            if (contextCallback == null)
                return null;

            return (_, sendContext) => contextCallback(sendContext);
        }
    }
}
