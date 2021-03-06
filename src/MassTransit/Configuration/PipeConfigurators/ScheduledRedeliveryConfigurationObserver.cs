namespace MassTransit.PipeConfigurators
{
    using System;
    using ConsumeConfigurators;
    using Courier.Contracts;
    using GreenPipes.Configurators;


    public class ScheduledRedeliveryConfigurationObserver :
        ConfigurationObserver,
        IMessageConfigurationObserver
    {
        readonly IConsumePipeConfigurator _configurator;
        readonly Action<IRetryConfigurator> _configure;

        public ScheduledRedeliveryConfigurationObserver(IConsumePipeConfigurator configurator, Action<IRetryConfigurator> configure)
            : base(configurator)
        {
            _configurator = configurator;
            _configure = configure;

            Connect(this);
        }

        public void MessageConfigured<TMessage>(IConsumePipeConfigurator configurator)
            where TMessage : class
        {
            AddRedeliveryPipeSpecification<TMessage>(configurator);

            if (typeof(TMessage) == typeof(RoutingSlip))
                return;

            var retrySpecification = new RedeliveryRetryPipeSpecification<TMessage>();

            _configure?.Invoke(retrySpecification);

            configurator.AddPipeSpecification(retrySpecification);
        }

        public override void BatchConsumerConfigured<TConsumer, TMessage>(IConsumerMessageConfigurator<TConsumer, Batch<TMessage>> configurator)
        {
            MessageConfigured<TMessage>(_configurator);
        }

        public override void ActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator, Uri compensateAddress)
        {
            base.ActivityConfigured(configurator, compensateAddress);

            var specification = new ExecuteContextRedeliveryPipeSpecification<TArguments>();

            _configure?.Invoke(specification);

            configurator.Arguments(x => x.AddPipeSpecification(specification));
        }

        public override void ExecuteActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator)
        {
            base.ExecuteActivityConfigured(configurator);

            var specification = new ExecuteContextRedeliveryPipeSpecification<TArguments>();

            _configure?.Invoke(specification);

            configurator.Arguments(x => x.AddPipeSpecification(specification));
        }

        public override void CompensateActivityConfigured<TActivity, TLog>(ICompensateActivityConfigurator<TActivity, TLog> configurator)
        {
            base.CompensateActivityConfigured(configurator);

            var specification = new CompensateContextRedeliveryPipeSpecification<TLog>();

            _configure?.Invoke(specification);

            configurator.Log(x => x.AddPipeSpecification(specification));
        }

        protected virtual void AddRedeliveryPipeSpecification<TMessage>(IConsumePipeConfigurator configurator)
            where TMessage : class
        {
            var redeliverySpecification = new ScheduledRedeliveryPipeSpecification<TMessage>();

            configurator.AddPipeSpecification(redeliverySpecification);
        }
    }
}
