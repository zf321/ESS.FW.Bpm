using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Common.Components;

namespace ESS.FW.Bpm.Engine.Impl.Interceptor
{
    /// <summary>
    ///     <para>
    ///         This <seealso cref="CommandContextFactory" /> allows to explicitly pass in a
    ///         <seealso cref="TransactionContextFactory" />.
    ///     </para>
    ///     
    /// </summary>
    public class TxContextCommandContextFactory : CommandContextFactory
    {
        protected internal ITransactionContextFactory transactionContextFactory;

        public virtual ITransactionContextFactory TransactionContextFactory
        {
            get { return transactionContextFactory; }
            set { transactionContextFactory = value; }
        }

        public override CommandContext CreateCommandContext()
        {
            IScope scope = ObjectContainer.BeginLifetimeScope();
            return new CommandContext(processEngineConfiguration, transactionContextFactory, scope);
        }
    }
}