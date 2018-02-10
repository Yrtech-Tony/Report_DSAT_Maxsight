using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Vgic.Report.Web.DSATReportServer;

namespace Vgic.Report.Web.Controllers
{
    public class AccountController : BaseController
    {
        // GET: Account
        public ActionResult Login(string ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        [HttpPost]
        public ActionResult Login(string sUserName, string sPassword, string ReturnUrl)
        {
            string loginStatus = _client.DSATReport_Login(sUserName, StrToMD5(sPassword));
            if (loginStatus == "1")
            {
                return Json(new { Status = false, sErrMsg = "用户名不存在！" });
            }
            else if (loginStatus == "2")
            {
                return Json(new { Status = false, sErrMsg = "密码不正确！" });
            }
            //登陆成功 用户信息放到session中
            UserInfoDto userInfo = _client.DSATReport_UserInfoSearch(sUserName);
            if (userInfo != null) {
                //FormsAuthentication.SetAuthCookie(userInfo.UserId, false);
                Session["LoginUser"] = userInfo;
                Session["UserId"] = userInfo.UserId;
                return Json(new { Status = true, sRedirectURL = HttpContext.Request.Url.GetLeftPart(UriPartial.Authority) + ReturnUrl });
            }
            else {
                return Json(new { Status = false, sErrMsg = "用户信息不存在！" });
            }

        }
        private string StrToMD5(string str)
        {
            byte[] data = Encoding.GetEncoding("GB2312").GetBytes(str);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] OutBytes = md5.ComputeHash(data);

            string OutString = "";
            for (int i = 0; i < OutBytes.Length; i++)
            {
                OutString += OutBytes[i].ToString("x2");
            }
            return OutString.ToLower();
        }

        public ActionResult Logoff()
        {
            //FormsAuthentication.SignOut();
            Session["LoginUser"] = null;
            Session["CloseNotice"] = null;
            return this.Redirect("~/");
        }
        public ActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ChangePassword(string sOldPassword, string sNewPassword)
        {
            try
            {
                string result = _client.DSATReport_PasswordModify(UserInfo.UserId, StrToMD5(sOldPassword), StrToMD5(sNewPassword));
                Session["LoginUser"] = null;
                //FormsAuthentication.SignOut();
                //return RedirectToAction("Logoff");
                return Json(new { nAction = 2, sRedirectURL = "../Account/Login" });
            }
            catch (Exception ex)
            {
                return Json(new { nAction = 1, sRedirectURL = "../Home/Index" });
            }
        }
    }
}