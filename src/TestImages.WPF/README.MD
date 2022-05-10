
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


#### References

- [FlaUI test framework](https://github.com/FlaUI/FlaUI)
  - [White test framework (deprecated)](https://github.com/TestStack/White)
- [CodedUI (commercial)](https://docs.microsoft.com/en-us/visualstudio/test/use-ui-automation-to-test-your-code?view=vs-2022)
- [testing suites for net wpf based applications](https://stackoverflow.com/questions/3234980/testing-suites-for-net-wpf-based-applications)
- [UIAutomationTests.cs](https://github.com/microsoft/TestApi/blob/master/Development/Samples/NUnit/Tests/UIAutomationTests.cs)
- [simulate mouse input](https://social.msdn.microsoft.com/Forums/vstudio/en-US/0555c1ea-5b0f-4c5e-863c-b54eb510ad40/simulate-mouse-entermoveleave-on-wpf-control-without-real-mouse-usage?forum=wpf)
- [programatically click a button](https://joshsmithonwpf.wordpress.com/2007/03/09/how-to-programmatically-click-a-button/)


