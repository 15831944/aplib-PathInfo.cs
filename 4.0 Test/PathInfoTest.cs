using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Xml.Serialization;using System.Runtime.Serialization;using System.Runtime.Serialization.Formatters.Binary;
using System.Security.AccessControl;
using IOPath = System.IO.Path;

/* Copyright © 2012 Vadim Baklanov (Ad), distributed under the MIT License
 * When copying, use or create derivative works do not remove or modify this attribution, and this license text.*/

namespace _4._0_Test
{
    
    
    /// <summary>
    ///This is a test class for PathInfoPathInfoTest and is intended
    ///to contain all PathInfoPathInfoTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PathInfoTest
    {


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
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for PathInfo Constructor
        ///</summary>
        [TestMethod()]
        public void Constructors()
        {
            // All constructors test

            // Parameterless constructor

            var empty_path = new PathInfo();
            Assert.IsTrue(empty_path.Empty, "Initialize Empty Field");

            // Create from string path

            var temp_path_string = IOPath.GetTempPath();
            var temp_path = new PathInfo(temp_path_string);
            Assert.AreEqual(temp_path.Path, temp_path_string, "Create from file system path string");
            Assert.AreEqual(temp_path.FullPath, temp_path_string, "Create from file system path string");

            // Create from other PathInfo

            var new_on_temp = new PathInfo(temp_path);
            Assert.AreEqual(new_on_temp, temp_path, "Create from other PathInfo");
        }

        
        /// <summary>
        ///A test for Base PathInfo operators
        ///</summary>
        [TestMethod()]
        public void Operators()
        {
            string eq_tes1 = null, eq_tes2 = null;

            Assert.AreEqual(    eq_tes1,   null,                    "string null  eq  null");
            Assert.IsTrue(      eq_tes1 == null,                    "string null  ==  null  is true");
            Assert.IsFalse(     eq_tes1 != null,                    "string null  !=  null  is false");
            Assert.AreEqual(    eq_tes1,   eq_tes2,                 "string null  eq  string null ");
            Assert.IsTrue(      eq_tes1 == eq_tes2,                 "string null  ==  string null  is true");
            Assert.IsFalse(     eq_tes1 != eq_tes2,                 "string null  !=  string null  is false");

            PathInfo path_eq_tes1 = null, path_eq_tes2 = null;

            // Compare not assigned PathInfo property with null - null == null

            Assert.AreEqual(    path_eq_tes1,   null,               "PathInfo null  eq  null");
            Assert.IsTrue(      path_eq_tes1 == null,               "PathInfo null  ==  null  is true");
            Assert.IsFalse(     path_eq_tes1 != null,               "PathInfo null  !=  null  is false");
            Assert.AreEqual(    path_eq_tes1,   path_eq_tes2,       "PathInfo null  eq  PathInfo null ");
            Assert.IsTrue(      path_eq_tes1 == path_eq_tes2,       "PathInfo null  ==  PathInfo null  is true");
            Assert.IsFalse(     path_eq_tes1 != path_eq_tes2,       "PathInfo null  !=  PathInfo null  is false");
            Assert.IsFalse(     path_eq_tes1 < path_eq_tes2,       "PathInfo null  <  PathInfo null  is false");
            Assert.IsFalse(     path_eq_tes1 > path_eq_tes2,       "PathInfo null  >  PathInfo null  is false");
            
            // Compare empty PathInfo() object with null - new PathInfo() != null

            Assert.AreNotEqual( new PathInfo(),   null,             "new PathInfo()  not eq  null");
            Assert.IsFalse(     new PathInfo() == null,             "new PathInfo() == null  is false");
            Assert.IsTrue(      new PathInfo() != null,             "new PathInfo() != null  is true");
            Assert.AreNotEqual( new PathInfo(),   path_eq_tes2,     "new PathInfo()  not eq  PathInfo null");
            Assert.IsFalse(     new PathInfo() == path_eq_tes2,     "new PathInfo() == PathInfo null  is false");
            Assert.IsTrue(      new PathInfo() != path_eq_tes2,     "new PathInfo() != PathInfo null  is true");
            Assert.IsFalse(     new PathInfo() < path_eq_tes2,     "new PathInfo() < PathInfo null  is false");
            Assert.IsTrue(      new PathInfo() > path_eq_tes2,     "new PathInfo() > PathInfo null  is true");
            Assert.IsTrue(      new PathInfo().CompareTo(path_eq_tes2) > 0, "new PathInfo().CompareTo(PathInfo null) > 0  is true");

            // Compare empty PathInfo() object with empty PathInfo() - new PathInfo() == new PathInfo() as two empty paths

            path_eq_tes2 = new PathInfo();
            Assert.AreEqual(    new PathInfo(),   new PathInfo(),   "new PathInfo()  eq  new PathInfo()");
            Assert.IsTrue(      new PathInfo() == new PathInfo(),   "new PathInfo() == new PathInfo()  is true");
            Assert.IsFalse(     new PathInfo() != new PathInfo(),   "new PathInfo() != new PathInfo()  is false");
            Assert.IsFalse(     new PathInfo() < new PathInfo(),   "new PathInfo() < new PathInfo()  is false");
            Assert.IsFalse(     new PathInfo() > new PathInfo(),   "new PathInfo() > new PathInfo()  is false");
            Assert.IsTrue(      new PathInfo().CompareTo(new PathInfo()) == 0, "new PathInfo().CompareTo(new PathInfo()) == 0  is true");

            // Verification equivalence checking of two equivalent paths

            var temp_path_string = IOPath.GetTempPath();
            Assert.AreEqual(new PathInfo(temp_path_string), new PathInfo(new PathInfo(temp_path_string)), "new PathInfo(GetTempPath())  eq  new PathInfo(GetTempPath())");
            Assert.IsTrue(new PathInfo(temp_path_string) == new PathInfo(new PathInfo(temp_path_string)), "new PathInfo(GetTempPath()) == new PathInfo(GetTempPath())  is true");
            Assert.IsFalse(new PathInfo(temp_path_string) != new PathInfo(new PathInfo(temp_path_string)), "new PathInfo(GetTempPath()) != new PathInfo(GetTempPath())  is false");

            // Verification equivalence checking of two inequivalent paths

            var temp_file_string = IOPath.GetTempFileName();
            Assert.AreNotEqual(new PathInfo(temp_path_string), new PathInfo(new PathInfo(temp_file_string)), "new PathInfo(GetTempPath())  not eq  new PathInfo(GetTempFileName())");
            Assert.IsFalse(new PathInfo(temp_path_string) == new PathInfo(new PathInfo(temp_file_string)), "new PathInfo(GetTempPath()) == new PathInfo(GetTempFileName())  is false");
            Assert.IsTrue(new PathInfo(temp_path_string) != new PathInfo(new PathInfo(temp_file_string)), "new PathInfo(GetTempPath()) != new PathInfo(GetTempFileName())  is true");

            // Verification comparison of two paths

            Assert.IsTrue(new PathInfo(@"C:\Z")     >   new PathInfo(@"C:\A"),      @" C:\Z) > C:\A  is true");
            Assert.IsTrue(new PathInfo(@"C:\A")     <   new PathInfo(@"C:\Z"),      @" C:\A) < C:\Z  is true");
            Assert.IsTrue(new PathInfo(@"C:\Z").CompareTo(new PathInfo(@"C:\A")) > 0, "new PathInfo(C:/Z).CompareTo(new PathInfo(C:/A)) > 0  is true");
            Assert.IsFalse(new PathInfo(@"C:\A\Z")  >   new PathInfo(@"C:\Z\A"),    @" C:\A\Z < C:\Z\A  is false");
            Assert.IsFalse(new PathInfo(@"C:\Z\A")  <   new PathInfo(@"C:\A\Z"),    @" C:\Z\A > C:\A\Z  is false");
            Assert.IsTrue(new PathInfo(@"C:\A\Z").CompareTo(new PathInfo(@"C:\Z\A")) < 0, "new PathInfo(C:/A/Z).CompareTo(new PathInfo(C:/Z/A)) < 0  is true");

            Assert.IsTrue(new PathInfo(@"C:\Z").CompareTo(new PathInfo(@"C:\A\Z")) > 0,      @" 'C:\Z'.CompareTo('C:\A') > 0  is true");
            Assert.IsTrue(new PathInfo(@"C:\B").CompareTo(new PathInfo(@"C:\Z\A")) < 0,      @" 'C:\A'.CompareTo('C:\Z') < 0  is true");
            

            // Combine operator

            var temp_path = new PathInfo(temp_path_string);
            var some_file = temp_path / "some_file_name.txt";
            Assert.AreEqual(some_file, temp_path_string + Path.DirectorySeparatorChar + "some_file_name.txt", "Combine / operator");
        }

        [TestMethod()]
        public void HashCode()
        {
            Assert.IsTrue(new PathInfo(@"C:\Z").GetHashCode() == new PathInfo(@"c:\z").GetHashCode(),      @"A hash code case insensitive");
        }

        [TestMethod()]
        public void Enumerable()
        {
            PathInfo temp = Path.GetTempPath();
            // var temp = new PathInfo(@"C:\TEMP");
            // var temp = new PathInfo(@"C:\TEMP", subdir, filename);
            // var tmp = some_path.Combine("some path segment");

            try
            {
                // List files by search pattern

                var tmp_files = temp & "*.tmp"; // all .tmp files in temp directory

                // Equivalent code in the classical form

                var files = Directory.EnumerateFiles(temp, "*.tmp");

                
                // List files by match comparer

                var regex = new Regex(@"\\[0-9]\.tmp$");
                var tmp_digital = temp & (path => { return regex.IsMatch(path); });                 // .tmp files files with only numbers in the name.
                
                // Linq

                var selected = tmp_files .Where(path => path.Name.StartsWith("Z"));
                selected = tmp_files .Where(path => path.RegexIsMatch("^Z.*$"));


                // PathList +- Operator example
                tmp_digital -= "Z.*";
                tmp_digital -= (path => path.Name.StartsWith("Z"));
                tmp_digital -= selected;
                tmp_digital += selected;

            
                // Bulk moving list of files example

                try
                {
                    // Rename .tmp files to .tmp.bak
                    (temp & "*.tmp")
                        .Bulk(path => { path.FileMove(path + ".bak"); });
                }
                catch (BulkException<PathInfo> e)
                {
                    foreach(var ewrapper in e.Failed)
                        Assert.Fail("failed " + ewrapper.Object);
                }


                // Bulk creation directories example

                try
                {
                    // a b c hidden directories creation example

                    new PathList(new[] { temp / "a", temp / "b", temp / "c" })
                        .Bulk(path => { path.DirectoryCreate().DirectoryInfo.Attributes |= FileAttributes.Hidden; });

                }
                catch (BulkException<PathInfo> e)
                {
                    foreach(var ewrapper in e.Failed)
                        Assert.Fail("failed " + ewrapper.Object);
                }
            }
            catch (Exception e)
            {
                // prepare list error
                throw;
            }
        }


        [TestMethod()]
        public void XMLSerialization()
        {
            var serializer = new XmlSerializer(typeof(PathInfo));
            var builder = new StringBuilder();
            using(var writer_to_builder = new StringWriter(builder))
                serializer.Serialize(writer_to_builder, new PathInfo(@"C:\TEMP\A\Z"));

            var deserialized = (PathInfo)serializer.Deserialize(new StringReader(builder.ToString()));

            Assert.AreEqual(deserialized.FullPath, @"C:\TEMP\A\Z", "xml serialization simple file path");
        }

        [TestMethod()]
        public void Serialization()
        {
            var serializer = new BinaryFormatter();
            var mem = new MemoryStream();
            new BinaryFormatter().Serialize(mem, new PathInfo(@"C:\TEMP\A\Z"));
            
            mem.Position = 0;
            var deserialized = (PathInfo)new BinaryFormatter().Deserialize(mem);

            Assert.AreEqual(deserialized.FullPath, @"C:\TEMP\A\Z", "xml serialization simple file path");
        }

        // Examples

        [TestMethod()]
        public void IniFile()
        {
            var ini = new IniFile(PathInfo.TEMP / "test.ini");

            ini[null] = "default value";
            ini["value 1"] = "1";
            ini["section 1", "value 2"] = "1-2";

            var another_instance = new IniFile(PathInfo.TEMP / "test.ini");
            Assert.AreEqual(another_instance[null], "default value", "default value");
            Assert.AreEqual(another_instance["value 1"], "1", "1");
            Assert.AreEqual(another_instance["section 1", "value 2"], "1-2", "1-2");

            ini.TryFileDelete();
            // or
            PathInfo.TEMP.TryFileDelete("test.ini");
            
        }
    }
}
