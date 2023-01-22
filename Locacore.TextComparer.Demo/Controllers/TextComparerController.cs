using System;
using System.Collections.Generic;
using Locacore.TextComparer;
using Microsoft.AspNetCore.Mvc;

namespace TextComparerDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TextComparerController : ControllerBase
    {
        private ILineBasedTextComparer LineBasedTextComparer { get; set; }

        public TextComparerController(ILineBasedTextComparer textComparer)
        {
            this.LineBasedTextComparer = textComparer;
        }

        private string CreateWord(int length, int seed)
        {
            var r = new Random(seed);
            string result = "";
            for (int index = 0; index < length; index++)
            {
                var character = (char)(97+r.Next(26));
                result += character;
            }
            return result;
        }

        private string Remove(string a, int num)
        {
            var r = new Random(9998);
            for (int t = 0; t < num; t++)
            {
                var rl = 3 + r.Next(12);
                var pos = r.Next(a.Length - rl);
                a = a.Substring(0, pos) + a.Substring(pos + rl);
            }
            return a;
        }

        private string Add(string a, int num)
        {
            var r = new Random(23);
            for (int t = 0; t < num; t++)
            {
                var rl = 3 + r.Next(12);
                var pos = r.Next(a.Length - rl);
                var posfrom = r.Next(a.Length - rl);
                a = a.Substring(0, pos) + a.Substring(posfrom,rl)+a.Substring(pos);
            }
            return a;
        }

        private string Change(string a, int num)
        {
            var r = new Random(678);
            for (int t = 0; t < num; t++)
            {
                var rl = 5 + r.Next(10);
                var pos = r.Next(a.Length - rl);
                var aa = a.Substring(pos, rl).ToCharArray();
                Array.Reverse(aa);
                a = a.Substring(0, pos) + (new string(aa)) + a.Substring(pos+rl);
            }
            return a;
        }

        private string AddLineBreaks(string a, int num)
        {
            var r = new Random(544);
            for (int t = 0; t < num; t++)
            {
                var pos = r.Next(a.Length-1);
                a = a.Substring(0, pos) + Environment.NewLine + a.Substring(pos);
            }
            return a;
        }

        [HttpPost]
        public ActionResult<List<LineBasedComparisonResult>> Post([FromBody] OriginalTexts value)
        {
            if ((value.Text1.Length > 1_000_000) || (value.Text2.Length > 1_000_000))
                return BadRequest("Texts are too large");
            
            LineBasedTextComparer lineBasedTextComparerObject = new LineBasedTextComparer();
            lineBasedTextComparerObject.TextComparerConfiguration.MinimumSizeForRangesOfDifferentText = 3;
            ILineBasedTextComparer lineBasedTextComparer = lineBasedTextComparerObject;
            
            return this.LineBasedTextComparer.CompareTextsLineBased(value.Text1, value.Text2);
        }

        public class OriginalTexts
        {
            public string Text1 { get; set; }
            public string Text2 { get; set; }
        }
    }
}