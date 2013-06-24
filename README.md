PathInfo.cs
===========

C# PathInfo.cs module encapsulates a file system path (not shell path!) and wraps Path, File, Directory static class methods from System.IO namespace, provides FileInfo DirectoryInfo objects, has additional properties and methods.
This is a simple module distributed under MIT license, you can use in any project and write code in a little more functional style.

Nearest projects
-----------
FluentPath http://fluentpath.codeplex.com/
NDepend.Helpers(?) https://github.com/curasystems/externals/tree/master/libs/NDepend.Helpers.FileDirectoryPath.1.0

Features
-----------

PathInfo

- Creating, combining, comparing, enumerating file system paths.
- Special folders, name generators, all file and directory operations.
- / * ^ & Operators

PathList

- + - Operators
- Bulk file operations


See examples in the unit test, and in the folder Examples


```csharp
// Creating

PathInfo some_path = Path.GetTempPath();                                    // by assigning the path
some_path = new PathInfo(@"C:\TEMP", "subdir", "filename.txt");             // from segments
some_path = PathInfo.Create(@"C:\TEMP\subdir", "filename.txt");             // static factory
some_path = new PathInfo(@"C:\TEMP\subdir\filename.txt");                   // from full path

// Special folders

some_path = PathInfo.APPLICATION_DATA_LOCAL / "some app folder";
var temp_path = PathInfo.TEMP;
PathInfo app_local = Environment.SpecialFolder.LocalApplicationData;        // by assigning the Environment.SpecialFolder value
app_local = (PathInfo)Environment.SpecialFolder.LocalApplicationData / "some app folder";

// Combining

some_path = some_path.Parent.Combine("some path segment", "filename.txt");  // Combine()
some_path = some_path.Parent / "some path segment" / "filename.txt";        // "/" Operator 

// Enumerating

var tmp_files = temp_path.Files("*.tmp");                                   // Enumerate .tmp files in temp directory
var tmp_dirs = temp_path.Directories("*.tmp");
tmp_files = temp_path & "*.tmp";                                            // eq & Operator enumerate files

var regex = new Regex(@"\\[0-9]*\.tmp$");
var tmp_digital = temp_path & (path => { return regex.IsMatch(path); });    // Enumerate by match comparer
            
var hidden_dirs = temp_path.Directories().Where(dir => dir.Attributes.HasFlag(FileAttributes.Hidden)); // Enumerate hidden directories in temp directory

// Enumerable

var selected = tmp_files .Where(path => path.Name.StartsWith("Z"));
selected = tmp_files .WhereFileName("^Z.*$");                               // Regex mathing

// PathList, set operations

tmp_digital += temp_path.Files().WhereFileName(@"[0-9]*\.exe");
tmp_digital -= "Z.*";
tmp_digital -= (path => path.Name.StartsWith("Z"));
var dirs = tmp_dirs + hidden_dirs;

// Bulk
            
try
{
    // files renaming example

    tmp_files .Bulk(file => { file.Rename(file.FileName + ".bak"); });

}
catch (BulkException<PathInfo> e)
{
    foreach(var renamed in e.Successful)
        Console.WriteLine(string.Format("renaming succesul, new file name: {0}", renamed.FileName));

    foreach(var fail in e.Failed)
        Console.WriteLine(string.Format("renaming failed {0}", fail.Object));
}

try
{
    // a b c hidden directories creation example

    new PathList(new[] { temp_path / "a", temp_path / "b", temp_path / "c" })
        .Bulk(path => { path.DirectoryCreate().DirectoryInfo.Attributes |= FileAttributes.Hidden; });

}
catch (BulkException<PathInfo> e)
{
    foreach (var fail in e.Failed)
        Console.WriteLine(string.Format("failed ", fail.Object));
}



```