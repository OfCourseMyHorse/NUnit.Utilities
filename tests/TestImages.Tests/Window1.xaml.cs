﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class Window1 : Window , INotifyPropertyChanged
    {
        public Window1()
        {
            InitializeComponent();

            ButtonAClickCmd = new Prism.Commands.DelegateCommand(() => { myButtonA.Content = "A Button Clicked"; _ButtonAClicked = true; } );
        }

        public ICommand ButtonAClickCmd { get; }

        public string DynamicProperty { get; private set; }

        private bool _ButtonAClicked;

        public event PropertyChangedEventHandler PropertyChanged;

        [Test]        
        public void RenderTest()
        {
            var image1 = WpfTestImage
                .FromRender(this)
                .AssertThat(ImageProperty.PixelArea, Is.GreaterThan(0))
                .SaveTo(AttachmentInfo.From("window1.png", "Window1 render") )
                .AssertThat(ImageProperty.CheckSum, Is.EqualTo(4030796632));            

            // run command
            ButtonAClickCmd.Execute(null);
            Assert.That(_ButtonAClicked);

            // change a property
            DynamicProperty = "Modified";
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DynamicProperty)));

            var image2 = WpfTestImage
                .FromRender(this)
                .AssertThat(ImageProperty.PixelArea, Is.GreaterThan(0))
                .SaveTo(AttachmentInfo.From("window2.png", "Window1 render after changes"))
                .AssertThat(ImageProperty.CheckSum, Is.EqualTo(201499534));

        }        
    }
}
