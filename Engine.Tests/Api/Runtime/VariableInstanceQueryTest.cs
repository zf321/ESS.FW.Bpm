//using System;
//using System.Collections.Generic;
//using System.Linq;
//using ESS.FW.Bpm.Engine.Common;
//using ESS.FW.Bpm.Engine.Runtime;
//using ESS.FW.Bpm.Engine.Tests.Api.Runtime.Util;
//using ESS.FW.Bpm.Engine.Variable.Type;
//using ESS.FW.Bpm.Engine.Variable.Value;
//using NUnit.Framework;

//namespace ESS.FW.Bpm.Engine.Tests.Api.Runtime
//{
//    /// <summary>
//    /// </summary>
//    [TestFixture]
//    public class VariableInstanceQueryTest : PluggableProcessEngineTestCase
//    {
//        [Test][Deployment(new string[]{"resources/api/runtime/oneTaskProcess.bpmn20.xml"}) ]
//        public virtual void testQuery()
//        {
//            // given
//            IDictionary<string, object> variables1 = new Dictionary<string, object>();
//            variables1["intVar"] = 123;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables1);

//            IDictionary<string, object> variables2 = new Dictionary<string, object>();
//            variables2["stringVar"] = "test";
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables2);

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery();

//            // then
//            Assert.NotNull(query);

//            var result = query
//                .ToList();
//            Assert.IsFalse(result.Count == 0);
//            Assert.AreEqual(2, result.Count);

//            Assert.AreEqual(2, query.Count());

//            foreach (var var in result)
//            {
//                Assert.NotNull(var.Id);
//                if (var.Name.Equals("intVar"))
//                {
//                    Assert.AreEqual("intVar", var.Name);
//                    Assert.AreEqual(123, var.Value);
//                }
//                else if (var.Name.Equals("stringVar"))
//                {
//                    Assert.AreEqual("stringVar", var.Name);
//                    Assert.AreEqual("test", var.Value);
//                }
//                else
//                {
//                    Assert.Fail("An unexpected variable '" + var.Name + "' was found with value " + var.Value);
//                }
//            }
//        }

//        [Test]
//        public virtual void testQueryByVariableId()
//        {
//            // given
//            IDictionary<string, object> variables = new Dictionary<string, object>();
//            variables["var1"] = "test";
//            variables["var2"] = "test";
//            var task = taskService.NewTask();
//            taskService.SaveTask(task);
//            taskService.SetVariablesLocal(task.Id, variables);
//            var result = runtimeService.CreateVariableInstanceQuery()
//                /*.VariableName("var1")*/
//                .First();
//            Assert.NotNull(result);

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                .VariableId(result.Id);

//            // then
//            Assert.NotNull(query);
//            var resultById = query.First();
//            Assert.AreEqual(result.Id, resultById.Id);

//            // Delete task
//            taskService.DeleteTask(task.Id, true);
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByVariableName()
//        {
//            // given
//            IDictionary<string, object> variables = new Dictionary<string, object>();
//            variables["stringVar"] = "test";
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables);

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                //.VariableName("stringVar");

//            // then
//            var result = query
//                .ToList();
//            Assert.IsFalse(result.Count == 0);
//            Assert.AreEqual(1, result.Count);

//            Assert.AreEqual(1, query.Count());

//            var var = result[0];
//            Assert.AreEqual("stringVar", var.Name);
//            Assert.AreEqual("test", var.Value);
//            Assert.AreEqual("string", var.TypeName);
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByVariableNames()
//        {
//            // given
//            var variableValue = "a";
//            IDictionary<string, object> variables = new Dictionary<string, object>();
//            variables["process"] = variableValue;
//            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables);

//            var task = taskService.CreateTaskQuery()
//                .Where(c=>c.ProcessInstanceId==processInstance.Id)
//                .First();
//            taskService.SetVariableLocal(task.Id, "task", variableValue);
//            runtimeService.SetVariableLocal(task.ExecutionId, "execution", variableValue);

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                .VariableNameIn("task", "process", "execution");

//            // then
//            var result = query
//                .ToList();
//            Assert.IsFalse(result.Count == 0);
//            Assert.AreEqual(3, result.Count);

//            Assert.AreEqual(3, query.Count());

//            foreach (var variableInstance in result)
//            {
//                Assert.AreEqual(variableValue, variableInstance.Value);
//                Assert.AreEqual("string", variableInstance.TypeName);
//            }

//            Assert.AreEqual(1, runtimeService.CreateVariableInstanceQuery()
//                //.VariableName("task")
//                .VariableNameIn("task", "execution")
//                .Count());
//            Assert.AreEqual(0, runtimeService.CreateVariableInstanceQuery()
//                //.VariableName("task")
//                .VariableNameIn("process", "execution")
//                .Count());
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByVariableNameLike()
//        {
//            // given
//            IDictionary<string, object> variables = new Dictionary<string, object>();
//            variables["string%Var"] = "test";
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables);

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                //.VariableNameLike("%ing\\%V%");

//            // then
//            var result = query
//                .ToList();
//            Assert.IsFalse(result.Count == 0);
//            Assert.AreEqual(1, result.Count);

//            Assert.AreEqual(1, query.Count());

//            var var = result[0];
//            Assert.AreEqual("string%Var", var.Name);
//            Assert.AreEqual("test", var.Value);
//            Assert.AreEqual("string", var.TypeName);
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByVariableNameLikeWithoutAnyResult()
//        {
//            // given
//            IDictionary<string, object> variables = new Dictionary<string, object>();
//            variables["stringVar"] = "test";
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables);

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                //.VariableNameLike("%ingV_");

//            // then
//            var result = query
//                .ToList();
//            Assert.True(result.Count == 0);

//            Assert.AreEqual(0, query.Count());
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByNameAndVariableValueEquals_String()
//        {
//            // given
//            IDictionary<string, object> variables = new Dictionary<string, object>();
//            variables["stringVar"] = "test";
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables);

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                //.VariableValueEquals("stringVar", "test");

//            // then
//            var result = query
//                .ToList();
//            Assert.IsFalse(result.Count == 0);
//            Assert.AreEqual(1, result.Count);

//            Assert.AreEqual(1, query.Count());

//            var var = result[0];
//            Assert.AreEqual("stringVar", var.Name);
//            Assert.AreEqual("test", var.Value);
//            Assert.AreEqual("string", var.TypeName);
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByNameAndVariableValueNotEquals_String()
//        {
//            // given
//            IDictionary<string, object> variables1 = new Dictionary<string, object>();
//            variables1["stringVar"] = "test";
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables1);

//            IDictionary<string, object> variables2 = new Dictionary<string, object>();
//            variables2["stringVar"] = "test123";
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables2);

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                .VariableValueNotEquals("stringVar", "test123");

//            // then
//            var result = query
//                .ToList();
//            Assert.IsFalse(result.Count == 0);
//            Assert.AreEqual(1, result.Count);

//            Assert.AreEqual(1, query.Count());

//            var var = result[0];
//            Assert.AreEqual("stringVar", var.Name);
//            Assert.AreEqual("test", var.Value);
//            Assert.AreEqual("string", var.TypeName);
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByNameAndVariableValueGreaterThan_String()
//        {
//            // given
//            IDictionary<string, object> variables1 = new Dictionary<string, object>();
//            variables1["stringVar"] = "a";
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables1);

//            IDictionary<string, object> variables2 = new Dictionary<string, object>();
//            variables2["stringVar"] = "b";
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables2);

//            IDictionary<string, object> variables3 = new Dictionary<string, object>();
//            variables3["stringVar"] = "c";
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables3);

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                .VariableValueGreaterThan("stringVar", "a");

//            // then
//            var result = query
//                .ToList();
//            Assert.IsFalse(result.Count == 0);
//            Assert.AreEqual(2, result.Count);

//            Assert.AreEqual(2, query.Count());

//            foreach (var var in result)
//            {
//                Assert.AreEqual("stringVar", var.Name);
//                Assert.AreEqual("string", var.TypeName);
//                if (var.Value.Equals("b"))
//                    Assert.AreEqual("b", var.Value);
//                else if (var.Value.Equals("c"))
//                    Assert.AreEqual("c", var.Value);
//                else
//                    Assert.Fail("A non expected value occured: " + var.Value);
//            }
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByNameAndVariableValueGreaterThanOrEqual_String()
//        {
//            // given
//            IDictionary<string, object> variables1 = new Dictionary<string, object>();
//            variables1["stringVar"] = "a";
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables1);

//            IDictionary<string, object> variables2 = new Dictionary<string, object>();
//            variables2["stringVar"] = "b";
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables2);

//            IDictionary<string, object> variables3 = new Dictionary<string, object>();
//            variables3["stringVar"] = "c";
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables3);

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                .VariableValueGreaterThanOrEqual("stringVar", "a");

//            // then
//            var result = query
//                .ToList();
//            Assert.IsFalse(result.Count == 0);
//            Assert.AreEqual(3, result.Count);

//            Assert.AreEqual(3, query.Count());

//            foreach (var var in result)
//            {
//                Assert.AreEqual("stringVar", var.Name);
//                Assert.AreEqual("string", var.TypeName);
//                if (var.Value.Equals("a"))
//                    Assert.AreEqual("a", var.Value);
//                else if (var.Value.Equals("b"))
//                    Assert.AreEqual("b", var.Value);
//                else if (var.Value.Equals("c"))
//                    Assert.AreEqual("c", var.Value);
//                else
//                    Assert.Fail("A non expected value occured: " + var.Value);
//            }
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByNameAndVariableValueLessThan_String()
//        {
//            // given
//            IDictionary<string, object> variables1 = new Dictionary<string, object>();
//            variables1["stringVar"] = "a";
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables1);

//            IDictionary<string, object> variables2 = new Dictionary<string, object>();
//            variables2["stringVar"] = "b";
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables2);

//            IDictionary<string, object> variables3 = new Dictionary<string, object>();
//            variables3["stringVar"] = "c";
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables3);

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                .VariableValueLessThan("stringVar", "c");

//            // then
//            var result = query
//                .ToList();
//            Assert.IsFalse(result.Count == 0);
//            Assert.AreEqual(2, result.Count);

//            Assert.AreEqual(2, query.Count());

//            foreach (var var in result)
//            {
//                Assert.AreEqual("stringVar", var.Name);
//                Assert.AreEqual("string", var.TypeName);
//                if (var.Value.Equals("a"))
//                    Assert.AreEqual("a", var.Value);
//                else if (var.Value.Equals("b"))
//                    Assert.AreEqual("b", var.Value);
//                else
//                    Assert.Fail("A non expected value occured: " + var.Value);
//            }
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByNameAndVariableValueLessThanOrEqual_String()
//        {
//            // given
//            IDictionary<string, object> variables1 = new Dictionary<string, object>();
//            variables1["stringVar"] = "a";
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables1);

//            IDictionary<string, object> variables2 = new Dictionary<string, object>();
//            variables2["stringVar"] = "b";
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables2);

//            IDictionary<string, object> variables3 = new Dictionary<string, object>();
//            variables3["stringVar"] = "c";
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables3);

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                .VariableValueLessThanOrEqual("stringVar", "c");

//            // then
//            var result = query
//                .ToList();
//            Assert.IsFalse(result.Count == 0);
//            Assert.AreEqual(3, result.Count);

//            Assert.AreEqual(3, query.Count());

