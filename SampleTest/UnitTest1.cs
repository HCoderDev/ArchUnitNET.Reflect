using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using System.Reflection;

namespace SampleTest
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Architecture ChefArchitecture =
            new ArchLoader().LoadAssemblies(System.Reflection.Assembly.Load("Test")).Build();
        Assert.Pass();
        }
    }
}