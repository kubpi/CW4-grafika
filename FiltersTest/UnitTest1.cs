
using CW4_grafika;

namespace FiltersTest
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class Tests : PageTest
    {
        [TestFixture]
        public class FilterTests
        {
            private WriteableBitmap testImage;

            [SetUp]
            public void Setup()
            {
                // Initialize your 10x10 test image here
                testImage = CreateTestImage();
            }

            private WriteableBitmap CreateTestImage()
            {
                // Create and return your 10x10 test image
            }

            private bool CompareImages(WriteableBitmap img1, WriteableBitmap img2)
            {
                // Implement comparison logic
            }

            [Test]
            public void TestSmoothingFilter()
            {
                var filters = new Filters();
                var result = filters.ApplySmoothingFilter(testImage);

                // Expected result should be manually created or known correct output
                var expectedResult = CreateExpectedResultForSmoothing();

                Assert.IsTrue(CompareImages(result, expectedResult), "Smoothing filter output does not match expected result.");
            }

            // Repeat for other filters...
        }

    }
}
