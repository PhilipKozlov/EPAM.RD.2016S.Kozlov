namespace UserStorage.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class UserTests
    {
        private readonly User user1 = new User
        {
            Name = "John",
            LastName = "Smith",
            DateOfBirth = DateTime.Now,
            PersonalId = "12345678901234",
            Gender = Gender.Male
        };

        private readonly User user2 = new User
        {
            Name = "John",
            LastName = "Smith",
            DateOfBirth = DateTime.Now,
            PersonalId = "12345678901234",
            Gender = Gender.Male
        };

        private readonly User user3 = new User
        {
            Name = "Jane",
            LastName = "Doe",
            DateOfBirth = DateTime.Now,
            PersonalId = "13342278911239",
            Gender = Gender.Female
        };

        [TestMethod]
        public void Equals_TwoEqualUsers_ReturnTrue()
        {
            Assert.IsTrue(this.user1.Equals(this.user2));
        }

        [TestMethod]
        public void Equals_TwoNotEqualUsers_ReturnFalse()
        {
            Assert.IsFalse(this.user1.Equals(this.user3));
        }

        [TestMethod]
        public void Equals_UserAndObject_ReturnFalse()
        {
            Assert.IsFalse(this.user1.Equals(new object()));
        }

        [TestMethod]
        public void Equals_UserAndEqualUserObject_ReturnTrue()
        {
            object user = this.user2;
            Assert.IsTrue(this.user1.Equals(user));
        }

        [TestMethod]
        public void Equals_UserAndNotEqualUserObject_ReturnFalse()
        {
            object user = this.user3;
            Assert.IsFalse(this.user1.Equals(user));
        }

        [TestMethod]
        public void GetHashCode_TwoEqualUsers_ReturnSameHashCode()
        {
            Assert.IsTrue(this.user1.GetHashCode() == this.user2.GetHashCode());
        }

        [TestMethod]
        public void GetHashCode_TwoNotEqualUsers_ReturnDifferentHashCode()
        {
            Assert.IsFalse(this.user1.GetHashCode() == this.user3.GetHashCode());
        }

        [TestMethod]
        public void ToString_User_ReturnStringRepresentation()
        {
            var expected = "0 John Smith 12345678901234";
            var actual = this.user1.ToString();
            Assert.AreEqual(expected, actual);
        }
    }
}
