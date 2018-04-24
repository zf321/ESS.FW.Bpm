//using System;
//using System.Collections.Generic;
//using ESS.FW.Bpm.Engine.Runtime;
//using ESS.FW.Bpm.Engine.Variable;
//using NUnit.Framework;

//namespace ESS.FW.Bpm.Engine.Tests.Bpmn.Callactivity
//{


//    /// <summary>
//    /// 
//    /// 
//    /// </summary>
//    public class CaseCallActivityTest : CmmnProcessEngineTestCase
//    {

//        protected internal readonly string PROCESS_DEFINITION_KEY = "process";
//        protected internal readonly string ONE_TASK_CASE = "oneTaskCase";
//        protected internal readonly string CALL_ACTIVITY_ID = "callActivity";
//        protected internal readonly string USER_TASK_ID = "userTask";
//        protected internal readonly string HUMAN_TASK_ID = "PI_HumanTask_1";

//        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testCallCaseAsConstant.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" }) public void testCallCaseAsConstant()
//        public virtual void testCallCaseAsConstant()
//        {
//            // given
//            // a deployed process definition and case definition

//            // when
//            string superProcessInstanceId = StartProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;

//            // then
//            string callActivityId = queryExecutionByActivityId(CALL_ACTIVITY_ID).Id;

//            CaseExecutionEntity subCaseInstance = (CaseExecutionEntity)queryOneTaskCaseInstance();
//            Assert.NotNull(subCaseInstance);

//            Assert.AreEqual(callActivityId, subCaseInstance.SuperExecutionId);

//            // complete
//            string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;

//            complete(humanTaskId);
//            close(subCaseInstance.Id);
//            assertProcessEnded(superProcessInstanceId);
//        }

//        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testCallCaseAsExpressionStartsWithDollar.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" }) public void testCallCaseAsExpressionStartsWithDollar()
//        public virtual void testCallCaseAsExpressionStartsWithDollar()
//        {
//            // given
//            // a deployed process definition and case definition

//            // when
//            string superProcessInstanceId = StartProcessInstanceByKey(PROCESS_DEFINITION_KEY, Variables.createVariables().putValue(ONE_TASK_CASE, ONE_TASK_CASE)).Id;

//            // then
//            string callActivityId = queryExecutionByActivityId(CALL_ACTIVITY_ID).Id;

//            CaseExecutionEntity subCaseInstance = (CaseExecutionEntity)queryOneTaskCaseInstance();
//            Assert.NotNull(subCaseInstance);

//            Assert.AreEqual(callActivityId, subCaseInstance.SuperExecutionId);

//            // complete
//            string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;

//            complete(humanTaskId);
//            close(subCaseInstance.Id);
//            assertProcessEnded(superProcessInstanceId);
//        }

//        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testCallCaseAsExpressionStartsWithHash.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" }) public void testCallCaseAsExpressionStartsWithHash()
//        public virtual void testCallCaseAsExpressionStartsWithHash()
//        {
//            // given
//            // a deployed process definition and case definition

//            // when
//            string superProcessInstanceId = StartProcessInstanceByKey(PROCESS_DEFINITION_KEY, Variables.createVariables().putValue(ONE_TASK_CASE, ONE_TASK_CASE)).Id;

//            // then
//            string callActivityId = queryExecutionByActivityId(CALL_ACTIVITY_ID).Id;

//            CaseExecutionEntity subCaseInstance = (CaseExecutionEntity)queryOneTaskCaseInstance();
//            Assert.NotNull(subCaseInstance);

//            Assert.AreEqual(callActivityId, subCaseInstance.SuperExecutionId);

//            // complete
//            string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;

//            complete(humanTaskId);
//            close(subCaseInstance.Id);
//            assertProcessEnded(superProcessInstanceId);
//        }

//        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testCallCaseWithCompositeExpression.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" }) public void testCallCaseWithCompositeExpression()
//        public virtual void testCallCaseWithCompositeExpression()
//        {
//            // given
//            // a deployed process definition and case definition

//            // when
//            string superProcessInstanceId = StartProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;

//            // then
//            string callActivityId = queryExecutionByActivityId(CALL_ACTIVITY_ID).Id;

//            CaseExecutionEntity subCaseInstance = (CaseExecutionEntity)queryOneTaskCaseInstance();
//            Assert.NotNull(subCaseInstance);

//            Assert.AreEqual(callActivityId, subCaseInstance.SuperExecutionId);

//            // complete
//            string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;

//            complete(humanTaskId);
//            close(subCaseInstance.Id);
//            assertProcessEnded(superProcessInstanceId);
//        }

//        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testCallLatestCase.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" }) public void testCallLatestCase()
//        public virtual void testCallLatestCase()
//        {
//            // given
//            string cmmnResourceName = "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn";

//            string deploymentId = repositoryService.CreateDeployment().AddClasspathResource(cmmnResourceName).Deploy().Id;

//            Assert.AreEqual(2, repositoryService.CreateCaseDefinitionQuery().Count());

//            string latestCaseDefinitionId = repositoryService.CreateCaseDefinitionQuery().CaseDefinitionKey(ONE_TASK_CASE)/*.LatestVersion()*/.First().Id;

//            // when
//            string superProcessInstanceId = StartProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;

//            // then
//            string callActivityId = queryExecutionByActivityId(CALL_ACTIVITY_ID).Id;

//            CaseExecutionEntity subCaseInstance = (CaseExecutionEntity)queryOneTaskCaseInstance();
//            Assert.NotNull(subCaseInstance);

//            Assert.AreEqual(callActivityId, subCaseInstance.SuperExecutionId);
//            Assert.AreEqual(latestCaseDefinitionId, subCaseInstance.CaseDefinitionId);

//            // complete ////////////////////////////////////////////////////////
//            string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;

//            complete(humanTaskId);
//            close(subCaseInstance.Id);
//            assertProcessEnded(superProcessInstanceId);

//            repositoryService.DeleteDeployment(deploymentId, true);
//        }

//        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testCallCaseByDeployment.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" }) public void testCallCaseByDeployment()
//        public virtual void testCallCaseByDeployment()
//        {
//            // given

//            string firstDeploymentId = repositoryService.CreateDeploymentQuery().First().Id;

//            string cmmnResourceName = "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn";
//            string deploymentId = repositoryService.CreateDeployment().AddClasspathResource(cmmnResourceName).Deploy().Id;

//            Assert.AreEqual(2, repositoryService.CreateCaseDefinitionQuery().Count());

//            string caseDefinitionIdInSameDeployment = repositoryService.CreateCaseDefinitionQuery().CaseDefinitionKey(ONE_TASK_CASE).deploymentId(firstDeploymentId).First().Id;

//            // when
//            string superProcessInstanceId = StartProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;

