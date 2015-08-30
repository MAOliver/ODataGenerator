using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using ODataGenerator.Core.Extensions;
using ODataGenerator.Core.ModelGeneration;

namespace ODataGenerator.Core
{
    public class TableMeta
    {
        public string TableSchema { get; set; }
        public string TableName { get; set; }

        public List<ColumnMeta> Columns { get; set; }
        public List<TableMeta> ForeignKeys { get; set; }  
     
    }

    public class ColumnMeta
    {
        public string DataType { get; set; }
        public string Name { get; set; }
        public int? Length { get; set; }
        public int? NumericScale { get; set; }
        public int? NumericPrecision { get; set; }
        public object Default { get; set; }
        public string Description { get; set; }
        public bool IsNullable { get; set; }
        public bool IsKey { get; set; }

        //TODO foreignkeys

    }

    public class KeyMeta
    {
        public string KeyTableSchema { get; set; }

        public string KeyTableName { get; set; }

        public string KeyTableColumn { get; set; }

        public ForeignKeyMeta ForeignKeyMeta { get; set; }

    }

    public class ForeignKeyMeta
    {
        public string ForeignKeySchema { get; set; }

        public string ForeignKeyTable { get; set; }

        public string ForeignKeyColumn { get; set; }
    }

    public interface IDbContextProvider
    {
        List<TableMeta> TableMeta { get; }
        List<KeyMeta> KeyMeta { get; } 
    }

    public interface INamespaceBuilder
    {
        INamespaceBuilder SetContextNamespace(string @namespace);

        INamespaceBuilder SetEntityNamespace(string @namespace);

        INamespaceBuilder SetControllerNamespace(string @namespace);

        IUsingBuilder Usings { get; }
    }

    public interface IUsingBuilder
    {
        IUsingBuilder AddEntityUsing(string @using);

        IUsingBuilder AddContextUsing( string @using );

        IUsingBuilder AddControllerUsing( string @using );

        INamespaceBuilder Namespaces { get; }
    }

    public interface ISourceBuilder
    {
        INamespaceBuilder Namespaces { get; }
        IUsingBuilder Usings { get; }

        ODataWriter Build();

    }

    public class RepoBuilder : INamespaceBuilder, IUsingBuilder
    {
        private readonly IDbContextProvider _contextProvider;
        private readonly List<string> _entityUsings;
        private readonly List<string> _contextUsings;
        private readonly List<string> _controllerUsings;
        private string _contextNamespace;
        private string _entityNamespce;
        private string _controllerNamespace;

        private RepoBuilder( IDbContextProvider contextProvider, string[] entityUsings, string[] contextUsings, string[] controllerUsings)
        {
            _contextProvider = contextProvider;
            _entityUsings = new List<string>(entityUsings);
            _contextUsings = new List<string>(contextUsings);
            _controllerUsings = new List<string>(controllerUsings);
        }

        public static RepoBuilder New(IDbContextProvider contextProvider)
        {
            return 
                new RepoBuilder
                (
                    contextProvider
                    , new [ ] { "using System;", "using System.ComponentModel.DataAnnotations;", "using System.ComponentModel.DataAnnotations.Schema;" }
                    , new [ ] { "using System;", "using System.Collections.Generic;", "using FluentData;" }
                    , new [ ] { "using System;", "using System.Data.Entity.Infrastructure;", "using System.Linq;", "using System.Net;", "using System.Web.Http;", "using System.Web.OData;" }
               );
        }

        public IUsingBuilder AddEntityUsing( string @namespace )
        {
            _entityUsings.Add( @namespace.AsUsing() );
            return this;
        }

        public IUsingBuilder AddContextUsing( string @namespace )
        {
            _contextUsings.Add( @namespace.AsUsing( ) );
            return this;
        }

        public IUsingBuilder AddControllerUsing( string @namespace )
        {
            _controllerUsings.Add( @namespace.AsUsing( ) );
            return this;
        }

