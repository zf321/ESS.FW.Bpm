//using System;
//using System.Threading;
//using PluggableProcessEngineTestCase = ESS.FW.Bpm.Engine.Tests.PluggableProcessEngineTestCase;



//namespace ESS.FW.Bpm.Engine.Tests.Bpmn.Mail
//{
    

//	/// <summary>
//	/// 
//	/// </summary>
//	public abstract class EmailTestCase : PluggableProcessEngineTestCase
//	{

//	  //private static readonly Logger LOG = TestLogger.TEST_LOGGER.Logger;

//	  protected internal Wiser wiser;

////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
////ORIGINAL LINE: @Override protected void setUp() throws Exception
//	  protected internal void setUp()
//	  {
//		base.SetUp();

//		int port = processEngineConfiguration.MailServerPort;

//		bool serverUpAndRunning = false;
//		while (!serverUpAndRunning)
//		{
//		  wiser = new Wiser();
//		  wiser.Port = port;

//		  try
//		  {
//			LOG.info("Starting Wiser mail server on port: " + port);
//			wiser.Start();
//			serverUpAndRunning = true;
//			LOG.info("Wiser mail server listening on port: " + port);
//		  }
//		  catch (Exception e)
//		  { // Fix for slow port-closing Jenkins
//			if (e.Message.ToLower().Contains("BindException"))
//			{
//			  Thread.Sleep(250L);
//			}
//		  }
//		}
//	  }

////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
////ORIGINAL LINE: @Override protected void tearDown() throws Exception
//	  protected internal void tearDown()
//	  {
//		wiser.stop();

//		// Fix for slow Jenkins
//		Thread.Sleep(250L);

//		base.TearDown();
//	  }

//	}

//}