//#define used_system_xml // If you do not import the System.Xml namespace, comment out this line
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.AccessControl;
using System.Text;
#region used_system_xml
#if used_system_xml
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
#endif
#endregion used_system_xml
using IOPath = System.IO.Path;

/* Copyright © 2012 Vadim Baklanov (Ad), distributed under the MIT License
 * When copying, use or create derivative works do not remove or modify this attribution, and this license text.*/

namespace System.IO
{
    /// <summary>This class encapsulates a file system path (not shell path!) and wraps Path, File, Directory static class methods
    /// from System.IO namespace, provides FileInfo DirectoryInfo objects, has additional properties and methods.
    /// See also my ShellPathInfo</summary>
#region used_system_xml
#if used_system_xml
    [XmlRoot("Path")]
#endif
#endregion used_system_xml
    [Serializable]
    [ComVisible(true)]
    public partial class PathInfo : IEquatable<PathInfo>, IComparable, IComparable<PathInfo>, ISerializable
#region used_system_xml
#if used_system_xml
        , IXmlSerializable
#endif
#endregion used_system_xml
    {
        // Constants

        public static char[] PathSeparators            = new[] { IOPath.DirectorySeparatorChar, IOPath.AltDirectorySeparatorChar };
        public static char   DirectorySeparatorChar    = IOPath.DirectorySeparatorChar;
        public static char   AltDirectorySeparatorChar = IOPath.AltDirectorySeparatorChar;
        public static char   VolumeSeparatorChar       = IOPath.VolumeSeparatorChar;
        public static char[] InvalidFileNameChars      = IOPath.GetInvalidFileNameChars().OrderBy(chr => chr).ToArray();
        public static char[] InvalidPathChars          = IOPath.GetInvalidPathChars().OrderBy(chr => chr).ToArray();

        // Basic attributes

        /// <summary>Gets a value indicating whether the path is relative to the Base path.</summary>
        public bool IsRelative
        {
            get { return ((object)_Base != null); }
        }

        bool _Empty = true;
        public bool Empty
        {
            get { return _Empty; }
        }

        public bool Immutable {get;set;}

        PathInfo _Base;
        /// <summary>Base for relative form of path. May be null.</summary>
        public PathInfo Base
        {
            get { return _Base; }
            set
            {
                if (Immutable)
                    throw new InvalidOperationException("This PathInfo object is immutable!");

                _Base = value;
                _FullPath = null;
                _FullPathUpperCase = null;

                _name_retrieved     = false;
                _Name               = null;
                file_name_retrieved = false;
                _FileName           = null;
                extension_retrieved = false;
                _Extension          = null;
                parent_retrieved    = false;
                _Parent             = null;
                segments_retrieved  = false;
                _Segments           = null;
                directory_info_retrieved = false;
                _DirectoryInfo      = null;
                file_info_retrieved = false;
                _FileInfo           = null;

                _Empty = ((object)_Path == null || _Path.Length == 0) && ((object)_Base == null || _Base.Empty);
            }
        }

        string _Path;
        /// <summary>Full path or relative to the Base path.</summary>
        public string Path
        {
            get { return _Path; }
            set
            {
                if (Immutable)
                    throw new InvalidOperationException("This PathInfo object is immutable!");

                _Path = value;
                _FullPath = null;
                _FullPathUpperCase = null;

                _name_retrieved     = false;
                _Name               = null;
                file_name_retrieved = false;
                _FileName           = null;
                extension_retrieved = false;
                _Extension          = null;
                parent_retrieved    = false;
                _Parent             = null;
                segments_retrieved  = false;
                _Segments           = null;
                directory_info_retrieved = false;
                _DirectoryInfo      = null;
                file_info_retrieved = false;
                _FileInfo           = null;

                _Empty = ((object)_Path == null || _Path.Length == 0) && ((object)_Base == null || _Base.Empty);
            }
        }

        // TODO
        public PathInfo ToRelativePath(PathInfo base_path)
        {
            // Получить относительный путь, относительно указанного базового

            //if ((object)_Base == null)
            //{
            //    // Путь не является относительным

            //    if (FullPathUpperCase.StartsWith(base_path.PathLowerCase))
            //    {
            //        // Да, это относительный путь, конструируем объект относительного пути:
            //        return new PathInfo(Path.Substring(base_path.Path.Length + 1)) { Base = new PathInfo(base_path) };
            //    }
            //}

            return null;
        }

        bool segments_retrieved;
        string[] _Segments;
        public string[] Segments
        {
            get
            {
                if (!segments_retrieved && (_Path != null || (_Base != null && _Base.FullPath != null)))
                {
                    _Segments = FullPath.Split(PathSeparators);
                    segments_retrieved = true;
                }
                return _Segments;
            }
        }

        string _FullPath;
        /// <summary>Get the full path</summary>
		public string FullPath
        {
            get
            {
                if (_FullPath == null)
                {
                    if ((object)_Base != null)
                    {
                        _FullPath = _Base.FullPath;

                        if ((object)_Path != null)
                        {
                            if (_FullPath.Length > 0)
                            {
                                char last_char = _FullPath[_FullPath.Length - 1];
                                if (last_char != DirectorySeparatorChar && last_char != AltDirectorySeparatorChar)
                                    _FullPath += DirectorySeparatorChar;

                                _FullPath += _Path;
                            }
                            else
                                _FullPath = DirectorySeparatorChar + _Path;
                        }
                    }
                    else
                        _FullPath = _Path;
                }

                return _FullPath;
            }
        }

        /// <summary>Full path to uppercase for case-insensitive comparison and quick search in lists.
        /// The need for this requisite arises from the lack of guarantees from the operating system to return the file path in the same case. Drive char case in equivalent path may be different as result different API requests.
        /// </summary>
        /// 
        string _FullPathUpperCase;
        public string FullPathUpperCase
        {
            get
            {
                if (_FullPathUpperCase == null && (_Path != null || (object)_Base != null))
                    _FullPathUpperCase = FullPath.ToUpperInvariant();

                return _FullPathUpperCase;
            }
        }

		string _FileName;
		bool file_name_retrieved;
        /// <summary>File name and extension</summary>
	    public string FileName
		{
			get
			{
				if (!file_name_retrieved)
				{
					_FileName = IOPath.GetFileName(Path);
					file_name_retrieved = true;
				}

				return _FileName;
			}
            set
            {
                if ((object)_Path == null || _Path.Length == 0)
                    Path = value;
                else
                {
                    var parent_path_string = IOPath.GetDirectoryName(Path);
                    Path = IOPath.Combine(parent_path_string, value);
                }
            }
		}

		string _Name;
		bool _name_retrieved;
        /// <summary>File name without extension</summary>
	    public string Name
		{
			get
			{
				if (!_name_retrieved)
				{
					_Name = IOPath.GetFileNameWithoutExtension(Path);
					_name_retrieved = true;
				}

				return _Name;
			}
		}

		string _Extension;
		bool extension_retrieved;
        /// <summary>File extension</summary>
	    public string Extension
		{
			get
			{
				if (!extension_retrieved)
				{
					_Extension = IOPath.GetExtension(Path);
					extension_retrieved = true;
				}

				return _Extension;
			}
		}

		PathInfo _Parent;
		bool parent_retrieved;
        /// <summary>Get parent path</summary>
	    public virtual PathInfo Parent
		{
            // TODO relative path Parent??? How do it ...?

			get
			{
				if (!parent_retrieved)
				{
					parent_retrieved = true;

					if ((object)_Path == null || _Path.Length == 0)
                    {
						_Parent = null;
                    }
					else
					{
                        // TODO relative path
						_Parent = new PathInfo(IOPath.GetDirectoryName(FullPath)) { Immutable = true };
					}
				}
					
				return _Parent;
			}
		}
        
        FileInfo _FileInfo;
		bool file_info_retrieved;
        public FileInfo FileInfo
		{
            get
            {
                if (!file_info_retrieved)
                {
                    _FileInfo = new FileInfo(FullPath);
                    file_info_retrieved = true;
                }
			    
                return _FileInfo;
            }
		}

        DirectoryInfo _DirectoryInfo;
		bool directory_info_retrieved;
        public DirectoryInfo DirectoryInfo
		{
            get
            {
                if (!directory_info_retrieved)
                {
                    _DirectoryInfo = new DirectoryInfo(FullPath);
                    directory_info_retrieved = true;
                }
			    
                return _DirectoryInfo;
            }
		}

        // From and to string conversion
        
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
		public static implicit operator PathInfo(string path)
        {
            return new PathInfo(path);
        }

        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
		public static implicit operator PathInfo(Environment.SpecialFolder special_folder)
        {
            return new PathInfo(Environment.GetFolderPath(special_folder));
        }

        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
		public static implicit operator string(PathInfo path_info)
		{
            if ((object)path_info == null)
                return null;

            return path_info.FullPath;
		}

        public override string ToString()
		{
            return FullPath;
		}

        // Combine / operator

        /// <summary> / - operator combines PathInfo and string to the new PathInfo</summary>
        public static PathInfo operator /(PathInfo path, string segment)
        {
            return path.Combine(segment);
        }

        // Equality comparers
        
        /// <summary>'Case-insensitive' hash code - hash code from FullPathUpperCase string.</summary>
        public override int GetHashCode()
        {
            var fullupper = FullPathUpperCase;
            return (fullupper == null) ? 0 : fullupper.GetHashCode();
        }

        /// <summary>Invariant ignore case equality of paths comparer. Empty (unassigned) PathInfo equal other empty PathInfo.</summary>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false; // Guidelines for Overloading Equals()

            // compare object references

            if ((object)this == obj)
                return true;

            // compare to PathInfo

            string other_path_string = null;

            var other_string = obj as string;
            if (other_string != null)
            {
                other_path_string = other_string;
            }
            else
            {
                var other = obj as PathInfo;
                if ((object)other != null)
                    other_path_string = other.FullPathUpperCase;              // Later this eliminate internal to upper case transformation in string.Equals
            }

            if (other_path_string == null)
                return true; // Guidelines for Overloading Equals()

            // compare to other path as string

            if (Empty)
                return false;

            return string.Equals(
                FullPathUpperCase,                              // Eliminate internal to upper case transformation
                other_path_string, 
                StringComparison.InvariantCultureIgnoreCase);   // Windows always treats file names and Universal Resource Identifiers as invariant
        }

        /// <summary>Invariant ignore case equality of paths comparer. Empty (unassigned) PathInfo equal other empty PathInfo.</summary>
        public bool Equals(PathInfo other)
        {
            var obj = (object)other;

            if (obj == null)
                return false; // Guidelines for Overloading Equals()

            // compare object references

            if ((object)this == obj)
                return true;

            // compare to PathInfo

            string other_path_string = other.FullPathUpperCase;              // Later this eliminate internal to upper case transformation in string.Equals

            if (other_path_string == null)
                return true; // Guidelines for Overloading Equals()

            // compare to other path as string

            if (Empty)
                return false;

            return string.Equals(
                FullPathUpperCase,                              // Eliminate internal to upper case transformation
                other_path_string, 
                StringComparison.InvariantCultureIgnoreCase);   // Windows always treats file names and Universal Resource Identifiers as invariant
        }

        /// <summary>Invariant ignore case equality of paths comparer. Empty (unassigned) PathInfo equal other empty PathInfo.</summary>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public static bool operator ==(PathInfo path1, PathInfo path2)
        {
            if ((object)path1 == null)
                return ((object)path2 == null || path2.Empty);

            if ((object)path1 == (object)path2)
                return true;

            return path1.Equals(path2);
        }

        /// <summary>Invariant ignore case inequality of paths comparer. Empty (unassigned) PathInfo equal other empty PathInfo.</summary>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public static bool operator !=(PathInfo path1, PathInfo path2)
        {
            if ((object)path1 == null)
                return ((object)path2 != null && !path2.Empty);

            return !path1.Equals(path2);
        }

