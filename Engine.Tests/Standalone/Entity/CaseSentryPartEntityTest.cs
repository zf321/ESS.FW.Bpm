//using PluggableProcessEngineTestCase = ESS.FW.Bpm.Engine.Tests.PluggableProcessEngineTestCase;

//namespace ESS.FW.Bpm.Engine.Tests.Standalone.Entity
//{
    
//	/// <summary>
//	/// @author Kristin Polenz
//	/// </summary>
//	public class CaseSentryPartEntityTest : PluggableProcessEngineTestCase
//	{

//	  public virtual void testSentryWithTenantId()
//	  {
//		CaseSentryPartEntity caseSentryPartEntity = new CaseSentryPartEntity();
//		caseSentryPartEntity.TenantId = "tenant1";

//		insertCaseSentryPart(caseSentryPartEntity);

//		caseSentryPartEntity = readCaseSentryPart();
//		Assert.That(caseSentryPartEntity.TenantId, @is("tenant1"));

//		deleteCaseSentryPart(caseSentryPartEntity);
//	  }

////JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
////ORIGINAL LINE: protected void insertCaseSentryPart(final impl.cmmn.Entity.Runtime.CaseSentryPartEntity caseSentryPartEntity)
//	  protected internal virtual void insertCaseSentryPart(CaseSentryPartEntity caseSentryPartEntity)
//	  {
//		processEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass(this, caseSentryPartEntity));
//	  }

//	  private class CommandAnonymousInnerClass : ICommand<object>
//	  {
//		  private readonly CaseSentryPartEntityTest outerInstance;

//		  private CaseSentryPartEntity caseSentryPartEntity;

//		  public CommandAnonymousInnerClass(CaseSentryPartEntityTest outerInstance, CaseSentryPartEntity caseSentryPartEntity)
//		  {
//			  this.outerInstance = outerInstance;
//			  this.CaseSentryPartEntity = caseSentryPartEntity;
//		  }


//		  public override object Execute(CommandContext commandContext)
//		  {
//			commandContext.CaseSentryPartManager.Insert(caseSentryPartEntity);
//			return null;
//		  }
//	  }

//	  protected internal virtual CaseSentryPartEntity readCaseSentryPart()
//	  {
//		ICommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequiresNew;
//		return (new CaseSentryPartQueryImpl(commandExecutor)).First();
//	  }

////JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
////ORIGINAL LINE: protected void deleteCaseSentryPart(final impl.cmmn.Entity.Runtime.CaseSentryPartEntity caseSentryPartEntity)
//	  protected internal virtual void deleteCaseSentryPart(CaseSentryPartEntity caseSentryPartEntity)
//	  {
//		processEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass2(this, caseSentryPartEntity));
//	  }

//	  private class CommandAnonymousInnerClass2 : ICommand<object>
//	  {
//		  private readonly CaseSentryPartEntityTest outerInstance;

//		  private CaseSentryPartEntity caseSentryPartEntity;

//		  public CommandAnonymousInnerClass2(CaseSentryPartEntityTest outerInstance, CaseSentryPartEntity caseSentryPartEntity)
//		  {
//			  this.outerInstance = outerInstance;
//			  this.CaseSentryPartEntity = caseSentryPartEntity;
//		  }


//		  public override object Execute(CommandContext commandContext)
//		  {
//			commandContext.CaseSentryPartManager.Delete(caseSentryPartEntity);
//			return null;
//		  }
//	  }

//	}

//}