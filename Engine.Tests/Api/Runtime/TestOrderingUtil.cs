using System;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Batch;
using ESS.FW.Bpm.Engine.Batch.History;
using ESS.FW.Bpm.Engine.Externaltask;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime
{
    /// <summary>
    ///     This class provides utils to verify the sorting of queries of engine entities.
    ///     Assuming we sort over a property x, there are two valid orderings when some entities
    ///     have values where x = null: Either, these values precede the overall list, or they trail it.
    ///     Thus, this class does not use regular comparators but a <seealso cref="NullTolerantComparator" />
    ///     that can be used to Assert a list of entites in both ways.
    /// </summary>
    public class TestOrderingUtil
    {
        // EXECUTION

        //public static NullTolerantComparator<IExecution> executionByProcessInstanceId()
        //{
        //    return propertyComparator(new PropertyAccessorAnonymousInnerClass());
        //}

        //public static NullTolerantComparator<IExecution> executionByProcessDefinitionId()
        //{
        //    return propertyComparator(new PropertyAccessorAnonymousInnerClass2());
        //}

        public static NullTolerantComparator<IExecution> executionByProcessDefinitionKey(IProcessEngine processEngine)
        {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.Camunda.bpm.Engine.IRuntimeService runtimeService = processEngine.GetRuntimeService();
            var runtimeService = processEngine.RuntimeService;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.Camunda.bpm.Engine.RepositoryService repositoryService = processEngine.GetRepositoryService();
            var repositoryService = processEngine.RepositoryService;
            return null;
            //return propertyComparator(new PropertyAccessorAnonymousInnerClass3(runtimeService, repositoryService));
        }

        //PROCESS INSTANCE

        //public static NullTolerantComparator<IProcessInstance> processInstanceByProcessInstanceId()
        //{
        //    //return propertyComparator(new PropertyAccessorAnonymousInnerClass4());
        //}

        //public static NullTolerantComparator<IProcessInstance> processInstanceByProcessDefinitionId()
        //{
        //    return propertyComparator(new PropertyAccessorAnonymousInnerClass5());
        //}

        //public static NullTolerantComparator<IProcessInstance> processInstanceByBusinessKey()
        //{
        //    return propertyComparator(new PropertyAccessorAnonymousInnerClass6());
        //}

        //HISTORIC PROCESS INSTANCE

        //public static NullTolerantComparator<IHistoricProcessInstance> historicProcessInstanceByProcessDefinitionId()
        //{
        //    return propertyComparator(new PropertyAccessorAnonymousInnerClass7());
        //}

        //public static NullTolerantComparator<IHistoricProcessInstance> historicProcessInstanceByProcessDefinitionKey()
        //{
        //    return propertyComparator(new PropertyAccessorAnonymousInnerClass8());
        //}

        //public static NullTolerantComparator<IHistoricProcessInstance> historicProcessInstanceByProcessDefinitionName()
        //{
        //    return propertyComparator(new PropertyAccessorAnonymousInnerClass9());
        //}

        //public static NullTolerantComparator<IHistoricProcessInstance> historicProcessInstanceByProcessDefinitionVersion
        //    ()
        //{
        //    return propertyComparator(new PropertyAccessorAnonymousInnerClass10());
        //}

        //public static NullTolerantComparator<IHistoricProcessInstance> historicProcessInstanceByProcessInstanceId()
        //{
        //    return propertyComparator(new PropertyAccessorAnonymousInnerClass11());
        //}

        // CASE EXECUTION

        //        public static NullTolerantComparator<ICaseExecution> caseExecutionByDefinitionId()
        //        {
        //            return propertyComparator(new PropertyAccessorAnonymousInnerClass12());
        //        }

        //        public static NullTolerantComparator<ICaseExecution> caseExecutionByDefinitionKey(IProcessEngine processEngine)
        //        {
        ////JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
        ////ORIGINAL LINE: final org.Camunda.bpm.Engine.RepositoryService repositoryService = processEngine.GetRepositoryService();
        //            IRepositoryService repositoryService = processEngine.RepositoryService;
        //            return propertyComparator(new PropertyAccessorAnonymousInnerClass13(repositoryService));
        //        }

        //        public static NullTolerantComparator<ICaseExecution> caseExecutionById()
        //        {
        //            return propertyComparator(new PropertyAccessorAnonymousInnerClass14());
        //        }

        // Resources.Task

        //public static NullTolerantComparator<ITask> taskById()
        //{
        //    return propertyComparator(new PropertyAccessorAnonymousInnerClass15());
        //}

        //public static NullTolerantComparator<ITask> taskByName()
        //{
        //    return propertyComparator(new PropertyAccessorAnonymousInnerClass16());
        //}

        //public static NullTolerantComparator<ITask> taskByPriority()
        //{
        //    return propertyComparator(new PropertyAccessorAnonymousInnerClass17());
        //}

        //public static NullTolerantComparator<ITask> taskByAssignee()
        //{
        //    return propertyComparator(new PropertyAccessorAnonymousInnerClass18());
        //}

        //public static NullTolerantComparator<ITask> taskByDescription()
        //{
        //    return propertyComparator(new PropertyAccessorAnonymousInnerClass19());
        //}

        //public static NullTolerantComparator<ITask> taskByProcessInstanceId()
        //{
        //    return propertyComparator(new PropertyAccessorAnonymousInnerClass20());
        //}

        //public static NullTolerantComparator<ITask> taskByExecutionId()
        //{
        //    return propertyComparator(new PropertyAccessorAnonymousInnerClass21());
        //}

        //public static NullTolerantComparator<ITask> taskByCreateTime()
        //{
        //    return propertyComparator(new PropertyAccessorAnonymousInnerClass22());
        //}

        //public static NullTolerantComparator<ITask> taskByDueDate()
        //{
        //    return propertyComparator(new PropertyAccessorAnonymousInnerClass23());
        //}

        //public static NullTolerantComparator<ITask> taskByFollowUpDate()
        //{
        //    return propertyComparator(new PropertyAccessorAnonymousInnerClass24());
        //}

        //public static NullTolerantComparator<ITask> taskByCaseInstanceId()
        //{
        //    return propertyComparator(new PropertyAccessorAnonymousInnerClass25());
        //}

        //public static NullTolerantComparator<ITask> taskByCaseExecutionId()
        //{
        //    return propertyComparator(new PropertyAccessorAnonymousInnerClass26());
        //}

        // HISTORIC JOB LOG

        //public static NullTolerantComparator<IHistoricJobLog> historicJobLogByTimestamp()
        //{
        //    return propertyComparator(new PropertyAccessorAnonymousInnerClass27());
        //}

        //public static NullTolerantComparator<IHistoricJobLog> historicJobLogByJobId()
        //{
        //    return propertyComparator(new PropertyAccessorAnonymousInnerClass28());
        //}

        //public static NullTolerantComparator<IHistoricJobLog> historicJobLogByJobDefinitionId()
        //{
        //    return propertyComparator(new PropertyAccessorAnonymousInnerClass29());
        //}

        //public static NullTolerantComparator<IHistoricJobLog> historicJobLogByJobDueDate()
        //{
        //    return propertyComparator(new PropertyAccessorAnonymousInnerClass30());
        //}

        //public static NullTolerantComparator<IHistoricJobLog> historicJobLogByJobRetries()
        //{
        //    return propertyComparator(new PropertyAccessorAnonymousInnerClass31());
        //}

        //public static NullTolerantComparator<IHistoricJobLog> historicJobLogByActivityId()
        //{
        //    return propertyComparator(new PropertyAccessorAnonymousInnerClass32());
        //}

        //public static NullTolerantComparator<IHistoricJobLog> historicJobLogByExecutionId()
        //{
        //    return propertyComparator(new PropertyAccessorAnonymousInnerClass33());
        //}

        //public static NullTolerantComparator<IHistoricJobLog> historicJobLogByProcessInstanceId()
        //{
        //    return propertyComparator(new PropertyAccessorAnonymousInnerClass34());
        //}

        //public static NullTolerantComparator<IHistoricJobLog> historicJobLogByProcessDefinitionId()
        //{
        //    return propertyComparator(new PropertyAccessorAnonymousInnerClass35());
        //}

        //        public static NullTolerantComparator<IHistoricJobLog> historicJobLogByProcessDefinitionKey(
        //            IProcessEngine processEngine)
        //        {
        ////JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
        ////ORIGINAL LINE: final org.Camunda.bpm.Engine.RepositoryService repositoryService = processEngine.GetRepositoryService();
        //            IRepositoryService repositoryService = processEngine.RepositoryService;

        //            return propertyComparator(new PropertyAccessorAnonymousInnerClass36(repositoryService));
        //        }

        //        public static NullTolerantComparator<IHistoricJobLog> historicJobLogByDeploymentId()
        //        {
        //            return propertyComparator(new PropertyAccessorAnonymousInnerClass37());
        //        }

        //        public static NullTolerantComparator<IHistoricJobLog> historicJobLogByJobPriority()
        //        {
        //            return propertyComparator(new PropertyAccessorAnonymousInnerClass38());
        //        }

        //        public static NullTolerantComparator<IHistoricJobLog> historicJobLogPartiallyByOccurence()
        //        {
        //            return propertyComparator(new PropertyAccessorAnonymousInnerClass39());
        //        }

        //        // jobs

        //        public static NullTolerantComparator<IJob> jobByPriority()
        //        {
        //            return propertyComparator(new PropertyAccessorAnonymousInnerClass40());
        //        }

        //        // external task

        //        public static NullTolerantComparator<IExternalTask> externalTaskById()
        //        {
        //            return propertyComparator(new PropertyAccessorAnonymousInnerClass41());
        //        }

        //        public static NullTolerantComparator<IExternalTask> externalTaskByProcessInstanceId()
        //        {
        //            return propertyComparator(new PropertyAccessorAnonymousInnerClass42());
        //        }

        //        public static NullTolerantComparator<IExternalTask> externalTaskByProcessDefinitionId()
        //        {
        //            return propertyComparator(new PropertyAccessorAnonymousInnerClass43());
        //        }

        //        public static NullTolerantComparator<IExternalTask> externalTaskByProcessDefinitionKey()
        //        {
        //            return propertyComparator(new PropertyAccessorAnonymousInnerClass44());
        //        }

        //        public static NullTolerantComparator<IExternalTask> externalTaskByLockExpirationTime()
        //        {
        //            return propertyComparator(new PropertyAccessorAnonymousInnerClass45());
        //        }

        //        public static NullTolerantComparator<IExternalTask> externalTaskByPriority()
        //        {
        //            return propertyComparator(new PropertyAccessorAnonymousInnerClass46());
        //        }

        // batch

        //public static NullTolerantComparator<IBatch> batchById()
        //{
        //    return propertyComparator(new PropertyAccessorAnonymousInnerClass47());
        //}

        public static NullTolerantComparator<IBatch> batchByTenantId()
        {
            return null; //propertyComparator(new PropertyAccessorAnonymousInnerClass48());
        }

        //public static NullTolerantComparator<IHistoricBatch> historicBatchById()
        //{
        //    return propertyComparator(new PropertyAccessorAnonymousInnerClass49());
        //}

        public static NullTolerantComparator<IHistoricBatch> historicBatchByTenantId()
        {
            return null; //propertyComparator(new PropertyAccessorAnonymousInnerClass50());
        }

        //public static NullTolerantComparator<IHistoricBatch> historicBatchByStartTime()
        //{
        //    return propertyComparator(new PropertyAccessorAnonymousInnerClass51());
        //}

        //public static NullTolerantComparator<IHistoricBatch> historicBatchByEndTime()
        //{
        //    return propertyComparator(new PropertyAccessorAnonymousInnerClass52());
        //}

        //public static NullTolerantComparator<IBatchStatistics> batchStatisticsById()
        //{
        //    return propertyComparator(new PropertyAccessorAnonymousInnerClass53());
        //}

        public static NullTolerantComparator<IBatchStatistics> batchStatisticsByTenantId()
        {
            return null; //propertyComparator(new PropertyAccessorAnonymousInnerClass54());
        }

        // HISTORIC EXTERNAL Resources.Task LOG

        //        public static NullTolerantComparator<IHistoricExternalTaskLog> historicExternalTaskByTimestamp()
        //        {
        //            return propertyComparator(new PropertyAccessorAnonymousInnerClass55());
        //        }

        //        public static NullTolerantComparator<IHistoricExternalTaskLog> historicExternalTaskLogByExternalTaskId()
        //        {
        //            return propertyComparator(new PropertyAccessorAnonymousInnerClass56());
        //        }

        //        public static NullTolerantComparator<IHistoricExternalTaskLog> historicExternalTaskLogByRetries()
        //        {
        //            return propertyComparator(new PropertyAccessorAnonymousInnerClass57());
        //        }

        //        public static NullTolerantComparator<IHistoricExternalTaskLog> historicExternalTaskLogByPriority()
        //        {
        //            return propertyComparator(new PropertyAccessorAnonymousInnerClass58());
        //        }

        //        public static NullTolerantComparator<IHistoricExternalTaskLog> historicExternalTaskLogByTopicName()
        //        {
        //            return propertyComparator(new PropertyAccessorAnonymousInnerClass59());
        //        }

        //        public static NullTolerantComparator<IHistoricExternalTaskLog> historicExternalTaskLogByWorkerId()
        //        {
        //            return propertyComparator(new PropertyAccessorAnonymousInnerClass60());
        //        }

        //        public static NullTolerantComparator<IHistoricExternalTaskLog> historicExternalTaskLogByActivityId()
        //        {
        //            return propertyComparator(new PropertyAccessorAnonymousInnerClass61());
        //        }

        //        public static NullTolerantComparator<IHistoricExternalTaskLog> historicExternalTaskLogByActivityInstanceId()
        //        {
        //            return propertyComparator(new PropertyAccessorAnonymousInnerClass62());
        //        }

        //        public static NullTolerantComparator<IHistoricExternalTaskLog> historicExternalTaskLogByExecutionId()
        //        {
        //            return propertyComparator(new PropertyAccessorAnonymousInnerClass63());
        //        }

        //        public static NullTolerantComparator<IHistoricExternalTaskLog> historicExternalTaskLogByProcessInstanceId()
        //        {
        //            return propertyComparator(new PropertyAccessorAnonymousInnerClass64());
        //        }

        //        public static NullTolerantComparator<IHistoricExternalTaskLog> historicExternalTaskLogByProcessDefinitionId()
        //        {
        //            return propertyComparator(new PropertyAccessorAnonymousInnerClass65());
        //        }

        //        public static NullTolerantComparator<IHistoricExternalTaskLog> historicExternalTaskLogByProcessDefinitionKey(
        //            IProcessEngine processEngine)
        //        {
        //            JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
        //ORIGINAL LINE: final org.Camunda.bpm.Engine.RepositoryService repositoryService = processEngine.GetRepositoryService();
        //            IRepositoryService repositoryService = processEngine.RepositoryService;

        //            return propertyComparator(new PropertyAccessorAnonymousInnerClass66(repositoryService));
        //        }

        //        public static NullTolerantComparator<IHistoricExternalTaskLog> historicExternalTaskLogByTenantId()
        //        {
        //            return propertyComparator(new PropertyAccessorAnonymousInnerClass67());
        //        }

        // general

        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: public static <T, P extends Comparable<P>> NullTolerantComparator<T> propertyComparator(final PropertyAccessor<T, P> accessor)
        //public static NullTolerantComparator<T> propertyComparator<T, P>(PropertyAccessor<T, P> accessor)
        //    where P : IComparable<P>
        //{
        //    return new NullTolerantComparatorAnonymousInnerClass(accessor);
        //}


        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: public static <T> NullTolerantComparator<T> inverted(final NullTolerantComparator<T> comparator)
        public static NullTolerantComparator<T> inverted<T>(NullTolerantComparator<T> comparator)
        {
            return null; //new NullTolerantComparatorAnonymousInnerClass2(comparator);
        }


        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: public static <T> NullTolerantComparator<T> hierarchical(final NullTolerantComparator<T> baseComparator, final NullTolerantComparator<T>.. minorOrderings)
        //public static NullTolerantComparator<T> hierarchical<T>(NullTolerantComparator<T> baseComparator,
        //    params NullTolerantComparator<T>[] minorOrderings)
        //{
        //    return new NullTolerantComparatorAnonymousInnerClass3(baseComparator, minorOrderings);
        //}

        public static void verifySorting<T>(IList<T> actualElements, NullTolerantComparator<T> expectedOrdering)
        {
            // check two orderings: one in which values with null properties are at the front of the list
            var leadingNullOrdering = orderingConsistent(actualElements, expectedOrdering, true);

            if (leadingNullOrdering)
                return;

            // and one where the values with null properties are at the end of the list
            var trailingNullOrdering = orderingConsistent(actualElements, expectedOrdering, false);
            Assert.True(trailingNullOrdering, "Ordering not consistent with comparator");
        }

        public static bool orderingConsistent<T>(IList<T> actualElements, NullTolerantComparator<T> expectedOrdering,
            bool nullPrecedes)
        {
            for (var i = 0; i < actualElements.Count - 1; i++)
            {
                var currentExecution = actualElements[i];
                var nextExecution = actualElements[i + 1];

                var comparison = expectedOrdering.Compare(currentExecution, nextExecution);
                if (comparison > 0)
                    return false;
            }

            return true;
        }

        // Resources.Task

        public static NullTolerantComparator<ITask> taskById()
        {
            //return propertyComparator(new PropertyAccessorAnonymousInnerClass15());
            return null;
        }

        public static NullTolerantComparator<ITask> taskByName()
        {
            //return propertyComparator(new PropertyAccessorAnonymousInnerClass16());
            return null;
        }

        //public static void VerifySortingAndCount<T, T1>(Query<T1> query, int expectedCount,
        //    NullTolerantComparator<T> expectedOrdering)
        //{
        //    IList<T> elements = query.ToList();
        //    Assert.AreEqual(expectedCount, elements.Count);

        //    verifySorting(elements, expectedOrdering);
        //}

        private class PropertyAccessorAnonymousInnerClass : PropertyAccessor<IExecution, string>
        {
            public virtual string getProperty(IExecution obj)
            {
                return obj.ProcessInstanceId;
            }
        }

        private class PropertyAccessorAnonymousInnerClass2 : PropertyAccessor<IExecution, string>
        {
            public virtual string getProperty(IExecution obj)
            {
                return ((ExecutionEntity) obj).ProcessDefinitionId;
            }
        }

        private class PropertyAccessorAnonymousInnerClass3 : PropertyAccessor<IExecution, string>
        {
            private readonly IRepositoryService repositoryService;
            private readonly IRuntimeService runtimeService;

            public PropertyAccessorAnonymousInnerClass3(IRuntimeService runtimeService,
                IRepositoryService repositoryService)
            {
                this.runtimeService = runtimeService;
                this.repositoryService = repositoryService;
            }

            public virtual string getProperty(IExecution obj)
            {
                var processInstance =
                    runtimeService.CreateProcessInstanceQuery()
                        //.SetProcessInstanceId(obj.ProcessInstanceId)
                        .First();
                var processDefinition = repositoryService.GetProcessDefinition(processInstance.ProcessDefinitionId);
                return processDefinition.Key;
            }
        }

        private class PropertyAccessorAnonymousInnerClass4 : PropertyAccessor<IProcessInstance, string>
        {
            public virtual string getProperty(IProcessInstance obj)
            {
                return obj.ProcessInstanceId;
            }
        }

        private class PropertyAccessorAnonymousInnerClass5 : PropertyAccessor<IProcessInstance, string>
        {
            public virtual string getProperty(IProcessInstance obj)
            {
                return obj.ProcessDefinitionId;
            }
        }

        private class PropertyAccessorAnonymousInnerClass6 : PropertyAccessor<IProcessInstance, string>
        {
            public virtual string getProperty(IProcessInstance obj)
            {
                return obj.BusinessKey;
            }
        }

        private class PropertyAccessorAnonymousInnerClass7 : PropertyAccessor<IHistoricProcessInstance, string>
        {
            public virtual string getProperty(IHistoricProcessInstance obj)
            {
                return obj.ProcessDefinitionId;
            }
        }

        private class PropertyAccessorAnonymousInnerClass8 : PropertyAccessor<IHistoricProcessInstance, string>
        {
            public virtual string getProperty(IHistoricProcessInstance obj)
            {
                return obj.ProcessDefinitionKey;
            }
        }

        private class PropertyAccessorAnonymousInnerClass9 : PropertyAccessor<IHistoricProcessInstance, string>
        {
            public virtual string getProperty(IHistoricProcessInstance obj)
            {
                return obj.ProcessDefinitionName;
            }
        }

        private class PropertyAccessorAnonymousInnerClass10 : PropertyAccessor<IHistoricProcessInstance, int>
        {
            public virtual int getProperty(IHistoricProcessInstance obj)
            {
                return obj.ProcessDefinitionVersion ?? 0;
            }
        }

        private class PropertyAccessorAnonymousInnerClass11 : PropertyAccessor<IHistoricProcessInstance, string>
        {
            public virtual string getProperty(IHistoricProcessInstance obj)
            {
                return obj.Id;
            }
        }

        private class PropertyAccessorAnonymousInnerClass12 : PropertyAccessor<ICaseExecution, string>
        {
            public virtual string getProperty(ICaseExecution obj)
            {
                return obj.CaseDefinitionId;
            }
        }

        private class PropertyAccessorAnonymousInnerClass13 : PropertyAccessor<ICaseExecution, string>
        {
            private readonly IRepositoryService repositoryService;

            public PropertyAccessorAnonymousInnerClass13(IRepositoryService repositoryService)
            {
                this.repositoryService = repositoryService;
            }

            public virtual string getProperty(ICaseExecution obj)
            {
                var caseDefinition = repositoryService.GetCaseDefinition(obj.CaseDefinitionId);
                return caseDefinition.Key;
            }
        }

        private class PropertyAccessorAnonymousInnerClass14 : PropertyAccessor<ICaseExecution, string>
        {
            public virtual string getProperty(ICaseExecution obj)
            {
                return obj.Id;
            }
        }

        private class PropertyAccessorAnonymousInnerClass15 : PropertyAccessor<ITask, string>
        {
            public virtual string getProperty(ITask obj)
            {
                return obj.Id;
            }
        }

        private class PropertyAccessorAnonymousInnerClass16 : PropertyAccessor<ITask, string>
        {
            public virtual string getProperty(ITask obj)
            {
                return obj.Name;
            }
        }

        private class PropertyAccessorAnonymousInnerClass17 : PropertyAccessor<ITask, int>
        {
            public virtual int getProperty(ITask obj)
            {
                return obj.Priority;
            }
        }

        private class PropertyAccessorAnonymousInnerClass18 : PropertyAccessor<ITask, string>
        {
            public virtual string getProperty(ITask obj)
            {
                return obj.Assignee;
            }
        }

        private class PropertyAccessorAnonymousInnerClass19 : PropertyAccessor<ITask, string>
        {
            public virtual string getProperty(ITask obj)
            {
                return obj.Description;
            }
        }

        private class PropertyAccessorAnonymousInnerClass20 : PropertyAccessor<ITask, string>
        {
            public virtual string getProperty(ITask obj)
            {
                return obj.ProcessInstanceId;
            }
        }

        private class PropertyAccessorAnonymousInnerClass21 : PropertyAccessor<ITask, string>
        {
            public virtual string getProperty(ITask obj)
            {
                return obj.ExecutionId;
            }
        }

        private class PropertyAccessorAnonymousInnerClass22 : PropertyAccessor<ITask, DateTime>
        {
            public virtual DateTime getProperty(ITask obj)
            {
                return obj.CreateTime;
            }
        }

        private class PropertyAccessorAnonymousInnerClass23 : PropertyAccessor<ITask, DateTime>
        {
            public virtual DateTime getProperty(ITask obj)
            {
                return obj.DueDate.Value;
            }
        }

        private class PropertyAccessorAnonymousInnerClass24 : PropertyAccessor<ITask, DateTime>
        {
            public virtual DateTime getProperty(ITask obj)
            {
                return obj.FollowUpDate.Value;
            }
        }

        private class PropertyAccessorAnonymousInnerClass25 : PropertyAccessor<ITask, string>
        {
            public virtual string getProperty(ITask obj)
            {
                return obj.CaseInstanceId;
            }
        }

        private class PropertyAccessorAnonymousInnerClass26 : PropertyAccessor<ITask, string>
        {
            public virtual string getProperty(ITask obj)
            {
                return obj.CaseExecutionId;
            }
        }

        private class PropertyAccessorAnonymousInnerClass27 : PropertyAccessor<IHistoricJobLog, DateTime>
        {
            public virtual DateTime getProperty(IHistoricJobLog obj)
            {
                return obj.TimeStamp;
            }
        }

        private class PropertyAccessorAnonymousInnerClass28 : PropertyAccessor<IHistoricJobLog, string>
        {
            public virtual string getProperty(IHistoricJobLog obj)
            {
                return obj.JobId;
            }
        }

        private class PropertyAccessorAnonymousInnerClass29 : PropertyAccessor<IHistoricJobLog, string>
        {
            public virtual string getProperty(IHistoricJobLog obj)
            {
                return obj.JobDefinitionId;
            }
        }

        private class PropertyAccessorAnonymousInnerClass30 : PropertyAccessor<IHistoricJobLog, DateTime>
        {
            public virtual DateTime getProperty(IHistoricJobLog obj)
            {
                return (DateTime)obj.JobDueDate;
            }
        }

        private class PropertyAccessorAnonymousInnerClass31 : PropertyAccessor<IHistoricJobLog, int>
        {
            public virtual int getProperty(IHistoricJobLog obj)
            {
                return obj.JobRetries;
            }
        }

        private class PropertyAccessorAnonymousInnerClass32 : PropertyAccessor<IHistoricJobLog, string>
        {
            public virtual string getProperty(IHistoricJobLog obj)
            {
                return obj.ActivityId;
            }
        }

        private class PropertyAccessorAnonymousInnerClass33 : PropertyAccessor<IHistoricJobLog, string>
        {
            public virtual string getProperty(IHistoricJobLog obj)
            {
                return obj.ExecutionId;
            }
        }

        private class PropertyAccessorAnonymousInnerClass34 : PropertyAccessor<IHistoricJobLog, string>
        {
            public virtual string getProperty(IHistoricJobLog obj)
            {
                return obj.ProcessInstanceId;
            }
        }

        private class PropertyAccessorAnonymousInnerClass35 : PropertyAccessor<IHistoricJobLog, string>
        {
            public virtual string getProperty(IHistoricJobLog obj)
            {
                return obj.ProcessDefinitionId;
            }
        }

        private class PropertyAccessorAnonymousInnerClass36 : PropertyAccessor<IHistoricJobLog, string>
        {
            private readonly IRepositoryService repositoryService;

            public PropertyAccessorAnonymousInnerClass36(IRepositoryService repositoryService)
            {
                this.repositoryService = repositoryService;
            }

            public virtual string getProperty(IHistoricJobLog obj)
            {
                var processDefinition = repositoryService.GetProcessDefinition(obj.ProcessDefinitionId);
                return processDefinition.Key;
            }
        }

        private class PropertyAccessorAnonymousInnerClass37 : PropertyAccessor<IHistoricJobLog, string>
        {
            public virtual string getProperty(IHistoricJobLog obj)
            {
                return obj.DeploymentId;
            }
        }

        private class PropertyAccessorAnonymousInnerClass38 : PropertyAccessor<IHistoricJobLog, long>
        {
            public virtual long getProperty(IHistoricJobLog obj)
            {
                return obj.JobPriority;
            }
        }

        private class PropertyAccessorAnonymousInnerClass39 : PropertyAccessor<IHistoricJobLog, long>
        {
            public virtual long getProperty(IHistoricJobLog obj)
            {
                return ((HistoricJobLogEventEntity) obj).SequenceCounter;
            }
        }

        private class PropertyAccessorAnonymousInnerClass40 : PropertyAccessor<IJob, long>
        {
            public virtual long getProperty(IJob obj)
            {
                return obj.Priority;
            }
        }

        private class PropertyAccessorAnonymousInnerClass41 : PropertyAccessor<IExternalTask, string>
        {
            public virtual string getProperty(ESS.FW.Bpm.Engine.Externaltask.IExternalTask obj)
            {
                return obj.Id;
            }
        }

        private class PropertyAccessorAnonymousInnerClass42 : PropertyAccessor<IExternalTask, string>
        {
            public virtual string getProperty(ESS.FW.Bpm.Engine.Externaltask.IExternalTask obj)
            {
                return obj.ProcessInstanceId;
            }
        }

        private class PropertyAccessorAnonymousInnerClass43 : PropertyAccessor<IExternalTask, string>
        {
            public virtual string getProperty(ESS.FW.Bpm.Engine.Externaltask.IExternalTask obj)
            {
                return obj.ProcessDefinitionId;
            }
        }

        private class PropertyAccessorAnonymousInnerClass44 : PropertyAccessor<IExternalTask, string>
        {
            public virtual string getProperty(ESS.FW.Bpm.Engine.Externaltask.IExternalTask obj)
            {
                return obj.ProcessDefinitionKey;
            }
        }

        private class PropertyAccessorAnonymousInnerClass45 : PropertyAccessor<IExternalTask, DateTime>
        {
            public virtual DateTime getProperty(ESS.FW.Bpm.Engine.Externaltask.IExternalTask obj)
            {
                return obj.LockExpirationTime;
            }
        }

        private class PropertyAccessorAnonymousInnerClass46 : PropertyAccessor<IExternalTask, long>
        {
            public virtual long getProperty(ESS.FW.Bpm.Engine.Externaltask.IExternalTask obj)
            {
                return obj.Priority;
            }
        }

        private class PropertyAccessorAnonymousInnerClass47 : PropertyAccessor<IBatch, string>
        {
            public virtual string getProperty(IBatch obj)
            {
                return obj.Id;
            }
        }

        private class PropertyAccessorAnonymousInnerClass48 : PropertyAccessor<IBatch, string>
        {
            public virtual string getProperty(IBatch obj)
            {
                return obj.TenantId;
            }
        }

        private class PropertyAccessorAnonymousInnerClass49 : PropertyAccessor<IHistoricBatch, string>
        {
            public virtual string getProperty(IHistoricBatch obj)
            {
                return obj.Id;
            }
        }

        private class PropertyAccessorAnonymousInnerClass50 : PropertyAccessor<IHistoricBatch, string>
        {
            public virtual string getProperty(IHistoricBatch obj)
            {
                return obj.TenantId;
            }
        }

        private class PropertyAccessorAnonymousInnerClass51 : PropertyAccessor<IHistoricBatch, DateTime>
        {
            public virtual DateTime getProperty(IHistoricBatch obj)
            {
                return obj.StartTime;
            }
        }

        private class PropertyAccessorAnonymousInnerClass52 : PropertyAccessor<IHistoricBatch, DateTime>
        {
            public virtual DateTime getProperty(IHistoricBatch obj)
            {
                return obj.EndTime;
            }
        }

        private class PropertyAccessorAnonymousInnerClass53 : PropertyAccessor<IBatchStatistics, string>
        {
            public virtual string getProperty(IBatchStatistics obj)
            {
                return obj.Id;
            }
        }

        private class PropertyAccessorAnonymousInnerClass54 : PropertyAccessor<IBatchStatistics, string>
        {
            public virtual string getProperty(IBatchStatistics obj)
            {
                return obj.TenantId;
            }
        }

        private class PropertyAccessorAnonymousInnerClass55 : PropertyAccessor<IHistoricExternalTaskLog, DateTime>
        {
            public virtual DateTime getProperty(IHistoricExternalTaskLog obj)
            {
                return obj.TimeStamp;
            }
        }

        private class PropertyAccessorAnonymousInnerClass56 : PropertyAccessor<IHistoricExternalTaskLog, string>
        {
            public virtual string getProperty(IHistoricExternalTaskLog obj)
            {
                return obj.ExternalTaskId;
            }
        }

        private class PropertyAccessorAnonymousInnerClass57 : PropertyAccessor<IHistoricExternalTaskLog, int>
        {
            public virtual int getProperty(IHistoricExternalTaskLog obj)
            {
                return obj.Retries ?? 0;
            }
        }

        private class PropertyAccessorAnonymousInnerClass58 : PropertyAccessor<IHistoricExternalTaskLog, long>
        {
            public virtual long getProperty(IHistoricExternalTaskLog obj)
            {
                return obj.Priority;
            }
        }

        private class PropertyAccessorAnonymousInnerClass59 : PropertyAccessor<IHistoricExternalTaskLog, string>
        {
            public virtual string getProperty(IHistoricExternalTaskLog obj)
            {
                return obj.TopicName;
            }
        }

        private class PropertyAccessorAnonymousInnerClass60 : PropertyAccessor<IHistoricExternalTaskLog, string>
        {
            public virtual string getProperty(IHistoricExternalTaskLog obj)
            {
                return obj.WorkerId;
            }
        }

        private class PropertyAccessorAnonymousInnerClass61 : PropertyAccessor<IHistoricExternalTaskLog, string>
        {
            public virtual string getProperty(IHistoricExternalTaskLog obj)
            {
                return obj.ActivityId;
            }
        }

        private class PropertyAccessorAnonymousInnerClass62 : PropertyAccessor<IHistoricExternalTaskLog, string>
        {
            public virtual string getProperty(IHistoricExternalTaskLog obj)
            {
                return obj.ActivityInstanceId;
            }
        }

        private class PropertyAccessorAnonymousInnerClass63 : PropertyAccessor<IHistoricExternalTaskLog, string>
        {
            public virtual string getProperty(IHistoricExternalTaskLog obj)
            {
                return obj.ExecutionId;
            }
        }

        private class PropertyAccessorAnonymousInnerClass64 : PropertyAccessor<IHistoricExternalTaskLog, string>
        {
            public virtual string getProperty(IHistoricExternalTaskLog obj)
            {
                return obj.ProcessInstanceId;
            }
        }

        private class PropertyAccessorAnonymousInnerClass65 : PropertyAccessor<IHistoricExternalTaskLog, string>
        {
            public virtual string getProperty(IHistoricExternalTaskLog obj)
            {
                return obj.ProcessDefinitionId;
            }
        }

        private class PropertyAccessorAnonymousInnerClass66 : PropertyAccessor<IHistoricExternalTaskLog, string>
        {
            private readonly IRepositoryService repositoryService;

            public PropertyAccessorAnonymousInnerClass66(IRepositoryService repositoryService)
            {
                this.repositoryService = repositoryService;
            }

            public virtual string getProperty(IHistoricExternalTaskLog obj)
            {
                var processDefinition = repositoryService.GetProcessDefinition(obj.ProcessDefinitionId);
                return processDefinition.Key;
            }
        }

        private class PropertyAccessorAnonymousInnerClass67 : PropertyAccessor<IHistoricExternalTaskLog, string>
        {
            public virtual string getProperty(IHistoricExternalTaskLog obj)
            {
                return obj.TenantId;
            }
        }

        //private class NullTolerantComparatorAnonymousInnerClass : NullTolerantComparator
        //{
        //    private readonly PropertyAccessor<T, P> accessor;

        //    public NullTolerantComparatorAnonymousInnerClass(PropertyAccessor<T, P> accessor)
        //    {
        //        this.accessor = accessor;
        //    }


        //    public override int compare(T o1, T o2)
        //    {
        //        P prop1 = accessor.GetProperty(o1);
        //        P prop2 = accessor.GetProperty(o2);

        //        return prop1.CompareTo(prop2);
        //    }

        //    public override bool hasNullProperty(T @object)
        //    {
        //        return accessor.GetProperty(@object) == null;
        //    }
        //}

        protected internal interface PropertyAccessor<T, P> where P : IComparable<P>
        {
            P getProperty(T obj);
        }

        //private class NullTolerantComparatorAnonymousInnerClass2 : NullTolerantComparator<T>
        //{
        //    private readonly NullTolerantComparator<T> comparator;

        //    public NullTolerantComparatorAnonymousInnerClass2(NullTolerantComparator<T> comparator)
        //    {
        //        this.comparator = comparator;
        //    }

        //    public virtual int compare(T o1, T o2)
        //    {
        //        return -comparator.Compare(o1, o2);
        //    }

        //    public override bool hasNullProperty(T @object)
        //    {
        //        return comparator.HasNullProperty(@object);
        //    }
        //}

        //private class NullTolerantComparatorAnonymousInnerClass3 : NullTolerantComparator
        //{
        //    private readonly NullTolerantComparator<T> baseComparator;
        //    private NullTolerantComparator<T>[] minorOrderings;

        //    public NullTolerantComparatorAnonymousInnerClass3(NullTolerantComparator<T> baseComparator,
        //        NullTolerantComparator<T>[] minorOrderings)
        //    {
        //        this.baseComparator = baseComparator;
        //        this.MinorOrderings = minorOrderings;
        //    }

        //    public override int compare(T o1, T o2, bool nullPrecedes)
        //    {
        //        int comparison = baseComparator.compare(o1, o2, nullPrecedes);

        //        var i = 0;
        //        while (comparison == 0 && i < minorOrderings.Length)
        //        {
        //            NullTolerantComparator<T> comparator = minorOrderings[i];
        //            comparison = comparator.compare(o1, o2, nullPrecedes);
        //            i++;
        //        }

        //        return comparison;
        //    }

        //    public virtual int compare(T o1, T o2)
        //    {
        //        throw new NotSupportedException();
        //    }

        //    public override bool hasNullProperty(T @object)
        //    {
        //        throw new NotSupportedException();
        //    }
        //}

        public abstract class NullTolerantComparator<T> : IComparer<T>
        {
            public virtual int Compare(T o1, T o2)
            {
                var o1Null = hasNullProperty(o1);
                var o2Null = hasNullProperty(o2);

                if (o1Null)
                    if (o2Null)
                        return 0;
                    else
                        return 1;
                if (o2Null)
                    //if (nullPrecedes)
                    //    return 1;
                    //else
                    return -1;

                return Compare(o1, o2);
            }

            public abstract bool hasNullProperty(T @object);
        }
    }
}