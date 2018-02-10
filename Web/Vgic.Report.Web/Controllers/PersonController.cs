using Infragistics.Documents.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vgic.Report.Web.Attributes;
using Vgic.Report.Web.DSATReportServer;
using Vgic.Report.Web.Models;

namespace Vgic.Report.Web.Controllers
{
    public class PersonController : BaseController
    {
        /// <summary>
        /// 经销商人员报告
        /// </summary>
        /// <returns></returns>
        public ActionResult InfoSearch()
        {
            ViewBag.CountPerPage = _countPerPage;
            ViewBag.RoleType = UserInfo.RoleTypeCode;
            ViewBag.UserId = UserInfo.UserId;
            ViewBag.ShopCodeForCurrentUser = UserInfo.ShopList[0].ShopCode;
            ViewBag.ShopNameForCurrentUser = UserInfo.ShopList[0].ShopName;
            return View();
        }
        public ActionResult StaffInfoSearch(string areaCode, string groupCode, string year, string month, string shopCode, string staffName, int pageNum, int countPerPage)
        {
            List<StaffInfoDto> staffInfoList = _client.DSATReport_StaffInfoSearch(areaCode, groupCode, year, month, shopCode, staffName, pageNum, countPerPage).ToList();
            int totalCount = _client.DSATReport_StaffInfoCountSearch(areaCode, groupCode, year, month, shopCode, staffName);
            bool isCurrentYearMonth = DateTime.Now.Year.Equals(int.Parse(year)) && DateTime.Now.Month.Equals(int.Parse(month));
            return Json(new { staffInfoList = staffInfoList, totalCount = totalCount, isCurrentYearMonth = isCurrentYearMonth }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Delete(int id)
        {
            _client.DSATReport_StaffInfoDelete(id, UserInfo.UserId);
            return Json(true);
        }
        public void DownloadReport(string areaCode, string groupCode, string year, string month, string shopCode, string staffName)
        {
            areaCode = areaCode == "undefined" ? "" : areaCode;
            groupCode = groupCode == "undefined" ? "" : groupCode;

            Workbook book = Workbook.Load(Server.MapPath("~") + @"Content\Excel\" + "人员信息表.xlsx", false);

            //填充数据
            Worksheet sheet = book.Worksheets[0];
            List<StaffInfoDto> staffInfoList = _client.DSATReport_StaffInfoSearch(areaCode, groupCode, year, month, shopCode, staffName, 1, 10000).ToList();
            this.DownloadReport_FillReport(sheet, staffInfoList);
            
            //保存excel文件
            string fileName = "StaffInfo.xlsx";
            string dirPath = Server.MapPath("~") + @"\Temp\";
            DirectoryInfo dir = new DirectoryInfo(dirPath);
            if (!dir.Exists)
            {
                dir.Create();
            }
            string filePath = dirPath + fileName;
            book.Save(filePath);

            //下载Excel，并删除临时文件
            this.DownloadExcel(fileName, filePath, true);

        }
        private void DownloadReport_FillReport(Worksheet sheet, List<StaffInfoDto> staffInfoList)
        {
            int rowIndex = 1;
            foreach (StaffInfoDto item in staffInfoList)
            {
                //序号
                sheet.GetCell("A"+ (rowIndex+1)).Value = rowIndex.ToString();

                //区域
                sheet.GetCell("B" + (rowIndex + 1)).Value = item.AreaName;

                //经销店类型
                sheet.GetCell("C" + (rowIndex + 1)).Value = item.ShopType;

                //经销商名称
                sheet.GetCell("D" + (rowIndex + 1)).Value = item.ShopName;

                //经销商代码
                sheet.GetCell("E" + (rowIndex + 1)).Value = item.ShopCode;

                //姓名
                sheet.GetCell("F" + (rowIndex + 1)).Value = item.StaffName;

                //性别
                sheet.GetCell("G" + (rowIndex + 1)).Value = item.Sex;

                //身份证号
                sheet.GetCell("H" + (rowIndex + 1)).Value = item.IdCardNO;

                //电子邮箱
                sheet.GetCell("I" + (rowIndex + 1)).Value = item.Email;

                //部门
                sheet.GetCell("J" + (rowIndex + 1)).Value = item.DepartmentName;

                //岗位
                sheet.GetCell("K" + (rowIndex + 1)).Value = item.PositionName;

                //手机
                sheet.GetCell("L" + (rowIndex + 1)).Value = item.TelNumber;

                //入职时间
                sheet.GetCell("M" + (rowIndex + 1)).Value = item.EntryDate.HasValue? item.EntryDate.Value.ToString("yyyy-MM-dd"):"";

                //上岗时间
                sheet.GetCell("N" + (rowIndex + 1)).Value = item.StartDate.HasValue ? item.StartDate.Value.ToString("yyyy-MM-dd") : "";

                //新员工来源
                sheet.GetCell("O" + (rowIndex + 1)).Value = item.SourceName;

                //是否在职
                sheet.GetCell("P" + (rowIndex + 1)).Value = item.OnJobChk?"是":"否";

                //离职时间
                sheet.GetCell("Q" + (rowIndex + 1)).Value = item.EndDate.HasValue ? item.EndDate.Value.ToString("yyyy-MM-dd") : "";

                //离职原因
                sheet.GetCell("R" + (rowIndex + 1)).Value = item.ReasonName;

                //离职去向
                sheet.GetCell("S" + (rowIndex + 1)).Value = item.FateName;

                //备注
                sheet.GetCell("T" + (rowIndex + 1)).Value = item.Remark;

                //创建时间
                sheet.GetCell("U" + (rowIndex + 1)).Value = item.InDateTime.HasValue ? item.InDateTime.Value.ToString("yyyy-MM-dd") : "";

                //更新时间
                sheet.GetCell("V" + (rowIndex + 1)).Value = item.ModifiDateTime.HasValue ? item.ModifiDateTime.Value.ToString("yyyy-MM-dd") : "";

                //销售表现
                sheet.GetCell("W" + (rowIndex + 1)).Value = item.SalesPerformanceName;
                //AC 成绩
                sheet.GetCell("X" + (rowIndex + 1)).Value = item.ACStatusName;

                rowIndex++;
            }
        }

        /// <summary>
        /// 经销商人员详情
        /// </summary>
        /// <param name="id">人员ID</param>
        /// <returns></returns>
        public ActionResult InfoDetail(int id)
        {
            StaffInfoDto staffInfo = _client.DSATReport_StaffInfoByIdSearch(id);
            ViewBag.StaffInfo = staffInfo;
            return View();
        }

        /// <summary>
        /// 经销商人员信息登记
        /// </summary>
        /// <returns></returns>
        public ActionResult InfoSave()
        {
            ViewBag.ShopCodeForCurrentUser = UserInfo.ShopList[0].ShopCode;
            ViewBag.ShopNameForCurrentUser = UserInfo.ShopList[0].ShopName;
            ViewBag.DepartmentList = new SelectList(_client.DSATReport_SearchDepartment(), "Code", "Name");
            ViewBag.PositionList = new SelectList(_client.DSATReport_SearchPositionByDepartment(1), "Code", "Name");
            ViewBag.SourceOfEmployeeList = new SelectList(_client.DSATReport_SearchSourceOfEmployee(), "Code", "Name");

            return View();
        }
        public ActionResult SearchPositionByDepartmentId(int departmentId)
        {
            return Json(_client.DSATReport_SearchPositionByDepartment(departmentId), JsonRequestBehavior.AllowGet);
        }
        public ActionResult CheckDuplicationForIdCardNo(string idCardNo)
        {
            StaffInfoDto staffInfoDto = _client.DSATReport_StaffInfoByIdCardNo(idCardNo);
            if (staffInfoDto.Id == 0 || staffInfoDto.OnJobChk == false || staffInfoDto.EndDate !=null)
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Save(int id, string shopCode, string shopName, string idCardNO, string staffName, string sex, string birthday, string email, int positionId, string telNumber, string entryDate, string startDate, int sourceId, string remark)
        {
            DateTime? birthdayTemp = String.IsNullOrEmpty(birthday) ? null : (DateTime?)DateTime.Parse(birthday);
            DateTime? entryDateTemp = String.IsNullOrEmpty(entryDate) ? null : (DateTime?)DateTime.Parse(entryDate);
            DateTime? startDateTemp = String.IsNullOrEmpty(startDate) ? null : (DateTime?)DateTime.Parse(startDate);

            StaffInfoDto staffInfo = new StaffInfoDto()
            {
                Id = id,
                ShopCode = shopCode,
                ShopName = shopName,
                IdCardNO = idCardNO,
                StaffName = staffName,
                Sex = sex,
                Birthday = birthdayTemp,
                Email = email,
                PositionId = positionId,
                TelNumber = telNumber,
                EntryDate = entryDateTemp,
                StartDate = startDateTemp,
                SourceId = sourceId,
                Remark = remark,
                OnJobChk = true
            };
            _client.DSATReport_StaffInfoSave1(staffInfo, UserInfo.UserId);
            return Json(true);
        }

        /// <summary>
        /// 经销商人员信息修改
        /// </summary>
        /// <returns></returns>
        public ActionResult InfoEdit(int id)
        {
            StaffInfoDto staffInfo = _client.DSATReport_StaffInfoByIdSearch(id);
            ViewBag.StaffInfo = staffInfo;
            ViewBag.DepartmentList = new SelectList(_client.DSATReport_SearchDepartment(), "Code", "Name", staffInfo.DepartmentCode.ToString());
            ViewBag.PositionList = new SelectList(_client.DSATReport_SearchPositionByDepartment(staffInfo.DepartmentCode), "Code", "Name", staffInfo.PositionId.ToString());
            ViewBag.SourceOfEmployeeList = new SelectList(_client.DSATReport_SearchSourceOfEmployee(), "Code", "Name", staffInfo.SourceId.ToString());
            ViewBag.LeaveReasonList = new SelectList(_client.DSATReport_SearchLeaveReason(), "Code", "Name", staffInfo.ReasonId.ToString());
            ViewBag.LeaveFateList = new SelectList(_client.DSATReport_SearchLeaveFate(), "Code", "Name", staffInfo.FateId.ToString());
            ViewBag.SalesPerformanceList = new SelectList(_client.DSATReport_SearchSalesPerformance(), "Code", "Name", staffInfo.SalesPerformanceId.ToString());

            return View();
        }
        [HttpPost]
        public ActionResult Update(int id, string staffName, string sex, string email, int positionId, string telNumber, 
            string startDate, int sourceId, string onJobChk, string endDate, string reasonId, string fateId, string remark, string salesPerformanceId)
        {
            //DateTime? entryDateTemp = String.IsNullOrEmpty(entryDate) ? null : (DateTime?)DateTime.Parse(entryDate);
            DateTime? startDateTemp = String.IsNullOrEmpty(startDate) ? null : (DateTime?)DateTime.Parse(startDate);
            DateTime? endDateTemp = String.IsNullOrEmpty(endDate) ? null : (DateTime?)DateTime.Parse(endDate);
            StaffInfoDto staffInfo = _client.DSATReport_StaffInfoByIdSearch(id);

            _client.DSATReport_StaffInfoSave(id,staffInfo.IdCardNO, staffInfo.Year, staffInfo.Month, staffInfo.ShopCode,staffName,sex,
                staffInfo.Birthday,email,positionId,telNumber, staffInfo.EntryDate, startDateTemp,
                endDateTemp,sourceId,onJobChk=="true"? true :false,String.IsNullOrEmpty(reasonId)?0:int.Parse(reasonId),
                String.IsNullOrEmpty(fateId) ? 0 : int.Parse(fateId), remark, String.IsNullOrEmpty(salesPerformanceId) ? 0 : int.Parse(salesPerformanceId), UserInfo.UserId);
            return Json(true);
        }

    }
}