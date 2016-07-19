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
        #endregion

        #region Constructors
        public Configurator()
        {
            services = new List<IUserService>();
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
                using (var myFile = File.Create(filePath)) ;
            }
            using (var xmlWriter = XmlWriter.Create(filePath))
            {
                (masterService as IXmlSerializable)?.WriteXml(xmlWriter);
            }
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
                using (var myFile = File.Create(filePath)) ;
            }
            using (var xmlReader = XmlReader.Create(filePath))
            {
                (masterService as IXmlSerializable)?.ReadXml(xmlReader);
            }
        }

        private void InstanciateServices(Dictionary<string, int> serviceDict)
        {
            // DI
            var kernel = new StandardKernel();
            kernel.Load<Resolver>();

            // instanciate services
            masterService = kernel.Get<IMasterUserService>();
            services.Add(masterService);
            LoadServiceState();
            for (int i = 0; i < serviceDict["Slave"]; i++)
            {
                var slaveService = kernel.Get<IUserService>();
                services.Add(slaveService);
            }
        }
        #endregion
    }
}
