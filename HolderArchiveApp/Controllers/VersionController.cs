using System.Web.Mvc;

namespace HolderArchiveApp.Controllers
{
    [AllowAnonymous]
    public class VersionController : Controller
    {
        public ActionResult Index()
        {
            return View(GetType().Assembly.GetName().Version);
        }
    }
}