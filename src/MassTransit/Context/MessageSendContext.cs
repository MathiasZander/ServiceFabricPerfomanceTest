namespace MassTransit.Context
{
    using System;
    using System.IO;
    using System.Net.Mime;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading;
    using GreenPipes;


    public class MessageSendContext<TMessage> :
        BasePipeContext,
        PublishContext<TMessage>,
        DelaySendContext
        where TMessage : class
    {
        readonly DictionarySendHeaders _headers;
        byte[] _body;
        string _bodyText;

        IMessageSerializer _serializer;

        public MessageSendContext(TMessage message, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            Message = message;

            _headers = new DictionarySendHeaders();

            var messageId = NewId.Next();

            MessageId = messageId.ToGuid();

            SentTime = messageId.Timestamp;

            Durable = true;
        }

        public MessageSendContext(TMessage message)
        {
            Message = message;

            _headers = new DictionarySendHeaders();

            var messageId = NewId.Next();

            MessageId = messageId.ToGuid();

            SentTime = messageId.Timestamp;

            Durable = true;
        }

        public byte[] Body
        {
            get
            {
                if (_body != null)
                    return _body;

                if (Serializer == null)
                    throw new SerializationException("No serializer specified");
                if (Message == null)
                    throw new SendException(typeof(TMessage), DestinationAddress, "No message specified");

                using var memoryStream = new MemoryStream(8192);

                Serializer.Serialize(memoryStream, this);

                _body = memoryStream.ToArray();

                return _body;
            }
        }

        public string BodyText
        {
            get
            {
                if (_bodyText != null)
                    return _bodyText;

                _bodyText = Encoding.UTF8.GetString(Body);

                return _bodyText;
            }
        }

        public long BodyLength => _bodyText?.Length ?? _body?.LongLength ?? throw new InvalidOperationException("The message body has not been serialized");

        /// <summary>
        /// Set to true if the message is being published
        /// </summary>
        public bool IsPublish { get; set; }

        public virtual TimeSpan? Delay { get; set; }

        public Guid? MessageId { get; set; }
        public Guid? RequestId { get; set; }
        public Guid? CorrelationId { get; set; }

        public Guid? ConversationId { get; set; }
        public Guid? InitiatorId { get; set; }

        public Guid? ScheduledMessageId { get; set; }

        public SendHeaders Headers => _headers;

        public Uri SourceAddress { get; set; }
        public Uri DestinationAddress { get; set; }
        public Uri ResponseAddress { get; set; }
        public Uri FaultAddress { get; set; }

        public TimeSpan? TimeToLive { get; set; }
        public DateTime? SentTime { get; private set; }

        public ContentType ContentType { get; set; }

        public IMessageSerializer Serializer
        {
            get => _serializer;
            set
            {
                _serializer = value;
                if (_serializer != null)
                    ContentType = _serializer.ContentType;
            }
        }

        SendContext<T> SendContext.CreateProxy<T>(T message)
        {
            return new SendContextProxy<T>(this, message);
        }

        public bool Durable { get; set; }

        public TMessage Message { get; }

        public bool Mandatory { get; set; }
    }
}
