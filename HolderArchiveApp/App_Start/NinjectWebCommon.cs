using System.Web.Http;
using System.Web.Http.Dependencies;
using System.Web.Mvc;
using DataAccessLevel.Abstract;
using DataAccessLevel.Concrete;
using Newtonsoft.Json;
//using Ninject.Web.Mvc;
using Ninject.Web.WebApi;
using IDependencyResolver = Microsoft.AspNet.SignalR.IDependencyResolver;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(HolderArchiveApp.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(HolderArchiveApp.NinjectWebCommon), "Stop")]

namespace HolderArchiveApp
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;

	public static class NinjectWebCommon
	{
		private static readonly Bootstrapper bootstrapper = new Bootstrapper();
		public static IDependencyResolver DependencyReslover { get; set; }

		/// <summary>
		/// Starts the application
		/// </summary>
		public static void Start()
		{
			DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
			DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
			bootstrapper.Initialize(CreateKernel);
		}

		/// <summary>
		/// Stops the application.
		/// </summary>
		public static void Stop()
		{
			bootstrapper.ShutDown();
		}

		/// <summary>
		/// Creates the kernel that will manage your application.
		/// </summary>
		/// <returns>The created kernel.</returns>
		private static IKernel CreateKernel()
		{
			var kernel = new StandardKernel();

			DependencyReslover = new NinjectSignalRDependencyResolver(kernel);

			try
			{
				kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
				kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

				RegisterServices(kernel);
				GlobalConfiguration.Configuration.DependencyResolver = new NinjectDependencyResolver(kernel);
				return kernel;
			}
			catch
			{
				kernel.Dispose();
				throw;
			}
		}

		/// <summary>
		/// Load your modules or register your services here!
		/// </summary>
		/// <param name="kernel">The kernel.</param>
		private static void RegisterServices(IKernel kernel)
		{
			#region DataAccess

			kernel.Bind<IPostgresRepository>().To<PostgresRepository>();
			kernel.Bind<IAcgdRepository>().To<AcgdRepository>();
			kernel.Bind<IIdentityServicesRepository>().To<IdentityServicesRepository>();

			#endregion
		}
	}
}

