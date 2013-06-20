PathInfo.cs
===========

C# PathInfo.cs module encapsulates a file system path (not shell path!) and wraps Path, File, Directory static class methods from System.IO namespace, provides FileInfo DirectoryInfo objects, has additional properties and methods.
This is one simple module, and several classes of complement system namespaces System.IO.


PathInfo temp = Path.GetTempPath();
// var temp = new PathInfo(@"C:\TEMP");
// var temp = new PathInfo(@"C:\TEMP", subdir, filename);
// var tmp  = some_path.Combine("some path segment");


// List files by search pattern

var tmp_files = temp & "*.tmp"; // all .tmp files in temp directory

// Equivalent code in the classical form

var files = Directory.EnumerateFiles(Path.Combine(temp, "*.tmp"));

                
// List files by match comparer

var regex = new Regex(@"\\[0-9]\.tmp$");
var tmp_digital = temp & (path => { return regex.IsMatch(path); }); // .tmp files files with only numbers in the name.
tmp_digital = temp & (path => { return Regex.IsMatch(path, @"\\[0-9]\.tmp$"); }); // A little simple, but more expensive.

                
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
