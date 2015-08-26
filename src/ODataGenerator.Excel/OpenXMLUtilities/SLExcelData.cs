using System.Collections.Generic;

namespace ODataGenerator.Sources.Excel.OpenXMLUtilities
{
    /// <summary>
    /// http://www.codeproject.com/Articles/670141/Read-and-Write-Microsoft-Excel-with-Open-XML-SDK
    /// </summary>
    public class SLExcelData
    {
        public SLExcelStatus Status { get; set; }
        public Columns ColumnConfigurations { get; set; }
        public List<string> Headers { get; set; }
        public List<List<string>> DataRows { get; set; }
        public string SheetName { get; set; }

        public SLExcelData( )
        {
            Status = new SLExcelStatus( );
            Headers = new List<string>( );
            DataRows = new List<List<string>>( );
        }
    }
}
