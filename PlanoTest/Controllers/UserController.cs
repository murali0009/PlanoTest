using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PlanoTest.Data;
using PlanoTest.Models;
using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Resources.NetStandard;
using System.Threading;


namespace PlanoTest.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class UserController : ControllerBase
    {

        private readonly ILogger<UserController> _logger;
        private UserContext _userContext;
        
        public UserController(ILogger<UserController> logger, UserContext uc)
        {
            _logger = logger;
            _userContext = uc;
        }

        [HttpPost]
        public ActionResult GetMessage([FromBody] long userId)
        {
            var user = _userContext.Users.Where(x => x.UserId == userId).FirstOrDefault();
            dynamic jsonObject = new JObject();
            jsonObject.FullName = user.FullName;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(user.Language);
            jsonObject.Message = Properties.Resources.Message;
            return Content(jsonObject.ToString(), "application/json");
        }

        [HttpPost]
        public ActionResult AddUser([FromBody] object input)
        {
            dynamic jsonObject = new JObject();
            try
            {
                JObject Inp = JObject.Parse(input.ToString());
                JToken Language;
                JToken FullName;
                Inp.TryGetValue("FullName", out FullName);
                Inp.TryGetValue("Language", out Language);

                if (IsValidCultureName(Language.ToString()))
                {
                    User _user = new User();
                    _user.FullName = FullName.ToString();
                    _user.Language = Language.ToString();
                    _userContext.Users.Add(_user);
                    _userContext.SaveChanges();

                    jsonObject.IsSuccess = true;
                }
                else
                {
                    jsonObject.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                jsonObject.IsSuccess = false;
            }

            return Content(jsonObject.ToString(), "application/json");
        }

        private static bool IsValidCultureName(string cultureName)
        {
            CultureInfo en_cultures =
                CultureInfo.GetCultureInfo("en-US");
            CultureInfo cn_cultures =
                CultureInfo.GetCultureInfo("zh-CN");
            CultureInfo ja_cultures =
                CultureInfo.GetCultureInfo("ja-JP");
            if (en_cultures.Name == cultureName || en_cultures.Name == cultureName || en_cultures.Name == cultureName)
                return true;
            return false;
        }
    }
}
