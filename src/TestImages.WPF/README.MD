
#### Overview

This project extends TestImages.Abstraction to provide WPF support.

Its main purpose is to help executing tests on individual WPF controls, basically
allowing to test if a specific control renders correctly within a unit test.

#### Example

```c#

using System;
using NUnit.Framework;
using TestImages;

namespace WpfRenderingTests
{
    [AttachmentPathFormat("?")]                           // directory attachments per test
    [RequiresThread(System.Threading.ApartmentState.STA)] // required for WPF rendering
    partial class UserControl1
    {
        public UserControl1()
        {
            InitializeComponent();
        }

        [Test]        
        public void RenderTest() // test method within a user control's partial class
        {
            WpfImage
                .FromRender(this)                                           // renders this WPF control to a bitmap.
                .SaveTo(AttachmentInfo.From("render.png"))                  // saves the bitmap as a test attachment.                
                .AssertThat(Property.PixelsHashCode, Is.EqualTo(79668204)); // Asserts that the rendered bitmap has the right hash code. 
        }
    }
}

```

Tests can be done on a control per control, but the recomendation is to create
a UserControl, and batch multiple controls inside, so all of them will be rendered in one shot.


#### References

- [FlaUI test framework](https://github.com/FlaUI/FlaUI)
  - [White test framework (deprecated)](https://github.com/TestStack/White)
- [CodedUI (commercial)](https://docs.microsoft.com/en-us/visualstudio/test/use-ui-automation-to-test-your-code?view=vs-2022)
- [testing suites for net wpf based applications](https://stackoverflow.com/questions/3234980/testing-suites-for-net-wpf-based-applications)
- [UIAutomationTests.cs](https://github.com/microsoft/TestApi/blob/master/Development/Samples/NUnit/Tests/UIAutomationTests.cs)
- [simulate mouse input](https://social.msdn.microsoft.com/Forums/vstudio/en-US/0555c1ea-5b0f-4c5e-863c-b54eb510ad40/simulate-mouse-entermoveleave-on-wpf-control-without-real-mouse-usage?forum=wpf)
- [programatically click a button](https://joshsmithonwpf.wordpress.com/2007/03/09/how-to-programmatically-click-a-button/)

#### Bonus track

When debugging images, it's good to use Visual Studio [ImageVisualizer](https://github.com/aberus/ImageVisualizer) extension.


