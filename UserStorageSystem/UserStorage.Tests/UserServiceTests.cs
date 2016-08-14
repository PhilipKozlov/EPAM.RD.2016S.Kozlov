namespace UserStorage.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.ServiceModel;
    using System.Xml;
    using DAL;
    using IDGenerator;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Validator;

    [TestClass]
    public class UserServiceTests
    {
        private readonly IUserRepository userRepository = new InMemoryUserRepository();
        private readonly IGenerator<int> idGenerator = new PrimeGenerator();
        private readonly IUserValidator userValidator = new UserValidator();
        private readonly IPEndPoint address = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1111);

        [TestMethod]
        public void CreateUser_NewUser_ReturnUserWithIdOne()
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
            Assert.AreEqual(expected, this.userRepository.GetAll().Count);
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
        [ExpectedException(typeof(FaultException))]
        public void CreateUser_CreatingUserOnSlave_FaultException()
        {
            var userService = new UserService(this.idGenerator, this.userValidator, this.userRepository, this.address, 0);
            userService.CreateUser(new User());
        }

        [TestMethod]
        [ExpectedException(typeof(FaultException))]
        public void DeleteUser_DeletingUserFromSlave_FaultException()
        {
            var userService = new UserService(this.idGenerator, this.userValidator, this.userRepository, this.address, 0);
            userService.DeleteUser(new User());
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void ReadXml_OnSlave_NotSupportedException()
        {
            var userService = new UserService(this.idGenerator, this.userValidator, this.userRepository, this.address, 0);
            using (var reader = XmlReader.Create("test.xml"))
            {
                userService.ReadXml(reader);
            }
        }

        [TestMethod]
        public void ReadXml_EmptyXmlFile_NoExceptionIsThrown()
        {
            var userService = new UserService(this.idGenerator, this.userValidator, this.userRepository, this.address, new List<IPEndPoint>());
            using (var reader = XmlReader.Create("test.xml"))
            {
                userService.ReadXml(reader);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void WriteXml_OnSlave_NotSupportedException()
        {
            var userService = new UserService(this.idGenerator, this.userValidator, this.userRepository, this.address, 0);
            using (var writer = XmlWriter.Create("test.xml"))
            {
                userService.WriteXml(writer);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WriteXml_NullXmlWriter_InvalidOperationException()
        {
            var userService = new UserService(this.idGenerator, this.userValidator, this.userRepository, this.address, new List<IPEndPoint>());
            userService.WriteXml(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_NullIdGenerator_ArgumentNullException()
        {
            var userService = new UserService(null, this.userValidator, this.userRepository, this.address, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_NullUserValidator_ArgumentNullException()
        {
            var userService = new UserService(this.idGenerator, null, this.userRepository, this.address, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_NullUserRepository_ArgumentNullException()
        {
            var userService = new UserService(this.idGenerator, this.userValidator, null, this.address, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_NullAddress_ArgumentNullException()
        {
            var userService = new UserService(this.idGenerator, this.userValidator, this.userRepository, null, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_NullServices_ArgumentNullException()
        {
            var userService = new UserService(this.idGenerator, this.userValidator, this.userRepository, this.address, null);
        }
    }
}
