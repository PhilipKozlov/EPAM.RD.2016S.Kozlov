using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Attributes;
using UserManager;
using System.Linq;

namespace UserManager.Tests
{
    [TestClass]
    public class UserCreatorTests
    {
        [TestMethod]
        public void CreateUsers_ReturnCollectionOfFourUsers()
        {
            var actual = UserCreator.CreateUsers();
            Assert.AreEqual(4, actual.Count());
        }
    }
}
