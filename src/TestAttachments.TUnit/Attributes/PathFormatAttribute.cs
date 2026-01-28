using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace TUnit
{
    /// <summary>
    /// Defines a format string that will be used to evaluate the output test subdirectory based on current test state.
    /// </summary>
    /// <remarks>    
    /// <para>
    /// <br/>
    /// predefined absolute path macross:<br/>
    /// - {DefaultDirectory} = context.TestDirectory | context.WorkDirectory<br/>
    /// - {WorkDirectory} or * = context.WorkDirectory<br/>
    /// - {TestDirectory} = context.TestDirectory<br/>
    /// - {TempDirectory} = System.IO.Path.GetTempPath()<br/>
    /// - {CurrentDirectory} = Environment.CurrentDirectory<br/>
    /// - {ProjectDirectory} = directory of the first .csproj found up the directory tree.
    /// - {SolutionDirectory} = directory of the first .sln found up the directory tree.
    /// - {RepositoryRoot} = directory containing .git or .svn directories
    /// </para>    
    /// <para>
    /// <br/>
    /// predefined relative path macross:<br/>
    /// - {ID} or ? = context.Test.ID<br/>     
    /// - {Name} = context.Test.Name<br/>        
    /// - {FullName} = context.Test.FullName<br/>
    /// - {ClassName} = context.Test.ClassName<br/>
    /// - {MethodName} = context.Test.MethodName<br/>
    /// - {CurrentRepeatCount} == context.CurrentRepeatCount<br/>
    /// - {WorkerId} == context.WorkerId<br/>
    /// - {Date} == DateTime.Now 'yyyMMdd'<br/>
    /// - {Time} == DateTime.Now 'hhmmss'<br/>
    /// </para>
    /// <para>
    /// <br/>
    /// Examples:<br/>
    /// <c>[AttachmentPathFormat("*/?")]</c><br/>
    /// <c>[AttachmentPathFormat("{WorkDirectory}/AttachmentResults/{ID}")]</c>
    /// </para>
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]    
    public abstract class PathFormatAttribute : PropertyAttribute
    {
        #region constructors

        /// <summary>
        /// Declares a directory formatter.
        /// </summary>
        /// <param name="format">Recommended patterns are: "?" , "*/?" or "*/yourClassName/?" </param>
        protected PathFormatAttribute(string key, string value) : base(key, value) { }

        #endregion

        #region properties

        public string Format => this.Value;

        #endregion
    }
}
