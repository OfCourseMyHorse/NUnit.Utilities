using System.Threading;
using System.Windows.Controls;

using NUnit.Framework;

namespace TestImages
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>    
    [Apartment(ApartmentState.STA)]
    public partial class UserControl1 : UserControl, IAttachmentWriter
    {
        public UserControl1()
        {
            InitializeComponent();
        }

        public AttachmentInfo Attach(string fileName, string description = null) => new AttachmentInfo(fileName, description);

        [Test]
        public void RenderTest()
        {
            WpfImage
                .Render(this)                           // renders this WPF control to a bitmap.
                .AssertThat(Property.PixelArea, Is.GreaterThan(0))
                .SaveTo(Attach("render.png"))           // saves the bitmap to current test directory.                
                .AssertThat(Property.PixelsHashCode, Is.EqualTo(79668204))
                .AssertThat(Is.EqualTo(WpfImage.Load("UserControl1.reference.png")))
                .AssertThat(Comparison.With(WpfImage.Load("UserControl1.reference.png")).ByStandardDeviation, Is.LessThan(1));
        }
    }
}
