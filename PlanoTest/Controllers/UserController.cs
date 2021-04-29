using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PlanoTest.Data;
using PlanoTest.Models;
using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Resources;

namespace PlanoTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {

        private readonly ILogger<UserController> _logger;
        private UserContext _userContext;
        private IDictionary resources;
        private ReadResource()
        {
            ResXResourceReader rr = new ResXResourceReader("resources.resx");
            foreach (DictionaryEntry d in rsxr)
            {
                resources.Add(d.Key, d.Value);
            }
            rsxr.Close();
        }
        public UserController(ILogger<UserController> logger, UserContext uc)
        {
            _logger = logger;
            _userContext = uc;
            ReadResource();
        }

        [HttpPost]
        public ActionResult<User> GetMessage([FromBody] long userId)
        {
            var user = _userContext.Users.Where(x => x.UserId == userId).FirstOrDefault();
            dynamic jsonObject = new JObject();
            jsonObject.FullName = user.FullName;
            jsonObject.Message = resources[user.Language];
            return jsonObject;
        }

        [HttpPost]
        public ActionResult<User> GetMessage([FromBody] string FullName, string Language)
        {
            dynamic jsonObject = new JObject();
            if (!IsValidCultureName(Language))
            {
                try
                {
                    User _user = new User();
                    _user.FullName = FullName;
                    _user.Language = Language;
                    _userContext.Users.Add(_user);
                    _userContext.SaveChanges();

                    return jsonObject.IsSuccess = true;
                }
                catch(Exception ex)
                {
                    return jsonObject.IsSuccess = false;
                }
            }
            return jsonObject.IsSuccess = false;
        }

        [HttpGet]
        public ActionResult<User> Get()
        {
            var user = _userContext.Users.FirstOrDefault();
            return null;
        }

        private static bool IsValidCultureName(string cultureName)
        {
            CultureInfo[] cultures =
                CultureInfo.GetCultures(CultureTypes.SpecificCultures);
            foreach (CultureInfo culture in cultures)
            {
                if (culture.Name == cultureName)
                    return true;
            }

            return false;
        }
    }
}
