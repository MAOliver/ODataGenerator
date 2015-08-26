using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using ODataGenerator.Sources.Excel.OpenXMLUtilities;

namespace ODataGenerator.Sources.Excel
{
    public class ExcelToDynamic
    {
        public List<dynamic> LoadDynamicListFromExcel( string excelFilePath )
        {
            Debug.WriteLine( $"Current Path {Environment.CurrentDirectory}, expected path of excelfile with keys {Path.Combine( Environment.CurrentDirectory, excelFilePath )}" );
            var file = File.Open( excelFilePath, FileMode.Open );
            var data = new SLExcelReader( ).ReadExcel( file );
            return data.DataRows.Select( dataRow => AsDynamic( dataRow, data.Headers ) ).ToList( );
        }

        public dynamic AsDynamic( List<string> datarow, List<string> header )
        {
            IDictionary<string, object> result = new ExpandoObject( );
            for ( var i = 0; i < datarow.Count; i++ )
            {
                var fieldName = header[ i ];
                result[ fieldName ] = datarow[ i ];
            }
            return result;
        }
    }
}
