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
        public void DaysLeftOne(DateTime millingStop)
        {
            int days = _analyseService.CalcDaysLeft(millingStop);

            int actual = (int)Math.Ceiling((millingStop - DateTime.Now).TotalDays);
            Assert.That(days, Is.EqualTo(actual));
        }
    }
}