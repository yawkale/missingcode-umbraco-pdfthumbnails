using System;
using System.Collections.Generic;
using Umbraco.Web.Media.ThumbnailProviders;

namespace MissingCode.Umbraco.PdfThumbnail
{

    public class PdfThumbnailProvider : AbstractThumbnailProvider
    {
        protected override bool TryGetThumbnailUrl(string fileUrl, out string thumbUrl)
        {
            
            try
            {
                thumbUrl = ThumbnailGenerator.Generate(fileUrl);

                return !string.IsNullOrEmpty(thumbUrl);
            }
            catch (Exception)
            {
                thumbUrl = "";
                return false;
            }

        }



        protected override IEnumerable<string> SupportedExtensions => new List<string> { ".pdf" };
    }
}
