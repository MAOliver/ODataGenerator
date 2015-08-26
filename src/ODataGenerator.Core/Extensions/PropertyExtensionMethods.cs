using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ODataGenerator.Core.Extensions
{
    public static partial class PropertyExtensionMethods
    {
        public static string[] ToArrayIfNotEmpty(this string input1, string input2 = null, string input3 = null, string input4 = null) 
            => new[] {input1, input2, input3, input4}.Where(i => !string.IsNullOrWhiteSpace(i)).ToArray();

        public static string Coalesce(this string[] inputs) => inputs?.FirstOrDefault(i => !string.IsNullOrWhiteSpace(i));
        
        public static string Coalesce(this string input)
        {
            return string.IsNullOrWhiteSpace(input) ? null : input;
        }

        public static string AsRequiredAttribute(this bool isRequired)
        {
            return isRequired ? "[Required]" : "";
        }

        public static string AsKeyAttribute(this bool isKey)
        {
            return isKey ? "[Key]" : "";
        }

        public static string AsRangeAttribute(this string dataType, int? scale, int? precision)
        {
            //var repeatingNines = precision.HasValue ?
            if (!scale.HasValue && !precision.HasValue) return "";

            var precisionString = "";
            if (precision.HasValue)
            {
                precisionString = precisionString.PadLeft(precision.Value, '9');
            }
            var precisionAndScale = precisionString;
            if (scale.HasValue && scale.Value > 0)
            {
                precisionAndScale = precisionAndScale.Insert(precisionAndScale.Length - scale.Value, ".");
            }
            switch (dataType)
            {
                case "decimal":
                    return $"[Range(-{precisionAndScale}, {precisionAndScale})]";
                case "double":
                    goto case "Int64";
                case "Int64":
                    goto case "Int32";
                case "Int32":
                    goto case "Int16";
                case "Int16":
                    return $"[Range(-{precisionString}, {precisionString})]";
                default:
                    return "";
            }
        }

        public static string AsColumnAttribute(this string columnName)
        {
            return $"[Column(\"{columnName}\")]";
        }

        public static string AsStringLengthAttribute(this string dataType, int? length )
        {
            return string.Equals(dataType, "string") && length.HasValue
                ? $"[StringLength({length.GetValueOrDefault()})]"
                : "";
        }

        public static bool AsBoolean(this string inputValue)
        {
            bool isNullable = Boolean.TryParse(inputValue, out isNullable) && isNullable;
            return isNullable;
        }

        public static string AsValidPropertyName(this string input)
        {
            //default to blank if null
            var result = string.IsNullOrWhiteSpace(input) ? "" : input;
            //replace any invalid inputs
            result = result
                .Replace(" ", "_")
                .Replace(".", "")
                .Replace("/","_OR_")
                .Replace("&", "_AND_")
                .Replace("?", "_IS_TRUE")
                .Replace("@", "")
                .Replace("-", "")
                .Replace("||", "_OR_")
                .Replace("(", "")
                .Replace(";", "_")
                .Replace(":", "")
                .Replace(")","")
                .Replace("`", "")
                .Replace("'", "")
                .Replace("#", "NUM")
                .Replace("=", "_IS_")
                .Replace(",", "")
                .Replace("+", "PLUS")
                .Replace("%", "PERCENT")
                .Replace("$", "AMOUNT");
            
            //if starts with digit, prepend "_"
            if (Regex.IsMatch(result, @"^\d"))
            {
                result = "_" + result;
            }
            return result;
        }


    }

    public static class UtilityExtensions
    {
        public static string AsUsing(this string @namespace)
        {
            return @namespace.StartsWith( "using ", StringComparison.InvariantCultureIgnoreCase )
                ? @namespace
                : $"using {@namespace};";
        }

        public static Dictionary<string, List<T>> ToDictionary<T>( this List<T> collection, Func<T, string> keyFunc )
        {
            var keyDictionary = new Dictionary<string, List<T>>( );
            foreach ( var o in collection )
            {
                var key = keyFunc(o);
                if ( !keyDictionary.ContainsKey( key ) )
                {
                    keyDictionary[ key ] = new List<T>( );
                }
                keyDictionary[ key ].Add( o );
            }
            return keyDictionary;
        }
    }
}