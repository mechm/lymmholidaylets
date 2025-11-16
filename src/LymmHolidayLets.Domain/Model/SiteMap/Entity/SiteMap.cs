using LymmHolidayLets.Domain.Interface;

namespace LymmHolidayLets.Domain.Model.SiteMap.Entity
{
    public sealed class SiteMap : IAggregateRoot
    {
        public SiteMap(string url)
        {
            Url = url;
        }

        public SiteMap(byte siteMapId, string url)
        {
            SiteMapId = siteMapId;
            Url = url;
        }

        public byte SiteMapId { get; set; }
        public string Url { get; set; }
    }
}