using System.Xml;
namespace FA.Automation.MessageBus
{
    public class FA_EAP_RMS_Interface
    {
        /// <summary>
        /// 将 message信息转换为 XMLDoc ---liuxinliang 20240815
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static XmlDocument GetXmlDoc(string msg)
        {
            try
            {
                if (string.IsNullOrEmpty(msg))
                    return default;

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(msg);

                return doc;
            }
            catch
            {
                return default;
            }
        }

        /// <summary>
        ///从DOC中获取节点的参数名称 ---liuxinliang 20240815
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static string GetPropertyValueFromDoc(XmlDocument doc, string propertyName)
        {
            try
            {
                if (doc == null) return string.Empty;

                XmlNode node = doc.SelectSingleNode("//" + propertyName);

                if (node == null)
                {
                    return string.Empty;
                }

                return node.InnerXml;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        ///从DOC中获取节点的参数值（未转义，解决字符串带特殊字符&->&amp;的问题） ---zhujincheng 20241022
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static string GetPropertyInnerTextFromDoc(XmlDocument doc, string propertyName)
        {
            try
            {
                if (doc == null) return string.Empty;

                XmlNode node = doc.SelectSingleNode("//" + propertyName);

                if (node == null)
                {
                    return string.Empty;
                }

                return node.InnerText;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 设置 DOC中的参数的值 ---liuxinliang 20240815
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        /// <returns></returns>
        public static string SetPropertyValueToDoc(XmlDocument doc, string propertyName, string propertyValue)
        {
            if (doc == null) return string.Empty;

            XmlNode node = doc.SelectSingleNode("//" + propertyName);

            if (node == null)
            {
                return doc.InnerXml;
            }

            doc.SelectSingleNode("//" + propertyName).InnerXml = propertyValue;
            return doc.InnerXml;
        }
    }
}
