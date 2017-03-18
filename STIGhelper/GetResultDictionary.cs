using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STIGhelper
{
    public class GetResultDictionary
    {
        public Dictionary<string, string> results
        { get; set; }

        string _ruleString = "href=";
        string _classString = "<li class=";
        string _passString = "pass";
        string _failString = "fail";
        int _tooDeepInLine = 20;

        public int LoadDictionary(string filename)
        {
            results = new Dictionary<string, string>();
            int numPassed = 0;
            string line;
            StreamReader file = new StreamReader(filename);
            while ((line = file.ReadLine()) != null)
            {
                if ((line.IndexOf(_ruleString) >= 0) && 
                    (line.IndexOf(_classString) >= 0))
                {
                    int passOffset = line.IndexOf(_passString);
                    int failOffset = line.IndexOf(_failString);
                    if ((passOffset > 0) && (passOffset < _tooDeepInLine))
                    {
                        results.Add(GetTestID(line), "pass");
                        numPassed++;
                    }
                    if ((failOffset > 0) && (failOffset < _tooDeepInLine))
                    {
                        results.Add(GetTestID(line), "fail");
                    }
                    Console.WriteLine(line);
                }
            }
            return numPassed;
        }

        private string GetTestID(string line)
        {
			string testStringStartString = "#SV";
			string testStringEndString = "_rule";
            string testId;
            int testStringStartIndex = line.IndexOf(testStringStartString);
            int testStringEndIndex = line.IndexOf(testStringEndString) + testStringEndString.Length;
            testId = line.Substring(testStringStartIndex, testStringEndIndex - testStringStartIndex);
            return testId;
        }
    }
}
