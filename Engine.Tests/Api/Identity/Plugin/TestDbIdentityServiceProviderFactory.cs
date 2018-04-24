//using System;
//using ESS.FW.Bpm.Engine.Impl.Interceptor;

///// 
//namespace ESS.FW.Bpm.Engine.Tests.Api.Identity.Plugin
//{
    

//	/// <summary>
//	/// 
//	/// 
//	/// </summary>
//	public class TestDbIdentityServiceProviderFactory : ISessionFactory
//	{

//	  public Type SessionType
//	  {
//		  get
//		  {
//			return typeof(TestDbIdentityServiceProviderExtension);
//		  }
//	  }

//	  public ISession OpenSession()
//	  {
//		return new TestDbIdentityServiceProviderExtension();
//	  }

//	}

//}