//            foreach (var var in result)
//            {
//                Assert.AreEqual("stringVar", var.Name);
//                Assert.AreEqual("string", var.TypeName);
//                if (var.Value.Equals("a"))
//                    Assert.AreEqual("a", var.Value);
//                else if (var.Value.Equals("b"))
//                    Assert.AreEqual("b", var.Value);
//                else if (var.Value.Equals("c"))
//                    Assert.AreEqual("c", var.Value);
//                else
//                    Assert.Fail("A non expected value occured: " + var.Value);
//            }
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByNameAndVariableValueLike_String()
//        {
//            // given
//            IDictionary<string, object> variables1 = new Dictionary<string, object>();
//            variables1["stringVar"] = "test123";
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables1);

//            IDictionary<string, object> variables2 = new Dictionary<string, object>();
//            variables2["stringVar"] = "test456";
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables2);

//            IDictionary<string, object> variables3 = new Dictionary<string, object>();
//            variables3["stringVar"] = "test789";
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables3);

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                .VariableValueLike("stringVar", "test%");

//            // then
//            var result = query
//                .ToList();
//            Assert.IsFalse(result.Count == 0);
//            Assert.AreEqual(3, result.Count);

//            Assert.AreEqual(3, query.Count());

//            foreach (var var in result)
//            {
//                Assert.AreEqual("stringVar", var.Name);
//                Assert.AreEqual("string", var.TypeName);
//                if (var.Value.Equals("test123"))
//                    Assert.AreEqual("test123", var.Value);
//                else if (var.Value.Equals("test456"))
//                    Assert.AreEqual("test456", var.Value);
//                else if (var.Value.Equals("test789"))
//                    Assert.AreEqual("test789", var.Value);
//                else
//                    Assert.Fail("A non expected value occured: " + var.Value);
//            }
//        }

//        [Test][Deployment(new string[] {"resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByNameAndVariableValueLikeWithEscape_String()
//        {
//            // given
//            IDictionary<string, object> variables1 = new Dictionary<string, object>();
//            variables1["stringVar"] = "test_123";
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables1);

//            IDictionary<string, object> variables2 = new Dictionary<string, object>();
//            variables2["stringVar"] = "test%456";
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables2);

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                .VariableValueLike("stringVar", "test\\_%");
//            verifyQueryResult(query, "test_123");

//            query = runtimeService.CreateVariableInstanceQuery()
//                .VariableValueLike("stringVar", "test\\%%");
//            verifyQueryResult(query, "test%456");
//        }

//        private void verifyQueryResult(IQueryable<IVariableInstance> query, string varValue)
//        {
//            // then
//            var result = query
//                .ToList();
//            Assert.IsFalse(result.Count == 0);
//            Assert.AreEqual(1, result.Count);

//            Assert.AreEqual(1, query.Count());

//            foreach (var var in result)
//            {
//                Assert.AreEqual("stringVar", var.Name);
//                Assert.AreEqual("string", var.TypeName);

//                Assert.AreEqual(varValue, var.Value);
//            }
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByNameAndVariableValueEquals_Integer()
//        {
//            // given
//            IDictionary<string, object> variables = new Dictionary<string, object>();
//            variables["intValue"] = 1234;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables);

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery();
//                ////.VariableValueEquals("intValue", 1234);

//            // then
//            var result = query
//                .ToList();
//            Assert.IsFalse(result.Count == 0);
//            Assert.AreEqual(1, result.Count);

//            Assert.AreEqual(1, query.Count());

//            var var = result[0];
//            Assert.AreEqual("intValue", var.Name);
//            Assert.AreEqual(1234, var.Value);
//            Assert.AreEqual("integer", var.TypeName);
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByNameAndVariableValueNotEquals_Integer()
//        {
//            // given
//            IDictionary<string, object> variables1 = new Dictionary<string, object>();
//            variables1["intValue"] = 1234;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables1);

//            IDictionary<string, object> variables2 = new Dictionary<string, object>();
//            variables2["intValue"] = 5555;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables2);

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                .VariableValueNotEquals("intValue", 5555);

//            // then
//            var result = query
//                .ToList();
//            Assert.IsFalse(result.Count == 0);
//            Assert.AreEqual(1, result.Count);

//            Assert.AreEqual(1, query.Count());

//            var var = result[0];
//            Assert.AreEqual("intValue", var.Name);
//            Assert.AreEqual(1234, var.Value);
//            Assert.AreEqual("integer", var.TypeName);
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByNameAndVariableGreaterThan_Integer()
//        {
//            // given
//            IDictionary<string, object> variables1 = new Dictionary<string, object>();
//            variables1["intValue"] = 1234;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables1);

//            IDictionary<string, object> variables2 = new Dictionary<string, object>();
//            variables2["intValue"] = 5555;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables2);

//            IDictionary<string, object> variables3 = new Dictionary<string, object>();
//            variables3["intValue"] = 9876;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables3);

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                .VariableValueGreaterThan("intValue", 1234);

//            // then
//            var result = query
//                .ToList();
//            Assert.IsFalse(result.Count == 0);
//            Assert.AreEqual(2, result.Count);

//            Assert.AreEqual(2, query.Count());

//            foreach (var var in result)
//            {
//                Assert.AreEqual("intValue", var.Name);
//                Assert.AreEqual("integer", var.TypeName);
//                if (var.Value.Equals(5555))
//                    Assert.AreEqual(5555, var.Value);
//                else if (var.Value.Equals(9876))
//                    Assert.AreEqual(9876, var.Value);
//                else
//                    Assert.Fail("A non expected value occured: " + var.Value);
//            }
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByNameAndVariableGreaterThanAndEqual_Integer()
//        {
//            // given
//            IDictionary<string, object> variables1 = new Dictionary<string, object>();
//            variables1["intValue"] = 1234;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables1);

//            IDictionary<string, object> variables2 = new Dictionary<string, object>();
//            variables2["intValue"] = 5555;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables2);

//            IDictionary<string, object> variables3 = new Dictionary<string, object>();
//            variables3["intValue"] = 9876;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables3);

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                .VariableValueGreaterThanOrEqual("intValue", 1234);

//            // then
//            var result = query
//                .ToList();
//            Assert.IsFalse(result.Count == 0);
//            Assert.AreEqual(3, result.Count);

//            Assert.AreEqual(3, query.Count());

//            foreach (var var in result)
//            {
//                Assert.AreEqual("intValue", var.Name);
//                Assert.AreEqual("integer", var.TypeName);
//                if (var.Value.Equals(1234))
//                    Assert.AreEqual(1234, var.Value);
//                else if (var.Value.Equals(5555))
//                    Assert.AreEqual(5555, var.Value);
//                else if (var.Value.Equals(9876))
//                    Assert.AreEqual(9876, var.Value);
//                else
//                    Assert.Fail("A non expected value occured: " + var.Value);
//            }
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByNameAndVariableLessThan_Integer()
//        {
//            // given
//            IDictionary<string, object> variables1 = new Dictionary<string, object>();
//            variables1["intValue"] = 1234;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables1);

//            IDictionary<string, object> variables2 = new Dictionary<string, object>();
//            variables2["intValue"] = 5555;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables2);

//            IDictionary<string, object> variables3 = new Dictionary<string, object>();
//            variables3["intValue"] = 9876;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables3);

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                .VariableValueLessThan("intValue", 9876);

//            // then
//            var result = query
//                .ToList();
//            Assert.IsFalse(result.Count == 0);
//            Assert.AreEqual(2, result.Count);

//            Assert.AreEqual(2, query.Count());

//            foreach (var var in result)
//            {
//                Assert.AreEqual("intValue", var.Name);
//                Assert.AreEqual("integer", var.TypeName);
//                if (var.Value.Equals(5555))
//                    Assert.AreEqual(5555, var.Value);
//                else if (var.Value.Equals(1234))
//                    Assert.AreEqual(1234, var.Value);
//                else
//                    Assert.Fail("A non expected value occured: " + var.Value);
//            }
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByNameAndVariableLessThanAndEqual_Integer()
//        {
//            // given
//            IDictionary<string, object> variables1 = new Dictionary<string, object>();
//            variables1["intValue"] = 1234;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables1);

//            IDictionary<string, object> variables2 = new Dictionary<string, object>();
//            variables2["intValue"] = 5555;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables2);

//            IDictionary<string, object> variables3 = new Dictionary<string, object>();
//            variables3["intValue"] = 9876;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables3);

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                .VariableValueLessThanOrEqual("intValue", 9876);

//            // then
//            var result = query
//                .ToList();
//            Assert.IsFalse(result.Count == 0);
//            Assert.AreEqual(3, result.Count);

//            Assert.AreEqual(3, query.Count());

//            foreach (var var in result)
//            {
//                Assert.AreEqual("intValue", var.Name);
//                Assert.AreEqual("integer", var.TypeName);
//                if (var.Value.Equals(1234))
//                    Assert.AreEqual(1234, var.Value);
//                else if (var.Value.Equals(5555))
//                    Assert.AreEqual(5555, var.Value);
//                else if (var.Value.Equals(9876))
//                    Assert.AreEqual(9876, var.Value);
//                else
//                    Assert.Fail("A non expected value occured: " + var.Value);
//            }
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByNameAndVariableValueEquals_Long()
//        {
//            // given
//            IDictionary<string, object> variables = new Dictionary<string, object>();
//            variables["longValue"] = 123456L;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables);

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                //.VariableValueEquals("longValue", 123456L);

//            // then
//            var result = query
//                .ToList();
//            Assert.IsFalse(result.Count == 0);
//            Assert.AreEqual(1, result.Count);

//            Assert.AreEqual(1, query.Count());

//            var var = result[0];
//            Assert.AreEqual("longValue", var.Name);
//            Assert.AreEqual(123456L, var.Value);
//            Assert.AreEqual("long", var.TypeName);
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByNameAndVariableValueNotEquals_Long()
//        {
//            // given
//            IDictionary<string, object> variables1 = new Dictionary<string, object>();
//            variables1["longValue"] = 123456L;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables1);

//            IDictionary<string, object> variables2 = new Dictionary<string, object>();
//            variables2["longValue"] = 987654L;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables2);

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                .VariableValueNotEquals("longValue", 987654L);

//            // then
//            var result = query
//                .ToList();
//            Assert.IsFalse(result.Count == 0);
//            Assert.AreEqual(1, result.Count);

//            Assert.AreEqual(1, query.Count());

//            var var = result[0];
//            Assert.AreEqual("longValue", var.Name);
//            Assert.AreEqual(123456L, var.Value);
//            Assert.AreEqual("long", var.TypeName);
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByNameAndVariableGreaterThan_Long()
//        {
//            // given
//            IDictionary<string, object> variables1 = new Dictionary<string, object>();
//            variables1["longValue"] = 123456L;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables1);

//            IDictionary<string, object> variables2 = new Dictionary<string, object>();
//            variables2["longValue"] = 987654L;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables2);

//            IDictionary<string, object> variables3 = new Dictionary<string, object>();
//            variables3["longValue"] = 555555L;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables3);

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                .VariableValueGreaterThan("longValue", 123456L);

//            // then
//            var result = query
//                .ToList();
//            Assert.IsFalse(result.Count == 0);
//            Assert.AreEqual(2, result.Count);

//            Assert.AreEqual(2, query.Count());

//            foreach (var var in result)
//            {
//                Assert.AreEqual("longValue", var.Name);
//                Assert.AreEqual("long", var.TypeName);
//                if (var.Value.Equals(555555L))
//                    Assert.AreEqual(555555L, var.Value);
//                else if (var.Value.Equals(987654L))
//                    Assert.AreEqual(987654L, var.Value);
//                else
//                    Assert.Fail("A non expected value occured: " + var.Value);
//            }
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByNameAndVariableGreaterThanAndEqual_Long()
//        {
//            // given
//            IDictionary<string, object> variables1 = new Dictionary<string, object>();
//            variables1["longValue"] = 123456L;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables1);

