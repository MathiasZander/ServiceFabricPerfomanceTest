namespace MassTransit.ActiveMqTransport.Topology
{
    using MassTransit.Topology;


    public interface IActiveMqConsumeTopologyConfigurator :
        IConsumeTopologyConfigurator,
        IActiveMqConsumeTopology
    {
        new IActiveMqConsumerEndpointQueueNameFormatter ConsumerEndpointQueueNameFormatter { set; }

        new IActiveMqTemporaryQueueNameFormatter TemporaryQueueNameFormatter { set; }

        new IActiveMqMessageConsumeTopologyConfigurator<T> GetMessageTopology<T>()
            where T : class;

        void AddSpecification(IActiveMqConsumeTopologySpecification specification);
    }
}
