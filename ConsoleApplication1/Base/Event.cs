using System;

namespace DataAnalyst.Base
{
    public class Event
    {
        public Event(DateTime eventDate, EventType eventType)
        {
            EventDate = eventDate;
            EventType = eventType;
        }

        public DateTime EventDate { get; set; }
        public EventType EventType { get; set; }
    }
}
