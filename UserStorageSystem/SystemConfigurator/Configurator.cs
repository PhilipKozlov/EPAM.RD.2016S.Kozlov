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

namespace SystemConfigurator
{
    /// <summary>
    /// Configures services.
    /// </summary>
    public class Configurator
    {
        #region Fields
        private IMasterUserService masterService;
        private List<IUserService> services;
        StandardKernel kernel;
        #endregion

        #region Constructors
        public Configurator()
        {
            services = new List<IUserService>();
            kernel = new StandardKernel();
            kernel.Load<Resolver>();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the collection of services.
        /// </summary>
        public IReadOnlyCollection<IUserService> Services => services;
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets a collection of services.
        /// </summary>
        /// <returns> Collection of services.</returns>
        public void ConfigurateServices()
        {
            //Read config
            ConfigurateServicesHelper();
            //var masterCount = services.Where(s => s.GetType() == typeof(IMasterUserService)).Count();
            //if (masterCount > 1 || masterCount < 1) throw new InvalidOperationException("Master service must be only one.");
        }

        /// <summary>
        /// Saves master service state to xml file.
        /// </summary>
        /// <param name="masterService"> IMasterUserService instance.</param>
        /// <param name="filePath"> path to xml file.</param>
        public void SaveServiceState()
        {
            var filePath = ConfigurationManager.AppSettings["Path"];
            if (!File.Exists(filePath))
            {
                using (var myFile = File.Create(filePath)) { };
            }
            (masterService as MasterUserService)?.WriteXml(filePath);
        }
        #endregion

        #region Private Methods

        private void ConfigurateServicesHelper()
        {
            var section = (ServiceConfigSection)ConfigurationManager.GetSection("ServiceConfig");
            if (section != null)
            {
                var i = 0;
                foreach (var si in section.ServiceItems)
                {
                    IUserService service = null;
                    if ((si as ServiceElement).Role == "Master")
                    {
                        service = CreateMasterInAppDomain($"MasterServiceDomain{i}", (si as ServiceElement).Type);
                        masterService = service as IMasterUserService;
                        LoadServiceState();
                    }
                    if ((si as ServiceElement).Role == "Slave")
                    {
                        service = CreateSlaveInAppDomain($"SlaveServiceDomain{i}", (si as ServiceElement).Type);
                    }
                    services.Add(service);
                    i++;
                }
            }
        }

        private void LoadServiceState()
        {
            var filePath = ConfigurationManager.AppSettings["Path"];
            if (!File.Exists(filePath))
            {
                using (var myFile = File.Create(filePath)) { };
            }
            (masterService as MasterUserService)?.ReadXml(filePath);
        }

        private IUserService CreateSlaveInAppDomain(string domainName, string slaveType)
        {
            var domain = AppDomain.CreateDomain(domainName, null, null);
            var typeToLoad = kernel.Get(Type.GetType(slaveType, true, false)).GetType().FullName;
            var assemblyToLoad = Type.GetType(slaveType, true, false).Assembly.FullName;

            // hardcode
            var repository = kernel.Get<IUserRepository>();
            //

            var service = domain.CreateInstanceAndUnwrap(assemblyToLoad, typeToLoad, false, BindingFlags.Default, null, new object[] { masterService, repository }, null, null) as IUserService;
            return service;
        }

        private IMasterUserService CreateMasterInAppDomain(string domainName, string masterType)
        {
            var domain = AppDomain.CreateDomain(domainName, null, null);
            var typeToLoad = kernel.Get(Type.GetType(masterType, true, false)).GetType().FullName;
            var assemblyToLoad = Type.GetType(masterType, true, false).Assembly.FullName;

            // hardcode
            var repository = kernel.Get<IUserRepository>();
            var generator = kernel.Get<IGenerator<int>>();
            var validator = kernel.Get<IUserValidator>();
            //

            var service = domain.CreateInstanceAndUnwrap(assemblyToLoad, typeToLoad, false, BindingFlags.Default, null, new object[] { generator, validator, repository }, null, null) as IMasterUserService;
            return service;
        }
        #endregion
    }
}
