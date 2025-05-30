using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;

using System.IO;
using System.Xml;

namespace EISDataFilter.Utils
{
    public class XMLHandler
    {
        /// <summary>
        /// H:HEADER, B:BODY
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public XmlDocument makeEISMessageSet(DataTable dt)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.AppendChild(xmlDocument.CreateXmlDeclaration("1.0", "utf-8", string.Empty));

            // MESSAGE Node
            XmlNode MESSAGENode = xmlDocument.CreateNode(XmlNodeType.Element, "message", string.Empty);
            xmlDocument.AppendChild(MESSAGENode);

            // HEADER Node
            XmlNode HEADERNode = xmlDocument.CreateNode(XmlNodeType.Element, "header", string.Empty);
            MESSAGENode.AppendChild(HEADERNode);

            // BODY Node
            XmlNode BODYNode = xmlDocument.CreateNode(XmlNodeType.Element, "body", string.Empty);
            MESSAGENode.AppendChild(BODYNode);

            // Set DataTableItem
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string sNodeType = dt.Rows[i]["nodeType"].ToString();
                int nNodeSEQ = Convert.ToInt32(dt.Rows[i]["nodeSEQ"]);
                string sNodeKey = dt.Rows[i]["nodeKey"].ToString();
                string sNodeValue = dt.Rows[i]["nodeValue"].ToString();

                XmlElement curNode = xmlDocument.CreateElement(sNodeKey);
                curNode.InnerText = sNodeValue;

                if (nNodeSEQ == 0)
                {
                    switch (sNodeType.ToUpper())
                    {
                        case "H":
                            HEADERNode.AppendChild(curNode);
                            break;

                        case "B":
                            BODYNode.AppendChild(curNode);
                            break;
                    }
                }
                else
                {
                    switch (sNodeType.ToUpper())
                    {
                        case "H":
                            findParentNode(HEADERNode, nNodeSEQ).AppendChild(curNode);
                            break;

                        case "B":
                            findParentNode(BODYNode, nNodeSEQ).AppendChild(curNode);
                            break;
                    }
                }
            }

            return xmlDocument;
        }

        public string makeNode(string Key, string Value)
        {
            string sNode = string.Empty;

            sNode = sNode + "<";
            sNode = sNode + Key;
            sNode = sNode + ">";
            sNode = sNode + Value;
            sNode = sNode + "</";
            sNode = sNode + Key;
            sNode = sNode + ">";

            return sNode;
        }

        public StringBuilder makeNode(string Key, StringBuilder Value)
        {
            StringBuilder sNode = new StringBuilder();

            sNode.Append("<");
            sNode.Append(Key);
            sNode.Append(">");
            sNode.Append(Value.ToString());
            sNode.Append("</");
            sNode.Append(Key);
            sNode.Append(">");

            return sNode;
        }

        /// <summary>
        /// 지정된 NODE의 SEQ만큼 ChildNode를 찾아 return한다..
        /// </summary>
        /// <param name="node"></param>
        /// <param name="nodeSEQ"></param>
        /// <returns></returns>
        private XmlNode findParentNode(XmlNode node, int nodeSEQ)
        {
            XmlNode parentNode = node;

            for (int i = 0; i < nodeSEQ; i++)
            {
                parentNode = parentNode.ChildNodes[parentNode.ChildNodes.Count - 1];
            }
            return parentNode;
        }

    }
}
