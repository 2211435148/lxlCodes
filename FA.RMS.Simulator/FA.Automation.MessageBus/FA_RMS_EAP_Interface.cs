using System.Xml;
using System.Xml.Serialization;
namespace FA.Automation.MessageBus
{
    public class FA_RMS_EAP_Interface
    {
        /// <summary>
        ///  EAP 返回给 RMS的 实体类
        /// </summary>
        public class Root
        {
            public HeaderModel Header { get; set; }

            public BodyModel Body { get; set; }

            public class HeaderModel
            {
                public string MESSAGENAME { get; set; }
                public string USERID { get; set; }
                public string TRANSACTIONID { get; set; }
            }

            public class BodyModel
            {
                public string EQPID { get; set; }
                public RECIPELIST RECIPELIST { get; set; }
                public string TRANSACTIONID { get; set; }
                public string RESULT { get; set; }
                public string COMMENT { get; set; }
            }
            public class RECIPELIST
            {
                /// <summary>
                /// L
                /// </summary>
                public List<string> L { get; set; }
            }
        }



        public static void GetPropertyFromRecipeListReplyRule(string msg,
            ref string equipmentId, ref string recipeListStr,
            ref string transactionId, ref string userId)
        {
            var docObj = GetXmlDoc(msg);
            equipmentId = GetPropertyValueFromDoc(docObj, "EQPID");
            recipeListStr = GetPropertyValueFromDoc(docObj, "RECIPELIST");
            transactionId = GetPropertyValueFromDoc(docObj, "TRANSACTIONID");
            userId = GetPropertyValueFromDoc(docObj, "USERID");
        }

        public static void GetPropertyFromRecipeBodyReplyRule(string msg,
            ref string equipmentId, ref string recipeId, ref string recipeBodyStr,
            ref string transactionId, ref string userId)
        {
            var docObj = GetXmlDoc(msg);
            equipmentId = GetPropertyValueFromDoc(docObj, "EQPID");
            recipeId = GetPropertyValueFromDoc(docObj, "RECIPEID");
            recipeBodyStr = GetPropertyValueFromDoc(docObj, "RECIPEBODY");
            transactionId = GetPropertyValueFromDoc(docObj, "TRANSACTIONID");
            userId = GetPropertyValueFromDoc(docObj, "USERID");
        }

        public static void GetPropertyFromRecipeCheckRequestRule(string msg,
            ref string equipmentId, ref string recipeId, ref string recipeType, ref string recipeBodyStr,
            ref string lotType, ref string productId,
            ref string transactionId, ref string userId)
        {
            var docObj = GetXmlDoc(msg);
            equipmentId = GetPropertyValueFromDoc(docObj, "EQPID");
            recipeId = GetPropertyValueFromDoc(docObj, "RECIPEID");
            recipeType = GetPropertyValueFromDoc(docObj, "RECIPEFORMAT");
            recipeBodyStr = GetPropertyValueFromDoc(docObj, "RECIPEBODY");
            lotType = GetPropertyValueFromDoc(docObj, "LOTTYPE");
            productId = GetPropertyValueFromDoc(docObj, "PRODUCTID");
            transactionId = GetPropertyValueFromDoc(docObj, "TRANSACTIONID");
            userId = GetPropertyValueFromDoc(docObj, "USERID");
        }

        public static string BulidRecipeListRequestRule(string equipmentId,
            string transactionId, string userId)
        {
            string newMessage = "<root><Header><MESSAGENAME>RECIPELISTREQUEST</MESSAGENAME><USERID>USERID1</USERID><TRANSACTIONID>ID123</TRANSACTIONID></Header><Body><EQPID>CEEOXE01</EQPID></Body></root>";

            var docOnj = GetXmlDoc(newMessage);
            newMessage = SetPropertyValueToDoc(docOnj, "EQPID", equipmentId);
            newMessage = SetPropertyValueToDoc(docOnj, "TRANSACTIONID", transactionId);
            newMessage = SetPropertyValueToDoc(docOnj, "USERID", userId);

            return newMessage;
        }
        public static string BuildRecipeBodyRequestRule(string equipmentId, string recipeId, string recipeType,
            string transactionId, string userId)
        {
            string newMessage = "<root><Header><MESSAGENAME>RECIPEBODYREQUEST</MESSAGENAME><USERID>USERID1</USERID><TRANSACTIONID>ID123</TRANSACTIONID></Header><Body><EQPID>CEEOXE01</EQPID><RECIPEID>RECIPEID1</RECIPEID><RECIPEFORMAT>RECIPE</RECIPEFORMAT></Body></root>";

            var docOnj = GetXmlDoc(newMessage);
            newMessage = SetPropertyValueToDoc(docOnj, "EQPID", equipmentId);
            newMessage = SetPropertyValueToDoc(docOnj, "TRANSACTIONID", transactionId);
            newMessage = SetPropertyValueToDoc(docOnj, "USERID", userId);
            newMessage = SetPropertyValueToDoc(docOnj, "RECIPEID", recipeId);
            newMessage = SetPropertyValueToDoc(docOnj, "RECIPEFORMAT", recipeType);
            return newMessage;
        }

