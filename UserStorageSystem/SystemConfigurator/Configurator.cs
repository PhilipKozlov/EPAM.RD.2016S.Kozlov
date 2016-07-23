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

namespace SystemConfigurator
{
    /// <summary>
    /// Configures services.
    /// </summary>
    public static class Configurator
    {
        #region Fields
        private static IUserService masterService;
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
            // TODO : Add master/slave related exceptions.
            
            var services = new List<IUserService>();
            var section = (ServiceConfigSection)ConfigurationManager.GetSection("ServiceConfig");
            if (section != null)
            {
                var i = 0;
                foreach (var si in section.ServiceItems)
                {
                    IUserService service = null;
                    if ((si as ServiceElement).Role == "Master")
                    {
                        service = kernel.Get(Type.GetType((si as ServiceElement).Type, true, false)) as IUserService;
                        //service = CreateMasterInAppDomain($"MasterServiceDomain{i}", (si as ServiceElement).Type);
                        masterService = service;
                        masterService.IsMaster = true;
                        LoadServiceState();
                    }
                    if ((si as ServiceElement).Role == "Slave")
                    {
                        service = CreateServiceInAppDomain($"SlaveServiceDomain{i}", (si as ServiceElement).Type);
                    }
                    services.Add(service);
                    i++;
                }
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

        private static IUserService CreateServiceInAppDomain(string domainName, string slaveType)
        {
            var domain = AppDomain.CreateDomain(domainName, null, null);
            var typeToLoad = kernel.Get(Type.GetType(slaveType, true, false)).GetType().FullName;
            string assemblyToLoad;

            // hardcode
            var repository = kernel.Get<IUserRepository>();
            //
            IUserService service;
            try
            {
                assemblyToLoad = Type.GetType(slaveType, true, false).Assembly.FullName;
                service = domain.CreateInstanceAndUnwrap(assemblyToLoad, typeToLoad, false, BindingFlags.Default, null, new object[] { masterService, repository }, null, null) as IUserService;
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

        //private static IUserService CreateMasterInAppDomain(string domainName, string masterType)
        //{
        //    var domain = AppDomain.CreateDomain(domainName, null, null);
        //    var typeToLoad = kernel.Get(Type.GetType(masterType, true, false)).GetType().FullName;
        //    string assemblyToLoad;

        //    // hardcode
        //    var repository = kernel.Get<IUserRepository>();
        //    var generator = kernel.Get<IGenerator<int>>();
        //    var validator = kernel.Get<IUserValidator>();
        //    //

        //    IUserService service;
        //    try
        //    {
        //        assemblyToLoad = Type.GetType(masterType, true, false).Assembly.FullName;
        //        service = domain.CreateInstanceAndUnwrap(assemblyToLoad, typeToLoad, false, BindingFlags.Default, null, new object[] { generator, validator, repository }, null, null) as IUserService;
        //    }

        //    return service;
        //}
        #endregion
    }
}
