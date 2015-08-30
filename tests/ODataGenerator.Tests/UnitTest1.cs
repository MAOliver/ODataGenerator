using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ODataGenerator.Core;
using ODataGenerator.Core.DBContextGeneration;

namespace ODataGenerator.Tests
{

    [TestClass]
    public class TestCodeGeneration
    {

        [TestMethod]
        public void TestGenerateEntities()
        {
            var rootPath = Directory.GetParent(Assembly.GetExecutingAssembly()?.Location)?.Parent?.Parent?.Parent?.Parent?.FullName ?? ".";
            var relativePath = @"docs\KeysForDB2.xlsx";
            var fullPath = Path.Combine(rootPath, relativePath);
            var repos = new GenerateRepos(new ODataWriter(null, null, null, null, null, null, null, null));//"DB2.Sql.DataAccess.Repositories", "DB2.Sql.DataAccess.Entities", "DB2.OData.Web.Controllers", fullPath);
            repos.WriteToFile(repos.Classes, repos.Repositories, repos.Controllers);

            foreach (var pocoMapping in repos.Classes)
            {
                Debug.WriteLine($"{Indents.ClassMethodBody}builder.EntitySet<{pocoMapping.ClassName}>(\"{pocoMapping.ClassName}s\");");

            }
        }
    }



   


}