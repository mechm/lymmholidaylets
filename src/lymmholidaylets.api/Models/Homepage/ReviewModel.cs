namespace LymmHolidayLets.Api.Models.Homepage;

public sealed class ReviewModel(
    string friendlyName,
    string? company,
    string description,
    string name,
    string? position,
    DateTime dateTimeAdded)
{
    public string FriendlyName { get; set; } = friendlyName;
    public string? Company { get; set; } = company;
    public string Description { get; set; } = description;
    public string Name { get; set; } = name;
    public string? Position { get; set; } = position;
    public DateTime DateTimeAdded { get; set; } = dateTimeAdded;
}