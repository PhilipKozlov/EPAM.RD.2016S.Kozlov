using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UserStorage;
using IDGenerator;
using Validator;
using DAL;
using System.Collections.Generic;
using System.Linq;

namespace UserStorage.Tests
{
    [TestClass]
    public class UserServiceTests
    {
        private IUserRepository userRepository = new InMemoryUserRepository();
        private IGenerator<int> idGenerator = new PrimeGenerator();
        private IUserValidator userValidator = new UserValidator();


        [TestMethod]
        public void CreateUser_NewUser_ReturnOne()
        {
            var userService = new UserService(idGenerator, userValidator, userRepository);
            userService.IsMaster = true;
            var user = new User()
            {
                Name = "John",
                LastName = "Doe",
                PersonalId = "12345678901234"
            };
            var expected = 1;
            var actual = userService.CreateUser(user);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void FindByName_John_ReturnIEnumerableOfOneUser()
        {
            var userService = new UserService(idGenerator, userValidator, userRepository);
            userService.IsMaster = true;
            var user = new User()
            {
                Name = "John",
                LastName = "Doe",
                PersonalId = "12345678901234"
            };
            userService.CreateUser(user);
            var expected = userService.FindByName("John");
            Assert.AreEqual(expected.ElementAt(0), user.Id);
        }

        [TestMethod]
        public void FindByNameAndLastName_JohnDoe_ReturnIEnumerableOfOneUser()
        {
            var userService = new UserService(idGenerator, userValidator, userRepository);
            userService.IsMaster = true;
            var user = new User()
            {
                Name = "John",
                LastName = "Doe",
                PersonalId = "12345678901234"
            };
            userService.CreateUser(user);
            var expected = userService.FindByNameAndLastName("John", "Doe");
            Assert.AreEqual(expected.ElementAt(0), user.Id);
        }

        [TestMethod]
        public void FindByPersonalId_12345678901234_ReturnIEnumerableOfOneUser()
        {
            var userService = new UserService(idGenerator, userValidator, userRepository);
            userService.IsMaster = true;
            var user = new User()
            {
                Name = "John",
                LastName = "Doe",
                PersonalId = "12345678901234"
            };
            userService.CreateUser(user);
            var expected = userService.FindByPersonalId("12345678901234");
            Assert.AreEqual(expected.ElementAt(0), user.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void CreateUser_NewUser_NotSupportedException()
        {
            var masterUserService = new UserService(idGenerator, userValidator, userRepository);
            var userService = new UserService(masterUserService, userRepository);
            var actual = userService.CreateUser(new User());
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void DeleteUser_User_NotSupportedException()
        {
            var masterUserService = new UserService(idGenerator, userValidator, userRepository);
            var userService = new UserService(masterUserService, userRepository);
            userService.DeleteUser(new User());
        }

    }
}
