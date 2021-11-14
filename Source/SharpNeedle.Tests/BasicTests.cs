using NUnit.Framework;

namespace SharpNeedle.Tests;

public class BasicTests
{
    [SetUp]
    public void Setup()
    {

    }

    [Test]
    public void MakeSignatureTest()
    {
        Assert.AreEqual(BinaryHelper.MakeSignature<short>("69"), 0x3936);
        Assert.AreEqual(BinaryHelper.MakeSignature<uint>("ARL2"), 0x324C5241);
        Assert.AreEqual(BinaryHelper.MakeSignature<long>("Model", 0x20), 0x2020206C65646F4D);
        Assert.AreEqual(BinaryHelper.MakeSignature<long>("Contexts", 0x20), 0x73747865746E6F43);
    }

    // TODO: Add tests for resource reading/writing
}