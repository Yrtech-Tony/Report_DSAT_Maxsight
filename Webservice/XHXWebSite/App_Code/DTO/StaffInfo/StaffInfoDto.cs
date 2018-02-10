using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

/// <summary>
/// Summary description for StaffInfoDto
/// </summary>
public class StaffInfoDto
{
    public StaffInfoDto()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public int Id { get; set; }
    public string IdCardNO { get; set; }
    public string Year { get; set; }
    public string Month { get; set; }
    public string ShopCode { get; set; }
    public string ShopName { get; set; }
    public string StaffName { get; set; }
    public string Sex { get; set; }
    public string Email { get; set; }
    public int PositionId { get; set; }
    public string PositionName { get; set; }
    public string TelNumber { get; set; }
    public DateTime? EntryDate { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int SourceId { get; set; }
    public string SourceName { get; set; }
    public bool OnJobChk { get; set; }
    public int ReasonId { get; set; }
    public string ReasonName { get; set; }
    public int FateId { get; set; }
    public string FateName { get; set; }
    public int SalesPerformanceId { get; set; }
    public string SalesPerformanceName { get; set; }
    public string Remark { get; set; }
    public string InUserId { get; set; }
    public DateTime? InDateTime { get; set; }
    public string ModifiUserId { get; set; }
    public DateTime? ModifiDateTime { get; set; }
    public string ShopType { get; set; }
    public string AreaName { get; set; }
    public DateTime? Birthday { get; set; }
    public int Age { get; set; }
    public string ACStatusName { get; set; }
    public int DepartmentCode { get; set; }
    public string DepartmentName { get; set; }
}
