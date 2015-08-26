using System;
using ODataGenerator.Core.DBContextGeneration;

namespace ODataGenerator.Core.ModelGeneration
{
    public class RepositoryMapping
    {
        public RepositoryMapping(string usingBlock, string ns, string className, PocoMapping poco)
        {
            Using = usingBlock;
            Ns = ns;
            ClassName = className;
            Poco = poco;
        }

        public string Ns { get; }

        public string Using { get; }

        public string ClassName { get; }

        public PocoMapping Poco { get; }
        
        private string NamespaceHeader => !string.IsNullOrWhiteSpace(Ns) ? $"namespace {Ns}{Environment.NewLine}{{{Environment.NewLine}" : "";
        private string NamespaceFooter => !string.IsNullOrWhiteSpace(Ns) ? "}" : "";
        private string ClassHeader => $"{Indents.Class}public class {ClassName}{Environment.NewLine}{Indents.ClassBracket}{{{Environment.NewLine}";

        private string ContextMethod => $"{Indents.ClassMethod}private static IDbContext Context => new DbContext().ConnectionStringName(\"DB2\", new DB2Provider());{Environment.NewLine}{Environment.NewLine}";

        private string SelectMethod
        {
            get
            {
                string methodSignature = $"{Indents.ClassMethod}public static {Poco.ClassName} Select({Poco.Keys}){Environment.NewLine}{Indents.ClassMethodBracket}{{{Environment.NewLine}";
                string topOfUsingContext = $"{Indents.ClassMethodBody}using (var context = Context){Environment.NewLine}{Indents.ClassMethodBody}{{{Environment.NewLine}";
                string indentUnderUsingContext = $"{Indents.ClassMethodBodyIndent}return context.Sql(\" SELECT * FROM {Poco.Schema}.{Poco.ClassName} WHERE {Poco.FindWhereClause}\"){Poco.Parameters}.QuerySingle<{Poco.ClassName}>();{Environment.NewLine}";
                string bottomOfUsingContext = $"{Indents.ClassMethodBody}}}{Environment.NewLine}";
                string endOfMethod = $"{Indents.ClassMethodBracket}}}{Environment.NewLine}";

                return $"{methodSignature}{topOfUsingContext}{indentUnderUsingContext}{bottomOfUsingContext}{endOfMethod}{Environment.NewLine}";   
            }
        }

        private string SelectAllMethod => $"{Indents.ClassMethod}public static List<{Poco.ClassName}> SelectAll() => SelectAll(string.Empty);{Environment.NewLine}{Environment.NewLine}";
        private string SelectAll2Method => $"{Indents.ClassMethod}public static List<{Poco.ClassName}> SelectAll(string sortExpression) => SelectAll(0, 0, sortExpression);{Environment.NewLine}{Environment.NewLine}";

        private string SelectAll3Method
        {
            get
            {
                string methodSignature = $"{Indents.ClassMethod}public static List<{Poco.ClassName}> SelectAll(int startRowIndex, int maximumRows, string sortExpression){Environment.NewLine}{Indents.ClassMethodBracket}{{{Environment.NewLine}";
                string topOfUsingContext = $"{Indents.ClassMethodBody}using (var context = Context){Environment.NewLine}{Indents.ClassMethodBody}{{{Environment.NewLine}";
                string firstResult = $"{Indents.ClassMethodBodyIndent}var select = context.Select<{Poco.ClassName}>(\" * \").From(\" {Poco.Schema}.{Poco.ClassName} \");{Environment.NewLine}{Environment.NewLine}";
                string maxRowsClause = $"{Indents.ClassMethodBodyIndent}if (maximumRows > 0) select.Paging(startRowIndex == 0 ? 1 : startRowIndex, maximumRows);{Environment.NewLine}";
                string sortExpressionClause = $"{Indents.ClassMethodBodyIndent}if (!string.IsNullOrEmpty(sortExpression)) select.OrderBy(sortExpression);{Environment.NewLine}";
                string returnStatement = $"{Indents.ClassMethodBodyIndent}return select.QueryMany();{Environment.NewLine}";
                string bottomOfUsingContext = $"{Indents.ClassMethodBody}}}{Environment.NewLine}";
                string endOfMethod = $"{Indents.ClassMethodBracket}}}{Environment.NewLine}";
                return $"{methodSignature}{topOfUsingContext}{firstResult}{maxRowsClause}{sortExpressionClause}{returnStatement}{bottomOfUsingContext}{endOfMethod}{Environment.NewLine}";
            }
        }

        private string CountAllMethod
        {
            get
            {
                string methodSignature = $"{Indents.ClassMethod}public static int CountAll(){Environment.NewLine}{Indents.ClassMethodBracket}{{{Environment.NewLine}";
                string topOfUsingContext = $"{Indents.ClassMethodBody}using (var context = Context){Environment.NewLine}{Indents.ClassMethodBody}{{{Environment.NewLine}";
                string indentUnderUsingContext = $"{Indents.ClassMethodBodyIndent}return context.Sql(\" SELECT count(*) FROM {Poco.Schema}.{Poco.ClassName}\").QuerySingle<int>();{Environment.NewLine}";
                string bottomOfUsingContext = $"{Indents.ClassMethodBody}}}{Environment.NewLine}";
                string endOfMethod = $"{Indents.ClassMethodBracket}}}{Environment.NewLine}";
                return $"{methodSignature}{topOfUsingContext}{indentUnderUsingContext}{bottomOfUsingContext}{endOfMethod}{Environment.NewLine}";
            }
        }

