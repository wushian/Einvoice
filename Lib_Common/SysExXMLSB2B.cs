﻿using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;

namespace NSysDB
{
    namespace NTSQL
    {
        /// <summary>
        /// </summary>
        public partial class SQL1
        {
            public bool ValidIsDate(string date)
            {
                DateTime dateTime;
                bool isDate = DateTime.TryParseExact(date, "yyyyMMdd",
                                            CultureInfo.InvariantCulture,
                                            DateTimeStyles.None,
                                            out dateTime);
                DateTime today = DateTime.UtcNow.AddHours(8);
                double totalSeconds = new TimeSpan(dateTime.Ticks - today.Ticks).TotalSeconds;
                bool valid = totalSeconds <= 0;
                return valid;
            }

            private string ConvertStringToBig5(string str)
            {
                byte[] Dbyte = System.Text.Encoding.UTF8.GetBytes(str);
                int DefaultCodePage = Encoding.UTF8.CodePage;
                string Dstring = System.Text.Encoding.UTF8.GetString(Dbyte);
                return Dstring;
            }

            //存證B2B-----S
            public bool ExXmlA0401(string A0401SN, string MInvoiceNumber, string sKind0up, string sKind0upAll, string sPgSN)
            {
                //要有Details才生成XML
                DataView dvResultD = Kind1SelectTbl2("", "A0401D", "MInvoiceNumber='" + MInvoiceNumber + "'", "DSequenceNumber", "");
                Console.WriteLine(MInvoiceNumber);
                if (dvResultD != null)
                {
                    NSysDB.XMLClass oXMLeParamts = new NSysDB.XMLClass();
                    string sFPathN = oXMLeParamts.GetParaXml("ExXMLPa");

                    DataView dvResultM = Kind1SelectTbl2("", sKind0upAll, "A0401SN='" + A0401SN + "'", "", "");
                    bool validDate = ValidIsDate(Convert.ToString(dvResultM.Table.Rows[0]["MInvoiceDate"]));
                    if (!validDate)
                    {
                        GoLogsAll(sPgSN, System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName, sKind0up, "日期過早，不上傳至turnkey，日期：" + Convert.ToString(dvResultM.Table.Rows[0]["MInvoiceDate"]), MInvoiceNumber, 23);
                        return false;
                    }
                    string strXMLAll = "", strXMLM = "", strXMLD = "", strXMLA = "";
                    strXMLM = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + Environment.NewLine
                          //+ "<Invoice xmlns=\"urn:GEINV:eInvoiceMessage:C0401:3.1\"/>" + Environment.NewLine
                          //+ "<Invoice xmlns=\"urn:GEINV:eInvoiceMessage:A0401:3.1\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" targetNamespace=\"urn:GEINV:eInvoiceMessage:A0401:3.1\" elementFormDefault=\"qualified\" attributeFormDefault=\"unqualified\">" + Environment.NewLine
                          + "<Invoice xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:GEINV:eInvoiceMessage:A0401:3.1\" xsi:schemaLocation=\"urn:GEINV:eInvoiceMessage:A0401:3.1 A0401.xsd\" >" + Environment.NewLine
                          //+ "<!-- 開立發票 A0401-->" + Environment.NewLine
                          + "<Main>" + Environment.NewLine
                          //+ "<!-- 發票號碼 -->" + Environment.NewLine
                          + "<InvoiceNumber>" + Convert.ToString(dvResultM.Table.Rows[0]["MInvoiceNumber"]) + "</InvoiceNumber>" + Environment.NewLine
                          //+ "<!-- 發票日期 -->" + Environment.NewLine
                          + "<InvoiceDate>" + Convert.ToString(dvResultM.Table.Rows[0]["MInvoiceDate"]) + "</InvoiceDate>" + Environment.NewLine
                          //+ "<!-- 發票時間 -->" + Environment.NewLine
                          + "<InvoiceTime>" + Convert.ToString(dvResultM.Table.Rows[0]["MInvoiceTime"]) + "</InvoiceTime>" + Environment.NewLine
                          //+ "<!-- 賣方 -->" + Environment.NewLine
                          + "<Seller>" + Environment.NewLine
                          //+ "<!-- 賣方 營業人統一編號 -->" + Environment.NewLine
                          + "<Identifier>" + Convert.ToString(dvResultM.Table.Rows[0]["MSIdentifier"]) + "</Identifier>" + Environment.NewLine
                          //+ "<!-- 賣方 營業人名稱 -->" + Environment.NewLine
                          + "<Name>" + Convert.ToString(dvResultM.Table.Rows[0]["MSName"]) + "</Name>" + Environment.NewLine
                          //+ "<!-- 賣方 營業人地址 -->" + Environment.NewLine 20171116 append by 俊晨
                          + "<Address>" + Convert.ToString(dvResultM.Table.Rows[0]["MSAddress"]) + "</Address>" + Environment.NewLine
                          //+ "<!-- 賣方 營業人負責人姓名 -->" + Environment.NewLine 20171116 append by 俊晨
                          + "<PersonInCharge>" + Convert.ToString(dvResultM.Table.Rows[0]["MSPersonInCharge"]) + "</PersonInCharge>" + Environment.NewLine

                    #region 因稅籍檔並未有電話與傳真 自我檢測時至將會無法通過 先註解 需要再解除註解 20171116 append by 俊晨

                          /*
                          //+ "<!-- 賣方 營業人電話 -->" + Environment.NewLine 20171116 append by 俊晨
                          + "<TelephoneNumber>" + Convert.ToString(dvResultM.Table.Rows[0]["MSTelephoneNumber"]) + "</TelephoneNumber>" + Environment.NewLine
                          //+ "<!-- 賣方 營業人傳真 -->" + Environment.NewLine 20171116 append by 俊晨
                          + "<FacsimileNumber>" + Convert.ToString(dvResultM.Table.Rows[0]["MSFacsimileNumber"]) + "</FacsimileNumber>" + Environment.NewLine
                          */

                    #endregion 因稅籍檔並未有電話與傳真 自我檢測時至將會無法通過 先註解 需要再解除註解 20171116 append by 俊晨

                          + "</Seller>" + Environment.NewLine
                          //+ "<!-- 買方 -->" + Environment.NewLine
                          + "<Buyer>" + Environment.NewLine
                          //+ "<!-- B2B 買方-營業人統一編號  B2C買方則填入10個0 -->" + Environment.NewLine
                          + "<Identifier>" + Convert.ToString(dvResultM.Table.Rows[0]["MBIdentifier"]) + "</Identifier>" + Environment.NewLine
                            //+ "<!-- B2B 買方-營業人名稱  B2C買方則填入4個0 -->" + Environment.NewLine
                            //+ "<Name><![CDATA[" + Convert.ToString(dvResultM.Table.Rows[0]["MBName"]) + "]]></Name>" + Environment.NewLine
                            + "<Name><![CDATA[" + ConvertStringToBig5(Convert.ToString(dvResultM.Table.Rows[0]["MBName"])) + "]]></Name>" + Environment.NewLine
                          + "</Buyer>" + Environment.NewLine
                          //+ "<!-- 發票類別 -->" + Environment.NewLine
                          + "<InvoiceType>" + Convert.ToString(dvResultM.Table.Rows[0]["MInvoiceType"]) + "</InvoiceType>" + Environment.NewLine
                          //+ "<!-- 捐贈註記 -->" + Environment.NewLine
                          + "<DonateMark>" + Convert.ToString(dvResultM.Table.Rows[0]["MDonateMark"]) + "</DonateMark>" + Environment.NewLine
                          + "</Main>" + Environment.NewLine
                          + "<Details>" + Environment.NewLine;

                    int sumAmount = 0;
                    for (int i = 0; i < dvResultD.Count; i++)
                    {
                        strXMLD += "<ProductItem>" + Environment.NewLine
                  //+ "<!-- 品名 -->" + Environment.NewLine
                  + "<Description><![CDATA[" + Convert.ToString(dvResultD.Table.Rows[i]["DDescription"]) + "]]></Description>" + Environment.NewLine
                  //+ "<!-- 數量 -->" + Environment.NewLine
                  + "<Quantity>" + Convert.ToString(dvResultD.Table.Rows[i]["DQuantity"]) + "</Quantity>" + Environment.NewLine
                  //+ "<!-- 單價 -->" + Environment.NewLine
                  + "<UnitPrice>" + Convert.ToString(dvResultD.Table.Rows[i]["DUnitPrice"]) + "</UnitPrice>" + Environment.NewLine
                  //+ "<!-- 金額 -->" + Environment.NewLine
                  + "<Amount>" + Convert.ToString(dvResultD.Table.Rows[i]["DAmount"]) + "</Amount>" + Environment.NewLine
                  //+ "<!-- 明細排列序號 -->" + Environment.NewLine
                  + "<SequenceNumber>" + Convert.ToString(dvResultD.Table.Rows[i]["DSequenceNumber"]) + "</SequenceNumber>" + Environment.NewLine
                  + "</ProductItem>" + Environment.NewLine;
                        if (string.IsNullOrEmpty(Convert.ToString(dvResultD.Table.Rows[i]["DAmount"])))
                            throw new Exception("明細 單品項金額為空值，停止XML生成作業。");
                        sumAmount += Convert.ToInt32(dvResultD.Table.Rows[i]["DAmount"]);
                    }

                    #region 判斷金額總計是否相符

                    int totalAmount = (Convert.ToInt32(dvResultM.Table.Rows[0]["ATotalAmount"]));
                    if ((sumAmount + Convert.ToInt32(dvResultM.Table.Rows[0]["ATaxAmount"])) != totalAmount)
                        throw new Exception(
                            string.Format(
                                @"金額總計與明細不相符，停止XML生成作業<br/> 
                                {0} {1}：  
                                
                                明細金額：{2} <br/> 
                                稅額：{3} <br/> 
                                加總金額： {4}",
                            sKind0up, Convert.ToString(dvResultM.Table.Rows[0]["MInvoiceNumber"]), sumAmount, Convert.ToInt32(dvResultM.Table.Rows[0]["ATaxAmount"]), totalAmount));

                    #endregion 判斷金額總計是否相符

                    //DAmount 加總 == (TotalAmount+ATaxAmount)

                    #region 判斷發票稅別是否為空值

                    if (string.IsNullOrEmpty(Convert.ToString(dvResultM.Table.Rows[0]["ATaxType"])))
                        throw new Exception(sKind0up + "：" + Convert.ToString(dvResultM.Table.Rows[0]["MInvoiceNumber"]) + "發票稅別為空值，停止XML生成作業。");

                    #endregion 判斷發票稅別是否為空值

                    strXMLA = "</Details>" + Environment.NewLine
    + "<Amount>" + Environment.NewLine
    //+ "<!-- 銷售額合計(新台幣)B2B:未稅  B2C:內含稅 -->" + Environment.NewLine
    + "<SalesAmount>" + Convert.ToString(dvResultM.Table.Rows[0]["ASalesAmount"]) + "</SalesAmount>" + Environment.NewLine
    //+ "<!-- 課稅別 -->" + Environment.NewLine
    + "<TaxType>" + Convert.ToString(dvResultM.Table.Rows[0]["ATaxType"]) + "</TaxType>" + Environment.NewLine
    //+ "<!-- 稅率 -->" + Environment.NewLine
    + "<TaxRate>" + Convert.ToString(dvResultM.Table.Rows[0]["ATaxRate"]) + "</TaxRate>" + Environment.NewLine
    //+ "<!-- 營業稅 -->" + Environment.NewLine
    + "<TaxAmount>" + Convert.ToString(dvResultM.Table.Rows[0]["ATaxAmount"]) + "</TaxAmount>" + Environment.NewLine
    //+ "<!-- 總計 -->" + Environment.NewLine
    + "<TotalAmount>" + Convert.ToString(dvResultM.Table.Rows[0]["ATotalAmount"]) + "</TotalAmount>" + Environment.NewLine
    //+ "<!-- 扣抵金額 -->" + Environment.NewLine
    + "<DiscountAmount>" + Convert.ToString(dvResultM.Table.Rows[0]["ADiscountAmount"]) + "</DiscountAmount>" + Environment.NewLine
    //+ "<!-- 原幣金額 -->" + Environment.NewLine
    + "<OriginalCurrencyAmount>" + Convert.ToString(dvResultM.Table.Rows[0]["AOriginalCurrencyAmount"]) + "</OriginalCurrencyAmount>" + Environment.NewLine
    //+ "<!-- 匯率 -->" + Environment.NewLine
    + "<ExchangeRate>" + Convert.ToString(dvResultM.Table.Rows[0]["AExchangeRate"]) + "</ExchangeRate>" + Environment.NewLine
    //+ "<!-- 幣別 -->" + Environment.NewLine
    + "<Currency>" + Convert.ToString(dvResultM.Table.Rows[0]["ACurrency"]) + "</Currency>" + Environment.NewLine
    + "</Amount>" + Environment.NewLine
    + "</Invoice>";

                    strXMLAll = strXMLM + strXMLD + strXMLA;
                    //Console.WriteLine(strXMLAll);
                    StreamWriter sw = new StreamWriter(sFPathN + sKind0up + "_" + MInvoiceNumber + ".XML");
                    sw.Write(strXMLAll);
                    sw.Close();

                    return true;
                }
                else
                {
                    GoLogsAll(sPgSN, System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName, sKind0up, "", MInvoiceNumber, 23);
                    return false;
                }
            }

