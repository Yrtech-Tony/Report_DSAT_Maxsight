using Aliyun.OpenServices.OpenStorageService;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vgic.Report.Web.DSATReportServer;

namespace Vgic.Report.Web.Controllers
{
    public class QSPController : BaseController
    {
        // GET: CSSOEM
        public ActionResult Index()
        {
            return View();
        }
        #region 季度报告
        public ActionResult QuarterlyReport()
        {
            ViewBag.CountPerPage = _countPerPage;
            ViewBag.RoleType = UserInfo.RoleTypeCode;
            ViewBag.CurrentQuarter = ConvertMonthToQuarter();
            ViewBag.ShopCodeForCurrentUser = UserInfo.ShopList[0].ShopCode;
            ViewBag.ShopNameForCurrentUser = UserInfo.ShopList[0].ShopName;
            if (UserInfo.GroupList.Count() > 0)
            {
                ViewBag.GroupCodeForCurrentUser = UserInfo.GroupList[0].GroupCode;
                ViewBag.GroupNameForCurrentUser = UserInfo.GroupList[0].GroupName;
            }
            if (UserInfo.SmallAreaList.Count() > 0)
            {
                ViewBag.AreaCodeForCurrentUser = UserInfo.SmallAreaList[0].AreaCode;
                ViewBag.AreaNameForCurrentUser = UserInfo.SmallAreaList[0].AreaName;
            }
            return View();
        }
        /// <summary>
        /// 当前月所在的季度
        /// </summary>
        /// <param name="month"></param>
        /// <returns></returns>
        public int ConvertMonthToQuarter()
        {
            int month = DateTime.Now.Month;
            double f = Convert.ToDouble(month) / 3f;
            if (f > Convert.ToInt32(f))
            {
                return Convert.ToInt32(f) + 1;
            }
            return Convert.ToInt32(f);
        }
        /// <summary>
        /// 季度报告查询分页
        /// </summary>
        /// <param name="areaCode"></param>
        /// <param name="groupCode"></param>
        /// <param name="shopCode"></param>
        /// <param name="year"></param>
        /// <param name="quarter"></param>
        /// <param name="pageNum"></param>
        /// <returns></returns>
        public ActionResult QuarterSearch(string areaCode, string groupCode, string shopCode, string year, string quarterormonth, int pageNum)
        {
            string reportPath = HttpContext.Server.MapPath("~/ReportFiles/QSP/QuarterReport");//季度报告所在的文件路径
            List<ShopDto> resultListTemp = GetFileList(areaCode, groupCode, shopCode, year, quarterormonth, reportPath);//获取符合条件的所有季度报告
            List<ShopDto> resultList = ((from u in resultListTemp orderby u.ShopCode select u).Skip(_countPerPage * (pageNum - 1)).Take(_countPerPage)).ToList<ShopDto>();//分页
            return Json(new { shopInfoList = resultList, totalCount = resultListTemp.Count }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region 申诉结果反馈
        public ActionResult Complaint()
        {
            ViewBag.CountPerPage = _countPerPage;
            ViewBag.RoleType = UserInfo.RoleTypeCode;
            ViewBag.CurrentQuarter = ConvertMonthToQuarter();
            ViewBag.ShopCodeForCurrentUser = UserInfo.ShopList[0].ShopCode;
            ViewBag.ShopNameForCurrentUser = UserInfo.ShopList[0].ShopName;
            if (UserInfo.GroupList.Count() > 0)
            {
                ViewBag.GroupCodeForCurrentUser = UserInfo.GroupList[0].GroupCode;
                ViewBag.GroupNameForCurrentUser = UserInfo.GroupList[0].GroupName;
            }
            if (UserInfo.SmallAreaList.Count() > 0)
            {
                ViewBag.AreaCodeForCurrentUser = UserInfo.SmallAreaList[0].AreaCode;
                ViewBag.AreaNameForCurrentUser = UserInfo.SmallAreaList[0].AreaName;
            }
            return View();
        }
        public ActionResult ComplaintSearch(string areaCode, string groupCode, string shopCode, string year, string quarterormonth, int pageNum)
        {
            string reportPath = HttpContext.Server.MapPath("~/ReportFiles/QSP/Complaint");//月度报告所在的文件路径
            List<ShopDto> resultListTemp = GetFileList(areaCode, groupCode, shopCode, year, quarterormonth, reportPath);//获取符合条件的所有月度报告
            List<ShopDto> resultList = ((from u in resultListTemp orderby u.ShopCode select u).Skip(_countPerPage * (pageNum - 1)).Take(_countPerPage)).ToList<ShopDto>();//分页
            return Json(new { shopInfoList = resultList, totalCount = resultListTemp.Count }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 申诉结果反馈视频
        public ActionResult ComplaintVideo()
        {
            ViewBag.CountPerPage = _countPerPage;
            ViewBag.RoleType = UserInfo.RoleTypeCode;
            ViewBag.UserId = UserInfo.UserId;
            ViewBag.CurrentQuarter = ConvertMonthToQuarter();
            ViewBag.ShopCodeForCurrentUser = UserInfo.ShopList[0].ShopCode;
            ViewBag.ShopNameForCurrentUser = UserInfo.ShopList[0].ShopName;
            if (UserInfo.GroupList.Count() > 0)
            {
                ViewBag.GroupCodeForCurrentUser = UserInfo.GroupList[0].GroupCode;
                ViewBag.GroupNameForCurrentUser = UserInfo.GroupList[0].GroupName;
            }
            if (UserInfo.SmallAreaList.Count() > 0)
            {
                ViewBag.AreaCodeForCurrentUser = UserInfo.SmallAreaList[0].AreaCode;
                ViewBag.AreaNameForCurrentUser = UserInfo.SmallAreaList[0].AreaName;
            }
            return View();
        }
        public ActionResult ComplaintVideoSearch(string areaCode, string groupCode, string shopCode, string year, string quarterormonth, string filename, int pageNum)
        {
            List<ShopDto> resultListTemp = GetVideoList(areaCode, groupCode, shopCode, year, quarterormonth, filename);//获取符合条件的所有月度报告
            List<ShopDto> resultList = ((from u in resultListTemp orderby u.ShopCode select u).Skip(_countPerPage * (pageNum - 1)).Take(_countPerPage)).ToList<ShopDto>();//分页
            return Json(new { shopInfoList = resultList, totalCount = resultListTemp.Count }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ComplaintVideoSave(VideoDto video)
        {
            try
            {
                string[] strArr = video.VideoFileName.Split('_');
                if (strArr.Length < 4)
                {
                    throw new Exception("文件命名不正确！");
                }
                else
                {
                    video.ShopCode = strArr[0];
                    video.Year = strArr[1];
                    video.MonthOrQuarter = strArr[2];
                }

                _client.DSATReport_QSPVideoSave(video.VideoFileName, video.Year, video.MonthOrQuarter, video.ShopCode, UserInfo.UserId);

                return Json(new { Status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, ErrMsg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult DeleteFile(string id,string filename)
        {            
            OssClient ossClient = new OssClient(endpoin, accessid, accessKey);
            ossClient.DeleteObject("vgic-max", filename);
            _client.DSATReport_QSPVideoDelete(id);
            return Json(true);
        }

        #endregion

        #region 指导手册
        public ActionResult DealerGuide()
        {
            ViewBag.CountPerPage = _countPerPage;
            ViewBag.RoleType = UserInfo.RoleTypeCode;
            ViewBag.CurrentQuarter = ConvertMonthToQuarter();
            return View();
        }
        public ActionResult DealerGuideSearch(int pageNum)
        {
            string reportPath = HttpContext.Server.MapPath("~/ReportFiles/QSP/DealerGuide");//指导手册所在的文件路径
            List<ShopDto> resultListTemp = new List<ShopDto>();
            DirectoryInfo dataDir = new DirectoryInfo(reportPath);
            FileInfo[] filesInfos = dataDir.GetFiles();
            foreach (FileInfo fileInfo in filesInfos)
            {
                ShopDto shopDto = new ShopDto();
                resultListTemp.Add(new ShopDto()
                {
                    UploadDate = fileInfo.CreationTime,
                    FileName = fileInfo.Name,
                });
            }
            List<ShopDto> resultList = ((from u in resultListTemp orderby u.ShopCode select u).Skip(_countPerPage * (pageNum - 1)).Take(_countPerPage)).ToList<ShopDto>();//分页
            return Json(new { shopInfoList = resultList, totalCount = resultListTemp.Count }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region 获取文件列表 季度或者月度
        public List<ShopDto> GetFileList(string areaCode, string groupCode, string shopCode, string year, string quarterormonth, string reportPath)
        {
            List<ShopDto> resultListTemp = new List<ShopDto>();
            List<ShopDto> list = _client.DSATReport_SearchReportShopInfo(areaCode, groupCode, shopCode, 1, 100000).ToList();
            DirectoryInfo dataDir = new DirectoryInfo(reportPath);
            FileInfo[] filesInfo = dataDir.GetFiles();
            try
            {

                // 添加经销商的文件
                foreach (ShopDto strShop in list)
                {
                    foreach (FileInfo file in filesInfo)
                    {
                        // 季度或月份没有选择的情况下只判断年份和经销商代码即可
                        if (string.IsNullOrEmpty(quarterormonth) || quarterormonth == "Q")
                        {
                            if (file.Name.Split('_')[1] == year && file.Name.Split('_')[0].Trim() == strShop.ShopCode.Trim())
                            {
                                ShopDto result = new ShopDto();
                                result.ShopCode = strShop.ShopCode;
                                result.ShopName = strShop.ShopName;
                                result.AreaName = strShop.AreaName;
                                result.GroupName = strShop.GroupName;
                                result.ShopType = strShop.ShopType;
                                result.FileName = file.Name;
                                result.UploadDate = file.CreationTime;

                                resultListTemp.Add(result);
                            }
                        }
                        else // 季度或月份选择的情况下
                        {
                            if (file.Name.Split('_')[1] == year
                                && (file.Name.Split('_')[2] == quarterormonth || file.Name.Split('_')[2] == "Q" + quarterormonth)
                                && file.Name.Split('_')[0].Trim() == strShop.ShopCode.Trim())
                            {
                                ShopDto result = new ShopDto();
                                result.ShopCode = strShop.ShopCode;
                                result.ShopName = strShop.ShopName;
                                result.AreaName = strShop.AreaName;
                                result.GroupName = strShop.GroupName;
                                result.ShopType = strShop.ShopType;
                                result.FileName = file.Name;
                                result.UploadDate = file.CreationTime;

                                resultListTemp.Add(result);
                            }
                        }

                    }
                }
                // 添加区域的文件，区域权限只能看到自己区域的报告
                if (!string.IsNullOrEmpty(areaCode))
                {
                    foreach (FileInfo file in filesInfo)
                    {
                        if (string.IsNullOrEmpty(quarterormonth) || quarterormonth == "Q")
                        {
                            if (file.Name.Split('_')[1] == year && file.Name.Split('_')[0].Trim() == areaCode.Trim())
                            {
                                ShopDto result = new ShopDto();
                                result.ShopCode = "";
                                result.ShopName = "";
                                result.AreaName = areaCode;
                                result.GroupName = "";
                                result.ShopType = "";
                                result.FileName = file.Name;
                                result.UploadDate = file.CreationTime;
                                resultListTemp.Add(result);
                            }
                        }
                        else
                        {
                            if (file.Name.Split('_')[1] == year
                                && (file.Name.Split('_')[2] == quarterormonth || file.Name.Split('_')[2] == "Q" + quarterormonth)
                                 && file.Name.Split('_')[0].Trim() == areaCode.Trim())
                            {
                                ShopDto result = new ShopDto();
                                result.ShopCode = "";
                                result.ShopName = "";
                                result.AreaName = areaCode;
                                result.GroupName = "";
                                result.ShopType = "";
                                result.FileName = file.Name;
                                result.UploadDate = file.CreationTime;
                                resultListTemp.Add(result);
                            }
                        }
                    }
                }
                // 添加全国文件,管理员的账号可以看到全国和区域的报告
                if (string.IsNullOrEmpty(areaCode) && string.IsNullOrEmpty(groupCode) && string.IsNullOrEmpty(shopCode))
                {
                    foreach (FileInfo file in filesInfo)
                    {
                        if (string.IsNullOrEmpty(quarterormonth) || quarterormonth == "Q")
                        {
                            if (file.Name.Split('_')[1] == year && (file.Name.Split('_')[0].Trim() == "Nation"
                                                                    || file.Name.Split('_')[0].Trim() == "East"
                                                                    || file.Name.Split('_')[0].Trim() == "Central"
                                                                    || file.Name.Split('_')[0].Trim() == "North"
                                                                    || file.Name.Split('_')[0].Trim() == "Phaeton"
                                                                    || file.Name.Split('_')[0].Trim() == "South"
                                                                    || file.Name.Split('_')[0].Trim() == "West"))
                            {
                                ShopDto result = new ShopDto();
                                result.ShopCode = "";
                                result.ShopName = "";
                                result.AreaName = "";
                                result.GroupName = "";
                                result.ShopType = "";
                                result.FileName = file.Name;
                                result.UploadDate = file.CreationTime;
                                resultListTemp.Add(result);
                            }
                        }
                        else
                        {
                            if (file.Name.Split('_')[1] == year
                                && (file.Name.Split('_')[2] == quarterormonth || file.Name.Split('_')[2] == "Q" + quarterormonth)
                                 && (file.Name.Split('_')[0].Trim() == "Nation"
                                                                    || file.Name.Split('_')[0].Trim() == "East"
                                                                    || file.Name.Split('_')[0].Trim() == "Central"
                                                                    || file.Name.Split('_')[0].Trim() == "North"
                                                                    || file.Name.Split('_')[0].Trim() == "Phaeton"
                                                                    || file.Name.Split('_')[0].Trim() == "South"
                                                                    || file.Name.Split('_')[0].Trim() == "West"))
                            {
                                ShopDto result = new ShopDto();
                                result.ShopCode = "";
                                result.ShopName = "";
                                result.AreaName = "";
                                result.GroupName = "";
                                result.ShopType = "";
                                result.FileName = file.Name;
                                result.UploadDate = file.CreationTime;
                                resultListTemp.Add(result);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return resultListTemp;
        }


        public List<ShopDto> GetVideoList(string areaCode, string groupCode, string shopCode, string year, string quarterormonth, string filename)
        {
            List<ShopDto> resultListTemp = new List<ShopDto>();
            List<ShopDto> shopDtoList = _client.DSATReport_SearchReportShopInfo(areaCode, groupCode, shopCode, 1, 100000).ToList();
            quarterormonth = "Q" + quarterormonth;
            List<VideoDto> list = _client.DSATReport_QSPVideoSearch("QSP", year, quarterormonth, shopCode, filename).ToList();

            foreach (VideoDto video in list)
            {
                ShopDto dto = shopDtoList.Where(x => x.ShopCode == video.ShopCode).FirstOrDefault();
                if (dto != null)
                {
                    ShopDto shopDto = new ShopDto();
                    shopDto.ShopCode = dto.ShopCode;
                    shopDto.ShopType = dto.ShopType;
                    shopDto.ShopName = dto.ShopName;
                    shopDto.GroupName = dto.GroupName;
                    shopDto.AreaName = dto.AreaName;
                    shopDto.FileName = video.VideoFileName;
                    shopDto.FileId = video.FileId;

                    resultListTemp.Add(shopDto);
                }
            }

            return resultListTemp;
        }

        #endregion
        #region 打包下载文件
        public ActionResult DownloadFiles(string areaCode, string groupCode, string shopCode, string year, string quarterormonth, string type)
        {
            try
            {
                string reportPath = HttpContext.Server.MapPath("~/ReportFiles/QSP/");
                List<ShopDto> resultListTemp = GetFileList(areaCode, groupCode, shopCode, year, quarterormonth, reportPath + @"\" + type);
                string temp = Path.Combine(reportPath, @"TEMP\" + "VGICReport" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".7z");
                if (System.IO.File.Exists(temp))
                {
                    System.IO.File.Delete(temp);
                }
                if (!SevenZipCompress(resultListTemp, type, reportPath, temp, 9))
                {
                    throw new Exception("打包文件失败！");
                }

                return Json(new { Status = true, File = temp.Replace(HttpContext.Server.MapPath("~"), "/") }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, ErrorMsg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        private static bool ZipInForFiles(List<ShopDto> fileNames, string foler, string folderToZip, string zipedFile, int level)
        {
            bool isSuccess = true;
            if (!Directory.Exists(folderToZip))
            {
                return false;
            }
            try
            {
                using (ZipOutputStream zipOutStream = new ZipOutputStream(System.IO.File.Create(zipedFile)))
                {
                    zipOutStream.SetLevel(level);
                    string comment = string.Empty;

                    //创建当前文件夹
                    ZipEntry entry = new ZipEntry(foler + "/"); //加上 “/” 才会当成是文件夹创建
                    zipOutStream.PutNextEntry(entry);
                    zipOutStream.Flush();

                    Crc32 crc = new Crc32();

                    foreach (ShopDto shop in fileNames)
                    {
                        string file = Path.Combine(folderToZip, foler, shop.FileName);
                        string extension = string.Empty;
                        if (!System.IO.File.Exists(file))
                        {
                            comment += foler + "，文件：" + shop.FileName + "不存在。\r\n";
                            continue;
                        }

                        using (FileStream fs = System.IO.File.OpenRead(Path.Combine(folderToZip, foler, shop.FileName)))
                        {
                            byte[] buffer = new byte[fs.Length];
                            fs.Read(buffer, 0, buffer.Length);
                            entry = new ZipEntry(foler + "/" + shop.FileName);
                            entry.DateTime = DateTime.Now;
                            entry.Size = fs.Length;
                            fs.Close();
                            crc.Reset();
                            crc.Update(buffer);
                            entry.Crc = crc.Value;
                            zipOutStream.PutNextEntry(entry);
                            zipOutStream.Write(buffer, 0, buffer.Length);
                        }
                    }

                    zipOutStream.SetComment(comment);
                    zipOutStream.Finish();
                }
            }
            catch (Exception)
            {
                isSuccess = false;
            }
            return isSuccess;
        }
        #endregion
    }
}