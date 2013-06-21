using System.IO;
using System.Runtime.InteropServices;
using System.Text;

/* INI File reader/writer
 * This is just an example of using the class PathInfo. */

namespace System.IO
{
    public class IniFile : PathInfo
    {
        [DllImport("kernel32.dll")]         
	    private extern static int GetPrivateProfileString(string AppName, string KeyName, string Default, StringBuilder ReturnedString, int Size, string FileName);

	    [DllImport("kernel32.dll")]
	    private extern static int WritePrivateProfileString(string AppName, string KeyName, string Str, string FileName);

        const int value_capacity = 4096;
        string app_name = (System.Reflection.Assembly.GetEntryAssembly() == null) ? "default" : System.Reflection.Assembly.GetEntryAssembly().GetName().Name;

        public string this[params string[] keys]
        {
            get
            {
                string section = null;
                string key = null;

                if (keys == null)
                {
                    section = app_name;
                    key = "default";
                }
                else if (keys.Length == 1)
                {
                    // only value key

                    section = app_name;
                    key = keys[0];
                }
                else if (keys.Length == 2)
                {
                    // section and key

                    section = keys[0];
                    key = keys[1];
                }
                else if (keys.Length == 3)
                    throw new OverflowException("Acceptable a maximum of two parameter");

                var builder = new StringBuilder(value_capacity);
                GetPrivateProfileString(section, key, null, builder, value_capacity, FullPath);

                return builder.ToString();
            }
            set
            {
                string section = null;
                string key = null;

                if (keys == null)
                {
                    section = app_name;
                    key = "default";
                }
                else if (keys.Length == 1)
                {
                    // only value key

                    section = app_name;
                    key = keys[0];
                }
                else if (keys.Length == 2)
                {
                    // section and key

                    section = keys[0];
                    key = keys[1];
                }
                else if (keys.Length == 3)
                    throw new OverflowException("Acceptable a maximum of two parameter");

                WritePrivateProfileString(section, key, value, FullPath);
            }
        }

        public IniFile(string path) : base(path) {}
    }
}