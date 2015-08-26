using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using ODataGenerator.Core.DBContextGeneration;
using ODataGenerator.Core.Extensions;

namespace ODataGenerator.Core.ModelGeneration
{
    public class PropertyField
    {
        public PropertyField
        (
            string tableSchema
            , string tableName
            , string columnName
            , string dataType
            , int? length
            , int? numericScale
            , int? numericPrecision
            , object columnDefault
            , string columnText
            , string columnHeading
            , bool? isNullable
            , string hasDefault
            , bool isKey
        )
        {
            TableSchema = tableSchema;
            TableName = tableName;
            ColumnName = columnName;
            IsNullable = isNullable.GetValueOrDefault(true);//isNullable == "Y"; //none of them are nullable
            DataType = dataType.AsDb2Type().GetValueOrDefault(DB2Type.VARCHAR).AsCSharpType(IsNullable);
            Length = length;
            NumericScale = numericScale;
            NumericPrecision = numericPrecision;
            ColumnDefault = columnDefault;
            ColumnText = columnText;
            ColumnHeading = columnHeading;
            IsKey = isKey;
            HasDefault = hasDefault == "Y";//.AsBoolean(); //all of our db2fields have defaults that I've seen

            ValidationAttributes = new ReadOnlyCollection<string>(new List<string>()
            {
                IsKey.AsKeyAttribute()
                ,IsNullable.AsRequiredAttribute()
                , DataType.AsStringLengthAttribute(Length)
                , ColumnName.AsColumnAttribute()
            }.Where(s => !string.IsNullOrWhiteSpace(s)).ToList());
        }

        public bool IsKey { get; }

        public string TableSchema { get; }

        public string TableName { get; }

        public string ColumnName { get; }

        public string DataType { get; }

        public int? Length { get; }

        public int? NumericScale { get; }

        public int? NumericPrecision { get; }

        public object ColumnDefault { get; }

        public string ColumnText { get; }

        public string ColumnHeading { get; }

        public bool IsNullable { get; }

        public bool HasDefault { get; }

        public string CalculatedPropertyName => (ColumnName + (CollisionCount > 0 ? CollisionCount.ToString() : "")).AsValidPropertyName();

        public int CollisionCount { get; set; }

        public IReadOnlyList<string> ValidationAttributes { get; }

        public override string ToString()
        {
            return new StringBuilder()
                .AppendLine(string.Join(Environment.NewLine+Indents.ClassMethod, ValidationAttributes))
                .Append(Indents.ClassMethod).AppendLine($"public {DataType} {CalculatedPropertyName} {{ get; set; }}").ToString();

        }
    }
}