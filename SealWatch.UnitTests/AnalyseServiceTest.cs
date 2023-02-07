using SealWatch.Code.CutterLayer;
using SealWatch.Code.Services;
using System;

namespace SealWatch.UnitTests
{
    public class Tests
    {
        private AnalyseService _analyseService = new();
        private DateTime _testDateNow = new(2022, 6, 4);

        [SetUp]
        public void Setup()
        {

        }

        [TestCase("2023, 4, 13", ExpectedResult = 313)]
        [TestCase("2022, 8, 2", ExpectedResult = 59)]
        [TestCase("2021, 3, 7", ExpectedResult = -454)]
        public double DaysLeftTest(DateTime millingStop)
        {
            double days = (millingStop - _testDateNow).TotalDays;
            double result = _analyseService.CalcRelativeTimeInDays(millingStop, _testDateNow);

            return result;
        }

        [TestCase("2023, 4, 13")]
        [TestCase("2022, 8, 2")]
        [TestCase("2021, 3, 7")]
        public void DaysLeft(DateTime millingStop)
        {
            double testResult = DaysLeftTest(millingStop);
            double funcResult = _analyseService.CalcRelativeTimeInDays(millingStop, _testDateNow);

            Assert.That(testResult, Is.EqualTo(funcResult));
        }



        [TestCase("2022, 2, 2", "2022, 4, 3", 0, ExpectedResult = 203)]
        [TestCase("2022, 2, 2", "2023, 4, 3", 0, ExpectedResult = 29)]
        [TestCase("2022, 2, 2", "2023, 2, 2", 0, ExpectedResult = 33)]
        [TestCase("2022, 2, 16", "2023, 6, 26", 0, ExpectedResult = 22)]
        [TestCase("2022, 2, 16", "2022, 6, 4", 0, ExpectedResult = 100)]
        public double DurabilityTest(DateTime millingStart, DateTime millingstop, int accuracy = 0)
        {
            double daysTotal = (millingstop - millingStart).TotalDays;
            double daysToWork = (millingstop - _testDateNow).TotalDays;
            double daysWorked = daysTotal - daysToWork;
            double workedFraction = daysWorked / (daysTotal / 100);

            double result = Math.Round(workedFraction, accuracy);
            return result;
        }

        [TestCase("2022, 2, 2", "2022, 4, 3", 0)]
        [TestCase("2022, 2, 2", "2023, 4, 3", 0)]
        [TestCase("2022, 2, 2", "2023, 2, 2", 0)]
        [TestCase("2022, 2, 16", "2023, 6, 26", 0)]
        [TestCase("2022, 2, 16", "2022, 6, 4", 0)]
        public void Durability(DateTime millingStart, DateTime millingstop, int accuracy = 0)
        {
            double testResult = DurabilityTest(millingStart, millingstop);
            double funcResult = _analyseService.CalcDurability(millingStart, millingstop, _testDateNow);

            Assert.That(testResult, Is.EqualTo(funcResult));
        }



        [TestCase("2023, 2, 7", 5, 8, 600, ExpectedResult = "2023, 5, 21")]
        public DateTime FailureDateTest(DateTime millingStart, int workDays, double millingPerDay, double lifespan)
        {
            DateTime start = millingStart;

            while (lifespan > 0 && lifespan > millingPerDay * workDays)
            {
                lifespan -= (millingPerDay * workDays);
                start = start.AddDays(7);
            }

            if (lifespan is not 0)
            {
                var left = lifespan / millingPerDay;
                start = start.AddDays(left);
            }

            return start;
        }

        [TestCase("2023, 2, 7", 5, 8, 600)]
        public void FailureDate(DateTime millingStart, int workDays, double millingPerDay, double lifespan)
        {
            DateTime testResult = FailureDateTest(millingStart, workDays, millingPerDay, lifespan);
            DateTime funcResult = _analyseService.CalcFailureDate(millingStart, workDays, millingPerDay, lifespan);

            Assert.That(testResult, Is.EqualTo(funcResult));
        }


        public List<DateTime> GetFailureDatesTest(AnalysedCutterDto cutter)
        {
            DateTime millingStopTotal = cutter.MillingStart.AddMonths((int)(12 * cutter.MillingDuration_y));
            List<DateTime> failureDates = new();

            do
            {
                DateTime failureDate = FailureDateTest(failureDates.Count == 0 ? cutter.MillingStart : failureDates.Last(), cutter.WorkDays, cutter.MillingPerDay_h, cutter.LifeSpan_h);
                failureDates.Add(failureDate);
            }
            while (millingStopTotal > failureDates.Last());

            return failureDates;
        }

        [Test]
        public void GetFailureDates()
        {
            AnalysedCutterDto cutter = new()
            {
                MillingStart = _testDateNow,
                MillingDuration_y = 1.5,
                WorkDays = 5,
                MillingPerDay_h = 9,
                LifeSpan_h = 600
            };

            List<DateTime> testResult = GetFailureDatesTest(cutter);
            List<DateTime> funcResult = _analyseService.GetFailureDates(cutter);

            Assert.That(testResult, Is.EqualTo(funcResult));
        }
    }
}   