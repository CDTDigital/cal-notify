using CalNotifyApi;
using Xunit;

namespace Tests.Integration
{
    [CollectionDefinition(TestCollection.Name)]
    public class TestCollection : ICollectionFixture<StartupFixture<Startup>>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.

        public const string Name = "Global Collection";
    }


   
}
