using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using Avalonia.Skia;
using Avalonia.Themes.Simple;
using Avalonia.Threading;

using NUnit.Framework;


// https://github.com/AvaloniaUI/Avalonia/issues/15179
// https://github.com/AvaloniaUI/Avalonia/issues/2843
// https://github.com/kekekeks/Avalonia-unit-testing-with-headless-platform
// https://github.com/AvaloniaUI/Avalonia/tree/master/src/Headless/Avalonia.Headless

namespace TestImages
{
    public partial class TestControl1 : UserControl
    {
        public TestControl1()
        {
            SkiaPlatform.Initialize();

            Styles.Add(new SimpleTheme());

            InitializeComponent();
        }

        [Test]
        public void RenderTest()
        {
            AvaloniaTestImage
                .FromRender(this, new PixelSize(256,256))                            // renders this WPF control to a bitmap.
                .AssertThat(ImageProperty.PixelArea, Is.GreaterThan(0))
                .SaveTo(AttachmentInfo.From("render.png"));                          // saves the bitmap to current test directory.                
        }
    }
}