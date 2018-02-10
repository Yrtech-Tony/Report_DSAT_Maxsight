using Aliyun.OpenServices.OpenStorageService;
using Infragistics.Documents.Excel;
using SevenZip;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Vgic.Report.Web.DSATReportServer;
using Vgic.Report.Web.Models;
using VgicReport.Filter;

namespace Vgic.Report.Web.Controllers
{
    [AuthenAdmin]
    public class BaseController : Controller
    {
        protected ServiceSoapClient _client;
        protected  int _countPerPage = 20;
        protected const string accessid = "OoJldDR2Tld8r3AD";
        protected const string accessKey = "oN9HwWhhqLtzVcGjPrfZ8USKX5kG3C";
        protected const string endpoin = "http://oss-cn-beijing.aliyuncs.com";
        public BaseController()
        {
            _client = new ServiceSoapClient("ServiceSoap");
        }

        protected UserInfoDto UserInfo
        {
            get
            {
                return (UserInfoDto)Session["LoginUser"];
            }
        }

        protected void CalcPages(int total)
        {
            int pages = total % _countPerPage == 0 ? total / _countPerPage : (total / _countPerPage + 1);
            ViewBag.Total = total;
            ViewBag.Pages = pages;
        }

        protected void ExportExcel<T>(string excelName, List<ColumModel> excelColumList, List<T> dataList)
        {
            Workbook book = new Workbook(WorkbookFormat.Excel97To2003);
            Worksheet sheet = book.Worksheets.Add(excelName);

            #region 创建列头

            for (int k = 0; k < excelColumList.Count; k++)
            {
                ColumModel colModel = excelColumList[k];
                sheet.Rows[0].Cells[k].Value = colModel.name;
                sheet.Rows[1].Cells[k].Value = colModel.label;

                if (colModel.hidden == true)
                {
                    sheet.Columns[k].Hidden = true;
                }

                string align = colModel.align;
                if (!string.IsNullOrEmpty(align))
                {
                    if (align.ToLower() == "left")
                    {
                        sheet.Columns[k].CellFormat.Alignment = HorizontalCellAlignment.Left;
                    }
                    else if (align.ToLower() == "right")
                    {
                        sheet.Columns[k].CellFormat.Alignment = HorizontalCellAlignment.Right;
                    }
                    else
                    {
                        sheet.Columns[k].CellFormat.Alignment = HorizontalCellAlignment.Center;
                    }
                }

                sheet.Columns[k].Width = colModel.width * 35;
            }

            sheet.Rows[0].Hidden = true;

            #endregion

            #region 创建数据

            if (dataList != null && dataList.Count > 0)
            {
                T genericObject = default(T);
                for (int r = 0; r < dataList.Count; r++)
                {
                    for (int c = 0; c < excelColumList.Count; c++)
                    {
                        ColumModel colModel = excelColumList[c];

                        genericObject = dataList[r];
                        Type type = genericObject.GetType();
                        PropertyInfo propertyInfo = type.GetProperty(colModel.name); //获取指定名称的属性
                        if (propertyInfo != null)
                        {
                            object value = propertyInfo.GetValue(genericObject, null); //获取属性值
                            if (value is bool)
                            {
                                sheet.Rows[r + 2].Cells[c].Value = (bool)value ? "√" : "×";
                            }
                            else
                            {
                                sheet.Rows[r + 2].Cells[c].Value = value;
                            }
                        }
                    }
                }
            }

            #endregion

            string fileName = excelName + @".xls";
            //保存excel文件
            string dirPath = this.Server.MapPath("~/Temp/");
            DirectoryInfo dir = new DirectoryInfo(dirPath);
            if (!dir.Exists)
            {
                dir.Create();
            }
            book.Save(dirPath + fileName);

            DownloadExcel(fileName, dirPath + fileName, true);
        }
        protected void DownloadExcel(string excelName, string filePath, bool isDeleteAfterDownload = false)
        {
            FileStream stream = new FileStream(filePath, FileMode.Open);
            if (stream == null) return;
            if (string.IsNullOrEmpty(excelName))
            {
                excelName = "GridtoExcel" + "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
            }
            byte[] bytes = new byte[(int)stream.Length];
            stream.Position = 0;
            stream.Read(bytes, 0, bytes.Length);
            stream.Close();
            Response.Clear();
            Response.Charset = "UTF-8";
            Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
            Response.AddHeader("content-type", "application/x-msdownload");
            Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(excelName, Encoding.GetEncoding("UTF-8")));
            Response.BinaryWrite(bytes);
            Response.End();
            if (isDeleteAfterDownload)
            {
                System.IO.File.Delete(filePath);
            }
        }

