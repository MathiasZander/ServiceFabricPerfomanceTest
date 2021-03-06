namespace MassTransit.Transports.InMemory.Configuration
{
    using MassTransit.Configuration;


    public interface IInMemoryReceiveEndpointConfiguration :
        IReceiveEndpointConfiguration,
        IInMemoryEndpointConfiguration
    {
        IInMemoryReceiveEndpointConfigurator Configurator { get; }

        void Build(IHost host);
    }
}
