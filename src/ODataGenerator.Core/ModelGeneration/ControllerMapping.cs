using System;
using ODataGenerator.Core.DBContextGeneration;

namespace ODataGenerator.Core.ModelGeneration
{

    public class ControllerMapping
    {
        public ControllerMapping(string usingBlock, string ns, string className, RepositoryMapping repository)
        {
            Using = usingBlock;
            Ns = ns;
            ClassName = className;
            Repository = repository;
        }

        public string Ns { get; }

        public string Using { get; }

        public string ClassName { get; }

        public RepositoryMapping Repository { get; }

        private string NamespaceHeader => !string.IsNullOrWhiteSpace(Ns) ? $"namespace {Ns}{Environment.NewLine}{{{Environment.NewLine}" : "";
        private string NamespaceFooter => !string.IsNullOrWhiteSpace(Ns) ? "}" : "";
        private string ClassHeader => $"{Indents.Class}public class {ClassName} : ODataController {Environment.NewLine}{Indents.ClassBracket}{{{Environment.NewLine}";

        private string GetAllMethod
        {
            get
            {
                string annotation = $"{Indents.ClassMethod}[EnableQuery]{Environment.NewLine}";
                string methodSignature = $"{Indents.ClassMethod}public IQueryable<{Repository.Poco.ClassName}> Get(){Environment.NewLine}{Indents.ClassMethodBracket}{{{Environment.NewLine}";
                string selectClause = $"{Indents.ClassMethodBody}return {Repository.ClassName}.SelectAll().AsQueryable();{Environment.NewLine}";
                string endOfMethod = $"{Indents.ClassMethodBracket}}}{Environment.NewLine}";

                return $"{annotation}{methodSignature}{selectClause}{endOfMethod}{Environment.NewLine}";
            }
        }

        private string GetMethod
        {
            get
            {
                string annotation = $"{Indents.ClassMethod}[EnableQuery]{Environment.NewLine}";
                string methodSignature = $"{Indents.ClassMethod}public SingleResult<{Repository.Poco.ClassName}> Get{Repository.Poco.ClassName}({Repository.Poco.KeysOData}){Environment.NewLine}{Indents.ClassMethodBracket}{{{Environment.NewLine}";
                string selectClause = $"{Indents.ClassMethodBody}return SingleResult.Create( {Repository.ClassName}.Select({Repository.Poco.KeysPassThru}).AsQuery() );{Environment.NewLine}";
                string endOfMethod = $"{Indents.ClassMethodBracket}}}{Environment.NewLine}";

                return $"{annotation}{methodSignature}{selectClause}{endOfMethod}{Environment.NewLine}";
            }
        }

        private string TryCatchNotExists(string body)
        {
            string tryClause = $"{Indents.ClassMethodBody}try{Environment.NewLine}{Indents.ClassMethodBody}{{{Environment.NewLine}{Indents.ClassMethodBodyIndent}{body}{Environment.NewLine}{Indents.ClassMethodBody}}}{Environment.NewLine}";
            string catchClauseBody = $"{Indents.ClassMethodBodyIndent}if ( !{Repository.Poco.ClassName}Exists({Repository.Poco.KeysPassThru}) ) return NotFound();{Environment.NewLine}{Indents.ClassMethodBodyIndent}throw;{Environment.NewLine}";
            string catchClause = $"{Indents.ClassMethodBody}catch ( DbUpdateConcurrencyException ){Environment.NewLine}{Indents.ClassMethodBody}{{{Environment.NewLine}{catchClauseBody}{Indents.ClassMethodBody}}}{Environment.NewLine}";

            return $"{tryClause}{catchClause}";
        }

        private string TryCatchExists(string body)
        {
            string tryClause = $"{Indents.ClassMethodBody}try{Environment.NewLine}{Indents.ClassMethodBody}{{{Environment.NewLine}{Indents.ClassMethodBodyIndent}{body}{Environment.NewLine}{Indents.ClassMethodBody}}}{Environment.NewLine}";
            string catchClauseBody = $"{Indents.ClassMethodBodyIndent}if ( {Repository.Poco.ClassName}Exists({Repository.Poco.KeysPassThru}) ) return Conflict();{Environment.NewLine}{Indents.ClassMethodBodyIndent}throw;{Environment.NewLine}";
            string catchClause = $"{Indents.ClassMethodBody}catch ( DbUpdateConcurrencyException ){Environment.NewLine}{Indents.ClassMethodBody}{{{Environment.NewLine}{catchClauseBody}{Indents.ClassMethodBody}}}{Environment.NewLine}";

            return $"{tryClause}{catchClause}";
        }

