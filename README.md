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
PathInfo temp = Path.GetTempPath();
// var temp = new PathInfo(@"C:\TEMP");
// var temp_file = PathInfo.TEMP / subdir / filename;
// var tmp  = some_path.Combine("some path segment");


// List files by search pattern

var tmp_files = temp & "*.tmp"; // all .tmp files in temp directory

// Equivalent code in the classical form

var files = Directory.EnumerateFiles(temp, "*.tmp");

                
// List files by match comparer

var regex = new Regex(@"\\[0-9]\.tmp$");
var tmp_digital = temp & (path => { return regex.IsMatch(path); }); // .tmp files files with only numbers in the name.

                
// Linq example

var selected = tmp_files .Where(path => path.Name.StartsWith("Z"));


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

```