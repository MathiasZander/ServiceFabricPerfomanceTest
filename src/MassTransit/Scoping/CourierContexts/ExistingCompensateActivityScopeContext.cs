namespace MassTransit.Scoping.CourierContexts
{
    using System;
    using System.Threading.Tasks;
    using Courier;


    public class ExistingCompensateActivityScopeContext<TActivity, TLog> :
        ICompensateActivityScopeContext<TActivity, TLog>
        where TActivity : class, ICompensateActivity<TLog>
        where TLog : class
    {
        readonly Action<TActivity> _disposeCallback;

        public ExistingCompensateActivityScopeContext(CompensateActivityContext<TActivity, TLog> context, Action<TActivity> disposeCallback = null)
        {
            _disposeCallback = disposeCallback;
            Context = context;
        }

        public CompensateActivityContext<TActivity, TLog> Context { get; }

        public ValueTask DisposeAsync()
        {
            _disposeCallback?.Invoke(Context.Activity);

            return default;
        }
    }
}
