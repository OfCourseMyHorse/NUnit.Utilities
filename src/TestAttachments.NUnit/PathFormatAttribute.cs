using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// - {ProjectDirectory} = directory of the first .csproj found up the directory tree.
    /// - {SolutionDirectory} = directory of the first .sln found up the directory tree.
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
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public abstract class PathFormatAttribute : PropertyAttribute
    {
        #region constructors

        /// <summary>
        /// Declares a directory formatter.
        /// </summary>
        /// <param name="format">Recommended patterns are: "?" , "*/?" or "*/yourClassName/?" </param>
        public PathFormatAttribute(string format) : base(format) { }

        #endregion        

        #region API

        internal static TestContext.PropertyBagAdapter _FindProperties<T>(TestContext context, string propertyName)
            where T : PathFormatAttribute
        {
            // look for attribute in current method:

            if (context.Test.Properties.ContainsKey(propertyName)) return context.Test.Properties;

            // look for attribute in current class:

            var instanceType = _GetCurrentClassInstanceType();

            var attribute = _TryGetFrom<T>(instanceType);
            if (attribute != null) return new TestContext.PropertyBagAdapter(attribute.Properties);

            // look for attribute in current assembly:

            attribute = _TryGetFrom<T>(instanceType.Assembly);
            if (attribute != null) return new TestContext.PropertyBagAdapter(attribute.Properties);

            return null;
        }

        private static T _TryGetFrom<T>(System.Reflection.ICustomAttributeProvider t)
            where T : Attribute
        {
            return t.GetCustomAttributes(typeof(T), true)
                .OfType<T>()
                .FirstOrDefault();
        }

        private static Type _GetCurrentClassInstanceType()
        {
            // this is a hack, but it's the only way to retrieve class level information from the TestContext.

            // see https://github.com/nunit/nunit/issues/548

            var testObject = NUnit.Framework.Internal.TestExecutionContext.CurrentContext.TestObject;
            return testObject.GetType();
        }

        #endregion
    }
}
