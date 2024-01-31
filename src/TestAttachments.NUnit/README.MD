


## Overview

When writing tests that depend heavily on resources and attachment files, it is usually
convenient to have an easy way to retrieve the resources, and also to have a way to
organize the attached files based on the test IDs.

NUnit provides the properties needed to build per test paths in the `TestContext` object,
but the rest is up to the developer.

This library provides a last mile API, easing the process of working with resources and file attachments.

## Usage

The library defines two attributes

- `ResourcePathFormat(string format)`
- `AttachmentPathFormat(string format)`

Which can be used in assemblies, classes and individual methods.

The format defines how the test output directory will be created for the given test, by using a number of macros:

Predefined absolute path macross:

|macro|value|
|-|-|
| WorkDirectory or * | context.WorkDirectory |
| TestDirectory | context.TestDirectory |
| TempDirectory | System.IO.Path.GetTempPath() |
| CurrentDirectory | Environment.CurrentDirectory |
| SolutionDirectory | finds the top most solution (.sln) file and takes its directory |

predefined relative path macross:

|macro|value|
|-|-|
| ID or ? | context.Test.ID |
| Name | context.Test.Name |
| FullName | context.Test.FullName |
| ClassName | context.Test.ClassName |
| MethodName | context.Test.MethodName |
| CurrentRepeatCount | context.CurrentRepeatCount |
| WorkerId | context.WorkerId |
| Date | DateTime.Now 'yyyMMdd' |
| Time | DateTime.Now 'hhmmss' |

By using these macros, we can define the attribute like this:

```c#
[AttachmentPathFormat("{WorkDirectory}/{ID}/{Date}")]
```

when running a test, it will replace the macros with the appropiate
values taken from the current test context.

the default and most common use will be

```c#
[AttachmentPathFormat("?")]
```

which creates a directory for every test.


## Example

```c#

using NUnit.Framework;

[assembly: ResourcePathFormat("{TestDirectory}/Resources")]
[assembly: AttachmentPathFormat("{WorkDirectory}/TestResults/{ID}")]

namespace TestNamespace
{
    [AttachmentPathFormat("{WorkDirectory}/{ID}")] // Alternatively: [AttachmentPathFormat("?")]
    public class TestClass
    {
        [Test]
        public void WriteTextAttachment()        
        {
            var text = ResourceInfo
                .From("hello.txt")
                .ReadAllText();

                text += " - good bye!";

            AttachmentInfo
                .From("result.txt")
                .WriteAllText(text);            
        }

        [Test]
        [AttachmentPathFormat("{WorkDirectory}/ExplicitMethodResult-{Date}-{Time}")]
        public void WriteExplicitTextAttachment()
        {
            var text = ResourceInfo
                .From("hello.txt")
                .ReadAllText();

            text += " - good bye!";

            AttachmentInfo
                .From("result.txt")
                .WriteAllText(text);     
        }
    }
}

```

## Related issues

[Nunit Proposal](https://github.com/nunit/nunit/issues/4020)