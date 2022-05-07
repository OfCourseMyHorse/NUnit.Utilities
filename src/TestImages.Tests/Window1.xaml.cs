using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using NUnit.Framework;

namespace TestImages
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    [Apartment(ApartmentState.STA)]    
    public partial class Window1 : Window , IAttachmentWriter
    {
        public AttachmentInfo Attach(string fileName, string description = null) => new AttachmentInfo(fileName, description);

        public Window1()
        {
            InitializeComponent();
        }        

        [Test]        
        public void RenderTest()
        {
            var render = WpfImage
                .Render(this)
                .AssertThat(Property.PixelArea, Is.GreaterThan(0))
                .SaveTo( Attach("window.png", "Window1 render") )
                .AssertThat(Property.PixelsHashCode, Is.EqualTo(-686246400));            
        }
    }
}
