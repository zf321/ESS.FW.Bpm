using System.Collections.Generic;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Common.Utilities;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization.Util
{
    /// <summary>
    /// </summary>
    public class AuthorizationScenarioInstance
    {
        protected internal IList<IAuthorization> createdAuthorizations = new List<IAuthorization>();
        protected internal IList<IAuthorization> missingAuthorizations = new List<IAuthorization>();

        protected internal AuthorizationScenario scenario;

        public AuthorizationScenarioInstance(AuthorizationScenario scenario, IAuthorizationService authorizationService,
            IDictionary<string, string> resourceBindings)
        {
            this.scenario = scenario;
            init(authorizationService, resourceBindings);
        }

        public virtual void init(IAuthorizationService authorizationService,
            IDictionary<string, string> resourceBindings)
        {
            foreach (var authorizationSpec in scenario.givenAuthorizations)
            {
                var authorization = authorizationSpec.Instantiate(authorizationService, resourceBindings);
                authorizationService.SaveAuthorization(authorization);
                createdAuthorizations.Add(authorization);
            }

            foreach (var authorizationSpec in scenario.missingAuthorizations)
            {
                var authorization = authorizationSpec.Instantiate(authorizationService, resourceBindings);
                missingAuthorizations.Add(authorization);
            }
        }

        public virtual void tearDown(IAuthorizationService authorizationService)
        {
            ISet<string> activeAuthorizations = new HashSet<string>();
            //foreach (IAuthorization activeAuthorization in authorizationService.CreateAuthorizationQuery()
            //    )
            //    activeAuthorizations.Add(activeAuthorization.Id);

            foreach (var createdAuthorization in createdAuthorizations)
                if (activeAuthorizations.Contains(createdAuthorization.Id))
                    authorizationService.DeleteAuthorization(createdAuthorization.Id);
        }

        public virtual void AssertAuthorizationException(AuthorizationException e)
        {
            if (missingAuthorizations.Count > 0 && e != null)
            {
                var message = e.Message;
                var AssertionFailureMessage =
                    DescribeScenarioFailure("Expected an authorization exception but the message was wrong: " +
                                            e.Message);

                IList<MissingAuthorization> actualMissingAuthorizations =
                    new List<MissingAuthorization>(e.MissingAuthorizations);
                //IList<MissingAuthorization> expectedMissingAuthorizations = MissingAuthorizationMatcher.asMissingAuthorizations(missingAuthorizations);

                //Assert.That(actualMissingAuthorizations, Is.EquivalentTo(expectedMissingAuthorizations));

                foreach (var missingAuthorization in missingAuthorizations)
                {
                    Assert.True(message.Contains(missingAuthorization.UserId), AssertionFailureMessage);
                    Assert.AreEqual(missingAuthorization.UserId, e.UserId);

                    foreach (var permission in missingAuthorization.GetPermissions((Permissions[]) typeof(Permissions).GetEnums()))
                        if (permission != Permissions.None)
                            Assert.True(message.Contains(permission.ToString()), AssertionFailureMessage);

                    if (!AuthorizationFields.Any.Equals(missingAuthorization.ResourceId))
                        Assert.True(message.Contains(missingAuthorization.ResourceId), AssertionFailureMessage);

                    //Resources resource = AuthorizationTestUtil.GetResourceByType(missingAuthorization.ResourceType);
                    //Assert.True( message.Contains(resource.ToString()/*.ResourceName()*/), AssertionFailureMessage);
                }
            }
            else if (missingAuthorizations.Count == 0 && e == null)
            {
                // nothing to do
            }
            else
            {
                if (e != null)
                    Assert.Fail(DescribeScenarioFailure("Expected no authorization exception but got one: " + e.Message));
                else
                    Assert.Fail(
                        DescribeScenarioFailure(
                            "Expected failure due to missing authorizations but code under test was successful"));
            }
        }

        protected internal virtual string DescribeScenarioFailure(string message)
        {
            return message + "\n" + "\n" + "Scenario: \n" + scenario;
        }
    }
}