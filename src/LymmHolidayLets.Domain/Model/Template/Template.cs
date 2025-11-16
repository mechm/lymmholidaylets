using LymmHolidayLets.Domain.Interface;

namespace LymmHolidayLets.Domain.Model.Template
{
    public sealed class Template : IAggregateRoot
    {
        public Template(string description)
        {
            Description = description;
        }

        public Template(int templateId, string description)
        {
            TemplateId = templateId;
            Description = description;
        }

        public int TemplateId { get; set; }
        public string Description { get; set; }
    }
}
