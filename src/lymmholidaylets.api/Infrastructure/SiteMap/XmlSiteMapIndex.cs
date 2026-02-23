using System.Xml;
using LymmHolidayLets.Application.Interface.Query;
using Microsoft.AspNetCore.Mvc;

namespace LymmHolidayLets.Api.Infrastructure.SiteMap
{
    public sealed class XmlSiteMapIndex : ActionResult
    {
        public override async Task ExecuteResultAsync(ActionContext context)
        {
            var siteMapQuery = context.HttpContext.RequestServices.GetRequiredService<ISiteMapQuery>();
            var config = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();

            var siteMapUrls = siteMapQuery.GetAll().ToList();
            var siteMaps = config["Keys:SiteMaps"];

            // Output content type
            context.HttpContext.Response.ContentType = "text/xml";

            await using var writer = XmlWriter.Create(context.HttpContext.Response.Body, new XmlWriterSettings { Async = true, Indent = true });
            await writer.WriteStartDocumentAsync();

            await writer.WriteStartElementAsync(null, "urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");

            if (!string.IsNullOrEmpty(siteMaps))
            {
                var siteMapsToDisplay = siteMaps.Split([','], StringSplitOptions.RemoveEmptyEntries).ToList();

                foreach (var siteMapItem in siteMapsToDisplay)
                {
                    // Add the base siteMapItem URL
                    await writer.WriteStartElementAsync(null, "url", null);
                    await writer.WriteStartElementAsync(null, "loc", null);
                    await writer.WriteStringAsync(siteMapItem);
                    await writer.WriteEndElementAsync();
                    await writer.WriteEndElementAsync();

                    foreach (var siteMapUrl in siteMapUrls)
                    {
                        await writer.WriteStartElementAsync(null, "url", null);
                        await writer.WriteStartElementAsync(null, "loc", null);
                        await writer.WriteStringAsync($"{siteMapItem.TrimEnd('/')}/{siteMapUrl.Url.TrimStart('/')}");
                        await writer.WriteEndElementAsync();
                        await writer.WriteEndElementAsync();
                    }
                }
            }

            await writer.WriteEndElementAsync(); // url set
            await writer.WriteEndDocumentAsync();
            await writer.FlushAsync();
        }
    }
}