using Aliyun.OpenServices.OpenStorageService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vgic.Report.Web.DSATReportServer;

namespace Vgic.Report.Web.Controllers
{
    public class BestPracticeController : BaseController
    {
        // GET: BestPractice
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Video()
        {
            ViewBag.CountPerPage = _countPerPage;
            ViewBag.RoleType = UserInfo.RoleTypeCode;
            ViewBag.UserId = UserInfo.UserId;
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
        #region 文件
        public ActionResult Document()
        {
            ViewBag.CountPerPage = _countPerPage;
            ViewBag.RoleType = UserInfo.RoleTypeCode;
            return View();
        }

        public ActionResult DocumentSearch(int pageNum)
        {
            string reportPath = HttpContext.Server.MapPath("~/ReportFiles/BestPractice/Document");//指导手册所在的文件路径
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
        public ActionResult Casebook()
        {
            ViewBag.CountPerPage = _countPerPage;
            ViewBag.RoleType = UserInfo.RoleTypeCode;
            return View();
        }
        public ActionResult CasebookSearch(int pageNum)
        {
            string reportPath = HttpContext.Server.MapPath("~/ReportFiles/BestPractice/Casebook");//指导手册所在的文件路径
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

        public ActionResult Search(string year, string quarter, string filename, int pageNum, int pageSize)
        {
            try
            {
                int total = _client.DSATReport_VideoSearchCount("Best", "", "", "", filename);
                List<VideoDto> resultListTemp = _client.DSATReport_VideoSearch("Best", year, "", "", filename, pageNum, pageSize, UserInfo.UserId).ToList();
                _countPerPage = pageSize;
                CalcPages(total);

                return Json(new { Status = true, ResultList = resultListTemp, PageCount = ViewBag.Pages, CurPage = pageNum }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, ErrMsg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult VideoSave(VideoDto video)
        {
            try
            {
                _client.DSATReport_VideoSave("Best", video.VideoFileName, "", "", "", UserInfo.UserId);

                return Json(new { Status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, ErrMsg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Detail(VideoDto dto)
        {
            ViewBag.Votes = _client.DSATReport_VoteSearch(dto.FileId);
            return View(dto);
        }

        public ActionResult Comment(string id, string comment)
        {
            try
            {
                _client.DSATReport_CommentSave(id, comment, UserInfo.UserId);

                return Json(new { Status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, ErrMsg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CommentSearch(string id, int pageNum)
        {
            try
            {
                CommentDto[] list = _client.DSATReport_CommentSearch(id);
                List<CommentDto> resultList = list.Skip(_countPerPage * (pageNum - 1)).Take(_countPerPage).ToList<CommentDto>();
                CalcPages(list.Count());

                return Json(new { Status = true, ResultList = resultList, PageCount = ViewBag.Pages, CurPage = pageNum }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, ErrMsg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Vote(string id)
        {
            try
            {
                int VoteCount = _client.DSATReport_VoteSave(id, UserInfo.UserId);

                return Json(new { Status = true, VoteCount = VoteCount }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, ErrMsg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}
