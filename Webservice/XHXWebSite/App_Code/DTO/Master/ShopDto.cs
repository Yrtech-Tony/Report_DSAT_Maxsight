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
/// Summary description for ShopDto
/// </summary>
public class ShopDto
{
    public ShopDto()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public string ShopCode { get; set; }
    public string ShopName { get; set; }
    public DateTime? InDateTime { get; set; }
    public string InUserId { get; set; }
    public bool? UseChk { get; set; }
    public string Province { get; set; }
    public string City { get; set; }
    public string SalesOrAfterSales { get; set; }
    public string GroupName { get; set; }
    public string CarType { get; set; }
    public string ShopType { get; set; }
    public string AreaName { get; set; }
    public string SaleSmallAreaCode { get; set; }
    public string AfterSmallAreaCode { get; set; }
    public string UserId { get; set; }
    public string Password { get; set; }
    public string ShortName { get; set; }
    public string FileName { get; set; }
    public string FileId { get; set; }
    public DateTime UploadDate { get; set; }
   

}
