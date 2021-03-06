using System;
using System.IO;
using System.Threading;
using log4net;

namespace Rhino.ServiceBus.Hosting
{
	using System.Reflection;
	using System.Text;

	public class RemoteAppDomainHost
    {
        private readonly Type boosterType;
        private readonly string assembly;
        private readonly ILog logger = LogManager.GetLogger(typeof (RemoteAppDomainHost));
        private readonly string path;
        private HostedService current;
        private string configurationFile;

        public RemoteAppDomainHost Configuration(string configFile)
        {
            configurationFile = configFile;
            return this;
        }

        public RemoteAppDomainHost(Type boosterType)
            :this(boosterType.Assembly.Location)
        {
            this.boosterType = boosterType;
        }

        public RemoteAppDomainHost(string assemblyPath)
        {
            assembly = Path.GetFileNameWithoutExtension(assemblyPath);
            path = Path.GetDirectoryName(assemblyPath);
        }

        public void Start()
        {
            HostedService service = CreateNewAppDomain();
            var watcher = new FileSystemWatcher(path);
            bool wasCalled = false;
            var @lock = new object();
            FileSystemEventHandler handler = (sender, e) =>
            {
                string extension = Path.GetExtension(e.FullPath);
                if (extension != ".dll" && extension != ".config" && extension != ".exe")
                    return;
                watcher.Dispose();
                lock (@lock)
                {
                    if (wasCalled)
                        return;
                    wasCalled = true;

                    logger.WarnFormat("Got change request for {0}, disposing current AppDomain",
                                      e.Name);
                    service.Stop();

                    Thread.Sleep(500); //allow for other events to happen
                    logger.Warn("Restarting...");
                    Start();
                }
            };
            watcher.Deleted += handler;
            watcher.Changed += handler;
            watcher.Created += handler;

            watcher.EnableRaisingEvents = true;

            current = service;

        	try
        	{
        		service.Start();
        	}
			catch (ReflectionTypeLoadException e)
			{
				var sb = new StringBuilder();
				foreach (var exception in e.LoaderExceptions)
				{
					sb.AppendLine(exception.ToString());
				}
				throw new TypeLoadException(sb.ToString(), e);
			}
        }

        private HostedService CreateNewAppDomain()
        {
            var appDomainSetup = new AppDomainSetup
            {
                ApplicationBase = path,
                ApplicationName = assembly,
                ConfigurationFile = ConfigurationFile,
                ShadowCopyFiles = "true" //yuck
            };
            AppDomain appDomain = AppDomain.CreateDomain(assembly, null, appDomainSetup);
            object instance = appDomain.CreateInstanceAndUnwrap("Rhino.ServiceBus",
                                                                "Rhino.ServiceBus.Hosting.DefaultHost");
            var hoster = (DefaultHost) instance;
            
            if (boosterType != null)
                hoster.SetBootStrapperTypeName(boosterType.FullName);

            return new HostedService
            {
                Stop = hoster.Close,
                Start = () => hoster.Start(assembly)
            };
        }

        private string ConfigurationFile
        {
            get
            {
                if (configurationFile != null)
                    return configurationFile;
                configurationFile = Path.Combine(path, assembly + ".dll.config");
                if (File.Exists(configurationFile) == false)
                    configurationFile = Path.Combine(path, assembly + ".exe.config");
                return configurationFile;
            }
        }

        public void Close()
        {
            if (current != null)
                current.Stop();
        }

        #region Nested type: HostedService

        private class HostedService
        {
            public Action Start;
            public Action Stop;
        }

        #endregion
    }
}