namespace Validator.Tests
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using UserStorage;

    [TestClass]
    public class UserValidatorTests
    {
        private readonly UserValidator validator = new UserValidator();
        private User user = new User
        {
            Id = 1,
            Name = "John",
            LastName = "Smith",
            DateOfBirth = DateTime.Now,
            PersonalId = "12345678901234",
            Gender = Gender.Male,
            VisaRecords = new List<VisaRecord>()
        };

        [TestMethod]
        public void IsValid_ValidUser_ReturnTrue()
        {
            var actual = this.validator.IsValid(this.user);
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
        public void IsValid_UserWithNullPersonalId_ReturnFalse()
        {
            this.user.PersonalId = null;
            var actual = this.validator.IsValid(this.user);
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void IsValid_UserWithEmptyPersonalId_ReturnFalse()
        {
            this.user.PersonalId = string.Empty;
            var actual = this.validator.IsValid(this.user);
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void IsValid_UserWithPersonalIdMoreThan14Characters_ReturnFalse()
        {
            this.user.PersonalId = "1111111111111111";
            var actual = this.validator.IsValid(this.user);
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void IsValid_UserWithPersonalIdLessThan14Characters_ReturnFalse()
        {
            this.user.PersonalId = "111";
            var actual = this.validator.IsValid(this.user);
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void IsValid_UserWithNameContainingNonLetters_ReturnFalse()
        {
            this.user.Name = "11";
            var actual = this.validator.IsValid(this.user);
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void IsValid_UserWithLastNameContainingNonLetters_ReturnFalse()
        {
            this.user.LastName = "11";
            var actual = this.validator.IsValid(this.user);
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void IsValid_UserWithVisaStartDateGreaterThanEndDate_ReturnFalse()
        {
            var visa = new VisaRecord { Start = DateTime.Now };
            var date = DateTime.Now;
            visa.End = date.AddDays(-1);
            this.user.VisaRecords.Add(visa);
            var actual = this.validator.IsValid(this.user);
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void IsValid_UserWithVisaCountryContainingNonLetters_ReturnFalse()
        {
            var visa = new VisaRecord { Start = DateTime.Now };
            var date = DateTime.Now;
            visa.End = date.AddDays(10);
            visa.Country = "10";
            this.user.VisaRecords.Add(visa);
            var actual = this.validator.IsValid(this.user);
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void IsValid_UserWithOneOfTwoVisasInvalid_ReturnFalse()
        {
            var visa = new VisaRecord { Start = DateTime.Now };
            var date = DateTime.Now;
            visa.End = date.AddDays(10);
            visa.Country = "10";
            var invalidVisa = new VisaRecord();
            this.user.VisaRecords.Add(visa);
            this.user.VisaRecords.Add(invalidVisa);
            var actual = this.validator.IsValid(this.user);
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void IsValid_UserWithNullVisaRecords_ReturnFalse()
        {
            this.user.VisaRecords = null;
            var actual = this.validator.IsValid(this.user);
            Assert.IsFalse(actual);
        }
    }
}
