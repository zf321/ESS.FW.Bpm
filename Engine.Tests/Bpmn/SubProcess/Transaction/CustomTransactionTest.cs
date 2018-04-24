using System.Linq;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.SubProcess.Transaction
{
    [TestFixture]
    public class CustomTransactionTest : PluggableProcessEngineTestCase
    {
        [Test]
        [Deployment]
        public void TestTransationServiceTaskSuccess()
        {
            //没有异常
            try
            {
                var processInstance = runtimeService.StartProcessInstanceByKey("transactionProcess");
                Assert.AreEqual(2, runtimeService.GetDbContext().Set<ProcessDefinitionEntity>().Where(m => m.Key == "TransationTest").Count());
                Assert.AreEqual(1, runtimeService.GetDbContext().Set<HistoricActivityInstanceEventEntity>().Where(m => m.ActivityId == "task_1").Count());
                Assert.AreEqual(1, runtimeService.GetDbContext().Set<HistoricActivityInstanceEventEntity>().Where(m => m.ActivityId == "task_2").Count());
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                CleanCustomProcessDbForKey("TransationTest");
            }
        }
        [Test]
        [Deployment]
        public void TestTransationServiceTaskError()
        {
            //第二个activity异常
            try
            {
                var processInstance = runtimeService.StartProcessInstanceByKey("transactionProcess");
                Assert.AreEqual(0, runtimeService.GetDbContext().Set<ProcessDefinitionEntity>().Where(m => m.Key == "TransationTest").Count());
                Assert.AreEqual(1, runtimeService.GetDbContext().Set<HistoricActivityInstanceEventEntity>().Where(m => m.ActivityId == "task_1").Count());
                Assert.AreEqual(1, runtimeService.GetDbContext().Set<HistoricActivityInstanceEventEntity>().Where(m => m.ActivityId == "task_2").Count());
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                CleanCustomProcessDbForKey("TransationTest");
            }
        }
        [Test]
        [Deployment("resources\\Bpmn\\SubProcess\\Transaction\\CustomTransactionTest.TestTransationServiceTaskThrow.bpmn20.xml")]
        public void TestTransationServiceTaskThrow()
        {
            //两个都异常，第二个Activity不会执行，只纪录第一个Activity历史信息
            try
            {
                var processInstance = runtimeService.StartProcessInstanceByKey("transactionProcess");
                Assert.AreEqual(0, runtimeService.GetDbContext().Set<ProcessDefinitionEntity>().Where(m => m.Key == "TransationTest").Count());
                Assert.AreEqual(1, runtimeService.GetDbContext().Set<HistoricActivityInstanceEventEntity>().Where(m => m.ActivityId == "task_1").Count());
                Assert.AreEqual(0, runtimeService.GetDbContext().Set<HistoricActivityInstanceEventEntity>().Where(m => m.ActivityId == "task_2").Count());
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                CleanCustomProcessDbForKey("TransationTest");
            }
        }
        private void CleanCustomProcessDbForKey(string key)
        {
            using (TestDbContext db = new TestDbContext())
            {
                var toDel = db.ProcessDefs.Where(m => m.Key == key).ToList();
                db.ProcessDefs.RemoveRange(toDel);
                db.SaveChanges();
            }
        }
    }
}
