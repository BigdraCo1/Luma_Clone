using System.Collections.Generic;
using TagModel = alma.Models.Tag;

namespace alma.Interfaces
{
    public interface ITagList
    {
        IList<TagModel> Tags { get; set; }
    }
}