using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Attributes;
using UserManager;
using System.Linq;

namespace UserManager.Tests
{
    [TestClass]
    public class UserValidatorTests
    {
        [TestMethod]
        public void ValidateUser_ValidUser_ReturnTrue()
        {
            var actual = UserValidator.ValidateUser(new User(1));
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void ValidateUser_UserWithInvalidId_ReturnFalse()
        {
            var actual = UserValidator.ValidateUser(new User(int.MaxValue));
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void ValidateUser_UserWithNameLongerThanThirtyCharacters_ReturnFalse()
        {
            var user = new User(1);
            user.FirstName = new string('*', 100);
            var actual = UserValidator.ValidateUser(user);
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void ValidateUser_UserWithLastNameLongerThanThirtyCharacters_ReturnFalse()
        {
            var user = new User(1);
            user.LastName = new string('*', 100);
            var actual = UserValidator.ValidateUser(user);
            Assert.IsFalse(actual);
        }
    }
}
