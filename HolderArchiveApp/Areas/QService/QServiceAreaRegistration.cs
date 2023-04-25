using System.Web.Mvc;

namespace HolderArchiveApp.Areas.QService
{
	public class QServiceAreaRegistration : AreaRegistration
	{
		public override string AreaName => "QServiceArea";

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute(
				"QService_default",
				"QServiceArea/{controller}/{action}/{id}",
				new { action = "Index", id = UrlParameter.Optional }
			);
		}
	}
}