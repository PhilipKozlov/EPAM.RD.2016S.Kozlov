using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Attributes;
using System.Reflection;
using System.ComponentModel;

namespace UserManager
{
    /// <summary>
    /// Provides functionality for creating users.
    /// </summary>
    public static class UserCreator
    {
        #region Public Methods
        /// <summary>
        /// Creates users using reflection.
        /// </summary>
        /// <returns> Collection of users.</returns>
        public static IEnumerable<User> CreateUsers()
        {
            var users = new List<User>();

            var asm = Assembly.LoadFrom("Attributes.dll");

            foreach (var advancedUserAttr in asm.GetCustomAttributes<InstantiateAdvancedUserAttribute>())
            {
                var advancedUserType = typeof(AdvancedUser);
                var advancedUserCtor = advancedUserType.GetConstructor(new[] { typeof(int), typeof(int) });
                AdvancedUser advancedUser;

                if (advancedUserAttr.Id == 0)
                {
                    advancedUser = GetUserWithoutId(advancedUserCtor, advancedUserType) as AdvancedUser;
                }
                else
                {
                    advancedUser = advancedUserCtor.Invoke(new object[] { advancedUserAttr.Id, advancedUserAttr.ExternalId }) as AdvancedUser;
                }

                advancedUser.FirstName = advancedUserAttr.Name;
                advancedUser.LastName = advancedUserAttr.LastName;
                if (UserValidator.ValidateUser(advancedUser)) users.Add(advancedUser);
            }


            var userType = typeof(User);
            foreach (var userAttr in userType.GetCustomAttributes<InstantiateUserAttribute>())
            {
                var userCtor = userType.GetConstructor(new[] { typeof(int) });
                User userInstance;

                if (userAttr.Id == 0)
                {
                    userInstance = GetUserWithoutId(userCtor, userType) as User;
                }
                else
                {
                    userInstance = userCtor.Invoke(new object[] { userAttr.Id }) as User;
                }
                userInstance.FirstName = userAttr.Name;
                userInstance.LastName = userAttr.LastName;
                if (UserValidator.ValidateUser(userInstance)) users.Add(userInstance);
            }

            return users;
        }
        #endregion

        #region Private Methods
        private static object GetUserWithoutId(ConstructorInfo ctor, Type userType)
        {
            var propNames = ctor.GetCustomAttributes<MatchParameterWithPropertyAttribute>().Select(attr => attr.PropertyName);
            var propValues = new List<object>();
            foreach (var propName in propNames)
            {
                var propDefaultValue = userType.GetProperty(propName).GetCustomAttribute<DefaultValueAttribute>().Value;
                propValues.Add(propDefaultValue);
            }
            return ctor.Invoke(propValues.ToArray());
        }
        #endregion
    }
}
