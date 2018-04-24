using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class VariableInTransactionTest : PluggableProcessEngineTestCase
    {
        [Test]
        public virtual void testCreateAndDeleteVariableInTransaction()
        {
            processEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass(this));
        }

        private class CommandAnonymousInnerClass : ICommand<object>
        {
            private readonly VariableInTransactionTest outerInstance;

            public CommandAnonymousInnerClass(VariableInTransactionTest outerInstance)
            {
                this.outerInstance = outerInstance;
            }

            public object Execute(CommandContext commandContext)
            {
                //create a variable
                var variable = VariableInstanceEntity.CreateAndInsert("aVariable",
                    ESS.FW.Bpm.Engine.Variable.Variables.ByteArrayValue(new byte[0]));
                var byteArrayId = variable.ByteArrayId;

                //Delete the variable
                variable.Delete();

                //check if the variable is deleted transient
                //-> no insert and Delete stmt will be flushed
                var dbEntityManager = commandContext.ByteArrayManager;
                var cachedEntity = dbEntityManager.Get( byteArrayId);

                //var entityState = cachedEntity.EntityState;
                //Assert.AreEqual(DbEntityState.DeletedTransient, entityState);

                return null;
            }
        }
    }
}