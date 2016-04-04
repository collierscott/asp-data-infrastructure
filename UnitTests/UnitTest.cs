using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Data;
using Infrastructure.Data.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// TODO More Tests!!! - Scott Collier 04/04/2016
namespace UnitTests
{
    /// <summary>
    /// Summary description for UnitTest2
    /// </summary>
    [TestClass]
    public class UnitTest
    {
        public UnitTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void GenericSqlQuery_ToString()
        {
            var query = new GenericSqlQuery
            {
                Id = "Test",
                Query = "SELECT * FROM some_table WHERE name = :NAME_ID AND hero = :HERO_ID",
                Parameters = new List<QueryParameter>
                {
                    new QueryParameter
                    {
                        ParameterName = "NAME_ID",
                        Value = "Scott"
                    },
                    new QueryParameter
                    {
                        ParameterName = "HERO_ID",
                        Value = "CAPTAIN AMERICA"
                    }
                }
            };

            Assert.AreEqual("SELECT * FROM some_table WHERE name = 'Scott' AND hero = 'CAPTAIN AMERICA'", query.ToString());
        }

        [TestMethod]
        public void AlphaNumComparator_Sort_Alphabetic_Strings()
        {
            var list = new List<string>
            {
                "Scott",
                "Ginger",
                "Iron Man",
                "Captain America",
                "Sooong",
                "Song",
                "Aabbcc",
                "AAbbcc"
            };

            var ordered = list.OrderBy(o => o, new AlphaNumComparator()).ToList();

            var expected = new List<string>
            {
                "AAbbcc",
                "Aabbcc",
                "Captain America",
                "Ginger",
                "Iron Man",
                "Scott",
                "Song",
                "Sooong",
                
            };

            CollectionAssert.AreEqual(expected, ordered);
        }

        [TestMethod]
        public void AlphaNumComparator_Sort_AlphaNumeric_Strings()
        {
            var list = new List<string>
            {
                "100F",
                "50F",
                "SR100",
                "SR9",
                "z1.doc",
                "z10.doc",
                "z100.doc",
                "z101.doc",
                "z102.doc",
                "z11.doc",
                "z12.doc",
                "z13.doc",
                "z14.doc",
                "z15.doc",
                "z16.doc",
                "z17.doc",
                "z19.doc",
                "z2.doc",
                "z20.doc",
                "z3.doc",
                "z4.doc",
                "z5.doc",
                "z6.doc",
                "z7.doc",
                "z8.doc",
                "z9.doc",
                "z18.doc"
            };

            var ordered = list.OrderBy(o => o, new AlphaNumComparator()).ToList();

            var expected = new List<string>
            {
                "50F",
                "100F",
                "SR9",
                "SR100",
                "z1.doc",
                "z2.doc",
                "z3.doc",
                "z4.doc",
                "z5.doc",
                "z6.doc",
                "z7.doc",
                "z8.doc",
                "z9.doc",
                "z10.doc",
                "z11.doc",
                "z12.doc",
                "z13.doc",
                "z14.doc",
                "z15.doc",
                "z16.doc",
                "z17.doc",
                "z18.doc",
                "z19.doc",
                "z20.doc",
                "z100.doc",
                "z101.doc",
                "z102.doc"
            };

            CollectionAssert.AreEqual(expected, ordered);

        }

        [TestMethod]
        public void DataParser_GetBoolean_Is_False()
        {
            Assert.AreEqual(false, DataParser.GetBoolean("N"));
            Assert.AreEqual(false, DataParser.GetBoolean("0"));
            Assert.AreEqual(false, DataParser.GetBoolean("fAlse"));
            Assert.AreEqual(false, DataParser.GetBoolean(string.Empty));
        }

        [TestMethod]
        public void DataParser_GetBoolean_Is_True()
        {
            Assert.AreEqual(true, DataParser.GetBoolean("Y"));
            Assert.AreEqual(true, DataParser.GetBoolean("1"));
            Assert.AreEqual(true, DataParser.GetBoolean("True"));
        }

        [TestMethod]
        public void DataParser_GetDateTime()
        {
            Assert.AreEqual(null, DataParser.GetDateTime("Y"));
            Assert.AreEqual(DateTime.Now, DataParser.GetDateTime("Y", true));
        }
    }
}
