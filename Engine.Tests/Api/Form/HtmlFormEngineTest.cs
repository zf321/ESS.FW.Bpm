using System;
using System.Linq;
using ESS.FW.Bpm.Engine.Form.Impl.Engine;
using ESS.FW.Bpm.Engine.Impl.Util;
using NUnit.Framework;

namespace Engine.Tests.Api.Form
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class HtmlFormEngineTest : PluggableProcessEngineTestCase
    {
        [Test]
        public virtual void testIsDefaultFormEngine()
        {
            // make sure the html form engine is the default form engine:
            var formEngines = processEngineConfiguration.FormEngines;
            Assert.True(formEngines[null] is HtmlFormEngine);
        }

        [Test]
        public virtual void testTransformNullFormData()
        {
            var formEngine = new HtmlFormEngine();
            Assert.IsNull(formEngine.RenderStartForm(null));
            Assert.IsNull(formEngine.RenderTaskForm(null));
        }

        [Test]
        public virtual void testHtmlElementWriter()
        {
            var htmlString = new HtmlDocumentBuilder(new HtmlElementWriter("someTagName")).EndElement()
                .HtmlString;
            AssertHtmlEquals("<someTagName></someTagName>", htmlString);

            htmlString = new HtmlDocumentBuilder(new HtmlElementWriter("someTagName", true)).EndElement()
                .HtmlString;
            AssertHtmlEquals("<someTagName />", htmlString);

            htmlString =
                new HtmlDocumentBuilder(new HtmlElementWriter("someTagName", true).Attribute("someAttr", "someAttrValue"))
                    .EndElement()
                    .HtmlString;
            AssertHtmlEquals("<someTagName someAttr=\"someAttrValue\" />", htmlString);

            htmlString =
                new HtmlDocumentBuilder(new HtmlElementWriter("someTagName").Attribute("someAttr", "someAttrValue"))
                    .EndElement()
                    .HtmlString;
            AssertHtmlEquals("<someTagName someAttr=\"someAttrValue\"></someTagName>", htmlString);

            htmlString =
                new HtmlDocumentBuilder(new HtmlElementWriter("someTagName").Attribute("someAttr", null)).EndElement()
                    .HtmlString;
            AssertHtmlEquals("<someTagName someAttr></someTagName>", htmlString);

            htmlString =
                new HtmlDocumentBuilder(new HtmlElementWriter("someTagName").TextContent("someTextContent")).EndElement()
                    .HtmlString;
            AssertHtmlEquals("<someTagName>someTextContent</someTagName>", htmlString);

            htmlString =
                new HtmlDocumentBuilder(new HtmlElementWriter("someTagName")).StartElement(
                        new HtmlElementWriter("someChildTag"))
                    .EndElement()
                    .EndElement()
                    .HtmlString;
            AssertHtmlEquals("<someTagName><someChildTag></someChildTag></someTagName>", htmlString);

            htmlString =
                new HtmlDocumentBuilder(new HtmlElementWriter("someTagName")).StartElement(
                        new HtmlElementWriter("someChildTag").TextContent("someTextContent"))
                    .EndElement()
                    .EndElement()
                    .HtmlString;
            AssertHtmlEquals("<someTagName><someChildTag>someTextContent</someChildTag></someTagName>", htmlString);

            htmlString =
                new HtmlDocumentBuilder(new HtmlElementWriter("someTagName").TextContent("someTextContent"))
                    .StartElement(new HtmlElementWriter("someChildTag"))
                    .EndElement()
                    .EndElement()
                    .HtmlString;
            AssertHtmlEquals("<someTagName><someChildTag></someChildTag>someTextContent</someTagName>", htmlString);

            // invalid usage

            try
            {
                new HtmlElementWriter("sometagname", true).TextContent("sometextcontet");
            }
            catch (InvalidOperationException e)
            {
                Assert.True(e.Message.Contains("Self-closing element cannot have text content"));
            }
        }

        [Test][Deployment ]
        public virtual void testRenderEmptyStartForm()
        {
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();

            Assert.IsNull(formService.GetRenderedStartForm(processDefinition.Id));
        }

        [Test]
        [Deployment]
        public virtual void testRenderStartForm()
        {
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();

            var renderedForm = (string) formService.GetRenderedStartForm(processDefinition.Id);

            var expectedForm = IoUtil.ReadFileAsString("resources/api/form/HtmlFormEngineTest.TestRenderStartForm.html");

            AssertHtmlEquals(expectedForm, renderedForm);
        }

        [Test]
        [Deployment]
        public virtual void testRenderEnumField()
        {
            runtimeService.StartProcessInstanceByKey("HtmlFormEngineTest.TestRenderEnumField");

            var t = taskService.CreateTaskQuery()
                .First();

            var renderedForm = (string) formService.GetRenderedTaskForm(t.Id);

            var expectedForm = IoUtil.ReadFileAsString("resources/api/form/HtmlFormEngineTest.TestRenderEnumField.html");

            AssertHtmlEquals(expectedForm, renderedForm);
        }

        [Test]
        [Deployment]
        public virtual void testRenderTaskForm()
        {
            runtimeService.StartProcessInstanceByKey("HtmlFormEngineTest.TestRenderTaskForm");

            var t = taskService.CreateTaskQuery()
                .First();

            var renderedForm = (string) formService.GetRenderedTaskForm(t.Id);

            var expectedForm = IoUtil.ReadFileAsString("resources/api/form/HtmlFormEngineTest.TestRenderTaskForm.html");

            AssertHtmlEquals(expectedForm, renderedForm);
        }

        [Test]
        [Deployment]
        public virtual void testRenderDateField()
        {
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();
            var renderedForm = (string) formService.GetRenderedStartForm(processDefinition.Id);

            var expectedForm = IoUtil.ReadFileAsString("resources/api/form/HtmlFormEngineTest.TestRenderDateField.html");

            AssertHtmlEquals(expectedForm, renderedForm);
        }

        [Test]
        [Deployment]
        public virtual void testLegacyFormPropertySupport()
        {
            runtimeService.StartProcessInstanceByKey("HtmlFormEngineTest.TestLegacyFormPropertySupport");

            var t = taskService.CreateTaskQuery()
                .First();

            var renderedForm = (string) formService.GetRenderedTaskForm(t.Id);

            var expectedForm =
                IoUtil.ReadFileAsString("resources/api/form/HtmlFormEngineTest.TestLegacyFormPropertySupport.html");

            AssertHtmlEquals(expectedForm, renderedForm);
        }

        [Test]
        [Deployment]
        public virtual void testLegacyFormPropertySupportReadOnly()
        {
            runtimeService.StartProcessInstanceByKey("HtmlFormEngineTest.TestLegacyFormPropertySupportReadOnly");

            var t = taskService.CreateTaskQuery()
                .First();

            var renderedForm = (string) formService.GetRenderedTaskForm(t.Id);

            var expectedForm =
                IoUtil.ReadFileAsString(
                    "resources/api/form/HtmlFormEngineTest.TestLegacyFormPropertySupportReadOnly.html");

            AssertHtmlEquals(expectedForm, renderedForm);
        }

        [Test]
        [Deployment]
        public virtual void testLegacyFormPropertySupportRequired()
        {
            runtimeService.StartProcessInstanceByKey("HtmlFormEngineTest.TestLegacyFormPropertySupportRequired");

            var t = taskService.CreateTaskQuery()
                .First();

            var renderedForm = (string) formService.GetRenderedTaskForm(t.Id);

            var expectedForm =
                IoUtil.ReadFileAsString(
                    "resources/api/form/HtmlFormEngineTest.TestLegacyFormPropertySupportRequired.html");

            AssertHtmlEquals(expectedForm, renderedForm);
        }

        [Test]
        [Deployment]
        public virtual void testBusinessKey()
        {
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();

            var renderedForm = (string) formService.GetRenderedStartForm(processDefinition.Id);

            var expectedForm = IoUtil.ReadFileAsString("resources/api/form/HtmlFormEngineTest.TestBusinessKey.html");

            AssertHtmlEquals(expectedForm, renderedForm);
        }

        public virtual void AssertHtmlEquals(string expected, string actual)
        {
            Assert.AreEqual(filterWhitespace(expected), filterWhitespace(actual));
        }

        protected internal virtual string filterWhitespace(string tofilter)
        {
            return tofilter.Replace("\\n", "")
                .Replace("\\s", "");
        }
    }
}