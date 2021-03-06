using Executioner.Contracts;
using Executioner.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Executioner.Tests.Classes {

	public class BaseMockLoader : IScriptLoader {
		private string[] _scriptElements;
		private DocumentSerializer _serializer;

		public BaseMockLoader() {
			_scriptElements = new string[0];
			this.Documents = new List<ScriptDocument>();
			_serializer = new DocumentSerializer();
		}

		public BaseMockLoader(string[] scriptElements) {
			_scriptElements = scriptElements;
			this.Documents = new List<ScriptDocument>();
			_serializer = new DocumentSerializer();
		}

		public IList<ScriptDocument> Documents { get; set; }

		public void LoadDocuments() {
			if (this.Documents.Count > 0) {
				return;
			}
			string doc = GetXmlDoc();
			if (doc == null) {
				return;
			}

			ScriptDocument sDoc = _serializer.Deserialize<ScriptDocument>(doc);
			Guid docId = sDoc.SysId;
			sDoc.Scripts = sDoc.Scripts.Select(x => { x.DocumentId = docId; return x; }).ToList();

			this.Documents = new List<ScriptDocument>() { sDoc };
		}

		public void Add(Script script) {
			ScriptDocument doc = this.Documents
				.Where(x => x.SysId == script.DocumentId)
				.SingleOrDefault();

			if (doc == null) {
				return;
			}
			int index = doc.Scripts.FindIndex(x => x.SysId == script.SysId);
			if (index == -1) {
				doc.Scripts.Add(script);
			}
			else {
				doc.Scripts[index] = script;
			}
		}

		private string GetXmlDoc() {
			string scriptStr = String.Empty;
			foreach (string item in _scriptElements) {
				scriptStr += $"{item}\n";
			}

			if (String.IsNullOrEmpty(scriptStr)) {
				return null;
			}

			string xmlStr = $@"<?xml version='1.0' encoding='utf-8'?>
				<ScriptDocument>
					<Id>ac04f1b3-219a-4a40-8d7d-869dac218cca</Id>
					<Order>2016-06-21</Order>
					<Scripts>
						{scriptStr}	
					</Scripts>
				</ScriptDocument>";

			return xmlStr;
		}

	}

}