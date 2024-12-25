using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace FA.Automation.MessageBus
{
    public class Utill
    {
        /// <summary>
        /// 判断字符串是否为xml
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsXml(string str)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(str); // 尝试加载字符串
                return true; // 如果没有异常，则认为是有效的XML
            }
            catch (Exception)
            {
                return false; // 如果有异常，则认为不是有效的XML
            }
        }

        /// <summary>
        /// xml序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToSerialize"></param>
        /// <returns></returns>
        public static string Convert2XmlStr<T>(T objectToSerialize) where T : class
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            using (StringWriter textWriter = new StringWriter())
            {
                serializer.Serialize(textWriter, objectToSerialize);
                return textWriter.ToString();
            }
        }
        public static string FormatXml(string xml)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            using (StringWriter textWriter = new StringWriter())
            {
                using (XmlTextWriter xmlTextWriter = new XmlTextWriter(textWriter))
                {
                    // 设置缩进
                    xmlTextWriter.Formatting = Formatting.Indented;
                    // 如果需要，可以设置缩进字符串
                    xmlTextWriter.Indentation = 4;
                    xmlTextWriter.IndentChar = ' ';

                    xmlDoc.WriteContentTo(xmlTextWriter);
                    xmlTextWriter.Flush();

                    return textWriter.ToString();
                }
            }
        }
        public static string EntityToXmlString(MyXMLEntity Entity)
        {
            if (Entity == null)
            {
                return "";
            }
            XmlDocument doc = new XmlDocument();
            XmlNode root = doc.CreateElement(Entity.NodeName);
            foreach (var attr in Entity.Attrs)
            {
                XmlAttribute nodeattr = doc.CreateAttribute(attr.Key);
                nodeattr.Value = attr.Value;
                root.Attributes.Append(nodeattr);
            }

            if (Entity.SubNodes.Count > 0)
            {
                foreach (var ChildrenNode in Entity.SubNodes)
                {
                    root.AppendChild(ChildNodeSerialize(doc, ChildrenNode));
                }
            }
            doc.AppendChild(root);
            return doc.InnerXml;

        }
        private static XmlNode ChildNodeSerialize(XmlDocument doc, MyXMLEntity Entity)
        {
            try
            {
                XmlNode root = doc.CreateElement(Entity.NodeName);
                foreach (var attr in Entity.Attrs)
                {
                    XmlAttribute nodeattr = doc.CreateAttribute(attr.Key);
                    nodeattr.Value = attr.Value;
                    root.Attributes.Append(nodeattr);
                }
                if (!string.IsNullOrWhiteSpace(Entity.InnerText))
                {
                    root.InnerText = Entity.InnerText;
                }
                if (!string.IsNullOrWhiteSpace(Entity.InnerXml))
                {
                    root.InnerXml = Entity.InnerXml;
                }
                if (Entity.SubNodes.Count > 0)
                {
                    foreach (var ChildrenNode in Entity.SubNodes)
                    {
                        root.AppendChild(ChildNodeSerialize(doc, ChildrenNode));
                    }
                }
                return root;
            }
            catch (Exception ex)
            {
                return null;
            }

        }


        public static string ReadAttribute(XmlNode xmlNode, string Attribute)
        {
            XmlElement xe = (XmlElement)xmlNode;
            if (xe.HasAttribute(Attribute))
                return xe.GetAttribute(Attribute).ToString();
            else
                return "";

        }

        public static string ReadInnerXml(XmlNode xmlNode)
        {
            if (xmlNode != null)
                return xmlNode.InnerXml;
            else
                return "";
        }



    }
}
