namespace UserStorage.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using DAL;
    using IDGenerator;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Validator;

    [TestClass]
    public class ProxyServiceTests
    {
        private static readonly IUserRepository MasterRepository = new InMemoryUserRepository();
        private static readonly IUserRepository SlaveRepository = new InMemoryUserRepository();
        private static readonly IGenerator<int> IdGenerator = new PrimeGenerator();
        private static readonly IUserValidator UserValidator = new UserValidator();
        private static readonly IPEndPoint MasterAddress = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1111);
        private static readonly IPEndPoint SlaveAddress = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2222);
        private static readonly IPEndPoint ProxyAddress = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3333);
        private static readonly IUserService Master = new UserService(IdGenerator, UserValidator, MasterRepository, MasterAddress, new List<IPEndPoint>());
        private static readonly IUserService Slave = new UserService(IdGenerator, UserValidator, SlaveRepository, SlaveAddress, 1234);
        private static readonly IList<IUserService> Services = new List<IUserService> { Master, Slave };
        private readonly ProxyService proxy = new ProxyService(ProxyAddress, Services);

        [TestMethod]
        public void CreateUser_NewUser_ReturnUserWithIdOne()
        {
            var user = new User
            {
                Name = "John",
                LastName = "Doe",
                PersonalId = "12345678901234"
            };
            var expected = 1;
            var actual = this.proxy.CreateUser(user);
            Assert.AreEqual(expected, actual.Id);
        }

        [TestMethod]
        public void DeleteUser_ExistingUser_UserDeleted()
        {
            var user = new User
            {
                Name = "John",
                LastName = "Doe",
                PersonalId = "12345678901234"
            };
            var expected = 0;
            this.proxy.DeleteUser(user);
            Assert.AreEqual(expected, MasterRepository.GetAll().Count);
            Assert.AreEqual(expected, SlaveRepository.GetAll().Count);
        }

        [TestMethod]
        public void FindByName_John_ReturnIEnumerableOfOneUser()
        {
            var user = new User
            {
                Name = "John",
                LastName = "Doe",
                PersonalId = "12345678901234"
            };
            this.proxy.CreateUser(user);
            var expected = this.proxy.FindByName("John");
            Assert.AreEqual(expected.ElementAt(0), user);
        }

        [TestMethod]
        public void FindByNameAndLastName_JohnDoe_ReturnIEnumerableOfOneUser()
        {
            var user = new User
            {
                Name = "John",
                LastName = "Doe",
                PersonalId = "12345678901234"
            };
            this.proxy.CreateUser(user);
            var expected = this.proxy.FindByNameAndLastName("John", "Doe");
            Assert.AreEqual(expected.ElementAt(0), user);
        }

        [TestMethod]
        public void FindByPersonalId_12345678901234_ReturnIEnumerableOfOneUser()
        {
            var user = new User
            {
                Name = "John",
                LastName = "Doe",
                PersonalId = "12345678901234"
            };
            this.proxy.CreateUser(user);
            var expected = this.proxy.FindByPersonalId("12345678901234");
            Assert.AreEqual(expected.ElementAt(0), user);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_NullAddress_ArgumentNullException()
        {
            var userService = new ProxyService(null, Services);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_NullServices_ArgumentNullException()
        {
            var userService = new ProxyService(ProxyAddress, null);
        }
    }
}
