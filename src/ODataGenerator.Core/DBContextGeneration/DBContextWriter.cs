using System;
using System.Collections.Generic;
using System.Text;
using ODataGenerator.Core.ModelGeneration;

namespace ODataGenerator.Core.DBContextGeneration
{
    public class DbContextWriter
    {
        public string NameSpace { get; set; }
        public List<string> Usings { get; set; }

        public string ClassName { get; set; }
        public List<PocoMapping> Models { get; set; }

        public override string ToString()
        {
            return new StringBuilder()
                .AppendLine(string.Join(Environment.NewLine, Usings))
                .Append(Indents.Namespace).Append("namespace ").Append(NameSpace).AppendLine()
                .Append(Indents.NamespaceBracket).Append("{").AppendLine()
                .AppendDbContextClassHeader(ClassName)
                .AppendEntitySets(Models.ToArray())
                .AppendOnModelCreatingMethod(Models.ToArray())
                .AppendDbContextClassFooter()
                .Append(Indents.NamespaceBracket).Append("}").AppendLine()
                .ToString();
        }
    }

    public static class ResourceFormatExtensions
    {
        public static StringBuilder AppendEntitySets(this StringBuilder sb, params PocoMapping[] models)
        {
            if (sb == null || models == null) return sb;

            foreach (var classField in models)
            {
                sb.Append(Indents.ClassMethod)
                    .Append("public virtual DbSet<")
                    .Append(classField.ClassName)
                    .Append("> ")
                    .Append(classField.ClassName.Pluralize())
                    .Append(" { get; set; }").AppendLine();
            }

            return sb.AppendLine();
        }

        public static StringBuilder AppendDbContextClassHeader(this StringBuilder sb, string className)
        {
            return sb
                .Append(Indents.Class)
                .Append("public partial class ")
                .Append(className)
                .Append(" : DbContext")
                .AppendLine()
                .Append(Indents.ClassBracket).Append("{").AppendLine()
                .Append(Indents.ClassMethod).Append("public ").Append(className).Append("( )").AppendLine()
                .Append(Indents.ClassMethodBody).Append(" : base( \"name=DefaultConnection\" ) ").AppendLine()
                .Append(Indents.ClassMethodBracket).AppendLine("{ }").AppendLine();
        }

        public static StringBuilder AppendDbContextClassFooter(this StringBuilder sb)
        {
            return sb
                .Append(Indents.ClassBracket).Append("}").AppendLine()
                .Append(Indents.NamespaceBracket).Append("}");
        }

        public static StringBuilder AppendOnModelCreatingMethod(this StringBuilder sb, params PocoMapping[] models)
        {
            if (sb == null || models == null) return sb;

            sb
                .Append(Indents.ClassMethod).Append("protected override void OnModelCreating( DbModelBuilder modelBuilder )").AppendLine()
                .Append(Indents.ClassMethodBracket).Append("{").AppendLine()
                ;

            foreach (var classField in models)
            {
                sb.Append(Indents.ClassMethodBody)
                    .Append("modelBuilder.Entity<")
                    .Append(classField.ClassName)
                    .Append(">( ); ")
                    .AppendLine();
            }

            return sb.Append(Indents.ClassMethodBracket).Append("}").AppendLine();
        }

        public static string Pluralize(this string className)
        {
            return className+"s";
        }
    }

    public static class Indents
    {
        public const string Namespace = "";
        public const string NamespaceBracket = Namespace;
        public const string Class = TabLevel;
        public const string ClassBracket = Class;
        public const string ClassMethod = Class + TabLevel;
        public const string ClassMethodBracket = ClassMethod;
        public const string ClassMethodBody = ClassMethod + TabLevel;
        public const string ClassMethodBodyIndent = ClassMethodBody + TabLevel;


        private const string TabLevel = "\t";
    }
}
