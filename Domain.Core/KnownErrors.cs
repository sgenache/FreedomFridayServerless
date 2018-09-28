using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace FreedomFridayServerless.Domain.Core
{
    public enum KnownErrors
	{
        [StringValue("Error: {0}")]
		Unknown = 0,

        [StringValue("Accounts with no Code")]
        AccountWithNoCode,

	    [StringValue("Retained Earnings system account not found")]
        RetainedEarningsAccount,

	    [StringValue("Cannot Post a Journal that is out of balance")]
        TransactionNotBalanced,

	    [StringValue("Cannot Post a Journal without Entries")]
        TransactionEmpty,

	    [StringValue("Cannot add Entry to an already posted Journal")]
        TransactionImmutable,

	    [StringValue("Journal with invalid date")]
        TransactionInvalidDate,

	    [StringValue("Account not available")]
        AccountNotAvailable,
    }

    public class StringValueAttribute : Attribute
    {
        private readonly string _stringValue;

        /// <summary>
        /// Constructs a new String Value Attribute
        /// </summary>
        /// <param name="value">The string value for this enum</param>
        public StringValueAttribute(string value)
        {
            _stringValue = value;
        }

        /// <summary>
        /// Holds the stringvalue for a value in an enum.
        /// </summary>
        public string StringValue
        {
            get { return _stringValue; }
        }
    }

    public static class EnumExtension
    {
        public static string ToStringValue(this Enum value)
        {
            Type type = value.GetType();

            // Get fieldinfo for this type
            FieldInfo fieldInfo = type.GetField(value.ToString());

            // Get the stringvalue attributes
            var attribs = fieldInfo.GetCustomAttributes(
                typeof(StringValueAttribute), false) as StringValueAttribute[];

            return attribs != null && attribs.Length > 0 ? attribs[0].StringValue : value.ToString();
        }

		public static string ToEnumMemberValue<T>(this T type)
			where T : struct
		{
			var enumType = typeof(T);
			var name = Enum.GetName(enumType, type);
			var enumMemberAttribute = ((EnumMemberAttribute[])enumType
				.GetField(name)
				.GetCustomAttributes(typeof(EnumMemberAttribute), true))
				.Single();
			return enumMemberAttribute.Value;
		}

		public static T ToEnum<T>(this string value)
			where T: struct
		{
			if (string.IsNullOrEmpty(value))
			{
				return default(T);
			}

			T result;
			return Enum.TryParse<T>(value, true, out result) ? result : default(T);
		}
	}
}