//using System;
//using ESS.FW.Bpm.Engine.Impl.Cfg;

//namespace ESS.FW.Bpm.Engine.Persistence
//{
//    //using EthernetAddress = com.fasterxml.uuid.EthernetAddress;
//	//using Generators = com.fasterxml.uuid.Generators;
//	//using TimeBasedGenerator = com.fasterxml.uuid.impl.TimeBasedGenerator;

//	/// <summary>
//	/// <seealso cref="IDGenerator"/> implementation based on the current time and the ethernet
//	/// address of the machine it is running on.
//	/// 
//	/// 
//	/// </summary>
//	public class StrongUuidGenerator : Impl.Cfg.IDGenerator
//	{

//	  // different ProcessEngines on the same classloader share one generator.
//	  //protected internal static TimeBasedGenerator timeBasedGenerator;

//	  public StrongUuidGenerator()
//	  {
//		EnsureGeneratorInitialized();
//	  }

//	  protected internal virtual void EnsureGeneratorInitialized()
//	  {
//            throw new NotImplementedException();
//		//if (timeBasedGenerator == null)
//		//{
//		//  lock (typeof(StrongUuidGenerator))
//		//  {
//		//	if (timeBasedGenerator == null)
//		//	{
//		//	  timeBasedGenerator = Generators.timeBasedGenerator(EthernetAddress.fromInterface());
//		//	}
//		//  }
//		//}
//	  }

//	  public virtual string NextId
//	  {
//		  get
//		  {
//                throw new NotImplementedException();
//                //return timeBasedGenerator.generate().ToString();
//		  }
//	  }

//	}

//}