        public ODataWriter Build()
        {
            return new ODataWriter(_contextProvider.TableMeta, _contextProvider.KeyMeta, _contextNamespace, _entityNamespce, _controllerNamespace, _entityUsings, _contextUsings, _controllerUsings);
        }

        public INamespaceBuilder SetContextNamespace(string @namespace)
        {
            _contextNamespace = @namespace;
            return this;
        }

        public INamespaceBuilder SetEntityNamespace(string @namespace)
        {
            _entityNamespce = @namespace;
            return this;
        }

        public INamespaceBuilder SetControllerNamespace(string @namespace)
        {
            _controllerNamespace = @namespace;
            return this;
        }

        public IUsingBuilder Usings => this;
        public INamespaceBuilder Namespaces => this;
    }

    public class ODataWriter
    {
        public List<TableMeta> TableMetas { get; }

        public List<KeyMeta> KeyMetas { get; }

        public string ContextNamespace { get; }

        public string EntityNamespace { get; }

        public string ControllerNamespace { get; }

        public string EntityUsings { get; }

        public string ContextUsings { get; }

        public string ControllerUsings { get; }

        public ODataWriter(List<TableMeta> tableMetas, List<KeyMeta> keyMetas, string contextNamespace, string entityNamespace, string controllerNamespace, List<string> entityUsings, List<string> contextUsings, List<string> controllerUsings)
        {
            TableMetas = tableMetas;
            KeyMetas = keyMetas;
            ContextNamespace = ContextNamespace;
            EntityNamespace = entityNamespace;
            ControllerNamespace = controllerNamespace;
            EntityUsings = string.Join( Environment.NewLine, entityUsings );
            ContextUsings = string.Join( Environment.NewLine, contextUsings );
            ControllerUsings = string.Join( Environment.NewLine, controllerUsings );
        }
    }

    public class GenerateRepos
    {

        public GenerateRepos()
        {
        }

   
        public Dictionary<string, List<TableMeta>> TableDictionary { get; }

        public List<PocoMapping> Classes { get; }
        public List<RepositoryMapping> Repositories { get; }

        public List<ControllerMapping> Controllers { get; }

        public GenerateRepos(ODataWriter writer)
        {
            TableDictionary = GetTableDictionary(writer.TableMetas, writer.KeyMetas);
            Classes = CreatePocoMappings(writer.EntityNamespace, writer.EntityUsings);
            Repositories = CreateRepositoryMappings(writer.ContextNamespace, writer.ContextUsings);
            Controllers = CreateControllerMappings(writer.ControllerNamespace, writer.ControllerUsings);
        }
#region initialization
        private static Dictionary<string, List<TableMeta>> GetTableDictionary( List<TableMeta> tableColumnList, List<KeyMeta> keyTableList )
        {

            var keyDictionary = keyTableList.ToDictionary((o) =>$"{o.KeyTableSchema}.{o.KeyTableName}");
            var tableDictionary = new Dictionary<string, List<TableMeta>>();

            foreach (var o in tableColumnList)
            {
                var key = o.TableName + "." + o.TableSchema;
                if (!tableDictionary.ContainsKey(key))
                {
                    tableDictionary[key] = new List<TableMeta>();
                }
                var keysList = new List<KeyMeta>();
                if (keyDictionary.ContainsKey(key))
                {
                    keysList = keyDictionary[key];
                }
                /*var keyInformation = keysList.FirstOrDefault(row => row.KeyTableColumn == o.co);
                o.IS_PRIMARY_KEY = false;
                o.HAS_FOREIGN_KEY = false;

                if (keyInformation != null)
                {
                    o.IS_PRIMARY_KEY = keyInformation.COLUMN_NAME == o.COLUMN_NAME;
                    o.FOREIGN_KEY = ((IDictionary<string, dynamic>)keyInformation).ContainsKey("FOREIGN_KEY") ? keyInformation.FOREIGN_KEY : null;
                }*/

                tableDictionary[key].Add(o);
            }
            return tableDictionary;
        }

