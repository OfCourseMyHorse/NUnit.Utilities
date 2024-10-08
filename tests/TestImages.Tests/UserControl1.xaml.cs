﻿using System.Threading;
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
                .AssertThat(ImageProperty.PixelArea, Is.GreaterThan(0))
                .SaveTo(AttachmentInfo.From("render.png"))                          // saves the bitmap to current test directory.                
                .AssertThat(ImageProperty.CheckSum, Is.EqualTo(1362971363))
                .AssertThat(Is.EqualTo(WpfTestImage.Load("UserControl1.reference.png")))
                .AssertThat(ImageComparison.With(WpfTestImage.Load("UserControl1.reference.png")).ByStandardDeviation, Is.LessThan(1));
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