        private string IsSingleResultMethod
        {
            get
            {
                string methodSignature = $"{Indents.ClassMethod}public static bool IsSingleResult({Poco.Keys}){Environment.NewLine}{Indents.ClassMethodBracket}{{{Environment.NewLine}";
                string topOfUsingContext = $"{Indents.ClassMethodBody}using (var context = Context){Environment.NewLine}{Indents.ClassMethodBody}{{{Environment.NewLine}";
                string indentUnderUsingContext = $"{Indents.ClassMethodBodyIndent}return context.Sql(\" SELECT count(*) FROM {Poco.Schema}.{Poco.ClassName} WHERE {Poco.FindWhereClause}\"){Poco.Parameters}.QuerySingle<int>() == 1;{Environment.NewLine}";
                string bottomOfUsingContext = $"{Indents.ClassMethodBody}}}{Environment.NewLine}";
                string endOfMethod = $"{Indents.ClassMethodBracket}}}{Environment.NewLine}";
                return $"{methodSignature}{topOfUsingContext}{indentUnderUsingContext}{bottomOfUsingContext}{endOfMethod}{Environment.NewLine}";
            }
        }

        private string InsertMethod
        {
            get
            {
                string methodSignature = $"{Indents.ClassMethod}public static bool Insert({Poco.ClassName} {Poco.ClassName.ToLowerInvariant()}){Environment.NewLine}{Indents.ClassMethodBracket}{{{Environment.NewLine}";
                string topOfUsingContext = $"{Indents.ClassMethodBody}using (var context = Context){Environment.NewLine}{Indents.ClassMethodBody}{{{Environment.NewLine}";
                string indentUnderUsingContext = $"{Indents.ClassMethodBodyIndent}return context.Insert<{Poco.ClassName}>(\"{Poco.Schema}.{Poco.ClassName}\", {Poco.ClassName.ToLowerInvariant()}).AutoMap().Execute() > 0;{Environment.NewLine}";
                string bottomOfUsingContext = $"{Indents.ClassMethodBody}}}{Environment.NewLine}";
                string endOfMethod = $"{Indents.ClassMethodBracket}}}{Environment.NewLine}";
                return $"{methodSignature}{topOfUsingContext}{indentUnderUsingContext}{bottomOfUsingContext}{endOfMethod}{Environment.NewLine}";
            }
        }

        private string UpdateMethod
        {
            get
            {
                string methodSignature = $"{Indents.ClassMethod}public static bool Update({Poco.ClassName} {Poco.ClassName.ToLowerInvariant()}){Environment.NewLine}{Indents.ClassMethodBracket}{{{Environment.NewLine}";
                string topOfUsingContext = $"{Indents.ClassMethodBody}using (var context = Context){Environment.NewLine}{Indents.ClassMethodBody}{{{Environment.NewLine}";
                string indentUnderUsingContext = $"{Indents.ClassMethodBodyIndent}return context.Update<{Poco.ClassName}>(\"{Poco.Schema}.{Poco.ClassName}\", {Poco.ClassName.ToLowerInvariant()}){Poco.WhereParameters}.AutoMap().Execute() > 0;{Environment.NewLine}";
                string bottomOfUsingContext = $"{Indents.ClassMethodBody}}}{Environment.NewLine}";
                string endOfMethod = $"{Indents.ClassMethodBracket}}}{Environment.NewLine}";
                return $"{methodSignature}{topOfUsingContext}{indentUnderUsingContext}{bottomOfUsingContext}{endOfMethod}{Environment.NewLine}";
            }
        }

        private string DeleteMethod
        {
            get
            {
                string methodSignature = $"{Indents.ClassMethod}public static bool Delete({Poco.ClassName} {Poco.ClassName.ToLowerInvariant()}){Environment.NewLine}{Indents.ClassMethodBracket}{{{Environment.NewLine}";
                string topOfUsingContext = $"{Indents.ClassMethodBody}using (var context = Context){Environment.NewLine}{Indents.ClassMethodBody}{{{Environment.NewLine}";
                string indentUnderUsingContext = $"{Indents.ClassMethodBodyIndent}return context.Delete<{Poco.ClassName}>(\"{Poco.Schema}.{Poco.ClassName}\", {Poco.ClassName.ToLowerInvariant()}){Poco.WhereParameters}.Execute() > 0;{Environment.NewLine}";
                string bottomOfUsingContext = $"{Indents.ClassMethodBody}}}{Environment.NewLine}";
                string endOfMethod = $"{Indents.ClassMethodBracket}}}{Environment.NewLine}";
                return $"{methodSignature}{topOfUsingContext}{indentUnderUsingContext}{bottomOfUsingContext}{endOfMethod}{Environment.NewLine}";
            }
        }

        private string ClassFooter => $"{Environment.NewLine}{Indents.ClassBracket}}}{Environment.NewLine}";
        private string UsingBlock => string.IsNullOrWhiteSpace(Using) ? "" : $"{Using}{Environment.NewLine}";
        public override string ToString()
        {
            return $"{UsingBlock}{NamespaceHeader}{ClassHeader}{ContextMethod}{SelectMethod}{SelectAllMethod}{SelectAll2Method}{SelectAll3Method}{CountAllMethod}{IsSingleResultMethod}{InsertMethod}{UpdateMethod}{DeleteMethod}{ClassFooter}{NamespaceFooter}";
        }
    }

}