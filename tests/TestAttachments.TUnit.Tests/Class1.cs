using TUnit;

[assembly: Property("Test","A")]

namespace TestAttachments.TUnit.Tests
{
    [Property("Test","B")]
    [ResourcePathFormat("{ProjectDirectory}/Resources/")]    
    public class TestPropertyHierarchy
    {

        [Property("Test", "C")]
        [Test]
        
        public async Task Test1()
        {
            TestContext.Current.Metadata.TestDetails.CustomProperties.TryGetValue("Test", out var list);


            var txt = ResourceInfo.From("text1.txt").ReadAllText();

            AttachmentInfo.From("result.txt").WriteAllText(txt);
        }

    }
}
