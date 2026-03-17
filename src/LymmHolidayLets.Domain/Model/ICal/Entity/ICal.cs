using LymmHolidayLets.Domain.Interface;

namespace LymmHolidayLets.Domain.Model.ICal.Entity
{
    public sealed class ICal(byte id, byte propertyId, string friendlyName, Guid identifier)
        : IAggregateRoot
    {
        // read

        public byte ID { get; init; } = id;
        public byte PropertyID { get; } = propertyId;
        public string FriendlyName { get; init; } = friendlyName;
        public Guid Identifier { get; } = identifier;
    }
}
