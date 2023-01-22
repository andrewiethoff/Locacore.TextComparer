using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Locacore.TextComparer;
using TextComparerUnitTests.LineBaseComparisonTests;
using Xunit;

namespace TextComparerUnitTests
{
    public class UnitTestLineBasedComparer
    {
        [Theory]
        [ClassData(typeof(LineBasedTextComparerTestClassData))]
        public void LineBasedTextComparer(LineBasedTextComparerTestData testData)
        {
            var lineBasedTextComparer = new LineBasedTextComparer();
            lineBasedTextComparer.TextComparerConfiguration.MinimumSizeForRangesOfDifferentText = testData.MinimumRange;
            var compareResult = lineBasedTextComparer.CompareTextsLineBased(testData.Text1, testData.Text2);

            CheckComparerLossLineBased(testData.Text1, testData.Text2, compareResult);
            CheckLineBasedComparisonResult(compareResult, testData.ExpectedLineBasedComparisonResult);
        }

        private void CheckLineBasedComparisonResult(
            List<LineBasedComparisonResult> result,
            List<LineBasedComparisonResult> expectedResult)
        {
            Assert.True(result.Count == expectedResult.Count);

            for (var lineIndex = 0; lineIndex < result.Count; lineIndex++)
            {
                Assert.True(result[lineIndex].LineOfText1?.LineTexts?.Length ==
                            expectedResult[lineIndex].LineOfText1?.LineTexts?.Length);
                Assert.True(result[lineIndex].LineOfText2?.LineTexts?.Length ==
                            expectedResult[lineIndex].LineOfText2?.LineTexts?.Length);

                for (var segment1Index = 0;
                    segment1Index < (result[lineIndex].LineOfText1?.LineTexts?.Length ?? 0);
                    segment1Index++)
                {
                    Assert.True(result[lineIndex].LineOfText1.LineTexts[segment1Index].ComparisonType ==
                                expectedResult[lineIndex].LineOfText1.LineTexts[segment1Index].ComparisonType);
                    Assert.True(result[lineIndex].LineOfText1.LineTexts[segment1Index].Text ==
                                expectedResult[lineIndex].LineOfText1.LineTexts[segment1Index].Text);
                }

                Assert.True(
                    result[lineIndex].LineOfText1?.LineNumber == expectedResult[lineIndex].LineOfText1?.LineNumber);
                Assert.True(result[lineIndex].LineOfText1?.AggregatedResultType ==
                            expectedResult[lineIndex].LineOfText1?.AggregatedResultType);

                for (var segment2Index = 0;
                    segment2Index < (result[lineIndex].LineOfText2?.LineTexts?.Length ?? 0);
                    segment2Index++)
                {
                    Assert.True(result[lineIndex].LineOfText2?.LineTexts?[segment2Index].ComparisonType ==
                                expectedResult[lineIndex].LineOfText2?.LineTexts?[segment2Index].ComparisonType);
                    Assert.True(result[lineIndex].LineOfText2?.LineTexts?[segment2Index].Text ==
                                expectedResult[lineIndex].LineOfText2?.LineTexts?[segment2Index].Text);
                }

                Assert.True(
                    result[lineIndex]?.LineOfText2?.LineNumber == expectedResult[lineIndex]?.LineOfText2?.LineNumber);
                Assert.True(result[lineIndex].LineOfText2?.AggregatedResultType ==
                            expectedResult[lineIndex].LineOfText2?.AggregatedResultType);
            }
        }

        private string AssembleResultToString(
            List<LineBasedComparisonResult> result,
            Func<LineBasedComparisonResult, LineInformation> accessor)
        {
            var assembledResult = "";
            int currentLine = 1;
            foreach (var a in result)
            {
                var z = accessor(a);
                if (z != null)
                {
                    while (currentLine < z.LineNumber)
                    {
                        assembledResult += "\n";
                        currentLine++;
                    }

                    assembledResult += string.Join("", z.LineTexts.Select(x => x.Text));
                }
            }

            return assembledResult;
        }

        private void CheckComparerLossLineBased(
            string text1, 
            string text2, 
            List<LineBasedComparisonResult> result)
        {
            var text1result = AssembleResultToString(result, x => x.LineOfText1);
            var text2result = AssembleResultToString(result, x => x.LineOfText2);

            Assert.Equal(text1result, text1.Replace("\r", ""));
            Assert.Equal(text2result, text2.Replace("\r", ""));
        }
    }

    public class LineBasedTextComparerTestClassData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                LineBasedComparisonTest1.TestData()
            };
            yield return new object[]
            {
                LineBasedComparisonTest2.TestData()
            };
            yield return new object[]
            {
                LineBasedComparisonTest3.TestData()
            };
            yield return new object[]
            {
                LineBasedComparisonTest4.TestData()
            };
            yield return new object[]
            {
                LineBasedComparisonTest5.TestData()
            };
            yield return new object[]
            {
                LineBasedComparisonTest6.TestData()
            };
            yield return new object[]
            {
                LineBasedComparisonTest7.TestData()
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}