            public bool ExXmlA0501(string ASN, string MInvoiceNumber, string sKind0up, string sKind0upAll, string sPgSN)
            {
                NSysDB.XMLClass oXMLeParamts = new NSysDB.XMLClass();
                string sFPathN = oXMLeParamts.GetParaXml("ExXMLPa");

                DataView dvResultM = Kind1SelectTbl2("", sKind0upAll, "A0501SN='" + ASN + "'", "", "");
                bool validInvoiceDate = ValidIsDate(Convert.ToString(dvResultM.Table.Rows[0]["InvoiceDate"]));
                if (!validInvoiceDate)
                {
                    GoLogsAll(sPgSN, System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName, sKind0up, "原發票日期過早，不上傳至turnkey，日期：" + Convert.ToString(dvResultM.Table.Rows[0]["InvoiceDate"]), MInvoiceNumber, 23);
                    return false;
                }
                bool validInvoiceCancelDate = ValidIsDate(Convert.ToString(dvResultM.Table.Rows[0]["CancelDate"]));
                if (!validInvoiceCancelDate)
                {
                    GoLogsAll(sPgSN, System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName, sKind0up, "作廢發票日期過早，不上傳至turnkey，日期：" + Convert.ToString(dvResultM.Table.Rows[0]["CancelDate"]), MInvoiceNumber, 23);
                    return false;
                }

                if (dvResultM != null)
                {
                    string strXMLAll = "";
                    strXMLAll = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + Environment.NewLine
                          //+ "<Invoice xmlns=\"urn:GEINV:eInvoiceMessage:C0401:3.1\"/>" + Environment.NewLine
                          //+ "<Invoice xmlns=\"urn:GEINV:eInvoiceMessage:A0501:3.1\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" targetNamespace=\"urn:GEINV:eInvoiceMessage:A0501:3.1\" elementFormDefault=\"qualified\" attributeFormDefault=\"unqualified\">" + Environment.NewLine
                          + "<CancelInvoice xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:GEINV:eInvoiceMessage:A0501:3.1\" xsi:schemaLocation=\"urn:GEINV:eInvoiceMessage:A0501:3.1 A0501.xsd\" >" + Environment.NewLine
                          //+ "<!-- 作廢發票 A0501-->" + Environment.NewLine
                          + "<CancelInvoiceNumber>" + Convert.ToString(dvResultM.Table.Rows[0]["CancelInvoiceNumber"]) + "</CancelInvoiceNumber>" + Environment.NewLine
                          //+ "<!-- 發票日期 -->" + Environment.NewLine
                          + "<InvoiceDate>" + Convert.ToString(dvResultM.Table.Rows[0]["InvoiceDate"]) + "</InvoiceDate>" + Environment.NewLine
                          //+ "<!-- 買方統一編號 -->" + Environment.NewLine
                          + "<BuyerId>" + Convert.ToString(dvResultM.Table.Rows[0]["BuyerId"]) + "</BuyerId>" + Environment.NewLine
                          //+ "<!-- 賣方統一編號 -->" + Environment.NewLine
                          + "<SellerId>" + Convert.ToString(dvResultM.Table.Rows[0]["SellerId"]) + "</SellerId>" + Environment.NewLine
                          //+ "<!-- 發票作廢日期 -->" + Environment.NewLine
                          + "<CancelDate>" + Convert.ToString(dvResultM.Table.Rows[0]["CancelDate"]) + "</CancelDate>" + Environment.NewLine
                          //+ "<!-- 發票作廢時間 -->" + Environment.NewLine
                          + "<CancelTime>" + Convert.ToString(dvResultM.Table.Rows[0]["CancelTime"]) + "</CancelTime>" + Environment.NewLine
                          //+ "<!-- 發票作廢原因 -->" + Environment.NewLine
                          + "<CancelReason><![CDATA[" + Convert.ToString(dvResultM.Table.Rows[0]["CancelReason"]) + "]]></CancelReason>" + Environment.NewLine
                          //+ "<!-- 專案作廢核准文號 -->" + Environment.NewLine
                          + "<ReturnTaxDocumentNumber>" + Convert.ToString(dvResultM.Table.Rows[0]["ReturnTaxDocumentNumber"]) + "</ReturnTaxDocumentNumber>" + Environment.NewLine
                          + "</CancelInvoice>" + Environment.NewLine;

                    StreamWriter sw = new StreamWriter(sFPathN + sKind0up + "_" + MInvoiceNumber + ".XML");
                    sw.Write(strXMLAll);
                    sw.Close();

                    return true;
                }
                else
                {
                    return false;
                }
            }

