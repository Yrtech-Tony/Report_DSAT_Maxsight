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
/// Summary description for VoteDto
/// </summary>
public class VoteDto
{
    public VoteDto()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public string FileId { get; set; }
    public string VoteId { get; set; }
    public string VoteUserId { get; set; }
    public DateTime VoteInDateTime { get; set; }
}
