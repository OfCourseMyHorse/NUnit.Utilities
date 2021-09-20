using System.Threading;
using System.Windows.Controls;

using NUnit.Framework;

namespace NUnitForImages
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>    
    [Apartment(ApartmentState.STA)]
    public partial class UserControl1 : UserControl
    {
        public UserControl1()
        {
            InitializeComponent();
        }

        [Test]
        public void RenderTest()
        {
            var render = WpfImage
                .Render(this)                           // renders this WPF control to a bitmap.
                .SaveToCurrentTest("render.png")        // saves the bitmap to current test directory.
                .AttachToTest("UserControl1 render")    // attaches the saved file to the test output.
                .AssertHashCodeIsAnyOf(79668204);      // Asserts that the rendered bitmap has the reference hash code.            


            var reference = WpfImage.Load("UserControl1.reference.png");

            Assert.AreEqual(reference, render);         // compare the rendered bitmap against the reference.
        }
    }
}