        private string TryCatchExistsPoco(string body)
        {
            string tryClause = $"{Indents.ClassMethodBody}try{Environment.NewLine}{Indents.ClassMethodBody}{{{Environment.NewLine}{Indents.ClassMethodBodyIndent}{body}{Environment.NewLine}{Indents.ClassMethodBody}}}{Environment.NewLine}";
            string catchClauseBody = $"{Indents.ClassMethodBodyIndent}if ( {Repository.Poco.ClassName}Exists({Repository.Poco.KeysPassThruEntity}) ) return Conflict();{Environment.NewLine}{Indents.ClassMethodBodyIndent}throw;{Environment.NewLine}";
            string catchClause = $"{Indents.ClassMethodBody}catch ( DbUpdateConcurrencyException ){Environment.NewLine}{Indents.ClassMethodBody}{{{Environment.NewLine}{catchClauseBody}{Indents.ClassMethodBody}}}{Environment.NewLine}";

            return $"{tryClause}{catchClause}";
        }

        private string PutMethod
        {
            get
            {
                string methodSignature = $"{Indents.ClassMethod}public IHttpActionResult Put({Repository.Poco.KeysOData}, Delta<{Repository.Poco.ClassName}> patch){Environment.NewLine}{Indents.ClassMethodBracket}{{{Environment.NewLine}";
                string validateClause = $"{Indents.ClassMethodBody}Validate( patch.GetEntity() );{Environment.NewLine}{Environment.NewLine}";
                string badRequestCheck = $"{Indents.ClassMethodBody}if ( !ModelState.IsValid ) return BadRequest( ModelState );{Environment.NewLine}{Environment.NewLine}";
                string findEntity = $"{Indents.ClassMethodBody}{Repository.Poco.ClassName} {Repository.Poco.ClassName.ToLowerInvariant()} = {Repository.Poco.ClassName}Gateway.Select({Repository.Poco.KeysPassThru});{Environment.NewLine}{Environment.NewLine}";
                string notFoundCheck = $"{Indents.ClassMethodBody}if ( {Repository.Poco.ClassName.ToLowerInvariant()} == null ) return NotFound();{Environment.NewLine}{Environment.NewLine}";
                string patchPut = $"{Indents.ClassMethodBody}patch.Put({Repository.Poco.ClassName.ToLowerInvariant()});{Environment.NewLine}";
                string tryCatchClause = TryCatchNotExists($"{Repository.Poco.ClassName}Gateway.Update({Repository.Poco.ClassName.ToLowerInvariant()});");
                string returnUpdated = $"{Indents.ClassMethodBody}return Updated({Repository.Poco.ClassName.ToLowerInvariant()});{Environment.NewLine}";
                string endOfMethod = $"{Indents.ClassMethodBracket}}}{Environment.NewLine}";

                return $"{methodSignature}{validateClause}{badRequestCheck}{findEntity}{notFoundCheck}{patchPut}{tryCatchClause}{returnUpdated}{endOfMethod}{Environment.NewLine}";
            }
        }

        private string PostMethod
        {
            get
            {
                string methodSignature = $"{Indents.ClassMethod}public IHttpActionResult Post({Repository.Poco.ClassName} {Repository.Poco.ClassName.ToLowerInvariant()}){Environment.NewLine}{Indents.ClassMethodBracket}{{{Environment.NewLine}";
                string badRequestCheck = $"{Indents.ClassMethodBody}if ( !ModelState.IsValid ) return BadRequest( ModelState );{Environment.NewLine}{Environment.NewLine}";
                string tryCatchClause = TryCatchExistsPoco($"{Repository.Poco.ClassName}Gateway.Insert({Repository.Poco.ClassName.ToLowerInvariant()});");
                string returnCreated = $"{Indents.ClassMethodBody}return Created({Repository.Poco.ClassName.ToLowerInvariant()});{Environment.NewLine}";
                string endOfMethod = $"{Indents.ClassMethodBracket}}}{Environment.NewLine}";

                return $"{methodSignature}{badRequestCheck}{tryCatchClause}{returnCreated}{endOfMethod}{Environment.NewLine}";
            }
        }

