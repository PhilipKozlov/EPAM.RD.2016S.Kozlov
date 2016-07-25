using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserStorage;
using Ninject;
using System.Reflection;
using CompositionRoot;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
using IDGenerator;
using System.Diagnostics;
using System.Net;
using Ninject.Parameters;

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
        /// Gets a collection of services.
        /// </summary>
        /// <returns> Collection of services.</returns>
        public static ProxyService ConfigurateServices()
        {
            var services = new List<IUserService>();
            var section = (ServiceConfigSection)ConfigurationManager.GetSection("ServiceConfig");
            if (section != null)
            {
                var serviceElements = section.ServiceItems.Cast<ServiceElement>();
                if (serviceElements.Where(si => si.Role == "Master").Count() > 1) throw new ArgumentException("Can have only one master.");
                if (serviceElements.Where(si => si.Role == "Slave").Count() < 1) throw new ArgumentException("Must have at least one slave.");

                var i = 0;
                // configure slaves
                foreach (var si in serviceElements)
                {
                    UserService service = null;
                    if (si.Role == "Slave")
                    {
                        service = CreateServiceInAppDomain($"SlaveServiceDomain{i}", si.Type, GetAddress(si));
                        services.Add(service);
                        i++;
                    }
                }

                // configure master
                var masterElement = serviceElements.SingleOrDefault(si => si.Role == "Master");
                var serviceAdresses = services.Where(s => s.IsMaster != true).Select(s => (s as UserService).Address).ToList();
                masterService = kernel.Get(Type.GetType(masterElement.Type, true, false), new ConstructorArgument("address", GetAddress(masterElement)),
                    new ConstructorArgument("services", serviceAdresses)) as UserService;
                LoadServiceState();
                services.Add(masterService);
            }
            return new ProxyService(services);
        }

        /// <summary>
        /// Saves master service state to xml file.
        /// </summary>
        /// <param name="masterService"> IMasterUserService instance.</param>
        /// <param name="filePath"> path to xml file.</param>
        public static void SaveServiceState()
        {
            var filePath = ConfigurationManager.AppSettings["Path"];
            if (!File.Exists(filePath))
            {
                using (var myFile = File.Create(filePath)) { };
            }
            using (var xmlWriter = XmlWriter.Create(filePath))
            {
                masterService.WriteXml(xmlWriter);
            }
        }
        #endregion

        #region Private Methods

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
            var typeToLoad = kernel.Get(Type.GetType(serviceType, true, false)).GetType().FullName;
            string assemblyToLoad;

            var repository = kernel.Get<IUserRepository>();
            var generator = kernel.Get<IGenerator<int>>();
            var validator = kernel.Get<IUserValidator>();

            UserService service;
            try
            {
                assemblyToLoad = Type.GetType(serviceType, true, false).Assembly.FullName;
                service = domain.CreateInstanceAndUnwrap(assemblyToLoad, typeToLoad, false, BindingFlags.Default, null, new object[] { generator, validator, repository, address }, null, null) as UserService;
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

        private static IPEndPoint GetAddress(ServiceElement si)
        {
            var ip = IPAddress.Parse(si.Host);
            var port = int.Parse(si.Port);
            var address = new IPEndPoint(ip, port);
            return address;
        }
        #endregion
    }
}
