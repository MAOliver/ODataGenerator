using System.Collections.Generic;
using System.Linq;
using FluentData;
using ODataGenerator.Core;

namespace ODataGenerator.Sources.DB2
{
    public class MetaProcessor
    {
        private readonly IDbContextProvider _provider;

        public MetaProcessor(IDbContextProvider provider)
        {
            _provider = provider;
        }

    }

    public class Db2ContextProvider : IDbContextProvider
    {
        public IDbContext Context => new DbContext( ).ConnectionStringName( "DB2", new DB2Provider( ) );

        public List<TableMeta> TableMeta
        {
            get
            {
                List<dynamic> results = new List<dynamic>();
                using ( var ctx = Context )
                {
                    //var path = Path.GetFullPath( keyFilePath );
                    results = ctx.Sql( "select COLUMN_NAME, TABLE_NAME, DATA_TYPE, LENGTH, NUMERIC_SCALE, NUMERIC_PRECISION, TABLE_SCHEMA, COLUMN_DEFAULT, COLUMN_TEXT, COLUMN_HEADING, IS_NULLABLE, HAS_DEFAULT from qsys2.syscolumns WHERE TABLE_SCHEMA = 'LSFILES' " ).QueryMany<dynamic>( );
                    //keyTableList = new ExcelToDynamic( ).LoadDynamicListFromExcel( path );//ctx.Sql("select APFILE, APLIB, APKEYF, APBOF, APBOL, APBOLF from PPOARCH.@@ACCPTH WHERE APLIB = 'LSFILES'").QueryMany<dynamic>();
                }

                return results.Select(tm => new TableMeta ()).ToList();
            }
            
        }
        public List<KeyMeta> KeyMeta { get; }
    }
}