//            IDictionary<string, object> variables2 = new Dictionary<string, object>();
//            variables2["longValue"] = 987654L;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables2);

//            IDictionary<string, object> variables3 = new Dictionary<string, object>();
//            variables3["longValue"] = 555555L;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables3);

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                .VariableValueGreaterThanOrEqual("longValue", 123456L);

//            // then
//            var result = query
//                .ToList();
//            Assert.IsFalse(result.Count == 0);
//            Assert.AreEqual(3, result.Count);

//            Assert.AreEqual(3, query.Count());

//            foreach (var var in result)
//            {
//                Assert.AreEqual("longValue", var.Name);
//                Assert.AreEqual("long", var.TypeName);
//                if (var.Value.Equals(123456L))
//                    Assert.AreEqual(123456L, var.Value);
//                else if (var.Value.Equals(555555L))
//                    Assert.AreEqual(555555L, var.Value);
//                else if (var.Value.Equals(987654L))
//                    Assert.AreEqual(987654L, var.Value);
//                else
//                    Assert.Fail("A non expected value occured: " + var.Value);
//            }
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByNameAndVariableLessThan_Long()
//        {
//            // given
//            IDictionary<string, object> variables1 = new Dictionary<string, object>();
//            variables1["longValue"] = 123456L;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables1);

//            IDictionary<string, object> variables2 = new Dictionary<string, object>();
//            variables2["longValue"] = 987654L;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables2);

//            IDictionary<string, object> variables3 = new Dictionary<string, object>();
//            variables3["longValue"] = 555555L;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables3);

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                .VariableValueLessThan("longValue", 987654L);

//            // then
//            var result = query
//                .ToList();
//            Assert.IsFalse(result.Count == 0);
//            Assert.AreEqual(2, result.Count);

//            Assert.AreEqual(2, query.Count());

//            foreach (var var in result)
//            {
//                Assert.AreEqual("longValue", var.Name);
//                Assert.AreEqual("long", var.TypeName);
//                if (var.Value.Equals(123456L))
//                    Assert.AreEqual(123456L, var.Value);
//                else if (var.Value.Equals(555555L))
//                    Assert.AreEqual(555555L, var.Value);
//                else
//                    Assert.Fail("A non expected value occured: " + var.Value);
//            }
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByNameAndVariableLessThanAndEqual_Long()
//        {
//            // given
//            IDictionary<string, object> variables1 = new Dictionary<string, object>();
//            variables1["longValue"] = 123456L;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables1);

//            IDictionary<string, object> variables2 = new Dictionary<string, object>();
//            variables2["longValue"] = 987654L;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables2);

//            IDictionary<string, object> variables3 = new Dictionary<string, object>();
//            variables3["longValue"] = 555555L;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables3);

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                .VariableValueLessThanOrEqual("longValue", 987654L);

//            // then
//            var result = query
//                .ToList();
//            Assert.IsFalse(result.Count == 0);
//            Assert.AreEqual(3, result.Count);

//            Assert.AreEqual(3, query.Count());

//            foreach (var var in result)
//            {
//                Assert.AreEqual("longValue", var.Name);
//                Assert.AreEqual("long", var.TypeName);
//                if (var.Value.Equals(123456L))
//                    Assert.AreEqual(123456L, var.Value);
//                else if (var.Value.Equals(555555L))
//                    Assert.AreEqual(555555L, var.Value);
//                else if (var.Value.Equals(987654L))
//                    Assert.AreEqual(987654L, var.Value);
//                else
//                    Assert.Fail("A non expected value occured: " + var.Value);
//            }
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByNameAndVariableValueEquals_Double()
//        {
//            // given
//            IDictionary<string, object> variables = new Dictionary<string, object>();
//            variables["doubleValue"] = 123.456;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables);

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                //.VariableValueEquals("doubleValue", 123.456);

//            // then
//            var result = query
//                .ToList();
//            Assert.IsFalse(result.Count == 0);
//            Assert.AreEqual(1, result.Count);

//            Assert.AreEqual(1, query.Count());

//            var var = result[0];
//            Assert.AreEqual("doubleValue", var.Name);
//            Assert.AreEqual(123.456, var.Value);
//            Assert.AreEqual("double", var.TypeName);
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByNameAndVariableValueNotEquals_Double()
//        {
//            // given
//            IDictionary<string, object> variables1 = new Dictionary<string, object>();
//            variables1["doubleValue"] = 123.456;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables1);

//            IDictionary<string, object> variables2 = new Dictionary<string, object>();
//            variables2["doubleValue"] = 654.321;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables2);

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                .VariableValueNotEquals("doubleValue", 654.321);

//            // then
//            var result = query
//                .ToList();
//            Assert.IsFalse(result.Count == 0);
//            Assert.AreEqual(1, result.Count);

//            Assert.AreEqual(1, query.Count());

//            var var = result[0];
//            Assert.AreEqual("doubleValue", var.Name);
//            Assert.AreEqual(123.456, var.Value);
//            Assert.AreEqual("double", var.TypeName);
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByNameAndVariableGreaterThan_Double()
//        {
//            // given
//            IDictionary<string, object> variables1 = new Dictionary<string, object>();
//            variables1["doubleValue"] = 123.456;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables1);

//            IDictionary<string, object> variables2 = new Dictionary<string, object>();
//            variables2["doubleValue"] = 654.321;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables2);

//            IDictionary<string, object> variables3 = new Dictionary<string, object>();
//            variables3["doubleValue"] = 999.999;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables3);

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                .VariableValueGreaterThan("doubleValue", 123.456);

//            // then
//            var result = query
//                .ToList();
//            Assert.IsFalse(result.Count == 0);
//            Assert.AreEqual(2, result.Count);

//            Assert.AreEqual(2, query.Count());

//            foreach (var var in result)
//            {
//                Assert.AreEqual("doubleValue", var.Name);
//                Assert.AreEqual("double", var.TypeName);
//                if (var.Value.Equals(654.321))
//                    Assert.AreEqual(654.321, var.Value);
//                else if (var.Value.Equals(999.999))
//                    Assert.AreEqual(999.999, var.Value);
//                else
//                    Assert.Fail("A non expected value occured: " + var.Value);
//            }
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByNameAndVariableGreaterThanAndEqual_Double()
//        {
//            // given
//            IDictionary<string, object> variables1 = new Dictionary<string, object>();
//            variables1["doubleValue"] = 123.456;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables1);

//            IDictionary<string, object> variables2 = new Dictionary<string, object>();
//            variables2["doubleValue"] = 654.321;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables2);

//            IDictionary<string, object> variables3 = new Dictionary<string, object>();
//            variables3["doubleValue"] = 999.999;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables3);

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                .VariableValueGreaterThanOrEqual("doubleValue", 123.456);

//            // then
//            var result = query
//                .ToList();
//            Assert.IsFalse(result.Count == 0);
//            Assert.AreEqual(3, result.Count);

//            Assert.AreEqual(3, query.Count());

//            foreach (var var in result)
//            {
//                Assert.AreEqual("doubleValue", var.Name);
//                Assert.AreEqual("double", var.TypeName);
//                if (var.Value.Equals(123.456))
//                    Assert.AreEqual(123.456, var.Value);
//                else if (var.Value.Equals(654.321))
//                    Assert.AreEqual(654.321, var.Value);
//                else if (var.Value.Equals(999.999))
//                    Assert.AreEqual(999.999, var.Value);
//                else
//                    Assert.Fail("A non expected value occured: " + var.Value);
//            }
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByNameAndVariableLessThan_Double()
//        {
//            // given
//            IDictionary<string, object> variables1 = new Dictionary<string, object>();
//            variables1["doubleValue"] = 123.456;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables1);

//            IDictionary<string, object> variables2 = new Dictionary<string, object>();
//            variables2["doubleValue"] = 654.321;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables2);

//            IDictionary<string, object> variables3 = new Dictionary<string, object>();
//            variables3["doubleValue"] = 999.999;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables3);

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                .VariableValueLessThan("doubleValue", 999.999);

//            // then
//            var result = query
//                .ToList();
//            Assert.IsFalse(result.Count == 0);
//            Assert.AreEqual(2, result.Count);

//            Assert.AreEqual(2, query.Count());

//            foreach (var var in result)
//            {
//                Assert.AreEqual("doubleValue", var.Name);
//                Assert.AreEqual("double", var.TypeName);
//                if (var.Value.Equals(123.456))
//                    Assert.AreEqual(123.456, var.Value);
//                else if (var.Value.Equals(654.321))
//                    Assert.AreEqual(654.321, var.Value);
//                else
//                    Assert.Fail("A non expected value occured: " + var.Value);
//            }
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByNameAndVariableLessThanAndEqual_Double()
//        {
//            // given
//            IDictionary<string, object> variables1 = new Dictionary<string, object>();
//            variables1["doubleValue"] = 123.456;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables1);

//            IDictionary<string, object> variables2 = new Dictionary<string, object>();
//            variables2["doubleValue"] = 654.321;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables2);

//            IDictionary<string, object> variables3 = new Dictionary<string, object>();
//            variables3["doubleValue"] = 999.999;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables3);

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                .VariableValueLessThanOrEqual("doubleValue", 999.999);

//            // then
//            var result = query
//                .ToList();
//            Assert.IsFalse(result.Count == 0);
//            Assert.AreEqual(3, result.Count);

//            Assert.AreEqual(3, query.Count());

//            foreach (var var in result)
//            {
//                Assert.AreEqual("doubleValue", var.Name);
//                Assert.AreEqual("double", var.TypeName);
//                if (var.Value.Equals(123.456))
//                    Assert.AreEqual(123.456, var.Value);
//                else if (var.Value.Equals(654.321))
//                    Assert.AreEqual(654.321, var.Value);
//                else if (var.Value.Equals(999.999))
//                    Assert.AreEqual(999.999, var.Value);
//                else
//                    Assert.Fail("A non expected value occured: " + var.Value);
//            }
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByNameAndVariableValueEquals_Short()
//        {
//            // given
//            IDictionary<string, object> variables = new Dictionary<string, object>();
//            variables["shortValue"] = (short) 123;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables);

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                //.VariableValueEquals("shortValue", (short) 123);

//            // then
//            var result = query
//                .ToList();
//            Assert.IsFalse(result.Count == 0);
//            Assert.AreEqual(1, result.Count);

//            Assert.AreEqual(1, query.Count());

//            var var = result[0];
//            Assert.AreEqual("shortValue", var.Name);
//            Assert.AreEqual((short) 123, var.Value);
//            Assert.AreEqual("short", var.TypeName);
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByVariableValueNotEquals_Short()
//        {
//            // given
//            IDictionary<string, object> variables1 = new Dictionary<string, object>();
//            variables1["shortValue"] = (short) 123;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables1);

//            IDictionary<string, object> variables2 = new Dictionary<string, object>();
//            variables2["shortValue"] = (short) 999;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables2);

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                .VariableValueNotEquals("shortValue", (short) 999);

//            // then
//            var result = query
//                .ToList();
//            Assert.IsFalse(result.Count == 0);
//            Assert.AreEqual(1, result.Count);

//            Assert.AreEqual(1, query.Count());

