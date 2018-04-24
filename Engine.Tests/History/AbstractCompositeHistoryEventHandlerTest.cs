using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.History.Impl.Handler;
using NUnit.Framework;

namespace Engine.Tests.History
{
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
    [TestFixture]
    public abstract class AbstractCompositeHistoryEventHandlerTest : PluggableProcessEngineTestCase
    {
        /// <summary>
        ///     Perform common setup.
        /// </summary>
        [SetUp]
        protected internal void SetUp()
        {
            //base.SetUp();
            // save current history event handler
            OriginalHistoryEventHandler = processEngineConfiguration.HistoryEventHandler;
            // clear the event counter
            CountCustomHistoryEventHandler = 0;
        }

        [TearDown]
        protected internal void tearDown()
        {
            //base.TearDown();
            // reset original history event handler
            //processEngineConfiguration.HistoryEventHandler = OriginalHistoryEventHandler;
        }

        protected internal IHistoryEventHandler OriginalHistoryEventHandler;

        /// <summary>
        ///     The counter used to check the amount of triggered events.
        /// </summary>
        protected internal int CountCustomHistoryEventHandler;

        /// <summary>
        ///     The helper method to execute the test task.
        /// </summary>
        protected internal virtual void StartProcessAndCompleteUserTask()
        {
            runtimeService.StartProcessInstanceByKey("HistoryLevelTest");
            var task = taskService.CreateTaskQuery()
                .First();
            taskService.Complete(task.Id);
        }

        /// <summary>
        ///     A <seealso cref="HistoryEventHandler" /> implementation to Count the history events.
        /// </summary>
        protected internal class CustomDbHistoryEventHandler : IHistoryEventHandler
        {
            private readonly AbstractCompositeHistoryEventHandlerTest _outerInstance;

            public CustomDbHistoryEventHandler(AbstractCompositeHistoryEventHandlerTest outerInstance)
            {
                _outerInstance = outerInstance;
            }


            public void HandleEvent(HistoryEvent historyEvent)
            {
                // take into account only variable related events
                if (historyEvent is HistoricVariableUpdateEventEntity)
                    _outerInstance.CountCustomHistoryEventHandler++;
            }

            public void HandleEvents(IList<HistoryEvent> historyEvents)
            {
                foreach (var historyEvent in historyEvents)
                    HandleEvent(historyEvent);
            }
        }
    }
}