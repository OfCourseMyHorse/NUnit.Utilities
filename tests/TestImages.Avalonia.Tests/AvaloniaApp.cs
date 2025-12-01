using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Avalonia.Styling;
using Avalonia;
using Avalonia.Headless;
using Avalonia.Themes.Simple;

using TestImages;

[assembly: AvaloniaTestApplication(typeof(TestApplication))]

namespace TestImages
{
    public class TestApplication : Application
    {
        public TestApplication()
        {
            Styles.Add(new SimpleTheme());
        }

        public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<TestApplication>()
            .UseSkia()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions
            {
                UseHeadlessDrawing = false
            });
    }
}
