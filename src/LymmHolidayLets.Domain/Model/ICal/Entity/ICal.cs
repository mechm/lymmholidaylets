using LymmHolidayLets.Domain.Interface;

namespace LymmHolidayLets.Domain.Model.ICal.Entity
{
    public sealed class ICal : IAggregateRoot
    {
        // read
        public ICal(byte id, byte propertyID, string friendlyName, Guid identifier)
        {
            ID = id;
            PropertyID = propertyID;
            FriendlyName = friendlyName;
            Identifier = identifier;       
        }

        public byte ID { get; init; }
        public byte PropertyID { get; init; }
        public string FriendlyName { get; init; }
        public Guid Identifier { get; init; }
      
    }
}
