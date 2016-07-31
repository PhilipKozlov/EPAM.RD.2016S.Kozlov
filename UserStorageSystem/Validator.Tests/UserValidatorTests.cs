using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UserStorage;

namespace Validator.Tests
{
    [TestClass]
    public class UserValidatorTests
    {
        private readonly UserValidator validator = new UserValidator();

        [TestMethod]
        public void IsValid_ValidUser_ReturnTrue()
        {
            var user = new User
            {
                Name = "John",
                LastName = "Doe",
                PersonalId = "12345678901234"
            };
            var actual = this.validator.IsValid(user);
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void IsValid_EmptyUser_ReturnFalse()
        {
            var actual = this.validator.IsValid(new User());
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void IsValid_NullUser_ReturnFalse()
        {
            var actual = this.validator.IsValid(null);
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void IsValid_UserWithInvalidPersonalId_ReturnFalse()
        {
            var actual = this.validator.IsValid(new User());
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void IsValid_UserWithPersonalIdMoreThan14Characters_ReturnFalse()
        {
            var user = new User {PersonalId = "1111111111111111"};
            var actual = this.validator.IsValid(user);
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void IsValid_UserWithPersonalIdLessThan14Characters_ReturnFalse()
        {
            var user = new User {PersonalId = "11"};
            var actual = this.validator.IsValid(user);
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void IsValid_UserWithNameContainingNonLetters_ReturnFalse()
        {
            var user = new User {Name = "10"};
            var actual = this.validator.IsValid(user);
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void IsValid_UserWithLastNameContainingNonLetters_ReturnFalse()
        {
            var user = new User {LastName = "10"};
            var actual = this.validator.IsValid(user);
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void IsValid_UserWithVisaStartDateGreaterThanEndDate_ReturnFalse()
        {
            var user = new User();
            var visa = new VisaRecord {Start = DateTime.Now};
            var date = DateTime.Now;
            visa.End = date.AddDays(-1);
            user.VisaRecords.Add(visa);
            var actual = this.validator.IsValid(user);
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void IsValid_UserWithVisaCountryContainingNonLetters_ReturnFalse()
        {
            var user = new User();
            var visa = new VisaRecord {Start = DateTime.Now};
            var date = DateTime.Now;
            visa.End = date.AddDays(10);
            visa.Country = "10";
            user.VisaRecords.Add(visa);
            var actual = this.validator.IsValid(user);
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void IsValid_UserWithOneOfTwoVisasInvalid_ReturnFalse()
        {
            var user = new User();
            var visa = new VisaRecord {Start = DateTime.Now};
            var date = DateTime.Now;
            visa.End = date.AddDays(10);
            visa.Country = "10";
            var invalidVisa = new VisaRecord();
            user.VisaRecords.Add(visa);
            user.VisaRecords.Add(invalidVisa);
            var actual = this.validator.IsValid(user);
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void IsValid_UserWithNullVisaRecords_ReturnFalse()
        {
            var user = new User {VisaRecords = null};
            var actual = this.validator.IsValid(user);
            Assert.IsFalse(actual);
        }


    }
}
