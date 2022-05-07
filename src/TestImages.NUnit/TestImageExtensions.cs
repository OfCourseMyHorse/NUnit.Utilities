using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NUnit.Framework
{
    public static class TestImageExtensions
    {
        /// <summary>
        ///  Apply a constraint to a <see cref="TestImages.TestImage"/>.
        ///  Returns without throwing an exception when inside a multiple assert block.
        /// </summary>        
        /// <param name="self">The <see cref="TestImages.TestImage"/> image to test.</param>        
        /// <param name="constraint">A Constraint expression to be applied.</param>
        /// <returns>Fluent self.</returns>
        public static TestImages.TestImage AssertThat(this TestImages.TestImage self, Constraints.IResolveConstraint constraint)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));            
            if (constraint == null) throw new ArgumentNullException(nameof(constraint));
            Assert.That(self, constraint);
            return self;
        }

        /// <summary>
        ///  Apply a constraint to a <see cref="TestImages.TestImage"/>.
        ///  Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <typeparam name="T">The actual value type to test.</typeparam>
        /// <param name="self">The <see cref="TestImages.TestImage"/> image to test.</param>
        /// <param name="imageEvaluator">A callback that evaluates the image into a measurable quantity.</param>
        /// <param name="constraint">A Constraint expression to be applied.</param>
        /// <returns>Fluent self.</returns>
        public static TestImages.TestImage AssertThat<T>(this TestImages.TestImage self, TestImages.TestImageEvaluator<T> imageEvaluator, Constraints.IResolveConstraint constraint)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            if (imageEvaluator == null) throw new ArgumentNullException(nameof(imageEvaluator));
            if (constraint == null) throw new ArgumentNullException(nameof(constraint));
            Assert.That(imageEvaluator.Invoke(self), constraint);
            return self;
        }        
    }
}
