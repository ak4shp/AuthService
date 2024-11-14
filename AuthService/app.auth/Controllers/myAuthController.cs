//using Microsoft.AspNetCore.Mvc;

//namespace app.auth.Controllers
//{

//    [ApiController]
//    [Route("api/auth")]
//    public class myAuthController : Controller
//    {

//        private readonly ILogger<myAuthController> logger;

//        public myAuthController(ILogger<myAuthController> logger)
//        {
//            this.logger = logger;
//        }

//        [HttpPost]
//        [Route("sign-up")]
//        public IActionResult SignUp()
//        {
//            return View();
//        }


//        [HttpPost]
//        [Route("sign-in")]
//        public IActionResult SignIn()
//        {
//            return View();
//        }


//        [HttpPost]
//        [Route("revoke-token")]
//        public IActionResult RevokeTokan()
//        {
//            return View();
//        }


//        [HttpPost]
//        [Route("renew-token")]
//        public IActionResult RenewToken()
//        {
//            return View();
//        }


//        [HttpGet]
//        [Route("get-item")]
//        public IActionResult GetItem()
//        {
//            return View();
//        }


//        [HttpPost]
//        [Route("save-item")]
//        public IActionResult SaveItem()
//        {
//            return View();
//        }
//    }
//}
