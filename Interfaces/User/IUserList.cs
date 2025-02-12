using System.Collections.Generic;
using UserModel = alma.Models.User; // Alias for the User class

namespace alma.Interfaces
{
    public interface IUserList
    {
        IList<UserModel> Users { get; set; }
    }
}