using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Diagram;
using ESS.FW.Bpm.Engine.Repository;
using NUnit.Framework;

namespace Engine.Tests.Api.Repository.Diagram
{
    


    
    
    public class ProcessDiagramRetrievalTest
    {

        
        private const bool OVERWRITE_EXPECTED_HTML_FILES = false;

        
        public ProcessEngineRule activitiRule = new ProvidedProcessEngineRule();

        
        
        public static ICollection<object[]> data()
        {
            return Array.AsReadOnly(new object[][]
            {
            new object[] {"testStartEventWithNegativeCoordinates", ".bpmn", ".png", "sid-61D1FC47-8031-4834-A9B4-84158E73F7B9"},
            new object[] {"testStartAndEndEventWithNegativeCoordinates", ".bpmn", ".png", "sid-61D1FC47-8031-4834-A9B4-84158E73F7B9"},
            new object[] {"testProcessWithTask", ".bpmn", ".png", "sid-1E142B16-AFAF-429E-A441-D1232CFBD560"},
            new object[] {"testProcessFromCamundaFoxDesigner", ".bpmn", ".png", "UserTask_1"},
            new object[] {"testProcessFromCamundaFoxDesigner", ".bpmn", ".jpg", "UserTask_1"},
            new object[] {"testProcessFromActivitiDesigner", ".bpmn20.xml", ".png", "Send_rejection_notification_via_email__3"},
            new object[] {"testSequenceFlowOutOfBounds", ".bpmn", ".png", "sid-61D1FC47-8031-4834-A9B4-84158E73F7B9"},
            new object[] {"testProcessFromAdonis", ".bpmn", ".png", "_16615"},
            new object[] {"testProcessFromIboPrometheus", ".bpmn", ".png", "ibo-5784efbe-35ac-44bc-bcbe-4c18a2f23d5d"},
            new object[] {"testProcessFromIboPrometheus", ".bpmn", ".jpg", "ibo-5784efbe-35ac-44bc-bcbe-4c18a2f23d5d"},
            new object[] {"testInvoiceProcessCamundaFoxDesigner", ".bpmn20.xml", ".jpg", "Rechnung_freigeben_125"},
            new object[] {"testInvoiceProcessSignavio", ".bpmn", ".png", "Freigebenden_zuordnen_143"},
            new object[] {"testInvoiceProcessFromBusinessProcessIncubator", ".bpmn", ".png", "Rechnung_kl_ren_148"},
            new object[] {"testProcessFromYaoqiang", ".bpmn", ".png", "_3"}
            });
        }

        private readonly string xmlFileName;
        private readonly string imageFileName;
        private readonly string highlightedActivityId;
        private IRepositoryService repositoryService;
        private string deploymentId;

        private IQueryable<IProcessDefinition> processDefinitionQuery;

        public ProcessDiagramRetrievalTest(string modelName, string xmlFileExtension, string imageFileExtension, string highlightedActivityId)
        {
            this.xmlFileName = modelName + xmlFileExtension;
            this.imageFileName = modelName + imageFileExtension;
            this.highlightedActivityId = highlightedActivityId;
        }

        [SetUp]
        public virtual void setup()
        {
            repositoryService = activitiRule.RepositoryService;
            deploymentId = repositoryService.CreateDeployment().AddClasspathResource("resources/api/repository/diagram/" + xmlFileName).AddClasspathResource("resources/api/repository/diagram/" + imageFileName).Deploy().Id;
            processDefinitionQuery = repositoryService.CreateProcessDefinitionQuery();
        }

        [TearDown]
        public virtual void teardown()
        {
            repositoryService.DeleteDeployment(deploymentId, true);
        }

        

