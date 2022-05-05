

## Overview

When writing tests that depend heavily on file attachments, it is usually convenient
to organize the files based on the test IDs.

NUnit provides the properties needed to build per test paths, but the rest is up to the
developer.

This library provides the last mile, so it is much easier to work with file attachments.

## Example

```c#

using NUnit.Framework;

[assembly: AttachmentPathFormat("{WorkDirectory}/AssemblyResults/{ID}")]

namespace TestNamespace
{
    [AttachmentPathFormat("{WorkDirectory}/{ID}")] // create a directory per test method
    public class TestClass
    {
        [Test]
        public void WriteTextAttachment()
        {
            TestContext.CurrentContext.Attachment("hello.txt").WriteText("hello world");            
        }

        [Test]
        [AttachmentPathFormat("{WorkDirectory}/ExplicitMethodResult-{Date}-{Time}")]
        public void WriteExplicitTextAttachment()
        {
            TestContext.CurrentContext.Attachment("hello.txt").WriteText("hello world");
        }
    }
}

```

## Related issues

[Nunit Proposal](https://github.com/nunit/nunit/issues/4020)