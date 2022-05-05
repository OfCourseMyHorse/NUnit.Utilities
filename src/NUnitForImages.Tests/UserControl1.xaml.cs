﻿using System.Threading;
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
            var render = WpfImage
                .Render(this)                           // renders this WPF control to a bitmap.
                .SaveTo(Attach("render.png"))        // saves the bitmap to current test directory.                
                .VerifyCodeIsAnyOf(79668204);      // Asserts that the rendered bitmap has the reference hash code.

            var reference = WpfImage.Load("UserControl1.reference.png");

            Assert.AreEqual(reference, render);         // compare the rendered bitmap against the reference.
        }
    }
}