        public ActionResult DownloadFile(string file, string fileName)
        {
            string path = HttpContext.Server.MapPath(file);
            FileInfo fileInfo = new FileInfo(file);
            string contentType = "application/x-msdownload";
            fileName = fileName == null ? Server.UrlEncode(fileInfo.Name) : fileName;

            return this.File(path, contentType, fileName);
        }

        protected bool SevenZipCompress(List<ShopDto> fileNames, string foler, string folderToZip, string zipedFile,int level)
        {
            if (!Directory.Exists(folderToZip))
            {
                return false;
            }

            try
            {
                string newZipFolder = zipedFile.Replace(Path.GetExtension(zipedFile), @"\");
                if (!Directory.Exists(newZipFolder))
                {
                    Directory.CreateDirectory(newZipFolder);
                }
                //打包文件拷贝到一个新文件夹
                foreach (ShopDto shop in fileNames)
                {
                    string file = Path.Combine(folderToZip, foler, shop.FileName);
                    string extension = string.Empty;
                    if (!System.IO.File.Exists(file))
                    {
                        continue;
                    }
                    else
                    {
                        System.IO.File.Copy(file, newZipFolder + shop.FileName,true);
                    }
                }




                //Process process = new Process();
                //process.StartInfo.FileName = @"C:\Program Files\7-zip\7z.exe";
                //process.StartInfo.Arguments = @" e" + zipedFile + @" -o" + newZipFolder;
                //process.Start();

                SevenZipCompressor.SetLibraryPath(Server.MapPath(@"~/bin/7z64.dll"));
                SevenZipCompressor sevenZipCom = new SevenZipCompressor();
                sevenZipCom.CompressDirectory(newZipFolder, zipedFile);

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("SevenZipCompress Error !"+ex.ToString());
                log(ex.ToString());
                return false;
            }
        }
        public static void log(string message)
        {
            string appDomainPath = AppDomain.CurrentDomain.BaseDirectory;
            string fileName = appDomainPath + @"\" + "Log" + @"\" + DateTime.Now.ToString("yyyyMMdd") + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
            //File.Create(fileName);
            if (!Directory.Exists(appDomainPath + @"\" + "Log"))
            {
                Directory.CreateDirectory(appDomainPath + @"\" + "Log");
            }
            if (!Directory.Exists(appDomainPath + @"\" + "Log" + @"\" + DateTime.Now.ToString("yyyyMMdd")))
            {
                Directory.CreateDirectory(appDomainPath + @"\" + "Log" + @"\" + DateTime.Now.ToString("yyyyMMdd"));
            }
            using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                byte[] by = WriteStringToByte(message, fs);
                fs.Flush();
            }
        }
        public static byte[] WriteStringToByte(string str, FileStream fs)
        {
            byte[] info = new UTF8Encoding(true).GetBytes(str);
            fs.Write(info, 0, info.Length);
            return info;
        }


        public ActionResult DownloadOSSFile(string filename)
        {
            try
            {
                OssClient ossClient = new OssClient(endpoin, accessid, accessKey);
                OssObject file = ossClient.GetObject("vgic-max", filename);
                return File(file.Content, "application/octet-stream", Path.GetFileName(filename));
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public ActionResult LoadTopNotice()
        {
            try
            {
                if (Session["CloseNotice"] != null && (bool)Session["CloseNotice"])
                {
                    return null;
                }
                NoticeDto[] notices = _client.DSATReport_NoticeSearch(UserInfo.UserId);
                return Json(new { Status = true, DataList = notices }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, ErrMsg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult UpdateNotice(int noticeId)
        {
            try
            {
                _client.DSATReport_NoticeUserSave(noticeId,UserInfo.UserId);
                return Json(new { Status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, ErrMsg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public void CloseNotice()
        {
            Session["CloseNotice"] = true;
        }
    }
}