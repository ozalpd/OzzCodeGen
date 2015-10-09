using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzzCodeGen.CodeEngines.Metadata
{
    public static class DataTypes
    {
        public static string NotSpecial = "Not Special";
        public static string Custom = "Custom";
        public static string DateTime = "DateTime";
        public static string Date = "Date";
        public static string Time = "Time";
        public static string Duration = "Duration";
        public static string PhoneNumber = "PhoneNumber";
        public static string Currency = "Currency";
        public static string Text = "Text";
        public static string Html = "Html";
        public static string MultilineText = "MultilineText";
        public static string EmailAddress = "EmailAddress";
        public static string Password = "Password";
        public static string Url = "Url";
        public static string ImageUrl = "ImageUrl";
        public static string CreditCard = "CreditCard";
        public static string PostalCode = "PostalCode";
        public static string Upload = "Upload";
        public static List<string> DataTypeList = new List<string>()
            {
                DataTypes.NotSpecial,
                DataTypes.Custom,
                DataTypes.DateTime,
                DataTypes.Date,
                DataTypes.Time,
                DataTypes.Duration,
                DataTypes.PhoneNumber,
                DataTypes.Currency,
                DataTypes.Text,
                DataTypes.Html,
                DataTypes.MultilineText,
                DataTypes.EmailAddress,
                DataTypes.Password,
                DataTypes.Url,
                DataTypes.ImageUrl,
                DataTypes.CreditCard,
                DataTypes.PostalCode,
                DataTypes.Upload
            };
    }
}
