using System.Collections.Generic;
using Engine.Tests.History;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;
//using ESS.FW.Bpm.Engine.Tests.History;

namespace Engine.Tests.Api.Variables
{


    //using PluggableProcessEngineTestCase = PluggableProcessEngineTestCase;
    //using IProcessInstance = Engine.Runtime.IProcessInstance;
    //using RemoveAndUpdateValueDelegate = RemoveAndUpdateValueDelegate;
    //using ReplaceAndUpdateValueDelegate = ReplaceAndUpdateValueDelegate;
    //using UpdateValueDelegate = UpdateValueDelegate;

    /// <summary>
    /// 
    /// </summary>
    public class ImplicitVariableUpdateTest : PluggableProcessEngineTestCase
    {

        [Test]
        [Deployment("resources/api/variables/ImplicitVariableUpdateTest.sequence.bpmn20.xml")]
        public virtual void testUpdate()
        {
            IProcessInstance instance = runtimeService.StartProcessInstanceByKey("serviceTaskProcess", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables().PutValue("listVar", new List<string>()).PutValue("delegate", new UpdateValueDelegate()));

            IList<string> list = (IList<string>)runtimeService.GetVariable(instance.Id, "listVar");
            Assert.NotNull(list);
            Assert.AreEqual(1, list.Count);
            //Assert.AreEqual(UpdateValueDelegate.NEW_ELEMENT, list[0]);
        }


        [Test]
        [Deployment("resources/api/variables/ImplicitVariableUpdateTest.sequence.bpmn20.xml")]
        public virtual void testUpdateParallelFlow()
        {
            // should also work when execution tree is expanded between the implicit update
            // and when the engine notices it

            IProcessInstance instance = runtimeService.StartProcessInstanceByKey("serviceTaskProcess", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables().PutValue("listVar", new List<string>()).PutValue("delegate", new UpdateValueDelegate()));

            IList<string> list = (IList<string>)runtimeService.GetVariable(instance.Id, "listVar");
            Assert.NotNull(list);
            Assert.AreEqual(1, list.Count);
            //Assert.AreEqual(UpdateValueDelegate.NEW_ELEMENT, list[0]);
        }


        [Test]
        [Deployment("resources/api/variables/ImplicitVariableUpdateTest.sequence.bpmn20.xml")]
        public virtual void testUpdatePreviousValue()
        {
            IProcessInstance instance = runtimeService.StartProcessInstanceByKey("serviceTaskProcess", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables().PutValue("listVar", new List<string>()).PutValue("delegate", new ReplaceAndUpdateValueDelegate()));

            IList<string> list = (IList<string>)runtimeService.GetVariable(instance.Id, "listVar");
            Assert.NotNull(list);
            Assert.True(list.Count == 0);
        }


        [Test]
        [Deployment("resources/api/variables/ImplicitVariableUpdateTest.sequence.bpmn20.xml")]
        public virtual void testRemoveAndUpdateValue()
        {
            IProcessInstance instance = runtimeService.StartProcessInstanceByKey("serviceTaskProcess", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables().PutValue("listVar", new List<string>()).PutValue("delegate", new RemoveAndUpdateValueDelegate()));

            object variableValue = runtimeService.GetVariable(instance.Id, "listVar");
            Assert.IsNull(variableValue);
        }
    }

}