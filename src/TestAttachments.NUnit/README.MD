

## Overview

When writing tests that depend heavily on file attachments, it is usually convenient
to organize the files based on the test IDs.

NUnit provides the properties needed to build per test paths, but the rest is up to the
developer.

This library provides the last mile, so it is much easier to work with file attachments.

## Usage

The library defines the attribute `AttachmentPathFormat(string format)` which can be
used in assemblies, classes and individual methods.

the format defines how the test output directory will be created for the given test, by using a number of macros:

predefined absolute path macross:

|macro|value|
|-|-|
| WorkDirectory or * | context.WorkDirectory |
| TestDirectory | context.TestDirectory |
| TempDirectory | System.IO.Path.GetTempPath() |
| CurrentDirectory | Environment.CurrentDirectory |

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

when runnung a test, it will replace the macros with the appropiate
values taken from the current test context.


## Example

```c#

using NUnit.Framework;

[assembly: AttachmentPathFormat("{WorkDirectory}/AssemblyResults/{ID}")]

namespace TestNamespace
{
    [AttachmentPathFormat("{WorkDirectory}/{ID}")] // Alternatively: [AttachmentPathFormat("?")]
    public class TestClass
    {
        static AttachmentInfo Attach(string name, string desc = null) => new AttachmentInfo(name,desc);

        [Test]
        public void WriteTextAttachment()
        {
            Attach("hello.txt").WriteText("hello world");            
        }

        [Test]
        [AttachmentPathFormat("{WorkDirectory}/ExplicitMethodResult-{Date}-{Time}")]
        public void WriteExplicitTextAttachment()
        {
            Attach("hello.txt").WriteText("hello world");
        }
    }
}

```

## Related issues

[Nunit Proposal](https://github.com/nunit/nunit/issues/4020)