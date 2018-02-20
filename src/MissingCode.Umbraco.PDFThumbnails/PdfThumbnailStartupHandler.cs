using System;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace MissingCode.Umbraco.PdfThumbnail
{
    public class PdfThumbnailStartupHandler : IApplicationEventHandler
    {
        public void OnApplicationInitialized(UmbracoApplicationBase umbracoApplication,
            ApplicationContext applicationContext)
        {

        }

        public void OnApplicationStarting(UmbracoApplicationBase umbracoApplication,
            ApplicationContext applicationContext)
        {
            
        }


        public void OnApplicationStarted(UmbracoApplicationBase umbracoApplication,
            ApplicationContext applicationContext)
        {
            MediaService.Saved += MediaService_Saved;
        }

        private void MediaService_Saved(IMediaService sender, SaveEventArgs<IMedia> e)
        {
            foreach (var entity in e.SavedEntities)
            {
                if (entity.ContentType.Alias.Equals("folder", StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                try
                {
                    var file = entity.GetValue<string>("umbracoFile");
                    if (string.IsNullOrEmpty(file))
                    {
                        LogHelper.Warn<PdfThumbnailStartupHandler>($"File name is empty for entity {entity.Id}");
                        continue;
                    }

                    if (file.EndsWith(".pdf", StringComparison.InvariantCultureIgnoreCase))
                    {
                        using (
                            ApplicationContext.Current.ProfilingLogger.TraceDuration<PdfThumbnailStartupHandler>(
                                "Started Creating Thumbnail", "Completed Creating Thumbnail"))
                        {
                            ThumbnailGenerator.Generate(file);
                        }
                    }
                }
                catch (Exception exception)
                {
                    LogHelper.Error<PdfThumbnailStartupHandler>("Error During Thumbnail", exception);
                }
               

            }
        }


    }
}
