using System.Collections.Generic;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.History.Impl;
using ESS.FW.Bpm.Engine.Persistence;
using ESS.FW.Common.Components;
using ESS.FW.DataAccess;
using NUnit.Framework;

namespace Engine.Tests
{
    /// <summary>
    ///     Base class for the process engine test cases.
    ///     The main reason not to use our own test support classes is that we need to
    ///     run our test suite with various configurations, e.g. with and without spring,
    ///     standalone or on a server etc.  Those requirements create some complications
    ///     so we think it's best to use a separate base class.  That way it is much easier
    ///     for us to maintain our own codebase and at the same time provide stability
    ///     on the test support classes that we offer as part of our api (in test).
    /// </summary>
    [TestFixture]
    public class PluggableProcessEngineTestCase : AbstractProcessEngineTestCase
    {
        protected internal static IProcessEngine cachedProcessEngine;
        protected internal override void InitializeProcessEngine()
        {
            base.ProcessEngine = OrInitializeCachedProcessEngine;
        }
        
        private static IProcessEngine OrInitializeCachedProcessEngine
        {
            get
            {
                if (cachedProcessEngine == null)
                    //try
                    //{
                        cachedProcessEngine =
                            ESS.FW.Bpm.Engine.ProcessEngineConfiguration.CreateStandaloneInMemProcessEngineConfiguration().BuildProcessEngine();
                cachedProcessEngine.ProcessEngineConfiguration.HistoryLevel = new HistoryLevelFull();

                ObjectContainer.Current.RegisterGeneric(typeof(IRepository<,>), typeof(AbstractManagerNet<>),
                    LifeStyle.Transient);
                //cachedProcessEngine =
                //    Engine.ProcessEngineConfiguration.CreateProcessEngineConfigurationFromResource(
                //        "camunda.cfg.xml").BuildProcessEngine();
                //}
                //catch (Exception ex)
                //{
                //    if ((ex.InnerException != null) && ex.InnerException is FileNotFoundException)
                //        cachedProcessEngine =
                //            Engine.ProcessEngineConfiguration.CreateProcessEngineConfigurationFromResource(
                //                "activiti.cfg.xml").BuildProcessEngine();
                //    else
                //        throw ex;
                //}
                return cachedProcessEngine;
            }
        }

        public new static IProcessEngine ProcessEngine
        {
            get { return OrInitializeCachedProcessEngine; }
        }
    }
}