# NUnit Utilities
![GitHub](https://img.shields.io/github/license/OfCourseMyHorse/NUnit.Utilities)
[![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/NUnitForImages)](https://www.nuget.org/packages?q=nunitforimages)

### Overview

Repository to keep several NUnit utilities and extensions.

### NUnitForImages

These projects contain NUnit helper classes to test images types.

In particular, the design idea is to be able to do tests against GUI controls.

The first implementation is NUnitForImages.WPF which contains a helper class used to render a WPF control to a bitmap, inside a unit test, and assert that the rendered result is correct.

A typical used case of WPF testing is to check whether data bindings are broken or not. This is usually not trivial, or requires some degree of code analysis. With NUnitForImages.WPF you can simply bind a ModelView object to a DataContext, render the control in a unit test, and see if the rendered result is the same as previously recorded.

NUnitForImages.WPF usage is very simple, [check this example](src/NUnitForImages.Tests/UserControl1.xaml.cs).



