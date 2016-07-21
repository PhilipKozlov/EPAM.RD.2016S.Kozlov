using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserStorage;
using DAL;
using Validator;
using IDGenerator;

namespace CompositionRoot
{
    /// <summary>
    /// Provides bindings for application.
    /// </summary>
    public class Resolver : NinjectModule
    {
        #region Constructors
        /// <summary>
        /// Loads NInject module.
        /// </summary>
        public override void Load()
        {
            LoadDal();
            LoadBll();
        }
        #endregion

        #region Private Methods
        private void LoadDal()
        {
            Bind<IUserRepository>().To<InMemoryUserRepository>().InTransientScope();
        }

        private void LoadBll()
        {
            Bind<IUserService>().To<SlaveUserService>().InTransientScope();
            Bind<IMasterUserService>().To<MasterUserService>().InSingletonScope();
            Bind<IUserValidator>().To<UserValidator>().InTransientScope();
            Bind<IGenerator<int>>().To<PrimeGenerator>().InSingletonScope();
        }
        #endregion
    }

}
