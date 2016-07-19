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

namespace SystemConfigurator
{
    /// <summary>
    /// Configures services.
    /// </summary>
    public static class Configurator
    {
        #region Fields
        private static IMasterUserService masterService;
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets a collection of services.
        /// </summary>
        /// <returns> Collection of services.</returns>
        public static List<IUserService> ConfigureServices()
        {
            var kernel = new StandardKernel();
            kernel.Load<Resolver>();

            var serviceDict = new Dictionary<string, int>();
            ReadConfig(serviceDict);
            if (serviceDict["Master"] > 1 || serviceDict["Master"] < 0) throw new InvalidOperationException("Master service must be only one.");
            if (serviceDict["Slave"] < 0) throw new InvalidOperationException("Must have zero o more slave services.");

            var services = new List<IUserService>();

            // instantiate master
            masterService = kernel.Get<IMasterUserService>();
            services.Add(masterService);
            LoadServiceState();

            // instantiate slaves
            for (int i = 0; i < serviceDict["Slave"]; i++)
            {
                var slaveService = kernel.Get<IUserService>();
                services.Add(slaveService);
            }
            return services;
        }


        /// <summary>
        /// Saves master service state to xml file.
        /// </summary>
        /// <param name="masterService"> IMasterUserService instance.</param>
        /// <param name="filePath"> path to xml file.</param>
        public static void SaveServiceState()
        {
            var filePath = ConfigurationManager.AppSettings["Path"];
            using (var xmlWriter = XmlWriter.Create(filePath))
            {
                (masterService as MastrerUserService).WriteXml(xmlWriter);
            }
        }
        #endregion

        #region Private Methods

        private static void ReadConfig(Dictionary<string, int> serviceDict)
        {
            var section = (ServiceConfigSection)ConfigurationManager.GetSection("ServiceConfig");
            if (section != null)
            {
                foreach (var si in section.ServiceItems)
                {
                    var type = (si as ServiceElement).ServiceType;
                    var number = Convert.ToInt32((si as ServiceElement).Number);
                    serviceDict.Add(type, number);
                }
            }
        }
        private static void LoadServiceState()
        {
            var filePath = ConfigurationManager.AppSettings["Path"];
            using (var xmlReader = XmlReader.Create(filePath))
            {
                (masterService as MastrerUserService).ReadXml(xmlReader);
            }
        }
        #endregion
    }
}
