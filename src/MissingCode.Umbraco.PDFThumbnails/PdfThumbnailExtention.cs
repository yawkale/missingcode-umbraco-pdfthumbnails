using Umbraco.Core.Models;

namespace MissingCode.Umbraco.PdfThumbnail
{
    public static class PdfThumbnailExtention
    {
        public static string GetPdfThumbnails(this IPublishedContent media,bool createIfNotExists=true)
        {
            var result = ThumbnailGenerator.GetThumbnail(media.Url);
            if (createIfNotExists && string.IsNullOrEmpty(result))
            {
                result = ThumbnailGenerator.Generate(media.Url);
            }
            return result;
        }
    }
}