        private string PatchMethod
        {
            get
            {
                string annotation = $"{Indents.ClassMethod}[AcceptVerbs(\"PATCH\",\"MERGE\")]{Environment.NewLine}";
                string methodSignature = $"{Indents.ClassMethod}public IHttpActionResult Patch({Repository.Poco.KeysOData}, Delta<{Repository.Poco.ClassName}> patch){Environment.NewLine}{Indents.ClassMethodBracket}{{{Environment.NewLine}";
                string validateClause = $"{Indents.ClassMethodBody}Validate( patch.GetEntity() );{Environment.NewLine}{Environment.NewLine}";
                string badRequestCheck = $"{Indents.ClassMethodBody}if ( !ModelState.IsValid ) return BadRequest( ModelState );{Environment.NewLine}{Environment.NewLine}";
                string findEntity = $"{Indents.ClassMethodBody}{Repository.Poco.ClassName} {Repository.Poco.ClassName.ToLowerInvariant()} = {Repository.Poco.ClassName}Gateway.Select({Repository.Poco.KeysPassThru});{Environment.NewLine}{Environment.NewLine}";
                string notFoundCheck = $"{Indents.ClassMethodBody}if ( {Repository.Poco.ClassName.ToLowerInvariant()} == null ) return NotFound();{Environment.NewLine}{Environment.NewLine}";
                string patchPut = $"{Indents.ClassMethodBody}patch.Put({Repository.Poco.ClassName.ToLowerInvariant()});{Environment.NewLine}";
                string tryCatchClause = TryCatchNotExists($"{Repository.Poco.ClassName}Gateway.Update({Repository.Poco.ClassName.ToLowerInvariant()});");
                string returnUpdated = $"{Indents.ClassMethodBody}return Updated({Repository.Poco.ClassName.ToLowerInvariant()});{Environment.NewLine}";
                string endOfMethod = $"{Indents.ClassMethodBracket}}}{Environment.NewLine}";

                return $"{annotation}{methodSignature}{validateClause}{badRequestCheck}{findEntity}{notFoundCheck}{patchPut}{tryCatchClause}{returnUpdated}{endOfMethod}{Environment.NewLine}";
            }
        }

        private string DeleteMethod
        {
            get
            {
                string methodSignature = $"{Indents.ClassMethod}public IHttpActionResult Delete({Repository.Poco.KeysOData}){Environment.NewLine}{Indents.ClassMethodBracket}{{{Environment.NewLine}";
                string findEntity = $"{Indents.ClassMethodBody}{Repository.Poco.ClassName} {Repository.Poco.ClassName.ToLowerInvariant()} = {Repository.Poco.ClassName}Gateway.Select({Repository.Poco.KeysPassThru});{Environment.NewLine}{Environment.NewLine}";
                string notFoundCheck = $"{Indents.ClassMethodBody}if ( {Repository.Poco.ClassName.ToLowerInvariant()} == null ) return NotFound();{Environment.NewLine}{Environment.NewLine}";
                string deleteEntity = $"{Indents.ClassMethodBody}{Repository.Poco.ClassName}Gateway.Delete({Repository.Poco.ClassName.ToLowerInvariant()});{Environment.NewLine}{Environment.NewLine}";
                string returnNoContent = $"{Indents.ClassMethodBody}return StatusCode( HttpStatusCode.NoContent );{Environment.NewLine}";
                string endOfMethod = $"{Indents.ClassMethodBracket}}}{Environment.NewLine}";

                return $"{methodSignature}{findEntity}{notFoundCheck}{deleteEntity}{returnNoContent}{endOfMethod}{Environment.NewLine}";
            }
        }

        private string ExistsMethod
        {
            get
            {
                string methodSignature = $"{Indents.ClassMethod}private bool {Repository.Poco.ClassName}Exists({Repository.Poco.Keys}){Environment.NewLine}{Indents.ClassMethodBracket}{{{Environment.NewLine}";
                string returnIsSingleResult = $"{Indents.ClassMethodBody}return {Repository.Poco.ClassName}Gateway.IsSingleResult( {Repository.Poco.KeysPassThru} );{Environment.NewLine}";
                string endOfMethod = $"{Indents.ClassMethodBracket}}}{Environment.NewLine}";

                return $"{methodSignature}{returnIsSingleResult}{endOfMethod}{Environment.NewLine}";
            }
        }


        private string ClassFooter => $"{Environment.NewLine}{Indents.ClassBracket}}}{Environment.NewLine}";
        private string UsingBlock => string.IsNullOrWhiteSpace(Using) ? "" : $"{Using}{Environment.NewLine}";
        public override string ToString()
        {
            return $"{UsingBlock}{NamespaceHeader}{ClassHeader}{GetAllMethod}{GetMethod}{PutMethod}{PostMethod}{PatchMethod}{DeleteMethod}{ExistsMethod}{ClassFooter}{NamespaceFooter}";
        }
    }
}
