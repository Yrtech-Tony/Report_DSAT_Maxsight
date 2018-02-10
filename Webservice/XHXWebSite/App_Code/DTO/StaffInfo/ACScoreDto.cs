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
/// Summary description for ACScoreDto
/// </summary>
public class ACScoreDto
{
    public ACScoreDto()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public string IdCardNO { get; set; }
    public string Year { get; set; }
    public string Month { get; set; }
    public int ACStatusId { get; set; }
    public string ACStatusName { get; set; }
    public string ShopCode { get; set; }
    public string ShopName { get; set; }
    public DateTime? InDateTime { get; set; }
    public string StaffName { get; set; }
    public DateTime? ModifyDateTime { get; set; }
    public int PositionCode { get; set; }
    public string PositionName { get; set; }
}
