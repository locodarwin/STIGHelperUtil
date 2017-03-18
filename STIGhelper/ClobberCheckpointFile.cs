using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace STIGhelper
{
    public class ClobberCheckpointFile
    {
		private enum DocumenElements { DontCare, RuleId, result};
        public string fileName
        { get; set; }

		public void clobber(Dictionary<string, string> results, bool overRideAllSettings)
		{ // http://www.codeproject.com/Articles/9494/Manipulate-XML-data-with-XPath-and-XmlDocument-C
			XmlTextReader reader = new XmlTextReader(fileName);
			XmlDocument doc = new XmlDocument();
			doc.Load(reader);
			reader.Close();

			foreach (KeyValuePair<string, string> entry in results)
			{
				// grab the node
				XmlNode oldVuln;
				XmlElement root = doc.DocumentElement;
				string nodeSearchString = string.Format("/CHECKLIST/VULN[STIG_DATA/ATTRIBUTE_DATA='{0}']", entry.Key.Replace("#", ""));
				oldVuln = root.SelectSingleNode(nodeSearchString);
				string originalInnerXml = oldVuln.InnerXml;

				// override settings if desired
				if (overRideAllSettings)
				{
					originalInnerXml = originalInnerXml.Replace("<STATUS>NotAFinding</STATUS>", "<STATUS>Not_Reviewed</STATUS>");
					originalInnerXml = originalInnerXml.Replace("<STATUS>Not_Applicable</STATUS>", "<STATUS>Not_Reviewed</STATUS>");
					originalInnerXml = originalInnerXml.Replace("<STATUS>Open</STATUS>", "<STATUS>Not_Reviewed</STATUS>");
				}

				// get replacement string
				string newStatus = entry.Value == "pass" ? "NotAFinding" : "Open";
				string replacementStatus = string.Format("<STATUS>{0}</STATUS>", newStatus);
				string newInnerXML = originalInnerXml.Replace("<STATUS>Not_Reviewed</STATUS>", replacementStatus);

				// add a comment to the STIG if there is no comment already
				if (originalInnerXml != newInnerXML)
				{
					newInnerXML = newInnerXML.Replace("<COMMENTS></COMMENTS>", 
						"<COMMENTS>Status field auto populated by EID tool based on results of benchmark test.</COMMENTS>");
				}
				
				// create new node and replace
				XmlElement newVuln = doc.CreateElement("VULN");
				newVuln.InnerXml = newInnerXML;
				root.ReplaceChild(newVuln, oldVuln);
			}

			doc.Save(fileName);
		}

    }
}
