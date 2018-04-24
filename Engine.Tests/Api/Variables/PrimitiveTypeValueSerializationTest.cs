using System;
using System.Collections.Generic;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using NUnit.Framework;

namespace Engine.Tests.Api.Variables
{


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.AreEqual;

	using IProcessInstance = ESS.FW.Bpm.Engine.Runtime.IProcessInstance;
	using ProvidedProcessEngineRule = ProvidedProcessEngineRule;
	using ITypedValue = ESS.FW.Bpm.Engine.Variable.Value.ITypedValue;

    /// <summary>
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Parameterized.class) public class PrimitiveTypeValueSerializationTest
[TestFixture]
	public class PrimitiveTypeValueSerializationTest
	{

	  protected internal const string BPMN_FILE = "org/camunda/bpm/engine/test/api/variables/oneTaskProcess.bpmn20.xml";
	  protected internal const string PROCESS_DEFINITION_KEY = "oneTaskProcess";

	  protected internal const string VARIABLE_NAME = "variable";
       
        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Parameters(name = "{index}: variable = {0}") public static java.util.Collection<Object[]> data()
        public static ICollection<object[]> data()
        {

            return (new object[][]
            {
            new object[] {ESS.FW.Bpm.Engine.Variable.Variables.StringValue("a"), ESS.FW.Bpm.Engine.Variable.Variables.StringValue(null)},
            new object[] {ESS.FW.Bpm.Engine.Variable.Variables.BooleanValue(true), ESS.FW.Bpm.Engine.Variable.Variables.BooleanValue(null)},
            new object[] {ESS.FW.Bpm.Engine.Variable.Variables.IntegerValue(4), ESS.FW.Bpm.Engine.Variable.Variables.IntegerValue(null)},
            new object[] {ESS.FW.Bpm.Engine.Variable.Variables.ShortValue((short) 2), ESS.FW.Bpm.Engine.Variable.Variables.ShortValue(null)},
            new object[] {ESS.FW.Bpm.Engine.Variable.Variables.LongValue(6L), ESS.FW.Bpm.Engine.Variable.Variables.LongValue(null)},
            new object[] {ESS.FW.Bpm.Engine.Variable.Variables.DoubleValue(4.2), ESS.FW.Bpm.Engine.Variable.Variables.DoubleValue(null)},
            new object[] {ESS.FW.Bpm.Engine.Variable.Variables.DateValue(DateTime.Now), ESS.FW.Bpm.Engine.Variable.Variables.DateValue(DateTime.Now)}
            });
        }
        

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameter(0) public ITypedValue typedValue;
	  public ITypedValue typedValue;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameter(1) public ITypedValue nullValue;
	  public ITypedValue nullValue;

	  private IRuntimeService runtimeService;
	  private IRepositoryService repositoryService;
	  private string deploymentId;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public ProcessEngineRule rule = new util.ProvidedProcessEngineRule();
	  public ProcessEngineRule rule = new ProvidedProcessEngineRule();

        [SetUp]
	  public virtual void setup()
	  {
		runtimeService = rule.RuntimeService;
		repositoryService = rule.RepositoryService;

		deploymentId = repositoryService.CreateDeployment().AddClasspathResource(BPMN_FILE).Deploy().Id;
	  }

      [TearDown]
	  public virtual void teardown()
	  {
		repositoryService.DeleteDeployment(deploymentId, true);
	  }

        [Test]
	  public virtual void shouldGetUntypedVariable()
	  {
		IProcessInstance instance = runtimeService.StartProcessInstanceByKey(PROCESS_DEFINITION_KEY);

		runtimeService.SetVariable(instance.Id, VARIABLE_NAME, typedValue);

		object variableValue = runtimeService.GetVariable(instance.Id, VARIABLE_NAME);
		Assert.AreEqual(typedValue.Value, variableValue);
	  }

        [Test]
	  public virtual void shouldGetTypedVariable()
	  {
		IProcessInstance instance = runtimeService.StartProcessInstanceByKey(PROCESS_DEFINITION_KEY);

		runtimeService.SetVariable(instance.Id, VARIABLE_NAME, typedValue);

		ITypedValue typedVariableValue = runtimeService.GetVariableTyped<ITypedValue>(instance.Id, VARIABLE_NAME);
		Assert.AreEqual(typedValue.Type, typedVariableValue.Type);
		Assert.AreEqual(typedValue.Value, typedVariableValue.Value);
	  }

        [Test]
	  public virtual void shouldGetTypedNullVariable()
	  {
		IProcessInstance instance = runtimeService.StartProcessInstanceByKey(PROCESS_DEFINITION_KEY);

		runtimeService.SetVariable(instance.Id, VARIABLE_NAME, nullValue);

		Assert.AreEqual(null, runtimeService.GetVariable(instance.Id, VARIABLE_NAME));

		ITypedValue typedVariableValue = runtimeService.GetVariableTyped<ITypedValue>(instance.Id, VARIABLE_NAME);
		Assert.AreEqual(nullValue.Type, typedVariableValue.Type);
		Assert.AreEqual(null, typedVariableValue.Value);
	  }

	}

}