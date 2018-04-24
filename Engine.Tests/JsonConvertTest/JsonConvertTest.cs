using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Common.Components;
using ESS.FW.DataAccess;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Engine.Tests.JsonConvertTest
{
    public class JsonConvertTest
    {
        [Test]
        public void DeploymentEntityConvertTest()
        {
            using (var scope = ObjectContainer.BeginLifetimeScope())
            {
                var repository = scope.Resolve<IRepository<DeploymentEntity, string>>();
                var depls = repository.GetAll().ToList();
                var str = JsonConvert.SerializeObject(depls);
                var obj = JsonConvert.DeserializeObject<IList<DeploymentEntity>>(str);
            }
        }
    }
}
