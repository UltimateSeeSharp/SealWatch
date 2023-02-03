using NuGet.Frameworks;
using SealWatch.Code.Services;

namespace SealWatch.UnitTests
{
    public class Tests
    {
        private AnalyseService _analyseService = new();

        [SetUp]
        public void Setup()
        {
        }

        [TestCase("2023, 4, 13")]
        [TestCase("2022, 8, 2")]
        [Test]
        public void DaysLeft(DateTime millingStop)
        {

        }
    }
}