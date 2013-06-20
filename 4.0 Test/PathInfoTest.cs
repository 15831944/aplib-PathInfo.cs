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
            
            // Compare empty PathInfo() object with null - new PathInfo() != null

            Assert.AreNotEqual( new PathInfo(),   null,             "new PathInfo()  not eq  null");
            Assert.IsFalse(     new PathInfo() == null,             "new PathInfo() == null  is false");
            Assert.IsTrue(      new PathInfo() != null,             "new PathInfo() != null  is true");
            Assert.AreNotEqual( new PathInfo(),   path_eq_tes2,     "new PathInfo()  not eq  PathInfo null");
            Assert.IsFalse(     new PathInfo() == path_eq_tes2,     "new PathInfo() == PathInfo null  is false");
            Assert.IsTrue(      new PathInfo() != path_eq_tes2,     "new PathInfo() != PathInfo null  is true");

            // Compare empty PathInfo() object with empty PathInfo() - new PathInfo() == new PathInfo() as two empty paths

            path_eq_tes2 = new PathInfo();
            Assert.AreEqual(    new PathInfo(),   new PathInfo(),   "new PathInfo()  eq  new PathInfo()");
            Assert.IsTrue(      new PathInfo() == new PathInfo(),   "new PathInfo() == new PathInfo()  is true");
            Assert.IsFalse(     new PathInfo() != new PathInfo(),   "new PathInfo() != new PathInfo()  is false");

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
            Assert.IsFalse(new PathInfo(@"C:\A\Z")  >   new PathInfo(@"C:\Z\A"),    @" C:\A\Z < C:\Z\A  is false");
            Assert.IsFalse(new PathInfo(@"C:\Z\A")  <   new PathInfo(@"C:\A\Z"),    @" C:\Z\A > C:\A\Z  is false");

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

                var files = Directory.EnumerateFiles(Path.Combine(temp, "*.tmp"));

                
                // List files by match comparer

                var regex = new Regex(@"\\[0-9]\.tmp$");
                var tmp_digital = temp & (path => { return regex.IsMatch(path); });                 // .tmp files files with only numbers in the name.
                tmp_digital = temp & (path => { return Regex.IsMatch(path, @"\\[0-9]\.tmp$"); });   // A little simple, but more expensive.

                
                // Linq example

                var selected = tmp_files .Where(path => path.Name.StartsWith("Z"));


                // PathList +- Operator example

                tmp_digital -= selected;
                tmp_digital += selected;

            
                // Bulk moving list of files example

                try
                {
                    // Rename .tmp files to .tmp.bak
                    (temp & "*.tmp")
                        .Bulk(path => { path.FileMove(path.FullPath + ".bak"); });
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

                    new PathList(new[] { temp / "a", temp/ "b", temp / "c" })
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


        

    //    /// <summary>
    //    ///A test for PathInfo Constructor
    //    ///</summary>
    //    [TestMethod()]
    //    public void PathInfoConstructorTest5()
    //    {
    //        string base_path = string.Empty; // TODO: Initialize to an appropriate value
    //        string child1 = string.Empty; // TODO: Initialize to an appropriate value
    //        string child2 = string.Empty; // TODO: Initialize to an appropriate value
    //        string child3 = string.Empty; // TODO: Initialize to an appropriate value
    //        PathInfo target = new PathInfo(base_path, child1, child2, child3);
    //        Assert.Inconclusive("TODO: Implement code to verify target");
    //    }

    //    /// <summary>
    //    ///A test for CheckFileNameValidity
    //    ///</summary>
    //    [TestMethod()]
    //    public void CheckFileNameValidityTest()
    //    {
    //        string name = string.Empty; // TODO: Initialize to an appropriate value
    //        bool expected = false; // TODO: Initialize to an appropriate value
    //        bool actual;
    //        actual = PathInfo.CheckFileNameValidity(name);
    //        Assert.AreEqual(expected, actual);
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for ChildFileExists
    //    ///</summary>
    //    [TestMethod()]
    //    public void ChildFileExistsTest()
    //    {
    //        PathInfo target = new PathInfo(); // TODO: Initialize to an appropriate value
    //        string file_name = string.Empty; // TODO: Initialize to an appropriate value
    //        bool expected = false; // TODO: Initialize to an appropriate value
    //        bool actual;
    //        actual = target.ChildFileExists(file_name);
    //        Assert.AreEqual(expected, actual);
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for Childs
    //    ///</summary>
    //    [TestMethod()]
    //    public void ChildsTest()
    //    {
    //        PathInfo target = new PathInfo(); // TODO: Initialize to an appropriate value
    //        string search_pattern = string.Empty; // TODO: Initialize to an appropriate value
    //        IEnumerable<PathInfo> expected = null; // TODO: Initialize to an appropriate value
    //        IEnumerable<PathInfo> actual;
    //        actual = target.Childs(search_pattern);
    //        Assert.AreEqual(expected, actual);
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for Combine
    //    ///</summary>
    //    [TestMethod()]
    //    public void CombineTest()
    //    {
    //        PathInfo target = new PathInfo(); // TODO: Initialize to an appropriate value
    //        string[] sub_path = null; // TODO: Initialize to an appropriate value
    //        PathInfo expected = null; // TODO: Initialize to an appropriate value
    //        PathInfo actual;
    //        actual = target.Combine(sub_path);
    //        Assert.AreEqual(expected, actual);
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for CompareTo
    //    ///</summary>
    //    [TestMethod()]
    //    public void CompareToTest()
    //    {
    //        PathInfo target = new PathInfo(); // TODO: Initialize to an appropriate value
    //        PathInfo other = null; // TODO: Initialize to an appropriate value
    //        int expected = 0; // TODO: Initialize to an appropriate value
    //        int actual;
    //        actual = target.CompareTo(other);
    //        Assert.AreEqual(expected, actual);
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for CompareTo
    //    ///</summary>
    //    [TestMethod()]
    //    public void CompareToTest1()
    //    {
    //        PathInfo target = new PathInfo(); // TODO: Initialize to an appropriate value
    //        object obj = null; // TODO: Initialize to an appropriate value
    //        int expected = 0; // TODO: Initialize to an appropriate value
    //        int actual;
    //        actual = target.CompareTo(obj);
    //        Assert.AreEqual(expected, actual);
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for Create
    //    ///</summary>
    //    [TestMethod()]
    //    public void CreateTest()
    //    {
    //        string[] fragments = null; // TODO: Initialize to an appropriate value
    //        PathInfo expected = null; // TODO: Initialize to an appropriate value
    //        PathInfo actual;
    //        actual = PathInfo.Create(fragments);
    //        Assert.AreEqual(expected, actual);
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for CreateDirectory
    //    ///</summary>
    //    [TestMethod()]
    //    public void CreateDirectoryTest()
    //    {
    //        PathInfo target = new PathInfo(); // TODO: Initialize to an appropriate value
    //        target.CreateDirectory();
    //        Assert.Inconclusive("A method that does not return a value cannot be verified.");
    //    }

    //    /// <summary>
    //    ///A test for Directories
    //    ///</summary>
    //    [TestMethod()]
    //    public void DirectoriesTest()
    //    {
    //        PathInfo target = new PathInfo(); // TODO: Initialize to an appropriate value
    //        string search_pattern = string.Empty; // TODO: Initialize to an appropriate value
    //        bool recursively = false; // TODO: Initialize to an appropriate value
    //        IEnumerable<PathInfo> expected = null; // TODO: Initialize to an appropriate value
    //        IEnumerable<PathInfo> actual;
    //        actual = target.Directories(search_pattern, recursively);
    //        Assert.AreEqual(expected, actual);
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for DirectoryAccessible
    //    ///</summary>
    //    [TestMethod()]
    //    public void DirectoryAccessibleTest()
    //    {
    //        PathInfo target = new PathInfo(); // TODO: Initialize to an appropriate value
    //        bool expected = false; // TODO: Initialize to an appropriate value
    //        bool actual;
    //        actual = target.DirectoryAccessible();
    //        Assert.AreEqual(expected, actual);
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for DirectoryExists
    //    ///</summary>
    //    [TestMethod()]
    //    public void DirectoryExistsTest()
    //    {
    //        PathInfo target = new PathInfo(); // TODO: Initialize to an appropriate value
    //        bool expected = false; // TODO: Initialize to an appropriate value
    //        bool actual;
    //        actual = target.DirectoryExists();
    //        Assert.AreEqual(expected, actual);
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for DirectoryExists
    //    ///</summary>
    //    [TestMethod()]
    //    public void DirectoryExistsTest1()
    //    {
    //        PathInfo target = new PathInfo(); // TODO: Initialize to an appropriate value
    //        string dir_name = string.Empty; // TODO: Initialize to an appropriate value
    //        bool expected = false; // TODO: Initialize to an appropriate value
    //        bool actual;
    //        actual = target.DirectoryExists(dir_name);
    //        Assert.AreEqual(expected, actual);
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for DirectoryInfo
    //    ///</summary>
    //    [TestMethod()]
    //    public void DirectoryInfoTest()
    //    {
    //        PathInfo target = new PathInfo(); // TODO: Initialize to an appropriate value
    //        DirectoryInfo expected = null; // TODO: Initialize to an appropriate value
    //        DirectoryInfo actual;
    //        actual = target.DirectoryInfo();
    //        Assert.AreEqual(expected, actual);
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for DirectorySecurity
    //    ///</summary>
    //    [TestMethod()]
    //    public void DirectorySecurityTest()
    //    {
    //        PathInfo target = new PathInfo(); // TODO: Initialize to an appropriate value
    //        AccessControlSections include_sections = new AccessControlSections(); // TODO: Initialize to an appropriate value
    //        DirectorySecurity expected = null; // TODO: Initialize to an appropriate value
    //        DirectorySecurity actual;
    //        actual = target.DirectorySecurity(include_sections);
    //        Assert.AreEqual(expected, actual);
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for Equals
    //    ///</summary>
    //    [TestMethod()]
    //    public void EqualsTest()
    //    {
    //        PathInfo target = new PathInfo(); // TODO: Initialize to an appropriate value
    //        object obj = null; // TODO: Initialize to an appropriate value
    //        bool expected = false; // TODO: Initialize to an appropriate value
    //        bool actual;
    //        actual = target.Equals(obj);
    //        Assert.AreEqual(expected, actual);
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for FileAccessable
    //    ///</summary>
    //    [TestMethod()]
    //    public void FileAccessableTest()
    //    {
    //        PathInfo target = new PathInfo(); // TODO: Initialize to an appropriate value
    //        bool expected = false; // TODO: Initialize to an appropriate value
    //        bool actual;
    //        actual = target.FileAccessable();
    //        Assert.AreEqual(expected, actual);
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for FileCopy
    //    ///</summary>
    //    [TestMethod()]
    //    public void FileCopyTest()
    //    {
    //        PathInfo target = new PathInfo(); // TODO: Initialize to an appropriate value
    //        PathInfo destination_file_path = null; // TODO: Initialize to an appropriate value
    //        bool overwrite = false; // TODO: Initialize to an appropriate value
    //        PathInfo expected = null; // TODO: Initialize to an appropriate value
    //        PathInfo actual;
    //        actual = target.FileCopy(destination_file_path, overwrite);
    //        Assert.AreEqual(expected, actual);
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for FileCopyToFolder
    //    ///</summary>
    //    [TestMethod()]
    //    public void FileCopyToFolderTest()
    //    {
    //        PathInfo target = new PathInfo(); // TODO: Initialize to an appropriate value
    //        PathInfo destination_dir_path = null; // TODO: Initialize to an appropriate value
    //        bool overwrite = false; // TODO: Initialize to an appropriate value
    //        PathInfo expected = null; // TODO: Initialize to an appropriate value
    //        PathInfo actual;
    //        actual = target.FileCopyToFolder(destination_dir_path, overwrite);
    //        Assert.AreEqual(expected, actual);
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for FileDelete
    //    ///</summary>
    //    [TestMethod()]
    //    public void FileDeleteTest()
    //    {
    //        PathInfo target = new PathInfo(); // TODO: Initialize to an appropriate value
    //        target.FileDelete();
    //        Assert.Inconclusive("A method that does not return a value cannot be verified.");
    //    }

    //    /// <summary>
    //    ///A test for FileExists
    //    ///</summary>
    //    [TestMethod()]
    //    public void FileExistsTest()
    //    {
    //        PathInfo target = new PathInfo(); // TODO: Initialize to an appropriate value
    //        bool expected = false; // TODO: Initialize to an appropriate value
    //        bool actual;
    //        actual = target.FileExists();
    //        Assert.AreEqual(expected, actual);
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for FileInfo
    //    ///</summary>
    //    [TestMethod()]
    //    public void FileInfoTest()
    //    {
    //        PathInfo target = new PathInfo(); // TODO: Initialize to an appropriate value
    //        FileInfo expected = null; // TODO: Initialize to an appropriate value
    //        FileInfo actual;
    //        actual = target.FileInfo();
    //        Assert.AreEqual(expected, actual);
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for FileMove
    //    ///</summary>
    //    [TestMethod()]
    //    public void FileMoveTest()
    //    {
    //        PathInfo target = new PathInfo(); // TODO: Initialize to an appropriate value
    //        PathInfo destination_file_path = null; // TODO: Initialize to an appropriate value
    //        PathInfo expected = null; // TODO: Initialize to an appropriate value
    //        PathInfo actual;
    //        actual = target.FileMove(destination_file_path);
    //        Assert.AreEqual(expected, actual);
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for FileOpen
    //    ///</summary>
    //    [TestMethod()]
    //    public void FileOpenTest()
    //    {
    //        PathInfo target = new PathInfo(); // TODO: Initialize to an appropriate value
    //        FileMode mode = new FileMode(); // TODO: Initialize to an appropriate value
    //        FileAccess access = new FileAccess(); // TODO: Initialize to an appropriate value
    //        FileShare share = new FileShare(); // TODO: Initialize to an appropriate value
    //        FileStream expected = null; // TODO: Initialize to an appropriate value
    //        FileStream actual;
    //        actual = target.FileOpen(mode, access, share);
    //        Assert.AreEqual(expected, actual);
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for FileSystemEntries
    //    ///</summary>
    //    [TestMethod()]
    //    public void FileSystemEntriesTest()
    //    {
    //        PathInfo target = new PathInfo(); // TODO: Initialize to an appropriate value
    //        string search_pattern = string.Empty; // TODO: Initialize to an appropriate value
    //        bool recursive = false; // TODO: Initialize to an appropriate value
    //        IEnumerable<PathInfo> expected = null; // TODO: Initialize to an appropriate value
    //        IEnumerable<PathInfo> actual;
    //        actual = target.FileSystemEntries(search_pattern, recursive);
    //        Assert.AreEqual(expected, actual);
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for Files
    //    ///</summary>
    //    [TestMethod()]
    //    public void FilesTest()
    //    {
    //        PathInfo target = new PathInfo(); // TODO: Initialize to an appropriate value
    //        string search_pattern = string.Empty; // TODO: Initialize to an appropriate value
    //        bool recursively = false; // TODO: Initialize to an appropriate value
    //        IEnumerable<PathInfo> expected = null; // TODO: Initialize to an appropriate value
    //        IEnumerable<PathInfo> actual;
    //        actual = target.Files(search_pattern, recursively);
    //        Assert.AreEqual(expected, actual);
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for ForceParentDirectory
    //    ///</summary>
    //    [TestMethod()]
    //    public void ForceParentDirectoryTest()
    //    {
    //        PathInfo target = new PathInfo(); // TODO: Initialize to an appropriate value
    //        FileAttributes attributes = new FileAttributes(); // TODO: Initialize to an appropriate value
    //        PathInfo expected = null; // TODO: Initialize to an appropriate value
    //        PathInfo actual;
    //        actual = target.ForceParentDirectory(attributes);
    //        Assert.AreEqual(expected, actual);
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for GetHashCode
    //    ///</summary>
    //    [TestMethod()]
    //    public void GetHashCodeTest()
    //    {
    //        PathInfo target = new PathInfo(); // TODO: Initialize to an appropriate value
    //        int expected = 0; // TODO: Initialize to an appropriate value
    //        int actual;
    //        actual = target.GetHashCode();
    //        Assert.AreEqual(expected, actual);
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for HasChild
    //    ///</summary>
    //    [TestMethod()]
    //    public void HasChildTest()
    //    {
    //        PathInfo target = new PathInfo(); // TODO: Initialize to an appropriate value
    //        bool expected = false; // TODO: Initialize to an appropriate value
    //        bool actual;
    //        actual = target.HasChild();
    //        Assert.AreEqual(expected, actual);
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for HasFiles
    //    ///</summary>
    //    [TestMethod()]
    //    public void HasFilesTest()
    //    {
    //        PathInfo target = new PathInfo(); // TODO: Initialize to an appropriate value
    //        bool expected = false; // TODO: Initialize to an appropriate value
    //        bool actual;
    //        actual = target.HasFiles();
    //        Assert.AreEqual(expected, actual);
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for HasSubdirectory
    //    ///</summary>
    //    [TestMethod()]
    //    public void HasSubdirectoryTest()
    //    {
    //        PathInfo target = new PathInfo(); // TODO: Initialize to an appropriate value
    //        bool expected = false; // TODO: Initialize to an appropriate value
    //        bool actual;
    //        actual = target.HasSubdirectory();
    //        Assert.AreEqual(expected, actual);
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for ToRelativePath
    //    ///</summary>
    //    [TestMethod()]
    //    public void ToRelativePathTest()
    //    {
    //        PathInfo target = new PathInfo(); // TODO: Initialize to an appropriate value
    //        PathInfo base_path = null; // TODO: Initialize to an appropriate value
    //        PathInfo expected = null; // TODO: Initialize to an appropriate value
    //        PathInfo actual;
    //        actual = target.ToRelativePath(base_path);
    //        Assert.AreEqual(expected, actual);
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for ToString
    //    ///</summary>
    //    [TestMethod()]
    //    public void ToStringTest()
    //    {
    //        PathInfo target = new PathInfo(); // TODO: Initialize to an appropriate value
    //        string expected = string.Empty; // TODO: Initialize to an appropriate value
    //        string actual;
    //        actual = target.ToString();
    //        Assert.AreEqual(expected, actual);
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for TryCreateDirectory
    //    ///</summary>
    //    [TestMethod()]
    //    public void TryCreateDirectoryTest()
    //    {
    //        PathInfo target = new PathInfo(); // TODO: Initialize to an appropriate value
    //        bool expected = false; // TODO: Initialize to an appropriate value
    //        bool actual;
    //        actual = target.TryCreateDirectory();
    //        Assert.AreEqual(expected, actual);
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for TryCreateParentDirectory
    //    ///</summary>
    //    [TestMethod()]
    //    public void TryCreateParentDirectoryTest()
    //    {
    //        PathInfo target = new PathInfo(); // TODO: Initialize to an appropriate value
    //        bool expected = false; // TODO: Initialize to an appropriate value
    //        bool actual;
    //        actual = target.TryCreateParentDirectory();
    //        Assert.AreEqual(expected, actual);
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for TryDirectoryExists
    //    ///</summary>
    //    [TestMethod()]
    //    public void TryDirectoryExistsTest()
    //    {
    //        PathInfo target = new PathInfo(); // TODO: Initialize to an appropriate value
    //        bool expected = false; // TODO: Initialize to an appropriate value
    //        bool actual;
    //        actual = target.TryDirectoryExists();
    //        Assert.AreEqual(expected, actual);
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for TryFileDelete
    //    ///</summary>
    //    [TestMethod()]
    //    public void TryFileDeleteTest()
    //    {
    //        PathInfo target = new PathInfo(); // TODO: Initialize to an appropriate value
    //        bool expected = false; // TODO: Initialize to an appropriate value
    //        bool actual;
    //        actual = target.TryFileDelete();
    //        Assert.AreEqual(expected, actual);
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for TryForceParentDirectory
    //    ///</summary>
    //    [TestMethod()]
    //    public void TryForceParentDirectoryTest()
    //    {
    //        PathInfo target = new PathInfo(); // TODO: Initialize to an appropriate value
    //        PathInfo expected = null; // TODO: Initialize to an appropriate value
    //        PathInfo actual;
    //        actual = target.TryForceParentDirectory();
    //        Assert.AreEqual(expected, actual);
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for ValidateFileName
    //    ///</summary>
    //    [TestMethod()]
    //    public void ValidateFileNameTest()
    //    {
    //        string _name = string.Empty; // TODO: Initialize to an appropriate value
    //        string expected = string.Empty; // TODO: Initialize to an appropriate value
    //        string actual;
    //        actual = PathInfo.ValidateFileName(_name);
    //        Assert.AreEqual(expected, actual);
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for op_BitwiseAnd
    //    ///</summary>
    //    [TestMethod()]
    //    public void op_BitwiseAndTest()
    //    {
    //        PathInfo path = null; // TODO: Initialize to an appropriate value
    //        string search_pattern = string.Empty; // TODO: Initialize to an appropriate value
    //        IEnumerable<PathInfo> expected = null; // TODO: Initialize to an appropriate value
    //        IEnumerable<PathInfo> actual;
    //        actual = (path & search_pattern);
    //        Assert.AreEqual(expected, actual);
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for op_Division
    //    ///</summary>
    //    [TestMethod()]
    //    public void op_DivisionTest()
    //    {
    //        PathInfo path = null; // TODO: Initialize to an appropriate value
    //        string path_fragment = string.Empty; // TODO: Initialize to an appropriate value
    //        PathInfo expected = null; // TODO: Initialize to an appropriate value
    //        PathInfo actual;
    //        actual = (path / path_fragment);
    //        Assert.AreEqual(expected, actual);
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for op_Equality
    //    ///</summary>
    //    [TestMethod()]
    //    public void op_EqualityTest()
    //    {
    //        PathInfo path1 = null; // TODO: Initialize to an appropriate value
    //        PathInfo path2 = null; // TODO: Initialize to an appropriate value
    //        bool expected = false; // TODO: Initialize to an appropriate value
    //        bool actual;
    //        actual = (path1 == path2);
    //        Assert.AreEqual(expected, actual);
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for op_ExclusiveOr
    //    ///</summary>
    //    [TestMethod()]
    //    public void op_ExclusiveOrTest()
    //    {
    //        PathInfo path = null; // TODO: Initialize to an appropriate value
    //        string search_pattern = string.Empty; // TODO: Initialize to an appropriate value
    //        IEnumerable<PathInfo> expected = null; // TODO: Initialize to an appropriate value
    //        IEnumerable<PathInfo> actual;
    //        actual = (path ^ search_pattern);
    //        Assert.AreEqual(expected, actual);
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for op_GreaterThan
    //    ///</summary>
    //    [TestMethod()]
    //    public void op_GreaterThanTest()
    //    {
    //        PathInfo path1 = null; // TODO: Initialize to an appropriate value
    //        PathInfo path2 = null; // TODO: Initialize to an appropriate value
    //        bool expected = false; // TODO: Initialize to an appropriate value
    //        bool actual;
    //        actual = (path1 > path2);
    //        Assert.AreEqual(expected, actual);
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for op_Implicit
    //    ///</summary>
    //    [TestMethod()]
    //    public void op_ImplicitTest()
    //    {
    //        PathInfo path_info = null; // TODO: Initialize to an appropriate value
    //        string expected = string.Empty; // TODO: Initialize to an appropriate value
    //        string actual;
    //        actual = path_info;
    //        Assert.AreEqual(expected, actual);
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for op_Implicit
    //    ///</summary>
    //    [TestMethod()]
    //    public void op_ImplicitTest1()
    //    {
    //        string path = string.Empty; // TODO: Initialize to an appropriate value
    //        PathInfo expected = null; // TODO: Initialize to an appropriate value
    //        PathInfo actual;
    //        actual = path;
    //        Assert.AreEqual(expected, actual);
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for op_Inequality
    //    ///</summary>
    //    [TestMethod()]
    //    public void op_InequalityTest()
    //    {
    //        PathInfo path1 = null; // TODO: Initialize to an appropriate value
    //        PathInfo path2 = null; // TODO: Initialize to an appropriate value
    //        bool expected = false; // TODO: Initialize to an appropriate value
    //        bool actual;
    //        actual = (path1 != path2);
    //        Assert.AreEqual(expected, actual);
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for op_LessThan
    //    ///</summary>
    //    [TestMethod()]
    //    public void op_LessThanTest()
    //    {
    //        PathInfo path1 = null; // TODO: Initialize to an appropriate value
    //        PathInfo path2 = null; // TODO: Initialize to an appropriate value
    //        bool expected = false; // TODO: Initialize to an appropriate value
    //        bool actual;
    //        actual = (path1 < path2);
    //        Assert.AreEqual(expected, actual);
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for op_Multiply
    //    ///</summary>
    //    [TestMethod()]
    //    public void op_MultiplyTest()
    //    {
    //        PathInfo path = null; // TODO: Initialize to an appropriate value
    //        string search_pattern = string.Empty; // TODO: Initialize to an appropriate value
    //        IEnumerable<PathInfo> expected = null; // TODO: Initialize to an appropriate value
    //        IEnumerable<PathInfo> actual;
    //        actual = (path * search_pattern);
    //        Assert.AreEqual(expected, actual);
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for Base
    //    ///</summary>
    //    [TestMethod()]
    //    public void BaseTest()
    //    {
    //        PathInfo target = new PathInfo(); // TODO: Initialize to an appropriate value
    //        PathInfo expected = null; // TODO: Initialize to an appropriate value
    //        PathInfo actual;
    //        target.Base = expected;
    //        actual = target.Base;
    //        Assert.AreEqual(expected, actual);
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for Empty
    //    ///</summary>
    //    [TestMethod()]
    //    public void EmptyTest()
    //    {
    //        PathInfo target = new PathInfo(); // TODO: Initialize to an appropriate value
    //        bool actual;
    //        actual = target.Empty;
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for Extension
    //    ///</summary>
    //    [TestMethod()]
    //    public void ExtensionTest()
    //    {
    //        PathInfo target = new PathInfo(); // TODO: Initialize to an appropriate value
    //        string actual;
    //        actual = target.Extension;
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for FileName
    //    ///</summary>
    //    [TestMethod()]
    //    public void FileNameTest()
    //    {
    //        PathInfo target = new PathInfo(); // TODO: Initialize to an appropriate value
    //        string expected = string.Empty; // TODO: Initialize to an appropriate value
    //        string actual;
    //        target.FileName = expected;
    //        actual = target.FileName;
    //        Assert.AreEqual(expected, actual);
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for Fragments
    //    ///</summary>
    //    [TestMethod()]
    //    public void FragmentsTest()
    //    {
    //        PathInfo target = new PathInfo(); // TODO: Initialize to an appropriate value
    //        string[] actual;
    //        actual = target.Fragments;
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for FullPath
    //    ///</summary>
    //    [TestMethod()]
    //    public void FullPathTest()
    //    {
    //        PathInfo target = new PathInfo(); // TODO: Initialize to an appropriate value
    //        string actual;
    //        actual = target.FullPath;
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for FullPathUpperCase
    //    ///</summary>
    //    [TestMethod()]
    //    public void FullPathUpperCaseTest()
    //    {
    //        PathInfo target = new PathInfo(); // TODO: Initialize to an appropriate value
    //        string actual;
    //        actual = target.FullPathUpperCase;
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for IsRelative
    //    ///</summary>
    //    [TestMethod()]
    //    public void IsRelativeTest()
    //    {
    //        PathInfo target = new PathInfo(); // TODO: Initialize to an appropriate value
    //        bool actual;
    //        actual = target.IsRelative;
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for Name
    //    ///</summary>
    //    [TestMethod()]
    //    public void NameTest()
    //    {
    //        PathInfo target = new PathInfo(); // TODO: Initialize to an appropriate value
    //        string actual;
    //        actual = target.Name;
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for Parent
    //    ///</summary>
    //    [TestMethod()]
    //    public void ParentTest()
    //    {
    //        PathInfo target = new PathInfo(); // TODO: Initialize to an appropriate value
    //        PathInfo actual;
    //        actual = target.Parent;
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }

    //    /// <summary>
    //    ///A test for Path
    //    ///</summary>
    //    [TestMethod()]
    //    public void PathTest()
    //    {
    //        PathInfo target = new PathInfo(); // TODO: Initialize to an appropriate value
    //        string expected = string.Empty; // TODO: Initialize to an appropriate value
    //        string actual;
    //        target.Path = expected;
    //        actual = target.Path;
    //        Assert.AreEqual(expected, actual);
    //        Assert.Inconclusive("Verify the correctness of this test method.");
    //    }
    }
}
