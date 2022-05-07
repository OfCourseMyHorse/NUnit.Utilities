using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NUnit.Framework
{
    /// <summary>
    /// Defines a format string that will be used to evaluate the output test subdirectory based on current test state.
    /// </summary>
    /// <remarks>    
    /// <para>
    /// <br/>
    /// predefined absolute path macross:<br/>
    /// - {WorkDirectory} or * = context.WorkDirectory<br/>
    /// - {TestDirectory} = context.TestDirectory<br/>
    /// - {TempDirectory} = System.IO.Path.GetTempPath()<br/>
    /// - {CurrentDirectory} = Environment.CurrentDirectory<br/>
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
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AttachmentPathFormatAttribute : PropertyAttribute
    {

        #region constructors
        
        /// <summary>
        /// Declares a directory formatter to write test attachments.
        /// </summary>
        /// <param name="format">Recommended patterns are: "?" , "*/?" or "*/yourClassName/?" </param>
        public AttachmentPathFormatAttribute(string format) : base(format) { }        

        #endregion
    }
}