        /// <summary>Invariant ignore case of paths comparer.</summary>
        public static bool operator >(PathInfo path1, PathInfo path2)
        {
            if ((object)path1 == null)
                return false;
            
            if ((object)path2 == null)
                return true;

            // Compare path as string is incorrect, compare segments of path separately at each level of the hierarchy is the right way

            var segments1 = path1.Segments;
            var segments2 = path2.Segments;

            if (segments2 == null)
                return (segments1 != null);
            else if (segments1 == null)
                return false;

            // compare common part of of these two paths

            for(int i = 0, c1 = segments1.Length, c2 = segments2.Length; i < c1 && i < c2; i++)
            {
                var comparsion_result = string.Compare(segments1[i], segments2[i]);
                if (comparsion_result > 0)
                    return true;
                else if (comparsion_result < 0)
                    return false;
            }

            //: common part are equivalent

            return segments1.Length > segments2.Length;
        }

        /// <summary>Invariant ignore case of paths comparer.</summary>
		public static bool operator <(PathInfo path1, PathInfo path2)
        {
            if ((object)path2 == null)
                return false;

            if ((object)path1 == null)
                return true;

            // compare path as string is incorrect, compare segments of path separately at each level of the hierarchy is the right way

            var segments1 = path1.Segments;
            var segments2 = path2.Segments;

            if (segments1 == null)
                return (segments2 != null);
            else if (segments2 == null)
                return false;

            // compare common part of of these two paths

            for(int i = 0, c1 = segments1.Length, c2 = segments2.Length; i < c1 && i < c2; i++)
            {
                var comparsion_result = string.Compare(segments1[i], segments2[i]);
                if (comparsion_result < 0)
                    return true;
                else if (comparsion_result > 0)
                    return false;
            }

            //: common part are equivalent

            return segments1.Length < segments2.Length;
        }

        /// <summary>Invariant ignore case equality of paths comparer.</summary>
		public int CompareTo(PathInfo other)
		{
            if ((object)other == null)
                return 1;

            if (other.Empty)
                return (Empty) ? 0 : 1;
            
            if (Empty)
                return -1;

            // Compare path as string is incorrect, compare segments of path separately at each level of the hierarchy is the right way

            var segments1 = this.Segments;
            var segments2 = other.Segments;

            // compare common part of of these two paths

            for(int i = 0, c1 = segments1.Length, c2 = segments2.Length; i < c1 && i < c2; i++)
            {
                int level_compare_result = string.Compare(segments1[i], segments2[i]);
                if (level_compare_result != 0)
                    return level_compare_result;
            }

            //: common part are equivalent

            return segments1.Length - segments2.Length;
		}

        /// <summary>Invariant ignore case equality of paths comparer.
        /// Comparison to the string path is correct, but will turn a decrease in performance!
        /// Comparison to values of other types returns 0.
        /// </summary>
		public int CompareTo(object obj)
		{
            if ((object)obj == null)
                return 1;

			var other = obj as PathInfo;
            if ((object)other != null)
                return CompareTo(other);

            throw new ArgumentException();
		}

        // Constructors

        /// <summary>Static factory method. Combine strings into a new PathInfo object.</summary>
        /// <param name="segments">Path segments</param>
        public static PathInfo Create(params string[] segments)
		{
			return new PathInfo(segments);
		}

        /// <summary>Combine PathInfo and strings into a new PathInfo object</summary>
        /// <param name="segments">Path segments</param>
		public PathInfo Combine(params string[] segments)
		{
			if ((object)segments == null || segments.Length == 0)
				return new PathInfo(Path);

            // precalc length for path buffer

			int len = (((object)_Path == null) ? 0 : _Path.Length);
			for (int i = 0, c = segments.Length; i < c; i++)
			{
				string seg = segments[i];

                // Allowed null path, null segment not allowed

                if ((object)seg == null || seg.Length == 0)
                    throw new ArgumentException("Path segment cannot be null or empty!");

				len += seg.Length + 1;
			}

            // build path

			var builder = new StringBuilder(len);
			bool opened = false;

			if ((object)_Path != null && _Path.Length > 0)
			{
				builder.Append(_Path);
                
                // if path ends with separator then does not add new separator
                char last_char = builder[builder.Length - 1];
				opened = (last_char != DirectorySeparatorChar && last_char != AltDirectorySeparatorChar);
			}

			for (int i = 0, c = segments.Length; i < c; i++)
			{
				string seg = segments[i];

				if (opened)
					builder.Append(IOPath.DirectorySeparatorChar);

				builder.Append(seg);
				opened = true;
			}

			return new PathInfo(builder.ToString());
		}

        public PathInfo()
		{
		}

        public PathInfo(Environment.SpecialFolder special_folder, Environment.SpecialFolderOption option = Environment.SpecialFolderOption.DoNotVerify)
		{
            Path = Environment.GetFolderPath(special_folder, option);
		}

		public PathInfo(PathInfo path)
		{
            if (path.IsRelative)
                Base = new PathInfo(path.Base);

            Path = path.Path;
		}

		public PathInfo(string path)
		{
			Path = path;
		}

		public PathInfo(string segment1, string segment2)
		{
            if ((object)segment1 == null || segment1.Length == 0
            || (object)segment2 == null || segment2.Length == 0)
                throw new ArgumentException("Path segment cannot be null or empty!");

			Path = segment1 + IOPath.DirectorySeparatorChar + segment2;
		}

        public PathInfo(string segment1, string segment2, string segment3)
		{
			if ((object)segment1 == null || segment1.Length == 0
            || (object)segment2 == null || segment2.Length == 0
            || (object)segment3 == null || segment3.Length == 0)
                throw new ArgumentException("Path segment cannot be null or empty!");

			Path = segment1 + IOPath.DirectorySeparatorChar + segment2 + IOPath.DirectorySeparatorChar + segment3;
		}

        public PathInfo(string segment1, string segment2, string segment3, string segment4)
		{
			if ((object)segment1 == null || segment1.Length == 0
            || (object)segment2 == null || segment2.Length == 0
            || (object)segment3 == null || segment3.Length == 0
            || (object)segment4 == null || segment4.Length == 0)
                throw new ArgumentException("Path segment cannot be null or empty!");

			Path = segment1 + IOPath.DirectorySeparatorChar + segment2 + IOPath.DirectorySeparatorChar + segment3 + IOPath.DirectorySeparatorChar + segment4;
		}

        public PathInfo(params string[] segments)
		{
            // precalc length for path buffer

			int len = (((object)_Path == null) ? 0 : _Path.Length);
			for (int i = 0, c = segments.Length; i < c; i++)
			{
				string seg = segments[i];

                // Allowed null path, null segment not allowed

                if ((object)seg == null || seg.Length == 0)
                    throw new ArgumentException("Path segment cannot be null or empty!");

				len += seg.Length + 1;
			}

            // build path

			var builder = new StringBuilder(len);
			bool opened = false;
			for (int i = 0, c = segments.Length; i < c; i++)
			{
				if (opened)
					builder.Append(IOPath.DirectorySeparatorChar);

				builder.Append(segments[i]);
				opened = true;
			}

            Path = builder.ToString();
		}

        // Serialization

        public PathInfo(SerializationInfo info, StreamingContext context)
		{
            string _path = null, _base = null, _relative = null;
            foreach(var item in info)
            {
                switch(item.Name)
                {
                    case "Base":
                        _base = (string)item.Value;
                        break;
                    case "RelativePath":
                        _relative = (string)item.Value;
                        break;
                    case "Path":
                        _path = (string)item.Value;
                        break;
                }
            }

            if (_base != null || _relative != null)
            {
                Base = _base;
                Path = _relative;
            }
            else
            {
                Path = _path;
            }

        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
            if (IsRelative)
            {
                if ((object)_Base != null)
                    info.AddValue("Base", _Base.FullPath);
                if ((object)_Path != null)
                    info.AddValue("RelativePath", _Path);
            }
            else
            {
                if (FullPath != null)
                    info.AddValue("Path", FullPath);
            }
        }

        // XML serialization

#region used_system_xml
#if used_system_xml

        public XmlSchema GetSchema()
        {
            return null;
        }
        
        public void ReadXml(XmlReader reader)
        {
            var content = reader.ReadString();
            if (content != null)
            {
                Path = content;
            }
            else
            {
                var _base = reader.ReadElementContentAsString("Base", null);
                var _rel = reader.ReadElementContentAsString("RelativePath", null);
                if (_base != null || _rel != null)
                {
                    Base = _base;
                    Path = _rel;
                }
            }
        }
        
        public void WriteXml(XmlWriter writer)
        {
            if (IsRelative)
            {
                
                if ((object)_Base != null)
                    writer.WriteElementString("Base", _Base.FullPath);
                    
                if ((object)_Path != null)
                    writer.WriteElementString("RelativePath", _Path);
            }
            else
            {
                writer.WriteString(FullPath);
            }
        }
		
#endif
#endregion used_system_xml
        
        // * ^ & child enumeration operators and other enumerators
               
        /// <summary>
        /// Returns an enumerable collection of file-system entries that match a search pattern in a specified path.
        /// </summary>
        /// <param name="search_pattern">The search string to match against the names of directories in path.</param>
        /// <exception cref="System.ArgumentException">path is a zero-length string, contains only white space, or contains invalid
        /// characters as defined by System.IO.Path.GetInvalidPathChars().- or -searchPattern
        /// does not contain a valid pattern.</exception>
        /// <exception cref="System.ArgumentNullException">path is null.-or-searchPattern is null.</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">path is invalid, such as referring to an unmapped drive.</exception>
        /// <exception cref="System.IO.IOException">path is a file name.</exception>
        /// <exception cref="System.IO.PathTooLongException">The specified path, file name, or combined exceed the system-defined maximum
        /// length. For example, on Windows-based platforms, paths must be less than
        /// 248 characters and file names must be less than 260 characters.</exception>
        /// <exception cref="System.Security.SecurityException">The caller does not have the required permission.</exception>
        /// <exception cref="System.UnauthorizedAccessException">The caller does not have the required permission.</exception>
        /// <returns>An enumerable collection of file-system entries in the directory specified by path and that match search_pattern.</returns>
        public static PathList operator *(PathInfo path, string search_pattern)
        {
            return path .FileSystemEntries(search_pattern) .ToList();
        }

        public static PathList operator *(PathInfo path, Func<string,bool> match_comparer)
        {
            return path .FileSystemEntries(match_comparer) .ToList();
        }

