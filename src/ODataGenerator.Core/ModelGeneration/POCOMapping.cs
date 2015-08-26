using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using ODataGenerator.Core.DBContextGeneration;
using ODataGenerator.Core.Extensions;

namespace ODataGenerator.Core.ModelGeneration
{
    public class PocoMapping
    {
        private readonly List<PropertyField> _propertyFields;

        public PocoMapping(string usingBlock, string ns, string schema, string className,  List<PropertyField> propertyFields)
        {
            Using = usingBlock;
            Ns = ns;
            Schema = schema;
            ClassName = className;
            _propertyFields = propertyFields;
        }

        public string Schema { get; }

        public string Ns { get; }

        public string Using { get; }

        public string ClassName { get; }

        public bool HasKeys => PropertyFields.Any(pf => pf.IsKey);

        public string Keys => string.Join(",", PropertyFields.Where(pf => pf.IsKey).OrderBy(ent=>ent.ColumnName).Select(pf => $"{pf.DataType} {pf.ColumnName.AsValidPropertyName().ToLowerInvariant()}").ToList());

        public string KeysOData => string.Join(",", PropertyFields.Where(pf => pf.IsKey).OrderBy(ent => ent.ColumnName).Select(pf => $"[FromODataUri] {pf.DataType} {pf.ColumnName.AsValidPropertyName().ToLowerInvariant()}").ToList());

        public string KeysPassThru => string.Join(",", PropertyFields.Where(pf => pf.IsKey).OrderBy(ent => ent.ColumnName).Select(pf => $"{pf.ColumnName.AsValidPropertyName().ToLowerInvariant()}").ToList());

        public string KeysPassThruEntity => string.Join(",", PropertyFields.Where(pf => pf.IsKey).OrderBy(ent => ent.ColumnName).Select(pf => $"{ClassName.ToLowerInvariant()}.{pf.ColumnName.AsValidPropertyName()}").ToList());


        public string FindWhereClause => string.Join(" AND ", PropertyFields.Where(pf => pf.IsKey).OrderBy(ent => ent.ColumnName).Select(pf => $"{pf.ColumnName} = @{pf.ColumnName}").ToList());

        public string Parameters => string.Join("", PropertyFields.Where(pf => pf.IsKey).OrderBy(ent => ent.ColumnName).Select(pf => $".Parameter(\"{pf.ColumnName}\", {pf.ColumnName.AsValidPropertyName().ToLowerInvariant()})").ToList());

        public string WhereParameters => string.Join("", PropertyFields.Where(pf => pf.IsKey).OrderBy(ent => ent.ColumnName).Select(pf => $".Where(ent=>ent.{pf.ColumnName.AsValidPropertyName()})").ToList());
        
        public IList<PropertyField> PropertyFields => new ReadOnlyCollection<PropertyField>(_propertyFields);

        private string ClassBody()
        {
            var body = new StringBuilder();

            var listOfNames = new HashSet<string>();

            foreach (var propertyField in _propertyFields)
            {
                while (!listOfNames.Add(propertyField.CalculatedPropertyName))
                {
                    propertyField.CollisionCount++;
                }

                body.Append($"{Environment.NewLine}\t\t").Append(propertyField);
            }
            return body.AppendLine().ToString();
        }

        private string NamespaceHeader => !string.IsNullOrWhiteSpace(Ns) ? $"namespace {Ns}{Environment.NewLine}{{{Environment.NewLine}" : "";

        private string NamespaceFooter => !string.IsNullOrWhiteSpace(Ns) ? "}" : "";

        private string ClassHeader => $"{Indents.Class}public class {ClassName}{Environment.NewLine}{Indents.ClassBracket}{{";

        private string ClassFooter => $"{Environment.NewLine}{Indents.ClassBracket}}}{Environment.NewLine}";

        private string UsingBlock => string.IsNullOrWhiteSpace(Using) ? "" : $"{Using}{Environment.NewLine}";

        public override string ToString()
        {
            return $"{UsingBlock}{NamespaceHeader}{ClassHeader}{ClassBody()}{ClassFooter}{NamespaceFooter}";
        }
    }
}