using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Srs {

    public partial class ServiceData {

        public Guid? UserId;
        
    }

    public partial class Service {

		// User login
        public Task<Data.ReturnInfo> LoginUserAsync(ServiceData origin, string name, string password) {
            if (name == null || password == null) return Task.FromResult(CreateReturn(false, "What is happening?", "danger"));
            else if (name.Length < 3) return Task.FromResult(CreateReturn(false, "Incorrect username", "warning"));
            else if (password.Length < 3) return Task.FromResult(CreateReturn(false, "Incorrect password", "warning"));
            Data.ReturnInfo info = Access.Current.LoginUser(name,password);
            if (!info.Success) return Task.FromResult(info);
            origin.UserId = new Guid(info.Message);
            return Task.FromResult(CreateReturn(true, name, "success"));
        }

        // User logout
        public Task<Data.ReturnInfo> LogoutUserAsync(ServiceData origin) {
            origin.UserId = null; origin.Deck = new Data.DeckFull { Cards = new SortedDictionary<int, Data.Card>() };
            return Task.FromResult(CreateReturn(true, "Logout successful", "success"));
        }

        // Create user
        public Task<Data.ReturnInfo> CreateUserAsync(string name, string password) {
            if (name == null || password == null) return Task.FromResult(CreateReturn(false, "What is happening?", "danger"));
            else if (name.Length < 3) return Task.FromResult(CreateReturn(false, "Username too short", "warning"));
            else if (password.Length < 3) return Task.FromResult(CreateReturn(false, "Password too short", "warning"));
            Data.ReturnInfo info = Access.Current.CreateUser(name,password);
            return Task.FromResult(info);
        }

        public Task<Data.ReturnInfo> ChangeUserPassword(ServiceData origin, string current, string password, string confirm) {
            if (current == null) return Task.FromResult(CreateReturn(false, "Current password missing", "warning"));
            else if (password == null || confirm == null) return Task.FromResult(CreateReturn(false, "Passwords do not match", "warning"));
            else if (current == password) return Task.FromResult(CreateReturn(false, "Cannot change into current password", "warning"));
            else if (password != confirm) return Task.FromResult(CreateReturn(false, "Passwords do not match", "warning"));
            Data.ReturnInfo info = Access.Current.ChangePassword(origin.UserId, current, password);
            return Task.FromResult(info);
        }

        // Get user name
        public Task<string> GetUserNameAsync(ServiceData origin) {
            return Task.FromResult(Access.Current.GetUserName(origin.UserId));
        }

        // Create Return
        public Data.ReturnInfo CreateReturn(bool success, string message, string type) {
            return new Data.ReturnInfo {Success = success, Message = message, Type = type};
        }
    }
}