            public bool ExXmlA0601(string ASN, string MInvoiceNumber, string sKind0up, string sKind0upAll, string sPgSN)
            {
                NSysDB.XMLClass oXMLeParamts = new NSysDB.XMLClass();
                string sFPathN = oXMLeParamts.GetParaXml("ExXMLPa");

                DataView dvResultM = Kind1SelectTbl2("", sKind0upAll, "A0601SN='" + ASN + "'", "", "");
                bool validInvoiceDate = ValidIsDate(Convert.ToString(dvResultM.Table.Rows[0]["InvoiceDate"]));
                if (!validInvoiceDate)
                {
                    GoLogsAll(sPgSN, System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName, sKind0up, "原發票日期過早，不上傳至turnkey，日期：" + Convert.ToString(dvResultM.Table.Rows[0]["InvoiceDate"]), MInvoiceNumber, 23);
                    return false;
                }
                if (dvResultM != null)
                {
                    string strXMLAll = "";
                    strXMLAll = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + Environment.NewLine
                          //+ "<Invoice xmlns=\"urn:GEINV:eInvoiceMessage:C0401:3.1\"/>" + Environment.NewLine
                          //+ "<Invoice xmlns=\"urn:GEINV:eInvoiceMessage:A0601:3.1\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" targetNamespace=\"urn:GEINV:eInvoiceMessage:A0601:3.1\" elementFormDefault=\"qualified\" attributeFormDefault=\"unqualified\">" + Environment.NewLine
                          + "<RejectInvoice xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:GEINV:eInvoiceMessage:A0601:3.1\" xsi:schemaLocation=\"urn:GEINV:eInvoiceMessage:A0601:3.1 A0601.xsd\" >" + Environment.NewLine
                          //+ "<!-- 退回(拒收)發票號碼 -->" + Environment.NewLine
                          + "<RejectInvoiceNumber>" + Convert.ToString(dvResultM.Table.Rows[0]["RejectInvoiceNumber"]) + "</RejectInvoiceNumber>" + Environment.NewLine
                          //+ "<!-- 發票開立日期 -->" + Environment.NewLine
                          + "<InvoiceDate>" + Convert.ToString(dvResultM.Table.Rows[0]["InvoiceDate"]) + "</InvoiceDate>" + Environment.NewLine
                          //+ "<!-- 買方統一編號 -->" + Environment.NewLine
                          + "<BuyerId>" + Convert.ToString(dvResultM.Table.Rows[0]["BuyerId"]) + "</BuyerId>" + Environment.NewLine
                          //+ "<!-- 賣方統一編號 -->" + Environment.NewLine
                          + "<SellerId>" + Convert.ToString(dvResultM.Table.Rows[0]["SellerId"]) + "</SellerId>" + Environment.NewLine
                          //+ "<!-- 發票退回(拒收)日期 -->" + Environment.NewLine
                          + "<RejectDate>" + Convert.ToString(dvResultM.Table.Rows[0]["RejectDate"]) + "</RejectDate>" + Environment.NewLine
                          //+ "<!-- 發票退回(拒收)時間 -->" + Environment.NewLine
                          + "<RejectTime>" + Convert.ToString(dvResultM.Table.Rows[0]["RejectTime"]) + "</RejectTime>" + Environment.NewLine
                          //+ "<!-- 退回(拒收)原因 -->" + Environment.NewLine
                          + "<RejectReason><![CDATA[" + Convert.ToString(dvResultM.Table.Rows[0]["RejectReason"]) + "]]></RejectReason>" + Environment.NewLine
                          + "</RejectInvoice>" + Environment.NewLine;

                    StreamWriter sw = new StreamWriter(sFPathN + sKind0up + "_" + MInvoiceNumber + ".XML");
                    sw.Write(strXMLAll);
                    sw.Close();

                    return true;
                }
                else
                {
                    return false;
                }
            }

