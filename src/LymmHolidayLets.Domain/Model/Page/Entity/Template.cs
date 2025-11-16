namespace LymmHolidayLets.Domain.Model.Page.Entity
{
    public sealed class Template
    {
        public Template(byte templateId, string description)
        {
            TemplateId = templateId;
            Description = description;
        }

        public byte TemplateId { get; }
        public string Description { get; }
    }
}
