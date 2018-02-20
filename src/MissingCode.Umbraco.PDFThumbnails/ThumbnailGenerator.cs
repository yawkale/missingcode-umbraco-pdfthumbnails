using System;
using System.IO;
using System.Text;
using GhostscriptSharp;
using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.Logging;

namespace MissingCode.Umbraco.PdfThumbnail
{
    public class ThumbnailGenerator
    {
        public static string GenerateThumbnailFilename(string filename)
        {
            return Path.Combine(Path.GetDirectoryName(filename), $"{Path.GetFileNameWithoutExtension(filename)}_thumb.jpg");
        }

        public static string GetThumbnail(string filename)
        {
            var thubmnailPath = GenerateThumbnailFilename(filename);
            var fileSystem = FileSystemProviderManager.Current.GetFileSystemProvider<MediaFileSystem>();

            var result=fileSystem.FileExists(fileSystem.GetFullPath(thubmnailPath));

            if (!result)
            {
                return null;
            }
            return thubmnailPath;
        }

        public static string Generate(string file)
        {
            using (ApplicationContext.Current.ProfilingLogger.TraceDuration<ThumbnailGenerator>("Started Creatning Thumbnail", "Completed Creating Thumbnail"))
            {
                
                    var fileSystem = FileSystemProviderManager.Current.GetFileSystemProvider<MediaFileSystem>();

                    var outputUrl = GenerateThumbnailFilename(file);
                    var output = fileSystem.GetFullPath(outputUrl);

                    var temp = Path.ChangeExtension(Path.GetRandomFileName(), "pdf");

                    var tempFilename =
                        Path.Combine(Path.GetDirectoryName(fileSystem.GetFullPath(fileSystem.GetFullPath(file))), temp);
                    var tempOutputFilename = GenerateThumbnailFilename(tempFilename);

                try
                {
                    //var source = fileSystem.GetFullPath(file);
                    fileSystem.CopyFile(file, tempFilename);

                    LogHelper.Info<ThumbnailGenerator>($"Generate {tempFilename} to {tempOutputFilename}");
                    GhostscriptWrapper.GeneratePageThumb(tempFilename, tempOutputFilename, 1, 100, 100);
                    LogHelper.Info<ThumbnailGenerator>($"Generated {tempFilename} to {tempOutputFilename}");

                    fileSystem.DeleteFile(tempFilename);

                    fileSystem.CopyFile(tempOutputFilename, output);

                    fileSystem.DeleteFile(tempOutputFilename);

                    return outputUrl;
                }
                catch (Exception ex)
                {
                    LogHelper.Error<ThumbnailGenerator>("Faled generat pdf thumbnail", ex);
                    return null;

                }
                finally
                {
                    if (fileSystem.FileExists(tempFilename))
                        fileSystem.DeleteFile(tempFilename);

                    if (fileSystem.FileExists(tempOutputFilename))
                        fileSystem.DeleteFile(tempOutputFilename);
                }
            }
        }
    }
}
