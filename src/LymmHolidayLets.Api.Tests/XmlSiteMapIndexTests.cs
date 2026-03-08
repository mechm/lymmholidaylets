using System.Text;
using LymmHolidayLets.Api.Infrastructure.SiteMap;
using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Domain.Model.SiteMap.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;


namespace LymmHolidayLets.Api.Tests
{
    public class XmlSiteMapIndexTests
    {
        private static ActionContext BuildActionContext(
            Mock<ISiteMapQuery> siteMapQueryMock,
            Mock<IConfiguration> configMock)
        {
            var services = new Mock<IServiceProvider>();
            services.Setup(s => s.GetService(typeof(ISiteMapQuery))).Returns(siteMapQueryMock.Object);
            services.Setup(s => s.GetService(typeof(IConfiguration))).Returns(configMock.Object);

            var httpContext = new DefaultHttpContext
            {
                RequestServices = services.Object,
                Response = { Body = new MemoryStream() }
            };

            return new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        }

        private static async Task<string> GetResponseBodyAsync(ActionContext context)
        {
            context.HttpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(context.HttpContext.Response.Body, Encoding.UTF8);
            return await reader.ReadToEndAsync();
        }

        [Fact]
        public async Task ExecuteResultAsync_SetsContentTypeToTextXml()
        {
            var siteMapQuery = new Mock<ISiteMapQuery>();
            siteMapQuery.Setup(q => q.GetAll()).Returns([]);

            var config = new Mock<IConfiguration>();
            config.Setup(c => c["Keys:SiteMaps"]).Returns((string?)null);

            var context = BuildActionContext(siteMapQuery, config);
            var result = new XmlSiteMapIndex();

            await result.ExecuteResultAsync(context);

            Assert.Equal("text/xml", context.HttpContext.Response.ContentType);
        }

        [Fact]
        public async Task ExecuteResultAsync_WritesValidXml_WithUrlsetElement()
        {
            var siteMapQuery = new Mock<ISiteMapQuery>();
            siteMapQuery.Setup(q => q.GetAll()).Returns([]);

            var config = new Mock<IConfiguration>();
            config.Setup(c => c["Keys:SiteMaps"]).Returns((string?)null);

            var context = BuildActionContext(siteMapQuery, config);
            var result = new XmlSiteMapIndex();

            await result.ExecuteResultAsync(context);

            var xml = await GetResponseBodyAsync(context);
            Assert.Contains("urlset", xml);
            Assert.Contains("http://www.sitemaps.org/schemas/sitemap/0.9", xml);
        }

        [Fact]
        public async Task ExecuteResultAsync_NoSiteMapsConfig_WritesEmptyUrlset()
        {
            var siteMapQuery = new Mock<ISiteMapQuery>();
            siteMapQuery.Setup(q => q.GetAll()).Returns([new SiteMap("/page1"), new SiteMap("/page2")]);

            var config = new Mock<IConfiguration>();
            config.Setup(c => c["Keys:SiteMaps"]).Returns((string?)null);

            var context = BuildActionContext(siteMapQuery, config);
            var result = new XmlSiteMapIndex();

            await result.ExecuteResultAsync(context);

            var xml = await GetResponseBodyAsync(context);
            Assert.DoesNotContain("<loc>", xml);
        }

        [Fact]
        public async Task ExecuteResultAsync_WithSiteMapBaseUrl_WritesBaseUrlAsEntry()
        {
            var siteMapQuery = new Mock<ISiteMapQuery>();
            siteMapQuery.Setup(q => q.GetAll()).Returns([]);

            var config = new Mock<IConfiguration>();
            config.Setup(c => c["Keys:SiteMaps"]).Returns("https://example.com");

            var context = BuildActionContext(siteMapQuery, config);
            var result = new XmlSiteMapIndex();

            await result.ExecuteResultAsync(context);

            var xml = await GetResponseBodyAsync(context);
            Assert.Contains("<loc>https://example.com</loc>", xml);
        }

        [Fact]
        public async Task ExecuteResultAsync_WithSiteMapUrlsAndBase_WritesCombinedUrls()
        {
            var siteMapQuery = new Mock<ISiteMapQuery>();
            siteMapQuery.Setup(q => q.GetAll()).Returns(
            [
                new SiteMap("/about"),
                new SiteMap("/contact")
            ]);

            var config = new Mock<IConfiguration>();
            config.Setup(c => c["Keys:SiteMaps"]).Returns("https://example.com");

            var context = BuildActionContext(siteMapQuery, config);
            var result = new XmlSiteMapIndex();

            await result.ExecuteResultAsync(context);

            var xml = await GetResponseBodyAsync(context);
            Assert.Contains("<loc>https://example.com/about</loc>", xml);
            Assert.Contains("<loc>https://example.com/contact</loc>", xml);
        }

        [Fact]
        public async Task ExecuteResultAsync_WithMultipleSiteMapBases_WritesCombinedUrlsForEach()
        {
            var siteMapQuery = new Mock<ISiteMapQuery>();
            siteMapQuery.Setup(q => q.GetAll()).Returns([new SiteMap("/page")]);

            var config = new Mock<IConfiguration>();
            config.Setup(c => c["Keys:SiteMaps"]).Returns("https://site1.com,https://site2.com");

            var context = BuildActionContext(siteMapQuery, config);
            var result = new XmlSiteMapIndex();

            await result.ExecuteResultAsync(context);

            var xml = await GetResponseBodyAsync(context);
            Assert.Contains("<loc>https://site1.com/page</loc>", xml);
            Assert.Contains("<loc>https://site2.com/page</loc>", xml);
        }

        [Fact]
        public async Task ExecuteResultAsync_UrlTrailingSlash_DoesNotDoubleSlash()
        {
            var siteMapQuery = new Mock<ISiteMapQuery>();
            siteMapQuery.Setup(q => q.GetAll()).Returns([new SiteMap("/about")]);

            var config = new Mock<IConfiguration>();
            config.Setup(c => c["Keys:SiteMaps"]).Returns("https://example.com/");

            var context = BuildActionContext(siteMapQuery, config);
            var result = new XmlSiteMapIndex();

            await result.ExecuteResultAsync(context);

            var xml = await GetResponseBodyAsync(context);
            Assert.DoesNotContain("//about", xml);
            Assert.Contains("<loc>https://example.com/about</loc>", xml);
        }
    }
}