//            // then
//            string callActivityId = queryExecutionByActivityId(CALL_ACTIVITY_ID).Id;

//            CaseExecutionEntity subCaseInstance = (CaseExecutionEntity)queryOneTaskCaseInstance();
//            Assert.NotNull(subCaseInstance);

//            Assert.AreEqual(callActivityId, subCaseInstance.SuperExecutionId);
//            Assert.AreEqual(caseDefinitionIdInSameDeployment, subCaseInstance.CaseDefinitionId);

//            // complete ////////////////////////////////////////////////////////
//            string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;

//            complete(humanTaskId);
//            close(subCaseInstance.Id);
//            assertProcessEnded(superProcessInstanceId);

//            repositoryService.DeleteDeployment(deploymentId, true);
//        }

//        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testCallCaseByVersion.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" }) public void testCallCaseByVersion()
//        public virtual void testCallCaseByVersion()
//        {
//            // given

//            string cmmnResourceName = "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn";
//            string secondDeploymentId = repositoryService.CreateDeployment().AddClasspathResource(cmmnResourceName).Deploy().Id;

//            string thirdDeploymentId = repositoryService.CreateDeployment().AddClasspathResource(cmmnResourceName).Deploy().Id;

//            Assert.AreEqual(3, repositoryService.CreateCaseDefinitionQuery().Count());

//            string caseDefinitionIdInSecondDeployment = repositoryService.CreateCaseDefinitionQuery().CaseDefinitionKey(ONE_TASK_CASE).deploymentId(secondDeploymentId).First().Id;

//            // when
//            string superProcessInstanceId = StartProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;

//            // then
//            string callActivityId = queryExecutionByActivityId(CALL_ACTIVITY_ID).Id;

//            CaseExecutionEntity subCaseInstance = (CaseExecutionEntity)queryOneTaskCaseInstance();
//            Assert.NotNull(subCaseInstance);

//            Assert.AreEqual(callActivityId, subCaseInstance.SuperExecutionId);
//            Assert.AreEqual(caseDefinitionIdInSecondDeployment, subCaseInstance.CaseDefinitionId);

//            // complete ////////////////////////////////////////////////////////
//            string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;

//            complete(humanTaskId);
//            close(subCaseInstance.Id);
//            assertProcessEnded(superProcessInstanceId);

//            repositoryService.DeleteDeployment(secondDeploymentId, true);
//            repositoryService.DeleteDeployment(thirdDeploymentId, true);
//        }

//        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testCallCaseByVersionAsExpression.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" }) public void testCallCaseByVersionAsExpression()
//        public virtual void testCallCaseByVersionAsExpression()
//        {
//            // given

//            string cmmnResourceName = "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn";

//            string secondDeploymentId = repositoryService.CreateDeployment().AddClasspathResource(cmmnResourceName).Deploy().Id;

//            string thirdDeploymentId = repositoryService.CreateDeployment().AddClasspathResource(cmmnResourceName).Deploy().Id;

//            Assert.AreEqual(3, repositoryService.CreateCaseDefinitionQuery().Count());

//            string caseDefinitionIdInSecondDeployment = repositoryService.CreateCaseDefinitionQuery().CaseDefinitionKey(ONE_TASK_CASE).deploymentId(secondDeploymentId).First().Id;

//            VariableMap variables = Variables.createVariables().putValue("myVersion", 2);

//            // when
//            string superProcessInstanceId = StartProcessInstanceByKey(PROCESS_DEFINITION_KEY, variables).Id;

//            // then
//            CaseExecutionEntity subCaseInstance = (CaseExecutionEntity)queryOneTaskCaseInstance();
//            Assert.NotNull(subCaseInstance);

//            Assert.AreEqual(caseDefinitionIdInSecondDeployment, subCaseInstance.CaseDefinitionId);

//            // complete ////////////////////////////////////////////////////////
//            string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;

//            complete(humanTaskId);
//            close(subCaseInstance.Id);
//            assertProcessEnded(superProcessInstanceId);

//            repositoryService.DeleteDeployment(secondDeploymentId, true);
//            repositoryService.DeleteDeployment(thirdDeploymentId, true);
//        }

//        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testCallCaseAsConstant.bpmn20.xml" }) public void testCaseNotFound()
//        public virtual void testCaseNotFound()
//        {
//            // given

//            try
//            {
//                // when
//                StartProcessInstanceByKey(PROCESS_DEFINITION_KEY);
//                Assert.Fail("It should not be possible to start a not existing case instance.");
//            }
//            catch (CaseDefinitionNotFoundException)
//            {
//            }
//        }

//        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testInputBusinessKey.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" }) public void testInputBusinessKey()
//        public virtual void testInputBusinessKey()
//        {
//            // given
//            string businessKey = "myBusinessKey";

//            // when
//            string superProcessInstanceId = StartProcessInstanceByKey(PROCESS_DEFINITION_KEY, null, businessKey).Id;

//            // then
//            string callActivityId = queryExecutionByActivityId(CALL_ACTIVITY_ID).Id;

//            CaseExecutionEntity subCaseInstance = (CaseExecutionEntity)queryOneTaskCaseInstance();
//            Assert.NotNull(subCaseInstance);

//            Assert.AreEqual(callActivityId, subCaseInstance.SuperExecutionId);
//            Assert.AreEqual(businessKey, subCaseInstance.BusinessKey);

//            // complete ////////////////////////////////////////////////////////
//            string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;

//            complete(humanTaskId);
//            close(subCaseInstance.Id);
//            assertProcessEnded(superProcessInstanceId);
//        }

//        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testInputDifferentBusinessKey.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" }) public void testInputDifferentBusinessKey()
//        public virtual void testInputDifferentBusinessKey()
//        {
//            // given
//            string myBusinessKey = "myBusinessKey";
//            string myOwnBusinessKey = "myOwnBusinessKey";

//            VariableMap variables = Variables.createVariables().putValue(myOwnBusinessKey, myOwnBusinessKey);

//            // when
//            string superProcessInstanceId = StartProcessInstanceByKey(PROCESS_DEFINITION_KEY, variables, myBusinessKey).Id;

//            // then
//            string callActivityId = queryExecutionByActivityId(CALL_ACTIVITY_ID).Id;

//            CaseExecutionEntity subCaseInstance = (CaseExecutionEntity)queryOneTaskCaseInstance();
//            Assert.NotNull(subCaseInstance);

//            Assert.AreEqual(callActivityId, subCaseInstance.SuperExecutionId);
//            Assert.AreEqual(myOwnBusinessKey, subCaseInstance.BusinessKey);

//            // complete ////////////////////////////////////////////////////////
//            string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;

