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
        private IWindsorContainer windsorContainer;

    	public IWindsorContainer Container
    	{
			get { return windsorContainer; }
    	}

    	public override void Init()
        {
			base.Init();
            logger.Info("Starting Unit Of Work Application");
            InitializeContainer();
            BeginRequest += new EventHandler(context_BeginRequest);
            EndRequest += new EventHandler(context_EndRequest);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void InitializeContainer()
        {
            if(IoC.IsInitialized)
                return;
        	string windsorConfig = Settings.Default.WindsorConfig;
        	if(!Path.IsPathRooted(windsorConfig))
        	{
        		//In ASP.Net apps, the current directory and the base path are NOT the same.
				windsorConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, windsorConfig);
        	}
        	windsorContainer = new RhinoContainer(windsorConfig);
            IoC.Initialize(windsorContainer);
        }

        private void context_BeginRequest(object sender, EventArgs e)
        {
            if(IoC.IsInitialized==false)
                InitializeContainer();
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
            logger.Info("Disposing Unit Of Work Application");
            IoC.Reset(windsorContainer);            
            windsorContainer.Dispose();
        }
    }
}