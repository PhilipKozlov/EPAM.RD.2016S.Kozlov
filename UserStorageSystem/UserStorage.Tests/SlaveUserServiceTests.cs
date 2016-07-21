using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DAL;
using IDGenerator;
using Validator;

namespace UserStorage.Tests
{
    [TestClass]
    public class SlaveUserServiceTests
    {
        private IUserRepository userRepository = new InMemoryUserRepository();
        private IUserRepository slaveRepository = new InMemoryUserRepository();
        private IGenerator<int> idGenerator = new PrimeGenerator();
        private IUserValidator userValidator = new UserValidator();

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void CreateUser_NewUser_NotSupportedException()
        {
            var masterUserService = new MasterUserService(idGenerator, userValidator, userRepository);
            var userService = new SlaveUserService(masterUserService, userRepository);
            var actual = userService.CreateUser(new User());
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void DeleteUser_User_NotSupportedException()
        {
            var masterUserService = new MasterUserService(idGenerator, userValidator, userRepository);
            var userService = new SlaveUserService(masterUserService, userRepository);
            userService.DeleteUser(new User());
        }

        [TestMethod]
        public void FindByName_John_ReturnIEnumerableOfOneUser()
        {
            var masterUserService = new MasterUserService(idGenerator, userValidator, userRepository);
            var userService = new SlaveUserService(masterUserService, slaveRepository);
            var user = new User()
            {
                Name = "John",
                LastName = "Doe",
                PersonalId = "12345678901234"
            };
            masterUserService.CreateUser(user);
            var expected = userService.FindByName("John");
            Assert.AreEqual(expected.ElementAt(0), user.Id);
        }

        [TestMethod]
        public void FindByNameAndLastName_JohnDoe_ReturnIEnumerableOfOneUser()
        {
            var masterUserService = new MasterUserService(idGenerator, userValidator, userRepository);
            var userService = new SlaveUserService(masterUserService, slaveRepository);
            var user = new User()
            {
                Name = "John",
                LastName = "Doe",
                PersonalId = "12345678901234"
            };
            masterUserService.CreateUser(user);
            var expected = userService.FindByNameAndLastName("John", "Doe");
            Assert.AreEqual(expected.ElementAt(0), user.Id);
        }

        [TestMethod]
        public void FindByPersonalId_12345678901234_ReturnIEnumerableOfOneUser()
        {
            var masterUserService = new MasterUserService(idGenerator, userValidator, userRepository);
            var userService = new SlaveUserService(masterUserService, slaveRepository);
            var user = new User()
            {
                Name = "John",
                LastName = "Doe",
                PersonalId = "12345678901234"
            };
            masterUserService.CreateUser(user);
            var expected = userService.FindByPersonalId("12345678901234");
            Assert.AreEqual(expected.ElementAt(0), user.Id);
        }
    }
}