        /// <summary>
        /// Returns an enumerable collection of file-system entries that match a search pattern in a specified path.
        /// </summary>
        /// <param name="recursive">The value One of the values of the System.IO.SearchOption enumeration that specifies
        /// whether the search operation should include only the current directory or
        /// should include all subdirectories.The default value is System.IO.SearchOption.TopDirectoryOnly.</param>
        /// <exception cref="System.ArgumentException">path is a zero-length string, contains only white space, or contains invalid
        /// characters as defined by System.IO.Path.GetInvalidPathChars().- or -searchPattern
        /// does not contain a valid pattern.</exception>
        /// <exception cref="System.ArgumentNullException">path is null.-or-searchPattern is null.</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">path is invalid, such as referring to an unmapped drive.</exception>
        /// <exception cref="System.IO.IOException">path is a file name.</exception>
        /// <exception cref="System.IO.PathTooLongException">The specified path, file name, or combined exceed the system-defined maximum
        /// length. For example, on Windows-based platforms, paths must be less than
        /// 248 characters and file names must be less than 260 characters.</exception>
        /// <exception cref="System.Security.SecurityException">The caller does not have the required permission.</exception>
        /// <exception cref="System.UnauthorizedAccessException">The caller does not have the required permission.</exception>
        /// <returns>An enumerable collection of file-system entries in the directory specified by path and that match search_pattern.</returns>
        public PathList FileSystemEntries(bool recursive = false)
        {
            if (!recursive)
                return Directory
                    .EnumerateFileSystemEntries(FullPath)
                    .Select(path => new PathInfo(path))
                    .ToList();

            return Directory
                .EnumerateFileSystemEntries(FullPath, "*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                .Select(path => new PathInfo(path))
                .ToList();
        }

        /// <summary>
        /// Returns an enumerable collection of file-system entries that match a search pattern in a specified path.
        /// </summary>
        /// <param name="search_pattern">The search string to match against the names of directories in path.</param>
        /// <param name="recursive">The value One of the values of the System.IO.SearchOption enumeration that specifies
        /// whether the search operation should include only the current directory or
        /// should include all subdirectories.The default value is System.IO.SearchOption.TopDirectoryOnly.</param>
        /// <exception cref="System.ArgumentException">path is a zero-length string, contains only white space, or contains invalid
        /// characters as defined by System.IO.Path.GetInvalidPathChars().- or -searchPattern
        /// does not contain a valid pattern.</exception>
        /// <exception cref="System.ArgumentNullException">path is null.-or-searchPattern is null.</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">path is invalid, such as referring to an unmapped drive.</exception>
        /// <exception cref="System.IO.IOException">path is a file name.</exception>
        /// <exception cref="System.IO.PathTooLongException">The specified path, file name, or combined exceed the system-defined maximum
        /// length. For example, on Windows-based platforms, paths must be less than
        /// 248 characters and file names must be less than 260 characters.</exception>
        /// <exception cref="System.Security.SecurityException">The caller does not have the required permission.</exception>
        /// <exception cref="System.UnauthorizedAccessException">The caller does not have the required permission.</exception>
        /// <returns>An enumerable collection of file-system entries in the directory specified by path and that match search_pattern.</returns>
        public PathList FileSystemEntries(string search_pattern, bool recursive = false)
        {
            if (search_pattern == null)
            {
                if (!recursive)
                    return Directory
                        .EnumerateFileSystemEntries(FullPath)
                        .Select(path => new PathInfo(path))
                        .ToList();

                search_pattern = "*";
            }

            return Directory
                .EnumerateFileSystemEntries(FullPath, search_pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                .Select(path => new PathInfo(path))
                .ToList();
        }

        public PathList FileSystemEntries(Func<string,bool> match_comparer, bool recursive = false)
        {
            if (!recursive)
                return Directory
                    .EnumerateFileSystemEntries(FullPath)
                    .Where(path => match_comparer(path))
                    .Select(path => new PathInfo(path))
                    .ToList();

            return Directory
                .EnumerateFileSystemEntries(FullPath, "*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                .Where(path => match_comparer(path))
                .Select(path => new PathInfo(path))
                .ToList();
        }

        public static PathList operator ^(PathInfo path, string search_pattern)
        {
            return path .Directories(search_pattern) .ToList();
        }

        public static PathList operator ^(PathInfo path, Func<string,bool> match_comparer)
        {
            return path .Directories(match_comparer) .ToList();
        }

        public PathList Directories(bool recursive = false)
        {
            
            if (!recursive)
                return Directory
                    .EnumerateDirectories(FullPath)
                    .Select(path => new PathInfo(path))
                    .ToList();

            return Directory
                .EnumerateDirectories(FullPath, "*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                .Select(path => new PathInfo(path))
                .ToList();
        }

        public PathList Directories(string search_pattern, bool recursive = false)
        {
            if (search_pattern == null)
            {
                if (!recursive)
                    return Directory
                        .EnumerateDirectories(FullPath)
                        .Select(path => new PathInfo(path))
                    .ToList();

                search_pattern = "*";
            }

            return Directory
                .EnumerateDirectories(FullPath, search_pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                .Select(path => new PathInfo(path))
                    .ToList();
        }

        public PathList Directories(Func<string,bool> match_comparer, bool recursive = false)
        {

            if (!recursive)
                return Directory
                    .EnumerateDirectories(FullPath)
                    .Where(path => match_comparer(path))
                    .Select(path => new PathInfo(path))
                    .ToList();

            return Directory
                .EnumerateDirectories(FullPath, "*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                .Where(path => match_comparer(path))
                .Select(path => new PathInfo(path))
                    .ToList();
        }

        public static PathList operator &(PathInfo path, string search_pattern)
        {
            return path .Files(search_pattern) .ToList();
        }

        public static PathList operator &(PathInfo path, Func<string,bool> match_comparer)
        {
            return path .Files(match_comparer) .ToList();
        }

        public PathList Files(bool recursive = false)
        {
            if (!recursive)
                return Directory
                    .EnumerateFiles(FullPath)
                    .Select(path => new PathInfo(path))
                    .ToList();

            return Directory
                .EnumerateFiles(FullPath, "*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                .Select(path => new PathInfo(path))
                    .ToList();
        }

        public PathList Files(string search_pattern, bool recursive = false)
        {
            if (search_pattern == null)
            {
                if (!recursive)
                    return Directory
                        .EnumerateFiles(FullPath)
                        .Select(path => new PathInfo(path))
                    .ToList();

                search_pattern = "*";
            }

            return Directory
                .EnumerateFiles(FullPath, search_pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                .Select(path => new PathInfo(path))
                    .ToList();
        }

        public PathList Files(Func<string,bool> match_comparer, bool recursive = false)
        {
            if (!recursive)
                return Directory
                    .EnumerateFiles(FullPath)
                    .Where(path => match_comparer(path))
                    .Select(path => new PathInfo(path))
                    .ToList();

            return Directory
                .EnumerateFiles(FullPath, "*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                .Where(path => match_comparer(path))
                .Select(path => new PathInfo(path))
                    .ToList();
        }

        // Other

        public void Delete(bool recursive = false)
		{
            if (Directory.Exists(FullPath))
                Directory.Delete(FullPath, recursive);
            else
			    File.Delete(Path);
		}

        public void Rename(string new_name, bool overwrite_destination = false)
		{
            if (Immutable)
                throw new InvalidOperationException("This PathInfo object is immutable");

            if ((object)_Path == null || _Path.Length == 0)
                throw new InvalidOperationException("Path or relative path is empty");

            string new_path = IOPath.Combine(Parent.FullPath, new_name);

            // overwriting destination

            bool is_file = false;
            bool is_directory = DirectoryExists();
            if (!is_directory)
                is_file = FileExists();

            if (Directory.Exists(new_path))
            {
                if (is_file)
                {
                    // Rename the file but exists directory

                    throw new InvalidOperationException("There is a directory with the same name!");
                }

                if (!overwrite_destination)
                    throw new InvalidOperationException("Directory already exists!");

                Directory.Delete(new_path, true);
            }
            else if (File.Exists(new_path))
            {
                if (is_directory)
                {
                    // Rename the directory but exists file

                    throw new InvalidOperationException("There is a file with the same name!");
                }

                if (!overwrite_destination)
                    throw new InvalidOperationException("File already exists!");

                File.Delete(new_path);
            }

            if (Directory.Exists(FullPath))
                Directory.Move(FullPath, new_path);
            else
			    File.Move(FullPath, new_path);

            // renaming succeeded, now rename this PathInfo object:

            FileName = new_name;
		}
        
        public static bool MatchesMaskComparer(string text, string _mask)
        {
            // mask wildcards * and ?
            
            if ((object)text == null || text.Length == 0)
                return false;

            var mask = _mask.ToCharArray();
            int text_scan = 0; // index of char in 'text' string
            int count = text.Length;

            for(int i = 0, c = mask.Length - 1; i <= c; i++)
            {
                if (text_scan >= count)
                    return false;

                char maskchar = mask[i];
                
                switch(maskchar)
                {
                    case '?':
                        text_scan++; // skip one char in text, match preserved
                        continue;

                    case '*': // current is '*'

                        if (i == c)
                            return true; // '*' - is the last char in mask

                        char next_mask_char = mask[i+1];
                        if (next_mask_char == '*')
                        {
                            continue; // skip from current '*' to next '*'
                        }
                        else if (next_mask_char == '?')
                        {
                            mask[i+1] = '*';
                            continue; // set '*' to next char and skip to this char
                        }

                        text_scan = text.IndexOf(next_mask_char, text_scan);
                        if (text_scan < 0)
                            return false; // not match

                        continue;

                    default: // any char
                        if (maskchar == text[text_scan])
                        {
                            text_scan++; // skip one char in text, match preserved
                            continue;
                        }
                        else
                            return false; // not match
                }

                // search char matches


            }


            // Not implemented yet
            return true;
        }

        public bool RegexIsMatch(string pattern, Text.RegularExpressions.RegexOptions options = Text.RegularExpressions.RegexOptions.CultureInvariant | Text.RegularExpressions.RegexOptions.IgnoreCase)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(FullPath, pattern, options);
        }

        public bool RegexFileNameIsMatch(string pattern, Text.RegularExpressions.RegexOptions options = Text.RegularExpressions.RegexOptions.CultureInvariant | Text.RegularExpressions.RegexOptions.IgnoreCase)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(FileName, pattern, options);
        }

        public static string ValidateFileName(string _name)
		{
            // Преобразовать текст к корректному для имени файла значению, заменить спецсимволы

            StringBuilder name = new StringBuilder(_name);

            for(int i = name.Length - 1; i >= 0; i--)
            if (InvalidFileNameChars.Contains(name[i]))
                name[i] = '-';

            return name.ToString();
        }

		public static bool CheckFileNameValidity(string name)
		{
			if (string.IsNullOrWhiteSpace(name))
				return false;

			if (name.IndexOfAny(InvalidFileNameChars) >= 0)
				return false;

			return true;
		}


        // Special folders /////////////////////////////////////////////////////////////////////////////////////////////


        public static PathInfo TEMP
        {
            get { return new PathInfo(IOPath.GetTempPath()); }
        }

        public static PathInfo APPLICATION_DATA_ROAMING
        {
            get { return new PathInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)); }
        }

        public static PathInfo APPLICATION_DATA_LOCAL
        {
            get { return new PathInfo(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)); }
        }

        public static PathInfo DESKTOP_DIRECTORY
        {
            get { return new PathInfo(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)); }
        }

        public static PathInfo USER
        {
            get { return new PathInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)); }
        }


        // Name generators /////////////////////////////////////////////////////////////////////////////////////////////
        

        /// <summary>Create random path.</summary>
        /// <param name="name_pattern">Can contains {0} wildcard</param>
        /// <param name="extension">File or directory name extension</param>
        /// <param name="force_unique">To validate the uniqueness the path in the file system</param>
        /// <returns></returns>
        public PathInfo GenerateRandom(string name_pattern = null, string extension = null, bool force_unique = true)
        {
            bool name_pattern_formattable = (name_pattern != null && name_pattern.Contains("{0}"));

            var chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ123456789090"; // length 64
            var builder = new StringBuilder(((name_pattern == null) ? 0 : name_pattern.Length) + ((extension == null) ? 0 : extension.Length) + 7);
            
            builder.Append(name_pattern);

            // generate random key

            var rnd = new Random(Environment.TickCount);
            for(int i = 0; i < 6; i++)
                builder.Append(chars[rnd.Next(63)]);
            
            // append extension

            if (extension != null)
            {
                if (!extension.StartsWith("."))
                    builder.Append('.');

                builder.Append(extension);
            }

            var name = (name_pattern_formattable) ? string.Format(name_pattern, builder.ToString()) : builder.ToString();

            // uniqueness check file path

            if (force_unique)
            {
                // Retry generate if finded matching name

                if (FileSystemEntries(name).Any())
                    return GenerateRandom(name_pattern, extension, force_unique);
            }

            return Combine(name);
        }

        /// <summary>Create a number of random paths.</summary>
        /// <param name="number"></param>
        /// <param name="name_pattern"></param>
        /// <param name="extension"></param>
        /// <param name="force_unique"></param>
        /// <returns></returns>
        public PathList GenerateManyRandom(int number, string name_pattern = null, string extension = null, bool force_unique = true)
        {
            var result = new PathList(number);
            
            for(int i = 0; i < number; i++)
                result.Add(GenerateRandom(name_pattern, extension, force_unique));

            return result;
        }

        public PathList GenerateManyNumbered(int start, int number,  string name_pattern = null, string extension = null, bool force_unique = true)
        {
            var result = new PathList(number);
            bool name_pattern_formattable = (name_pattern != null && name_pattern.Contains("{0}"));
            
            for(int i = start, c = start + number; i < c; i++)
            {
                if (name_pattern_formattable)
                    result.Add(Combine(string.Format(name_pattern, i.ToString()) + extension));
                else
                    result.Add(Combine(name_pattern + " " + i.ToString() + extension));
            }

            return result;
        }

        /// <summary>Generate new unique name</summary>
        /// <param name="name_pattern">Valid file name chars only</param>
        /// <param name="extension"></param>
        /// <param name="force_unique"></param>
        /// <returns></returns>
        public PathInfo GenerateNewName(string name_pattern = null, string extension = null)
        {
            // Validate name

            var name_filter = new StringBuilder();
            var chars = new string(InvalidFileNameChars);
            for(int i = 0, c = name_pattern.Length; i < c; i++)
            {
                char chr = name_pattern[i];
                if (Array.BinarySearch<char>(InvalidFileNameChars, chr) < 0)
                    name_filter.Append(chr);
            }

            string name_base = name_filter.ToString();

            // Check name_pattern + extension file name

            string simple_filename = name_base + extension;

            if (!FileSystemEntries(simple_filename).Any()) // File system entries with name_pattern+extension not exists
                return Combine(simple_filename);
            
            // that name already exists
            
            int biggest_number = 0;

            string check_pattern = name_base;
            if (check_pattern.Length > 0 && !check_pattern.EndsWith(" "))
                check_pattern += " ";
            check_pattern += "(*)" + extension;

			foreach(string path in Directory.EnumerateFileSystemEntries(FullPath, check_pattern))
			{
                string fname = IOPath.GetFileName(path);
                int left_parenthesis = fname.LastIndexOf('(');
                int right_parenthesis = fname.LastIndexOf(')');
                string probably_number = fname.Substring(left_parenthesis + 1, right_parenthesis - left_parenthesis - 1);
                int coll_number = 0;
                if (int.TryParse(probably_number, out coll_number))
                {
                    if (coll_number >= biggest_number)
                        biggest_number = coll_number;
                }
			}

            return Combine(check_pattern.Replace("*", (biggest_number + 1).ToString()));
        }

        // System.IO.Path /////////////////////////////////////////////////////////////////////////////////////////////


        /// <summary>Gets the root directory information of the specified path.</summary>
        /// <exception cref="System.ArgumentException">path contains one or more of the invalid characters defined in System.IO.Path.GetInvalidPathChars().-or-
        /// System.String.Empty was passed to path.</exception>
        /// <returns>The root directory of path, such as "C:\", or null if path is null, or an empty string if path does not contain root directory information.</returns>
        public PathInfo PathRoot
        {
            get
            {
                string path = ((object)_Base != null) ? _Base.FullPath : _Path;
            
                if ((object)path == null || path.Length == 0)
                    return null;

                return new PathInfo(IOPath.GetPathRoot(path));
            }
        }

        /// <summary>Gets a value indicating whether the specified path string contains a root.
        /// true if path contains a root; otherwise, false.</summary>
        /// <exception cref="System.ArgumentException">path contains one or more of the invalid characters defined in System.IO.Path.GetInvalidPathChars().</exception>
        public bool PathIsRooted
        {
            get
            {
                string path = ((object)_Base != null) ? _Base.FullPath : _Path;
            
                if ((object)path == null || path.Length == 0)
                    return false;

                return IOPath.IsPathRooted(path);
            }
        }
        
        public PathList PathGetLogicalDrives()
        {
            return new PathList(Directory.GetLogicalDrives());
        }

        // TODO Additional Path methods ///////////////////////////////////////////////////////////////////////////////

        /**/

        // TODO System.IO.File ////////////////////////////////////////////////////////////////////////////////////////


        /// <summary>Appends lines to a file, and then closes the file. The file is created if it does not already exist.</summary>
        /// <param name="contents">The lines to append to the file.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <exception cref="System.ArgumentException">path is a zero-length string, contains only white space, or contains one
        /// more invalid characters defined by the System.IO.Path.GetInvalidPathChars() method.</exception>
        /// <exception cref="System.ArgumentNullException">Either path or contents is null.</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">path is invalid (for example, it is on an unmapped drive).</exception>
        /// <exception cref="System.IO.FileNotFoundException">The file specified by path was not found.</exception>
        /// <exception cref="System.IO.IOException">An I/O error occurred while opening the file.</exception>
        /// <exception cref="System.IO.PathTooLongException">path exceeds the system-defined maximum length. For example, on Windows-based
        /// platforms, paths must be less than 248 characters and file names must be less than 260 characters.</exception>
        /// <exception cref="System.NotSupportedException">path is in an invalid format.</exception>
        /// <exception cref="System.Security.SecurityException">The caller does not have permission to write to the file.</exception>
        /// <exception cref="System.UnauthorizedAccessException">path specifies a file that is read-only.-or-This operation is not supported
        /// on the current platform.-or-path is a directory.-or-The caller does not have the required permission.</exception>
        public PathInfo FileAppendAllLines(IEnumerable<string> contents, Encoding encoding = null)
		{
            if (encoding == null)
                encoding = Encoding.UTF8; // default UTF-8

			File.AppendAllLines(FullPath, contents, encoding);
            return this;
		}

        public PathInfo FileAppendAllLines(string file_name, IEnumerable<string> contents, Encoding encoding = null)
		{
            if (encoding == null)
                encoding = Encoding.UTF8; // default UTF-8

            string full_path = IOPath.Combine(FullPath, file_name);

			File.AppendAllLines(full_path, contents, encoding);

            return new PathInfo(full_path);
		}

        public PathInfo FileAppendAllText(string contents, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8; // default UTF-8

			File.AppendAllText(FullPath, contents, encoding);
            return this;
		}

        public PathInfo FileAppendAllText(string file_name, string contents, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8; // default UTF-8

            string full_path = IOPath.Combine(FullPath, file_name);

			File.AppendAllText(full_path, contents, encoding);
            
            return new PathInfo(full_path);
		}
        
        public StreamWriter FileAppendTextStreamWriter(Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8; // default UTF-8

            return new StreamWriter(FullPath, true, encoding);
        }

        public StreamWriter FileAppendTextStreamWriter(string file_name, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8; // default UTF-8

            return new StreamWriter(IOPath.Combine(FullPath, file_name), true, encoding);
        }

		public PathInfo FileCopy(PathInfo destination_file_path, bool overwrite = false)
		{
			File.Copy(FullPath, destination_file_path, overwrite);
			return destination_file_path;
		}

        public PathInfo FileCopy(string file_name, PathInfo destination_file_path, bool overwrite = false)
		{
            string full_path = IOPath.Combine(FullPath, file_name);
            File.Copy(full_path, destination_file_path, overwrite);
			return destination_file_path;
		}

        public FileStream FileCreate(string file_name = null)
		{
			return (file_name == null) ? File.Create(FullPath) : File.Create(IOPath.Combine(FullPath, file_name));
		}

        public FileStream FileCreate(int buffer_size, FileOptions options, FileSecurity file_security = null)
		{
            if (file_security == null)
			    return File.Create(FullPath, buffer_size, options);
            else
                return File.Create(FullPath, buffer_size, options, file_security);
		}

        public FileStream FileCreate(string file_name, int buffer_size, FileOptions options, FileSecurity file_security = null)
        {
            string full_path = (file_name == null) ? FullPath : IOPath.Combine(FullPath, file_name);

            if (file_security == null)
			    return File.Create(full_path, buffer_size, options);
            else
                return File.Create(full_path, buffer_size, options, file_security);
        }

        public StreamWriter FileCreateTextStreamWriter(bool overwrite = true, Encoding encoding = null)
        {
            if (File.Exists(FullPath) && !overwrite)
                throw new IOException("File already exists!");

            if (encoding == null)
                encoding = Encoding.UTF8; // default UTF-8

            return new StreamWriter(FullPath, false, encoding);
        }

        public StreamWriter FileCreateTextStreamWriter(string file_name, bool overwrite = true, Encoding encoding = null)
        {
            return new PathInfo(IOPath.Combine(FullPath, file_name)) .FileCreateTextStreamWriter(overwrite, encoding);
        }

        public StreamReader FileCreateTextStreamReader(Encoding encoding = null, bool detectEncodingFromByteOrderMarks = false, int bufferSize = 0)
        {
            if (encoding == null)
                encoding = Encoding.UTF8; // default UTF-8

            return (bufferSize == 0)
                ? new StreamReader(FullPath, encoding, detectEncodingFromByteOrderMarks)
                : new StreamReader(FullPath, encoding, detectEncodingFromByteOrderMarks, bufferSize);
        }

        public StreamReader FileCreateTextStreamReader(string file_name, Encoding encoding = null, bool detectEncodingFromByteOrderMarks = false, int bufferSize = 0)
        {
            if (encoding == null)
                encoding = Encoding.UTF8; // default UTF-8

            string full_path =  IOPath.Combine(FullPath, file_name);

            return (bufferSize == 0)
                ? new StreamReader(full_path, encoding, detectEncodingFromByteOrderMarks)
                : new StreamReader(full_path, encoding, detectEncodingFromByteOrderMarks, bufferSize);
        }

        public PathInfo FileDecrypt(string file_name = null)
		{
            if (file_name == null)
            {
                File.Decrypt(FullPath);
                return this;
            }
            else
            {
                string full_path =  IOPath.Combine(FullPath, file_name);
			    File.Decrypt(full_path);
                return new PathInfo(full_path);
            }
		}

        public PathInfo FileDelete(string file_name = null)
		{
            if (file_name == null)
            {
                File.Delete(FullPath);
                return this;
            }
            else
            {
                string full_path =  IOPath.Combine(FullPath, file_name);
			    File.Delete(full_path);
                return new PathInfo(full_path);
            }
		}

        public PathInfo FileEncrypt(string file_name = null)
		{
            if (file_name == null)
            {
                File.Encrypt(FullPath);
                return this;
            }
            else
            {
                string full_path =  IOPath.Combine(FullPath, file_name);
			    File.Encrypt(full_path);
                return new PathInfo(full_path);
            }
		}

        /// <summary>(There are no exceptions!) Determines whether the specified file exists.</summary>
        /// <returns>true if the caller has the required permissions and path contains the name
        ///     of an existing file; otherwise, false. This method also returns false if
        ///     path is null, an invalid path, or a zero-length string. If the caller does
        ///     not have sufficient permissions to read the specified file, no exception
        ///     is thrown and the method returns false regardless of the existence of path.</returns>
		public bool FileExists(string file_name = null)
		{
            string full_path = (file_name == null) ? FullPath : IOPath.Combine(FullPath, file_name);
			return File.Exists(full_path);
		}

        /// <summary>Gets a System.Security.AccessControl.FileSecurity object that encapsulates
        /// the specified type of access control list (ACL) entries for a particular file.
        /// 
        /// </summary>
        /// <param name="include_sections">One of the System.Security.AccessControl.AccessControlSections values that
        /// specifies the type of access control list (ACL) information to receive.</param>
        /// <returns>A System.Security.AccessControl.FileSecurity object that encapsulates the
        ///     access control rules for the file described by the path parameter.</returns>
        ///     <exception cref="System.IO.IOException">An I/O error occurred while opening the file.</exception>
        ///     <exception cref="System.Runtime.InteropServices.SEHException"></exception>
        ///     <exception cref="The path is null."></exception>
        ///     <exception cref="System.SystemException">The file could not be found.</exception>
        ///     <exception cref="System.UnauthorizedAccessException">The path parameter specified a file that is read-only.-or- This operation
        /// is not supported on the current platform.-or- The path parameter specified
        /// a directory.-or- The caller does not have the required permission.</exception>
        public FileSecurity FileGetAccessControl(AccessControlSections include_sections = AccessControlSections.All)
        {
            return File.GetAccessControl(FullPath, include_sections);
        }

        public FileAttributes FileGetAttributes(string file_name = null)
        {
            string full_path = (file_name == null) ? FullPath : IOPath.Combine(FullPath, file_name);
			return File.GetAttributes(full_path);
        }

        public DateTime FileGetCreationTime(string file_name = null)
        {
            string full_path = (file_name == null) ? FullPath : IOPath.Combine(FullPath, file_name);
			return File.GetCreationTime(full_path);
        }

        public DateTime FileGetCreationTimeUtc(string file_name = null)
        {
            string full_path = (file_name == null) ? FullPath : IOPath.Combine(FullPath, file_name);
			return File.GetCreationTimeUtc(full_path);
        }

        public DateTime FileGetLastAccessTime(string file_name = null)
        {
            string full_path = (file_name == null) ? FullPath : IOPath.Combine(FullPath, file_name);
			return File.GetLastAccessTime(full_path);
        }

        public DateTime FileGetLastAccessTimeUtc(string file_name = null)
        {
            string full_path = (file_name == null) ? FullPath : IOPath.Combine(FullPath, file_name);
			return File.GetLastAccessTimeUtc(full_path);
        }

        public DateTime FileGetLastWriteTime(string file_name = null)
        {
            string full_path = (file_name == null) ? FullPath : IOPath.Combine(FullPath, file_name);
			return File.GetLastWriteTime(full_path);
        }

        public DateTime FileGetLastWriteTimeUtc(string file_name = null)
        {
            string full_path = (file_name == null) ? FullPath : IOPath.Combine(FullPath, file_name);
			return File.GetLastWriteTimeUtc(full_path);
        }
        
        public PathInfo FileMove(PathInfo destination_file_path)
		{
            // Перенос файла, целевой путь - файл
			File.Move(FullPath, destination_file_path);
			return destination_file_path;
		}

        public FileStream FileOpen(FileMode mode = FileMode.Open, FileAccess access = FileAccess.Read, FileShare share = FileShare.Read)
		{
			return File.Open(FullPath, mode, access, share);
		}

        public byte[] FileReadAllBytes(string file_name = null)
        {
            string full_path = (file_name == null) ? FullPath : IOPath.Combine(FullPath, file_name);
			return File.ReadAllBytes(full_path);
        }

        public string[] FileReadAllLines(Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8; // default UTF-8

			return File.ReadAllLines(FullPath, encoding);
        }

        public string[] FileReadAllLines(string file_name, Encoding encoding = null)
        {
			if (encoding == null)
                encoding = Encoding.UTF8; // default UTF-8

			return File.ReadAllLines(IOPath.Combine(FullPath, file_name), encoding);
        }

        public string FileReadAllText(Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8; // default UTF-8

			return File.ReadAllText(FullPath, encoding);
        }

        public string FileReadAllText(string file_name, Encoding encoding = null)
        {
			if (encoding == null)
                encoding = Encoding.UTF8; // default UTF-8

			return File.ReadAllText(IOPath.Combine(FullPath, file_name), encoding);
        }
        
        // On very large files, ReadLines can be more efficient

        public IEnumerable<string> FileReadLines(Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8; // default UTF-8

			return File.ReadLines(FullPath, encoding);
        }

        public IEnumerable<string> FileReadLines(string file_name, Encoding encoding = null)
        {
			if (encoding == null)
                encoding = Encoding.UTF8; // default UTF-8

			return File.ReadLines(IOPath.Combine(FullPath, file_name), encoding);
        }

        public PathInfo FileReplace(PathInfo destination_file_path, PathInfo destination_backup_file_path)
        {
            File.Replace(FullPath, destination_file_path, destination_backup_file_path);
            return this;
        }

        /// <summary>Source, replace file and backup in same directory</summary>
        /// <param name="file_name"></param>
        /// <param name="destination_file_name"></param>
        /// <param name="destination_backup_file_name"></param>
        /// <returns></returns>
        public PathInfo FileReplace(string file_name, string destination_file_name, string destination_backup_file_name)
        {
            File.Replace(IOPath.Combine(FullPath, file_name), IOPath.Combine(FullPath, destination_file_name), IOPath.Combine(FullPath, destination_backup_file_name));
            return this;
        }

        // TODO public static void Replace(string sourceFileName, string destinationFileName, string destinationBackupFileName, bool ignoreMetadataErrors);

        public PathInfo FileSetAccessControl(FileSecurity fileSecurity)
        {
            File.SetAccessControl(FullPath, fileSecurity);
            return this;
        }

        public PathInfo FileSetAccessControl(string file_name, FileSecurity fileSecurity)
        {
            string full_path = IOPath.Combine(FullPath, file_name);
            File.SetAccessControl(full_path, fileSecurity);
            return new PathInfo(full_path);
        }

        public PathInfo FileSetAttributes(FileAttributes file_attributes)
        {
            File.SetAttributes(FullPath, file_attributes);
            return this;
        }

        public PathInfo FileSetAttributes(string file_name, FileAttributes file_attributes)
        {
            string full_path = IOPath.Combine(FullPath, file_name);
            File.SetAttributes(full_path, file_attributes);
            return new PathInfo(full_path);
        }

        public PathInfo FileSetCreationTime(DateTime creationTime)
        {
            File.SetCreationTime(FullPath, creationTime);
            return this;
        }

        public PathInfo FileSetCreationTime(string file_name, DateTime creationTime)
        {
            string full_path = IOPath.Combine(FullPath, file_name);
            File.SetCreationTime(full_path, creationTime);
            return new PathInfo(full_path);
        }

        public PathInfo FileSetCreationTimeUtc(DateTime creationTime)
        {
            File.SetCreationTimeUtc(FullPath, creationTime);
            return this;
        }

        public PathInfo FileSetCreationTimeUtc(string file_name, DateTime creationTime)
        {
            string full_path = IOPath.Combine(FullPath, file_name);
            File.SetCreationTimeUtc(full_path, creationTime);
            return new PathInfo(full_path);
        }

        public PathInfo FileSetLastAccessTime(DateTime creationTime)
        {
            File.SetLastAccessTime(FullPath, creationTime);
            return this;
        }

        public PathInfo FileSetLastAccessTime(string file_name, DateTime creationTime)
        {
            string full_path = IOPath.Combine(FullPath, file_name);
            File.SetLastAccessTime(full_path, creationTime);
            return new PathInfo(full_path);
        }

        public PathInfo FileSetLastAccessTimeUtc(DateTime creationTime)
        {
            File.SetLastAccessTimeUtc(FullPath, creationTime);
            return this;
        }

        public PathInfo FileSetLastAccessTimeUtc(string file_name, DateTime creationTime)
        {
            string full_path = IOPath.Combine(FullPath, file_name);
            File.SetLastAccessTimeUtc(full_path, creationTime);
            return new PathInfo(full_path);
        }


        public PathInfo FileSetLastWriteTime(DateTime creationTime)
        {
            File.SetLastWriteTime(FullPath, creationTime);
            return this;
        }

        public PathInfo FileSetLastWriteTime(string file_name, DateTime creationTime)
        {
            string full_path = IOPath.Combine(FullPath, file_name);
            File.SetLastWriteTime(full_path, creationTime);
            return new PathInfo(full_path);
        }

        public PathInfo FileSetLastWriteTimeUtc(DateTime creationTime)
        {
            File.SetLastWriteTimeUtc(FullPath, creationTime);
            return this;
        }

        public PathInfo FileSetLastWriteTimeUtc(string file_name, DateTime creationTime)
        {
            string full_path = IOPath.Combine(FullPath, file_name);
            File.SetLastWriteTimeUtc(full_path, creationTime);
            return new PathInfo(full_path);
        }

        //
        // Summary:
        //     Creates a new file, writes the specified byte array to the file, and then
        //     closes the file. If the target file already exists, it is overwritten.
        //
        // Parameters:
        //   path:
        //     The file to write to.
        //
        //   bytes:
        //     The bytes to write to the file.
        //
        // Exceptions:
        //   System.ArgumentException:
        //     path is a zero-length string, contains only white space, or contains one
        //     or more invalid characters as defined by System.IO.Path.InvalidPathChars.
        //
        //   System.ArgumentNullException:
        //     path is null or the byte array is empty.
        //
        //   System.IO.PathTooLongException:
        //     The specified path, file name, or both exceed the system-defined maximum
        //     length. For example, on Windows-based platforms, paths must be less than
        //     248 characters, and file names must be less than 260 characters.
        //
        //   System.IO.DirectoryNotFoundException:
        //     The specified path is invalid (for example, it is on an unmapped drive).
        //
        //   System.IO.IOException:
        //     An I/O error occurred while opening the file.
        //
        //   System.UnauthorizedAccessException:
        //     path specified a file that is read-only.-or- This operation is not supported
        //     on the current platform.-or- path specified a directory.-or- The caller does
        //     not have the required permission.
        //
        //   System.IO.FileNotFoundException:
        //     The file specified in path was not found.
        //
        //   System.NotSupportedException:
        //     path is in an invalid format.
        //
        //   System.Security.SecurityException:
        //     The caller does not have the required permission.
        public PathInfo FileWriteAllBytes(byte[] bytes)
        {
            File.WriteAllBytes(FullPath, bytes);
            return this;
        }

        public PathInfo FileWriteAllBytes(string file_name, byte[] bytes)
        {
            string full_path = IOPath.Combine(FullPath, file_name);
            File.WriteAllBytes(full_path, bytes);
            return new PathInfo(full_path);
        }

        //
        // Summary:
        //     Creates a new file, writes a collection of strings to the file, and then
        //     closes the file.
        //
        // Parameters:
        //   path:
        //     The file to write to.
        //
        //   contents:
        //     The lines to write to the file.
        //
        // Exceptions:
        //   System.ArgumentException:
        //     path is a zero-length string, contains only white space, or contains one
        //     or more invalid characters defined by the System.IO.Path.GetInvalidPathChars()
        //     method.
        //
        //   System.ArgumentNullException:
        //     Either path or contents is null.
        //
        //   System.IO.DirectoryNotFoundException:
        //     path is invalid (for example, it is on an unmapped drive).
        //
        //   System.IO.FileNotFoundException:
        //     The file specified by path was not found.
        //
        //   System.IO.IOException:
        //     An I/O error occurred while opening the file.
        //
        //   System.IO.PathTooLongException:
        //     path exceeds the system-defined maximum length. For example, on Windows-based
        //     platforms, paths must be less than 248 characters and file names must be
        //     less than 260 characters.
        //
        //   System.NotSupportedException:
        //     path is in an invalid format.
        //
        //   System.Security.SecurityException:
        //     The caller does not have the required permission.
        //
        //   System.UnauthorizedAccessException:
        //     path specifies a file that is read-only.-or-This operation is not supported
        //     on the current platform.-or-path is a directory.-or-The caller does not have
        //     the required permission.
        public PathInfo FileWriteAllLines(IEnumerable<string> contents, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8; // default UTF-8

            File.WriteAllLines(FullPath, contents, encoding);
            return this;
        }

        public PathInfo FileWriteAllLines(string file_name, IEnumerable<string> contents, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8; // default UTF-8

            string full_path = IOPath.Combine(FullPath, file_name);
            File.WriteAllLines(full_path, contents, encoding);
            return new PathInfo(full_path);
        }

        //
        // Summary:
        //     Creates a new file, write the specified string array to the file, and then
        //     closes the file.
        //
        // Parameters:
        //   path:
        //     The file to write to.
        //
        //   contents:
        //     The string array to write to the file.
        //
        // Exceptions:
        //   System.ArgumentException:
        //     path is a zero-length string, contains only white space, or contains one
        //     or more invalid characters as defined by System.IO.Path.InvalidPathChars.
        //
        //   System.ArgumentNullException:
        //     Either path or contents is null.
        //
        //   System.IO.PathTooLongException:
        //     The specified path, file name, or both exceed the system-defined maximum
        //     length. For example, on Windows-based platforms, paths must be less than
        //     248 characters, and file names must be less than 260 characters.
        //
        //   System.IO.DirectoryNotFoundException:
        //     The specified path is invalid (for example, it is on an unmapped drive).
        //
        //   System.IO.IOException:
        //     An I/O error occurred while opening the file.
        //
        //   System.UnauthorizedAccessException:
        //     path specified a file that is read-only.-or- This operation is not supported
        //     on the current platform.-or- path specified a directory.-or- The caller does
        //     not have the required permission.
        //
        //   System.IO.FileNotFoundException:
        //     The file specified in path was not found.
        //
        //   System.NotSupportedException:
        //     path is in an invalid format.
        //
        //   System.Security.SecurityException:
        //     The caller does not have the required permission.
        public PathInfo FileWriteAllLines(string[] contents, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8; // default UTF-8

            File.WriteAllLines(FullPath, contents, encoding);
            return this;
        }

        public PathInfo FileWriteAllLines(string file_name, string[] contents, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8; // default UTF-8

            string full_path = IOPath.Combine(FullPath, file_name);
            File.WriteAllLines(full_path, contents, encoding);
            return new PathInfo(full_path);
        }

        //
        // Summary:
        //     Creates a new file, writes the specified string to the file using the specified
        //     encoding, and then closes the file. If the target file already exists, it
        //     is overwritten.
        //
        // Parameters:
        //   path:
        //     The file to write to.
        //
        //   contents:
        //     The string to write to the file.
        //
        //   encoding:
        //     The encoding to apply to the string.
        //
        // Exceptions:
        //   System.ArgumentException:
        //     path is a zero-length string, contains only white space, or contains one
        //     or more invalid characters as defined by System.IO.Path.InvalidPathChars.
        //
        //   System.ArgumentNullException:
        //     path is null or contents is empty.
        //
        //   System.IO.PathTooLongException:
        //     The specified path, file name, or both exceed the system-defined maximum
        //     length. For example, on Windows-based platforms, paths must be less than
        //     248 characters, and file names must be less than 260 characters.
        //
        //   System.IO.DirectoryNotFoundException:
        //     The specified path is invalid (for example, it is on an unmapped drive).
        //
        //   System.IO.IOException:
        //     An I/O error occurred while opening the file.
        //
        //   System.UnauthorizedAccessException:
        //     path specified a file that is read-only.-or- This operation is not supported
        //     on the current platform.-or- path specified a directory.-or- The caller does
        //     not have the required permission.
        //
        //   System.IO.FileNotFoundException:
        //     The file specified in path was not found.
        //
        //   System.NotSupportedException:
        //     path is in an invalid format.
        //
        //   System.Security.SecurityException:
        //     The caller does not have the required permission.
        public PathInfo FileWriteAllText(string contents, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8; // default UTF-8

            File.WriteAllText(FullPath, contents, encoding);

            return this;
        }

        public PathInfo FileWriteAllText(string file_name, string contents, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8; // default UTF-8

            string full_path = IOPath.Combine(FullPath, file_name);
            File.WriteAllText(full_path, contents, encoding);
            return new PathInfo(full_path);
        }


        // TODO Additional File methods. //////////////////////////////////////////////////////////////////////////////


        /// <summary>(The method does not throw any exceptions!)
        /// 
        /// </summary>
        /// <returns></returns>
        public bool TryFileDelete(string file_name = null)
		{
            string full_path = (file_name == null) ? FullPath : IOPath.Combine(FullPath, file_name);
            try
            {
			    File.Delete(full_path);
			    return true;
            }
            catch
            {
            }

            return false;
		}

        
        // TODO System.IO.Directory ///////////////////////////////////////////////////////////////////////////////////


        /// <summary>Creates all directories and subdirectories in the specified path.</summary>
        /// <exception cref="System.IO.IOException">The directory specified by path is a file .-or-The network name is not known.</exception>
        /// <exception cref="System.UnauthorizedAccessException">The caller does not have the required permission.</exception>
        /// <exception cref="System.ArgumentException">path is a zero-length string, contains only white space, or contains one
        /// or more invalid characters as defined by System.IO.Path.InvalidPathChars.-or-path
        /// is prefixed with, or contains only a colon character (:).</exception>
        /// <exception cref="System.ArgumentNullException">path is null.</exception>
        /// <exception cref="System.IO.PathTooLongException"> The specified path, file name, or both exceed the system-defined maximum
        /// length. For example, on Windows-based platforms, paths must be less than
        /// 248 characters and file names must be less than 260 characters.</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive).</exception>
        /// <exception cref="System.NotSupportedException">path contains a colon character (:) that is not part of a drive label ("C:\").</exception>
        /// <returns>Path of created directory with assigned DirectoryInfo property.</returns>
        public PathInfo DirectoryCreate(DirectorySecurity directory_security = null)
		{
			if (directory_security != null)
				_DirectoryInfo = Directory.CreateDirectory(FullPath, directory_security);
            else
                _DirectoryInfo = Directory.CreateDirectory(FullPath);

            directory_info_retrieved = true;

            return this;
		}

        public PathInfo DirectoryCreate(FileAttributes attributes, DirectorySecurity directory_security = null)
		{
            if (directory_security != null)
				_DirectoryInfo = Directory.CreateDirectory(FullPath, directory_security);
            else
                _DirectoryInfo = Directory.CreateDirectory(FullPath);

            directory_info_retrieved = true;

            if (attributes != FileAttributes.Directory)
                _DirectoryInfo.Attributes = attributes | FileAttributes.Directory;

            return this;
		}

        public PathInfo DirectoryCreate(string subdirectory_name, DirectorySecurity directory_security = null)
		{
			return Combine(subdirectory_name).DirectoryCreate(directory_security);
		}

        public PathInfo DirectoryCreate(string subdirectory_name, FileAttributes attributes, DirectorySecurity directory_security = null)
		{
            return Combine(subdirectory_name).DirectoryCreate(attributes, directory_security);
		}

        /// <summary>Deletes the specified directory and, if indicated, any subdirectories and files in the directory.</summary>
        /// 
        /// <exception cref="System.IO.IOException">A file with the same name and location specified by path exists.-or-The directory
        /// specified by path is read-only, or recursive is false and path is not an
        /// empty directory. -or-The directory is the application's current working directory.
        /// -or-The directory contains a read-only file.-or-The directory is being used
        /// by another process.There is an open handle on the directory or on one of
        /// its files, and the operating system is Windows XP or earlier. This open handle
        /// can result from enumerating directories and files. For more information,
        /// see How to: Enumerate Directories and Files.</exception>
        /// 
        /// <exception cref="System.UnauthorizedAccessException">The caller does not have the required permission.</exception>
        /// <exception cref="System.ArgumentException">path is a zero-length string, contains only white space, or contains one
        /// or more invalid characters as defined by System.IO.Path.InvalidPathChars.</exception>
        /// <exception cref="System.ArgumentNullException">path is null</exception>
        /// <exception cref="System.IO.PathTooLongException">The specified path, file name, or both exceed the system-defined maximum
        /// length. For example, on Windows-based platforms, paths must be less than
        /// 248 characters and file names must be less than 260 characters.</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">path does not exist or could not be found.-or-path refers to a file instead
        /// of a directory.-or-The specified path is invalid (for example, it is on an
        /// unmapped drive).</exception>
        /// 
        /// <param name="recursive">true to remove directories, subdirectories, and files in path; otherwise, false.</param>
        public void DirectoryDelete(bool recursive = false)
        {
            Directory.Delete(FullPath, recursive);
        }

        public void DirectoryDelete(string subdirectory_name, bool recursive = false)
        {
            Directory.Delete(IOPath.Combine(FullPath, subdirectory_name), recursive);
        }

        /// <summary>
        /// (There are exceptions!)Determines whether the given path refers to an existing directory on disk.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">The path parameter is null.</exception>
        /// <exception cref="System.UnauthorizedAccessException">??? todo</exception>
        /// <seealso cref="TryDirectoryExists"/>
        /// <returns>true if path refers to an existing directory; otherwise, false.</returns>
		public bool DirectoryExists()
		{
            return Directory.Exists(FullPath);
		}

        public bool DirectoryExists(string subdirectory_name)
		{
            return Directory.Exists(IOPath.Combine(FullPath, subdirectory_name));
		}

        /// <summary>Gets a System.Security.AccessControl.DirectorySecurity object that encapsulates 
        /// the specified type of access control list (ACL) entries for a specified directory.
        /// </summary>
        /// <param name="include_sections">One of the System.Security.AccessControl.AccessControlSections values that
        /// specifies the type of access control list (ACL) information to receive.</param>
        /// <exception cref="System.ArgumentNullException">The path parameter is null.</exception>
        /// <exception cref="System.IO.IOException">An I/O error occurred while opening the directory.</exception>
        /// <exception cref="System.PlatformNotSupportedException">The current operating system is not Windows 2000 or later.</exception>
        /// <exception cref="System.SystemException">The directory could not be found.</exception>
        /// <exception cref="System.UnauthorizedAccessException">The path parameter specified a directory that is read-only.-or- This operation
        /// is not supported on the current platform.-or- The caller does not have the
        /// required permission.</exception>
        /// <returns>A System.Security.AccessControl.DirectorySecurity object that encapsulates
        /// the access control rules for the file described by the path parameter.</returns>
        public DirectorySecurity DirectoryGetAccessControl(AccessControlSections include_sections = AccessControlSections.All)
		{
            return Directory.GetAccessControl(FullPath, include_sections);
		}
    
        public DirectorySecurity DirectoryGetAccessControl(string subdirectory_name, AccessControlSections include_sections = AccessControlSections.All)
		{
            return Directory.GetAccessControl(IOPath.Combine(FullPath, subdirectory_name), include_sections);
		}

        //
        // Summary:
        //     Gets the creation date and time of a directory.
        //
        // Parameters:
        //   path:
        //     The path of the directory.
        //
        // Returns:
        //     A System.DateTime structure set to the creation date and time for the specified
        //     directory. This value is expressed in local time.
        //
        // Exceptions:
        //   System.UnauthorizedAccessException:
        //     The caller does not have the required permission.
        //
        //   System.ArgumentException:
        //     path is a zero-length string, contains only white space, or contains one
        //     or more invalid characters as defined by System.IO.Path.InvalidPathChars.
        //
        //   System.ArgumentNullException:
        //     path is null.
        //
        //   System.IO.PathTooLongException:
        //     The specified path, file name, or both exceed the system-defined maximum
        //     length. For example, on Windows-based platforms, paths must be less than
        //     248 characters and file names must be less than 260 characters.
        public DateTime DirectoryGetCreationTime()
        {
            return Directory.GetCreationTime(FullPath);
        }

        public DateTime DirectoryGetCreationTime(string subdirectory_name)
        {
            return Directory.GetCreationTime(IOPath.Combine(FullPath, subdirectory_name));
        }

        //
        // Summary:
        //     Gets the creation date and time, in Coordinated Universal Time (UTC) format,
        //     of a directory.
        //
        // Parameters:
        //   path:
        //     The path of the directory.
        //
        // Returns:
        //     A System.DateTime structure set to the creation date and time for the specified
        //     directory. This value is expressed in UTC time.
        //
        // Exceptions:
        //   System.UnauthorizedAccessException:
        //     The caller does not have the required permission.
        //
        //   System.ArgumentException:
        //     path is a zero-length string, contains only white space, or contains one
        //     or more invalid characters as defined by System.IO.Path.InvalidPathChars.
        //
        //   System.ArgumentNullException:
        //     path is null.
        //
        //   System.IO.PathTooLongException:
        //     The specified path, file name, or both exceed the system-defined maximum
        //     length. For example, on Windows-based platforms, paths must be less than
        //     248 characters and file names must be less than 260 characters.
        public DateTime DirectoryGetCreationTimeUtc()
        {
            return Directory.GetCreationTimeUtc(FullPath);
        }

        public DateTime DirectoryGetCreationTimeUtc(string subdirectory_name)
        {
            return Directory.GetCreationTimeUtc(IOPath.Combine(FullPath, subdirectory_name));
        }

        public PathInfo DirectoryGetCurrentDirectory()
        {
            return new PathInfo(Directory.GetCurrentDirectory());
        }

        //
        // Summary:
        //     Returns the date and time the specified file or directory was last accessed.
        //
        // Parameters:
        //   path:
        //     The file or directory for which to obtain access date and time information.
        //
        // Returns:
        //     A System.DateTime structure set to the date and time the specified file or
        //     directory was last accessed. This value is expressed in local time.
        //
        // Exceptions:
        //   System.UnauthorizedAccessException:
        //     The caller does not have the required permission.
        //
        //   System.ArgumentException:
        //     path is a zero-length string, contains only white space, or contains one
        //     or more invalid characters as defined by System.IO.Path.InvalidPathChars.
        //
        //   System.ArgumentNullException:
        //     path is null.
        //
        //   System.IO.PathTooLongException:
        //     The specified path, file name, or both exceed the system-defined maximum
        //     length. For example, on Windows-based platforms, paths must be less than
        //     248 characters and file names must be less than 260 characters.
        //
        //   System.NotSupportedException:
        //     The path parameter is in an invalid format.
        public DateTime DirectoryGetLastAccessTime()
        {
            return Directory.GetLastAccessTime(FullPath);
        }

        public DateTime DirectoryGetLastAccessTime(string subdirectory_name)
        {
            return Directory.GetLastAccessTime(IOPath.Combine(FullPath, subdirectory_name));
        }

        //
        // Summary:
        //     Returns the date and time, in Coordinated Universal Time (UTC) format, that
        //     the specified file or directory was last accessed.
        //
        // Parameters:
        //   path:
        //     The file or directory for which to obtain access date and time information.
        //
        // Returns:
        //     A System.DateTime structure set to the date and time the specified file or
        //     directory was last accessed. This value is expressed in UTC time.
        //
        // Exceptions:
        //   System.UnauthorizedAccessException:
        //     The caller does not have the required permission.
        //
        //   System.ArgumentException:
        //     path is a zero-length string, contains only white space, or contains one
        //     or more invalid characters as defined by System.IO.Path.InvalidPathChars.
        //
        //   System.ArgumentNullException:
        //     path is null.
        //
        //   System.IO.PathTooLongException:
        //     The specified path, file name, or both exceed the system-defined maximum
        //     length. For example, on Windows-based platforms, paths must be less than
        //     248 characters and file names must be less than 260 characters.
        //
        //   System.NotSupportedException:
        //     The path parameter is in an invalid format.
        public DateTime DirectoryGetLastAccessTimeUtc()
        {
            return Directory.GetLastAccessTimeUtc(FullPath);
        }

        public DateTime DirectoryGetLastAccessTimeUtc(string subdirectory_name)
        {
            return Directory.GetLastAccessTimeUtc(IOPath.Combine(FullPath, subdirectory_name));
        }

        //
        // Summary:
        //     Returns the date and time the specified file or directory was last written
        //     to.
        //
        // Parameters:
        //   path:
        //     The file or directory for which to obtain modification date and time information.
        //
        // Returns:
        //     A System.DateTime structure set to the date and time the specified file or
        //     directory was last written to. This value is expressed in local time.
        //
        // Exceptions:
        //   System.UnauthorizedAccessException:
        //     The caller does not have the required permission.
        //
        //   System.ArgumentException:
        //     path is a zero-length string, contains only white space, or contains one
        //     or more invalid characters as defined by System.IO.Path.InvalidPathChars.
        //
        //   System.ArgumentNullException:
        //     path is null.
        //
        //   System.IO.PathTooLongException:
        //     The specified path, file name, or both exceed the system-defined maximum
        //     length. For example, on Windows-based platforms, paths must be less than
        //     248 characters and file names must be less than 260 characters.
        public DateTime DirectoryGetLastWriteTime()
        {
            return Directory.GetLastWriteTime(FullPath);
        }

        public DateTime DirectoryGetLastWriteTime(string subdirectory_name)
        {
            return Directory.GetLastWriteTime(IOPath.Combine(FullPath, subdirectory_name));
        }

        //
        // Summary:
        //     Returns the date and time, in Coordinated Universal Time (UTC) format, that
        //     the specified file or directory was last written to.
        //
        // Parameters:
        //   path:
        //     The file or directory for which to obtain modification date and time information.
        //
        // Returns:
        //     A System.DateTime structure set to the date and time the specified file or
        //     directory was last written to. This value is expressed in UTC time.
        //
        // Exceptions:
        //   System.UnauthorizedAccessException:
        //     The caller does not have the required permission.
        //
        //   System.ArgumentException:
        //     path is a zero-length string, contains only white space, or contains one
        //     or more invalid characters as defined by System.IO.Path.InvalidPathChars.
        //
        //   System.ArgumentNullException:
        //     path is null.
        //
        //   System.IO.PathTooLongException:
        //     The specified path, file name, or both exceed the system-defined maximum
        //     length. For example, on Windows-based platforms, paths must be less than
        //     248 characters and file names must be less than 260 characters.
        public DateTime DirectoryGetLastWriteTimeUtc()
        {
            return Directory.GetLastWriteTimeUtc(FullPath);
        }

        public DateTime DirectoryGetLastWriteTimeUtc(string subdirectory_name)
        {
            return Directory.GetLastWriteTimeUtc(IOPath.Combine(FullPath, subdirectory_name));
        }

        public PathInfo DirectoryMove(PathInfo destination_directory_path)
        {
            Directory.Move(FullPath, destination_directory_path);
            return destination_directory_path;
        }

        public PathInfo DirectoryMove(string subdirectory_name, PathInfo destination_directory_path)
        {
            string full_path = IOPath.Combine(FullPath, subdirectory_name);
            Directory.Move(full_path, destination_directory_path);
            return destination_directory_path;
        }

        public PathInfo DirectorySetAccessControl(DirectorySecurity directory_security)
        {
            Directory.SetAccessControl(FullPath, directory_security);
            return this;
        }

        public PathInfo DirectorySetAccessControl(string subdirectory_name, DirectorySecurity directory_security)
        {
            string full_path = IOPath.Combine(FullPath, subdirectory_name);
            Directory.SetAccessControl(full_path, directory_security);
            return new PathInfo(full_path);
        }

        //
        // Summary:
        //     Sets the creation date and time for the specified file or directory.
        //
        // Parameters:
        //   path:
        //     The file or directory for which to set the creation date and time information.
        //
        //   creationTime:
        //     A System.DateTime containing the value to set for the creation date and time
        //     of path. This value is expressed in local time.
        //
        // Exceptions:
        //   System.IO.FileNotFoundException:
        //     The specified path was not found.
        //
        //   System.ArgumentException:
        //     path is a zero-length string, contains only white space, or contains one
        //     or more invalid characters as defined by System.IO.Path.InvalidPathChars.
        //
        //   System.ArgumentNullException:
        //     path is null.
        //
        //   System.IO.PathTooLongException:
        //     The specified path, file name, or both exceed the system-defined maximum
        //     length. For example, on Windows-based platforms, paths must be less than
        //     248 characters and file names must be less than 260 characters.
        //
        //   System.UnauthorizedAccessException:
        //     The caller does not have the required permission.
        //
        //   System.ArgumentOutOfRangeException:
        //     creationTime specifies a value outside the range of dates or times permitted
        //     for this operation.
        //
        //   System.PlatformNotSupportedException:
        //     The current operating system is not Windows NT or later.
        public PathInfo DirectorySetCreationTime(DateTime creation_time)
        {
            Directory.SetCreationTime(FullPath, creation_time);
            return this;
        }

        public PathInfo DirectorySetCreationTime(string subdirectory_name, DateTime creation_time)
        {
            string full_path = IOPath.Combine(FullPath, subdirectory_name);
            Directory.SetCreationTime(full_path, creation_time);
            return new PathInfo(full_path);
        }

        //
        // Summary:
        //     Sets the creation date and time, in Coordinated Universal Time (UTC) format,
        //     for the specified file or directory.
        //
        // Parameters:
        //   path:
        //     The file or directory for which to set the creation date and time information.
        //
        //   creationTimeUtc:
        //     A System.DateTime containing the value to set for the creation date and time
        //     of path. This value is expressed in UTC time.
        //
        // Exceptions:
        //   System.IO.FileNotFoundException:
        //     The specified path was not found.
        //
        //   System.ArgumentException:
        //     path is a zero-length string, contains only white space, or contains one
        //     or more invalid characters as defined by System.IO.Path.InvalidPathChars.
        //
        //   System.ArgumentNullException:
        //     path is null.
        //
        //   System.IO.PathTooLongException:
        //     The specified path, file name, or both exceed the system-defined maximum
        //     length. For example, on Windows-based platforms, paths must be less than
        //     248 characters and file names must be less than 260 characters.
        //
        //   System.UnauthorizedAccessException:
        //     The caller does not have the required permission.
        //
        //   System.ArgumentOutOfRangeException:
        //     creationTime specifies a value outside the range of dates or times permitted
        //     for this operation.
        //
        //   System.PlatformNotSupportedException:
        //     The current operating system is not Windows NT or later.
        public PathInfo DirectorySetCreationTimeUtc(DateTime creation_time_Utc)
        {
            Directory.SetCreationTimeUtc(FullPath, creation_time_Utc);
            return this;
        }

        public PathInfo DirectorySetCreationTimeUtc(string subdirectory_name, DateTime creation_time_Utc)
        {
            string full_path = IOPath.Combine(FullPath, subdirectory_name);
            Directory.SetCreationTimeUtc(full_path, creation_time_Utc);
            return new PathInfo(full_path);
        }

        //
        // Summary:
        //     Sets the application's current working directory to the specified directory.
        //
        // Parameters:
        //   path:
        //     The path to which the current working directory is set.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurred.
        //
        //   System.ArgumentException:
        //     path is a zero-length string, contains only white space, or contains one
        //     or more invalid characters as defined by System.IO.Path.InvalidPathChars.
        //
        //   System.ArgumentNullException:
        //     path is null.
        //
        //   System.IO.PathTooLongException:
        //     The specified path, file name, or both exceed the system-defined maximum
        //     length. For example, on Windows-based platforms, paths must be less than
        //     248 characters and file names must be less than 260 characters.
        //
        //   System.Security.SecurityException:
        //     The caller does not have the required permission to access unmanaged code.
        //
        //   System.IO.FileNotFoundException:
        //     The specified path was not found.
        //
        //   System.IO.DirectoryNotFoundException:
        //     The specified directory was not found.
        public PathInfo DirectorySetCurrentDirectory()
        {
            Directory.SetCurrentDirectory(FullPath);
            return this;
        }

        public PathInfo DirectorySetCurrentDirectory(string subdirectory_name)
        {
            string full_path = IOPath.Combine(FullPath, subdirectory_name);
            Directory.SetCurrentDirectory(full_path);
            return new PathInfo(full_path);
        }

        //
        // Summary:
        //     Sets the date and time the specified file or directory was last accessed.
        //
        // Parameters:
        //   path:
        //     The file or directory for which to set the access date and time information.
        //
        //   lastAccessTime:
        //     A System.DateTime containing the value to set for the access date and time
        //     of path. This value is expressed in local time.
        //
        // Exceptions:
        //   System.IO.FileNotFoundException:
        //     The specified path was not found.
        //
        //   System.ArgumentException:
        //     path is a zero-length string, contains only white space, or contains one
        //     or more invalid characters as defined by System.IO.Path.InvalidPathChars.
        //
        //   System.ArgumentNullException:
        //     path is null.
        //
        //   System.IO.PathTooLongException:
        //     The specified path, file name, or both exceed the system-defined maximum
        //     length. For example, on Windows-based platforms, paths must be less than
        //     248 characters and file names must be less than 260 characters.
        //
        //   System.UnauthorizedAccessException:
        //     The caller does not have the required permission.
        //
        //   System.PlatformNotSupportedException:
        //     The current operating system is not Windows NT or later.
        //
        //   System.ArgumentOutOfRangeException:
        //     lastAccessTime specifies a value outside the range of dates or times permitted
        //     for this operation.
        public PathInfo DirectorySetLastAccessTime(DateTime last_access_time)
        {
            Directory.SetLastAccessTime(FullPath, last_access_time);
            return this;
        }

        public PathInfo DirectorySetLastAccessTime(string subdirectory_name, DateTime last_access_time)
        {
            string full_path = IOPath.Combine(FullPath, subdirectory_name);
            Directory.SetLastAccessTime(full_path, last_access_time);
            return new PathInfo(full_path);
        }

        //
        // Summary:
        //     Sets the date and time, in Coordinated Universal Time (UTC) format, that
        //     the specified file or directory was last accessed.
        //
        // Parameters:
        //   path:
        //     The file or directory for which to set the access date and time information.
        //
        //   lastAccessTimeUtc:
        //     A System.DateTime containing the value to set for the access date and time
        //     of path. This value is expressed in UTC time.
        //
        // Exceptions:
        //   System.IO.FileNotFoundException:
        //     The specified path was not found.
        //
        //   System.ArgumentException:
        //     path is a zero-length string, contains only white space, or contains one
        //     or more invalid characters as defined by System.IO.Path.InvalidPathChars.
        //
        //   System.ArgumentNullException:
        //     path is null.
        //
        //   System.IO.PathTooLongException:
        //     The specified path, file name, or both exceed the system-defined maximum
        //     length. For example, on Windows-based platforms, paths must be less than
        //     248 characters and file names must be less than 260 characters.
        //
        //   System.UnauthorizedAccessException:
        //     The caller does not have the required permission.
        //
        //   System.PlatformNotSupportedException:
        //     The current operating system is not Windows NT or later.
        //
        //   System.ArgumentOutOfRangeException:
        //     lastAccessTimeUtc specifies a value outside the range of dates or times permitted
        //     for this operation.
        public PathInfo DirectorySetLastAccessTimeUtc(DateTime last_access_time_Utc)
        {
            Directory.SetLastAccessTimeUtc(FullPath, last_access_time_Utc);
            return this;
        }

        public PathInfo DirectorySetLastAccessTimeUtc(string subdirectory_name, DateTime last_access_time_Utc)
        {
            string full_path = IOPath.Combine(FullPath, subdirectory_name);
            Directory.SetLastAccessTimeUtc(full_path, last_access_time_Utc);
            return new PathInfo(full_path);
        }

        //
        // Summary:
        //     Sets the date and time a directory was last written to.
        //
        // Parameters:
        //   path:
        //     The path of the directory.
        //
        //   lastWriteTime:
        //     The date and time the directory was last written to. This value is expressed
        //     in local time.
        //
        // Exceptions:
        //   System.IO.FileNotFoundException:
        //     The specified path was not found.
        //
        //   System.ArgumentException:
        //     path is a zero-length string, contains only white space, or contains one
        //     or more invalid characters as defined by System.IO.Path.InvalidPathChars.
        //
        //   System.ArgumentNullException:
        //     path is null.
        //
        //   System.IO.PathTooLongException:
        //     The specified path, file name, or both exceed the system-defined maximum
        //     length. For example, on Windows-based platforms, paths must be less than
        //     248 characters and file names must be less than 260 characters.
        //
        //   System.UnauthorizedAccessException:
        //     The caller does not have the required permission.
        //
        //   System.PlatformNotSupportedException:
        //     The current operating system is not Windows NT or later.
        //
        //   System.ArgumentOutOfRangeException:
        //     lastWriteTime specifies a value outside the range of dates or times permitted
        //     for this operation.
        public PathInfo DirectorySetLastWriteTime(DateTime last_write_time)
        {
            Directory.SetLastWriteTime(FullPath, last_write_time);
            return this;
        }

        public PathInfo DirectorySetLastWriteTime(string subdirectory_name, DateTime last_write_time)
        {
            string full_path = IOPath.Combine(FullPath, subdirectory_name);
            Directory.SetLastWriteTime(full_path, last_write_time);
            return new PathInfo(full_path);
        }

        //
        // Summary:
        //     Sets the date and time, in Coordinated Universal Time (UTC) format, that
        //     a directory was last written to.
        //
        // Parameters:
        //   path:
        //     The path of the directory.
        //
        //   lastWriteTimeUtc:
        //     The date and time the directory was last written to. This value is expressed
        //     in UTC time.
        //
        // Exceptions:
        //   System.IO.FileNotFoundException:
        //     The specified path was not found.
        //
        //   System.ArgumentException:
        //     path is a zero-length string, contains only white space, or contains one
        //     or more invalid characters as defined by System.IO.Path.InvalidPathChars.
        //
        //   System.ArgumentNullException:
        //     path is null.
        //
        //   System.IO.PathTooLongException:
        //     The specified path, file name, or both exceed the system-defined maximum
        //     length. For example, on Windows-based platforms, paths must be less than
        //     248 characters and file names must be less than 260 characters.
        //
        //   System.UnauthorizedAccessException:
        //     The caller does not have the required permission.
        //
        //   System.PlatformNotSupportedException:
        //     The current operating system is not Windows NT or later.
        //
        //   System.ArgumentOutOfRangeException:
        //     lastWriteTimeUtc specifies a value outside the range of dates or times permitted
        //     for this operation.
        public PathInfo DirectorySetLastWriteTimeUtc(DateTime last_write_time_Utc)
        {
            Directory.SetLastWriteTimeUtc(FullPath, last_write_time_Utc);
            return this;
        }

        public PathInfo DirectorySetLastWriteTimeUtc(string subdirectory_name, DateTime last_write_time_Utc)
        {
            string full_path = IOPath.Combine(FullPath, subdirectory_name);
            Directory.SetLastWriteTimeUtc(full_path, last_write_time_Utc);
            return new PathInfo(full_path);
        }
        
        // TODO Additional Directory methods. //////////////////////////////////////////////////////////////////////////


        /// <summary>(The method does not throw any exceptions!)
        /// 
        /// </summary>
        /// <returns></returns>
        public bool TryDirectoryCreate(DirectorySecurity directory_security = null)
		{
			if (Directory.Exists(FullPath))
                return true;

			try
			{
                if (directory_security != null)
				    _DirectoryInfo = Directory.CreateDirectory(FullPath, directory_security);
                else
                    _DirectoryInfo = Directory.CreateDirectory(FullPath);

                directory_info_retrieved = true;

				return true;
			}
			catch
            {
            }

			return false;
		}

        /// <summary>(The method does not throw any exceptions!)
        /// 
        /// </summary>
        /// <returns></returns>
        public bool TryDirectoryCreate(FileAttributes attributes, DirectorySecurity directory_security = null)
		{
			if (Directory.Exists(FullPath))
                return true;

			try
			{
                if (directory_security != null)
				    _DirectoryInfo = Directory.CreateDirectory(FullPath, directory_security);
                else
                    _DirectoryInfo = Directory.CreateDirectory(FullPath);

                directory_info_retrieved = true;

                if (attributes != FileAttributes.Directory)
                    _DirectoryInfo.Attributes = attributes | FileAttributes.Directory;

				return true;
			}
			catch
            {
            }

			return false;
		}

        public bool TryDirectoryCreate(string subdirectory, DirectorySecurity directory_security = null)
		{
            return Combine(subdirectory).TryDirectoryCreate(directory_security);
		}

        public bool TryDirectoryCreate(string subdirectory, FileAttributes attributes, DirectorySecurity directory_security = null)
		{
			return Combine(subdirectory).TryDirectoryCreate(attributes, directory_security);
		}

        /// <summary>(The method does not throw any exceptions!) Determines whether the given path refers to an existing directory on disk.</summary>
        /// <returns>true if the caller has the required permissions and path contains the name
        /// of an existing directory; otherwise, false. This method also returns false if
        /// path is null, an invalid path, or a zero-length string. If the caller does
        /// not have sufficient permissions to read the specified path, no exception
        /// is thrown and the method returns false regardless of the existence of path.</returns>
        public bool TryDirectoryExists()
		{
            try
            {
			    return Directory.Exists(FullPath);
            }
            catch
            {
            }

            return false;
		}

        public bool TryDirectoryExists(string subdirectory)
		{
            try
            {
			    return Directory.Exists(IOPath.Combine(FullPath, subdirectory));
            }
            catch
            {
            }

            return false;
		}

    }
    
