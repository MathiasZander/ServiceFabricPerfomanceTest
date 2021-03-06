namespace MassTransit.Transports.InMemory.Contexts
{
    using System.IO;
    using Context;
    using Fabric;


    public sealed class InMemoryReceiveContext :
        BaseReceiveContext
    {
        readonly byte[] _body;
        readonly InMemoryTransportMessage _message;

        public InMemoryReceiveContext(InMemoryTransportMessage message, InMemoryReceiveEndpointContext receiveEndpointContext)
            : base(message.DeliveryCount > 0, receiveEndpointContext)
        {
            _body = message.Body;
            _message = message;
        }

        protected override IHeaderProvider HeaderProvider => new DictionaryHeaderProvider(_message.Headers);

        public override byte[] GetBody()
        {
            return _body;
        }

        public override Stream GetBodyStream()
        {
            return new MemoryStream(_body, 0, _body.Length, false);
        }
    }
}
