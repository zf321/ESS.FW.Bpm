using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Authorization;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization.Util
{

    /// <summary>
    /// 
    /// 
    /// </summary>
    public class AuthorizationTestUtil
    {


        protected internal static IDictionary<int?, Resources> resourcesByType = new Dictionary<int?, Resources>();

        static AuthorizationTestUtil()
        {
            //foreach (Resources resource in Resources.values())
            //{
            //  resourcesByType[resource.ResourceType()] = resource;
            //}
        }

        public static Resources GetResourceByType(int type)
        {
            return resourcesByType[type];
        }

        /// <summary>
        /// Checks if the info has the expected parameters.
        /// </summary>
        /// <param name="expectedPermissionName"> to use </param>
        /// <param name="expectedResourceName"> to use </param>
        /// <param name="expectedResourceId"> to use </param>
        /// <param name="info"> to check </param>
        public static void AssertExceptionInfo(string expectedPermissionName, string expectedResourceName, string expectedResourceId, MissingAuthorization info)
        {
            Assert.AreEqual(expectedPermissionName, info.ViolatedPermissionName);
            Assert.AreEqual(expectedResourceName, info.ResourceType);
            Assert.AreEqual(expectedResourceId, info.ResourceId);
        }
    }

}