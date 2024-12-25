using MySecsDriver.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FA.Automation.MessageBus
{
    public static class SecsService
    {
        public static SECSTransaction SecsMessageFromText(string content)
        {
            SECSTransaction secsMessage = null;
            string error = string.Empty;
            try
            {
                secsMessage = stringToSECSTrx(content, ref error);
                if (!string.IsNullOrEmpty(error))
                {
                    throw new FormatException($"解析SecsMessage内容失败，{error}");
                }
                if (secsMessage == null)
                {
                    throw new FormatException("SecsMessage内容不是正确的格式");
                }
            }
            catch (FormatException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new FormatException("SecsMessage内容解析异常", ex);
            }
            return secsMessage;
        }

        public static string SecsMessageToXml(IFormatCollection sItem)
        {
            var xmlDoc = new XmlDocument();
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,            // 启用缩进
                IndentChars = "    ",     // 缩进字符，默认为4个空格
                NewLineChars = "\r\n",    // 换行符
                NewLineHandling = NewLineHandling.Replace // 替换换行符
            };
            var xmlNode = xmlDoc.CreateElement("SECSMessage");
            SecsMessageToXml(sItem, xmlDoc, xmlNode);
            return xmlNode.InnerXml.ToString();
        }

        private static void SecsMessageToXml(IFormatCollection sItem, XmlDocument xmlDoc, XmlNode xmlNode)
        {
            if (sItem.Count > 0)
            {
                for (int nIdx = 0; nIdx < sItem.Count; nIdx++)
                {
                    if (sItem[nIdx] is ListFormat)
                    {
                        XmlNode xmlNodeTmp = xmlDoc.CreateNode(
                            XmlNodeType.Element,
                            ((MySecsDriver.Structure.Format)sItem[nIdx]).LogType,
                            null
                        );
                        SecsMessageToXml(sItem[nIdx].Children, xmlDoc, xmlNodeTmp);
                        xmlNode.AppendChild(xmlNodeTmp);
                    }
                    else
                    {
                        if (sItem[nIdx].Value != null)
                        {
                            XmlNode xmlNodeTmp = xmlDoc.CreateNode(
                                XmlNodeType.Element,
                                ((MySecsDriver.Structure.Format)sItem[nIdx]).LogType,
                                null
                            );
                            xmlNodeTmp.InnerText = sItem[nIdx].Value.ToString();
                            xmlNode.AppendChild(xmlNodeTmp);
                        }
                    }
                }
            }
        }

        private static SECSTransaction stringToSECSTrx(string strSECSValue, ref string strRet)
        {
            SECSTransaction secsTrx = null;
            bool bWFlag = false, bResult = false;
            bool bSecondlevel = false, bThirdLevel = false, bFourthLevel = false,
                bFifthLevel = false, bSixthLevel = false, bSeventhLevel = false, bEighthLevel = false,
                bNinthLevel = false, b10Level = false, b11Level = false,
                b12Level = false, b13Level = false, b14Level = false, b15Level = false,
                b16Level = false, b17Level = false, b18Level = false, b19Level = false, b20Level = false,
                b21Level = false, b22Level = false, b23Level = false, b24Level = false, b25Level = false,
                b26Level = false, b27Level = false, b28Level = false, b29Level = false, b30Level = false;
            int iFlag = 0, iStream = 0, iFunction = 0;
            int iFirstLevel = -1, iSecondlevel = -1, iThirdLevel = -1, iFourthLevel = -1,
                iFifthLevel = -1, iSixthLevel = -1, iSeventhLevel = -1, iEighthLevel = -1,
                iNinthLevel = -1, i10Level = -1, i11Level = -1, i12Level = -1, i13Level = -1, i14Level = -1, i15Level = -1,
                i16Level = -1, i17Level = -1, i18Level = -1, i19Level = -1, i20Level = -1, i21Level = -1,
                i22Level = -1, i23Level = -1, i24Level = -1, i25Level = -1, i26Level = -1, i27Level = -1,
                i28Level = -1, i29Level = -1, i30Level = -1;

            if (strSECSValue.IndexOf("< \"") >= 0)
            {
                strRet = "The message has invalid data [< ]";
                return null;
            }
            if (strSECSValue.IndexOf("\" >") >= 0)
            {
                strRet = "The message has invalid data [\" >]";
                return null;
            }

            if (strSECSValue.IndexOf("' >") >= 0)
            {
                strRet = "The message has invalid data [' >]";
                return null;
            }
            //if (strSECSValue.IndexOf("\">") >= 0)
            //{
            //    strRet = "The message has invalid data [\">]";
            //    return null;
            //}
            if (strSECSValue.IndexOf("<Boolean") >= 0)
            {
                strRet = "The message has invalid data type [<Boolean]";
                return null;
            }
            if (strSECSValue.IndexOf("<boolean") >= 0)
            {
                strRet = "The message has invalid data type [<Boolean]";
                return null;
            }

            strSECSValue = strSECSValue.Trim('\n').Trim('\r').Trim('\r').Trim('\n');
            string strLineValue = "", strLevelPath = "", strDeviceID = "";
            string[] strLevels = null;
            string[] strItems = strSECSValue.Split(new string[] { "<" }, StringSplitOptions.RemoveEmptyEntries);
            for (int iIndex = 0; iIndex < strItems.Length; iIndex++)
            {
                strLineValue = strItems[iIndex];
                if (iIndex == 50)
                {
                    // string s = "";
                }
                if (iIndex == 0)
                {
                    string[] strSubItems = strItems[iIndex].Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    for (int iIndexTmp = 0; iIndexTmp < strSubItems.Length; iIndexTmp++)
                    {
                        if (strSubItems[iIndexTmp].Substring(0, 1) == "S")
                        {
                            iFlag = 1;
                            if (secsTrx == null)
                            {
                                bResult = GetStreamFunction(strSubItems[iIndexTmp], ref iStream, ref iFunction, ref bWFlag, ref strDeviceID);

                                if (bResult == false)
                                {
                                    strRet = "Get Stram Function fail from string[" + strSubItems[iIndexTmp] + "]";
                                    return null;
                                    ;
                                }
                                if (iStream == 0 || iFunction == 0)
                                {
                                    return null;
                                }
                                secsTrx = new SECSTransaction(iStream, iFunction, bWFlag);
                                if (strDeviceID.Length > 0)
                                    secsTrx.DeviceId = Convert.ToInt32(strDeviceID);
                                break;
                            }
                        }
                    }
                    continue;
                }

                if (iFlag == 0)
                {
                    strRet = "Message header is error.";
                    return null;
                }

                //第一个字符不是L
                if (iFirstLevel < 0 && strLineValue.Substring(0, 1) != "L")
                {
                    if (strItems.Length > 2)
                    {
                        strRet = "error format.If format of the first item is not L,and item count more than 2,so it is error format message. ";
                        return null;
                    }
                    bResult = AddItemToTrx(strLineValue, "", ref secsTrx, ref strRet);
                    if (bResult == true)
                        return secsTrx;
                    else
                        return null;
                }
                //第一个是L
                if (iFirstLevel < 0)
                {
                    bResult = AddItemToTrx("L", "", ref secsTrx, ref strRet);
                    if (bResult == true)
                    {
                        iFirstLevel++;
                        strLevelPath = iFirstLevel.ToString();
                        continue;
                    }
                    else
                        return null;
                }

                #region "List"

                if (strLineValue.Substring(0, 2).Trim() == "L")
                {
                    strLevels = strLevelPath.Split('~');
                    if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == true &&
                        bFifthLevel == true && bSixthLevel == true && bSeventhLevel == true &&
                        bEighthLevel == true && bNinthLevel == true && b10Level == true &&
                        b11Level == true && b12Level == true && b13Level == true && b14Level == true && b15Level == true &&
                        b16Level == true && b17Level == true && b18Level == true && b19Level == true && b20Level == true &&
                        b21Level == true && b22Level == true && b23Level == true && b24Level == true && b25Level == true &&
                        b26Level == true && b27Level == true && b28Level == true && b29Level == true && b30Level == false)
                    {
                        strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2] + "~" + strLevels[3] + "~" + strLevels[4] +
                            "~" + strLevels[5] + "~" + strLevels[6] + "~" + strLevels[7] + "~" + strLevels[8] + "~" + strLevels[9] +
                            "~" + strLevels[10] + "~" + strLevels[11] + "~" + strLevels[12] + "~" + strLevels[13] + "~" + strLevels[14] +
                            "~" + strLevels[15] + "~" + strLevels[16] + "~" + strLevels[17] + "~" + strLevels[18] + "~" + strLevels[19] +
                            "~" + strLevels[20] + "~" + strLevels[21] + "~" + strLevels[22] + "~" + strLevels[23] + "~" + strLevels[24] +
                            "~" + strLevels[25] + "~" + strLevels[26] + "~" + strLevels[27] + "~" + strLevels[28];
                        bResult = AddItemToTrx("L", strLevelPath, ref secsTrx, ref strRet);
                        if (bResult == true)
                        {
                            b30Level = true;
                            i30Level++;
                            strLevelPath = strLevelPath + "~" + i30Level.ToString();
                        }
                        else
                            continue;
                    }
                    if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == true &&
                        bFifthLevel == true && bSixthLevel == true && bSeventhLevel == true &&
                        bEighthLevel == true && bNinthLevel == true && b10Level == true &&
                        b11Level == true && b12Level == true && b13Level == true && b14Level == true && b15Level == true &&
                        b16Level == true && b17Level == true && b18Level == true && b19Level == true && b20Level == true &&
                        b21Level == true && b22Level == true && b23Level == true && b24Level == true && b25Level == true &&
                        b26Level == true && b27Level == true && b28Level == true && b29Level == false && b30Level == false)
                    {
                        strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2] + "~" + strLevels[3] + "~" + strLevels[4] +
                            "~" + strLevels[5] + "~" + strLevels[6] + "~" + strLevels[7] + "~" + strLevels[8] + "~" + strLevels[9] +
                            "~" + strLevels[10] + "~" + strLevels[11] + "~" + strLevels[12] + "~" + strLevels[13] + "~" + strLevels[14] +
                            "~" + strLevels[15] + "~" + strLevels[16] + "~" + strLevels[17] + "~" + strLevels[18] + "~" + strLevels[19] +
                            "~" + strLevels[20] + "~" + strLevels[21] + "~" + strLevels[22] + "~" + strLevels[23] + "~" + strLevels[24] +
                            "~" + strLevels[25] + "~" + strLevels[26] + "~" + strLevels[27];
                        bResult = AddItemToTrx("L", strLevelPath, ref secsTrx, ref strRet);
                        if (bResult == true)
                        {
                            b29Level = true;
                            i29Level++;
                            strLevelPath = strLevelPath + "~" + i29Level.ToString();
                        }
                        else
                            continue;
                    }
                    if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == true &&
                        bFifthLevel == true && bSixthLevel == true && bSeventhLevel == true &&
                        bEighthLevel == true && bNinthLevel == true && b10Level == true &&
                        b11Level == true && b12Level == true && b13Level == true && b14Level == true && b15Level == true &&
                        b16Level == true && b17Level == true && b18Level == true && b19Level == true && b20Level == true &&
                        b21Level == true && b22Level == true && b23Level == true && b24Level == true && b25Level == true &&
                        b26Level == true && b27Level == true && b28Level == false && b29Level == false && b30Level == false)
                    {
                        strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2] + "~" + strLevels[3] + "~" + strLevels[4] +
                            "~" + strLevels[5] + "~" + strLevels[6] + "~" + strLevels[7] + "~" + strLevels[8] + "~" + strLevels[9] +
                            "~" + strLevels[10] + "~" + strLevels[11] + "~" + strLevels[12] + "~" + strLevels[13] + "~" + strLevels[14] +
                            "~" + strLevels[15] + "~" + strLevels[16] + "~" + strLevels[17] + "~" + strLevels[18] + "~" + strLevels[19] +
                            "~" + strLevels[20] + "~" + strLevels[21] + "~" + strLevels[22] + "~" + strLevels[23] + "~" + strLevels[24] +
                            "~" + strLevels[25] + "~" + strLevels[26];
                        bResult = AddItemToTrx("L", strLevelPath, ref secsTrx, ref strRet);
                        if (bResult == true)
                        {
                            b28Level = true;
                            i28Level++;
                            strLevelPath = strLevelPath + "~" + i28Level.ToString();
                        }
                        else
                            continue;
                    }
                    if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == true &&
                        bFifthLevel == true && bSixthLevel == true && bSeventhLevel == true &&
                        bEighthLevel == true && bNinthLevel == true && b10Level == true &&
                        b11Level == true && b12Level == true && b13Level == true && b14Level == true && b15Level == true &&
                        b16Level == true && b17Level == true && b18Level == true && b19Level == true && b20Level == true &&
                        b21Level == true && b22Level == true && b23Level == true && b24Level == true && b25Level == true &&
                        b26Level == true && b27Level == false && b28Level == false && b29Level == false && b30Level == false)
                    {
                        strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2] + "~" + strLevels[3] + "~" + strLevels[4] +
                            "~" + strLevels[5] + "~" + strLevels[6] + "~" + strLevels[7] + "~" + strLevels[8] + "~" + strLevels[9] +
                            "~" + strLevels[10] + "~" + strLevels[11] + "~" + strLevels[12] + "~" + strLevels[13] + "~" + strLevels[14] +
                            "~" + strLevels[15] + "~" + strLevels[16] + "~" + strLevels[17] + "~" + strLevels[18] + "~" + strLevels[19] +
                            "~" + strLevels[20] + "~" + strLevels[21] + "~" + strLevels[22] + "~" + strLevels[23] + "~" + strLevels[24] +
                            "~" + strLevels[25];
                        bResult = AddItemToTrx("L", strLevelPath, ref secsTrx, ref strRet);
                        if (bResult == true)
                        {
                            b27Level = true;
                            i27Level++;
                            strLevelPath = strLevelPath + "~" + i27Level.ToString();
                        }
                        else
                            continue;
                    }
                    if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == true &&
                        bFifthLevel == true && bSixthLevel == true && bSeventhLevel == true &&
                        bEighthLevel == true && bNinthLevel == true && b10Level == true &&
                        b11Level == true && b12Level == true && b13Level == true && b14Level == true && b15Level == true &&
                        b16Level == true && b17Level == true && b18Level == true && b19Level == true && b20Level == true &&
                        b21Level == true && b22Level == true && b23Level == true && b24Level == true && b25Level == true &&
                        b26Level == false && b27Level == false && b28Level == false && b29Level == false && b30Level == false)
                    {
                        strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2] + "~" + strLevels[3] + "~" + strLevels[4] +
                            "~" + strLevels[5] + "~" + strLevels[6] + "~" + strLevels[7] + "~" + strLevels[8] + "~" + strLevels[9] +
                            "~" + strLevels[10] + "~" + strLevels[11] + "~" + strLevels[12] + "~" + strLevels[13] + "~" + strLevels[14] +
                            "~" + strLevels[15] + "~" + strLevels[16] + "~" + strLevels[17] + "~" + strLevels[18] + "~" + strLevels[19] +
                            "~" + strLevels[20] + "~" + strLevels[21] + "~" + strLevels[22] + "~" + strLevels[23] + "~" + strLevels[24];
                        bResult = AddItemToTrx("L", strLevelPath, ref secsTrx, ref strRet);
                        if (bResult == true)
                        {
                            b26Level = true;
                            i26Level++;
                            strLevelPath = strLevelPath + "~" + i26Level.ToString();
                        }
                        else
                            continue;
                    }
                    if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == true &&
                        bFifthLevel == true && bSixthLevel == true && bSeventhLevel == true &&
                        bEighthLevel == true && bNinthLevel == true && b10Level == true &&
                        b11Level == true && b12Level == true && b13Level == true && b14Level == true && b15Level == true &&
                        b16Level == true && b17Level == true && b18Level == true && b19Level == true && b20Level == true &&
                        b21Level == true && b22Level == true && b23Level == true && b24Level == true && b25Level == false &&
                        b26Level == false && b27Level == false && b28Level == false && b29Level == false && b30Level == false)
                    {
                        strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2] + "~" + strLevels[3] + "~" + strLevels[4] +
                            "~" + strLevels[5] + "~" + strLevels[6] + "~" + strLevels[7] + "~" + strLevels[8] + "~" + strLevels[9] +
                            "~" + strLevels[10] + "~" + strLevels[11] + "~" + strLevels[12] + "~" + strLevels[13] + "~" + strLevels[14] +
                            "~" + strLevels[15] + "~" + strLevels[16] + "~" + strLevels[17] + "~" + strLevels[18] + "~" + strLevels[19] +
                            "~" + strLevels[20] + "~" + strLevels[21] + "~" + strLevels[22] + "~" + strLevels[23];
                        bResult = AddItemToTrx("L", strLevelPath, ref secsTrx, ref strRet);
                        if (bResult == true)
                        {
                            b25Level = true;
                            i25Level++;
                            strLevelPath = strLevelPath + "~" + i25Level.ToString();
                        }
                        else
                            continue;
                    }
                    if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == true &&
                        bFifthLevel == true && bSixthLevel == true && bSeventhLevel == true &&
                        bEighthLevel == true && bNinthLevel == true && b10Level == true &&
                        b11Level == true && b12Level == true && b13Level == true && b14Level == true && b15Level == true &&
                        b16Level == true && b17Level == true && b18Level == true && b19Level == true && b20Level == true &&
                        b21Level == true && b22Level == true && b23Level == true && b24Level == true && b25Level == false &&
                        b26Level == false && b27Level == false && b28Level == false && b29Level == false && b30Level == false)
                    {
                        strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2] + "~" + strLevels[3] + "~" + strLevels[4] +
                            "~" + strLevels[5] + "~" + strLevels[6] + "~" + strLevels[7] + "~" + strLevels[8] + "~" + strLevels[9] +
                            "~" + strLevels[10] + "~" + strLevels[11] + "~" + strLevels[12] + "~" + strLevels[13] + "~" + strLevels[14] +
                            "~" + strLevels[15] + "~" + strLevels[16] + "~" + strLevels[17] + "~" + strLevels[18] + "~" + strLevels[19] +
                            "~" + strLevels[20] + "~" + strLevels[21] + "~" + strLevels[22] + "~" + strLevels[23];
                        bResult = AddItemToTrx("L", strLevelPath, ref secsTrx, ref strRet);
                        if (bResult == true)
                        {
                            b25Level = true;
                            i25Level++;
                            strLevelPath = strLevelPath + "~" + i25Level.ToString();
                        }
                        else
                            continue;
                    }
                    if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == true &&
                        bFifthLevel == true && bSixthLevel == true && bSeventhLevel == true &&
                        bEighthLevel == true && bNinthLevel == true && b10Level == true &&
                        b11Level == true && b12Level == true && b13Level == true && b14Level == true && b15Level == true &&
                        b16Level == true && b17Level == true && b18Level == true && b19Level == true && b20Level == true &&
                        b21Level == true && b22Level == true && b23Level == true && b24Level == false && b25Level == false &&
                        b26Level == false && b27Level == false && b28Level == false && b29Level == false && b30Level == false)
                    {
                        strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2] + "~" + strLevels[3] + "~" + strLevels[4] +
                            "~" + strLevels[5] + "~" + strLevels[6] + "~" + strLevels[7] + "~" + strLevels[8] + "~" + strLevels[9] +
                            "~" + strLevels[10] + "~" + strLevels[11] + "~" + strLevels[12] + "~" + strLevels[13] + "~" + strLevels[14] +
                            "~" + strLevels[15] + "~" + strLevels[16] + "~" + strLevels[17] + "~" + strLevels[18] + "~" + strLevels[19] +
                            "~" + strLevels[20] + "~" + strLevels[21] + "~" + strLevels[22];
                        bResult = AddItemToTrx("L", strLevelPath, ref secsTrx, ref strRet);
                        if (bResult == true)
                        {
                            b24Level = true;
                            i24Level++;
                            strLevelPath = strLevelPath + "~" + i24Level.ToString();
                        }
                        else
                            continue;
                    }
                    if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == true &&
                        bFifthLevel == true && bSixthLevel == true && bSeventhLevel == true &&
                        bEighthLevel == true && bNinthLevel == true && b10Level == true &&
                        b11Level == true && b12Level == true && b13Level == true && b14Level == true && b15Level == true &&
                        b16Level == true && b17Level == true && b18Level == true && b19Level == true && b20Level == true &&
                        b21Level == true && b22Level == true && b23Level == false && b24Level == false && b25Level == false &&
                        b26Level == false && b27Level == false && b28Level == false && b29Level == false && b30Level == false)
                    {
                        strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2] + "~" + strLevels[3] + "~" + strLevels[4] +
                            "~" + strLevels[5] + "~" + strLevels[6] + "~" + strLevels[7] + "~" + strLevels[8] + "~" + strLevels[9] +
                            "~" + strLevels[10] + "~" + strLevels[11] + "~" + strLevels[12] + "~" + strLevels[13] + "~" + strLevels[14] +
                            "~" + strLevels[15] + "~" + strLevels[16] + "~" + strLevels[17] + "~" + strLevels[18] + "~" + strLevels[19] +
                            "~" + strLevels[20] + "~" + strLevels[21];
                        bResult = AddItemToTrx("L", strLevelPath, ref secsTrx, ref strRet);
                        if (bResult == true)
                        {
                            b23Level = true;
                            i23Level++;
                            strLevelPath = strLevelPath + "~" + i23Level.ToString();
                        }
                        else
                            continue;
                    }
                    if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == true &&
                        bFifthLevel == true && bSixthLevel == true && bSeventhLevel == true &&
                        bEighthLevel == true && bNinthLevel == true && b10Level == true &&
                        b11Level == true && b12Level == true && b13Level == true && b14Level == true && b15Level == true &&
                        b16Level == true && b17Level == true && b18Level == true && b19Level == true && b20Level == true &&
                        b21Level == true && b22Level == false && b23Level == false && b24Level == false && b25Level == false &&
                        b26Level == false && b27Level == false && b28Level == false && b29Level == false && b30Level == false)
                    {
                        strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2] + "~" + strLevels[3] + "~" + strLevels[4] +
                            "~" + strLevels[5] + "~" + strLevels[6] + "~" + strLevels[7] + "~" + strLevels[8] + "~" + strLevels[9] +
                            "~" + strLevels[10] + "~" + strLevels[11] + "~" + strLevels[12] + "~" + strLevels[13] + "~" + strLevels[14] +
                            "~" + strLevels[15] + "~" + strLevels[16] + "~" + strLevels[17] + "~" + strLevels[18] + "~" + strLevels[19] +
                            "~" + strLevels[20];
                        bResult = AddItemToTrx("L", strLevelPath, ref secsTrx, ref strRet);
                        if (bResult == true)
                        {
                            b22Level = true;
                            i22Level++;
                            strLevelPath = strLevelPath + "~" + i22Level.ToString();
                        }
                        else
                            continue;
                    }
                    if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == true &&
                        bFifthLevel == true && bSixthLevel == true && bSeventhLevel == true &&
                        bEighthLevel == true && bNinthLevel == true && b10Level == true &&
                        b11Level == true && b12Level == true && b13Level == true && b14Level == true && b15Level == true &&
                        b16Level == true && b17Level == true && b18Level == true && b19Level == true && b20Level == true &&
                        b21Level == false && b22Level == false && b23Level == false && b24Level == false && b25Level == false &&
                        b26Level == false && b27Level == false && b28Level == false && b29Level == false && b30Level == false)
                    {
                        strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2] + "~" + strLevels[3] + "~" + strLevels[4] +
                            "~" + strLevels[5] + "~" + strLevels[6] + "~" + strLevels[7] + "~" + strLevels[8] + "~" + strLevels[9] +
                            "~" + strLevels[10] + "~" + strLevels[11] + "~" + strLevels[12] + "~" + strLevels[13] + "~" + strLevels[14] +
                            "~" + strLevels[15] + "~" + strLevels[16] + "~" + strLevels[17] + "~" + strLevels[18] + "~" + strLevels[19];
                        bResult = AddItemToTrx("L", strLevelPath, ref secsTrx, ref strRet);
                        if (bResult == true)
                        {
                            b21Level = true;
                            i21Level++;
                            strLevelPath = strLevelPath + "~" + i21Level.ToString();
                        }
                        else
                            continue;
                    }
                    if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == true &&
                        bFifthLevel == true && bSixthLevel == true && bSeventhLevel == true &&
                        bEighthLevel == true && bNinthLevel == true && b10Level == true &&
                        b11Level == true && b12Level == true && b13Level == true && b14Level == true && b15Level == true &&
                        b16Level == true && b17Level == true && b18Level == true && b19Level == true && b20Level == false &&
                        b21Level == false && b22Level == false && b23Level == false && b24Level == false && b25Level == false &&
                        b26Level == false && b27Level == false && b28Level == false && b29Level == false && b30Level == false)
                    {
                        strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2] + "~" + strLevels[3] + "~" + strLevels[4] +
                            "~" + strLevels[5] + "~" + strLevels[6] + "~" + strLevels[7] + "~" + strLevels[8] + "~" + strLevels[9] +
                            "~" + strLevels[10] + "~" + strLevels[11] + "~" + strLevels[12] + "~" + strLevels[13] + "~" + strLevels[14] +
                            "~" + strLevels[15] + "~" + strLevels[16] + "~" + strLevels[17] + "~" + strLevels[18];
                        bResult = AddItemToTrx("L", strLevelPath, ref secsTrx, ref strRet);
                        if (bResult == true)
                        {
                            b20Level = true;
                            i20Level++;
                            strLevelPath = strLevelPath + "~" + i20Level.ToString();
                        }
                        else
                            continue;
                    }
                    if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == true &&
                        bFifthLevel == true && bSixthLevel == true && bSeventhLevel == true &&
                        bEighthLevel == true && bNinthLevel == true && b10Level == true &&
                        b11Level == true && b12Level == true && b13Level == true && b14Level == true && b15Level == true &&
                        b16Level == true && b17Level == true && b18Level == true && b19Level == false && b20Level == false &&
                        b21Level == false && b22Level == false && b23Level == false && b24Level == false && b25Level == false &&
                        b26Level == false && b27Level == false && b28Level == false && b29Level == false && b30Level == false)
                    {
                        strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2] + "~" + strLevels[3] + "~" + strLevels[4] +
                            "~" + strLevels[5] + "~" + strLevels[6] + "~" + strLevels[7] + "~" + strLevels[8] + "~" + strLevels[9] +
                            "~" + strLevels[10] + "~" + strLevels[11] + "~" + strLevels[12] + "~" + strLevels[13] + "~" + strLevels[14] +
                            "~" + strLevels[15] + "~" + strLevels[16] + "~" + strLevels[17];
                        bResult = AddItemToTrx("L", strLevelPath, ref secsTrx, ref strRet);
                        if (bResult == true)
                        {
                            b19Level = true;
                            i19Level++;
                            strLevelPath = strLevelPath + "~" + i19Level.ToString();
                        }
                        else
                            continue;
                    }
                    if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == true &&
                        bFifthLevel == true && bSixthLevel == true && bSeventhLevel == true &&
                        bEighthLevel == true && bNinthLevel == true && b10Level == true &&
                        b11Level == true && b12Level == true && b13Level == true && b14Level == true && b15Level == true &&
                        b16Level == true && b17Level == true && b18Level == false && b19Level == false && b20Level == false &&
                        b21Level == false && b22Level == false && b23Level == false && b24Level == false && b25Level == false &&
                        b26Level == false && b27Level == false && b28Level == false && b29Level == false && b30Level == false)
                    {
                        strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2] + "~" + strLevels[3] + "~" + strLevels[4] +
                            "~" + strLevels[5] + "~" + strLevels[6] + "~" + strLevels[7] + "~" + strLevels[8] + "~" + strLevels[9] +
                            "~" + strLevels[10] + "~" + strLevels[11] + "~" + strLevels[12] + "~" + strLevels[13] + "~" + strLevels[14] +
                            "~" + strLevels[15] + "~" + strLevels[16];
                        bResult = AddItemToTrx("L", strLevelPath, ref secsTrx, ref strRet);
                        if (bResult == true)
                        {
                            b18Level = true;
                            i18Level++;
                            strLevelPath = strLevelPath + "~" + i18Level.ToString();
                        }
                        else
                            continue;
                    }
                    if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == true &&
                        bFifthLevel == true && bSixthLevel == true && bSeventhLevel == true &&
                        bEighthLevel == true && bNinthLevel == true && b10Level == true &&
                        b11Level == true && b12Level == true && b13Level == true && b14Level == true && b15Level == true &&
                        b16Level == true && b17Level == false && b18Level == false && b19Level == false && b20Level == false &&
                        b21Level == false && b22Level == false && b23Level == false && b24Level == false && b25Level == false &&
                        b26Level == false && b27Level == false && b28Level == false && b29Level == false && b30Level == false)
                    {
                        strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2] + "~" + strLevels[3] + "~" + strLevels[4] +
                            "~" + strLevels[5] + "~" + strLevels[6] + "~" + strLevels[7] + "~" + strLevels[8] + "~" + strLevels[9] +
                            "~" + strLevels[10] + "~" + strLevels[11] + "~" + strLevels[12] + "~" + strLevels[13] + "~" + strLevels[14] +
                            "~" + strLevels[15];
                        bResult = AddItemToTrx("L", strLevelPath, ref secsTrx, ref strRet);
                        if (bResult == true)
                        {
                            b17Level = true;
                            i17Level++;
                            strLevelPath = strLevelPath + "~" + i17Level.ToString();
                        }
                        else
                            continue;
                    }
                    if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == true &&
                        bFifthLevel == true && bSixthLevel == true && bSeventhLevel == true &&
                        bEighthLevel == true && bNinthLevel == true && b10Level == true &&
                        b11Level == true && b12Level == true && b13Level == true && b14Level == true && b15Level == true &&
                        b16Level == false && b17Level == false && b18Level == false && b19Level == false && b20Level == false &&
                        b21Level == false && b22Level == false && b23Level == false && b24Level == false && b25Level == false &&
                        b26Level == false && b27Level == false && b28Level == false && b29Level == false && b30Level == false)
                    {
                        strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2] + "~" + strLevels[3] + "~" + strLevels[4] +
                            "~" + strLevels[5] + "~" + strLevels[6] + "~" + strLevels[7] + "~" + strLevels[8] + "~" + strLevels[9] +
                            "~" + strLevels[10] + "~" + strLevels[11] + "~" + strLevels[12] + "~" + strLevels[13] + "~" + strLevels[14];
                        bResult = AddItemToTrx("L", strLevelPath, ref secsTrx, ref strRet);
                        if (bResult == true)
                        {
                            b16Level = true;
                            i16Level++;
                            strLevelPath = strLevelPath + "~" + i16Level.ToString();
                        }
                        else
                            continue;
                    }
                    if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == true &&
                        bFifthLevel == true && bSixthLevel == true && bSeventhLevel == true &&
                        bEighthLevel == true && bNinthLevel == true && b10Level == true &&
                        b11Level == true && b12Level == true && b13Level == true && b14Level == true && b15Level == false &&
                        b16Level == false && b17Level == false && b18Level == false && b19Level == false && b20Level == false &&
                        b21Level == false && b22Level == false && b23Level == false && b24Level == false && b25Level == false &&
                        b26Level == false && b27Level == false && b28Level == false && b29Level == false && b30Level == false)
                    {
                        strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2] + "~" + strLevels[3] + "~" + strLevels[4] +
                            "~" + strLevels[5] + "~" + strLevels[6] + "~" + strLevels[7] + "~" + strLevels[8] + "~" + strLevels[9] +
                            "~" + strLevels[10] + "~" + strLevels[11] + "~" + strLevels[12] + "~" + strLevels[13];
                        bResult = AddItemToTrx("L", strLevelPath, ref secsTrx, ref strRet);
                        if (bResult == true)
                        {
                            b15Level = true;
                            i15Level++;
                            strLevelPath = strLevelPath + "~" + i15Level.ToString();
                        }
                        else
                            continue;
                    }
                    if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == true &&
                        bFifthLevel == true && bSixthLevel == true && bSeventhLevel == true &&
                        bEighthLevel == true && bNinthLevel == true && b10Level == true &&
                        b11Level == true && b12Level == true && b13Level == true && b14Level == false && b15Level == false &&
                        b16Level == false && b17Level == false && b18Level == false && b19Level == false && b20Level == false &&
                        b21Level == false && b22Level == false && b23Level == false && b24Level == false && b25Level == false &&
                        b26Level == false && b27Level == false && b28Level == false && b29Level == false && b30Level == false)
                    {
                        strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2] + "~" + strLevels[3] + "~" + strLevels[4] +
                            "~" + strLevels[5] + "~" + strLevels[6] + "~" + strLevels[7] + "~" + strLevels[8] + "~" + strLevels[9] +
                            "~" + strLevels[10] + "~" + strLevels[11] + "~" + strLevels[12];
                        bResult = AddItemToTrx("L", strLevelPath, ref secsTrx, ref strRet);
                        if (bResult == true)
                        {
                            b14Level = true;
                            i14Level++;
                            strLevelPath = strLevelPath + "~" + i14Level.ToString();
                        }
                        else
                            continue;
                    }
                    if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == true &&
                        bFifthLevel == true && bSixthLevel == true && bSeventhLevel == true &&
                        bEighthLevel == true && bNinthLevel == true && b10Level == true &&
                        b11Level == true && b12Level == true && b13Level == false && b14Level == false && b15Level == false &&
                        b16Level == false && b17Level == false && b18Level == false && b19Level == false && b20Level == false &&
                        b21Level == false && b22Level == false && b23Level == false && b24Level == false && b25Level == false &&
                        b26Level == false && b27Level == false && b28Level == false && b29Level == false && b30Level == false)
                    {
                        strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2] + "~" + strLevels[3] + "~" + strLevels[4] +
                            "~" + strLevels[5] + "~" + strLevels[6] + "~" + strLevels[7] + "~" + strLevels[8] + "~" + strLevels[9] +
                            "~" + strLevels[10] + "~" + strLevels[11];
                        bResult = AddItemToTrx("L", strLevelPath, ref secsTrx, ref strRet);
                        if (bResult == true)
                        {
                            b13Level = true;
                            i13Level++;
                            strLevelPath = strLevelPath + "~" + i13Level.ToString();
                        }
                        else
                            continue;
                    }
                    if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == true &&
                        bFifthLevel == true && bSixthLevel == true && bSeventhLevel == true &&
                        bEighthLevel == true && bNinthLevel == true && b10Level == true &&
                        b11Level == true && b12Level == false && b13Level == false && b14Level == false && b15Level == false &&
                        b16Level == false && b17Level == false && b18Level == false && b19Level == false && b20Level == false &&
                        b21Level == false && b22Level == false && b23Level == false && b24Level == false && b25Level == false &&
                        b26Level == false && b27Level == false && b28Level == false && b29Level == false && b30Level == false)
                    {
                        strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2] + "~" + strLevels[3] + "~" + strLevels[4] +
                            "~" + strLevels[5] + "~" + strLevels[6] + "~" + strLevels[7] + "~" + strLevels[8] + "~" + strLevels[9] +
                            "~" + strLevels[10];
                        bResult = AddItemToTrx("L", strLevelPath, ref secsTrx, ref strRet);
                        if (bResult == true)
                        {
                            b12Level = true;
                            i12Level++;
                            strLevelPath = strLevelPath + "~" + i12Level.ToString();
                        }
                        else
                            continue;
                    }
                    if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == true &&
                        bFifthLevel == true && bSixthLevel == true && bSeventhLevel == true &&
                        bEighthLevel == true && bNinthLevel == true && b10Level == true &&
                        b11Level == false && b12Level == false && b13Level == false && b14Level == false && b15Level == false &&
                        b16Level == false && b17Level == false && b18Level == false && b19Level == false && b20Level == false &&
                        b21Level == false && b22Level == false && b23Level == false && b24Level == false && b25Level == false &&
                        b26Level == false && b27Level == false && b28Level == false && b29Level == false && b30Level == false)
                    {
                        strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2] + "~" + strLevels[3] + "~" + strLevels[4] +
                            "~" + strLevels[5] + "~" + strLevels[6] + "~" + strLevels[7] + "~" + strLevels[8] + "~" + strLevels[9];
                        bResult = AddItemToTrx("L", strLevelPath, ref secsTrx, ref strRet);
                        if (bResult == true)
                        {
                            b11Level = true;
                            i11Level++;
                            strLevelPath = strLevelPath + "~" + i11Level.ToString();
                        }
                        else
                            continue;
                    }
                    if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == true &&
                        bFifthLevel == true && bSixthLevel == true && bSeventhLevel == true &&
                        bEighthLevel == true && bNinthLevel == true && b10Level == false &&
                        b11Level == false && b12Level == false && b13Level == false && b14Level == false && b15Level == false &&
                        b16Level == false && b17Level == false && b18Level == false && b19Level == false && b20Level == false &&
                        b21Level == false && b22Level == false && b23Level == false && b24Level == false && b25Level == false &&
                        b26Level == false && b27Level == false && b28Level == false && b29Level == false && b30Level == false)
                    {
                        strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2] + "~" + strLevels[3] + "~" + strLevels[4] +
                            "~" + strLevels[5] + "~" + strLevels[6] + "~" + strLevels[7] + "~" + strLevels[8];
                        bResult = AddItemToTrx("L", strLevelPath, ref secsTrx, ref strRet);
                        if (bResult == true)
                        {
                            b10Level = true;
                            i10Level++;
                            strLevelPath = strLevelPath + "~" + i10Level.ToString();
                        }
                        else
                            continue;
                    }
                    if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == true &&
                        bFifthLevel == true && bSixthLevel == true && bSeventhLevel == true &&
                        bEighthLevel == true && bNinthLevel == false && b10Level == false &&
                        b11Level == false && b12Level == false && b13Level == false && b14Level == false && b15Level == false &&
                        b16Level == false && b17Level == false && b18Level == false && b19Level == false && b20Level == false &&
                        b21Level == false && b22Level == false && b23Level == false && b24Level == false && b25Level == false &&
                        b26Level == false && b27Level == false && b28Level == false && b29Level == false && b30Level == false)
                    {
                        strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2] + "~" + strLevels[3] + "~" + strLevels[4] +
                            "~" + strLevels[5] + "~" + strLevels[6] + "~" + strLevels[7];
                        bResult = AddItemToTrx("L", strLevelPath, ref secsTrx, ref strRet);
                        if (bResult == true)
                        {
                            bNinthLevel = true;
                            iNinthLevel++;
                            strLevelPath = strLevelPath + "~" + iNinthLevel.ToString();
                        }
                        else
                            continue;
                    }
                    if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == true &&
                        bFifthLevel == true && bSixthLevel == true && bSeventhLevel == true &&
                        bEighthLevel == false && bNinthLevel == false && b10Level == false &&
                        b11Level == false && b12Level == false && b13Level == false && b14Level == false && b15Level == false &&
                        b16Level == false && b17Level == false && b18Level == false && b19Level == false && b20Level == false &&
                        b21Level == false && b22Level == false && b23Level == false && b24Level == false && b25Level == false &&
                        b26Level == false && b27Level == false && b28Level == false && b29Level == false && b30Level == false)
                    {
                        strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2]
                            + "~" + strLevels[3] + "~" + strLevels[4] + "~" + strLevels[5]
                            + "~" + strLevels[6];
                        bResult = AddItemToTrx("L", strLevelPath, ref secsTrx, ref strRet);
                        if (bResult == true)
                        {
                            bEighthLevel = true;
                            iEighthLevel++;
                            strLevelPath = strLevelPath + "~" + iEighthLevel.ToString();
                        }
                        else
                            continue;
                    }
                    if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == true &&
                        bFifthLevel == true && bSixthLevel == true && bSeventhLevel == false &&
                        bEighthLevel == false && bNinthLevel == false && b10Level == false &&
                        b11Level == false && b12Level == false && b13Level == false && b14Level == false && b15Level == false &&
                        b16Level == false && b17Level == false && b18Level == false && b19Level == false && b20Level == false &&
                        b21Level == false && b22Level == false && b23Level == false && b24Level == false && b25Level == false &&
                        b26Level == false && b27Level == false && b28Level == false && b29Level == false && b30Level == false)
                    {
                        strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2]
                            + "~" + strLevels[3] + "~" + strLevels[4] + "~" + strLevels[5];
                        bResult = AddItemToTrx("L", strLevelPath, ref secsTrx, ref strRet);
                        if (bResult == true)
                        {
                            bSeventhLevel = true;
                            iSeventhLevel++;
                            strLevelPath = strLevelPath + "~" + iSeventhLevel.ToString();
                        }
                        else
                            continue;
                    }
                    if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == true &&
                        bFifthLevel == true && bSixthLevel == false && bSeventhLevel == false &&
                        bEighthLevel == false && bNinthLevel == false && b10Level == false &&
                        b11Level == false && b12Level == false && b13Level == false && b14Level == false && b15Level == false &&
                        b16Level == false && b17Level == false && b18Level == false && b19Level == false && b20Level == false &&
                        b21Level == false && b22Level == false && b23Level == false && b24Level == false && b25Level == false &&
                        b26Level == false && b27Level == false && b28Level == false && b29Level == false && b30Level == false)
                    {
                        strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2]
                            + "~" + strLevels[3] + "~" + strLevels[4];
                        bResult = AddItemToTrx("L", strLevelPath, ref secsTrx, ref strRet);
                        if (bResult == true)
                        {
                            bSixthLevel = true;
                            iSixthLevel++;
                            strLevelPath = strLevelPath + "~" + iSixthLevel.ToString();
                        }
                        else
                            continue;
                    }
                    if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == true &&
                        bFifthLevel == false && bSixthLevel == false && bSeventhLevel == false &&
                        bEighthLevel == false && bNinthLevel == false && b10Level == false &&
                        b11Level == false && b12Level == false && b13Level == false && b14Level == false && b15Level == false &&
                        b16Level == false && b17Level == false && b18Level == false && b19Level == false && b20Level == false &&
                        b21Level == false && b22Level == false && b23Level == false && b24Level == false && b25Level == false &&
                        b26Level == false && b27Level == false && b28Level == false && b29Level == false && b30Level == false)
                    {
                        strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2]
                            + "~" + strLevels[3];
                        bResult = AddItemToTrx("L", strLevelPath, ref secsTrx, ref strRet);
                        if (bResult == true)
                        {
                            bFifthLevel = true;
                            iFifthLevel++;
                            strLevelPath = strLevelPath + "~" + iFifthLevel.ToString();
                        }
                        else
                            continue;
                    }
                    if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == false &&
                        bFifthLevel == false && bSixthLevel == false && bSeventhLevel == false &&
                        bEighthLevel == false && bNinthLevel == false && b10Level == false &&
                        b11Level == false && b12Level == false && b13Level == false && b14Level == false && b15Level == false &&
                        b16Level == false && b17Level == false && b18Level == false && b19Level == false && b20Level == false &&
                        b21Level == false && b22Level == false && b23Level == false && b24Level == false && b25Level == false &&
                        b26Level == false && b27Level == false && b28Level == false && b29Level == false && b30Level == false)
                    {
                        strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2];
                        bResult = AddItemToTrx("L", strLevelPath, ref secsTrx, ref strRet);
                        if (bResult == true)
                        {
                            bFourthLevel = true;
                            iFourthLevel++;
                            strLevelPath = strLevelPath + "~" + iFourthLevel.ToString();
                        }
                        else
                            continue;
                    }
                    if (bSecondlevel == true && bThirdLevel == false && bFourthLevel == false &&
                        bFifthLevel == false && bSixthLevel == false && bSeventhLevel == false &&
                        bEighthLevel == false && bNinthLevel == false && b10Level == false &&
                        b11Level == false && b12Level == false && b13Level == false && b14Level == false && b15Level == false &&
                        b16Level == false && b17Level == false && b18Level == false && b19Level == false && b20Level == false &&
                        b21Level == false && b22Level == false && b23Level == false && b24Level == false && b25Level == false &&
                        b26Level == false && b27Level == false && b28Level == false && b29Level == false && b30Level == false)
                    {
                        strLevelPath = strLevels[0] + "~" + strLevels[1];
                        bResult = AddItemToTrx("L", strLevelPath, ref secsTrx, ref strRet);
                        if (bResult == true)
                        {
                            bThirdLevel = true;
                            iThirdLevel++;
                            strLevelPath = strLevelPath + "~" + iThirdLevel.ToString();
                        }
                        else
                            continue;
                    }
                    if (bSecondlevel == false && bThirdLevel == false && bFourthLevel == false &&
                        bFifthLevel == false && bSixthLevel == false && bSeventhLevel == false &&
                        bEighthLevel == false && bNinthLevel == false && b10Level == false &&
                        b11Level == false && b12Level == false && b13Level == false && b14Level == false && b15Level == false &&
                        b16Level == false && b17Level == false && b18Level == false && b19Level == false && b20Level == false &&
                        b21Level == false && b22Level == false && b23Level == false && b24Level == false && b25Level == false &&
                        b26Level == false && b27Level == false && b28Level == false && b29Level == false && b30Level == false)
                    {
                        strLevelPath = strLevels[0];
                        bResult = AddItemToTrx("L", strLevels[0], ref secsTrx, ref strRet);
                        if (bResult == true)
                        {
                            bSecondlevel = true;
                            iSecondlevel++;
                            strLevelPath = strLevels[0] + "~" + iSecondlevel.ToString();
                        }
                        else
                            continue;
                    }

                    //检查是否LIST的结束符号
                    bool isListFlag = false;
                    int iEndCharCount1 = GetListEndChar(strLineValue, ref isListFlag);
                    if (iEndCharCount1 == -100)
                        return secsTrx;
                    if (iEndCharCount1 == 0)
                        continue;

                    #region "GetListEndChar Result"

                    if (iEndCharCount1 > 0)
                    {
                        for (int iEndChrIndex = 1; iEndChrIndex <= iEndCharCount1; iEndChrIndex++)
                        {
                            if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == true &&
                                bFifthLevel == true && bSixthLevel == true && bSeventhLevel == true &&
                                bEighthLevel == true && bNinthLevel == true && b10Level == true &&
                                b11Level == true && b12Level == true && b13Level == true && b14Level == true && b15Level == true &&
                                b16Level == true && b17Level == true && b18Level == true && b19Level == true && b20Level == true &&
                                b21Level == true && b22Level == true && b23Level == true && b24Level == true && b25Level == true &&
                                b26Level == true && b27Level == true && b28Level == true && b29Level == true && b30Level == true)
                            {
                                strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2] + "~" + strLevels[3] + "~" + strLevels[4] +
                                    "~" + strLevels[5] + "~" + strLevels[6] + "~" + strLevels[7] + "~" + strLevels[8] + "~" + strLevels[9] +
                                    "~" + strLevels[10] + "~" + strLevels[11] + "~" + strLevels[12] + "~" + strLevels[13] + "~" + strLevels[14] +
                                    "~" + strLevels[15] + "~" + strLevels[16] + "~" + strLevels[17] + "~" + strLevels[18] + "~" + strLevels[19] +
                                    "~" + strLevels[20] + "~" + strLevels[21] + "~" + strLevels[22] + "~" + strLevels[23] + "~" + strLevels[24] +
                                    "~" + strLevels[25] + "~" + strLevels[26] + "~" + strLevels[27] + "~" + strLevels[28];
                                b30Level = false;
                                if (iEndCharCount1 > 1 && iEndChrIndex < iEndCharCount1)
                                    i30Level = -1;
                                continue;
                            }
                            else if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == true &&
                                bFifthLevel == true && bSixthLevel == true && bSeventhLevel == true &&
                                bEighthLevel == true && bNinthLevel == true && b10Level == true &&
                                b11Level == true && b12Level == true && b13Level == true && b14Level == true && b15Level == true &&
                                b16Level == true && b17Level == true && b18Level == true && b19Level == true && b20Level == true &&
                                b21Level == true && b22Level == true && b23Level == true && b24Level == true && b25Level == true &&
                                b26Level == true && b27Level == true && b28Level == true && b29Level == true && b30Level == false)
                            {
                                strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2] + "~" + strLevels[3] + "~" + strLevels[4] +
                                    "~" + strLevels[5] + "~" + strLevels[6] + "~" + strLevels[7] + "~" + strLevels[8] + "~" + strLevels[9] +
                                    "~" + strLevels[10] + "~" + strLevels[11] + "~" + strLevels[12] + "~" + strLevels[13] + "~" + strLevels[14] +
                                    "~" + strLevels[15] + "~" + strLevels[16] + "~" + strLevels[17] + "~" + strLevels[18] + "~" + strLevels[19] +
                                    "~" + strLevels[20] + "~" + strLevels[21] + "~" + strLevels[22] + "~" + strLevels[23] + "~" + strLevels[24] +
                                    "~" + strLevels[25] + "~" + strLevels[26] + "~" + strLevels[27];
                                b29Level = false;
                                if (iEndCharCount1 > 1 && iEndChrIndex < iEndCharCount1)
                                    i29Level = -1;
                                continue;
                            }
                            else if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == true &&
                                bFifthLevel == true && bSixthLevel == true && bSeventhLevel == true &&
                                bEighthLevel == true && bNinthLevel == true && b10Level == true &&
                                b11Level == true && b12Level == true && b13Level == true && b14Level == true && b15Level == true &&
                                b16Level == true && b17Level == true && b18Level == true && b19Level == true && b20Level == true &&
                                b21Level == true && b22Level == true && b23Level == true && b24Level == true && b25Level == true &&
                                b26Level == true && b27Level == true && b28Level == true && b29Level == false && b30Level == false)
                            {
                                strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2] + "~" + strLevels[3] + "~" + strLevels[4] +
                                    "~" + strLevels[5] + "~" + strLevels[6] + "~" + strLevels[7] + "~" + strLevels[8] + "~" + strLevels[9] +
                                    "~" + strLevels[10] + "~" + strLevels[11] + "~" + strLevels[12] + "~" + strLevels[13] + "~" + strLevels[14] +
                                    "~" + strLevels[15] + "~" + strLevels[16] + "~" + strLevels[17] + "~" + strLevels[18] + "~" + strLevels[19] +
                                    "~" + strLevels[20] + "~" + strLevels[21] + "~" + strLevels[22] + "~" + strLevels[23] + "~" + strLevels[24] +
                                    "~" + strLevels[25] + "~" + strLevels[26];
                                b28Level = false;
                                if (iEndCharCount1 > 1 && iEndChrIndex < iEndCharCount1)
                                    i28Level = -1;
                                continue;
                            }
                            else if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == true &&
                               bFifthLevel == true && bSixthLevel == true && bSeventhLevel == true &&
                               bEighthLevel == true && bNinthLevel == true && b10Level == true &&
                               b11Level == true && b12Level == true && b13Level == true && b14Level == true && b15Level == true &&
                               b16Level == true && b17Level == true && b18Level == true && b19Level == true && b20Level == true &&
                               b21Level == true && b22Level == true && b23Level == true && b24Level == true && b25Level == true &&
                               b26Level == true && b27Level == true && b28Level == false && b29Level == false && b30Level == false)
                            {
                                strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2] + "~" + strLevels[3] + "~" + strLevels[4] +
                                    "~" + strLevels[5] + "~" + strLevels[6] + "~" + strLevels[7] + "~" + strLevels[8] + "~" + strLevels[9] +
                                    "~" + strLevels[10] + "~" + strLevels[11] + "~" + strLevels[12] + "~" + strLevels[13] + "~" + strLevels[14] +
                                    "~" + strLevels[15] + "~" + strLevels[16] + "~" + strLevels[17] + "~" + strLevels[18] + "~" + strLevels[19] +
                                    "~" + strLevels[20] + "~" + strLevels[21] + "~" + strLevels[22] + "~" + strLevels[23] + "~" + strLevels[24] +
                                    "~" + strLevels[25];
                                b27Level = false;
                                if (iEndCharCount1 > 1 && iEndChrIndex < iEndCharCount1)
                                    i27Level = -1;
                                continue;
                            }
                            else if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == true &&
                               bFifthLevel == true && bSixthLevel == true && bSeventhLevel == true &&
                               bEighthLevel == true && bNinthLevel == true && b10Level == true &&
                               b11Level == true && b12Level == true && b13Level == true && b14Level == true && b15Level == true &&
                               b16Level == true && b17Level == true && b18Level == true && b19Level == true && b20Level == true &&
                               b21Level == true && b22Level == true && b23Level == true && b24Level == true && b25Level == true &&
                               b26Level == true && b27Level == false && b28Level == false && b29Level == false && b30Level == false)
                            {
                                strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2] + "~" + strLevels[3] + "~" + strLevels[4] +
                                    "~" + strLevels[5] + "~" + strLevels[6] + "~" + strLevels[7] + "~" + strLevels[8] + "~" + strLevels[9] +
                                    "~" + strLevels[10] + "~" + strLevels[11] + "~" + strLevels[12] + "~" + strLevels[13] + "~" + strLevels[14] +
                                    "~" + strLevels[15] + "~" + strLevels[16] + "~" + strLevels[17] + "~" + strLevels[18] + "~" + strLevels[19] +
                                    "~" + strLevels[20] + "~" + strLevels[21] + "~" + strLevels[22] + "~" + strLevels[23] + "~" + strLevels[24];
                                b26Level = false;
                                if (iEndCharCount1 > 1 && iEndChrIndex < iEndCharCount1)
                                    i26Level = -1;
                                continue;
                            }
                            else if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == true &&
                               bFifthLevel == true && bSixthLevel == true && bSeventhLevel == true &&
                               bEighthLevel == true && bNinthLevel == true && b10Level == true &&
                               b11Level == true && b12Level == true && b13Level == true && b14Level == true && b15Level == true &&
                               b16Level == true && b17Level == true && b18Level == true && b19Level == true && b20Level == true &&
                               b21Level == true && b22Level == true && b23Level == true && b24Level == true && b25Level == true &&
                               b26Level == false && b27Level == false && b28Level == false && b29Level == false && b30Level == false)
                            {
                                strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2] + "~" + strLevels[3] + "~" + strLevels[4] +
                                    "~" + strLevels[5] + "~" + strLevels[6] + "~" + strLevels[7] + "~" + strLevels[8] + "~" + strLevels[9] +
                                    "~" + strLevels[10] + "~" + strLevels[11] + "~" + strLevels[12] + "~" + strLevels[13] + "~" + strLevels[14] +
                                    "~" + strLevels[15] + "~" + strLevels[16] + "~" + strLevels[17] + "~" + strLevels[18] + "~" + strLevels[19] +
                                    "~" + strLevels[20] + "~" + strLevels[21] + "~" + strLevels[22] + "~" + strLevels[23];
                                b25Level = false;
                                if (iEndCharCount1 > 1 && iEndChrIndex < iEndCharCount1)
                                    i25Level = -1;
                                continue;
                            }
                            else if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == true &&
                               bFifthLevel == true && bSixthLevel == true && bSeventhLevel == true &&
                               bEighthLevel == true && bNinthLevel == true && b10Level == true &&
                               b11Level == true && b12Level == true && b13Level == true && b14Level == true && b15Level == true &&
                               b16Level == true && b17Level == true && b18Level == true && b19Level == true && b20Level == true &&
                               b21Level == true && b22Level == true && b23Level == true && b24Level == true && b25Level == false &&
                               b26Level == false && b27Level == false && b28Level == false && b29Level == false && b30Level == false)
                            {
                                strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2] + "~" + strLevels[3] + "~" + strLevels[4] +
                                    "~" + strLevels[5] + "~" + strLevels[6] + "~" + strLevels[7] + "~" + strLevels[8] + "~" + strLevels[9] +
                                    "~" + strLevels[10] + "~" + strLevels[11] + "~" + strLevels[12] + "~" + strLevels[13] + "~" + strLevels[14] +
                                    "~" + strLevels[15] + "~" + strLevels[16] + "~" + strLevels[17] + "~" + strLevels[18] + "~" + strLevels[19] +
                                    "~" + strLevels[20] + "~" + strLevels[21] + "~" + strLevels[22];
                                b24Level = false;
                                if (iEndCharCount1 > 1 && iEndChrIndex < iEndCharCount1)
                                    i24Level = -1;
                                continue;
                            }
                            else if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == true &&
                               bFifthLevel == true && bSixthLevel == true && bSeventhLevel == true &&
                               bEighthLevel == true && bNinthLevel == true && b10Level == true &&
                               b11Level == true && b12Level == true && b13Level == true && b14Level == true && b15Level == true &&
                               b16Level == true && b17Level == true && b18Level == true && b19Level == true && b20Level == true &&
                               b21Level == true && b22Level == true && b23Level == true && b24Level == false && b25Level == false &&
                               b26Level == false && b27Level == false && b28Level == false && b29Level == false && b30Level == false)
                            {
                                strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2] + "~" + strLevels[3] + "~" + strLevels[4] +
                                    "~" + strLevels[5] + "~" + strLevels[6] + "~" + strLevels[7] + "~" + strLevels[8] + "~" + strLevels[9] +
                                    "~" + strLevels[10] + "~" + strLevels[11] + "~" + strLevels[12] + "~" + strLevels[13] + "~" + strLevels[14] +
                                    "~" + strLevels[15] + "~" + strLevels[16] + "~" + strLevels[17] + "~" + strLevels[18] + "~" + strLevels[19] +
                                    "~" + strLevels[20] + "~" + strLevels[21];
                                b23Level = false;
                                if (iEndCharCount1 > 1 && iEndChrIndex < iEndCharCount1)
                                    i23Level = -1;
                                continue;
                            }
                            else if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == true &&
                               bFifthLevel == true && bSixthLevel == true && bSeventhLevel == true &&
                               bEighthLevel == true && bNinthLevel == true && b10Level == true &&
                               b11Level == true && b12Level == true && b13Level == true && b14Level == true && b15Level == true &&
                               b16Level == true && b17Level == true && b18Level == true && b19Level == true && b20Level == true &&
                               b21Level == true && b22Level == true && b23Level == false && b24Level == false && b25Level == false &&
                               b26Level == false && b27Level == false && b28Level == false && b29Level == false && b30Level == false)
                            {
                                strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2] + "~" + strLevels[3] + "~" + strLevels[4] +
                                    "~" + strLevels[5] + "~" + strLevels[6] + "~" + strLevels[7] + "~" + strLevels[8] + "~" + strLevels[9] +
                                    "~" + strLevels[10] + "~" + strLevels[11] + "~" + strLevels[12] + "~" + strLevels[13] + "~" + strLevels[14] +
                                    "~" + strLevels[15] + "~" + strLevels[16] + "~" + strLevels[17] + "~" + strLevels[18] + "~" + strLevels[19] +
                                    "~" + strLevels[20];
                                b22Level = false;
                                if (iEndCharCount1 > 1 && iEndChrIndex < iEndCharCount1)
                                    i22Level = -1;
                                continue;
                            }
                            else if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == true &&
                               bFifthLevel == true && bSixthLevel == true && bSeventhLevel == true &&
                               bEighthLevel == true && bNinthLevel == true && b10Level == true &&
                               b11Level == true && b12Level == true && b13Level == true && b14Level == true && b15Level == true &&
                               b16Level == true && b17Level == true && b18Level == true && b19Level == true && b20Level == true &&
                               b21Level == true && b22Level == false && b23Level == false && b24Level == false && b25Level == false &&
                               b26Level == false && b27Level == false && b28Level == false && b29Level == false && b30Level == false)
                            {
                                strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2] + "~" + strLevels[3] + "~" + strLevels[4] +
                                    "~" + strLevels[5] + "~" + strLevels[6] + "~" + strLevels[7] + "~" + strLevels[8] + "~" + strLevels[9] +
                                    "~" + strLevels[10] + "~" + strLevels[11] + "~" + strLevels[12] + "~" + strLevels[13] + "~" + strLevels[14] +
                                    "~" + strLevels[15] + "~" + strLevels[16] + "~" + strLevels[17] + "~" + strLevels[18] + "~" + strLevels[19];
                                b21Level = false;
                                if (iEndCharCount1 > 1 && iEndChrIndex < iEndCharCount1)
                                    i21Level = -1;
                                continue;
                            }
                            else if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == true &&
                               bFifthLevel == true && bSixthLevel == true && bSeventhLevel == true &&
                               bEighthLevel == true && bNinthLevel == true && b10Level == true &&
                               b11Level == true && b12Level == true && b13Level == true && b14Level == true && b15Level == true &&
                               b16Level == true && b17Level == true && b18Level == true && b19Level == true && b20Level == true &&
                               b21Level == false && b22Level == false && b23Level == false && b24Level == false && b25Level == false &&
                               b26Level == false && b27Level == false && b28Level == false && b29Level == false && b30Level == false)
                            {
                                strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2] + "~" + strLevels[3] + "~" + strLevels[4] +
                                    "~" + strLevels[5] + "~" + strLevels[6] + "~" + strLevels[7] + "~" + strLevels[8] + "~" + strLevels[9] +
                                    "~" + strLevels[10] + "~" + strLevels[11] + "~" + strLevels[12] + "~" + strLevels[13] + "~" + strLevels[14] +
                                    "~" + strLevels[15] + "~" + strLevels[16] + "~" + strLevels[17] + "~" + strLevels[18];
                                b20Level = false;
                                if (iEndCharCount1 > 1 && iEndChrIndex < iEndCharCount1)
                                    i20Level = -1;
                                continue;
                            }
                            else if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == true &&
                               bFifthLevel == true && bSixthLevel == true && bSeventhLevel == true &&
                               bEighthLevel == true && bNinthLevel == true && b10Level == true &&
                               b11Level == true && b12Level == true && b13Level == true && b14Level == true && b15Level == true &&
                               b16Level == true && b17Level == true && b18Level == true && b19Level == true && b20Level == false &&
                               b21Level == false && b22Level == false && b23Level == false && b24Level == false && b25Level == false &&
                               b26Level == false && b27Level == false && b28Level == false && b29Level == false && b30Level == false)
                            {
                                strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2] + "~" + strLevels[3] + "~" + strLevels[4] +
                                    "~" + strLevels[5] + "~" + strLevels[6] + "~" + strLevels[7] + "~" + strLevels[8] + "~" + strLevels[9] +
                                    "~" + strLevels[10] + "~" + strLevels[11] + "~" + strLevels[12] + "~" + strLevels[13] + "~" + strLevels[14] +
                                    "~" + strLevels[15] + "~" + strLevels[16] + "~" + strLevels[17];
                                b19Level = false;
                                if (iEndCharCount1 > 1 && iEndChrIndex < iEndCharCount1)
                                    i19Level = -1;
                                continue;
                            }
                            else if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == true &&
                               bFifthLevel == true && bSixthLevel == true && bSeventhLevel == true &&
                               bEighthLevel == true && bNinthLevel == true && b10Level == true &&
                               b11Level == true && b12Level == true && b13Level == true && b14Level == true && b15Level == true &&
                               b16Level == true && b17Level == true && b18Level == true && b19Level == false && b20Level == false &&
                               b21Level == false && b22Level == false && b23Level == false && b24Level == false && b25Level == false &&
                               b26Level == false && b27Level == false && b28Level == false && b29Level == false && b30Level == false)
                            {
                                strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2] + "~" + strLevels[3] + "~" + strLevels[4] +
                                    "~" + strLevels[5] + "~" + strLevels[6] + "~" + strLevels[7] + "~" + strLevels[8] + "~" + strLevels[9] +
                                    "~" + strLevels[10] + "~" + strLevels[11] + "~" + strLevels[12] + "~" + strLevels[13] + "~" + strLevels[14] +
                                    "~" + strLevels[15] + "~" + strLevels[16];
                                b18Level = false;
                                if (iEndCharCount1 > 1 && iEndChrIndex < iEndCharCount1)
                                    i18Level = -1;
                                continue;
                            }
                            else if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == true &&
                               bFifthLevel == true && bSixthLevel == true && bSeventhLevel == true &&
                               bEighthLevel == true && bNinthLevel == true && b10Level == true &&
                               b11Level == true && b12Level == true && b13Level == true && b14Level == true && b15Level == true &&
                               b16Level == true && b17Level == true && b18Level == false && b19Level == false && b20Level == false &&
                               b21Level == false && b22Level == false && b23Level == false && b24Level == false && b25Level == false &&
                               b26Level == false && b27Level == false && b28Level == false && b29Level == false && b30Level == false)
                            {
                                strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2] + "~" + strLevels[3] + "~" + strLevels[4] +
                                    "~" + strLevels[5] + "~" + strLevels[6] + "~" + strLevels[7] + "~" + strLevels[8] + "~" + strLevels[9] +
                                    "~" + strLevels[10] + "~" + strLevels[11] + "~" + strLevels[12] + "~" + strLevels[13] + "~" + strLevels[14] +
                                    "~" + strLevels[15];
                                b17Level = false;
                                if (iEndCharCount1 > 1 && iEndChrIndex < iEndCharCount1)
                                    i17Level = -1;
                                continue;
                            }
                            else if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == true &&
                               bFifthLevel == true && bSixthLevel == true && bSeventhLevel == true &&
                               bEighthLevel == true && bNinthLevel == true && b10Level == true &&
                               b11Level == true && b12Level == true && b13Level == true && b14Level == true && b15Level == true &&
                               b16Level == true && b17Level == false && b18Level == false && b19Level == false && b20Level == false &&
                               b21Level == false && b22Level == false && b23Level == false && b24Level == false && b25Level == false &&
                               b26Level == false && b27Level == false && b28Level == false && b29Level == false && b30Level == false)
                            {
                                strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2] + "~" + strLevels[3] + "~" + strLevels[4] +
                                    "~" + strLevels[5] + "~" + strLevels[6] + "~" + strLevels[7] + "~" + strLevels[8] + "~" + strLevels[9] +
                                    "~" + strLevels[10] + "~" + strLevels[11] + "~" + strLevels[12] + "~" + strLevels[13] + "~" + strLevels[14];
                                b16Level = false;
                                if (iEndCharCount1 > 1 && iEndChrIndex < iEndCharCount1)
                                    i16Level = -1;
                                continue;
                            }
                            else if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == true &&
                                bFifthLevel == true && bSixthLevel == true && bSeventhLevel == true &&
                                bEighthLevel == true && bNinthLevel == true && b10Level == true &&
                                b11Level == true && b12Level == true && b13Level == true && b14Level == true && b15Level == true &&
                               b16Level == false && b17Level == false && b18Level == false && b19Level == false && b20Level == false &&
                               b21Level == false && b22Level == false && b23Level == false && b24Level == false && b25Level == false &&
                               b26Level == false && b27Level == false && b28Level == false && b29Level == false && b30Level == false)
                            {
                                strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2]
                                    + "~" + strLevels[3] + "~" + strLevels[4] + "~" + strLevels[5]
                                    + "~" + strLevels[6] + "~" + strLevels[7] + "~" + strLevels[8]
                                    + "~" + strLevels[9] + "~" + strLevels[10] + "~" + strLevels[11]
                                    + "~" + strLevels[12] + "~" + strLevels[13];
                                b15Level = false;
                                if (iEndCharCount1 > 1 && iEndChrIndex < iEndCharCount1)
                                    i15Level = -1;
                                continue;
                            }
                            else if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == true &&
                                bFifthLevel == true && bSixthLevel == true && bSeventhLevel == true &&
                                bEighthLevel == true && bNinthLevel == true && b10Level == true &&
                                b11Level == true && b12Level == true && b13Level == true && b14Level == true && b15Level == false &&
                               b16Level == false && b17Level == false && b18Level == false && b19Level == false && b20Level == false &&
                               b21Level == false && b22Level == false && b23Level == false && b24Level == false && b25Level == false &&
                               b26Level == false && b27Level == false && b28Level == false && b29Level == false && b30Level == false)
                            {
                                strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2]
                                    + "~" + strLevels[3] + "~" + strLevels[4] + "~" + strLevels[5]
                                    + "~" + strLevels[6] + "~" + strLevels[7] + "~" + strLevels[8]
                                    + "~" + strLevels[9] + "~" + strLevels[10] + "~" + strLevels[11]
                                    + "~" + strLevels[12];
                                b14Level = false;
                                //if (iNinthLevel > 0)
                                //    iNinthLevel--;
                                if (iEndCharCount1 > 1 && iEndChrIndex < iEndCharCount1)
                                    i14Level = -1;
                                continue;
                            }
                            else if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == true &&
                                bFifthLevel == true && bSixthLevel == true && bSeventhLevel == true &&
                                bEighthLevel == true && bNinthLevel == true && b10Level == true &&
                                b11Level == true && b12Level == true && b13Level == true && b14Level == false && b15Level == false &&
                               b16Level == false && b17Level == false && b18Level == false && b19Level == false && b20Level == false &&
                               b21Level == false && b22Level == false && b23Level == false && b24Level == false && b25Level == false &&
                               b26Level == false && b27Level == false && b28Level == false && b29Level == false && b30Level == false)
                            {
                                strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2]
                                    + "~" + strLevels[3] + "~" + strLevels[4] + "~" + strLevels[5]
                                    + "~" + strLevels[6] + "~" + strLevels[7] + "~" + strLevels[8]
                                    + "~" + strLevels[9] + "~" + strLevels[10] + "~" + strLevels[11];
                                b13Level = false;
                                //if (iNinthLevel > 0)
                                //    iNinthLevel--;
                                if (iEndCharCount1 > 1 && iEndChrIndex < iEndCharCount1)
                                    i13Level = -1;
                                continue;
                            }
                            else if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == true &&
                                bFifthLevel == true && bSixthLevel == true && bSeventhLevel == true &&
                                bEighthLevel == true && bNinthLevel == true && b10Level == true &&
                                b11Level == true && b12Level == true && b13Level == false && b14Level == false && b15Level == false &&
                               b16Level == false && b17Level == false && b18Level == false && b19Level == false && b20Level == false &&
                               b21Level == false && b22Level == false && b23Level == false && b24Level == false && b25Level == false &&
                               b26Level == false && b27Level == false && b28Level == false && b29Level == false && b30Level == false)
                            {
                                strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2]
                                    + "~" + strLevels[3] + "~" + strLevels[4] + "~" + strLevels[5]
                                    + "~" + strLevels[6] + "~" + strLevels[7] + "~" + strLevels[8]
                                    + "~" + strLevels[9] + "~" + strLevels[10];
                                b12Level = false;
                                //if (iNinthLevel > 0)
                                //    iNinthLevel--;
                                if (iEndCharCount1 > 1 && iEndChrIndex < iEndCharCount1)
                                    i12Level = -1;
                                continue;
                            }
                            else if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == true &&
                                bFifthLevel == true && bSixthLevel == true && bSeventhLevel == true &&
                                bEighthLevel == true && bNinthLevel == true && b10Level == true &&
                                b11Level == true && b12Level == false && b13Level == false && b14Level == false && b15Level == false &&
                               b16Level == false && b17Level == false && b18Level == false && b19Level == false && b20Level == false &&
                               b21Level == false && b22Level == false && b23Level == false && b24Level == false && b25Level == false &&
                               b26Level == false && b27Level == false && b28Level == false && b29Level == false && b30Level == false)
                            {
                                strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2]
                                    + "~" + strLevels[3] + "~" + strLevels[4] + "~" + strLevels[5]
                                    + "~" + strLevels[6] + "~" + strLevels[7] + "~" + strLevels[8]
                                    + "~" + strLevels[9];
                                b11Level = false;
                                //if (iNinthLevel > 0)
                                //    iNinthLevel--;
                                if (iEndCharCount1 > 1 && iEndChrIndex < iEndCharCount1)
                                    i11Level = -1;
                                continue;
                            }
                            else if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == true &&
                                bFifthLevel == true && bSixthLevel == true && bSeventhLevel == true &&
                                bEighthLevel == true && bNinthLevel == true && b10Level == true &&
                                b11Level == false && b12Level == false && b13Level == false && b14Level == false && b15Level == false &&
                               b16Level == false && b17Level == false && b18Level == false && b19Level == false && b20Level == false &&
                               b21Level == false && b22Level == false && b23Level == false && b24Level == false && b25Level == false &&
                               b26Level == false && b27Level == false && b28Level == false && b29Level == false && b30Level == false)
                            {
                                strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2]
                                    + "~" + strLevels[3] + "~" + strLevels[4] + "~" + strLevels[5]
                                    + "~" + strLevels[6] + "~" + strLevels[7] + "~" + strLevels[8];
                                b10Level = false;
                                //if (iNinthLevel > 0)
                                //    iNinthLevel--;
                                if (iEndCharCount1 > 1 && iEndChrIndex < iEndCharCount1)
                                    i10Level = -1;
                                continue;
                            }
                            if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == true &&
                                bFifthLevel == true && bSixthLevel == true && bSeventhLevel == true &&
                                bEighthLevel == true && bNinthLevel == true && b10Level == false &&
                                b11Level == false && b12Level == false && b13Level == false && b14Level == false && b15Level == false &&
                                b16Level == false && b17Level == false && b18Level == false && b19Level == false && b20Level == false &&
                                b21Level == false && b22Level == false && b23Level == false && b24Level == false && b25Level == false &&
                                b26Level == false && b27Level == false && b28Level == false && b29Level == false && b30Level == false)
                            {
                                strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2]
                                    + "~" + strLevels[3] + "~" + strLevels[4] + "~" + strLevels[5]
                                    + "~" + strLevels[6] + "~" + strLevels[7];
                                bNinthLevel = false;
                                //if (iNinthLevel > 0)
                                //    iNinthLevel--;
                                if (iEndCharCount1 > 1 && iEndChrIndex < iEndCharCount1)
                                    iNinthLevel = -1;
                                continue;
                            }
                            if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == true &&
                                bFifthLevel == true && bSixthLevel == true && bSeventhLevel == true &&
                                bEighthLevel == true && bNinthLevel == false && b10Level == false &&
                                b11Level == false && b12Level == false && b13Level == false && b14Level == false && b15Level == false &&
                                b16Level == false && b17Level == false && b18Level == false && b19Level == false && b20Level == false &&
                                b21Level == false && b22Level == false && b23Level == false && b24Level == false && b25Level == false &&
                                b26Level == false && b27Level == false && b28Level == false && b29Level == false && b30Level == false)
                            {
                                strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2]
                                    + "~" + strLevels[3] + "~" + strLevels[4] + "~" + strLevels[5]
                                    + "~" + strLevels[6];
                                bEighthLevel = false;
                                //if (iEighthLevel > 0)
                                //    iEighthLevel--;

                                if (iEndCharCount1 > 1 && iEndChrIndex < iEndCharCount1)
                                    iEighthLevel = -1;
                                continue;
                            }
                            if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == true &&
                                bFifthLevel == true && bSixthLevel == true && bSeventhLevel == true &&
                                bEighthLevel == false && bNinthLevel == false && b10Level == false &&
                                b11Level == false && b12Level == false && b13Level == false && b14Level == false && b15Level == false &&
                                b16Level == false && b17Level == false && b18Level == false && b19Level == false && b20Level == false &&
                                b21Level == false && b22Level == false && b23Level == false && b24Level == false && b25Level == false &&
                                b26Level == false && b27Level == false && b28Level == false && b29Level == false && b30Level == false)
                            {
                                strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2]
                                    + "~" + strLevels[3] + "~" + strLevels[4] + "~" + strLevels[5];
                                bSeventhLevel = false;
                                //if (iSeventhLevel > 0)
                                //    iSeventhLevel--;

                                if (iEndCharCount1 > 1 && iEndChrIndex < iEndCharCount1)
                                    iSeventhLevel = -1;
                                continue;
                            }
                            if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == true &&
                                bFifthLevel == true && bSixthLevel == true && bSeventhLevel == false &&
                                bEighthLevel == false && bNinthLevel == false && b10Level == false &&
                                b11Level == false && b12Level == false && b13Level == false && b14Level == false && b15Level == false &&
                                b16Level == false && b17Level == false && b18Level == false && b19Level == false && b20Level == false &&
                                b21Level == false && b22Level == false && b23Level == false && b24Level == false && b25Level == false &&
                                b26Level == false && b27Level == false && b28Level == false && b29Level == false && b30Level == false)
                            {
                                strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2]
                                    + "~" + strLevels[3] + "~" + strLevels[4];
                                bSixthLevel = false;
                                //if (iSixthLevel > 0)
                                //    iSixthLevel--;

                                if (iEndCharCount1 > 1 && iEndChrIndex < iEndCharCount1)
                                    iSixthLevel = -1;
                                continue;
                            }
                            if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == true &&
                                bFifthLevel == true && bSixthLevel == false && bSeventhLevel == false &&
                                bEighthLevel == false && bNinthLevel == false && b10Level == false &&
                                b11Level == false && b12Level == false && b13Level == false && b14Level == false && b15Level == false &&
                                b16Level == false && b17Level == false && b18Level == false && b19Level == false && b20Level == false &&
                                b21Level == false && b22Level == false && b23Level == false && b24Level == false && b25Level == false &&
                                b26Level == false && b27Level == false && b28Level == false && b29Level == false && b30Level == false)
                            {
                                strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2]
                                    + "~" + strLevels[3];
                                bFifthLevel = false;
                                //if (iFifthLevel > 0)
                                //    iFifthLevel--;

                                if (iEndCharCount1 > 1 && iEndChrIndex < iEndCharCount1)
                                    iFifthLevel = -1;
                                continue;
                            }
                            if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == true &&
                                bFifthLevel == false && bSixthLevel == false && bSeventhLevel == false &&
                                bEighthLevel == false && bNinthLevel == false && b10Level == false &&
                                b11Level == false && b12Level == false && b13Level == false && b14Level == false && b15Level == false &&
                                b16Level == false && b17Level == false && b18Level == false && b19Level == false && b20Level == false &&
                                b21Level == false && b22Level == false && b23Level == false && b24Level == false && b25Level == false &&
                                b26Level == false && b27Level == false && b28Level == false && b29Level == false && b30Level == false)
                            {
                                strLevelPath = strLevels[0] + "~" + strLevels[1] + "~" + strLevels[2];
                                bFourthLevel = false;
                                //if (iFourthLevel > 0)
                                //    iFourthLevel--;

                                if (iEndCharCount1 > 1 && iEndChrIndex < iEndCharCount1)
                                    iFourthLevel = -1;
                                continue;
                            }
                            if (bSecondlevel == true && bThirdLevel == true && bFourthLevel == false &&
                                bFifthLevel == false && bSixthLevel == false && bSeventhLevel == false &&
                                bEighthLevel == false && bNinthLevel == false && b10Level == false &&
                                b11Level == false && b12Level == false && b13Level == false && b14Level == false && b15Level == false &&
                                b16Level == false && b17Level == false && b18Level == false && b19Level == false && b20Level == false &&
                                b21Level == false && b22Level == false && b23Level == false && b24Level == false && b25Level == false &&
                                b26Level == false && b27Level == false && b28Level == false && b29Level == false && b30Level == false)
                            {
                                strLevelPath = strLevels[0] + "~" + strLevels[1];
                                bThirdLevel = false;
                                //if (iThirdLevel > 0)
                                //    iThirdLevel--;

                                if (iEndCharCount1 > 1 && iEndChrIndex < iEndCharCount1)
                                    iThirdLevel = -1;
                                continue;
                            }
                            if (bSecondlevel == true && bThirdLevel == false && bFourthLevel == false &&
                                bFifthLevel == false && bSixthLevel == false && bSeventhLevel == false &&
                                bEighthLevel == false && bNinthLevel == false && b10Level == false &&
                                b11Level == false && b12Level == false && b13Level == false && b14Level == false && b15Level == false &&
                                b16Level == false && b17Level == false && b18Level == false && b19Level == false && b20Level == false &&
                                b21Level == false && b22Level == false && b23Level == false && b24Level == false && b25Level == false &&
                                b26Level == false && b27Level == false && b28Level == false && b29Level == false && b30Level == false)
                            {
                                strLevelPath = strLevels[0];
                                bSecondlevel = false;
                                //if (iSecondlevel > 0)
                                //    iSecondlevel--;

                                if (iEndCharCount1 > 1 && iEndChrIndex < iEndCharCount1)
                                    iSecondlevel = -1;
                                continue;
                            }
                        }
                    }

                    #endregion "GetListEndChar Result"
                }

                #endregion "List"

                #region "Other"

                if (strLineValue.Substring(0, 2).Trim() != "L")
                {
                    //检查是A类型数据，需要把当前A类型数据开始，到下一个数据类型的值拼接上，才是完成的数据，因为数据中间
                    //可能出现<,">

                    if (strLineValue.Contains("A "))
                    {
                        string strLineValueTmp = "";
                        for (int iIndexTmp = iIndex; iIndexTmp < strItems.Length; iIndexTmp++)
                        {
                            if (strItems[iIndexTmp].Contains("'>") == false)
                            {
                                //如果已经是最后一个ITEM，直接取值
                                if (iIndexTmp == (strItems.Length - 1))
                                {
                                    strLineValueTmp = strLineValueTmp + strItems[iIndexTmp].Trim();
                                    break;
                                }
                                else
                                {
                                    //检查一个数据，如果是SECS ITEM，直接取值
                                    if (CheckSECSItem(strItems[iIndexTmp + 1]) == true)
                                    {
                                        strLineValueTmp = strLineValueTmp + strItems[iIndexTmp].Trim();
                                        break;
                                    }
                                    else
                                        strLineValueTmp = strLineValueTmp + strItems[iIndexTmp].Trim() + "<";
                                }
                            }
                            else
                            {
                                strLineValueTmp = strLineValueTmp + strItems[iIndexTmp].Trim();
                                iIndex = iIndexTmp;
                                break;
                            }
                        }
                        bResult = AddItemToTrx(strLineValueTmp, strLevelPath, ref secsTrx, ref strRet);
                        strLineValue = strLineValueTmp;
                    }
                    else
                        bResult = AddItemToTrx(strLineValue, strLevelPath, ref secsTrx, ref strRet);
                    if (bResult == true)
                    {
                        string[] strItemslevelTmps = strLevelPath.Split('~');
                        switch (strItemslevelTmps.Length)
                        {
                            case 1:
                                iSecondlevel++;
                                //strLevelPath = iSecondlevel.ToString();
                                break;

                            case 2:
                                iThirdLevel++;
                                //strLevelPath = strItemslevelTmps[0] + "~" + iThirdLevel.ToString();
                                break;

                            case 3:
                                iFourthLevel++;
                                //strLevelPath = strItemslevelTmps[0] + "~" + strItemslevelTmps[1] + "~" + iFourthLevel.ToString();
                                break;

                            case 4:
                                iFifthLevel++;
                                //strLevelPath = strItemslevelTmps[0] + "~" + strItemslevelTmps[1] + "~" + strItemslevelTmps[2] + "~" +
                                //    iFifthLevel.ToString();
                                break;

                            case 5:
                                iSixthLevel++;
                                //strLevelPath = strItemslevelTmps[0] + "~" + strItemslevelTmps[1] + "~" + strItemslevelTmps[2] + "~" +
                                //    strItemslevelTmps[3] + "~" + iFifthLevel.ToString();
                                break;

                            case 6:
                                iSeventhLevel++;
                                //strLevelPath = strItemslevelTmps[0] + "~" + strItemslevelTmps[1] + "~" + strItemslevelTmps[2] + "~" +
                                //    strItemslevelTmps[3] + "~" + strItemslevelTmps[4] + "~" + iSeventhLevel.ToString();
                                break;

                            case 7:
                                iEighthLevel++;
                                //strLevelPath = strItemslevelTmps[0] + "~" + strItemslevelTmps[1] + "~" + strItemslevelTmps[2] + "~" +
                                //    strItemslevelTmps[3] + "~" + strItemslevelTmps[4] + "~" + strItemslevelTmps[5] + "~" + iEighthLevel.ToString();
                                break;

                            case 8:
                                iNinthLevel++;
                                //strLevelPath = strItemslevelTmps[0] + "~" + strItemslevelTmps[1] + "~" + strItemslevelTmps[2] + "~" +
                                //    strItemslevelTmps[3] + "~" + strItemslevelTmps[4] + "~" + strItemslevelTmps[5] + "~" +
                                //    strItemslevelTmps[6] + "~" + iNinthLevel.ToString();
                                break;

                            case 9:
                                i10Level++;
                                break;

                            case 10:
                                i11Level++;
                                break;

                            case 11:
                                i12Level++;
                                break;

                            case 12:
                                i13Level++;
                                break;

                            case 13:
                                i14Level++;
                                break;

                            case 14:
                                i15Level++;
                                break;

                            case 15:
                                i16Level++;
                                break;

                            case 16:
                                i17Level++;
                                break;

                            case 17:
                                i18Level++;
                                break;

                            case 18:
                                i19Level++;
                                break;

                            case 19:
                                i20Level++;
                                break;

                            case 20:
                                i21Level++;
                                break;

                            case 21:
                                i22Level++;
                                break;

                            case 22:
                                i23Level++;
                                break;

                            case 23:
                                i24Level++;
                                break;

                            case 24:
                                i25Level++;
                                break;

                            case 25:
                                i26Level++;
                                break;

                            case 26:
                                i27Level++;
                                break;

                            case 27:
                                i28Level++;
                                break;

                            case 28:
                                i29Level++;
                                break;

                            case 29:
                                i30Level++;
                                break;
                        }

                        //检查是否LIST的结束符号
                        int iEndCharCount = GetListEndChar(strLineValue);
                        if (iEndCharCount == -100)
                            return secsTrx;

                        #region "GetListEndChar Result"

                        if (iEndCharCount > 0)
                        {
                            for (int iEndChrIndex = 1; iEndChrIndex <= iEndCharCount; iEndChrIndex++)
                            {
                                if (b30Level == true)
                                {
                                    b30Level = false;
                                    strLevelPath = strItemslevelTmps[0] + "~" + strItemslevelTmps[1] + "~" + strItemslevelTmps[2] + "~" + strItemslevelTmps[3] + "~" + strItemslevelTmps[4] +
                                        "~" + strItemslevelTmps[5] + "~" + strItemslevelTmps[6] + "~" + strItemslevelTmps[7] + "~" + strItemslevelTmps[8] + "~" + strItemslevelTmps[9] +
                                        "~" + strItemslevelTmps[10] + "~" + strItemslevelTmps[11] + "~" + strItemslevelTmps[12] + "~" + strItemslevelTmps[13] + "~" + strItemslevelTmps[14] +
                                        "~" + strItemslevelTmps[15] + "~" + strItemslevelTmps[16] + "~" + strItemslevelTmps[17] + "~" + strItemslevelTmps[18] + "~" + strItemslevelTmps[19] +
                                        "~" + strItemslevelTmps[20] + "~" + strItemslevelTmps[21] + "~" + strItemslevelTmps[22] + "~" + strItemslevelTmps[23] + "~" + strItemslevelTmps[24] +
                                        "~" + strItemslevelTmps[25] + "~" + strItemslevelTmps[26] + "~" + strItemslevelTmps[27] + "~" + strItemslevelTmps[28];
                                    continue;
                                }
                                if (b29Level == true)
                                {
                                    b29Level = false;
                                    i30Level = -1;
                                    strLevelPath = strItemslevelTmps[0] + "~" + strItemslevelTmps[1] + "~" + strItemslevelTmps[2] + "~" + strItemslevelTmps[3] + "~" + strItemslevelTmps[4] +
                                        "~" + strItemslevelTmps[5] + "~" + strItemslevelTmps[6] + "~" + strItemslevelTmps[7] + "~" + strItemslevelTmps[8] + "~" + strItemslevelTmps[9] +
                                        "~" + strItemslevelTmps[10] + "~" + strItemslevelTmps[11] + "~" + strItemslevelTmps[12] + "~" + strItemslevelTmps[13] + "~" + strItemslevelTmps[14] +
                                        "~" + strItemslevelTmps[15] + "~" + strItemslevelTmps[16] + "~" + strItemslevelTmps[17] + "~" + strItemslevelTmps[18] + "~" + strItemslevelTmps[19] +
                                        "~" + strItemslevelTmps[20] + "~" + strItemslevelTmps[21] + "~" + strItemslevelTmps[22] + "~" + strItemslevelTmps[23] + "~" + strItemslevelTmps[24] +
                                        "~" + strItemslevelTmps[25] + "~" + strItemslevelTmps[26] + "~" + strItemslevelTmps[27];
                                    continue;
                                }
                                if (b28Level == true)
                                {
                                    b28Level = false;
                                    i29Level = -1;
                                    strLevelPath = strItemslevelTmps[0] + "~" + strItemslevelTmps[1] + "~" + strItemslevelTmps[2] + "~" + strItemslevelTmps[3] + "~" + strItemslevelTmps[4] +
                                        "~" + strItemslevelTmps[5] + "~" + strItemslevelTmps[6] + "~" + strItemslevelTmps[7] + "~" + strItemslevelTmps[8] + "~" + strItemslevelTmps[9] +
                                        "~" + strItemslevelTmps[10] + "~" + strItemslevelTmps[11] + "~" + strItemslevelTmps[12] + "~" + strItemslevelTmps[13] + "~" + strItemslevelTmps[14] +
                                        "~" + strItemslevelTmps[15] + "~" + strItemslevelTmps[16] + "~" + strItemslevelTmps[17] + "~" + strItemslevelTmps[18] + "~" + strItemslevelTmps[19] +
                                        "~" + strItemslevelTmps[20] + "~" + strItemslevelTmps[21] + "~" + strItemslevelTmps[22] + "~" + strItemslevelTmps[23] + "~" + strItemslevelTmps[24] +
                                        "~" + strItemslevelTmps[25] + "~" + strItemslevelTmps[26];
                                    continue;
                                }
                                if (b27Level == true)
                                {
                                    b27Level = false;
                                    i28Level = -1;
                                    strLevelPath = strItemslevelTmps[0] + "~" + strItemslevelTmps[1] + "~" + strItemslevelTmps[2] + "~" + strItemslevelTmps[3] + "~" + strItemslevelTmps[4] +
                                        "~" + strItemslevelTmps[5] + "~" + strItemslevelTmps[6] + "~" + strItemslevelTmps[7] + "~" + strItemslevelTmps[8] + "~" + strItemslevelTmps[9] +
                                        "~" + strItemslevelTmps[10] + "~" + strItemslevelTmps[11] + "~" + strItemslevelTmps[12] + "~" + strItemslevelTmps[13] + "~" + strItemslevelTmps[14] +
                                        "~" + strItemslevelTmps[15] + "~" + strItemslevelTmps[16] + "~" + strItemslevelTmps[17] + "~" + strItemslevelTmps[18] + "~" + strItemslevelTmps[19] +
                                        "~" + strItemslevelTmps[20] + "~" + strItemslevelTmps[21] + "~" + strItemslevelTmps[22] + "~" + strItemslevelTmps[23] + "~" + strItemslevelTmps[24] +
                                        "~" + strItemslevelTmps[25];
                                    continue;
                                }
                                if (b26Level == true)
                                {
                                    b26Level = false;
                                    i27Level = -1;
                                    strLevelPath = strItemslevelTmps[0] + "~" + strItemslevelTmps[1] + "~" + strItemslevelTmps[2] + "~" + strItemslevelTmps[3] + "~" + strItemslevelTmps[4] +
                                        "~" + strItemslevelTmps[5] + "~" + strItemslevelTmps[6] + "~" + strItemslevelTmps[7] + "~" + strItemslevelTmps[8] + "~" + strItemslevelTmps[9] +
                                        "~" + strItemslevelTmps[10] + "~" + strItemslevelTmps[11] + "~" + strItemslevelTmps[12] + "~" + strItemslevelTmps[13] + "~" + strItemslevelTmps[14] +
                                        "~" + strItemslevelTmps[15] + "~" + strItemslevelTmps[16] + "~" + strItemslevelTmps[17] + "~" + strItemslevelTmps[18] + "~" + strItemslevelTmps[19] +
                                        "~" + strItemslevelTmps[20] + "~" + strItemslevelTmps[21] + "~" + strItemslevelTmps[22] + "~" + strItemslevelTmps[23] + "~" + strItemslevelTmps[24];
                                    continue;
                                }
                                if (b25Level == true)
                                {
                                    b25Level = false;
                                    i26Level = -1;
                                    strLevelPath = strItemslevelTmps[0] + "~" + strItemslevelTmps[1] + "~" + strItemslevelTmps[2] + "~" + strItemslevelTmps[3] + "~" + strItemslevelTmps[4] +
                                        "~" + strItemslevelTmps[5] + "~" + strItemslevelTmps[6] + "~" + strItemslevelTmps[7] + "~" + strItemslevelTmps[8] + "~" + strItemslevelTmps[9] +
                                        "~" + strItemslevelTmps[10] + "~" + strItemslevelTmps[11] + "~" + strItemslevelTmps[12] + "~" + strItemslevelTmps[13] + "~" + strItemslevelTmps[14] +
                                        "~" + strItemslevelTmps[15] + "~" + strItemslevelTmps[16] + "~" + strItemslevelTmps[17] + "~" + strItemslevelTmps[18] + "~" + strItemslevelTmps[19] +
                                        "~" + strItemslevelTmps[20] + "~" + strItemslevelTmps[21] + "~" + strItemslevelTmps[22] + "~" + strItemslevelTmps[23];
                                    continue;
                                }
                                if (b24Level == true)
                                {
                                    b24Level = false;
                                    i25Level = -1;
                                    strLevelPath = strItemslevelTmps[0] + "~" + strItemslevelTmps[1] + "~" + strItemslevelTmps[2] + "~" + strItemslevelTmps[3] + "~" + strItemslevelTmps[4] +
                                        "~" + strItemslevelTmps[5] + "~" + strItemslevelTmps[6] + "~" + strItemslevelTmps[7] + "~" + strItemslevelTmps[8] + "~" + strItemslevelTmps[9] +
                                        "~" + strItemslevelTmps[10] + "~" + strItemslevelTmps[11] + "~" + strItemslevelTmps[12] + "~" + strItemslevelTmps[13] + "~" + strItemslevelTmps[14] +
                                        "~" + strItemslevelTmps[15] + "~" + strItemslevelTmps[16] + "~" + strItemslevelTmps[17] + "~" + strItemslevelTmps[18] + "~" + strItemslevelTmps[19] +
                                        "~" + strItemslevelTmps[20] + "~" + strItemslevelTmps[21] + "~" + strItemslevelTmps[22];
                                    continue;
                                }
                                if (b23Level == true)
                                {
                                    b23Level = false;
                                    i24Level = -1;
                                    strLevelPath = strItemslevelTmps[0] + "~" + strItemslevelTmps[1] + "~" + strItemslevelTmps[2] + "~" + strItemslevelTmps[3] + "~" + strItemslevelTmps[4] +
                                        "~" + strItemslevelTmps[5] + "~" + strItemslevelTmps[6] + "~" + strItemslevelTmps[7] + "~" + strItemslevelTmps[8] + "~" + strItemslevelTmps[9] +
                                        "~" + strItemslevelTmps[10] + "~" + strItemslevelTmps[11] + "~" + strItemslevelTmps[12] + "~" + strItemslevelTmps[13] + "~" + strItemslevelTmps[14] +
                                        "~" + strItemslevelTmps[15] + "~" + strItemslevelTmps[16] + "~" + strItemslevelTmps[17] + "~" + strItemslevelTmps[18] + "~" + strItemslevelTmps[19] +
                                        "~" + strItemslevelTmps[20] + "~" + strItemslevelTmps[21];
                                    continue;
                                }
                                if (b22Level == true)
                                {
                                    b22Level = false;
                                    i23Level = -1;
                                    strLevelPath = strItemslevelTmps[0] + "~" + strItemslevelTmps[1] + "~" + strItemslevelTmps[2] + "~" + strItemslevelTmps[3] + "~" + strItemslevelTmps[4] +
                                        "~" + strItemslevelTmps[5] + "~" + strItemslevelTmps[6] + "~" + strItemslevelTmps[7] + "~" + strItemslevelTmps[8] + "~" + strItemslevelTmps[9] +
                                        "~" + strItemslevelTmps[10] + "~" + strItemslevelTmps[11] + "~" + strItemslevelTmps[12] + "~" + strItemslevelTmps[13] + "~" + strItemslevelTmps[14] +
                                        "~" + strItemslevelTmps[15] + "~" + strItemslevelTmps[16] + "~" + strItemslevelTmps[17] + "~" + strItemslevelTmps[18] + "~" + strItemslevelTmps[19] +
                                        "~" + strItemslevelTmps[20];
                                    continue;
                                }
                                if (b21Level == true)
                                {
                                    b21Level = false;
                                    i22Level = -1;
                                    strLevelPath = strItemslevelTmps[0] + "~" + strItemslevelTmps[1] + "~" + strItemslevelTmps[2] + "~" + strItemslevelTmps[3] + "~" + strItemslevelTmps[4] +
                                        "~" + strItemslevelTmps[5] + "~" + strItemslevelTmps[6] + "~" + strItemslevelTmps[7] + "~" + strItemslevelTmps[8] + "~" + strItemslevelTmps[9] +
                                        "~" + strItemslevelTmps[10] + "~" + strItemslevelTmps[11] + "~" + strItemslevelTmps[12] + "~" + strItemslevelTmps[13] + "~" + strItemslevelTmps[14]
                                        + "~" + strItemslevelTmps[15] + "~" + strItemslevelTmps[16] + "~" + strItemslevelTmps[17] + "~" + strItemslevelTmps[18] + "~" + strItemslevelTmps[19];
                                    continue;
                                }
                                if (b20Level == true)
                                {
                                    b20Level = false;
                                    i21Level = -1;
                                    strLevelPath = strItemslevelTmps[0] + "~" + strItemslevelTmps[1] + "~" + strItemslevelTmps[2] + "~" + strItemslevelTmps[3] + "~" + strItemslevelTmps[4] +
                                        "~" + strItemslevelTmps[5] + "~" + strItemslevelTmps[6] + "~" + strItemslevelTmps[7] + "~" + strItemslevelTmps[8] + "~" + strItemslevelTmps[9] +
                                        "~" + strItemslevelTmps[10] + "~" + strItemslevelTmps[11] + "~" + strItemslevelTmps[12] + "~" + strItemslevelTmps[13] + "~" + strItemslevelTmps[14]
                                        + "~" + strItemslevelTmps[15] + "~" + strItemslevelTmps[16] + "~" + strItemslevelTmps[17] + "~" + strItemslevelTmps[18];
                                    continue;
                                }
                                if (b19Level == true)
                                {
                                    b19Level = false;
                                    i20Level = -1;
                                    strLevelPath = strItemslevelTmps[0] + "~" + strItemslevelTmps[1] + "~" + strItemslevelTmps[2] + "~" + strItemslevelTmps[3] + "~" + strItemslevelTmps[4] +
                                        "~" + strItemslevelTmps[5] + "~" + strItemslevelTmps[6] + "~" + strItemslevelTmps[7] + "~" + strItemslevelTmps[8] + "~" + strItemslevelTmps[9] +
                                        "~" + strItemslevelTmps[10] + "~" + strItemslevelTmps[11] + "~" + strItemslevelTmps[12] + "~" + strItemslevelTmps[13] + "~" + strItemslevelTmps[14]
                                        + "~" + strItemslevelTmps[15] + "~" + strItemslevelTmps[16] + "~" + strItemslevelTmps[17];
                                    continue;
                                }
                                if (b18Level == true)
                                {
                                    b18Level = false;
                                    i19Level = -1;
                                    strLevelPath = strItemslevelTmps[0] + "~" + strItemslevelTmps[1] + "~" + strItemslevelTmps[2] + "~" + strItemslevelTmps[3] + "~" + strItemslevelTmps[4] +
                                        "~" + strItemslevelTmps[5] + "~" + strItemslevelTmps[6] + "~" + strItemslevelTmps[7] + "~" + strItemslevelTmps[8] + "~" + strItemslevelTmps[9] +
                                        "~" + strItemslevelTmps[10] + "~" + strItemslevelTmps[11] + "~" + strItemslevelTmps[12] + "~" + strItemslevelTmps[13] + "~" + strItemslevelTmps[14]
                                        + "~" + strItemslevelTmps[15] + "~" + strItemslevelTmps[16];
                                    continue;
                                }
                                if (b17Level == true)
                                {
                                    b17Level = false;
                                    i18Level = -1;
                                    strLevelPath = strItemslevelTmps[0] + "~" + strItemslevelTmps[1] + "~" + strItemslevelTmps[2] + "~" + strItemslevelTmps[3] + "~" + strItemslevelTmps[4] +
                                        "~" + strItemslevelTmps[5] + "~" + strItemslevelTmps[6] + "~" + strItemslevelTmps[7] + "~" + strItemslevelTmps[8] + "~" + strItemslevelTmps[9] +
                                        "~" + strItemslevelTmps[10] + "~" + strItemslevelTmps[11] + "~" + strItemslevelTmps[12] + "~" + strItemslevelTmps[13] + "~" + strItemslevelTmps[14]
                                        + "~" + strItemslevelTmps[15];
                                    continue;
                                }
                                if (b16Level == true)
                                {
                                    b16Level = false;
                                    i17Level = -1;
                                    strLevelPath = strItemslevelTmps[0] + "~" + strItemslevelTmps[1] + "~" + strItemslevelTmps[2] + "~" + strItemslevelTmps[3] + "~" + strItemslevelTmps[4] +
                                        "~" + strItemslevelTmps[5] + "~" + strItemslevelTmps[6] + "~" + strItemslevelTmps[7] + "~" + strItemslevelTmps[8] + "~" + strItemslevelTmps[9] +
                                        "~" + strItemslevelTmps[10] + "~" + strItemslevelTmps[11] + "~" + strItemslevelTmps[12] + "~" + strItemslevelTmps[13] + "~" + strItemslevelTmps[14];
                                    continue;
                                }
                                if (b15Level == true)
                                {
                                    b15Level = false;
                                    i16Level = -1;
                                    strLevelPath = strItemslevelTmps[0] + "~" + strItemslevelTmps[1] + "~" + strItemslevelTmps[2] + "~" +
                                        strItemslevelTmps[3] + "~" + strItemslevelTmps[4] + "~" + strItemslevelTmps[5] + "~" +
                                        strItemslevelTmps[6] + "~" + strItemslevelTmps[7] + "~" + strItemslevelTmps[8] +
                                        "~" + strItemslevelTmps[9] + "~" + strItemslevelTmps[10] + "~" + strItemslevelTmps[11] +
                                        "~" + strItemslevelTmps[12] + "~" + strItemslevelTmps[13];
                                    continue;
                                }
                                if (b14Level == true)
                                {
                                    b14Level = false;
                                    i15Level = -1;
                                    strLevelPath = strItemslevelTmps[0] + "~" + strItemslevelTmps[1] + "~" + strItemslevelTmps[2] + "~" +
                                        strItemslevelTmps[3] + "~" + strItemslevelTmps[4] + "~" + strItemslevelTmps[5] + "~" +
                                        strItemslevelTmps[6] + "~" + strItemslevelTmps[7] + "~" + strItemslevelTmps[8] +
                                        "~" + strItemslevelTmps[9] + "~" + strItemslevelTmps[10] + "~" + strItemslevelTmps[11] +
                                        "~" + strItemslevelTmps[12];
                                    continue;
                                }
                                if (b13Level == true)
                                {
                                    b13Level = false;
                                    i14Level = -1;
                                    strLevelPath = strItemslevelTmps[0] + "~" + strItemslevelTmps[1] + "~" + strItemslevelTmps[2] + "~" +
                                        strItemslevelTmps[3] + "~" + strItemslevelTmps[4] + "~" + strItemslevelTmps[5] + "~" +
                                        strItemslevelTmps[6] + "~" + strItemslevelTmps[7] + "~" + strItemslevelTmps[8] +
                                        "~" + strItemslevelTmps[9] + "~" + strItemslevelTmps[10] + "~" + strItemslevelTmps[11];
                                    continue;
                                }
                                if (b12Level == true)
                                {
                                    b12Level = false;
                                    i13Level = -1;
                                    strLevelPath = strItemslevelTmps[0] + "~" + strItemslevelTmps[1] + "~" + strItemslevelTmps[2] + "~" +
                                        strItemslevelTmps[3] + "~" + strItemslevelTmps[4] + "~" + strItemslevelTmps[5] + "~" +
                                        strItemslevelTmps[6] + "~" + strItemslevelTmps[7] + "~" + strItemslevelTmps[8] +
                                        "~" + strItemslevelTmps[9] + "~" + strItemslevelTmps[10];
                                    continue;
                                }
                                if (b11Level == true)
                                {
                                    b11Level = false;
                                    i12Level = -1;
                                    strLevelPath = strItemslevelTmps[0] + "~" + strItemslevelTmps[1] + "~" + strItemslevelTmps[2] + "~" +
                                        strItemslevelTmps[3] + "~" + strItemslevelTmps[4] + "~" + strItemslevelTmps[5] + "~" +
                                        strItemslevelTmps[6] + "~" + strItemslevelTmps[7] + "~" + strItemslevelTmps[8] +
                                        "~" + strItemslevelTmps[9];
                                    continue;
                                }
                                if (b10Level == true)
                                {
                                    b10Level = false;
                                    i11Level = -1;
                                    strLevelPath = strItemslevelTmps[0] + "~" + strItemslevelTmps[1] + "~" + strItemslevelTmps[2] + "~" +
                                        strItemslevelTmps[3] + "~" + strItemslevelTmps[4] + "~" + strItemslevelTmps[5] + "~" +
                                        strItemslevelTmps[6] + "~" + strItemslevelTmps[7] + "~" + strItemslevelTmps[8];
                                    continue;
                                }
                                if (bNinthLevel == true)
                                {
                                    bNinthLevel = false;
                                    i10Level = -1;
                                    strLevelPath = strItemslevelTmps[0] + "~" + strItemslevelTmps[1] + "~" + strItemslevelTmps[2] + "~" +
                                        strItemslevelTmps[3] + "~" + strItemslevelTmps[4] + "~" + strItemslevelTmps[5] + "~" +
                                        strItemslevelTmps[6] + "~" + strItemslevelTmps[7];
                                    continue;
                                }
                                if (bEighthLevel == true)
                                {
                                    bEighthLevel = false;
                                    //if (iEndCharCount > 1 && iEndChrIndex < iEndCharCount)
                                    iNinthLevel = -1;
                                    strLevelPath = strItemslevelTmps[0] + "~" + strItemslevelTmps[1] + "~" + strItemslevelTmps[2] + "~" +
                                        strItemslevelTmps[3] + "~" + strItemslevelTmps[4] + "~" + strItemslevelTmps[5] + "~" +
                                        strItemslevelTmps[6];
                                    continue;
                                }
                                if (bSeventhLevel == true)
                                {
                                    bSeventhLevel = false;
                                    iEighthLevel = -1;
                                    strLevelPath = strItemslevelTmps[0] + "~" + strItemslevelTmps[1] + "~" + strItemslevelTmps[2] + "~" +
                                        strItemslevelTmps[3] + "~" + strItemslevelTmps[4] + "~" + strItemslevelTmps[5];
                                    continue;
                                }
                                if (bSixthLevel == true)
                                {
                                    bSixthLevel = false;
                                    iSeventhLevel = -1;
                                    strLevelPath = strItemslevelTmps[0] + "~" + strItemslevelTmps[1] + "~" + strItemslevelTmps[2] + "~" +
                                        strItemslevelTmps[3] + "~" + strItemslevelTmps[4];
                                    continue;
                                }
                                if (bFifthLevel == true)
                                {
                                    bFifthLevel = false;
                                    //if (iEndCharCount > 1 && iEndChrIndex < iEndCharCount)
                                    iSixthLevel = -1;
                                    strLevelPath = strItemslevelTmps[0] + "~" + strItemslevelTmps[1] + "~" + strItemslevelTmps[2] + "~" +
                                        strItemslevelTmps[3];
                                    continue;
                                }
                                if (bFourthLevel == true)
                                {
                                    bFourthLevel = false;
                                    //if (iEndCharCount > 1 && iEndChrIndex < iEndCharCount)
                                    iFifthLevel = -1;
                                    strLevelPath = strItemslevelTmps[0] + "~" + strItemslevelTmps[1] + "~" + strItemslevelTmps[2];
                                    continue;
                                }
                                if (bThirdLevel == true)
                                {
                                    bThirdLevel = false;
                                    //if (iEndCharCount > 1 && iEndChrIndex < iEndCharCount)
                                    iFourthLevel = -1;

                                    strLevelPath = strItemslevelTmps[0] + "~" + strItemslevelTmps[1];
                                    continue;
                                }
                                if (bSecondlevel == true)
                                {
                                    bSecondlevel = false;
                                    //if (iEndCharCount > 1 && iEndChrIndex < iEndCharCount)
                                    iThirdLevel = -1;
                                    strLevelPath = strItemslevelTmps[0];
                                    continue;
                                }
                            }
                        }
                        continue;

                        #endregion "GetListEndChar Result"
                    }
                    else
                        return null;

                    #endregion "Other"
                }
                iFlag++;
            }

            return secsTrx;
        }

        private static bool GetStreamFunction(string strValue, ref int iStream, ref int iFunction, ref bool bWFlag, ref string DeviceID)
        {
            try
            {
                strValue = strValue.Replace(" ", "");
                strValue = strValue.Replace("\r", "");
                strValue = strValue.Replace("\n", "");

                string strStreamResult = "", strFunctionResult = "";
                if (strValue.IndexOf("S") < 0)
                {
                    return false;
                }
                if (strValue.IndexOf("F") < 0)
                {
                    return false;
                }
                strStreamResult = strValue.Substring(strValue.IndexOf("S") + 1, strValue.IndexOf("F") - strValue.IndexOf("S") - 1);
                if (strValue.IndexOf("W") > 0 || strValue.IndexOf("w") > 0)
                {
                    bWFlag = true;
                    strFunctionResult = strValue.Substring(strValue.IndexOf("F") + 1, strValue.IndexOf("W") - strValue.IndexOf("F") - 1).Trim();
                }
                else if (strValue.IndexOf("w") > 0)
                {
                    bWFlag = true;
                    strFunctionResult = strValue.Substring(strValue.IndexOf("F") + 1, strValue.IndexOf("w") - strValue.IndexOf("F") - 1).Trim();
                }
                else
                {
                    if (strValue.IndexOf("D=") > 0 || strValue.IndexOf("d=") > 0)
                    {
                        bWFlag = false;
                        strFunctionResult = strValue.Substring(strValue.IndexOf("F") + 1, strValue.IndexOf("D=") - strValue.IndexOf("F") - 1).Trim();
                    }
                    else
                    {
                        bWFlag = false;
                        strFunctionResult = strValue.Substring(strValue.IndexOf("F") + 1).Trim();
                    }
                }
                iStream = int.Parse(strStreamResult);
                iFunction = int.Parse(strFunctionResult);
                if (strValue.IndexOf("D=") > 0 || strValue.IndexOf("d=") > 0)
                {
                    DeviceID = strValue.Substring(strValue.IndexOf("=") + 1, strValue.Length - strValue.IndexOf("=") - 1);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool AddItemToTrx(string strValue, string strListLevel, ref SECSTransaction secsTrx, ref string strRet)
        {
            Boolean bResult = true;
            Boolean bFlag = false;
            string[] strItems = strValue.Trim().Split(' ');
            string[] strLevelItems = strListLevel.Split('~');
            string strDataType = strItems[0].Trim().ToUpper();
            string strResultValue = "";
            if (strValue.Length >= 2 && strValue.Substring(0, 2) == "A ")
            {
                strItems = new string[2];
                strItems[0] = "A ";
                strItems[1] = strValue.Substring(2, strValue.Length - 2);
                strDataType = "A";
            }
            else
            {
                strItems = strValue.Trim().Split(' ');
                strLevelItems = strListLevel.Split('~');
                strResultValue = "";
            }
            strDataType = strItems[0].Trim().ToUpper();
            IFormat objF = null;
            if (strDataType != "L" && strDataType != "LIST" &&
                strDataType != "ASCII" && strDataType != "A" &&
                strDataType != "BOOLEAN" && strDataType != "BOOL" &&
                strDataType != "BINARY" && strDataType != "B" &&
                strDataType != "I1" && strDataType != "I2" &&
                strDataType != "I4" && strDataType != "I8" &&
                strDataType != "U1" && strDataType != "U2" &&
                strDataType != "U4" && strDataType != "U8" &&
                strDataType != "F4" && strDataType != "F8" &&
                strDataType != "I1" && strDataType != "I2" &&
                strDataType != "J" && strDataType != "JIS"
                )
            {
                if (strValue.Trim().IndexOf(">") > 0 && strItems.Length == 1)
                {
                    string strTmp = strValue.Trim().Substring(0, strValue.Trim().IndexOf(">")).ToUpper();
                    strDataType = strTmp;
                    bFlag = true;
                }
            }

            switch (strDataType.ToUpper())
            {
                case "LIST":
                case "L":
                    ListFormat lf = new ListFormat();
                    if (strListLevel == "")
                    {
                        secsTrx.add(lf);
                        bResult = true;
                        return bResult;
                    }

                    bResult = AddDataToTransaction(lf, strLevelItems, ref secsTrx, ref strRet);

                    return bResult;

                case "A":
                case "ASCII":
                    objF = new AsciiFormat();
                    break;

                case "BOOLEAN":
                case "BOOL":
                    objF = new BooleanFormat();
                    break;

                case "B":
                case "BINARY":
                    objF = new BinaryFormat();
                    break;

                case "I1":
                    objF = new Int1Format();
                    break;

                case "I2":
                    objF = new Int2Format();
                    break;

                case "I4":
                    objF = new Int4Format();
                    break;

                case "I8":
                    objF = new Int8Format();
                    break;

                case "U1":
                    objF = new Uint1Format();
                    break;

                case "U2":
                    objF = new Uint2Format();
                    break;

                case "U4":
                    objF = new Uint4Format();
                    break;

                case "U8":
                    objF = new Uint8Format();
                    break;

                case "F4":
                    objF = new Float4Format();
                    break;

                case "F8":
                    objF = new Float8Format();
                    break;

                case "J":
                case "JIS":
                    objF = new JISFormat();
                    break;

                default:
                    strRet = "data type error";
                    bResult = false;
                    return false;
            }

            if (strListLevel == "" && strDataType.ToUpper() != "L")
            {
                bResult = GetItemValue(strDataType, strValue, ref strResultValue, ref strRet);
                if (bResult == true)
                {
                    objF.Value = strResultValue;
                    bResult = AddDataToTransaction(objF, strLevelItems, ref secsTrx, ref strRet);
                    return bResult;
                }
                else
                    return bResult;
            }

            if (bFlag == true)
            {
                //处理值是空的
                objF.Value = System.String.Empty;
                bResult = AddDataToTransaction(objF, strLevelItems, ref secsTrx, ref strRet);
                return bResult;
            }
            bResult = GetItemValue(strDataType, strValue, ref strResultValue, ref strRet);
            if (bResult == true)
            {
                if (strValue != "L")
                {
                    objF.Value = strResultValue;
                    bResult = AddDataToTransaction(objF, strLevelItems, ref secsTrx, ref strRet);
                }
            }
            else
                return bResult;

            return bResult;
        }

        private static int GetListEndChar(string strDataValue, ref bool isListFlag)
        {
            int iResult = 0;
            isListFlag = false;
            string strTmp = strDataValue;
            if (strTmp.IndexOf("/*") > 0)
            {
                strTmp = strTmp.Substring(0, strTmp.IndexOf("/*")) +
                    strTmp.Substring(strTmp.IndexOf("*/"));
            }

            string[] strItems = strTmp.Split('>');
            //消息结束
            if (strItems[strItems.Length - 1].Trim().Length > 0)
            {
                if (strItems[strItems.Length - 1].Trim().Substring(0, 1) == ".")
                {
                    return -100;
                }
            }

            if (strItems.Length > 2)
            {
                if (strDataValue.Substring(0, 1) != "L")
                    iResult = strItems.Length - 1;
                else
                {
                    isListFlag = true;
                    iResult = strItems.Length - 1;
                }
            }
            else
            {
                if (strDataValue.Substring(0, 1) == "L")
                {
                    isListFlag = true;
                    iResult = strItems.Length - 1;
                }
            }
            return iResult;
        }

        private static bool CheckSECSItem(string strValue)
        {
            if (strValue.Trim().IndexOf(' ') < 1)
                return false;
            string strDataType = strValue.Trim().Substring(0, strValue.Trim().IndexOf(' '));

            if (strDataType != "L" && strDataType != "LIST" &&
                strDataType != "ASCII" && strDataType != "A" &&
                strDataType != "BOOLEAN" && strDataType != "BOOL" &&
                strDataType != "BINARY" && strDataType != "B" &&
                strDataType != "I1" && strDataType != "I2" &&
                strDataType != "I4" && strDataType != "I8" &&
                strDataType != "U1" && strDataType != "U2" &&
                strDataType != "U4" && strDataType != "U8" &&
                strDataType != "F4" && strDataType != "F8" &&
                strDataType != "I1" && strDataType != "I2" &&
                strDataType != "J" && strDataType != "JIS")
                return false;
            return true;
        }

        private static int GetListEndChar(string strDataValue)
        {
            int iResult = 0;
            string strTmp = strDataValue;
            if (strTmp.IndexOf("/*") > 0)
            {
                strTmp = strTmp.Substring(0, strTmp.IndexOf("/*")) +
                    strTmp.Substring(strTmp.IndexOf("*/"));
            }

            string[] strItems = null;
            if (strDataValue.Substring(0, 1) == "A")
            {
                string strTmp1 = "";
                if (strTmp.Contains("'>") == false)
                {
                    strTmp1 = strTmp;
                    strItems = strTmp1.Split('>');
                }
                else
                {
                    strTmp1 = strTmp.Substring(0, strTmp.IndexOf("'>"));
                    strTmp1 = strTmp1.Replace(">", "");
                    strTmp1 = strTmp1 + strTmp.Substring(strTmp.IndexOf("'>"), strTmp.Length - strTmp.IndexOf("'>"));
                    strItems = strTmp1.Split('>');
                }
            }
            else
                strItems = strTmp.Split('>');
            //消息结束
            if (strItems[strItems.Length - 1].Trim().Length > 0)
            {
                if (strItems[strItems.Length - 1].Trim().Substring(0, 1) == ".")
                {
                    return -100;
                }
            }
            if (strItems.Length > 2)
            {
                if (strDataValue.Substring(0, 1) != "L")
                    iResult = strItems.Length - 2;
                else
                    iResult = strItems.Length - 1;
            }
            else
            {
                if (strDataValue.Substring(0, 1) == "L")
                    iResult = strItems.Length - 1;
            }
            return iResult;
        }

        private static bool AddDataToTransaction(IFormat objF, string[] strLevelItems, ref SECSTransaction secsTrx, ref string strRet)
        {
            Boolean bResult = true;
            switch (strLevelItems.Length)
            {
                case 1:
                    if (strLevelItems[0].Length == 0 && secsTrx.Children.Count == 0)
                        secsTrx.Children.Add(objF);
                    else
                        secsTrx.Children[int.Parse(strLevelItems[0])].add(objF);

                    break;

                case 2:
                    secsTrx.Children[int.Parse(strLevelItems[0])].Children[int.Parse(strLevelItems[1])].add(objF);
                    break;

                case 3:
                    secsTrx.Children[int.Parse(strLevelItems[0])]
                        .Children[int.Parse(strLevelItems[1])]
                        .Children[int.Parse(strLevelItems[2])]
                        .add(objF);
                    break;

                case 4:
                    secsTrx.Children[int.Parse(strLevelItems[0])]
                        .Children[int.Parse(strLevelItems[1])]
                        .Children[int.Parse(strLevelItems[2])]
                        .Children[int.Parse(strLevelItems[3])]
                        .add(objF);
                    break;

                case 5:
                    secsTrx.Children[int.Parse(strLevelItems[0])]
                        .Children[int.Parse(strLevelItems[1])]
                        .Children[int.Parse(strLevelItems[2])]
                        .Children[int.Parse(strLevelItems[3])]
                        .Children[int.Parse(strLevelItems[4])]
                        .add(objF);
                    break;

                case 6:
                    secsTrx.Children[int.Parse(strLevelItems[0])]
                        .Children[int.Parse(strLevelItems[1])]
                        .Children[int.Parse(strLevelItems[2])]
                        .Children[int.Parse(strLevelItems[3])]
                        .Children[int.Parse(strLevelItems[4])]
                        .Children[int.Parse(strLevelItems[5])]
                        .add(objF);
                    break;

                case 7:
                    secsTrx.Children[int.Parse(strLevelItems[0])]
                        .Children[int.Parse(strLevelItems[1])]
                        .Children[int.Parse(strLevelItems[2])]
                        .Children[int.Parse(strLevelItems[3])]
                        .Children[int.Parse(strLevelItems[4])]
                        .Children[int.Parse(strLevelItems[5])]
                        .Children[int.Parse(strLevelItems[6])]
                        .add(objF);
                    break;

                case 8:
                    secsTrx.Children[int.Parse(strLevelItems[0])]
                        .Children[int.Parse(strLevelItems[1])]
                        .Children[int.Parse(strLevelItems[2])]
                        .Children[int.Parse(strLevelItems[3])]
                        .Children[int.Parse(strLevelItems[4])]
                        .Children[int.Parse(strLevelItems[5])]
                        .Children[int.Parse(strLevelItems[6])]
                        .Children[int.Parse(strLevelItems[7])]
                        .add(objF);
                    break;

                case 9:
                    secsTrx.Children[int.Parse(strLevelItems[0])]
                        .Children[int.Parse(strLevelItems[1])]
                        .Children[int.Parse(strLevelItems[2])]
                        .Children[int.Parse(strLevelItems[3])]
                        .Children[int.Parse(strLevelItems[4])]
                        .Children[int.Parse(strLevelItems[5])]
                        .Children[int.Parse(strLevelItems[6])]
                        .Children[int.Parse(strLevelItems[7])]
                        .Children[int.Parse(strLevelItems[8])]
                        .add(objF);
                    break;

                case 10:
                    secsTrx.Children[int.Parse(strLevelItems[0])]
                        .Children[int.Parse(strLevelItems[1])]
                        .Children[int.Parse(strLevelItems[2])]
                        .Children[int.Parse(strLevelItems[3])]
                        .Children[int.Parse(strLevelItems[4])]
                        .Children[int.Parse(strLevelItems[5])]
                        .Children[int.Parse(strLevelItems[6])]
                        .Children[int.Parse(strLevelItems[7])]
                        .Children[int.Parse(strLevelItems[8])]
                        .Children[int.Parse(strLevelItems[9])]
                        .add(objF);
                    break;

                case 11:
                    secsTrx.Children[int.Parse(strLevelItems[0])]
                        .Children[int.Parse(strLevelItems[1])]
                        .Children[int.Parse(strLevelItems[2])]
                        .Children[int.Parse(strLevelItems[3])]
                        .Children[int.Parse(strLevelItems[4])]
                        .Children[int.Parse(strLevelItems[5])]
                        .Children[int.Parse(strLevelItems[6])]
                        .Children[int.Parse(strLevelItems[7])]
                        .Children[int.Parse(strLevelItems[8])]
                        .Children[int.Parse(strLevelItems[9])]
                        .Children[int.Parse(strLevelItems[10])]
                        .add(objF);
                    break;

                case 12:
                    secsTrx.Children[int.Parse(strLevelItems[0])]
                        .Children[int.Parse(strLevelItems[1])]
                        .Children[int.Parse(strLevelItems[2])]
                        .Children[int.Parse(strLevelItems[3])]
                        .Children[int.Parse(strLevelItems[4])]
                        .Children[int.Parse(strLevelItems[5])]
                        .Children[int.Parse(strLevelItems[6])]
                        .Children[int.Parse(strLevelItems[7])]
                        .Children[int.Parse(strLevelItems[8])]
                        .Children[int.Parse(strLevelItems[9])]
                        .Children[int.Parse(strLevelItems[10])]
                        .Children[int.Parse(strLevelItems[11])]
                        .add(objF);
                    break;

                case 13:
                    secsTrx.Children[int.Parse(strLevelItems[0])]
                        .Children[int.Parse(strLevelItems[1])]
                        .Children[int.Parse(strLevelItems[2])]
                        .Children[int.Parse(strLevelItems[3])]
                        .Children[int.Parse(strLevelItems[4])]
                        .Children[int.Parse(strLevelItems[5])]
                        .Children[int.Parse(strLevelItems[6])]
                        .Children[int.Parse(strLevelItems[7])]
                        .Children[int.Parse(strLevelItems[8])]
                        .Children[int.Parse(strLevelItems[9])]
                        .Children[int.Parse(strLevelItems[10])]
                        .Children[int.Parse(strLevelItems[11])]
                        .Children[int.Parse(strLevelItems[12])]
                        .add(objF);
                    break;

                case 14:
                    secsTrx.Children[int.Parse(strLevelItems[0])]
                        .Children[int.Parse(strLevelItems[1])]
                        .Children[int.Parse(strLevelItems[2])]
                        .Children[int.Parse(strLevelItems[3])]
                        .Children[int.Parse(strLevelItems[4])]
                        .Children[int.Parse(strLevelItems[5])]
                        .Children[int.Parse(strLevelItems[6])]
                        .Children[int.Parse(strLevelItems[7])]
                        .Children[int.Parse(strLevelItems[8])]
                        .Children[int.Parse(strLevelItems[9])]
                        .Children[int.Parse(strLevelItems[10])]
                        .Children[int.Parse(strLevelItems[11])]
                        .Children[int.Parse(strLevelItems[12])]
                        .Children[int.Parse(strLevelItems[13])]
                        .add(objF);
                    break;

                case 15:
                    secsTrx.Children[int.Parse(strLevelItems[0])]
                        .Children[int.Parse(strLevelItems[1])]
                        .Children[int.Parse(strLevelItems[2])]
                        .Children[int.Parse(strLevelItems[3])]
                        .Children[int.Parse(strLevelItems[4])]
                        .Children[int.Parse(strLevelItems[5])]
                        .Children[int.Parse(strLevelItems[6])]
                        .Children[int.Parse(strLevelItems[7])]
                        .Children[int.Parse(strLevelItems[8])]
                        .Children[int.Parse(strLevelItems[9])]
                        .Children[int.Parse(strLevelItems[10])]
                        .Children[int.Parse(strLevelItems[11])]
                        .Children[int.Parse(strLevelItems[12])]
                        .Children[int.Parse(strLevelItems[13])]
                        .Children[int.Parse(strLevelItems[14])]
                        .add(objF);
                    break;

                case 16:
                    secsTrx.Children[int.Parse(strLevelItems[0])]
                        .Children[int.Parse(strLevelItems[1])]
                        .Children[int.Parse(strLevelItems[2])]
                        .Children[int.Parse(strLevelItems[3])]
                        .Children[int.Parse(strLevelItems[4])]
                        .Children[int.Parse(strLevelItems[5])]
                        .Children[int.Parse(strLevelItems[6])]
                        .Children[int.Parse(strLevelItems[7])]
                        .Children[int.Parse(strLevelItems[8])]
                        .Children[int.Parse(strLevelItems[9])]
                        .Children[int.Parse(strLevelItems[10])]
                        .Children[int.Parse(strLevelItems[11])]
                        .Children[int.Parse(strLevelItems[12])]
                        .Children[int.Parse(strLevelItems[13])]
                        .Children[int.Parse(strLevelItems[14])]
                        .Children[int.Parse(strLevelItems[15])]
                        .add(objF);
                    break;

                case 17:
                    secsTrx.Children[int.Parse(strLevelItems[0])]
                        .Children[int.Parse(strLevelItems[1])]
                        .Children[int.Parse(strLevelItems[2])]
                        .Children[int.Parse(strLevelItems[3])]
                        .Children[int.Parse(strLevelItems[4])]
                        .Children[int.Parse(strLevelItems[5])]
                        .Children[int.Parse(strLevelItems[6])]
                        .Children[int.Parse(strLevelItems[7])]
                        .Children[int.Parse(strLevelItems[8])]
                        .Children[int.Parse(strLevelItems[9])]
                        .Children[int.Parse(strLevelItems[10])]
                        .Children[int.Parse(strLevelItems[11])]
                        .Children[int.Parse(strLevelItems[12])]
                        .Children[int.Parse(strLevelItems[13])]
                        .Children[int.Parse(strLevelItems[14])]
                        .Children[int.Parse(strLevelItems[15])]
                        .Children[int.Parse(strLevelItems[16])]
                        .add(objF);
                    break;

                case 18:
                    secsTrx.Children[int.Parse(strLevelItems[0])]
                        .Children[int.Parse(strLevelItems[1])]
                        .Children[int.Parse(strLevelItems[2])]
                        .Children[int.Parse(strLevelItems[3])]
                        .Children[int.Parse(strLevelItems[4])]
                        .Children[int.Parse(strLevelItems[5])]
                        .Children[int.Parse(strLevelItems[6])]
                        .Children[int.Parse(strLevelItems[7])]
                        .Children[int.Parse(strLevelItems[8])]
                        .Children[int.Parse(strLevelItems[9])]
                        .Children[int.Parse(strLevelItems[10])]
                        .Children[int.Parse(strLevelItems[11])]
                        .Children[int.Parse(strLevelItems[12])]
                        .Children[int.Parse(strLevelItems[13])]
                        .Children[int.Parse(strLevelItems[14])]
                        .Children[int.Parse(strLevelItems[15])]
                        .Children[int.Parse(strLevelItems[17])]
                        .add(objF);
                    break;

                case 19:
                    secsTrx.Children[int.Parse(strLevelItems[0])]
                        .Children[int.Parse(strLevelItems[1])]
                        .Children[int.Parse(strLevelItems[2])]
                        .Children[int.Parse(strLevelItems[3])]
                        .Children[int.Parse(strLevelItems[4])]
                        .Children[int.Parse(strLevelItems[5])]
                        .Children[int.Parse(strLevelItems[6])]
                        .Children[int.Parse(strLevelItems[7])]
                        .Children[int.Parse(strLevelItems[8])]
                        .Children[int.Parse(strLevelItems[9])]
                        .Children[int.Parse(strLevelItems[10])]
                        .Children[int.Parse(strLevelItems[11])]
                        .Children[int.Parse(strLevelItems[12])]
                        .Children[int.Parse(strLevelItems[13])]
                        .Children[int.Parse(strLevelItems[14])]
                        .Children[int.Parse(strLevelItems[15])]
                        .Children[int.Parse(strLevelItems[17])]
                        .Children[int.Parse(strLevelItems[18])]
                        .add(objF);
                    break;

                case 20:
                    secsTrx.Children[int.Parse(strLevelItems[0])]
                        .Children[int.Parse(strLevelItems[1])]
                        .Children[int.Parse(strLevelItems[2])]
                        .Children[int.Parse(strLevelItems[3])]
                        .Children[int.Parse(strLevelItems[4])]
                        .Children[int.Parse(strLevelItems[5])]
                        .Children[int.Parse(strLevelItems[6])]
                        .Children[int.Parse(strLevelItems[7])]
                        .Children[int.Parse(strLevelItems[8])]
                        .Children[int.Parse(strLevelItems[9])]
                        .Children[int.Parse(strLevelItems[10])]
                        .Children[int.Parse(strLevelItems[11])]
                        .Children[int.Parse(strLevelItems[12])]
                        .Children[int.Parse(strLevelItems[13])]
                        .Children[int.Parse(strLevelItems[14])]
                        .Children[int.Parse(strLevelItems[15])]
                        .Children[int.Parse(strLevelItems[17])]
                        .Children[int.Parse(strLevelItems[18])]
                        .Children[int.Parse(strLevelItems[19])]
                        .add(objF);
                    break;

                case 21:
                    secsTrx.Children[int.Parse(strLevelItems[0])]
                        .Children[int.Parse(strLevelItems[1])]
                        .Children[int.Parse(strLevelItems[2])]
                        .Children[int.Parse(strLevelItems[3])]
                        .Children[int.Parse(strLevelItems[4])]
                        .Children[int.Parse(strLevelItems[5])]
                        .Children[int.Parse(strLevelItems[6])]
                        .Children[int.Parse(strLevelItems[7])]
                        .Children[int.Parse(strLevelItems[8])]
                        .Children[int.Parse(strLevelItems[9])]
                        .Children[int.Parse(strLevelItems[10])]
                        .Children[int.Parse(strLevelItems[11])]
                        .Children[int.Parse(strLevelItems[12])]
                        .Children[int.Parse(strLevelItems[13])]
                        .Children[int.Parse(strLevelItems[14])]
                        .Children[int.Parse(strLevelItems[15])]
                        .Children[int.Parse(strLevelItems[17])]
                        .Children[int.Parse(strLevelItems[18])]
                        .Children[int.Parse(strLevelItems[20])]
                        .add(objF);
                    break;

                case 22:
                    secsTrx.Children[int.Parse(strLevelItems[0])]
                        .Children[int.Parse(strLevelItems[1])]
                        .Children[int.Parse(strLevelItems[2])]
                        .Children[int.Parse(strLevelItems[3])]
                        .Children[int.Parse(strLevelItems[4])]
                        .Children[int.Parse(strLevelItems[5])]
                        .Children[int.Parse(strLevelItems[6])]
                        .Children[int.Parse(strLevelItems[7])]
                        .Children[int.Parse(strLevelItems[8])]
                        .Children[int.Parse(strLevelItems[9])]
                        .Children[int.Parse(strLevelItems[10])]
                        .Children[int.Parse(strLevelItems[11])]
                        .Children[int.Parse(strLevelItems[12])]
                        .Children[int.Parse(strLevelItems[13])]
                        .Children[int.Parse(strLevelItems[14])]
                        .Children[int.Parse(strLevelItems[15])]
                        .Children[int.Parse(strLevelItems[17])]
                        .Children[int.Parse(strLevelItems[18])]
                        .Children[int.Parse(strLevelItems[20])]
                        .Children[int.Parse(strLevelItems[21])]
                        .add(objF);
                    break;

                case 23:
                    secsTrx.Children[int.Parse(strLevelItems[0])]
                        .Children[int.Parse(strLevelItems[1])]
                        .Children[int.Parse(strLevelItems[2])]
                        .Children[int.Parse(strLevelItems[3])]
                        .Children[int.Parse(strLevelItems[4])]
                        .Children[int.Parse(strLevelItems[5])]
                        .Children[int.Parse(strLevelItems[6])]
                        .Children[int.Parse(strLevelItems[7])]
                        .Children[int.Parse(strLevelItems[8])]
                        .Children[int.Parse(strLevelItems[9])]
                        .Children[int.Parse(strLevelItems[10])]
                        .Children[int.Parse(strLevelItems[11])]
                        .Children[int.Parse(strLevelItems[12])]
                        .Children[int.Parse(strLevelItems[13])]
                        .Children[int.Parse(strLevelItems[14])]
                        .Children[int.Parse(strLevelItems[15])]
                        .Children[int.Parse(strLevelItems[17])]
                        .Children[int.Parse(strLevelItems[18])]
                        .Children[int.Parse(strLevelItems[20])]
                        .Children[int.Parse(strLevelItems[21])]
                        .Children[int.Parse(strLevelItems[22])]
                        .add(objF);
                    break;

                case 24:
                    secsTrx.Children[int.Parse(strLevelItems[0])]
                        .Children[int.Parse(strLevelItems[1])]
                        .Children[int.Parse(strLevelItems[2])]
                        .Children[int.Parse(strLevelItems[3])]
                        .Children[int.Parse(strLevelItems[4])]
                        .Children[int.Parse(strLevelItems[5])]
                        .Children[int.Parse(strLevelItems[6])]
                        .Children[int.Parse(strLevelItems[7])]
                        .Children[int.Parse(strLevelItems[8])]
                        .Children[int.Parse(strLevelItems[9])]
                        .Children[int.Parse(strLevelItems[10])]
                        .Children[int.Parse(strLevelItems[11])]
                        .Children[int.Parse(strLevelItems[12])]
                        .Children[int.Parse(strLevelItems[13])]
                        .Children[int.Parse(strLevelItems[14])]
                        .Children[int.Parse(strLevelItems[15])]
                        .Children[int.Parse(strLevelItems[17])]
                        .Children[int.Parse(strLevelItems[18])]
                        .Children[int.Parse(strLevelItems[20])]
                        .Children[int.Parse(strLevelItems[21])]
                        .Children[int.Parse(strLevelItems[22])]
                        .Children[int.Parse(strLevelItems[23])]
                        .add(objF);
                    break;

                case 25:
                    secsTrx.Children[int.Parse(strLevelItems[0])]
                        .Children[int.Parse(strLevelItems[1])]
                        .Children[int.Parse(strLevelItems[2])]
                        .Children[int.Parse(strLevelItems[3])]
                        .Children[int.Parse(strLevelItems[4])]
                        .Children[int.Parse(strLevelItems[5])]
                        .Children[int.Parse(strLevelItems[6])]
                        .Children[int.Parse(strLevelItems[7])]
                        .Children[int.Parse(strLevelItems[8])]
                        .Children[int.Parse(strLevelItems[9])]
                        .Children[int.Parse(strLevelItems[10])]
                        .Children[int.Parse(strLevelItems[11])]
                        .Children[int.Parse(strLevelItems[12])]
                        .Children[int.Parse(strLevelItems[13])]
                        .Children[int.Parse(strLevelItems[14])]
                        .Children[int.Parse(strLevelItems[15])]
                        .Children[int.Parse(strLevelItems[17])]
                        .Children[int.Parse(strLevelItems[18])]
                        .Children[int.Parse(strLevelItems[20])]
                        .Children[int.Parse(strLevelItems[21])]
                        .Children[int.Parse(strLevelItems[22])]
                        .Children[int.Parse(strLevelItems[23])]
                        .Children[int.Parse(strLevelItems[24])]
                        .add(objF);
                    break;

                case 26:
                    secsTrx.Children[int.Parse(strLevelItems[0])]
                        .Children[int.Parse(strLevelItems[1])]
                        .Children[int.Parse(strLevelItems[2])]
                        .Children[int.Parse(strLevelItems[3])]
                        .Children[int.Parse(strLevelItems[4])]
                        .Children[int.Parse(strLevelItems[5])]
                        .Children[int.Parse(strLevelItems[6])]
                        .Children[int.Parse(strLevelItems[7])]
                        .Children[int.Parse(strLevelItems[8])]
                        .Children[int.Parse(strLevelItems[9])]
                        .Children[int.Parse(strLevelItems[10])]
                        .Children[int.Parse(strLevelItems[11])]
                        .Children[int.Parse(strLevelItems[12])]
                        .Children[int.Parse(strLevelItems[13])]
                        .Children[int.Parse(strLevelItems[14])]
                        .Children[int.Parse(strLevelItems[15])]
                        .Children[int.Parse(strLevelItems[17])]
                        .Children[int.Parse(strLevelItems[18])]
                        .Children[int.Parse(strLevelItems[20])]
                        .Children[int.Parse(strLevelItems[21])]
                        .Children[int.Parse(strLevelItems[22])]
                        .Children[int.Parse(strLevelItems[23])]
                        .Children[int.Parse(strLevelItems[24])]
                        .Children[int.Parse(strLevelItems[25])]
                        .add(objF);
                    break;

                case 27:
                    secsTrx.Children[int.Parse(strLevelItems[0])]
                        .Children[int.Parse(strLevelItems[1])]
                        .Children[int.Parse(strLevelItems[2])]
                        .Children[int.Parse(strLevelItems[3])]
                        .Children[int.Parse(strLevelItems[4])]
                        .Children[int.Parse(strLevelItems[5])]
                        .Children[int.Parse(strLevelItems[6])]
                        .Children[int.Parse(strLevelItems[7])]
                        .Children[int.Parse(strLevelItems[8])]
                        .Children[int.Parse(strLevelItems[9])]
                        .Children[int.Parse(strLevelItems[10])]
                        .Children[int.Parse(strLevelItems[11])]
                        .Children[int.Parse(strLevelItems[12])]
                        .Children[int.Parse(strLevelItems[13])]
                        .Children[int.Parse(strLevelItems[14])]
                        .Children[int.Parse(strLevelItems[15])]
                        .Children[int.Parse(strLevelItems[17])]
                        .Children[int.Parse(strLevelItems[18])]
                        .Children[int.Parse(strLevelItems[20])]
                        .Children[int.Parse(strLevelItems[21])]
                        .Children[int.Parse(strLevelItems[22])]
                        .Children[int.Parse(strLevelItems[23])]
                        .Children[int.Parse(strLevelItems[24])]
                        .Children[int.Parse(strLevelItems[25])]
                        .Children[int.Parse(strLevelItems[26])]
                        .add(objF);
                    break;

                case 28:
                    secsTrx.Children[int.Parse(strLevelItems[0])]
                        .Children[int.Parse(strLevelItems[1])]
                        .Children[int.Parse(strLevelItems[2])]
                        .Children[int.Parse(strLevelItems[3])]
                        .Children[int.Parse(strLevelItems[4])]
                        .Children[int.Parse(strLevelItems[5])]
                        .Children[int.Parse(strLevelItems[6])]
                        .Children[int.Parse(strLevelItems[7])]
                        .Children[int.Parse(strLevelItems[8])]
                        .Children[int.Parse(strLevelItems[9])]
                        .Children[int.Parse(strLevelItems[10])]
                        .Children[int.Parse(strLevelItems[11])]
                        .Children[int.Parse(strLevelItems[12])]
                        .Children[int.Parse(strLevelItems[13])]
                        .Children[int.Parse(strLevelItems[14])]
                        .Children[int.Parse(strLevelItems[15])]
                        .Children[int.Parse(strLevelItems[17])]
                        .Children[int.Parse(strLevelItems[18])]
                        .Children[int.Parse(strLevelItems[20])]
                        .Children[int.Parse(strLevelItems[21])]
                        .Children[int.Parse(strLevelItems[22])]
                        .Children[int.Parse(strLevelItems[23])]
                        .Children[int.Parse(strLevelItems[24])]
                        .Children[int.Parse(strLevelItems[25])]
                        .Children[int.Parse(strLevelItems[26])]
                        .Children[int.Parse(strLevelItems[27])]
                        .add(objF);
                    break;

                case 29:
                    secsTrx.Children[int.Parse(strLevelItems[0])]
                        .Children[int.Parse(strLevelItems[1])]
                        .Children[int.Parse(strLevelItems[2])]
                        .Children[int.Parse(strLevelItems[3])]
                        .Children[int.Parse(strLevelItems[4])]
                        .Children[int.Parse(strLevelItems[5])]
                        .Children[int.Parse(strLevelItems[6])]
                        .Children[int.Parse(strLevelItems[7])]
                        .Children[int.Parse(strLevelItems[8])]
                        .Children[int.Parse(strLevelItems[9])]
                        .Children[int.Parse(strLevelItems[10])]
                        .Children[int.Parse(strLevelItems[11])]
                        .Children[int.Parse(strLevelItems[12])]
                        .Children[int.Parse(strLevelItems[13])]
                        .Children[int.Parse(strLevelItems[14])]
                        .Children[int.Parse(strLevelItems[15])]
                        .Children[int.Parse(strLevelItems[17])]
                        .Children[int.Parse(strLevelItems[18])]
                        .Children[int.Parse(strLevelItems[20])]
                        .Children[int.Parse(strLevelItems[21])]
                        .Children[int.Parse(strLevelItems[22])]
                        .Children[int.Parse(strLevelItems[23])]
                        .Children[int.Parse(strLevelItems[24])]
                        .Children[int.Parse(strLevelItems[25])]
                        .Children[int.Parse(strLevelItems[26])]
                        .Children[int.Parse(strLevelItems[28])]
                        .add(objF);
                    break;

                case 30:
                    secsTrx.Children[int.Parse(strLevelItems[0])]
                        .Children[int.Parse(strLevelItems[1])]
                        .Children[int.Parse(strLevelItems[2])]
                        .Children[int.Parse(strLevelItems[3])]
                        .Children[int.Parse(strLevelItems[4])]
                        .Children[int.Parse(strLevelItems[5])]
                        .Children[int.Parse(strLevelItems[6])]
                        .Children[int.Parse(strLevelItems[7])]
                        .Children[int.Parse(strLevelItems[8])]
                        .Children[int.Parse(strLevelItems[9])]
                        .Children[int.Parse(strLevelItems[10])]
                        .Children[int.Parse(strLevelItems[11])]
                        .Children[int.Parse(strLevelItems[12])]
                        .Children[int.Parse(strLevelItems[13])]
                        .Children[int.Parse(strLevelItems[14])]
                        .Children[int.Parse(strLevelItems[15])]
                        .Children[int.Parse(strLevelItems[17])]
                        .Children[int.Parse(strLevelItems[18])]
                        .Children[int.Parse(strLevelItems[20])]
                        .Children[int.Parse(strLevelItems[21])]
                        .Children[int.Parse(strLevelItems[22])]
                        .Children[int.Parse(strLevelItems[23])]
                        .Children[int.Parse(strLevelItems[24])]
                        .Children[int.Parse(strLevelItems[25])]
                        .Children[int.Parse(strLevelItems[26])]
                        .Children[int.Parse(strLevelItems[29])]
                        .add(objF);
                    break;

                default:
                    strRet = "Data structure is too deep,max:30";
                    bResult = false;
                    break;
            }
            return bResult;
        }

        private static bool GetItemValue(string strDataType, string strDataValue, ref string strDataResult, ref string strRet)
        {
            bool bResult = true;
            strDataValue = strDataValue.Trim();//将两边的空格去掉
            if (strDataValue == "L")
                return true;
            if (strDataValue.IndexOf('>') < 0)
            {
                strRet = "Data value is error,there is not end char'>'.";
                return false;
            }
            //if (strDataValue.IndexOf(']') > 0)
            //{
            //    strDataValue = strDataValue.Substring(strDataValue.IndexOf(']') + 1);
            //}
            if (strDataType == "A" || strDataType == "ASCII")
            {
                if (strDataValue.IndexOf(" '") > 0)
                    strDataValue = strDataValue.Substring(strDataValue.IndexOf(" '") + 2);//取空格后的值
                int iEndIndex = 0;
                if (strDataValue.IndexOf("'>", iEndIndex) >= 0)
                {
                    iEndIndex = strDataValue.IndexOf("'>", iEndIndex);
                }
                if (iEndIndex == -1)
                    strDataValue = strDataValue.Substring(0, strDataValue.IndexOf("'>"));
                else
                    strDataValue = strDataValue.Substring(0, iEndIndex);

                strDataValue = strDataValue.Trim('\"').Trim('\'');//将两边的空格去掉
                strDataResult = strDataValue;
                return true;
            }
            if (strDataValue.IndexOf(' ') > 0)
                strDataValue = strDataValue.Substring(strDataValue.IndexOf(' '));//取空格后的值
            strDataValue = strDataValue.Trim();//将两边的空格去掉
            strDataValue = strDataValue.Substring(0, strDataValue.IndexOf('>'));
            strDataValue = strDataValue.Trim();//将两边的空格去掉
            strDataValue = strDataValue.Replace("'", "");
            strDataValue = strDataValue.Replace("\"", "");
            strDataResult = strDataValue;
            return bResult;
        }
    }
}
