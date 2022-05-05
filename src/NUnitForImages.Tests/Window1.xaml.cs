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

namespace NUnitForImages
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    [Apartment(ApartmentState.STA)]    
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        [Test]        
        public void RenderTest()
        {
            var render = WpfImage
                .Render(this)                           // renders this WPF control to a bitmap.                
                .SaveTo(TestContext.CurrentContext.GetAttachmentFileInfo("window.png").FullName)        // saves the bitmap to current test directory.
                .AttachToTest("Window1 render");    // attaches the saved file to the test output.            
        }
    }
}
