﻿using Ninject.Modules;
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
            Bind<IUserService>().To<UserService>().InTransientScope();
            Bind<IUserValidator>().To<UserValidator>().InTransientScope();
            Bind<IGenerator<int>>().To<PrimeGenerator>().InSingletonScope();
        }
        #endregion
    }

}
