using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;

namespace BarUTomaREST.Controllers
{
    [System.Web.Http.Authorize]
    public class UserController : BaseController
    {
        /// <summary>
        /// Get information about currently logged user.
        /// </summary>
        /// <returns>Information about currently logged user (only for self). (JSON)</returns>
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("user/")]
        public ActionResult GetUser()
        {
            return new JsonResult(){ Data = LoggedUser };
        }
    }
}
