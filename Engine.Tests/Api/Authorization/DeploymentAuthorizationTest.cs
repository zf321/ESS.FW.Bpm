using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Application;
using ESS.FW.Bpm.Engine.Application.Impl;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Repository;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization
{
    

	/// <summary>
	/// 
	/// 
	/// </summary>
	public class DeploymentAuthorizationTest : AuthorizationTest
	{

	  protected internal const string FIRST_RESOURCE = "resources/api/oneTaskProcess.bpmn20.xml";
	  protected internal const string SECOND_RESOURCE = "resources/api/authorization/messageBoundaryEventProcess.bpmn20.xml";

	  // query ////////////////////////////////////////////////////////////

	  public virtual void testSimpleDeploymentQueryWithoutAuthorization()
	  {
		// given
		string deploymentId = createDeployment(null);

		// when
		 IQueryable<IDeployment> query = repositoryService.CreateDeploymentQuery();

		// then
		//verifyQueryResults(query, 0);

		DeleteDeployment(deploymentId);
	  }

	  public virtual void testSimpleDeploymentQueryWithReadPermissionOnDeployment()
	  {
		// given
		string deploymentId = createDeployment(null);
		createGrantAuthorization(Resources.Deployment, deploymentId, userId, Permissions.Read);

		// when
		 IQueryable<IDeployment> query = repositoryService.CreateDeploymentQuery();

		// then
		//verifyQueryResults(query, 1);

		DeleteDeployment(deploymentId);
	  }

	  public virtual void testSimpleDeploymentQueryWithReadPermissionOnAnyDeployment()
	  {
		// given
		string deploymentId = createDeployment(null);
		createGrantAuthorization(Resources.Deployment, AuthorizationFields.Any, userId, Permissions.Read);

		// when
		 IQueryable<IDeployment> query = repositoryService.CreateDeploymentQuery();

		// then
		//verifyQueryResults(query, 1);

		DeleteDeployment(deploymentId);
	  }

	  public virtual void testSimpleDeploymentQueryWithMultiple()
	  {
		// given
		string deploymentId = createDeployment(null);
		createGrantAuthorization(Resources.Deployment, deploymentId, userId, Permissions.Read);
		createGrantAuthorization(Resources.Deployment, AuthorizationFields.Any, userId, Permissions.Read);

		// when
		 IQueryable<IDeployment> query = repositoryService.CreateDeploymentQuery();

		// then
		//verifyQueryResults(query, 1);

		DeleteDeployment(deploymentId);
	  }

	  public virtual void testDeploymentQueryWithoutAuthorization()
	  {
		// given
		string deploymentId1 = createDeployment("first");
		string deploymentId2 = createDeployment("second");

		// when
		 IQueryable<IDeployment> query = repositoryService.CreateDeploymentQuery();

		// then
		//verifyQueryResults(query, 0);

		DeleteDeployment(deploymentId1);
		DeleteDeployment(deploymentId2);
	  }

	  public virtual void testDeploymentQueryWithReadPermissionOnDeployment()
	  {
		// given
		string deploymentId1 = createDeployment("first");
		string deploymentId2 = createDeployment("second");
		createGrantAuthorization(Resources.Deployment, deploymentId1, userId, Permissions.Read);

		// when
		 IQueryable<IDeployment> query = repositoryService.CreateDeploymentQuery();

		// then
		//verifyQueryResults(query, 1);

		DeleteDeployment(deploymentId1);
		DeleteDeployment(deploymentId2);
	  }

	  public virtual void testDeploymentQueryWithReadPermissionOnAnyDeployment()
	  {
		// given
		string deploymentId1 = createDeployment("first");
		string deploymentId2 = createDeployment("second");
		createGrantAuthorization(Resources.Deployment, AuthorizationFields.Any, userId, Permissions.Read);

		// when
		 IQueryable<IDeployment> query = repositoryService.CreateDeploymentQuery();

		// then
		//verifyQueryResults(query, 2);

		DeleteDeployment(deploymentId1);
		DeleteDeployment(deploymentId2);
	  }

	  // create deployment ///////////////////////////////////////////////

	  public virtual void testCreateDeploymentWithoutAuthoriatzion()
	  {
		// given

		try
		{
		  // when
		  repositoryService.CreateDeployment().AddClasspathResource(FIRST_RESOURCE).Deploy();
		  Assert.Fail("Exception expected: It should not be possible to create a new deployment");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  AssertTextPresent(userId, message);
		  AssertTextPresent(Permissions.Create.ToString(), message);
		  AssertTextPresent(Resources.Deployment.ToString()/*.ResourceName()*/, message);
		}
	  }

	  public virtual void testCreateDeployment()
	  {
		// given
		createGrantAuthorization(Resources.Deployment, AuthorizationFields.Any, userId, Permissions.Create);

		// when
		IDeployment deployment = repositoryService.CreateDeployment().AddClasspathResource(FIRST_RESOURCE).Deploy();

		// then
		disableAuthorization();
		 IQueryable<IDeployment> query = repositoryService.CreateDeploymentQuery();
		//verifyQueryResults(query, 1);
		enableAuthorization();

		DeleteDeployment(deployment.Id);
	  }

	  // Delete deployment //////////////////////////////////////////////

	  public virtual void testDeleteDeploymentWithoutAuthorization()
	  {
		// given
		string deploymentId = createDeployment(null);

		try
		{
		  // when
		  repositoryService.DeleteDeployment(deploymentId);
		  Assert.Fail("Exception expected: it should not be possible to Delete a deployment");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  AssertTextPresent(userId, message);
		  AssertTextPresent(Permissions.Delete.ToString(), message);
		  AssertTextPresent(Resources.Deployment.ToString()/*.ResourceName()*/, message);
		}

		DeleteDeployment(deploymentId);
	  }

	  public virtual void testDeleteDeploymentWithDeletePermissionOnDeployment()
	  {
		// given
		string deploymentId = createDeployment(null);
		createGrantAuthorization(Resources.Deployment, deploymentId, userId, Permissions.Delete);

		// when
		repositoryService.DeleteDeployment(deploymentId);

		// then
		disableAuthorization();
		 IQueryable<IDeployment> query = repositoryService.CreateDeploymentQuery();
		//verifyQueryResults(query, 0);
		enableAuthorization();

		DeleteDeployment(deploymentId);
	  }

	  public virtual void testDeleteDeploymentWithDeletePermissionOnAnyDeployment()
	  {
		// given
		string deploymentId = createDeployment(null);
		createGrantAuthorization(Resources.Deployment, AuthorizationFields.Any, userId, Permissions.Delete);

		// when
		repositoryService.DeleteDeployment(deploymentId);

		// then
		disableAuthorization();
		 IQueryable<IDeployment> query = repositoryService.CreateDeploymentQuery();
		//verifyQueryResults(query, 0);
		enableAuthorization();

		DeleteDeployment(deploymentId);
	  }

	  // get deployment resource names //////////////////////////////////

	  public virtual void testGetDeploymentResourceNamesWithoutAuthorization()
	  {
		// given
		string deploymentId = createDeployment(null);

		try
		{
		  // when
		  repositoryService.GetDeploymentResourceNames(deploymentId);
		  Assert.Fail("Exception expected: it should not be possible to retrieve deployment resource names");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  AssertTextPresent(userId, message);
		  AssertTextPresent(Permissions.Read.ToString(), message);
		  AssertTextPresent(Resources.Deployment.ToString()/*.ResourceName()*/, message);
		}

		DeleteDeployment(deploymentId);
	  }

	  public virtual void testGetDeploymentResourceNamesWithReadPermissionOnDeployment()
	  {
		// given
		string deploymentId = createDeployment(null);
		createGrantAuthorization(Resources.Deployment, deploymentId, userId, Permissions.Read);

		// when
		IList<string> names = repositoryService.GetDeploymentResourceNames(deploymentId);

		// then
		Assert.IsFalse(names.Count == 0);
		Assert.AreEqual(2, names.Count);
		Assert.True(names.Contains(FIRST_RESOURCE));
		Assert.True(names.Contains(SECOND_RESOURCE));

		DeleteDeployment(deploymentId);
	  }

	  public virtual void testGetDeploymentResourceNamesWithReadPermissionOnAnyDeployment()
	  {
		// given
		string deploymentId = createDeployment(null);
		createGrantAuthorization(Resources.Deployment, AuthorizationFields.Any, userId, Permissions.Read);

		// when
		IList<string> names = repositoryService.GetDeploymentResourceNames(deploymentId);

		// then
		Assert.IsFalse(names.Count == 0);
		Assert.AreEqual(2, names.Count);
		Assert.True(names.Contains(FIRST_RESOURCE));
		Assert.True(names.Contains(SECOND_RESOURCE));

		DeleteDeployment(deploymentId);
	  }

	  // get deployment resources //////////////////////////////////

	  public virtual void testGetDeploymentResourcesWithoutAuthorization()
	  {
		// given
		string deploymentId = createDeployment(null);

		try
		{
		  // when
		  repositoryService.GetDeploymentResources(deploymentId);
		  Assert.Fail("Exception expected: it should not be possible to retrieve deployment resources");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  AssertTextPresent(userId, message);
		  AssertTextPresent(Permissions.Read.ToString(), message);
		  AssertTextPresent(Resources.Deployment.ToString()/*.ResourceName()*/, message);
		}

		DeleteDeployment(deploymentId);
	  }

	  public virtual void testGetDeploymentResourcesWithReadPermissionOnDeployment()
	  {
		// given
		string deploymentId = createDeployment(null);
		createGrantAuthorization(Resources.Deployment, deploymentId, userId, Permissions.Read);

		// when
		IList<IResource> resources = repositoryService.GetDeploymentResources(deploymentId);

		// then
		Assert.IsFalse(resources.Count == 0);
		Assert.AreEqual(2, resources.Count);

		DeleteDeployment(deploymentId);
	  }

	  public virtual void testGetDeploymentResourcesWithReadPermissionOnAnyDeployment()
	  {
		// given
		string deploymentId = createDeployment(null);
		createGrantAuthorization(Resources.Deployment, AuthorizationFields.Any, userId, Permissions.Read);

		// when
		IList<IResource> resources = repositoryService.GetDeploymentResources(deploymentId);

		// then
		Assert.IsFalse(resources.Count == 0);
		Assert.AreEqual(2, resources.Count);

		DeleteDeployment(deploymentId);
	  }

	  // get resource as stream //////////////////////////////////

	  public virtual void testGetResourceAsStreamWithoutAuthorization()
	  {
		// given
		string deploymentId = createDeployment(null);

		try
		{
		  // when
		  repositoryService.GetResourceAsStream(deploymentId, FIRST_RESOURCE);
		  Assert.Fail("Exception expected: it should not be possible to retrieve a resource as stream");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  AssertTextPresent(userId, message);
		  AssertTextPresent(Permissions.Read.ToString(), message);
		  AssertTextPresent(Resources.Deployment.ToString()/*.ResourceName()*/, message);
		}

		DeleteDeployment(deploymentId);
	  }

	  public virtual void testGetResourceAsStreamWithReadPermissionOnDeployment()
	  {
		// given
		string deploymentId = createDeployment(null);
		createGrantAuthorization(Resources.Deployment, deploymentId, userId, Permissions.Read);

		// when
		System.IO.Stream stream = repositoryService.GetResourceAsStream(deploymentId, FIRST_RESOURCE);

		// then
		Assert.NotNull(stream);

		DeleteDeployment(deploymentId);
	  }

	  public virtual void testGetResourceAsStreamWithReadPermissionOnAnyDeployment()
	  {
		// given
		string deploymentId = createDeployment(null);
		createGrantAuthorization(Resources.Deployment, AuthorizationFields.Any, userId, Permissions.Read);

		// when
		System.IO.Stream stream = repositoryService.GetResourceAsStream(deploymentId, FIRST_RESOURCE);

		// then
		Assert.NotNull(stream);

		DeleteDeployment(deploymentId);
	  }

	  // get resource as stream by id//////////////////////////////////

	  public virtual void testGetResourceAsStreamByIdWithoutAuthorization()
	  {
		// given
		string deploymentId = createDeployment(null);

		disableAuthorization();
		IList<IResource> resources = repositoryService.GetDeploymentResources(deploymentId);
		enableAuthorization();
		string resourceId = resources[0].Id;

		try
		{
		  // when
		  repositoryService.GetResourceAsStreamById(deploymentId, resourceId);
		  Assert.Fail("Exception expected: it should not be possible to retrieve a resource as stream");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  AssertTextPresent(userId, message);
		  AssertTextPresent(Permissions.Read.ToString(), message);
		  AssertTextPresent(Resources.Deployment.ToString()/*.ResourceName()*/, message);
		}

		DeleteDeployment(deploymentId);
	  }

	  public virtual void testGetResourceAsStreamByIdWithReadPermissionOnDeployment()
	  {
		// given
		string deploymentId = createDeployment(null);
		createGrantAuthorization(Resources.Deployment, deploymentId, userId, Permissions.Read);

		disableAuthorization();
		IList<IResource> resources = repositoryService.GetDeploymentResources(deploymentId);
		enableAuthorization();
		string resourceId = resources[0].Id;

		// when
		System.IO.Stream stream = repositoryService.GetResourceAsStreamById(deploymentId, resourceId);

		// then
		Assert.NotNull(stream);

		DeleteDeployment(deploymentId);
	  }

	  public virtual void testGetResourceAsStreamByIdWithReadPermissionOnAnyDeployment()
	  {
		// given
		string deploymentId = createDeployment(null);
		createGrantAuthorization(Resources.Deployment, AuthorizationFields.Any, userId, Permissions.Read);

		disableAuthorization();
		IList<IResource> resources = repositoryService.GetDeploymentResources(deploymentId);
		enableAuthorization();
		string resourceId = resources[0].Id;

		// when
		System.IO.Stream stream = repositoryService.GetResourceAsStreamById(deploymentId, resourceId);

		// then
		Assert.NotNull(stream);

		DeleteDeployment(deploymentId);
	  }

	  // should create authorization /////////////////////////////////////

	  public virtual void testCreateAuthorizationOnDeploy()
	  {
		// given
		createGrantAuthorization(Resources.Deployment, AuthorizationFields.Any, userId, Permissions.Create);
		IDeployment deployment = repositoryService.CreateDeployment().AddClasspathResource(FIRST_RESOURCE).Deploy();

		// when
		IAuthorization authorization = authorizationService.CreateAuthorizationQuery(c=>c.UserId== userId&& c.ResourceId==deployment.Id).First();

		// then
		Assert.NotNull(authorization);
		Assert.True(authorization.IsPermissionGranted(Permissions.Read));
		Assert.True(authorization.IsPermissionGranted(Permissions.Delete));
		Assert.IsFalse(authorization.IsPermissionGranted(Permissions.Update));

		DeleteDeployment(deployment.Id);
	  }

	  // clear authorization /////////////////////////////////////

	  public virtual void testClearAuthorizationOnDeleteDeployment()
	  {
		// given
		createGrantAuthorization(Resources.Deployment, AuthorizationFields.Any, userId, Permissions.Create);
		IDeployment deployment = repositoryService.CreateDeployment().AddClasspathResource(FIRST_RESOURCE).Deploy();

		string deploymentId = deployment.Id;

		IQueryable<IAuthorization> query = authorizationService.CreateAuthorizationQuery(c=>c.UserId== userId&& c.ResourceId==deploymentId);

		IAuthorization authorization = query.First();
		Assert.NotNull(authorization);

		// when
		repositoryService.DeleteDeployment(deploymentId);

		authorization = query.First();
		Assert.IsNull(authorization);

		DeleteDeployment(deploymentId);
	  }

	  // register process application ///////////////////////////////////

	  public virtual void testRegisterProcessApplicationWithoutAuthorization()
	  {
		// given
		EmbeddedProcessApplication processApplication = new EmbeddedProcessApplication();
		IProcessApplicationReference reference = processApplication.Reference;
		string deploymentId = createDeployment(null, FIRST_RESOURCE).Id;

		try
		{
		  // when
		  managementService.RegisterProcessApplication(deploymentId, reference);
		  Assert.Fail("Exception expected: It should not be possible to register a process application");
		}
		catch (AuthorizationException e)
		{
		  //then
		  string message = e.Message;
		  AssertTextPresent("ENGINE-03029 Required authenticated group 'camunda-admin'", message);

		}

		DeleteDeployment(deploymentId);
	  }

	  public virtual void testRegisterProcessApplicationAsCamundaAdmin()
	  {
		// given
		identityService.SetAuthentication(userId, new List<string>(){GroupsFields.CamundaAdmin});

		EmbeddedProcessApplication processApplication = new EmbeddedProcessApplication();
		IProcessApplicationReference reference = processApplication.Reference;
		string deploymentId = createDeployment(null, FIRST_RESOURCE).Id;

		// when
		IProcessApplicationRegistration registration = managementService.RegisterProcessApplication(deploymentId, reference);

		// then
		Assert.NotNull(registration);
		Assert.NotNull(GetProcessApplicationForDeployment(deploymentId));

		DeleteDeployment(deploymentId);
	  }

	  // unregister process application ///////////////////////////////////

	  public virtual void testUnregisterProcessApplicationWithoutAuthorization()
	  {
		// given
		EmbeddedProcessApplication processApplication = new EmbeddedProcessApplication();
		string deploymentId = createDeployment(null, FIRST_RESOURCE).Id;
		IProcessApplicationReference reference = processApplication.Reference;
		RegisterProcessApplication(deploymentId, reference);

		try
		{
		  // when
		  managementService.UnregisterProcessApplication(deploymentId, true);
		  Assert.Fail("Exception expected: It should not be possible to unregister a process application");
		}
		catch (AuthorizationException e)
		{
		  //then
		  string message = e.Message;
		  AssertTextPresent("ENGINE-03029 Required authenticated group 'camunda-admin'", message);

		}

		DeleteDeployment(deploymentId);
	  }

	  public virtual void testUnregisterProcessApplicationAsCamundaAdmin()
	  {
		// given
		identityService.SetAuthentication(userId, new List<string>(){GroupsFields.CamundaAdmin});

		EmbeddedProcessApplication processApplication = new EmbeddedProcessApplication();
		string deploymentId = createDeployment(null, FIRST_RESOURCE).Id;
		IProcessApplicationReference reference = processApplication.Reference;
		RegisterProcessApplication(deploymentId, reference);

		// when
		managementService.UnregisterProcessApplication(deploymentId, true);

		// then
		Assert.IsNull(GetProcessApplicationForDeployment(deploymentId));

		DeleteDeployment(deploymentId);
	  }

	  // get process application for deployment ///////////////////////////////////

	  public virtual void testGetProcessApplicationForDeploymentWithoutAuthorization()
	  {
		// given
		EmbeddedProcessApplication processApplication = new EmbeddedProcessApplication();
		string deploymentId = createDeployment(null, FIRST_RESOURCE).Id;
		IProcessApplicationReference reference = processApplication.Reference;
		RegisterProcessApplication(deploymentId, reference);

		try
		{
		  // when
		  managementService.GetProcessApplicationForDeployment(deploymentId);
		  Assert.Fail("Exception expected: It should not be possible to get the process application");
		}
		catch (AuthorizationException e)
		{
		  //then
		  string message = e.Message;
		  AssertTextPresent("ENGINE-03029 Required authenticated group 'camunda-admin'", message);

		}

		DeleteDeployment(deploymentId);
	  }

	  public virtual void testGetProcessApplicationForDeploymentAsCamundaAdmin()
	  {
		// given
		identityService.SetAuthentication(userId, new List<string>(){GroupsFields.CamundaAdmin});

		EmbeddedProcessApplication processApplication = new EmbeddedProcessApplication();
		string deploymentId = createDeployment(null, FIRST_RESOURCE).Id;
		IProcessApplicationReference reference = processApplication.Reference;
		RegisterProcessApplication(deploymentId, reference);

		// when
		string application = managementService.GetProcessApplicationForDeployment(deploymentId);

		// then
		Assert.NotNull(application);

		DeleteDeployment(deploymentId);
	  }

	  // get registered deployments ///////////////////////////////////

	  public virtual void testGetRegisteredDeploymentsWithoutAuthorization()
	  {
		// given
		string deploymentId = createDeployment(null, FIRST_RESOURCE).Id;

		try
		{
		  // when
		  var obj = managementService.RegisteredDeployments;
		  Assert.Fail("Exception expected: It should not be possible to get the registered deployments");
		}
		catch (AuthorizationException e)
		{
		  //then
		  string message = e.Message;
		  AssertTextPresent("ENGINE-03029 Required authenticated group 'camunda-admin'", message);

		}

		DeleteDeployment(deploymentId);
	  }

	  public virtual void testGetRegisteredDeploymentsAsCamundaAdmin()
	  {
		// given
		identityService.SetAuthentication(userId, new List<string>(){GroupsFields.CamundaAdmin});

		string deploymentId = createDeployment(null, FIRST_RESOURCE).Id;

		// when
		IList<string> deployments = managementService.RegisteredDeployments;

		// then
		Assert.True(deployments.Contains(deploymentId));

		DeleteDeployment(deploymentId);
	  }

	  // register deployment for job executor ///////////////////////////////////

	  public virtual void testRegisterDeploymentForJobExecutorWithoutAuthorization()
	  {
		// given
		string deploymentId = createDeployment(null, FIRST_RESOURCE).Id;

		try
		{
		  // when
		  managementService.RegisterDeploymentForJobExecutor(deploymentId);
		  Assert.Fail("Exception expected: It should not be possible to register the deployment");
		}
		catch (AuthorizationException e)
		{
		  //then
		  string message = e.Message;
		  AssertTextPresent("ENGINE-03029 Required authenticated group 'camunda-admin'", message);

		}

		DeleteDeployment(deploymentId);
	  }

	  public virtual void testRegisterDeploymentForJobExecutorAsCamundaAdmin()
	  {
		// given
		identityService.SetAuthentication(userId, new List<string>(){GroupsFields.CamundaAdmin});

		string deploymentId = createDeployment(null, FIRST_RESOURCE).Id;

		// when
		managementService.RegisterDeploymentForJobExecutor(deploymentId);

		// then
		Assert.True(RegisteredDeployments.Contains(deploymentId));

		DeleteDeployment(deploymentId);
	  }

	  // unregister deployment for job executor ///////////////////////////////////

	  public virtual void testUnregisterDeploymentForJobExecutorWithoutAuthorization()
	  {
		// given
		string deploymentId = createDeployment(null, FIRST_RESOURCE).Id;

		try
		{
		  // when
		  managementService.UnregisterDeploymentForJobExecutor(deploymentId);
		  Assert.Fail("Exception expected: It should not be possible to unregister the deployment");
		}
		catch (AuthorizationException e)
		{
		  //then
		  string message = e.Message;
		  AssertTextPresent("ENGINE-03029 Required authenticated group 'camunda-admin'", message);

		}

		DeleteDeployment(deploymentId);
	  }

	  public virtual void testUnregisterDeploymentForJobExecutorAsCamundaAdmin()
	  {
		// given
		identityService.SetAuthentication(userId, new List<string>(){GroupsFields.CamundaAdmin});

		string deploymentId = createDeployment(null, FIRST_RESOURCE).Id;

		// when
		managementService.UnregisterDeploymentForJobExecutor(deploymentId);

		// then
		Assert.IsFalse(RegisteredDeployments.Contains(deploymentId));

		DeleteDeployment(deploymentId);
	  }

	  // helper /////////////////////////////////////////////////////////

	  protected internal virtual void verifyQueryResults( IQueryable<IDeployment> query, int countExpected)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: //verifyQueryResults((org.camunda.bpm.Engine.impl.AbstractQuery<?, ?>) query, countExpected);
		////verifyQueryResults((AbstractQuery<object, object>) query, countExpected);
	  }

	  protected internal virtual string createDeployment(string name)
	  {
		return createDeployment(name, FIRST_RESOURCE, SECOND_RESOURCE).Id;
	  }

	  protected internal virtual void RegisterProcessApplication(string deploymentId, IProcessApplicationReference reference)
	  {
		disableAuthorization();
		managementService.RegisterProcessApplication(deploymentId, reference);
		enableAuthorization();
	  }

	  protected internal virtual string GetProcessApplicationForDeployment(string deploymentId)
	  {
		disableAuthorization();
		string applications = managementService.GetProcessApplicationForDeployment(deploymentId);
		enableAuthorization();
		return applications;
	  }

	  protected internal virtual IList<string> RegisteredDeployments
	  {
		  get
		  {
			disableAuthorization();
                IList<string> deployments = managementService.RegisteredDeployments;
			enableAuthorization();
			return deployments;
		  }
	  }

	}

}