        public static string BuildRecipeBodyCheckResultRule(string equipmentId,
            string recipeId, string lotType, string productId, string portId, string rmsCheckFlag,
            string checkResult, string comment,
            string transactionId, string userId, string recipetype, string holdLot = "N")
        {
            comment = comment.Replace("&", "&amp;");
            comment = comment.Replace("<", "&lt;");
            comment = comment.Replace(">", "&gt;");
            comment = comment.Replace("\"", "&quot;");
            comment = comment.Replace("\'", "&apos;");
            string newMessage = "<root><Header><MESSAGENAME>RECIPEBODYCHECKREPLY</MESSAGENAME><USERID>USERID1</USERID><TRANSACTIONID>ID123</TRANSACTIONID></Header><Body><EQPID>CEEOXE01</EQPID><RECIPEID>RECIPEID1</RECIPEID><PRODUCTID>productid</PRODUCTID><LOTTYPE>lottype</LOTTYPE><PORTID>1</PORTID><RECIPEFORMAT>RECIPE</RECIPEFORMAT><RMSCHECKFLAG>Y</RMSCHECKFLAG><CHECKRESULT>OK</CHECKRESULT> <HOLDLOT>N</HOLDLOT>      <COMMENT>COMMENT</COMMENT></Body></root>";

            var docObj = GetXmlDoc(newMessage);
            newMessage = SetPropertyValueToDoc(docObj, "EQPID", equipmentId);
            newMessage = SetPropertyValueToDoc(docObj, "TRANSACTIONID", transactionId);
            newMessage = SetPropertyValueToDoc(docObj, "USERID", userId);
            newMessage = SetPropertyValueToDoc(docObj, "RECIPEID", recipeId);
            newMessage = SetPropertyValueToDoc(docObj, "LOTTYPE", lotType);
            newMessage = SetPropertyValueToDoc(docObj, "PORTID", portId);
            newMessage = SetPropertyValueToDoc(docObj, "PRODUCTID", productId);
            newMessage = SetPropertyValueToDoc(docObj, "RMSCHECKFLAG", rmsCheckFlag);
            newMessage = SetPropertyValueToDoc(docObj, "CHECKRESULT", checkResult);
            newMessage = SetPropertyValueToDoc(docObj, "COMMENT", comment);
            newMessage = SetPropertyValueToDoc(docObj, "RECIPEFORMAT", recipetype);
            newMessage = SetPropertyValueToDoc(docObj, "HOLDLOT", holdLot);

            return newMessage;
        }

        public static string BuildRecipeBodyCheckResultRuleForHoldLot(string message, string isHoldLot)
        {
            var docObj = GetXmlDoc(message);
            message = SetPropertyValueToDoc(docObj, "HOLDLOT", isHoldLot);

            return message;
        }

