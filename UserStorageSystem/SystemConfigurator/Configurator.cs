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
            var serviceDict = new Dictionary<string, int>();
            ReadConfig(serviceDict);
            if (serviceDict["Master"] > 1 || serviceDict["Master"] < 0) throw new InvalidOperationException("Master service must be only one.");
            if (serviceDict["Slave"] < 0) throw new InvalidOperationException("Must have zero o more slave services.");
            InstanciateServices(serviceDict);
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

        private void ReadConfig(Dictionary<string, int> serviceDict)
        {
            var section = (ServiceConfigSection)ConfigurationManager.GetSection("ServiceConfig");
            if (section != null)
            {
                foreach (var si in section.ServiceItems)
                {
                    var type = (si as ServiceElement).Role;
                    var number = Convert.ToInt32((si as ServiceElement).Number);
                    serviceDict.Add(type, number);
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

        private void InstanciateServices(Dictionary<string, int> serviceDict)
        {
            masterService = CreateMasterInAppDomain("MasterServiceDomain");
            services.Add(masterService);
            LoadServiceState();

            for (int i = 0; i < serviceDict["Slave"]; i++)
            {
                var slaveService = CreateSlaveInAppDomain($"SlaveServiceDomain{i}");
                services.Add(slaveService);
            }
        }

        private IUserService CreateSlaveInAppDomain(string domainName)
        {
            var domain = AppDomain.CreateDomain(domainName, null, null);
            var repository = kernel.Get<IUserRepository>();
            var typeToLoad = kernel.Get<IUserService>().GetType().FullName;
            var assemblyToLoad = typeof(IUserService).Assembly.FullName;
            var service = domain.CreateInstanceAndUnwrap(assemblyToLoad, typeToLoad, false, BindingFlags.Default, null, new object[] { masterService, repository }, null, null) as IUserService;
            return service;
        }

        private IMasterUserService CreateMasterInAppDomain(string domainName)
        {
            var domain = AppDomain.CreateDomain(domainName, null, null);
            var repository = kernel.Get<IUserRepository>();
            var generator = kernel.Get<IGenerator<int>>();
            var validator = kernel.Get<IUserValidator>();
            var typeToLoad = kernel.Get<IMasterUserService>().GetType().FullName;
            var assemblyToLoad = typeof(IMasterUserService).Assembly.FullName;
            var service = domain.CreateInstanceAndUnwrap(assemblyToLoad, typeToLoad, false, BindingFlags.Default, null, new object[] { generator, validator, repository }, null, null) as IMasterUserService;
            return service;
        }
        #endregion
    }
}
