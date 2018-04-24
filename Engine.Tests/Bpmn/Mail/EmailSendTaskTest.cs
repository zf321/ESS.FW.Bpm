//using System;
//using System.Collections.Generic;
//using ESS.FW.Bpm.Engine.Impl.Util;
//using NUnit.Framework;


//namespace ESS.FW.Bpm.Engine.Tests.Bpmn.Mail
//{


//	/// <summary>
//	/// 
//	/// 
//	/// </summary>
//	public class EmailSendTaskTest : EmailTestCase
//	{

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @IDeployment public void testSimpleTextMail() throws Exception
////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//	  public virtual void testSimpleTextMail()
//	  {
//		runtimeService.StartProcessInstanceByKey("simpleTextOnly");

//		IList<WiserMessage> messages = wiser.Messages;
//		Assert.AreEqual(1, messages.Count);

//		WiserMessage message = messages[0];
//		AssertEmailSend(message, false, "Hello Kermit!", "This a text only e-mail.", "camunda@localhost", ("kermit@camunda.org"), null);
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @IDeployment public void testSimpleTextMailMultipleRecipients()
//	  public virtual void testSimpleTextMailMultipleRecipients()
//	  {
//		runtimeService.StartProcessInstanceByKey("simpleTextOnlyMultipleRecipients");

//		// 3 recipients == 3 emails in wiser with different receivers
//		IList<WiserMessage> messages = wiser.Messages;
//		Assert.AreEqual(3, messages.Count);

//		// sort recipients for easy Assertion
//		IList<string> recipients = new List<string>();
//		foreach (WiserMessage message in messages)
//		{
//		  recipients.Add(message.EnvelopeReceiver);
//		}
//		recipients.Sort();

//		Assert.AreEqual("fozzie@camunda.org", recipients[0]);
//		Assert.AreEqual("kermit@camunda.org", recipients[1]);
//		Assert.AreEqual("mispiggy@camunda.org", recipients[2]);
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @IDeployment public void testTextMailExpressions() throws Exception
////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//	  public virtual void testTextMailExpressions()
//	  {

//		string sender = "mispiggy@activiti.org";
//		string recipient = "fozziebear@activiti.org";
//		string recipientName = "Mr. Fozzie";
//		string subject = "Fozzie, you should see this!";

//		IDictionary<string, object> vars = new Dictionary<string, object>();
//		vars["sender"] = sender;
//		vars["recipient"] = recipient;
//		vars["recipientName"] = recipientName;
//		vars["subject"] = subject;

//		runtimeService.StartProcessInstanceByKey("textMailExpressions", vars);

//		IList<WiserMessage> messages = wiser.Messages;
//		Assert.AreEqual(1, messages.Count);

//		WiserMessage message = messages[0];
//		AssertEmailSend(message, false, subject, "Hello " + recipientName + ", this is an e-mail", sender, (recipient), null);
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @IDeployment public void testCcAndBcc() throws Exception
////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//	  public virtual void testCcAndBcc()
//	  {
//		runtimeService.StartProcessInstanceByKey("ccAndBcc");

//		IList<WiserMessage> messages = wiser.Messages;
//		AssertEmailSend(messages[0], false, "Hello world", "This is the content", "camunda@localhost", ("kermit@camunda.org"), ("fozzie@camunda.org"));

//		// Bcc is not stored in the header (obviously)
//		// so the only way to verify the bcc, is that there are three messages send.
//		Assert.AreEqual(3, messages.Count);
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @IDeployment public void testHtmlMail() throws Exception
////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//	  public virtual void testHtmlMail()
//	  {
//		runtimeService.StartProcessInstanceByKey("htmlMail", CollectionUtil.singletonMap("gender", "male"));

//		IList<WiserMessage> messages = wiser.Messages;
//		Assert.AreEqual(1, messages.Count);
//		AssertEmailSend(messages[0], true, "Test", "Mr. <b>Kermit</b>", "camunda@localhost", ("kermit@camunda.org"), null);
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @IDeployment public void testSendEmail() throws Exception
////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//	  public virtual void testSendEmail()
//	  {

//		string from = "ordershipping@activiti.org";
//		bool male = true;
//		string recipientName = "John Doe";
//		string recipient = "johndoe@alfresco.com";
//		DateTime now = DateTime.Now;
//		string orderId = "123456";

//		IDictionary<string, object> vars = new Dictionary<string, object>();
//		vars["sender"] = from;
//		vars["recipient"] = recipient;
//		vars["recipientName"] = recipientName;
//		vars["male"] = male;
//		vars["now"] = now;
//		vars["orderId"] = orderId;

//		runtimeService.StartProcessInstanceByKey("sendMailExample", vars);

//		IList<WiserMessage> messages = wiser.Messages;
//		Assert.AreEqual(1, messages.Count);

//		WiserMessage message = messages[0];
//		MimeMessage mimeMessage = message.MimeMessage;

//		Assert.AreEqual("Your order " + orderId + " has been shipped", mimeMessage.GetHeader("Subject", null));
//		Assert.AreEqual("\"" + from + "\" <" + from.ToString() + ">", mimeMessage.GetHeader("From", null));
//		Assert.True(mimeMessage.GetHeader("To", null).Contains(recipient));
//	  }

//	  // Helper

////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
////ORIGINAL LINE: private void AssertEmailSend(org.Subethamail.Wiser.WiserMessage emailMessage, boolean htmlMail, String subject, String message, String from, java.util.List<String> to, java.util.List<String> cc) throws java.io.IOException
//	  private void AssertEmailSend(WiserMessage emailMessage, bool htmlMail, string subject, string message, string from, IList<string> to, IList<string> cc)
//	  {
//		try
//		{
//		  MimeMessage mimeMessage = emailMessage.MimeMessage;

//		  if (htmlMail)
//		  {
//			Assert.True(mimeMessage.ContentType.Contains("multipart/mixed"));
//		  }
//		  else
//		  {
//			Assert.True(mimeMessage.ContentType.Contains("text/plain"));
//		  }

//		  Assert.AreEqual(subject, mimeMessage.GetHeader("Subject", null));
//		  Assert.AreEqual("\"" + from + "\" <" + from.ToString() + ">", mimeMessage.GetHeader("From", null));
//		  Assert.True(getMessage(mimeMessage).Contains(message));

//		  foreach (string t in to)
//		  {
//			Assert.True(mimeMessage.GetHeader("To", null).Contains(t));
//		  }

//		  if (cc != null)
//		  {
//			foreach (string c in cc)
//			{
//			  Assert.True(mimeMessage.GetHeader("Cc", null).Contains(c));
//			}
//		  }

//		}
//		catch (MessagingException e)
//		{
//		  Assert.Fail(e.Message);
//		}

//	  }

////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
////ORIGINAL LINE: protected String getMessage(javax.Mail.internet.MimeMessage mimeMessage) throws javax.Mail.MessagingException, java.io.IOException
//	  protected internal virtual string getMessage(MimeMessage mimeMessage)
//	  {
//		DataHandler dataHandler = mimeMessage.DataHandler;
//		System.IO.MemoryStream baos = new System.IO.MemoryStream();
//		dataHandler.WriteTo(baos);
//		baos.Flush();
//		return baos.ToString();
//	  }

//	}

//}