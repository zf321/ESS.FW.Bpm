using System.Collections.Generic;
using System.Text;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;
using System.Collections;
using ESS.FW.Common.Components;
using ESS.FW.Bpm.Engine.Impl.EL;

namespace ESS.FW.Bpm.Engine.Application.Impl
{
    /// <summary>
    ///     
    /// </summary>
    public class DefaultElResolverLookup
    {
        private static readonly ProcessApplicationLogger Log = ProcessEngineLogger.ProcessApplicationLogger;

        public static ELResolver LookupResolver( )
        {
            //IEnumerable<IProcessApplicationElResolver> providers = new List<IProcessApplicationElResolver>();
            //using (var scope = ObjectContainer.BeginLifetimeScope())
            //{
            //    providers = scope.Resolve<IEnumerable<IProcessApplicationElResolver>>();
            //}
            //IList<IProcessApplicationElResolver> sortedProviders = new List<IProcessApplicationElResolver>();
            //foreach (IProcessApplicationElResolver provider in providers)
            //{
            //    sortedProviders.Add(provider);
            //}

            //if (sortedProviders.Count == 0)
            //{
            //    return null;
            //}
            // sort providers first
            //sortedProviders.Sort(new ProcessApplicationElResolverProcessApplicationElResolverSorter());

            // add all providers to a composite resolver
            var compositeResolver = new CompositeELResolver();
            //var summary = new StringBuilder();
            //summary.Append(string.Format("ElResolvers found for Process Application {0}", processApplication.Name));

            //foreach (var processApplicationElResolver in sortedProviders)
            //{
            //    var elResolver = processApplicationElResolver.GetElResolver(processApplication);

            //    if (elResolver != null)
            //    {
            //        compositeResolver.Add(elResolver);
            //        summary.Append(string.Format("Class {0}", processApplicationElResolver.GetType().FullName));
            //    }
            //    else
            //    {
            //        Log.NoElResolverProvided(processApplication.Name, processApplicationElResolver.GetType().FullName);
            //    }
            //}

            //Log.PaElResolversDiscovered(summary.ToString());


            compositeResolver.Add(new ProcessApplicationElResolver());


            return compositeResolver;
        }
    }
}