﻿using EinvoiceUnity.Models;
using EinvoiceUnity.repositories;
using NSysDB.NTSQL;
using System;
using System.Collections;
using System.Collections.Generic;

public class ImA0601
{
    private Hashtable SetEinvoiceToHashtable(string[] charA, string sourceFile)
    {
        Hashtable data = new Hashtable();

        try
        {
            data["RejectInvoiceNumber"] = charA[0].ToString().Trim();
            data["InvoiceDate"] = charA[1].ToString().Trim();
            data["BuyerId"] = charA[2].ToString().Trim();
            data["SellerId"] = charA[3].ToString().Trim();
            data["RejectDate"] = charA[4].ToString().Trim();

            data["RejectTime"] = charA[5].ToString().Trim();
            data["RejectReason"] = charA[6].ToString().Trim();
            data["Remark"] = charA[7].ToString().Trim();

            data["TxFileNmae"] = sourceFile;
        }
        catch (Exception ex)
        {
            throw ex;
        }

        return data;
    }

    private string m_ProcessName = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
    public string ProcessName { get { return m_ProcessName; } set { m_ProcessName = value; } }

    public void Begin(string sKind0)
    {
        string[] sArr;
        using (NSysDB.NTSQL.SQL1 query = new NSysDB.NTSQL.SQL1())
        { query.ReturnArr(out sArr); }
        string sFPathN = sArr[0];
        string sFPathP = sArr[1];
        string sFPathY = sArr[2];
        string sPaPartition = sArr[3];
        //Console.WriteLine(sFPathN);

        //抓 A0601*.* 的所有檔案
        foreach (string OkFName in System.IO.Directory.GetFileSystemEntries(sFPathN, sKind0 + "*.*"))
        {
            Console.WriteLine("檔案名稱1:" + OkFName);
            string sPgSN = DateTime.Now.ToString("yyyyMMddHHmmssfff");

            try
            {
                System.IO.File.Move(OkFName, OkFName.Replace(sFPathN, sFPathP));
                string OkFNameP = OkFName.Replace(sFPathN, sFPathP);

                string line = "";
                int counter = 0;

                using (System.IO.StreamReader txtFile = new System.IO.StreamReader(OkFNameP, System.Text.Encoding.Default))
                {
                    Console.WriteLine("檔案名稱2:" + OkFNameP);

                    using (NSysDB.NTSQL.SQL1 query = new NSysDB.NTSQL.SQL1())
                    {
                        //開始匯入
                        query.GoLogsAll(sPgSN, System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName, OkFName, "", "", 1);
                    }

                    while ((line = txtFile.ReadLine()) != null)
                    {
                        if (line.Trim() != "")
                        {
                            //Console.WriteLine(line);
                            string[] CutS = { sPaPartition };
                            string[] charA = line.Split(CutS, StringSplitOptions.None);

                            //字串尾要分號//共9個分號
                            if (charA.Length == 9)
                            {
                                Console.WriteLine("退回(拒收)發票號碼:" + charA[0].ToString());
                                //for (int i = 0; i < charA.Length-1; i++)
                                //{
                                //    Console.WriteLine("Index : {0}, 字串 : {1}", (i + 1), charA[i]);
                                //}

                                using (NSysDB.NTSQL.SQL1 query = new NSysDB.NTSQL.SQL1())
                                {
                                    try
                                    {
                                        if (query.Kind1SelectTbl3("A0601SN", "RejectInvoiceNumber='" + charA[0].ToString() + "'", "A0601") == 0)
                                        {
                                            System.Collections.Hashtable data = new System.Collections.Hashtable();

                                            data["RejectInvoiceNumber"] = charA[0].ToString().Trim();
                                            data["InvoiceDate"] = charA[1].ToString().Trim();
                                            data["BuyerId"] = charA[2].ToString().Trim();
                                            data["SellerId"] = charA[3].ToString().Trim();
                                            data["RejectDate"] = charA[4].ToString().Trim();

                                            data["RejectTime"] = charA[5].ToString().Trim();
                                            data["RejectReason"] = charA[6].ToString().Trim();
                                            data["Remark"] = charA[7].ToString().Trim();

                                            data["TxFileNmae"] = OkFName.ToString().Trim();
                                            query.InsertDataNonKey("A0601", data);
                                            data = null;
                                        }
                                        else
                                        { query.GoLogsAll(sPgSN, System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName, OkFName, "[退回(拒收)發票號碼:" + charA[0].ToString() + "][退回(拒收)發票號碼 已存在!!", (counter + 1).ToString(), 11); }
                                    }
                                    catch (Exception ex)
                                    {
                                        query.GoLogsAll(sPgSN, System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName, OkFName, ex.ToString(), (counter + 1).ToString(), 11);
                                    }
                                }
                            }
                            else
                            {
                                using (NSysDB.NTSQL.SQL1 query = new NSysDB.NTSQL.SQL1())
                                {
                                    query.GoLogsAll(sPgSN, System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName, OkFName, "", (counter + 1).ToString(), 12);
                                }
                            }

                            Console.WriteLine("間隔數:" + charA.Length.ToString());
                            counter++;
                        }
                    }

                    using (NSysDB.NTSQL.SQL1 query = new NSysDB.NTSQL.SQL1())
                    {
                        //結束匯入
                        query.GoLogsAll(sPgSN, System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName, OkFName, "", "", 2);
                    }
                }
                Console.WriteLine("筆數:" + counter.ToString());

                System.IO.File.Move(OkFNameP, OkFNameP.Replace(sFPathP, sFPathY).Replace(".txt", "_" + sPgSN + ".txt"));
            }
            catch (Exception ex)
            {
                using (NSysDB.NTSQL.SQL1 query = new NSysDB.NTSQL.SQL1())
                {
                    query.GoLogsAll(sPgSN, System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName, OkFName, "此檔案已存於:" + sFPathP + " [" + ex + "]", "", 13);
                }
            }
        }
    }

