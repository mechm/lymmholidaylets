using LymmHolidayLets.Domain.Interface;

namespace LymmHolidayLets.Domain.Model.FAQ.Entity
{
    public sealed class FAQ : IAggregateRoot
    {        
        // create
        public FAQ(byte propertyID, string question, string answer, bool visible)
        {
            PropertyID = propertyID;
            Question = question;
            Answer = answer;
            Visible = visible;
        }

        // update and read
        public FAQ(byte id, byte propertyID, string question, string answer, bool visible) : 
            this(propertyID, question, answer, visible)
        {
            ID = id;
        }

        public byte ID { get; set; }
        public byte PropertyID { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public bool Visible { get; set; }
    }
}