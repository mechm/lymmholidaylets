namespace LymmHolidayLets.Domain.Model.Property.ValueObject;

public sealed record PropertyOccupancy(
    byte MinimumAdults,
    byte MaximumGuests,
    byte MaximumAdults,
    byte MaximumChildren,
    byte MaximumInfants);
