using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vgic.Report.Web.Attributes;
using Vgic.Report.Web.DSATReportServer;

namespace Vgic.Report.Web.Controllers
{
    public class ACScoreController : BaseController
    {
        //
        // GET: /ACScore/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Search(string idCardNO, string year, string month, int pageNum)
        {
            string shopCode = "";
            int total = _client.DSATReport_StaffACScoreCountSearch(idCardNO, shopCode, year, month);
            CalcPages(total);
            ACScoreDto[] acScores = _client.DSATReport_StaffACScoreSearch(idCardNO, shopCode, year, month, pageNum, _countPerPage);
           
            return Json(new { List = acScores, PageCount = ViewBag.Pages, CurPage = pageNum }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(string idCardNO,int positionCode)
        {
            ACScoreDto acScore = new ACScoreDto();
            if (string.IsNullOrEmpty(idCardNO))
            {
                acScore.Year = DateTime.Now.Year.ToString();
                acScore.Month = DateTime.Now.Month.ToString();
                acScore.ACStatusId = 0;
            }
            else
            {
                acScore = _client.DSATReport_StaffACScoreByIdCardNOAndPosition(idCardNO,positionCode);
            } 

            return PartialView("_PartialEdit", acScore);
        }

        public ActionResult Save(ACScoreDto acScore)
        {
            try
            {
                _client.DSATReport_StaffACScoreSave(acScore, UserInfo.UserId);
                return Json(new { Status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, ErrMsg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Delete(string idCardNO, int positionCode)
        {
            try
            {
                _client.DSATReport_StaffACScoreDelete(idCardNO, positionCode);
                return Json(new { Status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, ErrMsg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}