//            var var = result[0];
//            Assert.AreEqual("shortValue", var.Name);
//            Assert.AreEqual((short) 123, var.Value);
//            Assert.AreEqual("short", var.TypeName);
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByNameAndVariableGreaterThan_Short()
//        {
//            // given
//            IDictionary<string, object> variables1 = new Dictionary<string, object>();
//            variables1["shortValue"] = (short) 123;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables1);

//            IDictionary<string, object> variables2 = new Dictionary<string, object>();
//            variables2["shortValue"] = (short) 999;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables2);

//            IDictionary<string, object> variables3 = new Dictionary<string, object>();
//            variables3["shortValue"] = (short) 555;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables3);

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                .VariableValueGreaterThan("shortValue", (short) 123);

//            // then
//            var result = query
//                .ToList();
//            Assert.IsFalse(result.Count == 0);
//            Assert.AreEqual(2, result.Count);

//            Assert.AreEqual(2, query.Count());

//            foreach (var var in result)
//            {
//                Assert.AreEqual("shortValue", var.Name);
//                Assert.AreEqual("short", var.TypeName);
//                if (var.Value.Equals((short) 555))
//                    Assert.AreEqual((short) 555, var.Value);
//                else if (var.Value.Equals((short) 999))
//                    Assert.AreEqual((short) 999, var.Value);
//                else
//                    Assert.Fail("A non expected value occured: " + var.Value);
//            }
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByNameAndVariableGreaterThanAndEqual_Short()
//        {
//            // given
//            IDictionary<string, object> variables1 = new Dictionary<string, object>();
//            variables1["shortValue"] = (short) 123;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables1);

//            IDictionary<string, object> variables2 = new Dictionary<string, object>();
//            variables2["shortValue"] = (short) 999;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables2);

//            IDictionary<string, object> variables3 = new Dictionary<string, object>();
//            variables3["shortValue"] = (short) 555;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables3);

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                .VariableValueGreaterThanOrEqual("shortValue", (short) 123);

//            // then
//            var result = query
//                .ToList();
//            Assert.IsFalse(result.Count == 0);
//            Assert.AreEqual(3, result.Count);

//            Assert.AreEqual(3, query.Count());

//            foreach (var var in result)
//            {
//                Assert.AreEqual("shortValue", var.Name);
//                Assert.AreEqual("short", var.TypeName);
//                if (var.Value.Equals((short) 123))
//                    Assert.AreEqual((short) 123, var.Value);
//                else if (var.Value.Equals((short) 555))
//                    Assert.AreEqual((short) 555, var.Value);
//                else if (var.Value.Equals((short) 999))
//                    Assert.AreEqual((short) 999, var.Value);
//                else
//                    Assert.Fail("A non expected value occured: " + var.Value);
//            }
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByNameAndVariableLessThan_Short()
//        {
//            // given
//            IDictionary<string, object> variables1 = new Dictionary<string, object>();
//            variables1["shortValue"] = (short) 123;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables1);

//            IDictionary<string, object> variables2 = new Dictionary<string, object>();
//            variables2["shortValue"] = (short) 999;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables2);

//            IDictionary<string, object> variables3 = new Dictionary<string, object>();
//            variables3["shortValue"] = (short) 555;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables3);

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                .VariableValueLessThan("shortValue", (short) 999);

//            // then
//            var result = query
//                .ToList();
//            Assert.IsFalse(result.Count == 0);
//            Assert.AreEqual(2, result.Count);

//            Assert.AreEqual(2, query.Count());

//            foreach (var var in result)
//            {
//                Assert.AreEqual("shortValue", var.Name);
//                Assert.AreEqual("short", var.TypeName);
//                if (var.Value.Equals((short) 123))
//                    Assert.AreEqual((short) 123, var.Value);
//                else if (var.Value.Equals((short) 555))
//                    Assert.AreEqual((short) 555, var.Value);
//                else
//                    Assert.Fail("A non expected value occured: " + var.Value);
//            }
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByNameAndVariableLessThanAndEqual_Short()
//        {
//            // given
//            IDictionary<string, object> variables1 = new Dictionary<string, object>();
//            variables1["shortValue"] = (short) 123;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables1);

//            IDictionary<string, object> variables2 = new Dictionary<string, object>();
//            variables2["shortValue"] = (short) 999;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables2);

//            IDictionary<string, object> variables3 = new Dictionary<string, object>();
//            variables3["shortValue"] = (short) 555;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables3);

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                .VariableValueLessThanOrEqual("shortValue", (short) 999);

//            // then
//            var result = query
//                .ToList();
//            Assert.IsFalse(result.Count == 0);
//            Assert.AreEqual(3, result.Count);

//            Assert.AreEqual(3, query.Count());

//            foreach (var var in result)
//            {
//                Assert.AreEqual("shortValue", var.Name);
//                Assert.AreEqual("short", var.TypeName);
//                if (var.Value.Equals((short) 123))
//                    Assert.AreEqual((short) 123, var.Value);
//                else if (var.Value.Equals((short) 555))
//                    Assert.AreEqual((short) 555, var.Value);
//                else if (var.Value.Equals((short) 999))
//                    Assert.AreEqual((short) 999, var.Value);
//                else
//                    Assert.Fail("A non expected value occured: " + var.Value);
//            }
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByNameAndVariableValueEquals_Bytes()
//        {
//            // given
//            var bytes = "somebytes".GetBytes();
//            IDictionary<string, object> variables = new Dictionary<string, object>();
//            variables["bytesVar"] = bytes;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables);

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                //.VariableValueEquals("bytesVar", bytes);

//            // then
//            try
//            {
//                query
//                    .ToList();
//                Assert.Fail(
//                    "A ProcessEngineException was expected: Variables of type ByteArray cannot be used to query.");
//            }
//            catch (ProcessEngineException)
//            {
//                // expected exception
//            }
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByNameAndVariableValueEquals_Date()
//        {
//            // given
//            var now = DateTime.Now;

//            IDictionary<string, object> variables = new Dictionary<string, object>();
//            variables["date"] = now;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables);

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                //.VariableValueEquals("date", now);

//            // then
//            var result = query
//                .ToList();
//            Assert.IsFalse(result.Count == 0);
//            Assert.AreEqual(1, result.Count);

//            Assert.AreEqual(1, query.Count());

//            var var = result[0];
//            Assert.AreEqual("date", var.Name);
//            Assert.AreEqual(now, var.Value);
//            Assert.AreEqual("date", var.TypeName);
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByNameAndVariableValueEqualsWihtoutAnyResult()
//        {
//            // given
//            IDictionary<string, object> variables = new Dictionary<string, object>();
//            variables["stringVar"] = "test";
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables);

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                //.VariableValueEquals("stringVar", "notFoundValue");

//            // then
//            var result = query
//                .ToList();
//            Assert.True(result.Count == 0);

//            Assert.AreEqual(0, query.Count());
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByNameAndVariableValueEquals_NullValue()
//        {
//            // given
//            IDictionary<string, object> variables = new Dictionary<string, object>();
//            variables["nullValue"] = null;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables);

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                //.VariableValueEquals("nullValue", null);

//            // then
//            var result = query
//                .ToList();
//            Assert.IsFalse(result.Count == 0);
//            Assert.AreEqual(1, result.Count);

//            Assert.AreEqual(1, query.Count());

//            var var = result[0];
//            Assert.AreEqual("nullValue", var.Name);
//            Assert.AreEqual(null, var.Value);
//            Assert.AreEqual("null", var.TypeName);
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByVariableValueNotEquals_NullValue()
//        {
//            // given
//            IDictionary<string, object> variables1 = new Dictionary<string, object>();
//            variables1["value"] = null;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables1);

//            IDictionary<string, object> variables2 = new Dictionary<string, object>();
//            variables2["value"] = (short) 999;
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables2);

//            IDictionary<string, object> variables3 = new Dictionary<string, object>();
//            variables3["value"] = "abc";
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables3);

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                .VariableValueNotEquals("value", null);

//            // then
//            var result = query
//                .ToList();
//            Assert.IsFalse(result.Count == 0);
//            Assert.AreEqual(2, result.Count);

//            Assert.AreEqual(2, query.Count());

//            foreach (var var in result)
//            {
//                Assert.AreEqual("value", var.Name);
//                if (var.Value.Equals((short) 999))
//                {
//                    Assert.AreEqual((short) 999, var.Value);
//                    Assert.AreEqual("short", var.TypeName);
//                }
//                else if (var.Value.Equals("abc"))
//                {
//                    Assert.AreEqual("abc", var.Value);
//                    Assert.AreEqual("string", var.TypeName);
//                }
//                else
//                {
//                    Assert.Fail("A non expected value occured: " + var.Value);
//                }
//            }
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByProcessInstanceId()
//        {
//            // given
//            IDictionary<string, object> variables = new Dictionary<string, object>();
//            variables["stringVar"] = "test";
//            variables["myVar"] = "test123";
//            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables);

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                .ProcessInstanceIdIn(processInstance.Id);

//            // then
//            var result = query
//                .ToList();
//            Assert.IsFalse(result.Count == 0);
//            Assert.AreEqual(2, result.Count);

//            Assert.AreEqual(2, query.Count());

//            foreach (var var in result)
//            {
//                Assert.AreEqual("string", var.TypeName);
//                if (var.Name.Equals("myVar"))
//                {
//                    Assert.AreEqual("myVar", var.Name);
//                    Assert.AreEqual("test123", var.Value);
//                }
//                else if (var.Name.Equals("stringVar"))
//                {
//                    Assert.AreEqual("stringVar", var.Name);
//                    Assert.AreEqual("test", var.Value);
//                }
//                else
//                {
//                    Assert.Fail("An unexpected variable '" + var.Name + "' was found with value " + var.Value);
//                }
//            }
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByProcessInstanceIds()
//        {
//            // given
//            IDictionary<string, object> variables = new Dictionary<string, object>();
//            variables["stringVar"] = "test";
//            variables["myVar"] = "test123";
//            var processInstance1 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables);
//            var processInstance2 = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                .ProcessInstanceIdIn(processInstance1.Id, processInstance2.Id);

//            // then
//            var result = query
//                .ToList();
//            Assert.IsFalse(result.Count == 0);
//            Assert.AreEqual(2, result.Count);

//            Assert.AreEqual(2, query.Count());

//            foreach (var var in result)
//            {
//                Assert.AreEqual("string", var.TypeName);
//                if (var.Name.Equals("myVar"))
//                {
//                    Assert.AreEqual("myVar", var.Name);
//                    Assert.AreEqual("test123", var.Value);
//                }
//                else if (var.Name.Equals("stringVar"))
//                {
//                    Assert.AreEqual("stringVar", var.Name);
//                    Assert.AreEqual("test", var.Value);
//                }
//                else
//                {
//                    Assert.Fail("An unexpected variable '" + var.Name + "' was found with value " + var.Value);
//                }
//            }
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByProcessInstanceIdWithoutAnyResult()
//        {
//            // given
//            IDictionary<string, object> variables = new Dictionary<string, object>();
//            variables["stringVar"] = "test";
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables);
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess");

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                .ProcessInstanceIdIn("aProcessInstanceId");

//            // then
//            var result = query
//                .ToList();
//            Assert.True(result.Count == 0);

//            Assert.AreEqual(0, query.Count());
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByExecutionId()
//        {
//            // given
//            IDictionary<string, object> variables = new Dictionary<string, object>();
//            variables["stringVar"] = "test";
//            variables["myVar"] = "test123";
//            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables);

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                .ExecutionIdIn(processInstance.Id);

//            // then
//            var result = query
//                .ToList();
//            Assert.IsFalse(result.Count == 0);
//            Assert.AreEqual(2, result.Count);