//            complete(humanTaskId);
//            close(subCaseInstance.Id);
//            assertProcessEnded(superProcessInstanceId);
//        }

//        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testInputSource.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" }) public void testInputSource()
//        public virtual void testInputSource()
//        {
//            // given

//            VariableMap parameters = Variables.createVariables().putValue("aVariable", "abc").putValue("anotherVariable", 999).putValue("aThirdVariable", "def");

//            // when
//            string superProcessInstanceId = StartProcessInstanceByKey(PROCESS_DEFINITION_KEY, parameters).Id;

//            // then
//            CaseExecutionEntity subCaseInstance = (CaseExecutionEntity)queryOneTaskCaseInstance();
//            Assert.NotNull(subCaseInstance);

//            IList<VariableInstance> variables = runtimeService.CreateVariableInstanceQuery().caseInstanceIdIn(subCaseInstance.Id);

//            Assert.IsFalse(variables.Count == 0);
//            Assert.AreEqual(2, variables.Count);

//            foreach (VariableInstance variable in variables)
//            {
//                string name = variable.Name;
//                if ("aVariable".Equals(name))
//                {
//                    Assert.AreEqual("aVariable", name);
//                    Assert.AreEqual("abc", variable.Value);

//                }
//                else if ("anotherVariable".Equals(name))
//                {
//                    Assert.AreEqual("anotherVariable", name);
//                    Assert.AreEqual(999, variable.Value);

//                }
//                else
//                {
//                    Assert.Fail("Found an unexpected variable: '" + name + "'");
//                }
//            }

//            // complete ////////////////////////////////////////////////////////
//            string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;

//            complete(humanTaskId);
//            close(subCaseInstance.Id);
//            assertProcessEnded(superProcessInstanceId);
//        }

//        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testInputSourceDifferentTarget.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" }) public void testInputSourceDifferentTarget()
//        public virtual void testInputSourceDifferentTarget()
//        {
//            // given

//            VariableMap parameters = Variables.createVariables().putValue("aVariable", "abc").putValue("anotherVariable", 999).putValue("aThirdVariable", "def");

//            // when
//            string superProcessInstanceId = StartProcessInstanceByKey(PROCESS_DEFINITION_KEY, parameters).Id;

//            // then
//            CaseExecutionEntity subCaseInstance = (CaseExecutionEntity)queryOneTaskCaseInstance();
//            Assert.NotNull(subCaseInstance);

//            IList<VariableInstance> variables = runtimeService.CreateVariableInstanceQuery().caseInstanceIdIn(subCaseInstance.Id);

//            Assert.IsFalse(variables.Count == 0);
//            Assert.AreEqual(2, variables.Count);

//            foreach (VariableInstance variable in variables)
//            {
//                string name = variable.Name;
//                if ("myVariable".Equals(name))
//                {
//                    Assert.AreEqual("myVariable", name);
//                    Assert.AreEqual("abc", variable.Value);
//                }
//                else if ("myAnotherVariable".Equals(name))
//                {
//                    Assert.AreEqual("myAnotherVariable", name);
//                    Assert.AreEqual(999, variable.Value);
//                }
//                else
//                {
//                    Assert.Fail("Found an unexpected variable: '" + name + "'");
//                }
//            }

//            // complete ////////////////////////////////////////////////////////
//            string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;

//            complete(humanTaskId);
//            close(subCaseInstance.Id);
//            assertProcessEnded(superProcessInstanceId);

//        }

//        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testInputSource.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" }) public void testInputSourceNullValue()
//        public virtual void testInputSourceNullValue()
//        {
//            // given

//            // when
//            string superProcessInstanceId = StartProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;

//            // then
//            CaseExecutionEntity subCaseInstance = (CaseExecutionEntity)queryOneTaskCaseInstance();
//            Assert.NotNull(subCaseInstance);

//            IList<VariableInstance> variables = runtimeService.CreateVariableInstanceQuery().caseInstanceIdIn(subCaseInstance.Id);

//            Assert.IsFalse(variables.Count == 0);
//            Assert.AreEqual(2, variables.Count);

//            foreach (VariableInstance variable in variables)
//            {
//                string name = variable.Name;

//                if ("aVariable".Equals(name))
//                {
//                    Assert.AreEqual("aVariable", name);
//                }
//                else if ("anotherVariable".Equals(name))
//                {
//                    Assert.AreEqual("anotherVariable", name);
//                }
//                else
//                {
//                    Assert.Fail("Found an unexpected variable: '" + name + "'");
//                }

//                Assert.IsNull(variable.Value);
//            }

//            // complete ////////////////////////////////////////////////////////
//            string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;

//            complete(humanTaskId);
//            close(subCaseInstance.Id);
//            assertProcessEnded(superProcessInstanceId);
//        }

//        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testInputSourceExpression.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" }) public void testInputSourceExpression()
//        public virtual void testInputSourceExpression()
//        {
//            // given
//            VariableMap parameters = Variables.createVariables().putValue("aVariable", "abc").putValue("anotherVariable", 999);

//            // when
//            string superProcessInstanceId = StartProcessInstanceByKey(PROCESS_DEFINITION_KEY, parameters).Id;

//            // then
//            CaseExecutionEntity subCaseInstance = (CaseExecutionEntity)queryOneTaskCaseInstance();
//            Assert.NotNull(subCaseInstance);

//            IList<VariableInstance> variables = runtimeService.CreateVariableInstanceQuery().caseInstanceIdIn(subCaseInstance.Id);

//            Assert.IsFalse(variables.Count == 0);
//            Assert.AreEqual(2, variables.Count);

//            foreach (VariableInstance variable in variables)
//            {
//                string name = variable.Name;
//                if ("aVariable".Equals(name))
//                {
//                    Assert.AreEqual("aVariable", name);
//                    Assert.AreEqual("abc", variable.Value);

//                }
//                else if ("anotherVariable".Equals(name))
//                {
//                    Assert.AreEqual("anotherVariable", name);
//                    Assert.AreEqual((long)1000, variable.Value);

//                }
//                else
//                {
//                    Assert.Fail("Found an unexpected variable: '" + name + "'");
//                }
//            }

//            // complete ////////////////////////////////////////////////////////
//            string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;

//            complete(humanTaskId);
//            close(subCaseInstance.Id);
//            assertProcessEnded(superProcessInstanceId);
//        }

//        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testInputSourceAsCompositeExpression.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" }) public void testInputSourceAsCompositeExpression()
//        public virtual void testInputSourceAsCompositeExpression()
//        {
//            // given
//            VariableMap parameters = Variables.createVariables().putValue("aVariable", "abc").putValue("anotherVariable", 999);

//            // when
//            string superProcessInstanceId = StartProcessInstanceByKey(PROCESS_DEFINITION_KEY, parameters).Id;

