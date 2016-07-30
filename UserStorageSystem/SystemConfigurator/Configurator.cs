using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using UserStorage;
using Ninject;
using System.Reflection;
using CompositionRoot;
using System.Xml;
using System.IO;
using IDGenerator;
using System.Diagnostics;
using System.Net;
using Ninject.Parameters;
using System.ServiceModel;

namespace SystemConfigurator
{
    /// <summary>
    /// Configures services.
    /// </summary>
    public static class Configurator
    {
        #region Fields
        private static UserService masterService;
        private static readonly StandardKernel kernel;
        private static readonly BooleanSwitch boolSwitch = new BooleanSwitch("logSwitch", string.Empty);

        static ServiceHost serviceHost;

        #endregion

        #region Type Initializer
        static Configurator()
        {
            kernel = new StandardKernel();
            kernel.Load<Resolver>();
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
                if (serviceElement.Role == "Master")
                {
                    var slaveElements = serviceElement.Slaves.Cast<SlaveElement>();
                    ConfigureMaster(serviceElement, slaveElements);
                }
                else if (serviceElement.Role == "Proxy")
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
                ChannelFactory<IUserService> scf;
                scf = new ChannelFactory<IUserService>(new NetTcpBinding(), $"net.tcp://{GetAddress(se.Host, se.Port).ToString()}");
                IUserService s;
                s = scf.CreateChannel();
                services.Add(s);
            }
            var address = GetAddress(proxyElement.Host, proxyElement.Port);
            var service = new ProxyService(address, services);
            StartWCFService(service, address.ToString());
        }

        private static void ConfigureSlave(ServiceElement slaveElement)
        {
            var address = GetAddress(slaveElement.Host, slaveElement.Port);
            var service = CreateServiceInAppDomain($"SlaveServiceDomain{slaveElement.Port}", slaveElement.Type, address);
            StartWCFService(service, address.ToString());
        }

        private static void ConfigureMaster(ServiceElement masterElement, IEnumerable<SlaveElement> slaveElements)
        {
            var serviceAdresses = slaveElements.Select(s => new IPEndPoint(IPAddress.Parse(s.Host), Convert.ToInt32(s.Port))).ToList();

            try
            {
                masterService = kernel.Get(Type.GetType(masterElement.Type, true, false), new ConstructorArgument("address", GetAddress(masterElement.Host, masterElement.Port)),
                    new ConstructorArgument("services", serviceAdresses)) as UserService;
            }
            catch (TypeLoadException ex)
            {
                if (boolSwitch.Enabled)
                {
                    Trace.TraceError("{0:HH:mm:ss.fff} Exception {1}", DateTime.Now, ex);
                }
                throw;
            }
            catch (FileLoadException ex)
            {
                if (boolSwitch.Enabled)
                {
                    Trace.TraceError("{0:HH:mm:ss.fff} Exception {1}", DateTime.Now, ex);
                }
                throw;
            }
            catch (Exception ex)
            {
                if (boolSwitch.Enabled)
                {
                    Trace.TraceError("{0:HH:mm:ss.fff} Exception {1}", DateTime.Now, ex);
                }
                throw;
            }
            LoadServiceState();
            StartWCFService(masterService, GetAddress(masterElement.Host, masterElement.Port).ToString());
        }

        private static void SaveServiceState()
        {
            var filePath = ConfigurationManager.AppSettings["Path"];
            if (!File.Exists(filePath))
            {
                using (var myFile = File.Create(filePath)) { };
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
                using (var myFile = File.Create(filePath)) { };
            }
            using (var xmlReader = XmlReader.Create(filePath))
            {
                masterService.ReadXml(xmlReader);
            }
        }

        private static UserService CreateServiceInAppDomain(string domainName, string serviceType, IPEndPoint address)
        {
            var domain = AppDomain.CreateDomain(domainName, null, null);

            var repository = kernel.Get<IUserRepository>();
            var generator = kernel.Get<IGenerator<int>>();
            var validator = kernel.Get<IUserValidator>();

            UserService service;
            try
            {
                var typeToLoad = kernel.Get(Type.GetType(serviceType, true, false)).GetType().FullName;
                string assemblyToLoad = Type.GetType(serviceType, true, false).Assembly.FullName;
                var repo = domain.CreateInstanceAndUnwrap(repository.GetType().Assembly.FullName, repository.GetType().FullName);
                var gen = domain.CreateInstanceAndUnwrap(generator.GetType().Assembly.FullName, generator.GetType().FullName);
                var val = domain.CreateInstanceAndUnwrap(validator.GetType().Assembly.FullName, validator.GetType().FullName);
                service = domain.CreateInstanceAndUnwrap(assemblyToLoad, typeToLoad, false, BindingFlags.Default, null, new object[] { gen, val, repo, address }, null, null) as UserService;
            }
            catch (TypeLoadException ex)
            {
                if (boolSwitch.Enabled)
                {
                    Trace.TraceError("{0:HH:mm:ss.fff} Exception {1}", DateTime.Now, ex);
                }
                throw;
            }
            catch (FileLoadException ex)
            {
                if (boolSwitch.Enabled)
                {
                    Trace.TraceError("{0:HH:mm:ss.fff} Exception {1}", DateTime.Now, ex);
                }
                throw;
            }
            catch (Exception ex)
            {
                if (boolSwitch.Enabled)
                {
                    Trace.TraceError("{0:HH:mm:ss.fff} Exception {1}", DateTime.Now, ex);
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

        private static void StartWCFService(IUserService service, string address)
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
