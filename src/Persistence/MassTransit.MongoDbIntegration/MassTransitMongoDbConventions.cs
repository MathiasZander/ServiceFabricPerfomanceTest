namespace MassTransit.MongoDbIntegration
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using Audit;
    using Courier;
    using Courier.Documents;
    using Courier.Events;
    using MassTransit.Saga;
    using MongoDB.Bson.Serialization;
    using MongoDB.Bson.Serialization.Conventions;


    public class MassTransitMongoDbConventions
    {
        public MassTransitMongoDbConventions(ConventionFilter filter = default)
        {
            var conventionFilter = filter ?? IsMassTransitClass;

            var convention = new ConventionPack
            {
                new CamelCaseElementNameConvention(),
                new IgnoreExtraElementsConvention(true),
                new SagaConvention(),
                new MemberDefaultValueConvention(typeof(Guid), Guid.Empty)
            };

            ConventionRegistry.Register("MassTransitConventions", convention, type => conventionFilter(type));

            RegisterClass<RoutingSlipDocument>(x => x.TrackingNumber);
            RegisterClass<ExceptionInfoDocument>();
            RegisterClass<ActivityExceptionDocument>();
            RegisterClass<HostDocument>();
            RegisterClass<RoutingSlipEventDocument>();
            RegisterClass<RoutingSlipActivityCompensatedDocument>();
            RegisterClass<RoutingSlipActivityCompensationFailedDocument>();
            RegisterClass<RoutingSlipActivityCompletedDocument>();
            RegisterClass<RoutingSlipActivityFaultedDocument>();
            RegisterClass<RoutingSlipCompensationFailedDocument>();
            RegisterClass<RoutingSlipCompletedDocument>();
            RegisterClass<RoutingSlipFaultedDocument>();
            RegisterClass<RoutingSlipRevisedDocument>();
            RegisterClass<RoutingSlipTerminatedDocument>();
        }

        static bool IsMassTransitClass(Type type)
        {
            return type.FullName.StartsWith("MassTransit") || IsSagaClass(type) && type != typeof(AuditDocument);
        }

        static bool IsSagaClass(Type type)
        {
            return type.GetTypeInfo().IsClass && typeof(ISagaVersion).IsAssignableFrom(type);
        }

        public static void RegisterClass<T>(Expression<Func<T, Guid>> id)
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(T)))
                return;

            BsonClassMap.RegisterClassMap<T>(x =>
            {
                x.AutoMap();
                x.SetIdMember(x.GetMemberMap(id));
            });
        }

        public static void RegisterClass<T>()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(T)))
                return;

            BsonClassMap.RegisterClassMap<T>(x =>
            {
                x.AutoMap();
                x.SetDiscriminatorIsRequired(true);

                var typeName = typeof(T).Name;
                if (typeName.EndsWith("Document"))
                    x.SetDiscriminator(typeName.Substring(0, typeName.Length - "Document".Length));
            });
        }
    }
}
