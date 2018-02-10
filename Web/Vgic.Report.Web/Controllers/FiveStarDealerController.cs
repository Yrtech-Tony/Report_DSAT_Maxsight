using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vgic.Report.Web.DSATReportServer;

namespace Vgic.Report.Web.Controllers
{
    public class FiveStarDealerController : BaseController
    {
        //
        // GET: /FiveStarDealer/
        public ActionResult Index()
        {
            ViewBag.RoleType = UserInfo.RoleTypeCode;
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

        public ActionResult Search(string year, string filename, int pageNum, int pageSize)
        {
            try
            {
                int total = _client.DSATReport_VideoSearchCount("FiveStar", year, "", "", filename);
                List<VideoDto> resultListTemp = _client.DSATReport_VideoSearch("FiveStar", year, "", "", filename, pageNum, pageSize,UserInfo.UserId).ToList();
                CalcPages(total);

                return Json(new { Status = true, ResultList = resultListTemp, PageCount = ViewBag.Pages, CurPage = pageNum }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, ErrMsg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Save(VideoDto video)
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

        public ActionResult Vote(string id)
        {
            try
            {
                int VoteCount =  _client.DSATReport_VoteSave(id, UserInfo.UserId);

                return Json(new { Status = true, VoteCount = VoteCount }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, ErrMsg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}