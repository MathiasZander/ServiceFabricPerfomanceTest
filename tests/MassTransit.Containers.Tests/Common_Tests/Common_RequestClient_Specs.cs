namespace MassTransit.Containers.Tests.Common_Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;


    public abstract class Common_RequestClient_Context :
        InMemoryTestFixture
    {
        Guid _correlationId;

        public Common_RequestClient_Context()
        {
            SubsequentQueueName = "subsequent_queue";
            SubsequentQueueAddress = new Uri(BaseAddress, SubsequentQueueName);
        }

        protected Uri SubsequentQueueAddress { get; }
        string SubsequentQueueName { get; }

        protected abstract IRequestClient<InitialRequest> RequestClient { get; }

        protected abstract IBusRegistrationContext Registration { get; }

        [Test]
        public async Task Should_receive_the_response()
        {
            IRequestClient<InitialRequest> client = RequestClient;

            _correlationId = NewId.NextGuid();

            Response<InitialResponse> response = await client.GetResponse<InitialResponse>(new
            {
                CorrelationId = _correlationId,
                Value = "World"
            });

            Assert.That(response.Message.Value, Is.EqualTo("Hello, World"));
            Assert.That(response.ConversationId.Value, Is.EqualTo(response.Message.OriginalConversationId));
            Assert.That(response.InitiatorId.Value, Is.EqualTo(_correlationId));
            Assert.That(response.Message.OriginalInitiatorId, Is.EqualTo(_correlationId));
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint(SubsequentQueueName, cfg => cfg.ConfigureConsumer<SubsequentConsumer>(Registration));
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumer<InitialConsumer>(Registration);
        }

        protected void ConfigureRegistration(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<InitialConsumer>();
            configurator.AddConsumer<SubsequentConsumer>();

            configurator.AddBus(context => BusControl);

            configurator.AddRequestClient<InitialRequest>(InputQueueAddress);
            configurator.AddRequestClient(typeof(SubsequentRequest), SubsequentQueueAddress);
        }


        protected class InitialConsumer :
            IConsumer<InitialRequest>
        {
            readonly IRequestClient<SubsequentRequest> _client;

            public InitialConsumer(IRequestClient<SubsequentRequest> client)
            {
                _client = client;
            }

            public async Task Consume(ConsumeContext<InitialRequest> context)
            {
                Response<SubsequentResponse> response = await _client.GetResponse<SubsequentResponse>(context.Message);

                await context.RespondAsync<InitialResponse>(response.Message);
            }
        }


        protected class SubsequentConsumer :
            IConsumer<SubsequentRequest>
        {
            public Task Consume(ConsumeContext<SubsequentRequest> context)
            {
                return context.RespondAsync<SubsequentResponse>(new
                {
                    OriginalConversationId = context.ConversationId,
                    OriginalInitiatorId = context.InitiatorId,
                    Value = $"Hello, {context.Message.Value}"
                });
            }
        }


        public interface InitialRequest
        {
            Guid CorrelationId { get; }
            string Value { get; }
        }


        public interface InitialResponse
        {
            Guid OriginalConversationId { get; }
            Guid OriginalInitiatorId { get; }
            string Value { get; }
        }


        public interface SubsequentRequest
        {
            Guid CorrelationId { get; }
            string Value { get; }
        }


        public interface SubsequentResponse
        {
            Guid OriginalConversationId { get; }
            Guid OriginalInitiatorId { get; }
            string Value { get; }
        }
    }


    public abstract class Common_RequestClient_Generic :
        InMemoryTestFixture
    {
        Guid _correlationId;

        protected abstract IRequestClient<InitialRequest> RequestClient { get; }

        protected abstract IBusRegistrationContext Registration { get; }

        [Test]
        public async Task Should_receive_the_response()
        {
            IRequestClient<InitialRequest> client = RequestClient;

            _correlationId = NewId.NextGuid();

            Response<InitialResponse> response = await client.GetResponse<InitialResponse>(new
            {
                CorrelationId = _correlationId,
                Value = "World"
            });

            Assert.That(response.Message.Value, Is.EqualTo("Hello, World"));
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ConfigureEndpoints(Registration);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumer<InitialConsumer>(Registration);
        }

        protected void ConfigureRegistration(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<InitialConsumer>();
            configurator.AddConsumer<SubsequentConsumer>();

            configurator.AddBus(context => BusControl);
        }


        protected class InitialConsumer :
            IConsumer<InitialRequest>
        {
            readonly IRequestClient<SubsequentRequest> _client;

            public InitialConsumer(IRequestClient<SubsequentRequest> client)
            {
                _client = client;
            }

            public async Task Consume(ConsumeContext<InitialRequest> context)
            {
                Response<SubsequentResponse> response = await _client.GetResponse<SubsequentResponse>(context.Message);

                await context.RespondAsync<InitialResponse>(response.Message);
            }
        }


        protected class SubsequentConsumer :
            IConsumer<SubsequentRequest>
        {
            public Task Consume(ConsumeContext<SubsequentRequest> context)
            {
                return context.RespondAsync<SubsequentResponse>(new { Value = $"Hello, {context.Message.Value}" });
            }
        }


        public interface InitialRequest
        {
            string Value { get; }
        }


        public interface InitialResponse
        {
            string Value { get; }
        }


        public interface SubsequentRequest
        {
            string Value { get; }
        }


        public interface SubsequentResponse
        {
            string Value { get; }
        }
    }


    public abstract class Common_ScopedClientFactory :
        InMemoryTestFixture
    {
        Guid _correlationId;

        protected abstract IRequestClient<InitialRequest> RequestClient { get; }

        protected abstract IBusRegistrationContext Registration { get; }

        [Test]
        public async Task Should_receive_the_response()
        {
            IRequestClient<InitialRequest> client = RequestClient;

            _correlationId = NewId.NextGuid();

            Response<InitialResponse> response = await client.GetResponse<InitialResponse>(new
            {
                CorrelationId = _correlationId,
                Value = "World"
            });

            Assert.That(response.Message.Value, Is.EqualTo("Hello, World"));
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ConfigureEndpoints(Registration);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumer<InitialConsumer>(Registration);
        }

        protected void ConfigureRegistration(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<InitialConsumer>();
            configurator.AddConsumer<SubsequentConsumer>();
            configurator.AddRequestClient<InitialRequest>();

            configurator.AddBus(context => BusControl);
        }


        protected class InitialConsumer :
            IConsumer<InitialRequest>
        {
            readonly IRequestClient<SubsequentRequest> _client;

            public InitialConsumer(IScopedClientFactory clientFactory)
            {
                _client = clientFactory.CreateRequestClient<SubsequentRequest>();
            }

            public async Task Consume(ConsumeContext<InitialRequest> context)
            {
                Response<SubsequentResponse> response = await _client.GetResponse<SubsequentResponse>(context.Message);

                await context.RespondAsync<InitialResponse>(response.Message);
            }
        }


        protected class SubsequentConsumer :
            IConsumer<SubsequentRequest>
        {
            public Task Consume(ConsumeContext<SubsequentRequest> context)
            {
                return context.RespondAsync<SubsequentResponse>(new { Value = $"Hello, {context.Message.Value}" });
            }
        }


        public interface InitialRequest
        {
            string Value { get; }
        }


        public interface InitialResponse
        {
            string Value { get; }
        }


        public interface SubsequentRequest
        {
            string Value { get; }
        }


        public interface SubsequentResponse
        {
            string Value { get; }
        }
    }
}
