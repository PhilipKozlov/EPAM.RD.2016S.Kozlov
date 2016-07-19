using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UserStorage;

namespace Validator.Tests
{
    [TestClass]
    public class UserValidatorTests
    {
        private UserValidator validator = new UserValidator();

        [TestMethod]
        public void IsValid_ValidUser_ReturnTrue()
        {
            var user = new User()
            {
                Name = "John",
                LastName = "Doe",
                PersonalId = "12345678901234"
            };
            var actual = validator.IsValid(user);
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void IsValid_EmptyUser_ReturnFalse()
        {
            var actual = validator.IsValid(new User());
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void IsValid_Null_ReturnFalse()
        {
            var actual = validator.IsValid(null);
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void IsValid_UserWithInvalidPersonalId_ReturnFalse()
        {
            var actual = validator.IsValid(new User());
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void IsValid_UserWithNameContainingNonLetters_ReturnFalse()
        {
            var user = new User();
            user.Name = "10";
            var actual = validator.IsValid(user);
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void IsValid_UserWithLastNameContainingNonLetters_ReturnFalse()
        {
            var user = new User();
            user.LastName = "10";
            var actual = validator.IsValid(user);
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void IsValid_UserWithVisaStartDateGreaterThanEndDate_ReturnFalse()
        {
            var user = new User();
            var visa = new VisaRecord();
            visa.Start = DateTime.Now;
            var date = DateTime.Now;
            visa.End = date.AddDays(-1);
            user.VisaRecords.Add(visa);
            var actual = validator.IsValid(user);
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void IsValid_UserWithVisaCountryContainingNonLetters_ReturnFalse()
        {
            var user = new User();
            var visa = new VisaRecord();
            visa.Start = DateTime.Now;
            var date = DateTime.Now;
            visa.End = date.AddDays(10);
            visa.Country = "10";
            user.VisaRecords.Add(visa);
            var actual = validator.IsValid(user);
            Assert.IsFalse(actual);
        }


    }
}