         [Test]   public virtual void testGetProcessModel()
        {
            if (1 == processDefinitionQuery.Count())
            {
                IProcessDefinition processDefinition = processDefinitionQuery.First();
                System.IO.Stream expectedStream = new System.IO.FileStream("src/test/resources/resources/api/repository/diagram/" + xmlFileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                System.IO.Stream actualStream = repositoryService.GetProcessModel(processDefinition.Id);
                Assert.True(isEqual(expectedStream, actualStream));
            }
            else
            {
                // some test diagrams do not contain executable processes
                // and are therefore ignored by the engine
            }
        }


        /// <summary>
        /// Tests <seealso cref="RepositoryService#getProcessDiagram(String)"/>.
        /// </summary>
        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testGetProcessDiagram() throws Exception
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
         [Test]   public virtual void testGetProcessDiagram()
        {
            if (1 == processDefinitionQuery.Count())
            {
                IProcessDefinition processDefinition = processDefinitionQuery.First();
                System.IO.Stream expectedStream = new System.IO.FileStream("src/test/resources/resources/api/repository/diagram/" + imageFileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                System.IO.Stream actualStream = repositoryService.GetProcessDiagram(processDefinition.Id);
                //      writeToFile(repositoryService.GetProcessDiagram(processDefinition.GetId()),
                //              new File("src/test/resources/resources/api/repository/diagram/" + imageFileName + ".actual.png"));
                Assert.True(isEqual(expectedStream, actualStream));
            }
            else
            {
                // some test diagrams do not contain executable processes
                // and are therefore ignored by the engine
            }
        }

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testGetProcessDiagramAfterCacheWasCleaned()
         [Test]   public virtual void testGetProcessDiagramAfterCacheWasCleaned()
        {
            if (1 == processDefinitionQuery.Count())
            {
                activitiRule.ProcessEngineConfiguration.DeploymentCache.DiscardProcessDefinitionCache();
                // given
                IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();

                // when
                System.IO.Stream stream = repositoryService.GetProcessDiagram(processDefinition.Id);

                // then
                Assert.NotNull(processDefinition.DiagramResourceName);
                Assert.NotNull(stream);
            }
            else
            {
                // some test diagrams do not contain executable processes
                // and are therefore ignored by the engine
            }
        }

        /// <summary>
        /// Tests <seealso cref="RepositoryService#getProcessDiagramLayout(String)"/> and
        /// <seealso cref="ProcessDiagramLayoutFactory#getProcessDiagramLayout(InputStream, InputStream)"/>.
        /// </summary>
        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testGetProcessDiagramLayout() throws Exception
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
         [Test]   public virtual void testGetProcessDiagramLayout()
        {
            DiagramLayout processDiagramLayout;
            if (1 == processDefinitionQuery.Count())
            {
                IProcessDefinition processDefinition = processDefinitionQuery.First();
                Assert.NotNull(processDefinition);
                processDiagramLayout = repositoryService.GetProcessDiagramLayout(processDefinition.Id);
            }
            else
            {
                // some test diagrams do not contain executable processes
                // and are therefore ignored by the engine
                System.IO.Stream bpmnXmlStream = new System.IO.FileStream("src/test/resources/resources/api/repository/diagram/" + xmlFileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                System.IO.Stream imageStream = new System.IO.FileStream("src/test/resources/resources/api/repository/diagram/" + imageFileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                Assert.NotNull(bpmnXmlStream);
                Assert.NotNull(imageStream);
                processDiagramLayout = (new ProcessDiagramLayoutFactory()).GetProcessDiagramLayout(bpmnXmlStream, imageStream);
            }
            AssertLayoutCorrect(processDiagramLayout);
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: private void AssertLayoutCorrect(org.camunda.bpm.Engine.Repository.DiagramLayout processDiagramLayout) throws java.io.IOException
        private void AssertLayoutCorrect(DiagramLayout processDiagramLayout)
        {
            string html = generateHtmlCode(imageFileName, processDiagramLayout, highlightedActivityId);

            FileInfo htmlFile = new FileInfo("src/test/resources/resources/api/repository/diagram/" + imageFileName + ".html");
            if (OVERWRITE_EXPECTED_HTML_FILES)
            {
                //FileUtils.WriteStringToFile(htmlFile, html);
                //fail("The Assertions of this test only work if ProcessDiagramRetrievalTest#OVERWRITE_EXPECTED_HTML_FILES is set to false.");
            }
            //AssertEquals(FileUtils.ReadFileToString(htmlFile).Replace("\r", ""), html); // remove carriage returns in case the files have been fetched via Git on Windows
        }

        private static string generateHtmlCode(string imageUrl, DiagramLayout processDiagramLayout, string highlightedActivityId)
        {
            StringBuilder html = new StringBuilder();
            html.Append("<!DOCTYPE html>\n");
            html.Append("<html>\n");
            html.Append("  <head>\n");
            html.Append("    <style type=\"text/css\"><!--\n");
            html.Append("      .BPMNElement {\n");
            html.Append("        position: absolute;\n");
            html.Append("        border: 2px dashed lightBlue;\n");
            html.Append("        border-radius: 5px; -moz-border-radius: 5px;\n");
            html.Append("      }\n");
            if (!string.ReferenceEquals(highlightedActivityId, null) && highlightedActivityId.Length > 0)
            {
                html.Append("      #" + highlightedActivityId + " {border: 2px solid red;}\n");
            }
            html.Append("    --></style>");
            html.Append("  </head>\n");
            html.Append("  <body>\n");
            html.Append("    <div style=\"position: relative\">\n");
            html.Append("      <img src=\"" + imageUrl + "\" />\n");

            IList<DiagramNode> nodes = new List<DiagramNode>(processDiagramLayout.Nodes);
            // sort the nodes according to their ID property.
           // nodes.Sort(new DiagramNodeComparator());
            foreach (DiagramNode node in nodes)
            {
                html.Append("      <div");
                html.Append(" class=\"BPMNElement\"");
                html.Append(" id=\"" + node.Id + "\"");
                html.Append(" style=\"");
                html.Append(" left: " + (int)(node.X - 2) + "px;");
                html.Append(" top: " + (int)(node.Y - 2) + "px;");
                html.Append(" width: " + node.Width.Value + "px;");
                html.Append(" height: " + node.Height.Value + "px;\"></div>\n");
            }
            html.Append("    </div>\n");
            html.Append("  </body>\n");
            html.Append("</html>");
            return html.ToString();
        }

        public static byte[] StreamToBytes(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: private static boolean isEqual(java.io.InputStream stream1, java.io.InputStream stream2) throws java.io.IOException
        private static bool isEqual(System.IO.Stream stream1, System.IO.Stream stream2)
        {

            //ReadableByteChannel channel1 = Channels.NewChannel(stream1);
            //ReadableByteChannel channel2 = Channels.NewChannel(stream2);
            MemoryStream channel1 = new MemoryStream(StreamToBytes(stream1));
            MemoryStream channel2 = new MemoryStream(StreamToBytes(stream2));
            byte[] buffer1 = new byte[1024];
            byte[] buffer2 = new byte[1024];
            //ByteBuffer buffer2 = ByteBuffer.allocateDirect(1024);

            try
            {
                while (true)
                {

                    int bytesReadFromStream1 = channel1.Read(buffer1,0,1024);
                    int bytesReadFromStream2 = channel2.Read(buffer2, 0, 1024);

                    if (bytesReadFromStream1 == -1 || bytesReadFromStream2 == -1)
                    {
                        return bytesReadFromStream1 == bytesReadFromStream2;
                    }

                    //buffer1.Flip();
                    //buffer2.Flip();

                    for (int i = 0; i < Math.Min(bytesReadFromStream1, bytesReadFromStream2); i++)
                    {
                        if (buffer1 != buffer2)
                        {
                            return false;
                        }
                    }

                  //  buffer1.compact();
                  //  buffer2.compact();
                }

            }
            finally
            {
                if (stream1 != null)
                {
                    stream1.Close();
                }
                if (stream2 != null)
                {
                    stream2.Close();
                }
            }
        }

        /// <summary>
        /// Might be used for debugging <seealso cref="ProcessDiagramRetrievalTest#testGetProcessDiagram()"/>.
        /// </summary>
        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unused") private static void writeToFile(java.io.InputStream is, java.io.File file) throws Exception
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        private static void writeToFile(System.IO.Stream @is, FileStream file)
        {
            var bs =new byte[file.Length];
            file.Read(bs, 0, (int)file.Length);
            var @out = new MemoryStream(bs,true);
            int c;
            while ((c = @is.ReadByte()) != -1)
            {
                @out.WriteByte((byte)c);
            }
            @is.Close();
            @out.Close();
        }

        /// <summary>
        /// sorts <seealso cref="DiagramNode DiagramNodes"/> by ID 
        /// </summary>
        public class DiagramNodeComparator : IComparer<DiagramNode>
        {

            public virtual int Compare(DiagramNode o1, DiagramNode o2)
            {
                if (o1.Id == null)
                {
                    return 0;
                }
                return o1.Id.CompareTo(o2.Id);
            }

        }

    }

}