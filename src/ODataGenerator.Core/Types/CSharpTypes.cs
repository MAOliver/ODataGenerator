using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODataGenerator.Core.Types
{
    public class CSharpTypes
    {
        public IDictionary<string, Type> Types => new Dictionary<string, Type>()
        {
            { "Int64", typeof(Int64) }
            , { "Byte[]", typeof(Byte[]) }
            , { "Boolean", typeof(Boolean) }
            , { "String", typeof(String) }
            , { "Char[]", typeof(Char[]) }
            , { "DateTime", typeof(DateTime) }
            , { "DateTimeOffset", typeof(DateTimeOffset) }
            , { "Decimal", typeof(Decimal) }
            , { "Double", typeof(Double) }
            , { "Single", typeof(Single) }
            , { "Int16", typeof(Int16) }
            , { "TimeSpan", typeof(TimeSpan) }
            , { "Byte", typeof(Byte) }
            , { "Guid", typeof(Guid) }
            , { "Xml", typeof(String) }
        }; 
    }
}
