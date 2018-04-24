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
    public class CompositeHistoryEventHandlerTest : AbstractCompositeHistoryEventHandlerTest
    {
        [Test]
        public virtual void testCompositeHistoryEventHandlerNonArgumentConstructorAddNullEvent()
        {
            var compositeHistoryEventHandler = new CompositeHistoryEventHandler();
            try
            {
                compositeHistoryEventHandler.Add(null);
                Assert.Fail("NullValueException expected");
            }
            catch (NullValueException e)
            {
                AssertTextPresent("History event handler is null", e.Message);
            }
        }

        [Test]
        public virtual void testCompositeHistoryEventHandlerArgumentConstructorWithNullVarargs()
        {
            IHistoryEventHandler historyEventHandler = null;
            try
            {
                new CompositeHistoryEventHandler(historyEventHandler);
                Assert.Fail("NullValueException expected");
            }
            catch (NullValueException e)
            {
                AssertTextPresent("History event handler is null", e.Message);
            }
        }

        [Test]
        public virtual void testCompositeHistoryEventHandlerArgumentConstructorWithNullTwoVarargs()
        {
            try
            {
                new CompositeHistoryEventHandler(null, null);
                Assert.Fail("NullValueException expected");
            }
            catch (NullValueException e)
            {
                AssertTextPresent("History event handler is null", e.Message);
            }
        }

        [Test]
        public virtual void testCompositeHistoryEventHandlerArgumentConstructorWithNotEmptyListNullTwoEvents()
        {
            // prepare the list with two null events
            IList<IHistoryEventHandler> historyEventHandlers = new List<IHistoryEventHandler>();
            historyEventHandlers.Add(null);
            historyEventHandlers.Add(null);

            try
            {
                new CompositeHistoryEventHandler(historyEventHandlers);
                Assert.Fail("NullValueException expected");
            }
            catch (NullValueException e)
            {
                AssertTextPresent("History event handler is null", e.Message);
            }
        }

        [Test]
        [Deployment("resources/history/HistoryLevelTest.bpmn20.xml")]
        public virtual void testCompositeHistoryEventHandlerArgumentConstructorWithEmptyList()
        {
            var compositeHistoryEventHandler = new CompositeHistoryEventHandler(new List<IHistoryEventHandler>());
            processEngineConfiguration.HistoryEventHandler = compositeHistoryEventHandler;

            StartProcessAndCompleteUserTask();

            Assert.AreEqual(0, CountCustomHistoryEventHandler);
            Assert.AreEqual(0, historyService.CreateHistoricDetailQuery()
                .Count());
        }

        [Test]
        [Deployment("resources/history/HistoryLevelTest.bpmn20.xml")]
        public virtual void testCompositeHistoryEventHandlerArgumentConstructorWithNotEmptyListNotNullTwoEvents()
        {
            // prepare the list with two events
            IList<IHistoryEventHandler> historyEventHandlers = new List<IHistoryEventHandler>();
            historyEventHandlers.Add(new CustomDbHistoryEventHandler(this));
            historyEventHandlers.Add(new DbHistoryEventHandler());

            var compositeHistoryEventHandler = new CompositeHistoryEventHandler(historyEventHandlers);
            processEngineConfiguration.HistoryEventHandler = compositeHistoryEventHandler;

            StartProcessAndCompleteUserTask();

            Assert.AreEqual(2, CountCustomHistoryEventHandler);
            Assert.AreEqual(2, historyService.CreateHistoricDetailQuery()
                .Count());
        }

        [Test]
        [Deployment("resources/history/HistoryLevelTest.bpmn20.xml")]
        public virtual void testCompositeHistoryEventHandlerArgumentConstructorWithNotNullVarargsOneEvent()
        {
            var compositeHistoryEventHandler = new CompositeHistoryEventHandler(new CustomDbHistoryEventHandler(this));
            processEngineConfiguration.HistoryEventHandler = compositeHistoryEventHandler;

            StartProcessAndCompleteUserTask();

            Assert.AreEqual(2, CountCustomHistoryEventHandler);
            Assert.AreEqual(0, historyService.CreateHistoricDetailQuery()
                .Count());
        }

        [Test]
        [Deployment("resources/history/HistoryLevelTest.bpmn20.xml")]
        public virtual void testCompositeHistoryEventHandlerArgumentConstructorWithNotNullVarargsTwoEvents()
        {
            var compositeHistoryEventHandler = new CompositeHistoryEventHandler(new CustomDbHistoryEventHandler(this),
                new DbHistoryEventHandler());
            processEngineConfiguration.HistoryEventHandler = compositeHistoryEventHandler;

            StartProcessAndCompleteUserTask();

            Assert.AreEqual(2, CountCustomHistoryEventHandler);
            Assert.AreEqual(2, historyService.CreateHistoricDetailQuery()
                .Count());
        }

        [Test]
        [Deployment("resources/history/HistoryLevelTest.bpmn20.xml")]
        public virtual void testCompositeHistoryEventHandlerNonArgumentConstructor()
        {
            processEngineConfiguration.HistoryEventHandler = new CompositeHistoryEventHandler();

            StartProcessAndCompleteUserTask();

            Assert.AreEqual(0, CountCustomHistoryEventHandler);
            Assert.AreEqual(0, historyService.CreateHistoricDetailQuery()
                .Count());
        }

        [Test]
        [Deployment("resources/history/HistoryLevelTest.bpmn20.xml")]
        public virtual void testCompositeHistoryEventHandlerNonArgumentConstructorAddNotNullEvent()
        {
            var compositeHistoryEventHandler = new CompositeHistoryEventHandler();
            compositeHistoryEventHandler.Add(new CustomDbHistoryEventHandler(this));
            processEngineConfiguration.HistoryEventHandler = compositeHistoryEventHandler;

            StartProcessAndCompleteUserTask();

            Assert.AreEqual(2, CountCustomHistoryEventHandler);
            Assert.AreEqual(0, historyService.CreateHistoricDetailQuery()
                .Count());
        }

        [Test]
        [Deployment("resources/history/HistoryLevelTest.bpmn20.xml")]
        public virtual void testCompositeHistoryEventHandlerNonArgumentConstructorAddNotNullTwoEvents()
        {
            var compositeHistoryEventHandler = new CompositeHistoryEventHandler();
            compositeHistoryEventHandler.Add(new CustomDbHistoryEventHandler(this));
            compositeHistoryEventHandler.Add(new DbHistoryEventHandler());
            processEngineConfiguration.HistoryEventHandler = compositeHistoryEventHandler;

            StartProcessAndCompleteUserTask();

            Assert.AreEqual(2, CountCustomHistoryEventHandler);
            Assert.AreEqual(2, historyService.CreateHistoricDetailQuery()
                .Count());
        }

        [Test]
        [Deployment("resources/history/HistoryLevelTest.bpmn20.xml")]
        public virtual void testDefaultHistoryEventHandler()
        {
            // use default DbHistoryEventHandler
            processEngineConfiguration.HistoryEventHandler = new DbHistoryEventHandler();

            StartProcessAndCompleteUserTask();

            Assert.AreEqual(0, CountCustomHistoryEventHandler);
            Assert.AreEqual(2, historyService.CreateHistoricDetailQuery()
                .Count());
        }
    }
}