//            Assert.AreEqual(2, query.Count());

//            foreach (var var in result)
//            {
//                Assert.AreEqual("string", var.TypeName);
//                if (var.Name.Equals("myVar"))
//                {
//                    Assert.AreEqual("myVar", var.Name);
//                    Assert.AreEqual("test123", var.Value);
//                }
//                else if (var.Name.Equals("stringVar"))
//                {
//                    Assert.AreEqual("stringVar", var.Name);
//                    Assert.AreEqual("test", var.Value);
//                }
//                else
//                {
//                    Assert.Fail("An unexpected variable '" + var.Name + "' was found with value " + var.Value);
//                }
//            }
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByExecutionIds()
//        {
//            // given
//            IDictionary<string, object> variables1 = new Dictionary<string, object>();
//            variables1["stringVar"] = "test";
//            variables1["myVar"] = "test123";
//            var processInstance1 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables1);

//            IDictionary<string, object> variables2 = new Dictionary<string, object>();
//            variables2["myVar"] = "test123";
//            var processInstance2 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables2);

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                .ExecutionIdIn(processInstance1.Id, processInstance2.Id);

//            // then
//            var result = query
//                .ToList();
//            Assert.IsFalse(result.Count == 0);
//            Assert.AreEqual(3, result.Count);

//            Assert.AreEqual(3, query.Count());

//            foreach (var var in result)
//            {
//                Assert.AreEqual("string", var.TypeName);
//                if (var.Name.Equals("myVar"))
//                {
//                    Assert.AreEqual("myVar", var.Name);
//                    Assert.AreEqual("test123", var.Value);
//                }
//                else if (var.Name.Equals("stringVar"))
//                {
//                    Assert.AreEqual("stringVar", var.Name);
//                    Assert.AreEqual("test", var.Value);
//                }
//                else
//                {
//                    Assert.Fail("An unexpected variable '" + var.Name + "' was found with value " + var.Value);
//                }
//            }
//        }
//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByExecutionIdWithoutAnyResult()
//        {
//            // given
//            IDictionary<string, object> variables = new Dictionary<string, object>();
//            variables["stringVar"] = "test";
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables);
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess");

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                .ExecutionIdIn("anExecutionId");

//            // then
//            var result = query
//                .ToList();
//            Assert.True(result.Count == 0);

//            Assert.AreEqual(0, query.Count());
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByTaskId()
//        {
//            // given
//            IDictionary<string, object> variables = new Dictionary<string, object>();
//            variables["stringVar"] = "test";
//            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables);

//            var task = taskService.CreateTaskQuery()
//                .Where(c=>c.ProcessInstanceId==processInstance.Id)
//                .First();

//            taskService.SetVariableLocal(task.Id, "taskVariable", "aCustomValue");

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                .TaskIdIn(task.Id);

//            // then
//            var result = query
//                .ToList();
//            Assert.IsFalse(result.Count == 0);
//            Assert.AreEqual(1, result.Count);

//            Assert.AreEqual(1, query.Count());

//            var var = result[0];
//            Assert.AreEqual("taskVariable", var.Name);
//            Assert.AreEqual("aCustomValue", var.Value);
//            Assert.AreEqual("string", var.TypeName);
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByTaskIds()
//        {
//            // given
//            IDictionary<string, object> variables = new Dictionary<string, object>();
//            variables["stringVar"] = "test";
//            var processInstance1 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables);
//            var processInstance2 = runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//            var processInstance3 = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

//            var task1 = taskService.CreateTaskQuery()
//                .Where(c=>c.ProcessInstanceId==processInstance1.Id)
//                .First();
//            var task2 = taskService.CreateTaskQuery()
//                .Where(c=>c.ProcessInstanceId==processInstance2.Id)
//                .First();
//            var task3 = taskService.CreateTaskQuery()
//                .Where(c=>c.ProcessInstanceId==processInstance3.Id)
//                .First();

//            taskService.SetVariableLocal(task1.Id, "taskVariable", "aCustomValue");
//            taskService.SetVariableLocal(task2.Id, "anotherTaskVariable", "aCustomValue");

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                .TaskIdIn(task1.Id, task2.Id, task3.Id);

//            // then
//            var result = query
//                .ToList();
//            Assert.IsFalse(result.Count == 0);
//            Assert.AreEqual(2, result.Count);

//            Assert.AreEqual(2, query.Count());

//            foreach (var var in result)
//            {
//                Assert.AreEqual("string", var.TypeName);
//                if (var.Name.Equals("taskVariable"))
//                {
//                    Assert.AreEqual("taskVariable", var.Name);
//                    Assert.AreEqual("aCustomValue", var.Value);
//                }
//                else if (var.Name.Equals("anotherTaskVariable"))
//                {
//                    Assert.AreEqual("anotherTaskVariable", var.Name);
//                    Assert.AreEqual("aCustomValue", var.Value);
//                }
//                else
//                {
//                    Assert.Fail("An unexpected variable '" + var.Name + "' was found with value " + var.Value);
//                }
//            }
//        }
//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByTaskIdWithoutAnyResult()
//        {
//            // given
//            IDictionary<string, object> variables = new Dictionary<string, object>();
//            variables["stringVar"] = "test";
//            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables);

//            var task = taskService.CreateTaskQuery()
//                .Where(c=>c.ProcessInstanceId==processInstance.Id)
//                .First();

//            taskService.SetVariableLocal(task.Id, "taskVariable", "aCustomValue");

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                .TaskIdIn("aTaskId");

//            // then
//            var result = query
//                .ToList();
//            Assert.True(result.Count == 0);

//            Assert.AreEqual(0, query.Count());
//        }

//        [Test][Deployment(new string[] {"resources/api/runtime/VariableInstanceQueryTest.TaskInEmbeddedSubProcess.bpmn20.xml"}) ]
//        public virtual void testQueryByVariableScopeId()
//        {
//            var processInstance = runtimeService.StartProcessInstanceByKey("testProcess");

//            var task = taskService.CreateTaskQuery()
//                .Where(c=>c.ProcessInstanceId==processInstance.Id)
//                .First();
//            Assert.NotNull(task);

//            // get variable scope ids
//            var taskId = task.Id;
//            var executionId = task.ExecutionId;
//            var ProcessInstanceId = task.ProcessInstanceId;

//            // set variables
//            var VariableName = "foo";
//            IDictionary<string, string> variables = new Dictionary<string, string>();
//            variables[taskId] = "task";
//            variables[executionId] = "execution";
//            variables[ProcessInstanceId] = "processInstance";

//            taskService.SetVariableLocal(taskId, VariableName, variables[taskId]);
//            runtimeService.SetVariableLocal(executionId, VariableName, variables[executionId]);
//            runtimeService.SetVariableLocal(ProcessInstanceId, VariableName, variables[ProcessInstanceId]);

//            IList<IVariableInstance> variableInstances;

//            // query by variable scope id
//            foreach (var variableScopeId in variables.Keys)
//            {
//                variableInstances = runtimeService.CreateVariableInstanceQuery()
//                    .VariableScopeIdIn(variableScopeId)
//                    
//                    .ToList();
//                Assert.AreEqual(1, variableInstances.Count);
//                Assert.AreEqual(VariableName, variableInstances[0].Name);
//                Assert.AreEqual(variables[variableScopeId], variableInstances[0].Value);
//            }

//            // query by multiple variable scope ids
//            variableInstances = runtimeService.CreateVariableInstanceQuery()
//                .VariableScopeIdIn(taskId, executionId, ProcessInstanceId)
//                
//                .ToList();
//            Assert.AreEqual(3, variableInstances.Count);

//            // remove task variable
//            taskService.RemoveVariableLocal(taskId, VariableName);

//            variableInstances = runtimeService.CreateVariableInstanceQuery()
//                .VariableScopeIdIn(taskId)
//                
//                .ToList();
//            Assert.AreEqual(0, variableInstances.Count);

//            variableInstances = runtimeService.CreateVariableInstanceQuery()
//                .VariableScopeIdIn(taskId, executionId, ProcessInstanceId)
//                
//                .ToList();
//            Assert.AreEqual(2, variableInstances.Count);

//            // remove process instance variable variable
//            runtimeService.RemoveVariable(ProcessInstanceId, VariableName);

//            variableInstances = runtimeService.CreateVariableInstanceQuery()
//                .VariableScopeIdIn(ProcessInstanceId, taskId)
//                
//                .ToList();
//            Assert.AreEqual(0, variableInstances.Count);

//            variableInstances = runtimeService.CreateVariableInstanceQuery()
//                .VariableScopeIdIn(taskId, executionId, ProcessInstanceId)
//                
//                .ToList();
//            Assert.AreEqual(1, variableInstances.Count);

//            // remove execution variable
//            runtimeService.RemoveVariable(executionId, VariableName);

//            variableInstances = runtimeService.CreateVariableInstanceQuery()
//                .VariableScopeIdIn(taskId, executionId, ProcessInstanceId)
//                
//                .ToList();
//            Assert.AreEqual(0, variableInstances.Count);
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByActivityInstanceId()
//        {
//            // given
//            IDictionary<string, object> variables = new Dictionary<string, object>();
//            variables["stringVar"] = "test";
//            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables);
//            var activityId = runtimeService.GetActivityInstance(processInstance.Id)
//                .ChildActivityInstances[0].Id;

//            var task = taskService.CreateTaskQuery()
//                .Where(c=>c.ProcessInstanceId==processInstance.Id)
//                .First();
//            taskService.SetVariableLocal(task.Id, "taskVariable", "aCustomValue");

//            // when
//            var taskVariablesQuery = runtimeService.CreateVariableInstanceQuery()
//                .ActivityInstanceIdIn(activityId);
//            var processVariablesQuery = runtimeService.CreateVariableInstanceQuery()
//                .ActivityInstanceIdIn(processInstance.Id);

//            // then
//            var taskVar = taskVariablesQuery.First();
//            Assert.NotNull(taskVar);

//            Assert.AreEqual(1, taskVariablesQuery.Count());
//            Assert.AreEqual("string", taskVar.TypeName);
//            Assert.AreEqual("taskVariable", taskVar.Name);
//            Assert.AreEqual("aCustomValue", taskVar.Value);

//            var processVar = processVariablesQuery.First();

//            Assert.AreEqual(1, processVariablesQuery.Count());
//            Assert.AreEqual("string", processVar.TypeName);
//            Assert.AreEqual("stringVar", processVar.Name);
//            Assert.AreEqual("test", processVar.Value);
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryByActivityInstanceIds()
//        {
//            // given
//            IDictionary<string, object> variables1 = new Dictionary<string, object>();
//            variables1["stringVar"] = "test";
//            variables1["myVar"] = "test123";
//            var procInst1 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables1);

//            IDictionary<string, object> variables2 = new Dictionary<string, object>();
//            variables2["myVar"] = "test123";
//            var procInst2 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables2);

//            IDictionary<string, object> variables3 = new Dictionary<string, object>();
//            variables3["myVar"] = "test123";
//            var procInst3 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables3);

//            var task1 = taskService.CreateTaskQuery()
//                .Where(c=>c.ProcessInstanceId==procInst1.Id)
//                .First();
//            var task2 = taskService.CreateTaskQuery()
//                .Where(c=>c.ProcessInstanceId==procInst2.Id)
//                .First();

//            taskService.SetVariableLocal(task1.Id, "taskVariable", "aCustomValue");
//            taskService.SetVariableLocal(task2.Id, "anotherTaskVariable", "aCustomValue");

