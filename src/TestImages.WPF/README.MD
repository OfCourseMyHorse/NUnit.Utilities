
#### NUnit for WPF

This utility project allows rendering and testing WPF controls inside NUnit tests.

#### Example

```c#

using System;
...
using NUnit.Framework;
using NUnitForImages;

namespace WpfRenderingTests
{
    [RequiresThread(System.Threading.ApartmentState.STA)] // required for WPF rendering
    partial class UserControl1
    {
        public UserControl1()                       // default constructor
        {
            InitializeComponent();
        }

        [Test]        
        public void RenderTest()                    // test method within a user control's partial class
        {
            WpfImage
                .Render(this)                           // renders this WPF control to a bitmap.
                .SaveToCurrentTest("render.png")        // saves the bitmap to current test directory.
                .AttachToTest("UserControl1 render")    // attaches the saved file to the test output.
                .AssertHashCodeIsAnyOf(536880688);      // Asserts that the rendered bitmap has the right hash code. 
        }
    }
}

```


#### Bonus track

When debugging images, it's good to use Visual Studio [ImageVisualizer](https://github.com/aberus/ImageVisualizer) extension.