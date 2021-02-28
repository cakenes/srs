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
            if (name == null || password == null) return Task.FromResult(CreateReturn(false, "Login", "Username or password missing", "danger"));
            else if (name.Length < 3 || name.Length > 16) return Task.FromResult(CreateReturn(false, "Login", "Incorrect username", "warning"));
            else if (password.Length < 5 || password.Length > 20) return Task.FromResult(CreateReturn(false, "Login", "Incorrect password", "warning"));
            Data.ReturnInfo info = Access.Current.LoginUser(name,password);
            if (info.Success) { origin.UserId = new Guid(info.Message); info.Message = "Login successful"; }
            return Task.FromResult(info);
        }

        // User logout
        public Task<Data.ReturnInfo> LogoutUserAsync(ServiceData origin) {
            origin.UserId = null; origin.Review = new Data.DeckFull { Cards = new SortedDictionary<int, Data.Card>() };
            return Task.FromResult(CreateReturn(true, "Logout",  "Logout successful", "success"));
        }

        // Create user
        public Task<Data.ReturnInfo> CreateUserAsync(string name, string password) {
            if (name == null || password == null) return Task.FromResult(CreateReturn(false, "Register", "Username or password missing", "danger"));
            else if (name.Length < 3) return Task.FromResult(CreateReturn(false, "Register", "Username is too short, must be over 3", "warning"));
            else if (password.Length < 5) return Task.FromResult(CreateReturn(false, "Register", "Password too short, must be over 5", "warning"));
            else if (name.Length > 16) return Task.FromResult(CreateReturn(false, "Register", "Username is too long, keep it under 16", "warning"));
            else if (password.Length > 20) return Task.FromResult(CreateReturn(false, "Register", "Password too long, keep it under 20", "warning"));

            Data.ReturnInfo info = Access.Current.CreateUser(name,password);
            return Task.FromResult(info);
        }

        public Task<Data.ReturnInfo> ChangeUserPassword(ServiceData origin, string current, string password, string confirm) {
            if (current == null) return Task.FromResult(CreateReturn(false, "Change Password", "Current password missing", "warning"));
            else if (password == null || confirm == null) return Task.FromResult(CreateReturn(false, "Change Password", "Passwords do not match", "warning"));
            else if (current == password) return Task.FromResult(CreateReturn(false, "Change Password", "Cannot change into current password", "warning"));
            else if (password != confirm) return Task.FromResult(CreateReturn(false, "Change Password", "Passwords do not match", "warning"));
            Data.ReturnInfo info = Access.Current.ChangePassword(origin.UserId, current, password);
            return Task.FromResult(info);
        }

        // Get user name
        public Task<string> GetUserNameAsync(ServiceData origin) {
            return Task.FromResult(Access.Current.GetUserName(origin.UserId));
        }

        // Create Return
        public Data.ReturnInfo CreateReturn(bool success, string title, string message, string type) {
            return new Data.ReturnInfo {Success = success, Message = message, Type = type};
        }
    }
}