//            // when
//            var processVariablesQuery = runtimeService.CreateVariableInstanceQuery()
//                .ActivityInstanceIdIn(procInst1.Id, procInst2.Id, procInst3.Id);

//            var taskVariablesQuery = runtimeService.CreateVariableInstanceQuery()
//                .ActivityInstanceIdIn(runtimeService.GetActivityInstance(procInst1.Id)
//                    .ChildActivityInstances[0].Id, runtimeService.GetActivityInstance(procInst2.Id)
//                    .ChildActivityInstances[0].Id);

//            // then (process variables)
//            var result = processVariablesQuery
//                .ToList();
//            Assert.IsFalse(result.Count == 0);
//            Assert.AreEqual(4, result.Count);

//            Assert.AreEqual(4, processVariablesQuery.Count());

//            foreach (var var in result)
//            {
//                Assert.AreEqual("string", var.TypeName);
//                if (var.Name.Equals("myVar"))
//                {
//                    Assert.AreEqual("myVar", var.Name);
//                    Assert.AreEqual("test123", var.Value);
//                }
//                else if (var.Name.Equals("stringVar"))
//                {
//                    Assert.AreEqual("stringVar", var.Name);
//                    Assert.AreEqual("test", var.Value);
//                }
//                else
//                {
//                    Assert.Fail("An unexpected variable '" + var.Name + "' was found with value " + var.Value);
//                }
//            }

//            // then (task variables)
//            result = taskVariablesQuery
//                .ToList();
//            Assert.IsFalse(result.Count == 0);
//            Assert.AreEqual(2, result.Count);

//            Assert.AreEqual(2, taskVariablesQuery.Count());

//            foreach (var var in result)
//            {
//                Assert.AreEqual("string", var.TypeName);
//                if (var.Name.Equals("taskVariable"))
//                {
//                    Assert.AreEqual("taskVariable", var.Name);
//                    Assert.AreEqual("aCustomValue", var.Value);
//                }
//                else if (var.Name.Equals("anotherTaskVariable"))
//                {
//                    Assert.AreEqual("anotherTaskVariable", var.Name);
//                    Assert.AreEqual("aCustomValue", var.Value);
//                }
//                else
//                {
//                    Assert.Fail("An unexpected variable '" + var.Name + "' was found with value " + var.Value);
//                }
//            }
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryOrderByName_Asc()
//        {
//            // given
//            IDictionary<string, object> variables = new Dictionary<string, object>();
//            variables["stringVar"] = "test";
//            variables["myVar"] = "test123";
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables);

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                /*.OrderByVariableName()*/
//                /*.Asc()*/;

//            // then
//            var result = query
//                .ToList();
//            Assert.IsFalse(result.Count == 0);
//            Assert.AreEqual(2, result.Count);

//            var first = result[0];
//            var second = result[1];

//            Assert.AreEqual("myVar", first.Name);
//            Assert.AreEqual("stringVar", second.Name);
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryOrderByName_Desc()
//        {
//            // given
//            IDictionary<string, object> variables = new Dictionary<string, object>();
//            variables["stringVar"] = "test";
//            variables["myVar"] = "test123";
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables);

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                /*.OrderByVariableName()*/
//                /*.Desc()*/;

//            // then
//            var result = query
//                .ToList();
//            Assert.IsFalse(result.Count == 0);
//            Assert.AreEqual(2, result.Count);

//            var first = result[0];
//            var second = result[1];

//            Assert.AreEqual("stringVar", first.Name);
//            Assert.AreEqual("string", first.TypeName);
//            Assert.AreEqual("myVar", second.Name);
//            Assert.AreEqual("string", second.TypeName);
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryOrderByType_Asc()
//        {
//            // given
//            IDictionary<string, object> variables = new Dictionary<string, object>();
//            variables["intVar"] = 123;
//            variables["myVar"] = "test123";
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables);

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                .OrderByVariableType()
//                /*.Asc()*/;

//            // then
//            var result = query
//                .ToList();
//            Assert.IsFalse(result.Count == 0);
//            Assert.AreEqual(2, result.Count);

//            var first = result[0];
//            var second = result[1];

//            Assert.AreEqual("intVar", first.Name); // integer
//            Assert.AreEqual("integer", first.TypeName);
//            Assert.AreEqual("myVar", second.Name); // string
//            Assert.AreEqual("string", second.TypeName);
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryOrderByType_Desc()
//        {
//            // given
//            IDictionary<string, object> variables = new Dictionary<string, object>();
//            variables["intVar"] = 123;
//            variables["myVar"] = "test123";
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables);

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                .OrderByVariableType()
//                /*.Desc()*/;

//            // then
//            var result = query
//                .ToList();
//            Assert.IsFalse(result.Count == 0);
//            Assert.AreEqual(2, result.Count);

//            var first = result[0];
//            var second = result[1];

//            Assert.AreEqual("myVar", first.Name); // string
//            Assert.AreEqual("string", first.TypeName);
//            Assert.AreEqual("intVar", second.Name); // integer
//            Assert.AreEqual("integer", second.TypeName);
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryOrderByActivityInstanceId_Asc()
//        {
//            // given
//            IDictionary<string, object> variables1 = new Dictionary<string, object>();
//            variables1["intVar"] = 123;
//            var procInst1 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables1);
//            var activityId1 = runtimeService.GetActivityInstance(procInst1.Id)
//                .ChildActivityInstances[0].Id;

//            IDictionary<string, object> variables2 = new Dictionary<string, object>();
//            variables2["stringVar"] = "test";
//            var procInst2 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables2);
//            var activityId2 = runtimeService.GetActivityInstance(procInst2.Id)
//                .ChildActivityInstances[0].Id;

//            var comparisonResult = activityId1.CompareTo(activityId2);

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                .OrderByActivityInstanceId()
//                /*.Asc()*/;

//            // then
//            var result = query
//                .ToList();
//            Assert.IsFalse(result.Count == 0);
//            Assert.AreEqual(2, result.Count);

//            var first = result[0];
//            var second = result[1];

//            if (comparisonResult < 0)
//            {
//                Assert.AreEqual("intVar", first.Name);
//                Assert.AreEqual("integer", first.TypeName);
//                Assert.AreEqual("stringVar", second.Name);
//                Assert.AreEqual("string", second.TypeName);
//            }
//            else if (comparisonResult > 0)
//            {
//                Assert.AreEqual("stringVar", first.Name);
//                Assert.AreEqual("string", first.TypeName);
//                Assert.AreEqual("intVar", second.Name);
//                Assert.AreEqual("integer", second.TypeName);
//            }
//            else
//            {
//                Assert.Fail("Something went wrong: both activity instances have the same id " + activityId1 + " and " +
//                            activityId2);
//            }
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testQueryOrderByActivityInstanceId_Desc()
//        {
//            // given
//            IDictionary<string, object> variables1 = new Dictionary<string, object>();
//            variables1["intVar"] = 123;
//            var procInst1 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables1);

//            IDictionary<string, object> variables2 = new Dictionary<string, object>();
//            variables2["stringVar"] = "test";
//            var procInst2 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables2);

//            var activityId1 = runtimeService.GetActivityInstance(procInst1.Id)
//                .ChildActivityInstances[0].Id;
//            var activityId2 = runtimeService.GetActivityInstance(procInst2.Id)
//                .ChildActivityInstances[0].Id;

//            var comparisonResult = activityId1.CompareTo(activityId2);
//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                .OrderByActivityInstanceId()
//                /*.Desc()*/;

//            // then
//            var result = query
//                .ToList();
//            Assert.IsFalse(result.Count == 0);
//            Assert.AreEqual(2, result.Count);

//            var first = result[0];
//            var second = result[1];

//            if (comparisonResult < 0)
//            {
//                Assert.AreEqual("stringVar", first.Name);
//                Assert.AreEqual("string", first.TypeName);
//                Assert.AreEqual("intVar", second.Name);
//                Assert.AreEqual("integer", second.TypeName);
//            }
//            else if (comparisonResult > 0)
//            {
//                Assert.AreEqual("intVar", first.Name);
//                Assert.AreEqual("integer", first.TypeName);
//                Assert.AreEqual("stringVar", second.Name);
//                Assert.AreEqual("string", second.TypeName);
//            }
//            else
//            {
//                Assert.Fail("Something went wrong: both activity instances have the same id " + activityId1 + " and " +
//                            activityId2);
//            }
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
//        public virtual void testGetValueOfSerializableVar()
//        {
//            // given
//            IList<string> serializable = new List<string>();
//            serializable.Add("one");
//            serializable.Add("two");
//            serializable.Add("three");

//            IDictionary<string, object> variables = new Dictionary<string, object>();
//            variables["serializableVar"] = serializable;
//            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables);

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                .ProcessInstanceIdIn(processInstance.Id);

//            // then
//            var result = query
//                .ToList();
//            Assert.IsFalse(result.Count == 0);
//            Assert.AreEqual(1, result.Count);

//            var instance = result[0];

//            Assert.AreEqual("serializableVar", instance.Name);
//            Assert.NotNull(instance.Value);
//            Assert.AreEqual(serializable, instance.Value);
//            Assert.AreEqual(ValueTypeFields.Object.Name, instance.TypeName);
//        }


//        [Test]
//        [Deployment]
//        public virtual void testSubProcessVariablesWithParallelGateway()
//        {
//            // given
//            var processInstance = runtimeService.StartProcessInstanceByKey("processWithSubProcess");

//            var tree = runtimeService.GetActivityInstance(processInstance.Id);
//            Assert.NotNull(tree);
//            var subprocessInstances = tree.GetActivityInstances("SubProcess_1");
//            Assert.AreEqual(5, subprocessInstances.Length);

//            //when
//            var activityInstanceId1 = subprocessInstances[0].Id;
//            var query1 = runtimeService.CreateVariableInstanceQuery()
//                .ActivityInstanceIdIn(activityInstanceId1);

//            var activityInstanceId2 = subprocessInstances[1].Id;
//            var query2 = runtimeService.CreateVariableInstanceQuery()
//                .ActivityInstanceIdIn(activityInstanceId2);

//            var activityInstanceId3 = subprocessInstances[2].Id;
//            var query3 = runtimeService.CreateVariableInstanceQuery()
//                .ActivityInstanceIdIn(activityInstanceId3);

//            var activityInstanceId4 = subprocessInstances[3].Id;
//            var query4 = runtimeService.CreateVariableInstanceQuery()
//                .ActivityInstanceIdIn(activityInstanceId4);

//            var activityInstanceId5 = subprocessInstances[4].Id;
//            var query5 = runtimeService.CreateVariableInstanceQuery()
//                .ActivityInstanceIdIn(activityInstanceId5);

//            // then
//            checkVariables(query1
//                .ToList());
//            checkVariables(query2
//                .ToList());
//            checkVariables(query3
//                .ToList());
//            checkVariables(query4
//                .ToList());
//            checkVariables(query5
//                .ToList());
//        }