//            // then
//            CaseExecutionEntity subCaseInstance = (CaseExecutionEntity)queryOneTaskCaseInstance();
//            Assert.NotNull(subCaseInstance);

//            IList<VariableInstance> variables = runtimeService.CreateVariableInstanceQuery().caseInstanceIdIn(subCaseInstance.Id);

//            Assert.IsFalse(variables.Count == 0);
//            Assert.AreEqual(2, variables.Count);

//            foreach (VariableInstance variable in variables)
//            {
//                string name = variable.Name;
//                if ("aVariable".Equals(name))
//                {
//                    Assert.AreEqual("aVariable", name);
//                    Assert.AreEqual("Prefixabc", variable.Value);

//                }
//                else if ("anotherVariable".Equals(name))
//                {
//                    Assert.AreEqual("anotherVariable", name);
//                    Assert.AreEqual("Prefix" + (long)1000, variable.Value);

//                }
//                else
//                {
//                    Assert.Fail("Found an unexpected variable: '" + name + "'");
//                }
//            }

//            // complete ////////////////////////////////////////////////////////
//            string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;

//            complete(humanTaskId);
//            close(subCaseInstance.Id);
//            assertProcessEnded(superProcessInstanceId);
//        }

//        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testInputAll.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" }) public void testInputAll()
//        public virtual void testInputAll()
//        {
//            // given
//            VariableMap parameters = Variables.createVariables().putValue("aVariable", "abc").putValue("anotherVariable", 999);

//            // when
//            string superProcessInstanceId = StartProcessInstanceByKey(PROCESS_DEFINITION_KEY, parameters).Id;

//            // then
//            CaseExecutionEntity subCaseInstance = (CaseExecutionEntity)queryOneTaskCaseInstance();
//            Assert.NotNull(subCaseInstance);

//            IList<VariableInstance> variables = runtimeService.CreateVariableInstanceQuery().caseInstanceIdIn(subCaseInstance.Id);

//            Assert.IsFalse(variables.Count == 0);
//            Assert.AreEqual(2, variables.Count);

//            foreach (VariableInstance variable in variables)
//            {
//                string name = variable.Name;
//                if ("aVariable".Equals(name))
//                {
//                    Assert.AreEqual("aVariable", name);
//                    Assert.AreEqual("abc", variable.Value);
//                }
//                else if ("anotherVariable".Equals(name))
//                {
//                    Assert.AreEqual("anotherVariable", name);
//                    Assert.AreEqual(999, variable.Value);
//                }
//                else
//                {
//                    Assert.Fail("Found an unexpected variable: '" + name + "'");
//                }
//            }

//            // complete ////////////////////////////////////////////////////////
//            string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;

//            complete(humanTaskId);
//            close(subCaseInstance.Id);
//            assertProcessEnded(superProcessInstanceId);

//        }

//        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testCompleteCase.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" }) public void testCompleteCase()
//        public virtual void testCompleteCase()
//        {
//            // given
//            string superProcessInstanceId = StartProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;
//            string subCaseInstanceId = queryOneTaskCaseInstance().Id;

//            string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;

//            // when
//            complete(humanTaskId);

//            // then
//            ITask userTask = queryTaskByActivityId(USER_TASK_ID);
//            Assert.NotNull(userTask);

//            Execution callActivity = queryExecutionByActivityId(CALL_ACTIVITY_ID);
//            Assert.IsNull(callActivity);

//            // complete ////////////////////////////////////////////////////////

//            close(subCaseInstanceId);
//            assertCaseEnded(subCaseInstanceId);

//            taskService.Complete(userTask.Id);
//            assertCaseEnded(superProcessInstanceId);

//        }

//        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testOutputSource.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" }) public void testOutputSource()
//        public virtual void testOutputSource()
//        {
//            // given
//            string superProcessInstanceId = StartProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;
//            string subCaseInstanceId = queryOneTaskCaseInstance().Id;

//            caseService.withCaseExecution(subCaseInstanceId).SetVariable("aVariable", "abc").SetVariable("anotherVariable", 999).SetVariable("aThirdVariable", "def").execute();

//            string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;

//            // when
//            complete(humanTaskId);

//            // then
//            IList<VariableInstance> variables = runtimeService.CreateVariableInstanceQuery(c=>c.ProcessInstanceId == superProcessInstanceId);

//            Assert.IsFalse(variables.Count == 0);
//            Assert.AreEqual(2, variables.Count);

//            foreach (VariableInstance variable in variables)
//            {
//                string name = variable.Name;
//                if ("aVariable".Equals(name))
//                {
//                    Assert.AreEqual("aVariable", name);
//                    Assert.AreEqual("abc", variable.Value);
//                }
//                else if ("anotherVariable".Equals(name))
//                {
//                    Assert.AreEqual("anotherVariable", name);
//                    Assert.AreEqual(999, variable.Value);
//                }
//                else
//                {
//                    Assert.Fail("Found an unexpected variable: '" + name + "'");
//                }
//            }

//            // complete ////////////////////////////////////////////////////////
//            close(subCaseInstanceId);
//            assertCaseEnded(subCaseInstanceId);

//            string taskId = queryTaskByActivityId(USER_TASK_ID).Id;
//            taskService.Complete(taskId);
//            assertProcessEnded(superProcessInstanceId);

//        }

//        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testOutputSourceDifferentTarget.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" }) public void testOutputSourceDifferentTarget()
//        public virtual void testOutputSourceDifferentTarget()
//        {
//            // given
//            string superProcessInstanceId = StartProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;
//            string subCaseInstanceId = queryOneTaskCaseInstance().Id;

//            caseService.withCaseExecution(subCaseInstanceId).SetVariable("aVariable", "abc").SetVariable("anotherVariable", 999).execute();

//            string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;

//            // when
//            complete(humanTaskId);

//            // then
//            IList<VariableInstance> variables = runtimeService.CreateVariableInstanceQuery(c=>c.ProcessInstanceId == superProcessInstanceId);

//            Assert.IsFalse(variables.Count == 0);
//            Assert.AreEqual(2, variables.Count);

//            foreach (VariableInstance variable in variables)
//            {
//                string name = variable.Name;
//                if ("myVariable".Equals(name))
//                {
//                    Assert.AreEqual("myVariable", name);
//                    Assert.AreEqual("abc", variable.Value);
//                }
//                else if ("myAnotherVariable".Equals(name))
//                {
//                    Assert.AreEqual("myAnotherVariable", name);
//                    Assert.AreEqual(999, variable.Value);
//                }
//                else
//                {
//                    Assert.Fail("Found an unexpected variable: '" + name + "'");
//                }
//            }

//            // complete ////////////////////////////////////////////////////////
//            close(subCaseInstanceId);
//            assertCaseEnded(subCaseInstanceId);