        public static string BuildAlarmReturnResult(string sTRANSACTIONID, string sEQPID, string sALCD, string sALID, string sALTX, string sALARMLEVEL, string sISHOLDLOT, string sISHOLDEAP, string sHoldMode, string currentEqp)
        {
            string newMessage = "<root><Header><MESSAGENAME>EAPALARMREPLY</MESSAGENAME><TRANSACTIONID>ID123</TRANSACTIONID></Header><Body><EQPID>CEEOXE01</EQPID><ALCD>1</ALCD><ALID>123</ALID><ALTX>desc</ALTX><ALARMLEVEL>1</ALARMLEVEL><ISHOLDLOT>Y</ISHOLDLOT><ISHOLDEQP>Y</ISHOLDEQP><HOLDMODE>Y</HOLDMODE><CURRENTEQP>1</CURRENTEQP></Body></root>";


            var docObj = GetXmlDoc(newMessage);
            newMessage = SetPropertyValueToDoc(docObj, "TRANSACTIONID", sTRANSACTIONID);
            newMessage = SetPropertyValueToDoc(docObj, "EQPID", sEQPID);
            newMessage = SetPropertyValueToDoc(docObj, "ALCD", sALCD);
            newMessage = SetPropertyValueToDoc(docObj, "ALID", sALID);
            newMessage = SetPropertyValueToDoc(docObj, "ALTX", sALTX);
            newMessage = SetPropertyValueToDoc(docObj, "ALARMLEVEL", sALARMLEVEL);
            newMessage = SetPropertyValueToDoc(docObj, "ISHOLDLOT", sISHOLDLOT);
            newMessage = SetPropertyValueToDoc(docObj, "ISHOLDEQP", sISHOLDEAP);
            newMessage = SetPropertyValueToDoc(docObj, "HOLDMODE", sHoldMode);
            newMessage = SetPropertyValueToDoc(docObj, "CURRENTEQP", currentEqp);
            return newMessage;

        }

        public static string BuildNeedCheckParamListReturnResult(string sTRANSACTIONID, List<string> lstECParam, List<string> lstSVParam, string sEQPID)
        {
            string newMessage = "<root><Header><MESSAGENAME>NEEDBECHECKPARAMETERLISTREPLY</MESSAGENAME><TRANSACTIONID>ID123</TRANSACTIONID></Header><Body><EQPID>CEEOXE01</EQPID><PAMETERLIST><ECLIST></ECLIST><SVLIST></SVLIST></PAMETERLIST></Body></root>";
            string sParamItem = "<PARAMETER><ID>ID2</ID><VALUE></VALUE></PARAMETER>";
            string sECList = "";
            string sSVList = "";
            var docObj = GetXmlDoc(newMessage);
            var docObjParamEc = GetXmlDoc(sParamItem);
            var docObjParamSv = GetXmlDoc(sParamItem);

            foreach (var sparam in lstECParam)
            {
                sParamItem = SetPropertyValueToDoc(docObjParamEc, "ID", sparam);
                sECList += sParamItem;
            }
            foreach (var sparam in lstSVParam)
            {
                sParamItem = SetPropertyValueToDoc(docObjParamSv, "ID", sparam);
                sSVList += sParamItem;
            }

            newMessage = SetPropertyValueToDoc(docObj, "TRANSACTIONID", sTRANSACTIONID);
            newMessage = SetPropertyValueToDoc(docObj, "EQPID", sEQPID);
            newMessage = SetPropertyValueToDoc(docObj, "ECLIST", sECList);
            newMessage = SetPropertyValueToDoc(docObj, "SVLIST", sSVList);

            return newMessage;
        }

        public static string BuildParamValueRequestRule(string sTRANSACTIONID, List<string> lstECParam, List<string> lstSVParam, string sEQPID, string sUserID)
        {
            string newMessage = "<root><Header><MESSAGENAME>PARAMETERVALUEREQUEST</MESSAGENAME><USERID>USERID1</USERID><TRANSACTIONID>ID123</TRANSACTIONID></Header><Body><EQPID>CEEOXE01</EQPID><PAMETERLIST><ECLIST></ECLIST><SVLIST></SVLIST></PAMETERLIST></Body></root>";
            string sParamItem = "<PARAMETER><ID>ID2</ID><VALUE></VALUE></PARAMETER>";
            string sECList = "";
            string sSVList = "";
            var docObj = GetXmlDoc(newMessage);
            var docObjParamEc = GetXmlDoc(sParamItem);
            var docObjParamSv = GetXmlDoc(sParamItem);
            foreach (var sparam in lstECParam)
            {
                sParamItem = SetPropertyValueToDoc(docObjParamEc, "ID", sparam);
                sECList += sParamItem;
            }
            foreach (var sparam in lstSVParam)
            {
                sParamItem = SetPropertyValueToDoc(docObjParamSv, "ID", sparam);
                sSVList += sParamItem;
            }
            newMessage = SetPropertyValueToDoc(docObj, "USERID", sUserID);
            newMessage = SetPropertyValueToDoc(docObj, "TRANSACTIONID", sTRANSACTIONID);
            newMessage = SetPropertyValueToDoc(docObj, "EQPID", sEQPID);
            newMessage = SetPropertyValueToDoc(docObj, "ECLIST", sECList);
            newMessage = SetPropertyValueToDoc(docObj, "SVLIST", sSVList);

            return newMessage;
        }

