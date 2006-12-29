using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Web;
using Castle.Windsor;
using log4net;
using Rhino.Commons.Properties;

namespace Rhino.Commons.HttpModules
{
    public class UnitOfWorkApplication : HttpApplication, IContainerAccessor
    {
        static ILog logger = LogManager.GetLogger(typeof (UnitOfWorkApplication));
        private static IWindsorContainer windsorContainer;

    	public IWindsorContainer Container
    	{
			get { return windsorContainer; }
    	}

		public virtual void Application_Start(object sender, EventArgs e)
		{
			string windsorConfig = Settings.Default.WindsorConfig;
			if (!Path.IsPathRooted(windsorConfig))
			{
				//In ASP.Net apps, the current directory and the base path are NOT the same.
				windsorConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, windsorConfig);
			}
			windsorContainer = new RhinoContainer(windsorConfig);
			IoC.Initialize(windsorContainer);
		}


		public virtual void Application_End(object sender, EventArgs e)
		{
			if (windsorContainer != null)//can happen if this isn't the first app
			{
				IoC.Reset(windsorContainer);
				windsorContainer.Dispose();
			}
		}


    	public override void Init()
        {
			base.Init();
            logger.Info("Starting Unit Of Work Application");
            BeginRequest += new EventHandler(context_BeginRequest);
            EndRequest += new EventHandler(context_EndRequest);
        }

        private void context_BeginRequest(object sender, EventArgs e)
        {
            logger.Debug("Starting Unit Of Work For Request");
            UnitOfWork.Start();
        }

    	private void context_EndRequest(object sender, EventArgs e)
        {
            logger.Debug("Disposing Unit Of Work For Request");
            UnitOfWork.Current.Dispose();
        }

        public override void Dispose()
        {
            BeginRequest -= new EventHandler(context_BeginRequest);
            EndRequest -= new EventHandler(context_EndRequest);
        }
    }
}