    public class PathList : List<PathInfo>
    {
        public static PathList operator +(PathList paths1, IEnumerable<PathInfo> paths2)
        {
            var list  = new PathList();
            list.AddRange(paths1.Union(paths2));
            return list;
        }

        public static PathList operator -(PathList paths1, IEnumerable<PathInfo> paths2)
        {
            var list  = new PathList();
            
            foreach(var path1 in paths1)
            if (!paths2.Contains(path1))
                list.Add(path1);

            return list;
        }

        public static PathList operator -(PathList paths1, Func<PathInfo,bool> match_comparer)
        {
            var list  = new PathList(paths1.Count);

            for(int i = 0, c = paths1.Count; i < c; i++)
            {
                var path = paths1[i];

                if (!match_comparer(path))
                    list.Add(path);
            }
            
            return list;
        }

        public static PathList operator -(PathList paths1, string search_pattern)
        {
            var list  = new PathList(paths1.Count);

            for(int i = 0, c = paths1.Count; i < c; i++)
            {
                var path = paths1[i];

                if (!PathInfo.MatchesMaskComparer(path, search_pattern))
                    list.Add(path);
            }
            
            return list;
        }

        public static implicit operator PathList(string[] paths)
        {
            return new PathList(paths);
        }

        public static implicit operator PathList(PathInfo path)
        {
            var list = new PathList(1);
            list.Add(path);
            return list;
        }

