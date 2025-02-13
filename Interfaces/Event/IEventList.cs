using System.Collections.Generic;
using EventModel = alma.Models.Event; // Alias for the User class

namespace alma.Interfaces
{
    public interface IEventList
    {
        IList<EventModel> Events { get; set; }
    }
}