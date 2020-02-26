using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathBuilderNamespace
{
    public class PathBuilder
    {
        public static RelativePathBuilder CreateRelativePath()
        {
            return new RelativePathBuilder();
        }
        public static AbsolutePathBuilder CreateAbsolutePath()
        {
            return new AbsolutePathBuilder();
        }
        public static FormatPathBuilder FormatPath(string path)
        {
            return new FormatPathBuilder(path);
        }

        public class FormatPathBuilder
        {
            private string path = "";
            private char slash = '/';
            private bool terminateDirsWithSlash = false;
            private bool trimSlashes = false;

            public FormatPathBuilder(string str)
            {
                path = str;
            }

            public FormatPathBuilder UseBackslashes()
            {
                slash = '\\'; // '\' needs to be escaped
                return this;
            }
            public FormatPathBuilder UseForwardslashes()
            {
                slash = '/';
                return this;
            }
            public FormatPathBuilder TerminateDirsWithSlash()
            {
                terminateDirsWithSlash = true;
                return this;
            }
            public FormatPathBuilder TrimSlashes()
            {
                trimSlashes = true;
                return this;
            }
            public string Format()
            {
                path = path.Replace('/', slash);
                path = path.Replace('\\', slash);  // Escaped '\'

                // Check terminating slash for result if set
                if (terminateDirsWithSlash)
                {
                    path = TerminateWithSlash(path, slash);
                }

                // Trim slashes
                if (trimSlashes) path = Trim(path, slash);

                // We're done!
                return path;
            }
        }

        public class RelativePathBuilder
        {
            private string from = "";
            private string to = "";
            private char slash = '/';
            private bool terminateDirsWithSlash = false;
            private bool trimSlashes = false;

            public RelativePathBuilder From(string path)
            {
                from = path;
                return this;
            }
            public RelativePathBuilder To(string path)
            {
                to = path;
                return this;
            }
            public RelativePathBuilder UseBackslashes()
            {
                slash = '\\'; // '\' needs to be escaped
                return this;
            }
            public RelativePathBuilder UseForwardslashes()
            {
                slash = '/';
                return this;
            }
            public RelativePathBuilder TerminateDirsWithSlash()
            {
                terminateDirsWithSlash = true;
                return this;
            }
            public RelativePathBuilder TrimSlashes()
            {
                trimSlashes = true;
                return this;
            }
            public string Create()
            {
                // Convert slashes
                from = from.Replace('/', slash);
                from = from.Replace('\\', slash); // '\' needs to be escaped
                to = to.Replace('/', slash);
                to = to.Replace('\\', slash);   // '\' needs to be escaped

                // Add terminating slash for intermediate paths as well if set
                if (terminateDirsWithSlash)
                {
                    from = TerminateWithSlash(from, slash);
                }
                if (terminateDirsWithSlash)
                {
                    to = TerminateWithSlash(to, slash);
                }

                // Create relative path
                var result = new Uri(from).MakeRelativeUri(new Uri(to)).ToString();

                // Convert slashes again for result
                result = result.Replace('/', slash);
                result = result.Replace('\\', slash);  // '\' needs to be escaped

                // Check terminating slash for result if set
                if (terminateDirsWithSlash)
                {
                    result = TerminateWithSlash(result, slash);
                }

                // Trim slashes
                if (trimSlashes) result = Trim(result, slash);

                // We're done!
                return result;
            }
        }

        public class AbsolutePathBuilder
        {
            private string start = "";
            private List<string> goTo = new List<string>();
            private char slash = '/';
            private bool terminateDirsWithSlash = false;
            private bool trimSlashes = false;

            /// <summary>
            /// Sets the starting point of the path.
            /// </summary>
            public AbsolutePathBuilder StartAt(string path)
            {
                start = path;
                return this;
            }

            /// <summary>
            /// Goes to given path.
            /// </summary>
            /// 
            /// <example>
            /// var path = PathBuilder.CreateAbsolutePath
            ///             .StartAt("C:/start/dir/")
            ///             .GoTo("../")
            ///             .Create();
            ///             
            /// // Resulting path will be 'C:/start'.
            /// </example>
            /// <example>
            /// // You can combine multiple GoTos:
            /// var path = PathBuilder.CreateAbsolutePath
            ///            .StartAt("C:/start/dir/")
            ///            .GoTo("../")
            ///            .GoTo("another/dir")
            ///            .Create()
            ///            
            /// // Resulting path will be 'C:/start/another/dir'.
            /// </example>
            public AbsolutePathBuilder GoTo(string path)
            {
                goTo.Add(path);
                return this;
            }

            /// <summary>
            /// Use this option to create a path with backward slashes.
            /// </summary>
            public AbsolutePathBuilder UseBackslashes()
            {
                slash = '\\'; // '\' needs to be escaped
                return this;
            }

            /// <summary>
            /// Use this option to create a path with forward slashes.
            /// </summary>
            public AbsolutePathBuilder UseForwardslashes()
            {
                slash = '/';
                return this;
            }

            /// <summary>
            /// This option will add a slash at the end of the path if it does not
            /// end with a file extension.
            /// </summary>
            /// 'my/path/to/a/dir' would be changed to 'my/path/to/a/dir/'. Paths like
            /// 'my/paht/to/a/file.exe', however, would remain unchanged.
            public AbsolutePathBuilder TerminateDirsWithSlash()
            {
                terminateDirsWithSlash = true;
                return this;
            }

            /// <summary>
            /// Use this option to remove multiple consecutive slashes.
            /// </summary>
            /// If the path contains multiple consecutive slashes, like 'my//path/to/somehwere', 
            /// this option will trim the slashes to 'my/path/to/somewhere'. 
            public AbsolutePathBuilder TrimSlashes()
            {
                trimSlashes = true;
                return this;
            }

            /// <summary>
            /// Creates the resulting path.
            /// </summary>
            public string Create()
            {
                // Convert slashes
                start = start.Replace('/', slash);
                start = start.Replace('\\', slash); // Backslash needs to be escaped
                var convertedGoTo = new List<string>();

                // Add terminating slash for intermediate paths as well if set
                if (terminateDirsWithSlash)
                {
                    start = TerminateWithSlash(start, slash);
                }

                // Convert slashes and terminate (if option set) for each goto path as well
                foreach (var entry in goTo)
                {
                    var conv = entry;
                    conv = conv.Replace('/', slash);
                    conv = conv.Replace('\\', slash); // Backslash needs to be escaped

                    // Add terminating slash if option set
                    if (terminateDirsWithSlash)
                    {
                        conv = TerminateWithSlash(conv, slash);
                    }

                    convertedGoTo.Add(conv);
                }
                goTo = convertedGoTo;

                // Construct final path
                var result = start;
                foreach (var entry in goTo)
                {
                    result = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(result), entry));
                }

                // Convert slashes again for result
                result = result.Replace('/', slash);
                result = result.Replace('\\', slash);  // '\' needs to be escaped

                // Check terminating slash for result if set
                if (terminateDirsWithSlash)
                {
                    result = TerminateWithSlash(result, slash);
                }

                // Trim slashes
                if (trimSlashes) result = Trim(result, slash);

                // We're done!
                return result;
            }
        }

        private static string Trim(string str, char chr)
        {
            var result = "";

            var lastWasChr = false;
            foreach (var c in str)
            {
                if (c == chr && lastWasChr == false)
                {
                    result += c;
                    lastWasChr = true;
                }
                else if (c == chr && lastWasChr == true) continue;
                else
                {
                    result += c;
                    lastWasChr = false;
                }
            }

            return result;
        }
        private static string TerminateWithSlash(string str, char slash)
        {
            if (str.Length <= 0)
                return str;
            if (Path.GetExtension(str) != String.Empty)
                return str;
            if (str.Last() == slash)
                return str;

            str += slash;
            return str;
        }
    }
}