//            string taskId = queryTaskByActivityId(USER_TASK_ID).Id;
//            taskService.Complete(taskId);
//            assertProcessEnded(superProcessInstanceId);

//        }

//        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testOutputSource.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" }) public void testOutputSourceNullValue()
//        public virtual void testOutputSourceNullValue()
//        {
//            // given
//            string superProcessInstanceId = StartProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;
//            string subCaseInstanceId = queryOneTaskCaseInstance().Id;

//            string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;

//            // when
//            complete(humanTaskId);

//            // then
//            IList<VariableInstance> variables = runtimeService.CreateVariableInstanceQuery(c=>c.ProcessInstanceId == superProcessInstanceId);

//            Assert.IsFalse(variables.Count == 0);
//            Assert.AreEqual(2, variables.Count);

//            foreach (VariableInstance variable in variables)
//            {
//                string name = variable.Name;
//                if ("aVariable".Equals(name))
//                {
//                    Assert.AreEqual("aVariable", name);
//                }
//                else if ("anotherVariable".Equals(name))
//                {
//                    Assert.AreEqual("anotherVariable", name);
//                }
//                else
//                {
//                    Assert.Fail("Found an unexpected variable: '" + name + "'");
//                }

//                Assert.IsNull(variable.Value);
//            }

//            // complete ////////////////////////////////////////////////////////
//            close(subCaseInstanceId);
//            assertCaseEnded(subCaseInstanceId);

//            string taskId = queryTaskByActivityId(USER_TASK_ID).Id;
//            taskService.Complete(taskId);
//            assertProcessEnded(superProcessInstanceId);

//        }

//        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testOutputSourceExpression.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" }) public void testOutputSourceExpression()
//        public virtual void testOutputSourceExpression()
//        {
//            // given
//            string superProcessInstanceId = StartProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;
//            string subCaseInstanceId = queryOneTaskCaseInstance().Id;

//            caseService.withCaseExecution(subCaseInstanceId).SetVariable("aVariable", "abc").SetVariable("anotherVariable", 999).execute();

//            string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;

//            // when
//            complete(humanTaskId);

//            // then
//            IList<VariableInstance> variables = runtimeService.CreateVariableInstanceQuery(c=>c.ProcessInstanceId == superProcessInstanceId);

//            Assert.IsFalse(variables.Count == 0);
//            Assert.AreEqual(2, variables.Count);

//            foreach (VariableInstance variable in variables)
//            {
//                string name = variable.Name;
//                if ("aVariable".Equals(name))
//                {
//                    Assert.AreEqual("aVariable", name);
//                    Assert.AreEqual("abc", variable.Value);
//                }
//                else if ("anotherVariable".Equals(name))
//                {
//                    Assert.AreEqual("anotherVariable", name);
//                    Assert.AreEqual((long)1000, variable.Value);
//                }
//                else
//                {
//                    Assert.Fail("Found an unexpected variable: '" + name + "'");
//                }
//            }

//            // complete ////////////////////////////////////////////////////////
//            close(subCaseInstanceId);
//            assertCaseEnded(subCaseInstanceId);

//            string taskId = queryTaskByActivityId(USER_TASK_ID).Id;
//            taskService.Complete(taskId);
//            assertProcessEnded(superProcessInstanceId);

//        }

//        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testOutputSourceAsCompositeExpression.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" }) public void testOutputSourceAsCompositeExpression()
//        public virtual void testOutputSourceAsCompositeExpression()
//        {
//            // given
//            string superProcessInstanceId = StartProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;
//            string subCaseInstanceId = queryOneTaskCaseInstance().Id;

//            caseService.withCaseExecution(subCaseInstanceId).SetVariable("aVariable", "abc").SetVariable("anotherVariable", 999).execute();

//            string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;

//            // when
//            complete(humanTaskId);

//            // then
//            IList<VariableInstance> variables = runtimeService.CreateVariableInstanceQuery(c=>c.ProcessInstanceId == superProcessInstanceId);

//            Assert.IsFalse(variables.Count == 0);
//            Assert.AreEqual(2, variables.Count);

//            foreach (VariableInstance variable in variables)
//            {
//                string name = variable.Name;
//                if ("aVariable".Equals(name))
//                {
//                    Assert.AreEqual("aVariable", name);
//                    Assert.AreEqual("Prefixabc", variable.Value);
//                }
//                else if ("anotherVariable".Equals(name))
//                {
//                    Assert.AreEqual("anotherVariable", name);
//                    Assert.AreEqual("Prefix" + (long)1000, variable.Value);
//                }
//                else
//                {
//                    Assert.Fail("Found an unexpected variable: '" + name + "'");
//                }
//            }

//            // complete ////////////////////////////////////////////////////////
//            close(subCaseInstanceId);
//            assertCaseEnded(subCaseInstanceId);

//            string taskId = queryTaskByActivityId(USER_TASK_ID).Id;
//            taskService.Complete(taskId);
//            assertProcessEnded(superProcessInstanceId);

//        }

//        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testOutputAll.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" }) public void testOutputAll()
//        public virtual void testOutputAll()
//        {
//            // given
//            string superProcessInstanceId = StartProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;
//            string subCaseInstanceId = queryOneTaskCaseInstance().Id;

//            caseService.withCaseExecution(subCaseInstanceId).SetVariable("aVariable", "abc").SetVariable("anotherVariable", 999).execute();

//            string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;

//            // when
//            complete(humanTaskId);

//            // then
//            IList<VariableInstance> variables = runtimeService.CreateVariableInstanceQuery(c=>c.ProcessInstanceId == superProcessInstanceId);

//            Assert.IsFalse(variables.Count == 0);
//            Assert.AreEqual(2, variables.Count);

//            foreach (VariableInstance variable in variables)
//            {
//                string name = variable.Name;
//                if ("aVariable".Equals(name))
//                {
//                    Assert.AreEqual("aVariable", name);
//                    Assert.AreEqual("abc", variable.Value);
//                }
//                else if ("anotherVariable".Equals(name))
//                {
//                    Assert.AreEqual("anotherVariable", name);
//                    Assert.AreEqual(999, variable.Value);
//                }
//                else
//                {
//                    Assert.Fail("Found an unexpected variable: '" + name + "'");
//                }
//            }

//            // complete ////////////////////////////////////////////////////////
//            close(subCaseInstanceId);
//            assertCaseEnded(subCaseInstanceId);

//            string taskId = queryTaskByActivityId(USER_TASK_ID).Id;
//            taskService.Complete(taskId);
//            assertProcessEnded(superProcessInstanceId);

//        }

