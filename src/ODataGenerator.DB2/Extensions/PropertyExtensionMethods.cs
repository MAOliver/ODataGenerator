using System;
using ODataGenerator.Sources.DB2.Types;

namespace ODataGenerator.Sources.DB2.Extensions
{
    public static partial class PropertyExtensionMethods
    {

        public static string AsCSharpType( this ISeriesType type )
        {
            //return "string";
            switch ( type )
            {
                case ISeriesType.A:
                    return "string";
                case ISeriesType.B:
                    return "byte[]";
                case ISeriesType.L:
                    return "DateTime";
                case ISeriesType.P:
                    return "decimal";
                case ISeriesType.S:
                    return "decimal";
                case ISeriesType.T:
                    return "TimeSpan";
                default:
                    throw new NotImplementedException( "Property Type unexpected" );
            }
        }


        public static string AsCSharpType( this DB2Type type, bool isNullable = false )
        {
            //return "string";
            switch ( type )
            {
                case DB2Type.VARCHAR:
                    return "string";
                case DB2Type.CHAR:
                    return "string";
                case DB2Type.CLOB:
                    return "string";
                case DB2Type.DBCLOB:
                    return "string";
                case DB2Type.GRAPHIC:
                    return "string";
                case DB2Type.LONGVARC:
                    return "string";
                case DB2Type.LONGVARCHAR:
                    return "string";
                case DB2Type.LONGVARG:
                    return "string";
                case DB2Type.LONGVARGRAPHIC:
                    return "string";
                case DB2Type.VARG:
                    return "string";
                case DB2Type.VARGRAPHIC:
                    return "string";
                case DB2Type.DECIMAL:
                    return "decimal" + ( ( isNullable ) ? "?" : "" );
                case DB2Type.NUMERIC:
                    return "decimal" + ( ( isNullable ) ? "?" : "" );
                case DB2Type.DOUBLE:
                    return "decimal" + ( ( isNullable ) ? "?" : "" );
                case DB2Type.FLOAT:
                    return "double" + ( ( isNullable ) ? "?" : "" );
                case DB2Type.BIGINT:
                    return "Int64" + ( ( isNullable ) ? "?" : "" );
                case DB2Type.INTEGER:
                    return "Int32" + ( ( isNullable ) ? "?" : "" );
                case DB2Type.SMALLINT:
                    return "Int16" + ( ( isNullable ) ? "?" : "" );
                case DB2Type.BINARY:
                    return "byte[]";
                case DB2Type.BLOB:
                    return "byte[]";
                case DB2Type.VARBIN:
                    return "byte[]";
                case DB2Type.VARBINARY:
                    return "byte[]";
                case DB2Type.TIME:
                    return "TimeSpan" + ( ( isNullable ) ? "?" : "" );
                case DB2Type.TIMESTAMP:
                    return "DateTime" + ( ( isNullable ) ? "?" : "" );
                case DB2Type.DATE:
                    return "DateTime" + ( ( isNullable ) ? "?" : "" );
                case DB2Type.REAL:
                    return "single";
                default:
                    throw new NotImplementedException( "Property Type unexpected" );
            }
        }

        public static DB2Type? AsDb2Type( this string type )
        {
            DB2Type retType;
            return Enum.TryParse( type, true, out retType ) ? retType : ( DB2Type? ) null;
        }

        public static ISeriesType? AsPropertyType( this string type )
        {
            ISeriesType retType;
            return Enum.TryParse( type, true, out retType ) ? retType : ( ISeriesType? ) null;
        }
    }
}
