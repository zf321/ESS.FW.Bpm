//using System.Collections.Generic;
//using ESS.FW.Bpm.Engine.Tests.Bpmn.Mail;
//using NUnit.Framework;


//namespace ESS.FW.Bpm.Engine.Tests.Bpmn.Async
//{


//	/// 
//	/// 
//	/// <summary>
//	/// 
//	/// </summary>
//	public class AsyncEmailTaskTest : EmailTestCase
//	{

//	  // copied from bpmn.Mail.EmailServiceTaskTest
////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @IDeployment public void testSimpleTextMail() throws Exception
////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//	  public virtual void testSimpleTextMail()
//	  {
//		string procId = runtimeService.StartProcessInstanceByKey("simpleTextOnly").Id;

//		IList<WiserMessage> messages = wiser.Messages;
//		Assert.AreEqual(0, messages.Count);

//		waitForJobExecutorToProcessAllJobs(5000L);

//		messages = wiser.Messages;
//		Assert.AreEqual(1, messages.Count);

//		WiserMessage message = messages[0];
//		EmailServiceTaskTest.AssertEmailSend(message, false, "Hello Kermit!", "This a text only e-mail.", "camunda@localhost", ("kermit@camunda.org"), null);
//		AssertProcessEnded(procId);
//	  }

//	  // copied from bpmn.Mail.EmailSendTaskTest
////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @IDeployment public void testSimpleTextMailSendTask() throws Exception
////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//	  public virtual void testSimpleTextMailSendTask()
//	  {
//		runtimeService.StartProcessInstanceByKey("simpleTextOnly");

//		IList<WiserMessage> messages = wiser.Messages;
//		Assert.AreEqual(0, messages.Count);

//		waitForJobExecutorToProcessAllJobs(5000L);

//		messages = wiser.Messages;
//		Assert.AreEqual(1, messages.Count);

//		WiserMessage message = messages[0];
//		EmailServiceTaskTest.AssertEmailSend(message, false, "Hello Kermit!", "This a text only e-mail.", "camunda@localhost", ("kermit@camunda.org"), null);
//	  }

//	}

//}