//        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testOutputAll.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" }) public void testOutputVariablesShouldNotExistAnymore()
//        public virtual void testOutputVariablesShouldNotExistAnymore()
//        {
//            // given
//            string superProcessInstanceId = StartProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;

//            string callActivityId = queryExecutionByActivityId(CALL_ACTIVITY_ID).Id;

//            VariableMap parameters = Variables.createVariables().putValue("aVariable", "xyz").putValue("anotherVariable", 123);

//            runtimeService.setVariablesLocal(callActivityId, parameters);

//            string subCaseInstanceId = queryOneTaskCaseInstance().Id;

//            caseService.withCaseExecution(subCaseInstanceId).SetVariable("aVariable", "abc").SetVariable("anotherVariable", 999).execute();

//            string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;

//            // when
//            complete(humanTaskId);

//            // then

//            // the variables has been deleted
//            IList<VariableInstance> variables = runtimeService.CreateVariableInstanceQuery(c=>c.ProcessInstanceId == superProcessInstanceId);

//            Assert.True(variables.Count == 0);

//            // complete ////////////////////////////////////////////////////////
//            close(subCaseInstanceId);
//            assertCaseEnded(subCaseInstanceId);

//            string taskId = queryTaskByActivityId(USER_TASK_ID).Id;
//            taskService.Complete(taskId);
//            assertProcessEnded(superProcessInstanceId);

//        }

//        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testVariablesRoundtrip.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" }) public void testVariablesRoundtrip()
//        public virtual void testVariablesRoundtrip()
//        {
//            // given
//            VariableMap parameters = Variables.createVariables().putValue("aVariable", "xyz").putValue("anotherVariable", 999);

//            string superProcessInstanceId = StartProcessInstanceByKey(PROCESS_DEFINITION_KEY, parameters).Id;

//            string subCaseInstanceId = queryOneTaskCaseInstance().Id;

//            caseService.withCaseExecution(subCaseInstanceId).SetVariable("aVariable", "abc").SetVariable("anotherVariable", 1000).execute();

//            string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;

//            // when
//            complete(humanTaskId);

//            // then
//            IList<VariableInstance> variables = runtimeService.CreateVariableInstanceQuery(c=>c.ProcessInstanceId == superProcessInstanceId);

//            Assert.IsFalse(variables.Count == 0);
//            Assert.AreEqual(2, variables.Count);

//            foreach (VariableInstance variable in variables)
//            {
//                string name = variable.Name;
//                if ("aVariable".Equals(name))
//                {
//                    Assert.AreEqual("aVariable", name);
//                    Assert.AreEqual("abc", variable.Value);
//                }
//                else if ("anotherVariable".Equals(name))
//                {
//                    Assert.AreEqual("anotherVariable", name);
//                    Assert.AreEqual(1000, variable.Value);
//                }
//                else
//                {
//                    Assert.Fail("Found an unexpected variable: '" + name + "'");
//                }
//            }

//            // complete ////////////////////////////////////////////////////////
//            close(subCaseInstanceId);
//            assertCaseEnded(subCaseInstanceId);

//            string taskId = queryTaskByActivityId(USER_TASK_ID).Id;
//            taskService.Complete(taskId);
//            assertProcessEnded(superProcessInstanceId);

//        }

//        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testOutputAll.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCaseWithManualActivation.cmmn" }) public void testCallCaseOutputAllVariablesTypedToProcess()
//        public virtual void testCallCaseOutputAllVariablesTypedToProcess()
//        {
//            StartProcessInstanceByKey("process");
//            CaseInstance caseInstance = queryOneTaskCaseInstance();
//            string VariableName = "foo";
//            string variableName2 = "null";
//            TypedValue variableValue = Variables.stringValue("bar");
//            TypedValue variableValue2 = Variables.integerValue(null);
//            caseService.withCaseExecution(caseInstance.Id).SetVariable(VariableName, variableValue).SetVariable(variableName2, variableValue2).execute();
//            complete(caseInstance.Id);

//            ITask task = taskService.CreateTaskQuery().First();
//            TypedValue value = runtimeService.getVariableTyped(task.ProcessInstanceId, VariableName);
//            Assert.That(value, Is.EqualTo(variableValue));
//            value = runtimeService.getVariableTyped(task.ProcessInstanceId, variableName2);
//            Assert.That(value, Is.EqualTo(variableValue2));
//        }

//        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testCallCaseAsConstant.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" }) public void testDeleteProcessInstance()
//        public virtual void testDeleteProcessInstance()
//        {
//            // given
//            string superProcessInstanceId = StartProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;
//            string subCaseInstanceId = queryOneTaskCaseInstance().Id;

//            // when
//            runtimeService.deleteProcessInstance(superProcessInstanceId, null);

//            // then
//            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery().Count());

//            CaseInstance subCaseInstance = queryOneTaskCaseInstance();
//            Assert.NotNull(subCaseInstance);
//            Assert.True(subCaseInstance.Active);

//            // complete ////////////////////////////////////////////////////////
//            terminate(subCaseInstanceId);
//            close(subCaseInstanceId);
//            assertCaseEnded(subCaseInstanceId);
//        }

//        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testCallCaseAsConstant.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" }) public void testSuspendProcessInstance()
//        public virtual void testSuspendProcessInstance()
//        {
//            // given
//            string superProcessInstanceId = StartProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;
//            string subCaseInstanceId = queryOneTaskCaseInstance().Id;

//            string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;

//            // when (1)
//            runtimeService.suspendProcessInstanceById(superProcessInstanceId);

//            // then
//            Execution superProcessInstance = queryExecutionById(superProcessInstanceId);
//            Assert.NotNull(superProcessInstance);
//            Assert.True(superProcessInstance.Suspended);

//            CaseInstance subCaseInstance = queryOneTaskCaseInstance();
//            Assert.NotNull(subCaseInstance);
//            Assert.True(subCaseInstance.Active);

//            try
//            {
//                // when
//                complete(humanTaskId);
//                Assert.Fail("The super process instance is suspended.");
//            }
//            catch (Exception)
//            {
//                // expected
//            }

//            // complete ////////////////////////////////////////////////////////
//            runtimeService.activateProcessInstanceById(superProcessInstanceId);

//            complete(humanTaskId);
//            close(subCaseInstance.Id);
//            assertCaseEnded(subCaseInstanceId);
//            assertProcessEnded(superProcessInstanceId);

//            repositoryService.DeleteDeployment(deploymentId, true);

//        }

//        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testCallCaseAsConstant.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" }) public void testTerminateSubCaseInstance()
//        public virtual void testTerminateSubCaseInstance()
//        {
//            // given
//            string superProcessInstanceId = StartProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;
//            string subCaseInstanceId = queryOneTaskCaseInstance().Id;

//            // when
//            terminate(subCaseInstanceId);