        public PathList()
        {
        }

        public PathList(IEnumerable<PathInfo> paths) : base(paths)
        {
        }

        public PathList(IEnumerable<string> paths) : base(paths.Select(path => new PathInfo(path)))
        {
        }

        public PathList(int capacity) : base(capacity)
        {
        }
    }

    public static class PathEnumerable
    {
        public static PathList ToList(this IEnumerable<PathInfo> paths)
        {
            var list  = new PathList(paths.Count());
            list.AddRange(paths);
            return list;
        }
    }

    /// <summary>Safe walker with error handling and cancellation token</summary>
    public class PathWalker
    {
        // TODO
    }

    #region    ------------------ Bulk Pattern Origin ------------------

    /* Основная суть этого паттерна получить групповой результат выполнения операции над перечислимым множеством для последующей обработки ошибок или успехов пакетной операцией.
     * Паттерн еще не закончен в том смысле, что нужно еще добавить устоявшийся способ группировки результатов множества операций над одним или несколькими перечислимыми множествами.
     * 
     * The main essence of this pattern have group result of the operation of the enumerable set for later processing error or success batch operation.
     * The pattern is not yet complete in the sense that it is necessary to add an method of grouping results of multiple operations involving one or more enumerated sets. */

    public class WrapException<T> : Exception
    {
        public T Object;

        public WrapException(Exception e, T obj) : base("Wrapped: " + e.Message, e)
        {
            Object = obj;
        }
    }

    public class BulkException<T> : Exception
    {
        public IList<T> Successful;
        public IList<WrapException<T>> Failed;

        public BulkException()
        {
            Successful = new List<T>();
            Failed =  new List<WrapException<T>>();
        }

        public BulkException(IList<T> successful, IList<WrapException<T>> failed)
        {
            Successful = successful;
            Failed = failed;
        }
    }

    public static class BulkEnumerable
    {
        public static void Bulk<T>(this IEnumerable<T> paths, Action<T> action)
        {
            List<T> successful = new List<T>();
            List<WrapException<T>> failed = null;

            foreach(var path in paths)
            {
                try
                {
                    action(path);
                    successful.Add(path);
                }
                catch (Exception e)
                {
                    if (failed == null)
                        failed = new List<WrapException<T>>();

                    failed.Add(new WrapException<T>(e, path));
                }
            }

            if (failed != null)
                throw new BulkException<T>(successful, failed);
        }
    }

    #endregion ------------------ Bulk Pattern Origin ------------------
}
