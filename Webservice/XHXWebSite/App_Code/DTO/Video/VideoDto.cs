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
/// Summary description for VideoDto
/// </summary>
public class VideoDto
{
    public VideoDto()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public string FileId { get; set; }
    public string VideoFileName { get; set; }
    public string VideoType { get; set; }
    public string Year { get; set; }
    public string MonthOrQuarter { get; set; }
    public string ShopCode { get; set; }
    public string InUserId { get; set; }
    public DateTime InDateTime { get; set; }
    public int VoteCount { get; set; }
    public bool VoteChk { get; set; }

}
