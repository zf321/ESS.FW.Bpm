using System.Collections.Generic;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using NUnit.Framework;

namespace Engine.Tests.Standalone.Entity
{


    /// 
    /// <summary>
    ///  <christopher.zell@camunda.com>
    /// </summary>
    [TestFixture]
    public class ExecutionEntityTest
    {

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.ExpectedException thrown = org.junit.Rules.ExpectedException.None();
        // //public ExpectedException thrown = ExpectedException.None();

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testRestoreProcessInstance()
        [Test]
        public virtual void testRestoreProcessInstance()
        {
            //given parent execution
            IList<ExecutionEntity> entities = new List<ExecutionEntity>();
            ExecutionEntity parent = new ExecutionEntity();
            parent.Id = "parent";
            entities.Add(parent);
            //when restore process instance is called
            parent.RestoreProcessInstance(entities, null, null, null, null, null, null);
            //then no problem should occure

            //when child is added and restore is called again
            ExecutionEntity entity = new ExecutionEntity();
            entity.Id = "child";
            entity.SetParentId(parent.Id);
            entities.Add(entity);

            parent.RestoreProcessInstance(entities, null, null, null, null, null, null);
            //then again no problem should occure

            //when parent is deleted from the list
            entities.Remove(parent);
            //删除父后会异常
            var exc = Assert.Throws<ProcessEngineException>(() => parent.RestoreProcessInstance(entities, null, null, null, null, null, null));
            Assert.IsTrue(exc.Message.Contains("Cannot resolve parent with id 'parent' of execution 'child', perhaps it was deleted in the meantime"));
            //then exception is thrown because child reference to parent which does not exist anymore
            //thrown.Expect(typeof(ProcessEngineException));
            //thrown.ExpectMessage("Cannot resolve parent with id 'parent' of execution 'child', perhaps it was deleted in the meantime");
            //parent.RestoreProcessInstance(entities, null, null, null, null, null, null);
        }
    }

}