using System.Collections.Generic;
using UserModel = alma.Models.User;

namespace alma.Interfaces
{
    public interface IUserList
    {
        IList<UserModel> Users { get; set; }
    }
}