using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.History.Impl.Handler;
using NUnit.Framework;

namespace Engine.Tests.History
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class CompositeDbHistoryEventHandlerTest : AbstractCompositeHistoryEventHandlerTest
    {
        [Test]
        public virtual void testCompositeDbHistoryEventHandlerArgumentConstructorWithNullVarargs()
        {
            IHistoryEventHandler historyEventHandler = null;
            try
            {
                new CompositeDbHistoryEventHandler(historyEventHandler);
                Assert.Fail("NullValueException expected");
            }
            catch (NullValueException e)
            {
                AssertTextPresent("History event handler is null", e.Message);
            }
        }

        [Test]
        public virtual void testCompositeDbHistoryEventHandlerArgumentConstructorWithNullTwoVarargs()
        {
            try
            {
                new CompositeDbHistoryEventHandler(null, null);
                Assert.Fail("NullValueException expected");
            }
            catch (NullValueException e)
            {
                AssertTextPresent("History event handler is null", e.Message);
            }
        }

        [Test][Deployment( "resources/history/HistoryLevelTest.bpmn20.xml") ]
        public virtual void testCompositeDbHistoryEventHandlerArgumentConstructorWithEmptyList()
        {
            var compositeDbHistoryEventHandler = new CompositeDbHistoryEventHandler(new List<IHistoryEventHandler>());
            processEngineConfiguration.HistoryEventHandler = compositeDbHistoryEventHandler;

            StartProcessAndCompleteUserTask();

            Assert.AreEqual(0, CountCustomHistoryEventHandler);
            Assert.AreEqual(2, historyService.CreateHistoricDetailQuery()
                .Count());
        }

        [Test]
        public virtual void testCompositeDbHistoryEventHandlerArgumentConstructorWithNotEmptyListNullTwoEvents()
        {
            // prepare the list with two null events
            IList<IHistoryEventHandler> historyEventHandlers = new List<IHistoryEventHandler>();
            historyEventHandlers.Add(null);
            historyEventHandlers.Add(null);

            try
            {
                new CompositeDbHistoryEventHandler(historyEventHandlers);
                Assert.Fail("NullValueException expected");
            }
            catch (NullValueException e)
            {
                AssertTextPresent("History event handler is null", e.Message);
            }
        }

        [Test]
        [Deployment("resources/history/HistoryLevelTest.bpmn20.xml")]
        public virtual void testCompositeDbHistoryEventHandlerArgumentConstructorWithNotEmptyListNotNullTwoEvents()
        {
            // prepare the list with two events
            IList<IHistoryEventHandler> historyEventHandlers = new List<IHistoryEventHandler>();
            historyEventHandlers.Add(new CustomDbHistoryEventHandler(this));
            historyEventHandlers.Add(new CustomDbHistoryEventHandler(this));

            var compositeDbHistoryEventHandler = new CompositeDbHistoryEventHandler(historyEventHandlers);
            processEngineConfiguration.HistoryEventHandler = compositeDbHistoryEventHandler;

            StartProcessAndCompleteUserTask();

            Assert.AreEqual(4, CountCustomHistoryEventHandler);
            Assert.AreEqual(2, historyService.CreateHistoricDetailQuery()
                .Count());
        }

        [Test]
        [Deployment("resources/history/HistoryLevelTest.bpmn20.xml")]
        public virtual void testCompositeDbHistoryEventHandlerArgumentConstructorWithNotNullVarargsOneEvent()
        {
            var compositeDbHistoryEventHandler =
                new CompositeDbHistoryEventHandler(new CustomDbHistoryEventHandler(this));
            //processEngineConfiguration.HistoryEventHandler = compositeDbHistoryEventHandler;

            StartProcessAndCompleteUserTask();

            Assert.AreEqual(2, CountCustomHistoryEventHandler);
            Assert.AreEqual(2, historyService.CreateHistoricDetailQuery()
                .Count());
        }

        [Test]
        [Deployment("resources/history/HistoryLevelTest.bpmn20.xml")]
        public virtual void testCompositeDbHistoryEventHandlerArgumentConstructorWithNotNullVarargsTwoEvents()
        {
            var compositeDbHistoryEventHandler =
                new CompositeDbHistoryEventHandler(new CustomDbHistoryEventHandler(this),
                    new CustomDbHistoryEventHandler(this));
            //processEngineConfiguration.HistoryEventHandler = compositeDbHistoryEventHandler;

            StartProcessAndCompleteUserTask();

            Assert.AreEqual(4, CountCustomHistoryEventHandler);
            Assert.AreEqual(2, historyService.CreateHistoricDetailQuery()
                .Count());
        }

        [Test]
        [Deployment("resources/history/HistoryLevelTest.bpmn20.xml")]
        public virtual void testCompositeDbHistoryEventHandlerNonArgumentConstructor()
        {
            //processEngineConfiguration.HistoryEventHandler = new CompositeDbHistoryEventHandler();

            StartProcessAndCompleteUserTask();

            Assert.AreEqual(0, CountCustomHistoryEventHandler);
            Assert.AreEqual(2, historyService.CreateHistoricDetailQuery()
                .Count());
        }

        [Test]
        [Deployment("resources/history/HistoryLevelTest.bpmn20.xml")]
        public virtual void testCompositeDbHistoryEventHandlerNonArgumentConstructorAddNotNullEvent()
        {
            var compositeDbHistoryEventHandler = new CompositeDbHistoryEventHandler();
            compositeDbHistoryEventHandler.Add(new CustomDbHistoryEventHandler(this));
            //processEngineConfiguration.HistoryEventHandler = compositeDbHistoryEventHandler;

            StartProcessAndCompleteUserTask();

            Assert.AreEqual(2, CountCustomHistoryEventHandler);
            Assert.AreEqual(2, historyService.CreateHistoricDetailQuery()
                .Count());
        }

        [Test]
        public virtual void testCompositeDbHistoryEventHandlerNonArgumentConstructorAddNullEvent()
        {
            var compositeDbHistoryEventHandler = new CompositeDbHistoryEventHandler();
            try
            {
                compositeDbHistoryEventHandler.Add(null);
                Assert.Fail("NullValueException expected");
            }
            catch (NullValueException e)
            {
                AssertTextPresent("History event handler is null", e.Message);
            }
        }

        [Test]
        [Deployment("resources/history/HistoryLevelTest.bpmn20.xml")]
        public virtual void testCompositeDbHistoryEventHandlerNonArgumentConstructorAddTwoNotNullEvents()
        {
            var compositeDbHistoryEventHandler = new CompositeDbHistoryEventHandler();
            compositeDbHistoryEventHandler.Add(new CustomDbHistoryEventHandler(this));
            compositeDbHistoryEventHandler.Add(new CustomDbHistoryEventHandler(this));
            //processEngineConfiguration.HistoryEventHandler = compositeDbHistoryEventHandler;

            StartProcessAndCompleteUserTask();

            Assert.AreEqual(4, CountCustomHistoryEventHandler);
            Assert.AreEqual(2, historyService.CreateHistoricDetailQuery()
                .Count());
        }
    }
}