            public bool ExXmlB0401(string B0401SN, string MInvoiceNumber, string sKind0up, string sKind0upAll, string sPgSN)
            {
                //要有Details才生成XML
                DataView dvResultD = Kind1SelectTbl2("", "B0401D", "MAllowanceNumber='" + MInvoiceNumber + "'", "DAllowanceSequenceNumber", "");
                Console.WriteLine(MInvoiceNumber);
                if (dvResultD != null)
                {
                    NSysDB.XMLClass oXMLeParamts = new NSysDB.XMLClass();
                    string sFPathN = oXMLeParamts.GetParaXml("ExXMLPa");

                    DataView dvResultM = Kind1SelectTbl2("", sKind0upAll, "B0401SN='" + B0401SN + "'", "", "");
                    bool validInvoiceDate = ValidIsDate(Convert.ToString(dvResultM.Table.Rows[0]["MAllowanceDate"]));
                    if (!validInvoiceDate)
                    {
                        GoLogsAll(sPgSN, System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName, sKind0up, "折讓證明單日期過早，不上傳至turnkey，日期：" + Convert.ToString(dvResultM.Table.Rows[0]["MAllowanceDate"]), MInvoiceNumber, 23);
                        return false;
                    }
                    string strXMLAll = "", strXMLM = "", strXMLD = "", strXMLA = "";
                    strXMLM = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + Environment.NewLine
                          //+ "<Invoice xmlns=\"urn:GEINV:eInvoiceMessage:C0401:3.1\"/>" + Environment.NewLine
                          //+ "<Invoice xmlns=\"urn:GEINV:eInvoiceMessage:B0401:3.1\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" targetNamespace=\"urn:GEINV:eInvoiceMessage:B0401:3.1\" elementFormDefault=\"qualified\" attributeFormDefault=\"unqualified\">" + Environment.NewLine
                          + "<Allowance xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:GEINV:eInvoiceMessage:B0401:3.1\" xsi:schemaLocation=\"urn:GEINV:eInvoiceMessage:B0401:3.1 B0401.xsd\" >" + Environment.NewLine
                          //+ "<!-- 開立折讓證明單|傳送折讓證明 B0401-->" + Environment.NewLine
                          + "<Main>" + Environment.NewLine
                          //+ "<!-- 折讓證明單號碼 -->" + Environment.NewLine
                          + "<AllowanceNumber>" + Convert.ToString(dvResultM.Table.Rows[0]["MAllowanceNumber"]) + "</AllowanceNumber>" + Environment.NewLine
                          //+ "<!-- 折讓證明單日期 -->" + Environment.NewLine
                          + "<AllowanceDate>" + Convert.ToString(dvResultM.Table.Rows[0]["MAllowanceDate"]) + "</AllowanceDate>" + Environment.NewLine
                          //+ "<!-- 賣方 -->" + Environment.NewLine
                          + "<Seller>" + Environment.NewLine
                          //+ "<!-- 賣方 營業人統一編號 -->" + Environment.NewLine
                          + "<Identifier>" + Convert.ToString(dvResultM.Table.Rows[0]["MSIdentifier"]) + "</Identifier>" + Environment.NewLine
                          //+ "<!-- 賣方 營業人名稱 -->" + Environment.NewLine
                          + "<Name>" + Convert.ToString(dvResultM.Table.Rows[0]["MSName"]) + "</Name>" + Environment.NewLine
                          + "</Seller>" + Environment.NewLine
                          //+ "<!-- 買方 -->" + Environment.NewLine
                          + "<Buyer>" + Environment.NewLine
                          //+ "<!-- B2B 買方-營業人統一編號  B2C買方則填入10個0 -->" + Environment.NewLine
                          + "<Identifier>" + Convert.ToString(dvResultM.Table.Rows[0]["MBIdentifier"]) + "</Identifier>" + Environment.NewLine
                          //+ "<!-- B2B 買方-營業人名稱  B2C買方則填入4個0 -->" + Environment.NewLine
                          + "<Name>" + Convert.ToString(dvResultM.Table.Rows[0]["MBName"]) + "</Name>" + Environment.NewLine
                          + "</Buyer>" + Environment.NewLine
                          //+ "<!-- 折讓種類 -->" + Environment.NewLine
                          + "<AllowanceType>" + Convert.ToString(dvResultM.Table.Rows[0]["MAllowanceType"]) + "</AllowanceType>" + Environment.NewLine
                          + "</Main>" + Environment.NewLine
                          + "<Details>" + Environment.NewLine;

                    for (int i = 0; i < dvResultD.Count; i++)
                    {
                        if (string.IsNullOrEmpty(Convert.ToString(dvResultD.Table.Rows[i]["DTaxType"])))
                            throw new Exception(sKind0up + "：" + Convert.ToString(dvResultM.Table.Rows[0]["MAllowanceNumber"]) + "發票稅別為空值，停止XML生成作業。");

                        strXMLD += "<ProductItem>" + Environment.NewLine
                            //+ "<!-- 原發票日期 -->" + Environment.NewLine
                            + "<OriginalInvoiceDate>" + Convert.ToString(dvResultD.Table.Rows[i]["DOriginalInvoiceDate"]) + "</OriginalInvoiceDate>" + Environment.NewLine
                            //+ "<!-- 原發票號碼 -->" + Environment.NewLine
                            + "<OriginalInvoiceNumber>" + Convert.ToString(dvResultD.Table.Rows[i]["DOriginalInvoiceNumber"]) + "</OriginalInvoiceNumber>" + Environment.NewLine
                            //+ "<!-- 原品名 -->" + Environment.NewLine
                            + "<OriginalDescription>" + Convert.ToString(dvResultD.Table.Rows[i]["DOriginalDescription"]) + "</OriginalDescription>" + Environment.NewLine
                            //+ "<!-- 數量 -->" + Environment.NewLine
                            + "<Quantity>" + Convert.ToString(dvResultD.Table.Rows[i]["DQuantity"]) + "</Quantity>" + Environment.NewLine
                            //+ "<!-- 單價 -->" + Environment.NewLine
                            + "<UnitPrice>" + Convert.ToString(dvResultD.Table.Rows[i]["DUnitPrice"]) + "</UnitPrice>" + Environment.NewLine
                            //+ "<!-- 金額(不含稅之進貨額) -->" + Environment.NewLine
                            + "<Amount>" + Convert.ToString(dvResultD.Table.Rows[i]["DAmount"]) + "</Amount>" + Environment.NewLine
                            //+ "<!-- 營業稅額 -->" + Environment.NewLine
                            + "<Tax>" + Convert.ToString(dvResultD.Table.Rows[i]["DTax"]) + "</Tax>" + Environment.NewLine
                            //+ "<!-- 折讓證明單明細排列序號 -->" + Environment.NewLine
                            + "<AllowanceSequenceNumber>" + Convert.ToString(dvResultD.Table.Rows[i]["DAllowanceSequenceNumber"]) + "</AllowanceSequenceNumber>" + Environment.NewLine
                            //+ "<!-- 課稅別 -->" + Environment.NewLine
                            + "<TaxType>" + Convert.ToString(dvResultD.Table.Rows[i]["DTaxType"]) + "</TaxType>" + Environment.NewLine
                            + "</ProductItem>" + Environment.NewLine;
                    }

                    strXMLA = "</Details>" + Environment.NewLine
                   + "<Amount>" + Environment.NewLine
                   //+ "<!-- 營業稅額合計 -->" + Environment.NewLine
                   + "<TaxAmount>" + Convert.ToString(dvResultM.Table.Rows[0]["ATaxAmount"]) + "</TaxAmount>" + Environment.NewLine
                   //+ "<!-- 金額(不含稅之進貨額)合計 -->" + Environment.NewLine
                   + "<TotalAmount>" + Convert.ToString(dvResultM.Table.Rows[0]["ATotalAmount"]) + "</TotalAmount>" + Environment.NewLine
                   + "</Amount>" + Environment.NewLine
                   + "</Allowance>";

                    strXMLAll = strXMLM + strXMLD + strXMLA;
                    //Console.WriteLine(strXMLAll);
                    StreamWriter sw = new StreamWriter(sFPathN + sKind0up + "_" + MInvoiceNumber + ".XML");
                    sw.Write(strXMLAll);
                    sw.Close();

                    return true;
                }
                else
                {
                    GoLogsAll(sPgSN, System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName, sKind0up, "", MInvoiceNumber, 23);
                    return false;
                }
            }

