using System;
using System.Collections.Generic;
using System.Text;

namespace TestImages
{
    internal class Exceptions
    {
        #if NET6_0_OR_GREATER
        [System.Diagnostics.StackTraceHidden]
        #endif
        [System.Diagnostics.DebuggerStepThrough]
        public static void ReportFail(string message)
        {
            _TryThrowWithNUnit(message);

            // Todo: add XUnit
            // Todo: add MSTest

            // fallback

            if (message == null) throw new InvalidOperationException();
            else throw new InvalidOperationException(message);
        }

        #if NET6_0_OR_GREATER
        [System.Diagnostics.StackTraceHidden]
        #endif
        [System.Diagnostics.DebuggerStepThrough]
        private static void _TryThrowWithNUnit(string message)
        {
            if (message == null) message = string.Empty;

            var nunitAssertClass = Type.GetType("NUnit.Framework.Assert, nunit.framework");
            if (nunitAssertClass == null) return;

            var failMethod = nunitAssertClass.GetMethod("Fail", new[] { typeof(string) });
            if (failMethod == null) return;

            try
            {
                failMethod.Invoke(null, new object[] { message });
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                if (ex.InnerException != null) throw ex.InnerException; // must be an AssertException
            }
        }
    }
}
