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
using System.Collections.Generic;

/// <summary>
/// Summary description for UserInfoDto
/// </summary>
public class UserInfoDto
{
    public UserInfoDto()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public string UserId { get; set; }
    public string Password { get; set; }
    public string RoleTypeCode { get; set; }
    public List<AreaDto> SmallAreaList { get; set; }
    public List<GroupDto> GroupList { get; set; }
    public List<ShopDto> ShopList { get; set; }
}
