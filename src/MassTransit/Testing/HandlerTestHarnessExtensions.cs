namespace MassTransit.Testing
{
    using Util;


    public static class HandlerTestHarnessExtensions
    {
        public static HandlerTestHarness<T> Handler<T>(this BusTestHarness harness, MessageHandler<T> handler)
            where T : class
        {
            return new HandlerTestHarness<T>(harness, handler);
        }

        public static HandlerTestHarness<T> Handler<T>(this BusTestHarness harness)
            where T : class
        {
            return new HandlerTestHarness<T>(harness, context => TaskUtil.Completed);
        }
    }
}
