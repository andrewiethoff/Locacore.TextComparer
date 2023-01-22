# Text Comparer package

## Project Description

.NET Standard 2.1 package for comparing two texts similar to Diff 

### Overview
This package allows you to compare two texts and it will return segments 
indicating whether the second text has been deleted, added, changed or 
remained the same. The algorithm tries to find the optimal representation 
for the differences.

The API has two different modes to do this. The first mode divides the 
segments according to the differences, independent of text lines. The 
first mode can be used e.g. for diffs of changes in order to get as few 
segments as possible.
The second mode works on a line-by-line basis and is more suitable for
a comparative display.

### Installation

The package can be obtained via NuGet and is available in the latest version.

### Sample application

There is a sample Web application that accepts two texts and will show a 
representation of the differences between the two texts. The application 
uses ASP.NET core for the backend and a Vue.js frontend.

You can try it [here](https://www.locacore.com).

### Usage Examples

#### 1. Independent of text lines

```c#
  TextComparer textComparerObject = new TextComparer();

  textComparerObject.TextComparerConfiguration.MinimumSizeForRangesOfDifferentText = 3;
  textComparerObject.TextComparerConfiguration.ExpandRangesOfDifferentTextToWordBoundaries = true;
    
  ITextComparer textComparer = textComparerObject;
  List<ComparisonResult> comparisonResults = textComparer.CompareTexts(
    "Lorem ipsum dolor sit amet,\nconsetetur sadipscing elitr,\nsed diam eirmod tempor invidunt ut labore\net colera magna aliquyam.",
    "Lorem ipsumee dolor sit amet,\n  consetur sadipscing elitr,\n  sed diam nonumy eirmod tempor invidunt ut labore et dolore\n  magna aliquyam.");
```

The result of this comparison is a list of segments, each segment containing a 
fragment of the first and second text and a ComparisonType, which indicates whether 
the two texts in a segment are identical (Equals), different (Different) or 
missing or added in one or the other text (Addition or Deletion).

The enumeration for the comparison result looks like this:
```c#
public enum ComparisonResultType
{
    Equals,    /* 0 */
    Different, /* 1 */
    Deletion,  /* 2 */
    Addition   /* 3 */
}
```

The result of the comparison serialised as JSON:

```json
[
  {
    "ComparisonType": 0,    /* Equals */
    "Text1": "Lorem ipsum",
    "Text2": "Lorem ipsum"
  },
  {
    "ComparisonType": 3,    /* Addition */
    "Text1": "",
    "Text2": "ee"
  },
  {
    "ComparisonType": 0,    /* Equals */
    "Text1": " dolor sit amet,\n",
    "Text2": " dolor sit amet,\n"
  },
  {
    "ComparisonType": 3,    /* Addition */
    "Text1": "",
    "Text2": "  "
  },
  {
    "ComparisonType": 0,    /* Equals */
    "Text1": "conset",
    "Text2": "conset"
  },
  {
    "ComparisonType": 2,    /* Deletion */
    "Text1": "et",
    "Text2": ""
  },
  {
    "ComparisonType": 0,    /* Equals */
    "Text1": "ur sadipscing elitr,\n",
    "Text2": "ur sadipscing elitr,\n"
  },
  {
    "ComparisonType": 3,    /* Addition */
    "Text1": "",
    "Text2": "  "
  },
  {
    "ComparisonType": 0,    /* Equals */
    "Text1": "sed diam",
    "Text2": "sed diam"
  },
  {
    "ComparisonType": 3,    /* Addition */
    "Text1": "",
    "Text2": " nonumy"
  },
  {
    "ComparisonType": 0,    /* Equals */
    "Text1": " eirmod tempor invidunt ut labore",
    "Text2": " eirmod tempor invidunt ut labore"
  },
  {
    "ComparisonType": 1,    /* Different */
    "Text1": "\n",
    "Text2": " "
  },
  {
    "ComparisonType": 0,    /* Equals */
    "Text1": "et ",
    "Text2": "et "
  },
  {
    "ComparisonType": 1,    /* Different */
    "Text1": "colera",
    "Text2": "dolore\n "
  },
  {
    "ComparisonType": 0,    /* Equals */
    "Text1": " magna aliquyam.",
    "Text2": " magna aliquyam."
  }
]
```

#### 2. Line dependent comparison

```c#
  LineBasedTextComparer lineBasedTextComparerObject = new LineBasedTextComparer();

  lineBasedTextComparerObject.TextComparerConfiguration.MinimumSizeForRangesOfDifferentText = 3;
  lineBasedTextComparerObject.TextComparerConfiguration.ExpandRangesOfDifferentTextToWordBoundaries = true;

  ILineBasedTextComparer lineBasedTextComparer = lineBasedTextComparerObject;
  List<LineBasedComparisonResult> lineBasedComparisonResult = lineBasedTextComparer.CompareTextsLineBased(
    "Lorem ipsum dolor sit amet,\nconsetetur sadipscing elitr,\nsed diam eirmod tempor invidunt ut labore\net colera magna aliquyam.",
    "Lorem ipsumee dolor sit amet,\n  consetur sadipscing elitr,\n  sed diam nonumy eirmod tempor invidunt ut labore et dolore\n  magna aliquyam.");
```

The result of this comparison was divided into segments and separated at the 
row level, making it relatively easy to visualise the result 
(see sample application).

Since many frontend frameworks require a unique key for objects, this unique key 
is also (randomly) generated and returned in the result object in the property 
*Key*.

The following example shows the structure of the result. A list of objects, 
each containing *LineOfText1* and *LineOfText2*, which in turn have a 
list of segments within a line (but here only separated into text 1 and text 2).
In addition, the row object also contains the corresponding line number of 
the respective text.

Similar to the other mode, each segment contains a *ComparisonType*, 
which contains the type of comparison result for this segment. 
In addition to this, each line has an aggregated type, which depends on the
occurred comparison results. It is either *Different* if at least one 
different fragment is present in that row, *Addition* if the text is 
new or *Equals* if both texts match.

```json
[
  {
    "LineOfText1": {
      "LineNumber": 1,
      "AggregatedResultType": "Equals",
      "LineTexts": [
        {
          "Text": "Lorem ipsum",
          "ComparisonType": "Equals",
          "Key": "4270f0e554ea4cb6b9d1906bf8e04d69"
        },
        {
          "Text": " dolor sit amet,",
          "ComparisonType": "Equals",
          "Key": "18512cb278c446c9983ecedbbcb6cd7a"
        }
      ],
      "Key": "e0f413d133cb4c0cbbc332d778b24e5f"
    },
    "LineOfText2": {
      "LineNumber": 1,
      "AggregatedResultType": "Addition",
      "LineTexts": [
        {
          "Text": "Lorem ipsum",
          "ComparisonType": "Equals",
          "Key": "6361897d192449fa8aa89d5938d14702"
        },
        {
          "Text": "ee",
          "ComparisonType": "Addition",
          "Key": "c2a231f2d3114ee2b29fe28e80557ec1"
        },
        {
          "Text": " dolor sit amet,",
          "ComparisonType": "Equals",
          "Key": "c35ce225b5a1411b827c11a05bca7e66"
        }
      ],
      "Key": "f97ac62fb12b429db8f4187c88ec282d"
    },
    "Key": "56987538df0048998c16e3202a5bebea"
  },
  {
    "LineOfText1": {
      "LineNumber": 2,
      "AggregatedResultType": "Addition",
      "LineTexts": [
        {
          "Text": "",
          "ComparisonType": "Equals",
          "Key": "c35abbefd1384f39b6ee06b57f42b971"
        },
        {
          "Text": "conset",
          "ComparisonType": "Equals",
          "Key": "fb68fb3d35d94c409461e2b69b13dc6a"
        },
        {
          "Text": "et",
          "ComparisonType": "Addition",
          "Key": "0fa44ca898e6477a9f8c24a1562e2dbb"
        },
        {
          "Text": "ur sadipscing elitr,",
          "ComparisonType": "Equals",
          "Key": "e5baea1327ea4b8d96610d71f008b71f"
        }
      ],
      "Key": "8c1857f1bf58428faf173763cf0cc9f5"
    },
    "LineOfText2": {
      "LineNumber": 2,
      "AggregatedResultType": "Addition",
      "LineTexts": [
        {
          "Text": "",
          "ComparisonType": "Equals",
          "Key": "4be2f47a3f7f40c483f75cbb97944a12"
        },
        {
          "Text": "  ",
          "ComparisonType": "Addition",
          "Key": "eb64e523c5604a60818bcca16202f0d0"
        },
        {
          "Text": "conset",
          "ComparisonType": "Equals",
          "Key": "877d69fdc3a247b2bd081258c8c6134d"
        },
        {
          "Text": "",
          "ComparisonType": "Deletion",
          "Key": "5036096dbde0460d986eca8e3381b459"
        },
        {
          "Text": "ur sadipscing elitr,",
          "ComparisonType": "Equals",
          "Key": "d2f0ae96cf0d40bcbdad8dcaa036a32e"
        }
      ],
      "Key": "2faccb0f2d6142ab87c8a9236a32782c"
    },
    "Key": "65c92079f37a4a068c07ed9e619e9eeb"
  },
  {
    "LineOfText1": {
      "LineNumber": 3,
      "AggregatedResultType": "Equals",
      "LineTexts": [
        {
          "Text": "",
          "ComparisonType": "Equals",
          "Key": "efee13a2f21f4028af47ef65975e0295"
        },
        {
          "Text": "sed diam",
          "ComparisonType": "Equals",
          "Key": "60f475e071a34b2199dd6ece0de43d72"
        },
        {
          "Text": " eirmod tempor invidunt ut labore",
          "ComparisonType": "Equals",
          "Key": "8825fcd8327c4aceaac77a7085c96baa"
        },
        {
          "Text": "",
          "ComparisonType": "Different",
          "Key": "c15d551f430f4ab7abb05041d597b742"
        }
      ],
      "Key": "28448a38af0b4eadb5a7994a71ec5330"
    },
    "LineOfText2": {
      "LineNumber": 3,
      "AggregatedResultType": "Different",
      "LineTexts": [
        {
          "Text": "",
          "ComparisonType": "Equals",
          "Key": "20b9e676743843f99902bc1c15557591"
        },
        {
          "Text": "  ",
          "ComparisonType": "Addition",
          "Key": "625d1c2d7afa4cfabf1bb90e3f83ddce"
        },
        {
          "Text": "sed diam",
          "ComparisonType": "Equals",
          "Key": "cc9ee3fbd0d54508a67e1489af07a41d"
        },
        {
          "Text": " nonumy",
          "ComparisonType": "Addition",
          "Key": "ba17719da09c4e4cae1dcda4f1b24224"
        },
        {
          "Text": " eirmod tempor invidunt ut labore",
          "ComparisonType": "Equals",
          "Key": "a39b15ddb2da40dbb04109479552b72a"
        },
        {
          "Text": " ",
          "ComparisonType": "Addition",
          "Key": "8dbd8ddb31664685a5802fa16dfb8764"
        },
        {
          "Text": "et ",
          "ComparisonType": "Equals",
          "Key": "23309f6248834157a3eb9e4cd5f57eec"
        },
        {
          "Text": "dolore",
          "ComparisonType": "Different",
          "Key": "d64860a2058c4508a87553d6b67a0235"
        }
      ],
      "Key": "89fe46f924024252a578fca91f1f70c6"
    },
    "Key": "90d1974563be43d1a9d6dbe0f904d08c"
  },
  {
    "LineOfText1": {
      "LineNumber": 4,
      "AggregatedResultType": "Different",
      "LineTexts": [
        {
          "Text": "",
          "ComparisonType": "Different",
          "Key": "83c5e288f7ae4a008125ce3ede2efbb3"
        },
        {
          "Text": "et ",
          "ComparisonType": "Equals",
          "Key": "7685798ccd67436188d805b649511206"
        },
        {
          "Text": "colera",
          "ComparisonType": "Different",
          "Key": "09e600398aba42a3805cf185d2096feb"
        },
        {
          "Text": " magna aliquyam.",
          "ComparisonType": "Equals",
          "Key": "b316b2d54f87482fbb446bcdfd4fe35a"
        }
      ],
      "Key": "00fc39e114634fd5a6c3e37924f5d86f"
    },
    "LineOfText2": {
      "LineNumber": 4,
      "AggregatedResultType": "Different",
      "LineTexts": [
        {
          "Text": " ",
          "ComparisonType": "Different",
          "Key": "7b2d58dc995347a1b4921630d48cc465"
        },
        {
          "Text": " magna aliquyam.",
          "ComparisonType": "Equals",
          "Key": "cf332843d28c4009a68e08464589bd04"
        }
      ],
      "Key": "508e38d8c1b844c88397054cce04cff6"
    },
    "Key": "922f0b930337443f8e2b9dd6017e1f94"
  }
]
```

### Comparer configuration

The two comparators can be configured via two parameters.

Setting | Default Value | Description
------- | ------------- | -----------
MinimumSizeForRangesOfDifferentText | 3 | Sometimes there are several differences nearby. E.g., if the two texts "Lorem *ipsum* dolor" and "Lorem *insom* dolor" are compared, the result could be that either *p* and *u* are marked as different (different to *n* and *o*), or (as both are within the minimum size) *psu* will be detected as different (different to *nso*). Two nearby segments of different texts will be merged when within the minimum size.
ExpandRangesOfDifferentTextToWordBoundaries | true | Setting this flag will expand found differences to match word boundaries. For example, if the two texts "Lorem *ipsum* dolor" and "Lorem *ipsem* dolor" are compared, the result will be either that *ipsem* is marked as different (true) or *e* (false)

### Performance

On an Xeon E5-1650 @ 3.20 GHz introduced in 2012, these are the times in seconds for comparing texts with the given
number of characters and the number of differences within the text (the more differences there are, the longer the comparison will take)

Length / #Differences   | 2.000 | 1.000 | 500 | 250 | 125 | 64 | 32 | 16
-------------------|-------|-------|-----|-----|-----|----|----|---
**1.000.000**      | 11.4   | 9.3   | 9.1 | 7.3 | 6.2 |5.5 |4.8 |5.6
**500.000**        | 4.6   | 4.1   | 4.6 | 3.8 |3.2  |2.8 |2.4 |2.4
**250.000**        |2.0    | 1.8   | 2.1 |1.8  |1.5  |1.3 |1.2 |1.2
**120.000**        |1.2    |0.8    |0.8  |0.7  |0.7  |0.6 |0.5 |0.5
**60.000**         |0.9    |0.5    |0.4  |0.3  | 0.3 |0.3 |0.2 |0.2
**30.000**         |0.6    | 0.4   |0.2  |0.2  | 0.1 |0.1 |0.1 |0.1

For example, comparing source code between versions of typical size is usually 
completed within 0.1 - 0.2 seconds on this quite old machine.

### Contributions

You are welcome to branch this project and add improvements or bugfixes to be merged into the main branch.