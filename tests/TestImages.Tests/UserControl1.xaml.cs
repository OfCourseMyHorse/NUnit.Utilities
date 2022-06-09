using System.Threading;
using System.Windows.Controls;

using NUnit.Framework;

namespace TestImages
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>    
    [AttachmentPathFormat("?")]
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
            TestContext.CurrentContext.AttachFolderBrowserShortcut();

            WpfTestImage
                .FromRender(this)                                                       // renders this WPF control to a bitmap.
                .AssertThat(Property.PixelArea, Is.GreaterThan(0))
                .SaveTo(AttachmentInfo.From("render.png"))                          // saves the bitmap to current test directory.                
                .AssertThat(Property.PixelsHashCode, Is.EqualTo(79668204))
                .AssertThat(Is.EqualTo(WpfTestImage.Load("UserControl1.reference.png")))
                .AssertThat(Comparison.With(WpfTestImage.Load("UserControl1.reference.png")).ByStandardDeviation, Is.LessThan(1));
        }

        [Test]
        public void RenderTestCustomSize()
        {
            TestContext.CurrentContext.AttachFolderBrowserShortcut();

            WpfTestImage
                .FromRender(this, new System.Windows.Size(512,512))                
                .SaveTo(AttachmentInfo.From("render.png"));
        }
    }
}