            public bool ExXmlB0501(string ASN, string MInvoiceNumber, string sKind0up, string sKind0upAll, string sPgSN)
            {
                NSysDB.XMLClass oXMLeParamts = new NSysDB.XMLClass();
                string sFPathN = oXMLeParamts.GetParaXml("ExXMLPa");

                DataView dvResultM = Kind1SelectTbl2("", sKind0upAll, "B0501SN='" + ASN + "'", "", "");
                bool validInvoiceDate = ValidIsDate(Convert.ToString(dvResultM.Table.Rows[0]["AllowanceDate"]));
                if (!validInvoiceDate)
                {
                    GoLogsAll(sPgSN, System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName, sKind0up, "原折讓證明單日期過早，不上傳至turnkey，日期：" + Convert.ToString(dvResultM.Table.Rows[0]["AllowanceDate"]), MInvoiceNumber, 23);
                    return false;
                }

                bool validInvoiceCancelDate = ValidIsDate(Convert.ToString(dvResultM.Table.Rows[0]["CancelDate"]));
                if (!validInvoiceCancelDate)
                {
                    GoLogsAll(sPgSN, System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName, sKind0up, "作廢折讓證明單日期過早，不上傳至turnkey，日期：" + Convert.ToString(dvResultM.Table.Rows[0]["AllowanceDate"]), MInvoiceNumber, 23);
                    return false;
                }
                if (dvResultM != null)
                {
                    string strXMLAll = "";
                    strXMLAll = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + Environment.NewLine
                          //+ "<Invoice xmlns=\"urn:GEINV:eInvoiceMessage:C0401:3.1\"/>" + Environment.NewLine
                          //+ "<Invoice xmlns=\"urn:GEINV:eInvoiceMessage:A0501:3.1\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" targetNamespace=\"urn:GEINV:eInvoiceMessage:A0501:3.1\" elementFormDefault=\"qualified\" attributeFormDefault=\"unqualified\">" + Environment.NewLine
                          + "<CancelAllowance xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:GEINV:eInvoiceMessage:B0501:3.1\" xsi:schemaLocation=\"urn:GEINV:eInvoiceMessage:B0501:3.1 B0501.xsd\" >" + Environment.NewLine
                          //+ "<!-- 作廢折讓證明單號碼 B0501-->" + Environment.NewLine
                          + "<CancelAllowanceNumber>" + Convert.ToString(dvResultM.Table.Rows[0]["CancelAllowanceNumber"]) + "</CancelAllowanceNumber>" + Environment.NewLine
                          //+ "<!-- 折讓證明單日期 -->" + Environment.NewLine
                          + "<AllowanceDate>" + Convert.ToString(dvResultM.Table.Rows[0]["AllowanceDate"]) + "</AllowanceDate>" + Environment.NewLine
                          //+ "<!-- 買方統一編號 -->" + Environment.NewLine
                          + "<BuyerId>" + Convert.ToString(dvResultM.Table.Rows[0]["BuyerId"]) + "</BuyerId>" + Environment.NewLine
                          //+ "<!-- 賣方統一編號 -->" + Environment.NewLine
                          + "<SellerId>" + Convert.ToString(dvResultM.Table.Rows[0]["SellerId"]) + "</SellerId>" + Environment.NewLine
                          //+ "<!-- 折讓證明單作廢日期 -->" + Environment.NewLine
                          + "<CancelDate>" + Convert.ToString(dvResultM.Table.Rows[0]["CancelDate"]) + "</CancelDate>" + Environment.NewLine
                          //+ "<!-- 折讓證明單作廢時間 -->" + Environment.NewLine
                          + "<CancelTime>" + Convert.ToString(dvResultM.Table.Rows[0]["CancelTime"]) + "</CancelTime>" + Environment.NewLine
                          //+ "<!-- 折讓證明單作廢原因 -->" + Environment.NewLine
                          + "<CancelReason><![CDATA[" + Convert.ToString(dvResultM.Table.Rows[0]["CancelReason"]) + "]]></CancelReason>" + Environment.NewLine
                          + "</CancelAllowance>" + Environment.NewLine;

                    StreamWriter sw = new StreamWriter(sFPathN + sKind0up + "_" + MInvoiceNumber + ".XML");
                    sw.Write(strXMLAll);
                    sw.Close();

                    return true;
                }
                else
                {
                    return false;
                }
            }

            //存證B2B-----E
        }
    }
}