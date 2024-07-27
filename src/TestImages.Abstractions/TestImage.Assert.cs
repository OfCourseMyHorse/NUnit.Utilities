using System;
using System.Collections.Generic;
using System.Text;

namespace TestImages
{
    public readonly struct TestImageAssert
    {
        internal TestImageAssert(TestImage image)
        {
            _Image = image;
        }

        private readonly TestImage _Image;
    }
}
