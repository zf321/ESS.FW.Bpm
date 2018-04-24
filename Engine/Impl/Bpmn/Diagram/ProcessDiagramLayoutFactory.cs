using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Repository;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Diagram
{
    /// <summary>
    ///     Provides positions and dimensions of elements in a process diagram as
    ///     provided by <seealso cref="IRepositoryService#getProcessDiagram(String)" />.
    ///     
    /// </summary>
    public class ProcessDiagramLayoutFactory
    {
        private const int GreyThreshold = 175;

        /// <summary>
        ///     Provides positions and dimensions of elements in a process diagram as
        ///     provided by <seealso cref="IRepositoryService#getProcessDiagram(String)" />.
        ///     Currently, it only supports BPMN 2.0 models.
        /// </summary>
        /// <param name="bpmnXmlStream">
        ///     BPMN 2.0 XML file
        /// </param>
        /// <param name="imageStream">
        ///     BPMN 2.0 diagram in PNG format (JPEG and other formats supported
        ///     by <seealso cref="ImageIO" /> may also work)
        /// </param>
        /// <returns> Layout of the process diagram </returns>
        /// <returns> null when parameter imageStream is null </returns>
        public virtual DiagramLayout GetProcessDiagramLayout(Stream bpmnXmlStream, Stream imageStream)
        {
            //XmlDocument bpmnModel = parseXml(bpmnXmlStream);
            //return getBpmnProcessDiagramLayout(bpmnModel, imageStream);
            return null;
        }

        /// <summary>
        ///     Provides positions and dimensions of elements in a BPMN process diagram as
        ///     provided by <seealso cref="IRepositoryService#getProcessDiagram(String)" />.
        /// </summary>
        /// <param name="bpmnXmlStream">
        ///     BPMN 2.0 XML document
        /// </param>
        /// <param name="imageStream">
        ///     BPMN 2.0 diagram in PNG format (JPEG and other formats supported
        ///     by <seealso cref="ImageIO" /> may also work)
        /// </param>
        /// <returns> Layout of the process diagram </returns>
        /// <returns> null when parameter imageStream is null </returns>
        //public virtual DiagramLayout getBpmnProcessDiagramLayout(XmlDocument bpmnModel, Stream imageStream)
        //{
        //    if (imageStream == null)
        //    {
        //        return null;
        //    }
        //    var diagramBoundsXml = getDiagramBoundsFromBpmnDi(bpmnModel);
        //    DiagramNode diagramBoundsImage;
        //    if (isExportedFromAdonis50(bpmnModel))
        //    {
        //        var offsetTop = 29; // Adonis header
        //        var offsetBottom = 61; // Adonis footer
        //        diagramBoundsImage = getDiagramBoundsFromImage(imageStream, offsetTop, offsetBottom);
        //    }
        //    else
        //    {
        //        diagramBoundsImage = getDiagramBoundsFromImage(imageStream);
        //    }

        //    IDictionary<string, DiagramNode> listOfBounds = new Dictionary<string, DiagramNode>();
        //    listOfBounds[diagramBoundsXml.Id] = diagramBoundsXml;
        //    //    listOfBounds.putAll(getElementBoundsFromBpmnDi(bpmnModel));
        //    //JAVA TO C# CONVERTER TODO ITask: There is no .NET Dictionary equivalent to the Java 'putAll' method:
        //    foreach (var item in fixFlowNodePositionsIfModelFromAdonis(bpmnModel, getElementBoundsFromBpmnDi(bpmnModel)))
        //    {
        //        listOfBounds.Add(item);
        //    }

        //    var listOfBoundsForImage = transformBoundsForImage(diagramBoundsImage, diagramBoundsXml, listOfBounds);
        //    return new DiagramLayout(listOfBoundsForImage);
        //}

        //protected internal virtual XmlDocument parseXml(Stream bpmnXmlStream)
        //{
        //    // Initiate DocumentBuilderFactory
        //    DocumentBuilderFactory factory = DocumentBuilderFactory.newInstance();
        //    // Get one that understands namespaces
        //    factory.NamespaceAware = true;

        //    DocumentBuilder builder;
        //    XmlDocument bpmnModel;
        //    try
        //    {
        //        // Get DocumentBuilder
        //        builder = factory.newDocumentBuilder();
        //        // Parse and load the Document into memory
        //        bpmnModel = builder.parse(bpmnXmlStream);
        //    }
        //    catch (Exception e)
        //    {
        //        throw new ProcessEngineException("Error while parsing BPMN model.", e);
        //    }
        //    return bpmnModel;
        //}
        protected internal virtual DiagramNode GetDiagramBoundsFromBpmnDi(XmlDocument bpmnModel)
        {
            double? minX = null;
            double? minY = null;
            double? maxX = null;
            double? maxY = null;

            // Node positions and dimensions
            var setOfBounds = bpmnModel.GetElementsByTagName(BpmnParser.BpmnDcNs, "Bounds");
            for (var i = 0; i < setOfBounds.Count; i++)
            {
                var element = (XmlElement) setOfBounds.Item(i);
                double? x = Convert.ToDouble(element.Attributes["x"]);
                double? y = Convert.ToDouble(element.Attributes["y"]);
                double? width = Convert.ToDouble(element.Attributes["width"]);
                double? height = Convert.ToDouble(element.Attributes["height"]);

                if ((x == 0.0) && (y == 0.0) && (width == 0.0) && (height == 0.0))
                {
                    // Ignore empty labels like the ones produced by Yaoqiang:
                    // <bpmndi:BPMNLabel><dc:Bounds height="0.0" width="0.0" x="0.0" y="0.0"/></bpmndi:BPMNLabel>
                }
                else
                {
                    if ((minX == null) || (x < minX))
                        minX = x;
                    if ((minY == null) || (y < minY))
                        minY = y;
                    if ((maxX == null) || (maxX < x + width))
                        maxX = x + width;
                    if ((maxY == null) || (maxY < y + height))
                        maxY = y + height;
                }
            }

            // Edge bend points
            var waypoints = bpmnModel.GetElementsByTagName(BpmnParser.OmgDiNs, "waypoint");
            for (var i = 0; i < waypoints.Count; i++)
            {
                var waypoint = (XmlElement) waypoints.Item(i);
                double? x = Convert.ToDouble(waypoint.Attributes["x"]);
                double? y = Convert.ToDouble(waypoint.Attributes["y"]);

                if ((minX == null) || (x < minX))
                    minX = x;
                if ((minY == null) || (y < minY))
                    minY = y;
                if ((maxX == null) || (maxX < x))
                    maxX = x;
                if ((maxY == null) || (maxY < y))
                    maxY = y;
            }

            var diagramBounds = new DiagramNode("BPMNDiagram");
            diagramBounds.X = minX;
            diagramBounds.Y = minY;
            diagramBounds.Width = maxX - minX;
            diagramBounds.Height = maxY - minY;
            return diagramBounds;
        }

        //protected internal virtual DiagramNode getDiagramBoundsFromImage(Stream imageStream)
        //{
        //    return getDiagramBoundsFromImage(imageStream, 0, 0);
        //}

        //protected internal virtual DiagramNode getDiagramBoundsFromImage(Stream imageStream, int offsetTop,
        //    int offsetBottom)
        //{
        //    BufferedImage image;
        //    try
        //    {
        //        image = ImageIO.read(imageStream);
        //    }
        //    catch (IOException e)
        //    {
        //        throw new ProcessEngineException("Error while reading process diagram image.", e);
        //    }
        //    var diagramBoundsImage = getDiagramBoundsFromImage(image, offsetTop, offsetBottom);
        //    return diagramBoundsImage;
        //}

        //protected internal virtual DiagramNode getDiagramBoundsFromImage(BufferedImage image, int offsetTop,
        //    int offsetBottom)
        //{
        //    int width = image.Width;
        //    int height = image.Height;

        //    IDictionary<int?, bool> rowIsWhite = new SortedDictionary<int?, bool>();
        //    IDictionary<int?, bool> columnIsWhite = new SortedDictionary<int?, bool>();

        //    for (var row = 0; row < height; row++)
        //    {
        //        if (!rowIsWhite.ContainsKey(row))
        //        {
        //            rowIsWhite[row] = true;
        //        }
        //        if (row <= offsetTop || row > image.Height - offsetBottom)
        //        {
        //            rowIsWhite[row] = true;
        //        }
        //        else
        //        {
        //            for (var column = 0; column < width; column++)
        //            {
        //                if (!columnIsWhite.ContainsKey(column))
        //                {
        //                    columnIsWhite[column] = true;
        //                }
        //                int pixel = image.getRGB(column, row);
        //                var alpha = (pixel >> 24) & 0xff;
        //                var red = (pixel >> 16) & 0xff;
        //                var green = (pixel >> 8) & 0xff;
        //                var blue = (pixel >> 0) & 0xff;
        //                if (
        //                    !(alpha == 0 || (red >= GREY_THRESHOLD && green >= GREY_THRESHOLD && blue >= GREY_THRESHOLD)))
        //                {
        //                    rowIsWhite[row] = false;
        //                    columnIsWhite[column] = false;
        //                }
        //            }
        //        }
        //    }

        //    var marginTop = 0;
        //    for (var row = 0; row < height; row++)
        //    {
        //        if (rowIsWhite[row])
        //        {
        //            ++marginTop;
        //        }
        //        else
        //        {
        //            // Margin Top Found
        //            break;
        //        }
        //    }

        //    var marginLeft = 0;
        //    for (var column = 0; column < width; column++)
        //    {
        //        if (columnIsWhite[column])
        //        {
        //            ++marginLeft;
        //        }
        //        else
        //        {
        //            // Margin Left Found
        //            break;
        //        }
        //    }

        //    var marginRight = 0;
        //    for (var column = width - 1; column >= 0; column--)
        //    {
        //        if (columnIsWhite[column])
        //        {
        //            ++marginRight;
        //        }
        //        else
        //        {
        //            // Margin Right Found
        //            break;
        //        }
        //    }

        //    var marginBottom = 0;
        //    for (var row = height - 1; row >= 0; row--)
        //    {
        //        if (rowIsWhite[row])
        //        {
        //            ++marginBottom;
        //        }
        //        else
        //        {
        //            // Margin Bottom Found
        //            break;
        //        }
        //    }

        //    var diagramBoundsImage = new DiagramNode();
        //    diagramBoundsImage.X = marginLeft;
        //    diagramBoundsImage.Y = marginTop;
        //    diagramBoundsImage.Width = width - marginRight - marginLeft;
        //    diagramBoundsImage.Height = height - marginBottom - marginTop;
        //    return diagramBoundsImage;
        //}

        protected internal virtual IDictionary<string, DiagramNode> GetElementBoundsFromBpmnDi(XmlDocument bpmnModel)
        {
            IDictionary<string, DiagramNode> listOfBounds = new Dictionary<string, DiagramNode>();
            // iterate over all DI shapes
            var shapes = bpmnModel.GetElementsByTagName(BpmnParser.BpmnDiNs, "BPMNShape");
            for (var i = 0; i < shapes.Count; i++)
            {
                var shape = (XmlElement) shapes.Item(i);
                var bpmnElementId = shape.Attributes["bpmnElement"].Value;
                // get bounds of shape
                var childNodes = shape.ChildNodes;
                for (var j = 0; j < childNodes.Count; j++)
                {
                    var childNode = childNodes.Item(j);
                    if (childNode is XmlElement && BpmnParser.BpmnDcNs.Equals(childNode.NamespaceURI) &&
                        "Bounds".Equals(childNode.LocalName))
                    {
                        var bounds = ParseBounds((XmlElement) childNode);
                        bounds.Id = bpmnElementId;
                        listOfBounds[bpmnElementId] = bounds;
                        break;
                    }
                }
            }
            return listOfBounds;
        }

        protected internal virtual DiagramNode ParseBounds(XmlElement boundsElement)
        {
            var bounds = new DiagramNode();
            bounds.X = Convert.ToDouble(boundsElement.Attributes["x"]);
            bounds.Y = Convert.ToDouble(boundsElement.Attributes["y"]);
            bounds.Width = Convert.ToDouble(boundsElement.Attributes["width"]);
            bounds.Height = Convert.ToDouble(boundsElement.Attributes["height"]);
            return bounds;
        }

        protected internal virtual IDictionary<string, DiagramElement> TransformBoundsForImage(
            DiagramNode diagramBoundsImage, DiagramNode diagramBoundsXml, IDictionary<string, DiagramNode> listOfBounds)
        {
            IDictionary<string, DiagramElement> listOfBoundsForImage = new Dictionary<string, DiagramElement>();
            foreach (var bounds in listOfBounds)
                listOfBoundsForImage[bounds.Key] = TransformBoundsForImage(diagramBoundsImage, diagramBoundsXml,
                    bounds.Value);
            return listOfBoundsForImage;
        }

        protected internal virtual DiagramNode TransformBoundsForImage(DiagramNode diagramBoundsImage,
            DiagramNode diagramBoundsXml, DiagramNode elementBounds)
        {
            var scalingFactorX = diagramBoundsImage.Width.GetValueOrDefault()/diagramBoundsXml.Width.GetValueOrDefault();
            var scalingFactorY = diagramBoundsImage.Width.GetValueOrDefault()/diagramBoundsXml.Width.GetValueOrDefault();

            var elementBoundsForImage = new DiagramNode(elementBounds.Id);
            elementBoundsForImage.X =
                Math.Round(elementBounds.X.GetValueOrDefault() - diagramBoundsXml.X.GetValueOrDefault() +
                           diagramBoundsImage.X.GetValueOrDefault());
            elementBoundsForImage.Y =
                Math.Round((elementBounds.Y.GetValueOrDefault() - diagramBoundsXml.Y.GetValueOrDefault())*scalingFactorY +
                           diagramBoundsImage.Y.GetValueOrDefault());
            elementBoundsForImage.Width = Math.Round(elementBounds.Width.GetValueOrDefault()*scalingFactorX);
            elementBoundsForImage.Height = Math.Round(elementBounds.Height.GetValueOrDefault()*scalingFactorY);
            return elementBoundsForImage;
        }

        //protected internal virtual IDictionary<string, DiagramNode> fixFlowNodePositionsIfModelFromAdonis(
        //    XmlDocument bpmnModel, IDictionary<string, DiagramNode> elementBoundsFromBpmnDi)
        //{
        //    if (isExportedFromAdonis50(bpmnModel))
        //    {
        //        IDictionary<string, DiagramNode> mapOfFixedBounds = new Dictionary<string, DiagramNode>();
        //        XPathFactory xPathFactory = XPathFactory.newInstance();
        //        XPath xPath = xPathFactory.newXPath();
        //        xPath.NamespaceContext = new Bpmn20NamespaceContext();
        //        foreach (var entry in elementBoundsFromBpmnDi)
        //        {
        //            var elementId = entry.Key;
        //            var elementBounds = entry.Value;
        //            var expression = "local-name(//bpmn:*[@id = '" + elementId + "'])";
        //            try
        //            {
        //                XPathExpression xPathExpression = xPath.compile(expression);
        //                string elementLocalName = xPathExpression.Evaluate(bpmnModel);
        //                if (!"participant".Equals(elementLocalName) && !"lane".Equals(elementLocalName) &&
        //                    !"textAnnotation".Equals(elementLocalName) && !"group".Equals(elementLocalName))
        //                {
        //                    elementBounds.X = elementBounds.X - elementBounds.Width/2;
        //                    elementBounds.Y = elementBounds.Y - elementBounds.Height/2;
        //                }
        //            }
        //            catch (XPathExpressionException e)
        //            {
        //                throw new ProcessEngineException(
        //                    "Error while evaluating the following XPath expression on a BPMN XML document: '" +
        //                    expression + "'.", e);
        //            }
        //            mapOfFixedBounds[elementId] = elementBounds;
        //        }
        //        return mapOfFixedBounds;
        //    }
        //    return elementBoundsFromBpmnDi;
        //}

        protected internal virtual bool IsExportedFromAdonis50(XmlDocument bpmnModel)
        {
            return "ADONIS".Equals(bpmnModel.DocumentElement.Attributes["exporter"]) &&
                   "5.0".Equals(bpmnModel.DocumentElement.Attributes["exporterVersion"]);
        }
    }
}