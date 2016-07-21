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
    /// Provides functionality for validating users.
    /// </summary>
    public static class UserValidator
    {
        #region Public Methods
        /// <summary>
        /// Validates user entity.
        /// </summary>
        /// <param name="user"> User instance</param>
        /// <returns> True if user is valid; otherwise - false.</returns>
        public static bool ValidateUser(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (!ValidateFields(user)) return false;
            if (!ValidateProperties(user)) return false;
            return true;
        }
        #endregion

        #region Private Methods
        private static bool ValidateFields(User user)
        {
            foreach (var field in user.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public))
            {
                if (Attribute.IsDefined(field, typeof(IntValidatorAttribute)))
                {
                    var fieldAttr = field.GetCustomAttribute<IntValidatorAttribute>();
                    var fieldValue = (int)field.GetValue(user);
                    if (fieldValue < fieldAttr.FirstId || fieldValue > fieldAttr.LastId) return false;
                }
                if (Attribute.IsDefined(field, typeof(StringValidatorAttribute)))
                {
                    var filedAttr = field.GetCustomAttribute<StringValidatorAttribute>();
                    var fieldValue = (string)field.GetValue(user);
                    if (fieldValue?.Length > filedAttr.Length) return false;
                }
            }
            return true;
        }

        private static bool ValidateProperties(User user)
        {
            foreach (var property in user.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public))
            {
                if (Attribute.IsDefined(property, typeof(IntValidatorAttribute)))
                {
                    var propertyAttr = property.GetCustomAttribute<IntValidatorAttribute>();
                    var propertyValue = (int)property.GetValue(user);
                    if (propertyValue < propertyAttr.FirstId || propertyValue > propertyAttr.LastId) return false;
                }
                if (Attribute.IsDefined(property, typeof(StringValidatorAttribute)))
                {
                    var propertyAttr = property.GetCustomAttribute<StringValidatorAttribute>();
                    var propertyValue = (string)property.GetValue(user);
                    if (propertyValue?.Length > propertyAttr.Length) return false;
                }
            }
            return true;
        }
        #endregion
    }
}
