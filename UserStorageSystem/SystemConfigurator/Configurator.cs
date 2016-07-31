//-----------------------------------------------------------------------
// <copyright file="Configurator.cs" company="No Company">
//     No Company. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace SystemConfigurator
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.ServiceModel;
    using System.Xml;
    using CompositionRoot;
    using IDGenerator;
    using Ninject;
    using Ninject.Parameters;
    using UserStorage;

    /// <summary>
    /// Configures services.
    /// </summary>
    public static class Configurator
    {
        #region Fields

        private static readonly StandardKernel Kernel;
        private static readonly BooleanSwitch BoolSwitch = new BooleanSwitch("logSwitch", string.Empty);
        private static UserService masterService;
        private static ServiceHost serviceHost;

        #endregion

        #region Type Initializer

        static Configurator()
        {
            Kernel = new StandardKernel();
            Kernel.Load<Resolver>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Starts server.
        /// </summary>
        public static void StartServer()
        {
            var section = (ServiceConfigSection)ConfigurationManager.GetSection("ServiceConfig");
            if (section != null)
            {
                var serviceElement = section.ServiceItems.Cast<ServiceElement>().SingleOrDefault();
                if (serviceElement?.Role == "Master")
                {
                    var slaveElements = serviceElement.Slaves.Cast<SlaveElement>();
                    ConfigureMaster(serviceElement, slaveElements);
                }
                else if (serviceElement?.Role == "Proxy")
                {
                    var serviceElements = serviceElement.Slaves.Cast<SlaveElement>();
                    ConfigureProxy(serviceElement, serviceElements);
                }
                else
                {
                    ConfigureSlave(serviceElement);
                }
            }
        }

        /// <summary>
        /// Shut down server.
        /// </summary>
        public static void ShutDownServer()
        {
            if (masterService == null)
            {
                serviceHost.Close();
                return;
            }

            SaveServiceState();
            serviceHost.Close();
        }

        #endregion

        #region Private Methods

        private static void ConfigureProxy(ServiceElement proxyElement, IEnumerable<SlaveElement> serviceElements)
        {
            var services = new List<IUserService>();
            foreach (var se in serviceElements)
            {
                var cf = new ChannelFactory<IUserService>(new NetTcpBinding(), $"net.tcp://{GetAddress(se.Host, se.Port)}");
                IUserService s = cf.CreateChannel();
                services.Add(s);
            }

            var address = GetAddress(proxyElement.Host, proxyElement.Port);
            var service = new ProxyService(address, services);
            StartWcfService(service, address.ToString());
        }

        private static void ConfigureSlave(ServiceElement slaveElement)
        {
            var address = GetAddress(slaveElement.Host, slaveElement.Port);
            var service = CreateServiceInAppDomain($"SlaveServiceDomain{slaveElement.Port}", slaveElement.Type, address);
            StartWcfService(service, address.ToString());
        }

        private static void ConfigureMaster(ServiceElement masterElement, IEnumerable<SlaveElement> slaveElements)
        {
            var serviceAdresses = slaveElements.Select(s => new IPEndPoint(IPAddress.Parse(s.Host), Convert.ToInt32(s.Port))).ToList();

            try
            {
                masterService = Kernel.Get(
                    Type.GetType(masterElement.Type, true, false), 
                    new ConstructorArgument("address", GetAddress(masterElement.Host, masterElement.Port)),
                    new ConstructorArgument("services", serviceAdresses)) as UserService;
            }
            catch (TypeLoadException ex)
            {
                if (BoolSwitch.Enabled)
                {
                    Trace.TraceError($"{DateTime.Now} Exception {ex}");
                }

                throw;
            }
            catch (FileLoadException ex)
            {
                if (BoolSwitch.Enabled)
                {
                    Trace.TraceError($"{DateTime.Now} Exception {ex}");
                }

                throw;
            }
            catch (Exception ex)
            {
                if (BoolSwitch.Enabled)
                {
                    Trace.TraceError($"{DateTime.Now} Exception {ex}");
                }

                throw;
            }

            LoadServiceState();
            StartWcfService(masterService, GetAddress(masterElement.Host, masterElement.Port).ToString());
        }

        private static void SaveServiceState()
        {
            var filePath = ConfigurationManager.AppSettings["Path"];
            if (!File.Exists(filePath))
            {
                using (var myFile = File.Create(filePath))
                {
                }
            }

            using (var xmlWriter = XmlWriter.Create(filePath))
            {
                masterService?.WriteXml(xmlWriter);
            }
        }

        private static void LoadServiceState()
        {
            var filePath = ConfigurationManager.AppSettings["Path"];
            if (!File.Exists(filePath))
            {
                using (var myFile = File.Create(filePath))
                {
                }
            }

            using (var xmlReader = XmlReader.Create(filePath))
            {
                masterService.ReadXml(xmlReader);
            }
        }

        private static UserService CreateServiceInAppDomain(string domainName, string serviceType, IPEndPoint address)
        {
            var domain = AppDomain.CreateDomain(domainName, null, null);

            var repository = Kernel.Get<IUserRepository>();
            var generator = Kernel.Get<IGenerator<int>>();
            var validator = Kernel.Get<IUserValidator>();

            UserService service;
            try
            {
                var typeToLoad = Kernel.Get(Type.GetType(serviceType, true, false)).GetType().FullName;
                string assemblyToLoad = Type.GetType(serviceType, true, false).Assembly.FullName;
                var repo = domain.CreateInstanceAndUnwrap(repository.GetType().Assembly.FullName, repository.GetType().FullName);
                var gen = domain.CreateInstanceAndUnwrap(generator.GetType().Assembly.FullName, generator.GetType().FullName);
                var val = domain.CreateInstanceAndUnwrap(validator.GetType().Assembly.FullName, validator.GetType().FullName);
                service = domain.CreateInstanceAndUnwrap(assemblyToLoad, typeToLoad, false, BindingFlags.Default, null, new object[] { gen, val, repo, address }, null, null) as UserService;
            }
            catch (TypeLoadException ex)
            {
                if (BoolSwitch.Enabled)
                {
                    Trace.TraceError($"{DateTime.Now} Exception {ex}");
                }

                throw;
            }
            catch (FileLoadException ex)
            {
                if (BoolSwitch.Enabled)
                {
                    Trace.TraceError($"{DateTime.Now} Exception {ex}");
                }

                throw;
            }
            catch (Exception ex)
            {
                if (BoolSwitch.Enabled)
                {
                    Trace.TraceError($"{DateTime.Now} Exception {ex}");
                }

                throw;
            }

            return service;
        }

        private static IPEndPoint GetAddress(string host, string port)
        {
            IPAddress ip;
            IPAddress.TryParse(host, out ip);
            int p;
            int.TryParse(port, out p);
            var address = new IPEndPoint(ip, p);
            return address;
        }

        private static void StartWcfService(IUserService service, string address)
        {
            serviceHost = new ServiceHost(service);
            var behaviour = serviceHost.Description.Behaviors.Find<ServiceBehaviorAttribute>();
            behaviour.IncludeExceptionDetailInFaults = true;
            behaviour.InstanceContextMode = InstanceContextMode.Single;
            serviceHost.AddServiceEndpoint(typeof(IUserService), new NetTcpBinding(), $"net.tcp://{address}");
            serviceHost.Open();
        }
        #endregion
    }
}
