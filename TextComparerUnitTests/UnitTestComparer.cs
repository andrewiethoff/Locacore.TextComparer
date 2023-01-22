using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Locacore.TextComparer;
using TextComparerUnitTests.ComparisonTests;
using Xunit;

namespace TextComparerUnitTests
{
    public class UnitTestComparer
    {
        [Theory]
        [ClassData(typeof(TextComparerTestClassData))]
        public void TextComparer(TextComparerTestData testData)
        {
            var textComparer = new TextComparer();
            textComparer.TextComparerConfiguration.MinimumSizeForRangesOfDifferentText = testData.MinimumRange;
            var result = textComparer.CompareTexts(testData.Text1, testData.Text2);

            CheckComparerLoss(testData.Text1, testData.Text2, result);
            CheckSegments(result);
            CheckComparisonResult(result, testData.ExpectedComparisonResult);
        }

        private void CheckComparisonResult(List<ComparisonResult> result, List<ComparisonResult> expectedResult)
        {
            Assert.True(result.Count == expectedResult.Count);

            for (var index = 0; index < result.Count; index++)
            {
                Assert.True(result[index].ComparisonType == expectedResult[index].ComparisonType);
                Assert.True(result[index].Text1 == expectedResult[index].Text1);
                Assert.True(result[index].Text2 == expectedResult[index].Text2);
            }
        }

        private void CheckComparerLoss(string text1, string text2, List<ComparisonResult> result)
        {
            var text1Result = string.Join("", result.Select(x => x.Text1));
            var text2Result = string.Join("", result.Select(x => x.Text2));

            Assert.Equal(text1Result, text1);
            Assert.Equal(text2Result, text2);
        }

        private void CheckSegments(List<ComparisonResult> result)
        {
            foreach (var segment in result)
            {
                switch (segment.ComparisonType)
                {
                    case ComparisonResultType.Addition:
                        Assert.True(string.IsNullOrEmpty(segment.Text1));
                        Assert.False(string.IsNullOrEmpty(segment.Text2));
                        break;
                    case ComparisonResultType.Deletion:
                        Assert.False(string.IsNullOrEmpty(segment.Text1));
                        Assert.True(string.IsNullOrEmpty(segment.Text2));
                        break;
                    case ComparisonResultType.Different:
                        Assert.False(string.IsNullOrEmpty(segment.Text1));
                        Assert.False(string.IsNullOrEmpty(segment.Text2));
                        Assert.False(segment.Text1 == segment.Text2);
                        break;
                    case ComparisonResultType.Equals:
                        Assert.False(string.IsNullOrEmpty(segment.Text1));
                        Assert.False(string.IsNullOrEmpty(segment.Text2));
                        Assert.True(segment.Text1 == segment.Text2);
                        break;
                }
            }
        }
    }

    public class TextComparerTestClassData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                ComparisonTest1.TestData()
            };
            yield return new object[]
            {
                ComparisonTest2.TestData()
            };
            yield return new object[]
            {
                ComparisonTest3.TestData()
            };
            yield return new object[]
            {
                ComparisonTest4.TestData()
            };
            yield return new object[]
            {
                ComparisonTest5.TestData()
            };
            yield return new object[]
            {
                ComparisonTest6.TestData()
            };
            yield return new object[]
            {
                ComparisonTest7.TestData()
            };
            yield return new object[]
            {
                ComparisonTest8.TestData()
            };
            yield return new object[]
            {
                ComparisonTest9.TestData()
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}