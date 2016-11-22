﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using DbUpgrader.SqlServer;
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbUpgrader.Contracts.Interfaces;
using DbUpgrader.Contracts;
using DbUpgrader.Tests.FakeService;
using System.Reflection;

namespace DbUpgrader.Tests.SqlServer {

	[TestClass]
	public class ScriptFileTests {

		private static SqlServerUpgrader _upgrader;
		private static IDbCleaner _cleaner;
		private static Assembly _fakeServiceAssembly;

		[ClassInitialize]
		public static void Initialize(TestContext context) {
			string connectionString = ConfigurationManager.ConnectionStrings["UnitTestConnectionString"].ConnectionString;
			_cleaner = new TestSqlServerCleaner(connectionString);
			_upgrader = new SqlServerUpgrader(connectionString);
			_fakeServiceAssembly = typeof(MyFakeService).Assembly;
		}

		[TestMethod]
		public void ScriptsStayInOrder() {
			IList<ScriptDocument> documents = _upgrader.GetDocumentsToRun(_fakeServiceAssembly);

			for (int i = 0; i < documents.Count; ++i) {
				Script[] scripts = _upgrader.GetScriptsFromResource(_fakeServiceAssembly, documents[i].ResourceName);
				AssertOrder(
					scripts,
					"Date: 6/21/2016 Order: 0Date: 6/22/2016 Order: 0Date: 6/22/2016 Order: 1Date: 6/23/2016 Order: 0"
				);
			}
		}

		[TestMethod]
		public void UpgraderCanFindSqlScriptFile() {
			IList<ScriptDocument> documents = _upgrader.GetDocumentsToRun(_fakeServiceAssembly);
			for (int i = 0; i < documents.Count; ++i) {
				IList<Script> scriptsToRun = _upgrader.GetScriptsToRun(_fakeServiceAssembly, documents[i]);
				Assert.IsTrue(scriptsToRun.Count >= 0);
			}
		}

		[TestMethod]
		public void UpgraderCanFindSqlDocuments() {
			IList<ScriptDocument> documents = _upgrader.GetDocumentsToRun(_fakeServiceAssembly);
			Assert.IsTrue(documents.Count >= 0, "Unable to find any SQL documents for upgrader.");
		}

		private void AssertOrder(Script[] scripts, string expectedOrder) {
			string actualOrder = String.Empty;
			foreach (Script script in scripts) {
				actualOrder += $"Date: {script.DateCreatedUtc.ToShortDateString()} Order: {script.Order}";
			}

			Assert.AreEqual(expectedOrder, actualOrder);
		}

	}

}
