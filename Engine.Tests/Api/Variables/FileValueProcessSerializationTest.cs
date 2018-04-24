using System.IO;
using System.Linq;
using System.Text;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Engine.Variable;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Api.Variables
{
    
    /// <summary>
	/// @author Ronny Bräunlich
	/// 
	/// </summary>
	[TestFixture]
	public class FileValueProcessSerializationTest : PluggableProcessEngineTestCase
	{
      //  protected internal Variable.Variables;
        

      protected internal const string ONE_TASK_PROCESS = "org/camunda/bpm/engine/test/api/variables/oneTaskProcess.bpmn20.xml";

        [Test]
	  public virtual void testSerializeFileVariable()
	  {
		IBpmnModelInstance modelInstance = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process").StartEvent().UserTask().EndEvent().Done();
		IDeployment deployment = repositoryService.CreateDeployment().AddModelInstance("process.bpmn", modelInstance).Deploy();
		IVariableMap variables = ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables();
		string filename = "test.Txt";
		string type = "text/plain";
        IFileValue fileValue = ESS.FW.Bpm.Engine.Variable.Variables.FileValue(filename).File(File.Open(filename,FileMode.OpenOrCreate)).Encoding("UTF-8").MimeType(type).Create();
       // IFileValue fileValue = Variable.Variables.FileValue(filename).File("ABC".GetBytes()).Encoding("UTF-8").MimeType(type).Create();
        variables.Add("file", fileValue);
		runtimeService.StartProcessInstanceByKey("process", variables);
		ITask task = taskService.CreateTaskQuery().First();
		IVariableInstance result = runtimeService.CreateVariableInstanceQuery(c=>c.ProcessInstanceId == task.ProcessInstanceId).First();
		IFileValue value = (IFileValue) result.TypedValue;

		Assert.That(value.Filename, Is.EqualTo(filename));
		Assert.That(value.MimeType, Is.EqualTo(type));
		Assert.That(value.Encoding, Is.EqualTo("UTF-8"));
		Assert.That(value.EncodingAsCharset, Is.EqualTo(Encoding.UTF8));
		//Scanner scanner = new Scanner(value.Value);
            //Assert.That(scanner.NextLine(), Is.EqualTo("ABC"));
           
        Assert.That(value.Value, Is.EqualTo("ABC"));
            // clean up
            repositoryService.DeleteDeployment(deployment.Id, true);
	  }

        [Test]
	    public virtual void testSerializeNullMimeType()
	    {
	        string fileName = "test.Txt";
	        IProcessInstance pi = runtimeService.StartProcessInstanceByKey("oneTaskProcess", ESS.FW.Bpm.Engine.Variable.Variables
	            .CreateVariables().PutValue("fileVar", ESS.FW.Bpm.Engine.Variable.Variables.FileValue(fileName)
	                .File(File.Open(fileName, FileMode.OpenOrCreate)).Encoding("UTF-8").Create()));
	        //.File("ABC".GetBytes()).Encoding("UTF-8").Create()));

	        IFileValue fileVar = runtimeService.GetVariableTyped<IFileValue>(pi.Id, "fileVar");
	        Assert.IsNull(fileVar.MimeType);
	    }

        [Test]
      public virtual void testSerializeNullMimeTypeAndNullEncoding()
	  {
        string fileName = "test.Txt";
        IProcessInstance pi = runtimeService.StartProcessInstanceByKey("oneTaskProcess", ESS.FW.Bpm.Engine.Variable.Variables
            .CreateVariables().PutValue("fileVar", ESS.FW.Bpm.Engine.Variable.Variables.FileValue(fileName)
                .File(File.Open(fileName, FileMode.OpenOrCreate)).Encoding("UTF-8").Create()));
        //.File("ABC".GetBytes()).Encoding("UTF-8").Create()));

        IFileValue fileVar = runtimeService.GetVariableTyped<IFileValue>(pi.Id, "fileVar");
		Assert.IsNull(fileVar.MimeType);
		Assert.IsNull(fileVar.Encoding);
	  }

        [Test]
	  public virtual void testSerializeNullEncoding()
        {
            string fileName = "test.Txt";
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("oneTaskProcess", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables().PutValue("fileVar", 
            ESS.FW.Bpm.Engine.Variable.Variables.FileValue(fileName).MimeType("some mimetype")
            .File(File.Open(fileName, FileMode.OpenOrCreate)).Create()));

		IFileValue fileVar = runtimeService.GetVariableTyped<IFileValue>(pi.Id, "fileVar");
		Assert.IsNull(fileVar.Encoding);
	  }

        [Test]
	  public virtual void testSerializeNullValue()
	  {
		IProcessInstance pi = runtimeService.StartProcessInstanceByKey("oneTaskProcess", 
            ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables().PutValue("fileVar"
            , ESS.FW.Bpm.Engine.Variable.Variables.FileValue("test.Txt").Create()));

		IFileValue fileVar = runtimeService.GetVariableTyped<IFileValue>(pi.Id, "fileVar");
		Assert.IsNull(fileVar.MimeType);
	  }

	}

}