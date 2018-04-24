using ESS.FW.Bpm.Engine.EntityFramework;
using ESS.FW.Common.Autofac;
using ESS.FW.DataAccess.EF;
using NUnit.Framework;

namespace Engine.Tests
{

    [SetUpFixture]
    public class Startup
    {
        public Startup()
        {
            var conf = ESS.FW.Common.Configurations.Configuration.Create()
                .UseAutofac()
                .RegisterCommonComponents()
                .RegisterComponents(c => c.Contains("ESS"))
                //.UseInMemoryRepository()
                //.UseEfBpmRepository(typeof(BpmDbContext), false)
                //.UseMassTransit();
                ;

        }
    }
}