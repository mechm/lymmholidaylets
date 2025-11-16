using LymmHolidayLets.Domain.Interface;

namespace LymmHolidayLets.Domain.ReadModel.Page
{
    public sealed class Template : ITable
    {
        public int TemplateId { get; set; }
        public required string Description { get; set; }
    }
}