//            // then
//            CmmnExecution subCaseInstance = (CmmnExecution)queryOneTaskCaseInstance();
//            Assert.NotNull(subCaseInstance);
//            Assert.True(subCaseInstance.Terminated);

//            Execution callActivity = queryExecutionByActivityId(CALL_ACTIVITY_ID);
//            Assert.NotNull(callActivity);

//            // complete ////////////////////////////////////////////////////////

//            close(subCaseInstanceId);
//            assertCaseEnded(subCaseInstanceId);

//            runtimeService.deleteProcessInstance(superProcessInstanceId, null);
//        }

//        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testCallCaseAsConstant.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCaseWithManualActivation.cmmn" }) public void testSuspendSubCaseInstance()
//        public virtual void testSuspendSubCaseInstance()
//        {
//            // given
//            string superProcessInstanceId = StartProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;
//            string subCaseInstanceId = queryOneTaskCaseInstance().Id;

//            // when
//            suspend(subCaseInstanceId);

//            // then
//            CmmnExecution subCaseInstance = (CmmnExecution)queryOneTaskCaseInstance();
//            Assert.NotNull(subCaseInstance);
//            Assert.True(subCaseInstance.Suspended);

//            Execution callActivity = queryExecutionByActivityId(CALL_ACTIVITY_ID);
//            Assert.NotNull(callActivity);

//            // complete ////////////////////////////////////////////////////////

//            close(subCaseInstanceId);
//            assertCaseEnded(subCaseInstanceId);

//            runtimeService.deleteProcessInstance(superProcessInstanceId, null);
//        }

//        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivityTest.testCompletionOfCaseWithTwoTasks.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/twoTaskCase.cmmn" }) public void testCompletionOfTwoHumanTasks()
//        public virtual void testCompletionOfTwoHumanTasks()
//        {
//            // given
//            string superProcessInstanceId = StartProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;

//            // when (1)
//            string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;
//            manualStart(humanTaskId);
//            complete(humanTaskId);

//            // then (1)

//            Assert.AreEqual(0, taskService.CreateTaskQuery().Count());

//            // when (2)
//            humanTaskId = queryCaseExecutionByActivityId("PI_HumanTask_2").Id;
//            manualStart(humanTaskId);
//            complete(humanTaskId);

//            // then (2)
//            ITask task = taskService.CreateTaskQuery().First();
//            Assert.NotNull(task);

//            Assert.AreEqual(superProcessInstanceId, task.ProcessInstanceId);
//            Assert.AreEqual("userTask", task.TaskDefinitionKey);
//        }

//        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivity.testSubProcessLocalInputAllVariables.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" }) public void testSubProcessLocalInputAllVariables()
//        public virtual void testSubProcessLocalInputAllVariables()
//        {
//            IProcessInstance processInstance = StartProcessInstanceByKey("subProcessLocalInputAllVariables");
//            ITask beforeCallActivityTask = taskService.CreateTaskQuery().First();

//            // when setting a variable in a process instance
//            runtimeService.SetVariable(processInstance.Id, "callingProcessVar1", "val1");

//            // and executing the call activity
//            taskService.Complete(beforeCallActivityTask.Id);

//            // then only the local variable specified in the io mapping is passed to the called instance
//            CaseExecutionEntity calledInstance = (CaseExecutionEntity)queryOneTaskCaseInstance();
//            Assert.NotNull(calledInstance);

//            IDictionary<string, object> calledInstanceVariables = caseService.getVariables(calledInstance.Id);
//            Assert.AreEqual(1, calledInstanceVariables.Count);
//            Assert.AreEqual("val2", calledInstanceVariables["inputParameter"]);

//            // when setting a variable in the called instance
//            caseService.SetVariable(calledInstance.Id, "calledCaseVar1", 42L);

//            // and completing it
//            string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;
//            complete(humanTaskId);

//            // then the call activity output variable has been mapped to the process instance execution
//            // and the output mapping variable as well
//            IDictionary<string, object> callingInstanceVariables = runtimeService.getVariables(processInstance.Id);
//            Assert.AreEqual(3, callingInstanceVariables.Count);
//            Assert.AreEqual("val1", callingInstanceVariables["callingProcessVar1"]);
//            Assert.AreEqual(42L, callingInstanceVariables["calledCaseVar1"]);
//            Assert.AreEqual(43L, callingInstanceVariables["outputParameter"]);
//        }

//        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivity.testSubProcessLocalInputSingleVariable.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" }) public void testSubProcessLocalInputSingleVariable()
//        public virtual void testSubProcessLocalInputSingleVariable()
//        {
//            IProcessInstance processInstance = StartProcessInstanceByKey("subProcessLocalInputSingleVariable");
//            ITask beforeCallActivityTask = taskService.CreateTaskQuery().First();

//            // when setting a variable in a process instance
//            runtimeService.SetVariable(processInstance.Id, "callingProcessVar1", "val1");

//            // and executing the call activity
//            taskService.Complete(beforeCallActivityTask.Id);

//            // then the local variable specified in the io mapping is passed to the called instance
//            CaseExecutionEntity calledInstance = (CaseExecutionEntity)queryOneTaskCaseInstance();
//            Assert.NotNull(calledInstance);

//            IDictionary<string, object> calledInstanceVariables = caseService.getVariables(calledInstance.Id);
//            Assert.AreEqual(1, calledInstanceVariables.Count);
//            Assert.AreEqual("val2", calledInstanceVariables["mappedInputParameter"]);

//            // when setting a variable in the called instance
//            caseService.SetVariable(calledInstance.Id, "calledCaseVar1", 42L);

//            // and completing it
//            string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;
//            complete(humanTaskId);

//            // then the call activity output variable has been mapped to the process instance execution
//            // and the output mapping variable as well
//            IDictionary<string, object> callingInstanceVariables = runtimeService.getVariables(processInstance.Id);
//            Assert.AreEqual(4, callingInstanceVariables.Count);
//            Assert.AreEqual("val1", callingInstanceVariables["callingProcessVar1"]);
//            Assert.AreEqual("val2", callingInstanceVariables["mappedInputParameter"]);
//            Assert.AreEqual(42L, callingInstanceVariables["calledCaseVar1"]);
//            Assert.AreEqual(43L, callingInstanceVariables["outputParameter"]);
//        }

//        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivity.testSubProcessLocalInputSingleVariableExpression.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" }) public void testSubProcessLocalInputSingleVariableExpression()
//        public virtual void testSubProcessLocalInputSingleVariableExpression()
//        {
//            IProcessInstance processInstance = StartProcessInstanceByKey("subProcessLocalInputSingleVariableExpression");
//            ITask beforeCallActivityTask = taskService.CreateTaskQuery().First();

