namespace MassTransit.RabbitMqTransport.Topology.Configurators
{
    using System;
    using System.Collections.Generic;
    using Entities;


    public class ExchangeBindingConfigurator :
        ExchangeConfigurator,
        IExchangeBindingConfigurator
    {
        public ExchangeBindingConfigurator(string exchangeName, string exchangeType, bool durable = true, bool autoDelete = false, string routingKey = null)
            : base(exchangeName, exchangeType, durable, autoDelete)
        {
            RoutingKey = routingKey ?? "";

            BindingArguments = new Dictionary<string, object>();
        }

        public ExchangeBindingConfigurator(Exchange exchange, string routingKey = null)
            : base(exchange)
        {
            RoutingKey = routingKey ?? "";

            BindingArguments = new Dictionary<string, object>();
        }

        public IDictionary<string, object> BindingArguments { get; }

        public void SetBindingArgument(string key, object value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (value == null)
                BindingArguments.Remove(key);
            else
                BindingArguments[key] = value;
        }

        public string RoutingKey { get; set; }
    }
}
