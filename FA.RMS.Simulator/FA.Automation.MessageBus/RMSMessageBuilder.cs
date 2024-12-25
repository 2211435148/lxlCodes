using MySecsDriver.Structure;
using System.Xml;

namespace FA.Automation.MessageBus
{
    public class RMSMessageBuilder
    {
        public static string BuildRecipeBodyDownLoadReplyXML(string userId, string transactionId, string fromEqpId, string EqpId,string recipeId,string recipeVer ,string result, string COMMENT)
        {
            MyXMLEntity entity = new MyXMLEntity() { NodeName = "root" };

            MyXMLEntity headerEntity = new MyXMLEntity() { NodeName = "Header" };

            headerEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "MESSAGENAME", InnerText = "RECIPEBODYDOWNLOADREPLY" });
            headerEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "USERID", InnerText = userId });
            headerEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "TRANSACTIONID", InnerText = transactionId });
            entity.SubNodes.Add(headerEntity);


            MyXMLEntity bodyEntity = new MyXMLEntity() { NodeName = "Body" };
            bodyEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "FROMEQP", InnerText = fromEqpId });
            bodyEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "EQPID", InnerText = EqpId });
            bodyEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "RECIPEID", InnerText = recipeId });
            bodyEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "RECIPEVER", InnerText = recipeVer });
            XmlDocument xmlDoc = null;
            XmlNode xmlNode = null;
            bodyEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "RESULT", InnerText = result });
            bodyEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "COMMENT", InnerText = COMMENT });
            entity.SubNodes.Add(bodyEntity);
            return Utill.EntityToXmlString(entity);
        }

        public static string BuildRecipeListReplyXML(string userId, string transactionId, string EqpId, string recipeListStr, string result, string COMMENT)
        {
            MyXMLEntity entity = new MyXMLEntity() { NodeName = "root" };

            MyXMLEntity headerEntity = new MyXMLEntity() { NodeName = "Header" };

            headerEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "MESSAGENAME", InnerText = "RECIPELISTREPLY" });
            headerEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "USERID", InnerText = userId });
            headerEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "TRANSACTIONID", InnerText = transactionId });
            entity.SubNodes.Add(headerEntity);


            MyXMLEntity bodyEntity = new MyXMLEntity() { NodeName = "Body" };
            bodyEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "EQPID", InnerText = EqpId });
            XmlDocument xmlDoc = null;
            XmlNode xmlNode = null;
            ///bodyEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "RECIPELIST", InnerXml = ConvertSecsTransToXML(recipeListTrans.Children, ref xmlDoc, ref xmlNode) });
            bodyEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "RECIPELIST", InnerXml = recipeListStr });
            bodyEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "RESULT", InnerText = result });
            bodyEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "COMMENT", InnerText = COMMENT });
            entity.SubNodes.Add(bodyEntity);
            return Utill.EntityToXmlString(entity);
        }
        public static string BuildRecipeBodyReplyXML(string userId, string TransactionId, string EqpId, string recipeId, string recipeFormat, string recipeBodyStr, string result, string Comment)
        {
            MyXMLEntity entity = new MyXMLEntity() { NodeName = "root" };

            MyXMLEntity headerEntity = new MyXMLEntity() { NodeName = "Header" };
            headerEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "MESSAGENAME", InnerText = "RECIPEBODYREPLY" });
            headerEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "USERID", InnerText = userId });
            headerEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "TRANSACTIONID", InnerText = TransactionId });
            entity.SubNodes.Add(headerEntity);

            MyXMLEntity bodyEntity = new MyXMLEntity() { NodeName = "Body" };
            bodyEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "EQPID", InnerText = EqpId });
            bodyEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "RECIPEID", InnerText = recipeId });
            bodyEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "RECIPEFORMAT", InnerText = recipeFormat });
            XmlDocument xmlDoc = null;
            XmlNode xmlNode = null;
            //bodyEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "RECIPEBODY", InnerXml = ConvertSecsTransToXML(recipeBodyTrans.Children, ref xmlDoc, ref xmlNode) });
            bodyEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "RECIPEBODY", InnerXml = recipeBodyStr });
            bodyEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "RESULT", InnerText = result });
            bodyEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "COMMENT", InnerText = Comment });
            entity.SubNodes.Add(bodyEntity);

            return Utill.EntityToXmlString(entity);
        }
        public static string BuildRecipeBodyReplyXMLByDictionary(string userId, string TransactionId, string EqpId, string recipeId, string recipeFormat, Dictionary<string, Dictionary<string, string>> bodys, string result, string Comment)
        {
            MyXMLEntity entity = new MyXMLEntity() { NodeName = "root" };

            MyXMLEntity headerEntity = new MyXMLEntity() { NodeName = "Header" };
            headerEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "MESSAGENAME", InnerText = "RECIPEBODYREPLY" });
            headerEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "USERID", InnerText = userId });
            headerEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "TRANSACTIONID", InnerText = TransactionId });
            entity.SubNodes.Add(headerEntity);

            MyXMLEntity bodyEntity = new MyXMLEntity() { NodeName = "Body" };
            bodyEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "EQPID", InnerText = EqpId });
            bodyEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "RECIPEID", InnerText = recipeId });
            bodyEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "RECIPEFORMAT", InnerText = recipeFormat });
            MyXMLEntity Body = new MyXMLEntity();
            foreach (var a in bodys)
            {
                MyXMLEntity sonBody = new MyXMLEntity();
                sonBody.NodeName = "Father";
                sonBody.InnerText = a.Key;
                foreach (var c in a.Value)
                {
                    MyXMLEntity sonnode = new MyXMLEntity();
                    if (!c.Key.Trim().Equals(""))
                    {
                        sonnode.NodeName = "Son";
                    }
                    else
                    {
                        sonnode.NodeName = "null";
                    }
                    if (!c.Value.Trim().Equals(""))
                    {
                        sonnode.InnerText = c.Key.Trim().Replace(" ", "").ToString() + "&" + c.Value.Trim();
                    }

                    sonBody.SubNodes.Add(sonnode);
                }
                Body.SubNodes.Add(sonBody);
            }
            Body.NodeName = "Sum";
            MyXMLEntity recipebody = new MyXMLEntity();
            recipebody.NodeName = "RECIPEBODY";
            recipebody.SubNodes.Add(Body);
            bodyEntity.SubNodes.Add(recipebody);
            bodyEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "RESULT", InnerText = result });
            bodyEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "COMMENT", InnerText = Comment });
            entity.SubNodes.Add(bodyEntity);

            return Utill.EntityToXmlString(entity);
        }
        /*
        public static string BuildRecipeBodyReplyXML(string userId, string TransactionId, string EqpId, string recipeId, string recipeFormat, string RecipeBodyXML, string result, string Comment)
        {
            MyXMLEntity entity = new MyXMLEntity() { NodeName = "root" };

            MyXMLEntity headerEntity = new MyXMLEntity() { NodeName = "Header" };
            headerEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "MESSAGENAME", InnerText = "RECIPEBODYREPLY" });
            headerEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "USERID", InnerText = userId });
            headerEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "TRANSACTIONID", InnerText = TransactionId });
            entity.SubNodes.Add(headerEntity);

            MyXMLEntity bodyEntity = new MyXMLEntity() { NodeName = "Body" };
            bodyEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "EQPID", InnerText = EqpId });
            bodyEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "RECIPEID", InnerText = recipeId });
            bodyEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "RECIPEFORMAT", InnerText = recipeFormat });

            bodyEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "RECIPEBODY", InnerXml = RecipeBodyXML });
            bodyEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "RESULT", InnerText = result });
            bodyEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "COMMENT", InnerText = Comment });
            entity.SubNodes.Add(bodyEntity);

            return Utill.EntityToXmlString(entity);
        }
      */
        public static string BuildRecipeBodyCheckRequestXML(string userId, string transactionId, string EqpId, string recipeId, string productId, string lotType, string portId, string recipeBody, string recipeFormat)
        {
            MyXMLEntity entity = new MyXMLEntity() { NodeName = "root" };

            MyXMLEntity headerEntity = new MyXMLEntity() { NodeName = "Header" };
            headerEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "MESSAGENAME", InnerText = "RECIPEBODYCHECKREQUEST" });
            headerEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "USERID", InnerText = userId });
            headerEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "TRANSACTIONID", InnerText = transactionId });
            entity.SubNodes.Add(headerEntity);

            MyXMLEntity bodyEntity = new MyXMLEntity() { NodeName = "Body" };
            bodyEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "EQPID", InnerText = EqpId });
            bodyEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "RECIPEID", InnerText = recipeId });
            bodyEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "PRODUCTID", InnerText = productId });
            bodyEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "LOTTYPE", InnerText = lotType });
            bodyEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "PORTID", InnerText = portId });
            XmlDocument xmlDoc = null;
            XmlNode xmlNode = null;
            //bodyEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "RECIPEBODY", InnerXml = ConvertSecsTransToXML(recipeBodyTrans.Children, ref xmlDoc, ref xmlNode) });
            bodyEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "RECIPEBODY", InnerXml = recipeBody });
            bodyEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "RECIPEFORMAT", InnerText = recipeFormat });
            entity.SubNodes.Add(bodyEntity);

            return Utill.EntityToXmlString(entity);
        }
        public static string BuildRecipeBodyCheckRequestXMLByDictionary(string userId, string transactionId, string EqpId, string recipeId, string productId, string lotType, string portId, Dictionary<string, Dictionary<string, string>> bodys, string recipeFormat)
        {
            MyXMLEntity entity = new MyXMLEntity() { NodeName = "root" };

            MyXMLEntity headerEntity = new MyXMLEntity() { NodeName = "Header" };
            headerEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "MESSAGENAME", InnerText = "RECIPEBODYCHECKREQUEST" });
            headerEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "USERID", InnerText = userId });
            headerEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "TRANSACTIONID", InnerText = transactionId });
            entity.SubNodes.Add(headerEntity);

            MyXMLEntity bodyEntity = new MyXMLEntity() { NodeName = "Body" };
            bodyEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "EQPID", InnerText = EqpId });
            bodyEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "RECIPEID", InnerText = recipeId });
            bodyEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "PRODUCTID", InnerText = productId });
            bodyEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "LOTTYPE", InnerText = lotType });
            bodyEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "PORTID", InnerText = portId });
            MyXMLEntity Body = new MyXMLEntity();
            foreach (var a in bodys)
            {
                MyXMLEntity sonBody = new MyXMLEntity();
                sonBody.NodeName = "Father";
                sonBody.InnerText = a.Key;
                foreach (var c in a.Value)
                {
                    MyXMLEntity sonnode = new MyXMLEntity();
                    if (!c.Key.Trim().Equals(""))
                    {
                        sonnode.NodeName = "Son";
                    }
                    else
                    {
                        sonnode.NodeName = "null";
                    }
                    if (!c.Value.Trim().Equals(""))
                    {
                        sonnode.InnerText = c.Key.Trim().Replace(" ", "").ToString() + "&" + c.Value.Trim();
                    }

                    sonBody.SubNodes.Add(sonnode);
                }
                Body.SubNodes.Add(sonBody);
            }
            Body.NodeName = "Sum";
            MyXMLEntity recipebody = new MyXMLEntity();
            recipebody.NodeName = "RECIPEBODY";
            recipebody.SubNodes.Add(Body);
            bodyEntity.SubNodes.Add(recipebody);
            bodyEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "RECIPEFORMAT", InnerText = recipeFormat });
            entity.SubNodes.Add(bodyEntity);

            return Utill.EntityToXmlString(entity);
        }
        public static string BuildNeedBeCheckParameterListRequestXML(string transactionId, string eqpId, string productId, string lotType, string recipeId)
        {
            MyXMLEntity entity = new MyXMLEntity() { NodeName = "root" };

            MyXMLEntity headerEntity = new MyXMLEntity() { NodeName = "Header" };
            headerEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "MESSAGENAME", InnerText = "NEEDBECHECKPARAMETERLISTREQUEST" });
            headerEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "TRANSACTIONID", InnerText = transactionId });
            entity.SubNodes.Add(headerEntity);

            MyXMLEntity bodyEntity = new MyXMLEntity() { NodeName = "Body" };
            bodyEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "EQPID", InnerText = eqpId });
            bodyEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "PRODUCTID", InnerText = productId });
            bodyEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "LOTTYPE", InnerText = lotType });
            bodyEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "RECIPEID", InnerText = recipeId });
            entity.SubNodes.Add(bodyEntity);

            return Utill.EntityToXmlString(entity);
        }
        public static string BuildParameterValueReplyXML(string userId, string transactionId, string EqpId, Dictionary<string, string> EcValue, Dictionary<string, string> SvValue)
        {
            MyXMLEntity entity = new MyXMLEntity() { NodeName = "root" };

            MyXMLEntity headerEntity = new MyXMLEntity() { NodeName = "Header" };
            headerEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "MESSAGENAME", InnerText = "PARAMETERVALUEREPLY" });
            headerEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "TRANSACTIONID", InnerText = transactionId });
            entity.SubNodes.Add(headerEntity);

            MyXMLEntity bodyEntity = new MyXMLEntity() { NodeName = "Body" };
            bodyEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "EQPID", InnerText = EqpId });

            MyXMLEntity paramEntity = new MyXMLEntity() { NodeName = "PAMETERLIST" };
            MyXMLEntity ecEntity = new MyXMLEntity() { NodeName = "ECLIST" };
            foreach (KeyValuePair<string, string> key in EcValue)
            {
                MyXMLEntity param = new MyXMLEntity() { NodeName = "PARAMETER" };
                MyXMLEntity id = new MyXMLEntity() { NodeName = "ID", InnerText = key.Key };
                MyXMLEntity value = new MyXMLEntity() { NodeName = "VALUE", InnerText = key.Value };
                param.SubNodes.Add(id);
                param.SubNodes.Add(value);
                ecEntity.SubNodes.Add(param);
            }
            paramEntity.SubNodes.Add(ecEntity);

            MyXMLEntity svEntity = new MyXMLEntity() { NodeName = "SVLIST" };
            foreach (KeyValuePair<string, string> key in SvValue)
            {
                MyXMLEntity param = new MyXMLEntity() { NodeName = "PARAMETER" };
                MyXMLEntity id = new MyXMLEntity() { NodeName = "ID", InnerText = key.Key };
                MyXMLEntity value = new MyXMLEntity() { NodeName = "VALUE", InnerText = key.Value };
                param.SubNodes.Add(id);
                param.SubNodes.Add(value);
                svEntity.SubNodes.Add(param);
            }
            paramEntity.SubNodes.Add(svEntity);
            bodyEntity.SubNodes.Add(paramEntity);
            entity.SubNodes.Add(bodyEntity);
            return Utill.EntityToXmlString(entity);
        }
        public static string BuildParamererCheckRequestXML(string userId, string transactionId, string EqpId, string productId, string lotType, string recipeId, Dictionary<string, string> EcValue, Dictionary<string, string> SvValue)
        {
            MyXMLEntity entity = new MyXMLEntity() { NodeName = "root" };

            MyXMLEntity headerEntity = new MyXMLEntity() { NodeName = "Header" };
            headerEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "MESSAGENAME", InnerText = "PARAMETERCHECKREQUEST" });
            headerEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "TRANSACTIONID", InnerText = transactionId });
            entity.SubNodes.Add(headerEntity);

            MyXMLEntity bodyEntity = new MyXMLEntity() { NodeName = "Body" };
            bodyEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "EQPID", InnerText = EqpId });
            bodyEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "PRODUCTID", InnerText = productId });
            bodyEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "LOTTYPE", InnerText = lotType });
            bodyEntity.SubNodes.Add(new MyXMLEntity() { NodeName = "RECIPEID", InnerText = recipeId });

            MyXMLEntity paramEntity = new MyXMLEntity() { NodeName = "PAMETERLIST" };
            MyXMLEntity ecEntity = new MyXMLEntity() { NodeName = "ECLIST" };
            foreach (KeyValuePair<string, string> key in EcValue)
            {
                MyXMLEntity param = new MyXMLEntity() { NodeName = "PARAMETER" };
                MyXMLEntity id = new MyXMLEntity() { NodeName = "ID", InnerText = key.Key };
                MyXMLEntity value = new MyXMLEntity() { NodeName = "VALUE", InnerText = key.Value };
                param.SubNodes.Add(id);
                param.SubNodes.Add(value);
                ecEntity.SubNodes.Add(param);
            }
            paramEntity.SubNodes.Add(ecEntity);
            MyXMLEntity svEntity = new MyXMLEntity() { NodeName = "SVLIST" };
            foreach (KeyValuePair<string, string> key in SvValue)
            {
                MyXMLEntity param = new MyXMLEntity() { NodeName = "PARAMETER" };
                MyXMLEntity id = new MyXMLEntity() { NodeName = "ID", InnerText = key.Key };
                MyXMLEntity value = new MyXMLEntity() { NodeName = "VALUE", InnerText = key.Value };
                param.SubNodes.Add(id);
                param.SubNodes.Add(value);
                svEntity.SubNodes.Add(param);
            }
            paramEntity.SubNodes.Add(svEntity);
            bodyEntity.SubNodes.Add(paramEntity);
            entity.SubNodes.Add(bodyEntity);
            return Utill.EntityToXmlString(entity);
        }
        public static string ConvertSecsTransToXML(IFormatCollection sItem, ref XmlDocument xmlDoc, ref XmlNode xmlNode)
        {
            try
            {
                if (xmlDoc == null)
                {
                    xmlDoc = new XmlDocument();
                    xmlNode = xmlDoc.CreateElement("SECSMessage");
                }
                if (sItem.Count > 0)
                {
                    for (int nIdx = 0; nIdx < sItem.Count; nIdx++)
                    {
                        if (sItem[nIdx] is ListFormat)
                        {
                            XmlNode xmlNodeTmp = xmlDoc.CreateNode(XmlNodeType.Element, ((MySecsDriver.Structure.Format)sItem[nIdx]).LogType, null);
                            ConvertSecsTransToXML(sItem[nIdx].Children, ref xmlDoc, ref xmlNodeTmp);
                            xmlNode.AppendChild(xmlNodeTmp);
                        }
                        else
                        {
                            if (sItem[nIdx].Value != null)
                            {
                                XmlNode xmlNodeTmp = xmlDoc.CreateNode(XmlNodeType.Element, ((MySecsDriver.Structure.Format)sItem[nIdx]).LogType, null);
                                xmlNodeTmp.InnerText = sItem[nIdx].Value.ToString();
                                xmlNode.AppendChild(xmlNodeTmp);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            return xmlNode.InnerXml.ToString();
        }
    }
}
