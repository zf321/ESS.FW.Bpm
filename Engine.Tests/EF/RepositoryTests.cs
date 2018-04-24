//using System.Linq;
//using ESS.Common.Shared.Entities.Bpm.DynamicForms;
//using ESS.FW.Bpm.Engine.Dmn.Impl.Entity.Repository;
//using ESS.FW.Bpm.Engine.History.Impl.Event;
//using ESS.FW.Common.Autofac.Test;
//using ESS.FW.DataAccess;
//using NUnit.Framework;

//namespace Engine.Tests.EF
//{
//    [TestFixture]
//    public class RepositoryTests : TestBase
//    {
//        [Test]
//        public void HistoricJobLogEventEntityRepositoryTest()
//        {
//            var repository = Scope.Resolve<IRepository<HistoricJobLogEventEntity, string>>();
//            repository.First(c => true);
//        }

//        [Test]
//        public void DecisionDefinitionEntityRepositoryTest()
//        {
//            var repository = Scope.Resolve<IRepository<DecisionDefinitionEntity, string>>();
//            repository.First(c => true);
//        }
//        [Test]
//        public void DecisionRequirementsDefinitionEntityRepositoryTest()
//        {
//            var repository = Scope.Resolve<IRepository<DecisionRequirementsDefinitionEntity, string>>();
//            repository.First(c => true);
//        }
        
//        [Test]
//        public void DyFormTest()
//        {
//            var form = Scope.Resolve<IRepository<DyForm, string>>();
//            var test= form.Find(m=>m.Id!=null).ToList();
//            form.Delete(m => m.Id != null);
//        }
//        [Test]
//        public void DyFromElTest()
//        {
//            var form = Scope.Resolve<IRepository<DyFormElement, string>>();
//            var test = form.Find(m => m.Id != null).ToList();
//        }
//        [Test]
//        public void DyFromValiTest()
//        {
//            var form = Scope.Resolve<IRepository<DyFormValidator, string>>();
//            var test = form.Find(m => m.Id != null).ToList();
            

//        }
//    }
//}
