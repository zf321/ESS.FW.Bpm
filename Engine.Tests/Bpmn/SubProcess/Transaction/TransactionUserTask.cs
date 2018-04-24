using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior;

namespace Engine.Tests.Bpmn.SubProcess.Transaction
{
    public class TransactionUserTask
    {
        protected internal static readonly BpmnBehaviorLogger Log = ProcessEngineLogger.BpmnBehaviorLogger;
        public static int n = 0;
        public void Invoke()
        {
            Log.LogInfo("用户任务", "TransactionUserTask");
            #region 外部数据正确插入
            TestDbContext db = new TestDbContext();
            string id = NextId();
            db.ProcessDefs.Add(new ProcessDefTestEntity() { Id = id });
            int r = db.SaveChanges();
            Log.LogInfo("外部数据写入:", r.ToString() + "条" + " Id:" + id);
            #endregion
        }
        public void SameId()
        {
            Log.LogInfo("用户任务", "TransactionUserTask");
            #region 外部数据可能重复Id
            TestDbContext db = new TestDbContext();
            string id = "SameId";
            db.ProcessDefs.Add(new ProcessDefTestEntity() { Id = id });
            int r = db.SaveChanges();
            Log.LogInfo("外部数据写入:", r.ToString() + "条" + " Id:" + id);
            #endregion
        }
        public void Throw()
        {
            throw new System.Exception("发生未知错误");
        }
        private string NextId()
        {
            n++;
            return n.ToString();
        }
    }
}