//        private void checkVariables(IList<IVariableInstance> variableInstances)
//        {
//            Assert.IsFalse(variableInstances.Count == 0);
//            foreach (var instance in variableInstances)
//                if (instance.Name.Equals("nrOfInstances"))
//                {
//                    Assert.AreEqual("nrOfInstances", instance.Name);
//                    Assert.AreEqual("integer", instance.TypeName);
//                }
//                else if (instance.Name.Equals("nrOfCompletedInstances"))
//                {
//                    Assert.AreEqual("nrOfCompletedInstances", instance.Name);
//                    Assert.AreEqual("integer", instance.TypeName);
//                }
//                else if (instance.Name.Equals("nrOfActiveInstances"))
//                {
//                    Assert.AreEqual("nrOfActiveInstances", instance.Name);
//                    Assert.AreEqual("integer", instance.TypeName);
//                }
//                else if (instance.Name.Equals("loopCounter"))
//                {
//                    Assert.AreEqual("loopCounter", instance.Name);
//                    Assert.AreEqual("integer", instance.TypeName);
//                }
//                else if (instance.Name.Equals("nullVar"))
//                {
//                    Assert.AreEqual("nullVar", instance.Name);
//                    Assert.AreEqual("null", instance.TypeName);
//                }
//                else if (instance.Name.Equals("integerVar"))
//                {
//                    Assert.AreEqual("integerVar", instance.Name);
//                    Assert.AreEqual("integer", instance.TypeName);
//                }
//                else if (instance.Name.Equals("dateVar"))
//                {
//                    Assert.AreEqual("dateVar", instance.Name);
//                    Assert.AreEqual("date", instance.TypeName);
//                }
//                else if (instance.Name.Equals("stringVar"))
//                {
//                    Assert.AreEqual("stringVar", instance.Name);
//                    Assert.AreEqual("string", instance.TypeName);
//                }
//                else if (instance.Name.Equals("shortVar"))
//                {
//                    Assert.AreEqual("shortVar", instance.Name);
//                    Assert.AreEqual("short", instance.TypeName);
//                }
//                else if (instance.Name.Equals("longVar"))
//                {
//                    Assert.AreEqual("longVar", instance.Name);
//                    Assert.AreEqual("long", instance.TypeName);
//                }
//                else if (instance.Name.Equals("byteVar"))
//                {
//                    Assert.AreEqual("bytes", instance.TypeName);
//                }
//                else if (instance.Name.Equals("serializableVar"))
//                {
//                    Assert.AreEqual("serializableVar", instance.Name);
//                    try
//                    {
//                        var instanceValue = instance.Value;
//                    }
//                    catch (NullReferenceException)
//                    {
//                        // the serialized value has not been initially loaded
//                    }
//                }
//                else
//                {
//                    Assert.Fail("An unexpected variable '" + instance.Name + "' was found with value " + instance.Value);
//                }
//        }

//        [Test]
//        [Deployment]
//        public virtual void testSubProcessVariables()
//        {
//            // given
//            IDictionary<string, object> processVariables = new Dictionary<string, object>();
//            processVariables["processVariable"] = "aProcessVariable";
//            var processInstance = runtimeService.StartProcessInstanceByKey("processWithSubProcess", processVariables);

//            var tree = runtimeService.GetActivityInstance(processInstance.Id);
//            Assert.NotNull(tree);
//            Assert.AreEqual(1, tree.ChildActivityInstances.Length);

//            // when
//            var query1 = runtimeService.CreateVariableInstanceQuery()
//                .ActivityInstanceIdIn(tree.Id);

//            // then
//            var processVariable = query1.First();
//            Assert.NotNull(processVariable);
//            Assert.AreEqual("processVariable", processVariable.Name);
//            Assert.AreEqual("aProcessVariable", processVariable.Value);

//            // when
//            var subProcessActivityInstance = tree.GetActivityInstances("SubProcess_1")[0];
//            var query2 = runtimeService.CreateVariableInstanceQuery()
//                .ActivityInstanceIdIn(subProcessActivityInstance.Id);

//            // then
//            checkVariables(query2
//                .ToList());

//            // when setting a task local variable
//            var task = taskService.CreateTaskQuery()
//                .First();
//            taskService.SetVariableLocal(task.Id, "taskVariable", "taskVariableValue");

//            // skip mi body instance
//            var taskActivityInstance = subProcessActivityInstance.ChildActivityInstances[0];
//            var query3 = runtimeService.CreateVariableInstanceQuery()
//                .ActivityInstanceIdIn(taskActivityInstance.Id);

//            // then
//            var variables = query3
//                .ToList();
//            var taskVariable = query3.First();
//            Assert.NotNull(taskVariable);
//            Assert.AreEqual("taskVariable", taskVariable.Name);
//            Assert.AreEqual("taskVariableValue", taskVariable.Value);
//        }
//        [Test]
//        [Deployment]
//        public virtual void testParallelGatewayVariables()
//        {
//            // given
//            IDictionary<string, object> processVariables = new Dictionary<string, object>();
//            processVariables["processVariable"] = "aProcessVariable";
//            var processInstance = runtimeService.StartProcessInstanceByKey("parallelGatewayProcess", processVariables);

//            var execution = runtimeService.CreateExecutionQuery()
//                .ActivityId("task1")
//                .First();
//            runtimeService.SetVariableLocal(execution.Id, "aLocalVariable", "aLocalValue");

//            var tree = runtimeService.GetActivityInstance(processInstance.Id);
//            Assert.AreEqual(2, tree.ChildActivityInstances.Length);
//            var task1Instance = tree.GetActivityInstances("task1")[0];

//            var query = runtimeService.CreateVariableInstanceQuery()
//                /*.VariableName("aLocalVariable")*/
//                .ActivityInstanceIdIn(task1Instance.Id);
//            var localVariable = query.First();
//            Assert.NotNull(localVariable);
//            Assert.AreEqual("aLocalVariable", localVariable.Name);
//            Assert.AreEqual("aLocalValue", localVariable.Value);

//            var task = taskService.CreateTaskQuery()
//                .ExecutionId(execution.Id)
//                .First();
//            taskService.Complete(task.Id);

//            tree = runtimeService.GetActivityInstance(processInstance.Id);
//            Assert.AreEqual(2, tree.ChildActivityInstances.Length);
//            var task3Instance = tree.GetActivityInstances("task3")[0];

//            query = runtimeService.CreateVariableInstanceQuery()
//                /*.VariableName("aLocalVariable")*/
//                .ActivityInstanceIdIn(task3Instance.Id);
//            localVariable = query.First();
//            Assert.NotNull(localVariable);
//            Assert.AreEqual("aLocalVariable", localVariable.Name);
//            Assert.AreEqual("aLocalValue", localVariable.Value);
//        }

//        [Test]
//        [Deployment]
//        public virtual void testSimpleSubProcessVariables()
//        {
//            // given
//            IDictionary<string, object> processVariables = new Dictionary<string, object>();
//            processVariables["processVariable"] = "aProcessVariable";
//            var processInstance = runtimeService.StartProcessInstanceByKey("processWithSubProcess", processVariables);

//            var task = taskService.CreateTaskQuery()
//                .TaskDefinitionKey("UserTask_1")
//                .First();
//            runtimeService.SetVariableLocal(task.ExecutionId, "aLocalVariable", "aLocalValue");

//            var tree = runtimeService.GetActivityInstance(processInstance.Id);
//            Assert.AreEqual(1, tree.ChildActivityInstances.Length);
//            var subProcessInstance = tree.GetActivityInstances("SubProcess_1")[0];

//            // then the local variable has activity instance Id of the subprocess
//            var query = runtimeService.CreateVariableInstanceQuery()
//                .ActivityInstanceIdIn(subProcessInstance.Id);
//            var localVariable = query.First();
//            Assert.NotNull(localVariable);
//            Assert.AreEqual("aLocalVariable", localVariable.Name);
//            Assert.AreEqual("aLocalValue", localVariable.Value);

//            // and the global variable has the activity instance Id of the process instance:
//            query = runtimeService.CreateVariableInstanceQuery()
//                .ActivityInstanceIdIn(processInstance.Id);
//            var globalVariable = query.First();
//            Assert.NotNull(localVariable);
//            Assert.AreEqual("processVariable", globalVariable.Name);
//            Assert.AreEqual("aProcessVariable", globalVariable.Value);

//            taskService.Complete(task.Id);
//        }

//        [Test]
//        public virtual void testDisableBinaryFetching()
//        {
//            var binaryContent = "some binary content".GetBytes();

//            // given
//            IDictionary<string, object> variables = new Dictionary<string, object>();
//            variables["binaryVariable"] = binaryContent;
//            var task = taskService.NewTask();
//            taskService.SaveTask(task);
//            taskService.SetVariablesLocal(task.Id, variables);

//            // when binary fetching is enabled (default)
//            var query = runtimeService.CreateVariableInstanceQuery();

//            // then value is fetched
//            var result = query.First();
//            Assert.NotNull(result.Value);

//            // when binary fetching is disabled
//            query = runtimeService.CreateVariableInstanceQuery()
//                /*.DisableBinaryFetching()*/;

//            // then value is not fetched
//            result = query.First();
//            Assert.IsNull(result.Value);

//            // Delete task
//            taskService.DeleteTask(task.Id, true);
//        }

//        [Test][Deployment("resources/api/runtime/oneTaskProcess.bpmn20.xml") ]
//        public virtual void testDisableBinaryFetchingForFileValues()
//        {
//            // given
//            var fileName = "text.Txt";
//            var encoding = "crazy-encoding";
//            var mimeType = "martini/dry";

//            //FileValue fileValue = Variables.FileValue(fileName)
//            //    .File("ABC".GetBytes())
//            //    .Encoding(encoding)
//            //    .MimeType(mimeType)
//            //    .Create();

//            //runtimeService.StartProcessInstanceByKey("oneTaskProcess", Variable.Variables.CreateVariables()
//            //    .PutValueTyped("fileVar", fileValue));

//            // when enabling binary fetching
//            var fileVariableInstance = runtimeService.CreateVariableInstanceQuery()
//                .First();

//            // then the binary value is accessible
//            Assert.NotNull(fileVariableInstance.Value);

//            // when disabling binary fetching
//            fileVariableInstance = runtimeService.CreateVariableInstanceQuery()
//                /*.DisableBinaryFetching()*/
//                .First();

//            // then the byte value is not fetched
//            Assert.NotNull(fileVariableInstance);
//            Assert.AreEqual("fileVar", fileVariableInstance.Name);

//            Assert.IsNull(fileVariableInstance.Value);

//            var typedValue = (IFileValue) fileVariableInstance.TypedValue;
//            Assert.IsNull(typedValue.Value);

//            // but typed value metadata is accessible
//            //Assert.AreEqual(ValueType.FILE, typedValue.Type);
//            Assert.AreEqual(fileName, typedValue.Filename);
//            Assert.AreEqual(encoding, typedValue.Encoding);
//            Assert.AreEqual(mimeType, typedValue.MimeType);
//        }

//        [Test]
//        public virtual void testDisableCustomObjectDeserialization()
//        {
//            // given
//            IDictionary<string, object> variables = new Dictionary<string, object>();
//            variables["customSerializable"] = new CustomSerializable();
//            variables["failingSerializable"] = new FailingSerializable();
//            var task = taskService.NewTask();
//            taskService.SaveTask(task);
//            taskService.SetVariablesLocal(task.Id, variables);

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery()
//                .DisableCustomObjectDeserialization();

//            // then
//            var results = query
//                .ToList();

//            // both variables are not deserialized, but their serialized values are available
//            Assert.AreEqual(2, results.Count);

//            foreach (var variableInstance in results)
//            {
//                Assert.IsNull(variableInstance.ErrorMessage);

//                var typedValue = (IObjectValue) variableInstance.TypedValue;
//                Assert.NotNull(typedValue);
//                Assert.IsFalse(typedValue.Deserialized);
//                // cannot access the deserialized value
//                try
//                {
//                    var typedValueValue = typedValue.Value;
//                }
//                catch (InvalidOperationException e)
//                {
//                    AssertTextPresent("Object is not deserialized", e.Message);
//                }
//                Assert.NotNull(typedValue.ValueSerialized);
//            }

//            // Delete task
//            taskService.DeleteTask(task.Id, true);
//        }