    public void Begin2(string path, string sKind0, ErrorInfoModel errorInfo, string identityKey)
    {
        if (!string.IsNullOrEmpty(sKind0))
            sKind0 = sKind0.ToUpper();
        string[] sArr;
        using (SQL1 sqlAdapter = new SQL1())
        { sqlAdapter.ReturnArr(out sArr); }
        string sPaPartition = sArr[3];
        using (SQL1 sqlAdapter = new SQL1())
        {
            List<EinvoiceA0601Temp> tempData = new List<EinvoiceA0601Temp>();
            var query = sqlAdapter.Kind1SelectTbl2("*", "FILE_TEMP", " EINVOICE_TP='A0601' and IDENT_KEY = '" + identityKey + "'", "", "");
            if (query != null)
            {
                var rows = query.Table.Rows;
                for (int i = 0; i < rows.Count; i++)
                {
                    tempData.Add(new EinvoiceA0601Temp()
                    {
                        EinvoiceContent = rows[i]["FILE_CONTENT"].ToString(),
                        EinvoiceFIlePath = rows[i]["FILE_NM"].ToString(),
                    });
                }
            }
            else
                return;
            string sPgSN = DateTime.Now.ToString("yyyyMMddHHmmssfff");

            sqlAdapter.GoLogsAll(sPgSN, ProcessName, "FILE_TEMP", "", "", 1);

            foreach (var data in tempData)
            {
                int index = tempData.IndexOf(data) + 1;
                string sourceFile = data.EinvoiceFIlePath;
                string einvoiceNumber = string.Empty;
                string errorMsg = string.Empty;

                try
                {
                    string line = "";
                    List<Hashtable> einvoiceDataList = new List<Hashtable>();

                    line = data.EinvoiceContent;
                    if (line.Trim() != "")
                    {
                        string[] CutS = { sPaPartition };
                        string[] charA = line.Split(CutS, StringSplitOptions.None);
                        einvoiceNumber = charA[0].ToUpper();
                        //字串尾要分號//共9個分號
                        if (charA.Length == 9)
                        {
                            if (sqlAdapter.Kind1SelectTbl3("A0601SN", "RejectInvoiceNumber='" + charA[0].ToString() + "'", "A0601") == 0)
                            {
                                Console.WriteLine(string.Format("{0} 寫入正式資料庫 第{1}筆 發票號碼:{2}開始.", sKind0, index, einvoiceNumber));

                                Hashtable hashData = SetEinvoiceToHashtable(charA, sourceFile);
                                string insertMsg = sqlAdapter.InsertDataNonKey("A0601", hashData);

                                #region 寫入有錯誤之處理

                                if (!string.IsNullOrEmpty(insertMsg))
                                {
                                    errorMsg = "[正式][" + sKind0 + "]" + einvoiceNumber + "[TXT寫入正式資料庫發生錯誤，資料不寫入]" + insertMsg;
                                    sqlAdapter.GoLogsAll(sPgSN, ProcessName, sourceFile, errorMsg, index.ToString(), 51, false);
                                    EinvoiceRepository.AddEinvoiceToErrorBuffer(sKind0, einvoiceNumber, errorMsg, 2, 51, ref errorInfo, ProcessName);
                                }
                                else
                                    Console.WriteLine(string.Format("{0} 寫入正式資料庫 第{1}筆 發票號碼:{2}結束.", sKind0, index, einvoiceNumber));

                                #endregion 寫入有錯誤之處理
                            }
                            else
                            {
                                errorMsg = "[正式][發票號碼:" + einvoiceNumber + "][此發票號碼已存在,資料不寫入]";
                                sqlAdapter.GoLogsAll(sPgSN, ProcessName, sourceFile, errorMsg, index.ToString(), 11, false);
                                EinvoiceRepository.AddEinvoiceToErrorBuffer(sKind0, einvoiceNumber, errorMsg, 3, 11, ref errorInfo, ProcessName);
                            }
                        }
                        else
                        {
                            errorMsg = "[正式][發票號碼:" + einvoiceNumber + "][字串尾要分號，共9個分號]";
                            sqlAdapter.GoLogsAll(sPgSN, ProcessName, sourceFile, "[正式]", index.ToString(), 12);
                            EinvoiceRepository.AddEinvoiceToErrorBuffer(sKind0, einvoiceNumber, errorMsg, 4, 12, ref errorInfo, ProcessName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    errorMsg = "[正式][未知錯誤]";
                    sqlAdapter.GoLogsAll(sPgSN, ProcessName, sourceFile, errorMsg + ex.ToString(), "", 15, false);
                    EinvoiceRepository.AddEinvoiceToErrorBuffer(sKind0, einvoiceNumber, errorMsg, 5, 15, ref errorInfo, ProcessName);
                }
            }
            sqlAdapter.GoLogsAll(sPgSN, ProcessName, "FILE_TEMP", "", "", 2);
        }
    }
}

internal class EinvoiceA0601Temp
{
    public string EinvoiceContent { get; set; }
    public string EinvoiceFIlePath { get; set; }
}