//            // when executing the call activity
//            taskService.Complete(beforeCallActivityTask.Id);

//            // then the local input parameter can be resolved because its source expression variable
//            // is defined in the call activity's input mapping
//            CaseExecutionEntity calledInstance = (CaseExecutionEntity)queryOneTaskCaseInstance();
//            Assert.NotNull(calledInstance);

//            IDictionary<string, object> calledInstanceVariables = caseService.getVariables(calledInstance.Id);
//            Assert.AreEqual(1, calledInstanceVariables.Count);
//            Assert.AreEqual(43L, calledInstanceVariables["mappedInputParameter"]);

//            // and completing it
//            string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;
//            complete(humanTaskId);

//            // and executing a call activity in parameter where the source variable is not mapped by an activity
//            // input parameter fails

//            ITask beforeSecondCallActivityTask = taskService.CreateTaskQuery().First();
//            runtimeService.SetVariable(processInstance.Id, "globalVariable", "42");

//            try
//            {
//                taskService.Complete(beforeSecondCallActivityTask.Id);
//                Assert.Fail("expected exception");
//            }
//            catch (ProcessEngineException e)
//            {
//                assertTextPresent("Cannot resolve identifier 'globalVariable'", e.Message);
//            }
//        }

//        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivity.testSubProcessLocalOutputAllVariables.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCaseWithManualActivation.cmmn" }) public void testSubProcessLocalOutputAllVariables()
//        public virtual void testSubProcessLocalOutputAllVariables()
//        {
//            IProcessInstance processInstance = StartProcessInstanceByKey("subProcessLocalOutputAllVariables");
//            ITask beforeCallActivityTask = taskService.CreateTaskQuery().First();

//            // when setting a variable in a process instance
//            runtimeService.SetVariable(processInstance.Id, "callingProcessVar1", "val1");

//            // and executing the call activity
//            taskService.Complete(beforeCallActivityTask.Id);

//            // then all variables have been mapped into the called instance
//            CaseExecutionEntity calledInstance = (CaseExecutionEntity)queryOneTaskCaseInstance();
//            Assert.NotNull(calledInstance);

//            IDictionary<string, object> calledInstanceVariables = caseService.getVariables(calledInstance.Id);
//            Assert.AreEqual(2, calledInstanceVariables.Count);
//            Assert.AreEqual("val1", calledInstanceVariables["callingProcessVar1"]);
//            Assert.AreEqual("val2", calledInstanceVariables["inputParameter"]);

//            // when setting a variable in the called instance
//            caseService.SetVariable(calledInstance.Id, "calledCaseVar1", 42L);

//            // and completing it
//            string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;
//            manualStart(humanTaskId);
//            complete(humanTaskId);

//            // then only the output mapping variable has been mapped into the calling process instance
//            IDictionary<string, object> callingInstanceVariables = runtimeService.getVariables(processInstance.Id);
//            Assert.AreEqual(2, callingInstanceVariables.Count);
//            Assert.AreEqual("val1", callingInstanceVariables["callingProcessVar1"]);
//            Assert.AreEqual(43L, callingInstanceVariables["outputParameter"]);
//        }

//        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/callactivity/CaseCallActivity.testSubProcessLocalOutputSingleVariable.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" }) public void testSubProcessLocalOutputSingleVariable()
//        public virtual void testSubProcessLocalOutputSingleVariable()
//        {
//            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("subProcessLocalOutputSingleVariable");
//            ITask beforeCallActivityTask = taskService.CreateTaskQuery().First();

//            // when setting a variable in a process instance
//            runtimeService.SetVariable(processInstance.Id, "callingProcessVar1", "val1");

//            // and executing the call activity
//            taskService.Complete(beforeCallActivityTask.Id);

//            // then all variables have been mapped into the called instance
//            CaseExecutionEntity calledInstance = (CaseExecutionEntity)queryOneTaskCaseInstance();
//            Assert.NotNull(calledInstance);

//            IDictionary<string, object> calledInstanceVariables = caseService.getVariables(calledInstance.Id);
//            Assert.AreEqual(2, calledInstanceVariables.Count);
//            Assert.AreEqual("val1", calledInstanceVariables["callingProcessVar1"]);
//            Assert.AreEqual("val2", calledInstanceVariables["inputParameter"]);

//            // when setting a variable in the called instance
//            caseService.SetVariable(calledInstance.Id, "calledCaseVar1", 42L);

//            // and completing it
//            string humanTaskId = queryCaseExecutionByActivityId(HUMAN_TASK_ID).Id;
//            complete(humanTaskId);

//            // then only the output mapping variable has been mapped into the calling process instance
//            IDictionary<string, object> callingInstanceVariables = runtimeService.getVariables(processInstance.Id);
//            Assert.AreEqual(2, callingInstanceVariables.Count);
//            Assert.AreEqual("val1", callingInstanceVariables["callingProcessVar1"]);
//            Assert.AreEqual(43L, callingInstanceVariables["outputParameter"]);
//        }

//        protected internal virtual IProcessInstance StartProcessInstanceByKey(string processDefinitionKey)
//        {
//            return StartProcessInstanceByKey(processDefinitionKey, null);
//        }

//        protected internal virtual IProcessInstance StartProcessInstanceByKey(string processDefinitionKey, IDictionary<string, object> variables)
//        {
//            return StartProcessInstanceByKey(processDefinitionKey, variables, null);
//        }

//        protected internal virtual IProcessInstance StartProcessInstanceByKey(string processDefinitionKey, IDictionary<string, object> variables, string businessKey)
//        {
//            return runtimeService.StartProcessInstanceByKey(processDefinitionKey, businessKey, variables);
//        }

//        protected internal virtual CaseExecution queryCaseExecutionById(string id)
//        {
//            return caseService.createCaseExecutionQuery().caseExecutionId(id).First();
//        }

//        protected internal virtual CaseExecution queryCaseExecutionByActivityId(string activityId)
//        {
//            return caseService.createCaseExecutionQuery().activityId(activityId).First();
//        }

//        protected internal virtual CaseInstance queryOneTaskCaseInstance()
//        {
//            return caseService.createCaseInstanceQuery().CaseDefinitionKey(ONE_TASK_CASE).First();
//        }

//        protected internal virtual Execution queryExecutionById(string id)
//        {
//            return runtimeService.createExecutionQuery().executionId(id).First();
//        }

//        protected internal virtual Execution queryExecutionByActivityId(string activityId)
//        {
//            return runtimeService.createExecutionQuery().activityId(activityId).First();
//        }

//        protected internal virtual ITask queryTaskByActivityId(string activityId)
//        {
//            return taskService.CreateTaskQuery(c=>c.TaskDefinitionKey ==activityId).First();
//        }

//    }

//}