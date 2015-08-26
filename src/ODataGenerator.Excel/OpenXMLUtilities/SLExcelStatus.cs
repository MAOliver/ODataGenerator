namespace ODataGenerator.Sources.Excel.OpenXMLUtilities
{
    /// <summary>
    /// http://www.codeproject.com/Articles/670141/Read-and-Write-Microsoft-Excel-with-Open-XML-SDK
    /// </summary>
    public class SLExcelStatus
    {
        public string Message { get; set; }
        public bool Success
        {
            get { return string.IsNullOrWhiteSpace( Message ); }
        }
    }
}