        public static dynamic MapRow(string schemaName, string tableName, string columnName, string fkSchema, string fkTable, string fkColumn)
        {
            dynamic result = new ExpandoObject();
            result.TABLE_SCHEMA = schemaName;
            result.TABLE_NAME = tableName;
            result.COLUMN_NAME = columnName;

            if (!string.IsNullOrWhiteSpace(fkSchema))
            {
                dynamic foreignKey = new ExpandoObject();

                foreignKey.FOREIGN_KEY_SCHEMA_NAME = fkSchema;
                foreignKey.FOREIGN_KEY_TABLE_NAME =fkTable;
                foreignKey.FOREIGN_KEY_COLUMN_NAME = fkColumn;

                result.FOREIGN_KEY = foreignKey;
            }

            return result;
        }
#endregion initialization   
        
        public List<PocoMapping> CreatePocoMappings(string classNamespace, string classUsingBlock)
        {
            return TableDictionary.Select(kvp => CreatePoco(classUsingBlock, classNamespace, kvp)).Where(poco=>poco.HasKeys).ToList();
        }

        public List<RepositoryMapping> CreateRepositoryMappings(string classNamespace, string classUsingBlock)
        {
            return Classes.Select(poco => CreateRepository(classUsingBlock, classNamespace, poco)).ToList();
        }

        public List<ControllerMapping> CreateControllerMappings(string classNamespace, string classUsingBlock)
        {
            return Repositories.Select(repo => CreateController(classUsingBlock, classNamespace, repo)).ToList();
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public PocoMapping CreatePoco(string usingBlock, string nameSpace, KeyValuePair<string, List<TableMeta>> obj)
        {
            var schema = obj.Key.Split(new[] {"."}, StringSplitOptions.None)[0];
            var className = obj.Key.Split(new[] {"."}, StringSplitOptions.None)[1];
            return new PocoMapping(usingBlock, nameSpace, schema, className,obj.Value.Select(o => new PropertyField(/*o.TABLE_SCHEMA, o.TABLE_NAME, o.COLUMN_NAME, o.DATA_TYPE, o.LENGTH, o.NUMERIC_SCALE, o.NUMERIC_PRECISION, o.COLUMN_DEFAULT, o.COLUMN_TEXT, o.COLUMN_HEADING, o.IS_NULLABLE, o.HAS_DEFAULT, o.IS_PRIMARY_KEY*/)).ToList());
        }

        public RepositoryMapping CreateRepository(string usingBlock, string nameSpace, PocoMapping poco)
        {
            return new RepositoryMapping(string.Join(Environment.NewLine, usingBlock, $"using {poco.Ns};") , nameSpace, $"{poco.ClassName}Gateway" ,poco);
        }

        public ControllerMapping CreateController(string usingBlock, string nameSpace, RepositoryMapping repo)
        {
            return new ControllerMapping(usingBlock, nameSpace, $"{repo.Poco.ClassName}sController", repo);
        }

        public void WriteToFile(List<PocoMapping> pocos, List<RepositoryMapping> repositories, List<ControllerMapping> controllers )
        {
            Directory.CreateDirectory(@".\Entities");
            Directory.CreateDirectory(@".\Repositories");
            Directory.CreateDirectory(@".\Controllers");

            foreach (var pocoMapping in pocos)
            {
                File.WriteAllText($@".\Entities\{pocoMapping.ClassName}.cs", pocoMapping.ToString());
            }

            foreach (var repositoryMapping in repositories)
            {
                File.WriteAllText($@".\Repositories\{repositoryMapping.ClassName}.cs", repositoryMapping.ToString());
            }

            foreach (var controllerMapping in controllers)
            {
                File.WriteAllText($@".\Controllers\{controllerMapping.ClassName}.cs", controllerMapping.ToString());
            }
        }
    }
}
