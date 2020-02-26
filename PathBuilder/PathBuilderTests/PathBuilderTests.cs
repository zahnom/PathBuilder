using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PathBuilderNamespace;

namespace PathBuilderTestsNamespace
{
    [TestClass]
    public class PathBuilderTests
    {
        // Todo: Split this unit tests, this is horrible...

        [TestMethod]
        public void TestRelativePath()
        {
            var path1 = PathBuilder.CreateRelativePath()
                        .From("C:/some/path/")
                        .To("C:/other/path/somewhere/else/")
                        .Create();

            Console.WriteLine(path1);
            Assert.AreEqual("../../other/path/somewhere/else/", path1);


            var path2 = PathBuilder.CreateRelativePath()
                        .From(@"C:\some\path\")
                        .To("C:/other/path/somewhere/else.exe")
                        .TerminateDirsWithSlash()
                        .UseBackslashes()
                        .Create();

            Console.WriteLine(path2);
            Assert.AreEqual(@"..\..\other\path\somewhere\else.exe", path2);


            var path3 = PathBuilder.CreateRelativePath()
                        .From(@"C:\some\path\")
                        .To("C:/other/path/somewhere/else.exe")
                        .UseForwardslashes()
                        .Create();

            Console.WriteLine(path3);
            Assert.AreEqual(@"../../other/path/somewhere/else.exe", path3);


            var path4 = PathBuilder.CreateRelativePath()
                        .From(@"C:\some\path")
                        .To("C:/other/path/somewhere/else.exe")
                        .UseForwardslashes()
                        .Create();

            Console.WriteLine(path4);
            Assert.AreEqual(@"../other/path/somewhere/else.exe", path4);


            var path5 = PathBuilder.CreateRelativePath()
            .From(@"C:\some\path")
            .To("C:/other/path/somewhere/else.exe")
            .UseForwardslashes()
            .TerminateDirsWithSlash()
            .Create();

            Console.WriteLine(path5);
            Assert.AreEqual(@"../../other/path/somewhere/else.exe", path5);
        }

        [TestMethod]
        public void TestAbsolutePath()
        {
            var path1 = PathBuilder.CreateAbsolutePath()
                        .StartAt("C:/some/path/")
                        .GoTo("../other/path/")
                        .Create();

            Console.WriteLine(path1);
            Assert.AreEqual("C:/some/other/path/", path1);


            var path2 = PathBuilder.CreateAbsolutePath()
            .StartAt("C:/some/path/")
            .GoTo("abit/deeper/")
            .Create();

            Console.WriteLine(path2);
            Assert.AreEqual("C:/some/path/abit/deeper/", path2);


            var path3 = PathBuilder.CreateAbsolutePath()
            .StartAt("C:/some///path.exe")
            .GoTo("other/path/cmd.exe")
            .GoTo("thats/even/deeper")
            .GoTo(@"..\..\test.exe")
            .UseBackslashes()
            .TerminateDirsWithSlash()
            .TrimSlashes()
            .Create();

            Console.WriteLine(path3);
            Assert.AreEqual(@"C:\some\other\path\thats\test.exe", path3);


            var path4 = PathBuilder.CreateAbsolutePath()
                .StartAt("C:/start/dir/")
                .GoTo("..")
                .Create();

            Console.WriteLine(path4);
            Assert.AreEqual("C:/start", path4);
        }

        [TestMethod]
        public void TestAbsolutePathToFile_DirWithoutEndingSlash()
        {
            var resultingPath = PathBuilder.CreateAbsolutePath()
                .StartAt(@"C:\some\dir")
                .GoTo("my-file.txt")
                .UseForwardslashes()
                .TerminateDirsWithSlash()
                .Create();

            var expectedPath = @"C:/some/dir/my-file.txt";

            Console.WriteLine("Constructed path: ");
            Console.WriteLine(resultingPath);
            Console.WriteLine();
            Console.WriteLine("Expected path: ");
            Console.WriteLine(expectedPath);

            Assert.AreEqual(expectedPath, resultingPath,
                "Path was not constructed properly!");
        }
    }
}