        public static string BuildParamCheckReturnResult(string sTRANSACTIONID, string sCheckResult, string sComment, string sEQPID)
        {
            string newMessage = "<root><Header><MESSAGENAME>PARAMETERCHECKREPLY</MESSAGENAME><TRANSACTIONID>ID123</TRANSACTIONID></Header><Body><EQPID>CEEOXE01</EQPID><CHECKRESULT>OK/NG</CHECKRESULT><RESULTCOMMENT>DETAIL</RESULTCOMMENT></Body></root>  ";

            var docObj = GetXmlDoc(newMessage);
            newMessage = SetPropertyValueToDoc(docObj, "TRANSACTIONID", sTRANSACTIONID);
            newMessage = SetPropertyValueToDoc(docObj, "EQPID", sEQPID);
            newMessage = SetPropertyValueToDoc(docObj, "CHECKRESULT", sCheckResult);
            newMessage = SetPropertyValueToDoc(docObj, "RESULTCOMMENT", sComment);

            return newMessage;
        }

        /// <summary>
        /// 获取 发送Golden的数据格式字符串
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="TransactionID"></param>
        /// <param name="EQPID"></param>
        /// <param name="RecipeID"></param>
        /// <param name="GoldenRecipeBody"></param>
        /// <param name="describe"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static string BuildGoldenRecipeRequestReturnResult(string UserID, string TransactionID, string EQPID, string RecipeID, string GoldenRecipeBody, string describe, string result)
        {
            string newMessage = "<root><Header><MESSAGENAME>GOLDENRECIPEREPLY</MESSAGENAME><TRANSACTIONID>ID123</TRANSACTIONID><USERID>USERID1</USERID></Header><Body><EQPID>CEEOXE01</EQPID><RECIPEID>RECIPEID1</RECIPEID><RECIPEBODY>RECIPEBODY1</RECIPEBODY><GETGOLDENBODYRESULT>OK/NG</GETGOLDENBODYRESULT><COMMENT>DETAIL</COMMENT></Body></root> ";

            var docObj = GetXmlDoc(newMessage);
            newMessage = SetPropertyValueToDoc(docObj, "USERID", UserID);
            newMessage = SetPropertyValueToDoc(docObj, "TRANSACTIONID", TransactionID);
            newMessage = SetPropertyValueToDoc(docObj, "EQPID", EQPID);
            newMessage = SetPropertyValueToDoc(docObj, "RECIPEID", RecipeID);
            newMessage = SetPropertyValueToDoc(docObj, "RECIPEBODY", GoldenRecipeBody);
            newMessage = SetPropertyValueToDoc(docObj, "GETGOLDENBODYRESULT", result);
            newMessage = SetPropertyValueToDoc(docObj, "COMMENT", describe);

            return newMessage;
        }
        //}
        public static bool CheckPropertyExistOrNot(string msg, string propertyName)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(msg);
                XmlNode node = doc.SelectSingleNode("//" + propertyName);

                if (node == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

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


        public static string GetPropertyValueFromFAEAPMessage(string msg, string propertyName)
        {
            string propertyValue = "";
            try
            {
                if (!CheckPropertyExistOrNot(msg, propertyName))
                    return propertyValue;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(msg);
                XmlNode node = doc.SelectSingleNode("//" + propertyName);
                return node.InnerXml;

            }
            catch (Exception e)
            {
                return propertyValue;
            }

        }

        public static string SetPropertyValueToFAEAPMessage(string modelMsg, string propertyName, string propertyValue)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(modelMsg);
            doc.SelectSingleNode("//" + propertyName).InnerXml = propertyValue;
            return doc.InnerXml;
        }
    }
}
