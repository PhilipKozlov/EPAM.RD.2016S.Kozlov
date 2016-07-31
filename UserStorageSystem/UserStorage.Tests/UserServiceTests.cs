using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IDGenerator;
using Validator;
using DAL;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace UserStorage.Tests
{
    [TestClass]
    public class UserServiceTests
    {
        private readonly IUserRepository userRepository = new InMemoryUserRepository();
        private readonly IGenerator<int> idGenerator = new PrimeGenerator();
        private readonly IUserValidator userValidator = new UserValidator();
        private readonly IPEndPoint address = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1111);


        [TestMethod]
        public void CreateUser_NewUser_ReturnOne()
        {
            var userService = new UserService(this.idGenerator, this.userValidator, this.userRepository, this.address, new List<IPEndPoint>());
            var user = new User
            {
                Name = "John",
                LastName = "Doe",
                PersonalId = "12345678901234"
            };
            var expected = 1;
            var actual = userService.CreateUser(user);
            Assert.AreEqual(expected, actual.Id);
        }

        [TestMethod]
        public void DeleteUser_ExistingUser_UserDeleted()
        {
            var userService = new UserService(this.idGenerator, this.userValidator, this.userRepository, this.address, new List<IPEndPoint>());
            var user = new User
            {
                Name = "John",
                LastName = "Doe",
                PersonalId = "12345678901234"
            };
            var expected = 0;
            userService.CreateUser(user);
            userService.DeleteUser(user);
            Assert.AreEqual(expected, userRepository.GetAll().Count);
        }

        [TestMethod]
        public void FindByName_John_ReturnIEnumerableOfOneUser()
        {
            var userService = new UserService(this.idGenerator, this.userValidator, this.userRepository, this.address, new List<IPEndPoint>());
            var user = new User
            {
                Name = "John",
                LastName = "Doe",
                PersonalId = "12345678901234"
            };
            userService.CreateUser(user);
            var expected = userService.FindByName("John");
            Assert.AreEqual(expected.ElementAt(0), user);
        }

        [TestMethod]
        public void FindByNameAndLastName_JohnDoe_ReturnIEnumerableOfOneUser()
        {
            var userService = new UserService(this.idGenerator, this.userValidator, this.userRepository, this.address, new List<IPEndPoint>());
            var user = new User
            {
                Name = "John",
                LastName = "Doe",
                PersonalId = "12345678901234"
            };
            userService.CreateUser(user);
            var expected = userService.FindByNameAndLastName("John", "Doe");
            Assert.AreEqual(expected.ElementAt(0), user);
        }

        [TestMethod]
        public void FindByPersonalId_12345678901234_ReturnIEnumerableOfOneUser()
        {
            var userService = new UserService(this.idGenerator, this.userValidator, this.userRepository, this.address, new List<IPEndPoint>());
            var user = new User
            {
                Name = "John",
                LastName = "Doe",
                PersonalId = "12345678901234"
            };
            userService.CreateUser(user);
            var expected = userService.FindByPersonalId("12345678901234");
            Assert.AreEqual(expected.ElementAt(0), user);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void CreateUser_NewUser_NotSupportedException()
        {
            var userService = new UserService(this.idGenerator, this.userValidator, this.userRepository, this.address);
            userService.CreateUser(new User());
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void DeleteUser_User_NotSupportedException()
        {
            var userService = new UserService(this.idGenerator, this.userValidator, this.userRepository, this.address);
            userService.DeleteUser(new User());
        }

    }
}