//        [Test]
//        public virtual void testSerializableErrorMessage()
//        {
//            // given
//            IDictionary<string, object> variables = new Dictionary<string, object>();
//            variables["customSerializable"] = new CustomSerializable();
//            variables["failingSerializable"] = new FailingSerializable();
//            var task = taskService.NewTask();
//            taskService.SaveTask(task);
//            taskService.SetVariablesLocal(task.Id, variables);

//            // when
//            var query = runtimeService.CreateVariableInstanceQuery();

//            // then
//            var results = query
//                .ToList();

//            // both variables are fetched
//            Assert.AreEqual(2, results.Count);

//            foreach (var variableInstance in results)
//            {
//                if (variableInstance.Name.Equals("customSerializable"))
//                {
//                    Assert.NotNull(variableInstance.Value);
//                    Assert.True(variableInstance.Value is CustomSerializable);
//                }
//                if (variableInstance.Name.Equals("failingSerializable"))
//                {
//                    // no value was fetched
//                    Assert.IsNull(variableInstance.Value);
//                    // error message is present
//                    Assert.NotNull(variableInstance.ErrorMessage);
//                }
//            }

//            // Delete task
//            taskService.DeleteTask(task.Id, true);
//        }

//        [Test][Deployment(new string[] {"resources/api/cmmn/oneTaskCase.cmmn"})]
//        public virtual void testQueryByCaseExecutionId()
//        {
//            var instance = caseService.WithCaseDefinitionByKey("oneTaskCase")
//                .SetVariable("aVariableName", "abc")
//                .Create();

//            var query = runtimeService.CreateVariableInstanceQuery();

//            query.CaseExecutionIdIn(instance.Id);

//            var result = query.First();

//            Assert.NotNull(result);

//            Assert.AreEqual("aVariableName", result.Name);
//            Assert.AreEqual("abc", result.Value);
//            Assert.AreEqual(instance.Id, result.CaseExecutionId);
//            Assert.AreEqual(instance.Id, result.CaseInstanceId);

//            Assert.IsNull(result.ExecutionId);
//            Assert.IsNull(result.ProcessInstanceId);
//        }

//        [Test][Deployment(new string[] {"resources/api/cmmn/oneTaskCase.cmmn"})]
//        public virtual void testQueryByCaseExecutionIds()
//        {
//            var instance1 = caseService.WithCaseDefinitionByKey("oneTaskCase")
//                .SetVariable("aVariableName", "abc")
//                .Create();

//            var instance2 = caseService.WithCaseDefinitionByKey("oneTaskCase")
//                .SetVariable("anotherVariableName", "xyz")
//                .Create();

//            var query = runtimeService.CreateVariableInstanceQuery();

//            query.CaseExecutionIdIn(instance1.Id, instance2.Id)
//                /*.OrderByVariableName()*/
//                /*.Asc()*/;

//            var result = query
//                .ToList();

//            Assert.AreEqual(2, result.Count);

//            foreach (var variableInstance in result)
//                if (variableInstance.Name.Equals("aVariableName"))
//                {
//                    Assert.AreEqual("aVariableName", variableInstance.Name);
//                    Assert.AreEqual("abc", variableInstance.Value);
//                }
//                else if (variableInstance.Name.Equals("anotherVariableName"))
//                {
//                    Assert.AreEqual("anotherVariableName", variableInstance.Name);
//                    Assert.AreEqual("xyz", variableInstance.Value);
//                }
//                else
//                {
//                    Assert.Fail("Unexpected variable: " + variableInstance.Name);
//                }
//        }

//        [Test][Deployment(new string[] {"resources/api/cmmn/oneTaskCase.cmmn"}) ]
//        public virtual void testQueryByCaseInstanceId()
//        {
//            var instance = caseService.WithCaseDefinitionByKey("oneTaskCase")
//                .SetVariable("aVariableName", "abc")
//                .Create();

//            var query = runtimeService.CreateVariableInstanceQuery();

//            query.CaseInstanceIdIn(instance.Id);

//            var result = query.First();

//            Assert.NotNull(result);

//            Assert.AreEqual("aVariableName", result.Name);
//            Assert.AreEqual("abc", result.Value);
//            Assert.AreEqual(instance.Id, result.CaseExecutionId);
//            Assert.AreEqual(instance.Id, result.CaseInstanceId);

//            Assert.IsNull(result.ExecutionId);
//            Assert.IsNull(result.ProcessInstanceId);
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/cmmn/oneTaskCase.cmmn" })]
//        public virtual void testQueryByCaseInstanceIds()
//        {
//            var instance1 = caseService.WithCaseDefinitionByKey("oneTaskCase")
//                .SetVariable("aVariableName", "abc")
//                .Create();

//            var instance2 = caseService.WithCaseDefinitionByKey("oneTaskCase")
//                .SetVariable("anotherVariableName", "xyz")
//                .Create();

//            var query = runtimeService.CreateVariableInstanceQuery();

//            query.CaseInstanceIdIn(instance1.Id, instance2.Id)
//                /*.OrderByVariableName()*/
//                /*.Asc()*/;

//            var result = query
//                .ToList();

//            Assert.AreEqual(2, result.Count);

//            foreach (var variableInstance in result)
//                if (variableInstance.Name.Equals("aVariableName"))
//                {
//                    Assert.AreEqual("aVariableName", variableInstance.Name);
//                    Assert.AreEqual("abc", variableInstance.Value);
//                }
//                else if (variableInstance.Name.Equals("anotherVariableName"))
//                {
//                    Assert.AreEqual("anotherVariableName", variableInstance.Name);
//                    Assert.AreEqual("xyz", variableInstance.Value);
//                }
//                else
//                {
//                    Assert.Fail("Unexpected variable: " + variableInstance.Name);
//                }
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/cmmn/oneTaskCase.cmmn" })]
//        public virtual void testQueryByCaseActivityInstanceId()
//        {
//            var instance = caseService.WithCaseDefinitionByKey("oneTaskCase")
//                .SetVariable("aVariableName", "abc")
//                .Create();

//            var query = runtimeService.CreateVariableInstanceQuery();

//            query.ActivityInstanceIdIn(instance.Id);

//            var result = query.First();

//            Assert.NotNull(result);

//            Assert.AreEqual("aVariableName", result.Name);
//            Assert.AreEqual("abc", result.Value);
//            Assert.AreEqual(instance.Id, result.CaseExecutionId);
//            Assert.AreEqual(instance.Id, result.CaseInstanceId);

//            Assert.IsNull(result.ExecutionId);
//            Assert.IsNull(result.ProcessInstanceId);
//        }

//        [Test]
//        [Deployment(new string[] { "resources/api/cmmn/oneTaskCase.cmmn" })]
//        public virtual void testQueryByCaseActivityInstanceIds()
//        {
//            var instance1 = caseService.WithCaseDefinitionByKey("oneTaskCase")
//                .SetVariable("aVariableName", "abc")
//                .Create();

//            var instance2 = caseService.WithCaseDefinitionByKey("oneTaskCase")
//                .SetVariable("anotherVariableName", "xyz")
//                .Create();

//            var query = runtimeService.CreateVariableInstanceQuery();

//            query.ActivityInstanceIdIn(instance1.Id, instance2.Id)
//                /*.OrderByVariableName()*/
//                /*.Asc()*/;
//            // activityInstanceId == caseExecutionId

//            var result = query
//                .ToList();

//            Assert.AreEqual(2, result.Count);

//            foreach (var variableInstance in result)
//                if (variableInstance.Name.Equals("aVariableName"))
//                {
//                    Assert.AreEqual("aVariableName", variableInstance.Name);
//                    Assert.AreEqual("abc", variableInstance.Value);
//                }
//                else if (variableInstance.Name.Equals("anotherVariableName"))
//                {
//                    Assert.AreEqual("anotherVariableName", variableInstance.Name);
//                    Assert.AreEqual("xyz", variableInstance.Value);
//                }
//                else
//                {
//                    Assert.Fail("Unexpected variable: " + variableInstance.Name);
//                }
//        }


//        [Test]
//        [Deployment]
//        public virtual void testSequentialMultiInstanceSubProcess()
//        {
//            // given a process instance in sequential MI
//            var instance = runtimeService.StartProcessInstanceByKey("miSequentialSubprocess");

//            // when
//            var nrOfInstances = runtimeService.CreateVariableInstanceQuery()
//                //.VariableName("nrOfInstances")
//                .First();
//            var nrOfActiveInstances = runtimeService.CreateVariableInstanceQuery()
//                //.VariableName("nrOfActiveInstances")
//                .First();
//            var nrOfCompletedInstances = runtimeService.CreateVariableInstanceQuery()
//                //.VariableName("nrOfCompletedInstances")
//                .First();
//            var loopCounter = runtimeService.CreateVariableInstanceQuery()
//                //.VariableName("loopCounter")
//                .First();

//            // then the activity instance ids of the variable instances should be correct
//            var tree = runtimeService.GetActivityInstance(instance.Id);
//            Assert.AreEqual(tree.GetActivityInstances("miSubProcess#multiInstanceBody")[0].Id,
//                nrOfInstances.ActivityInstanceId);
//            Assert.AreEqual(tree.GetActivityInstances("miSubProcess#multiInstanceBody")[0].Id,
//                nrOfActiveInstances.ActivityInstanceId);
//            Assert.AreEqual(tree.GetActivityInstances("miSubProcess#multiInstanceBody")[0].Id,
//                nrOfCompletedInstances.ActivityInstanceId);
//            Assert.AreEqual(tree.GetActivityInstances("miSubProcess#multiInstanceBody")[0].Id,
//                loopCounter.ActivityInstanceId);
//        }

//        [Test]
//        [Deployment]
//        public virtual void testParallelMultiInstanceSubProcess()
//        {
//            // given a process instance in sequential MI
//            var instance = runtimeService.StartProcessInstanceByKey("miSequentialSubprocess");

//            // when
//            var nrOfInstances = runtimeService.CreateVariableInstanceQuery()
//                //.VariableName("nrOfInstances")
//                .First();
//            var nrOfActiveInstances = runtimeService.CreateVariableInstanceQuery()
//                //.VariableName("nrOfActiveInstances")
//                .First();
//            var nrOfCompletedInstances = runtimeService.CreateVariableInstanceQuery()
//                //.VariableName("nrOfCompletedInstances")
//                .First();
//            var loopCounters = runtimeService.CreateVariableInstanceQuery()
//                //.VariableName("loopCounter")
//                
//                .ToList();

//            // then the activity instance ids of the variable instances should be correct
//            var tree = runtimeService.GetActivityInstance(instance.Id);
//            Assert.AreEqual(tree.GetActivityInstances("miSubProcess#multiInstanceBody")[0].Id,
//                nrOfInstances.ActivityInstanceId);
//            Assert.AreEqual(tree.GetActivityInstances("miSubProcess#multiInstanceBody")[0].Id,
//                nrOfActiveInstances.ActivityInstanceId);
//            Assert.AreEqual(tree.GetActivityInstances("miSubProcess#multiInstanceBody")[0].Id,
//                nrOfCompletedInstances.ActivityInstanceId);

//            ISet<string> loopCounterActivityInstanceIds = new HashSet<string>();
//            foreach (var loopCounter in loopCounters)
//                loopCounterActivityInstanceIds.Add(loopCounter.ActivityInstanceId);

//            Assert.AreEqual(4, loopCounterActivityInstanceIds.Count);

//            foreach (var subProcessInstance in tree.GetActivityInstances("miSubProcess"))
//                Assert.True(loopCounterActivityInstanceIds.Contains(subProcessInstance.Id));
//        }
//    }
//}