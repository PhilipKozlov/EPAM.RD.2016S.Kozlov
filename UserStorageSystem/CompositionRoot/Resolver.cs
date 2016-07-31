//-----------------------------------------------------------------------
// <copyright file="Resolver.cs" company="No Company">
//     No Company. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace CompositionRoot
{
    using DAL;
    using IDGenerator;
    using Ninject.Modules;
    using UserStorage;
    using Validator;

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
            this.LoadDal();
            this.LoadBll();
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
