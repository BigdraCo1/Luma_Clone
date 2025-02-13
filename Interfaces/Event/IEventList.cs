using System.Collections.Generic;
using EventModel = alma.Models.Event;

namespace alma.Interfaces
{
    public interface IEventList
    {
        IList<EventModel> Events { get; set; }
    }
}