using System;
using System.Linq;
using System.Threading.Tasks;

namespace Srs {

    public partial class ServiceData {
        public Data.User User = new Data.User { Name = "User" };
        public Guid? UserId = null;
    }

    public partial class Service {

		// User login
        public Task<Data.ReturnInfo> LoginUserAsync(ServiceData origin) {
            if (origin.User.Name == null || origin.User.Password == null) return Task.FromResult(CreateReturn(false, "Login", "Username or password missing", "danger"));
            else if (origin.User.Name.Length < 3 || origin.User.Name.Length > 16) return Task.FromResult(CreateReturn(false, "Login", "Incorrect username or password", "warning"));
            else if (origin.User.Password.Length < 5 || origin.User.Password.Length > 20) return Task.FromResult(CreateReturn(false, "Login", "Incorrect username or password", "warning"));
            Data.ReturnInfo returnInfo = Access.Current.LoginUser(origin);
            return Task.FromResult(returnInfo);
        }

        // User logout
        public Task<Data.ReturnInfo> LogoutUserAsync(ServiceData origin) {
            if (origin.User.Name == "" || origin.UserId == null) return Task.FromResult(CreateReturn(false, "Logout", "You were never logged in", "danger"));
            Data.ReturnInfo returnInfo = Access.Current.LogoutUser(origin);
            return Task.FromResult(returnInfo);
        }

        // Create user
        public Task<Data.ReturnInfo> CreateUserAsync(ServiceData origin) {
            if (origin.User.Name == null || origin.User.Password == null) return Task.FromResult(CreateReturn(false, "Register", "Username or password missing", "danger"));
            else if (UserCache.Current.UserExists(origin.User.Name)) return Task.FromResult(CreateReturn(false, "Register", "Username is already in use", "danger"));
            else if (origin.User.Name.Any(x => !char.IsLetterOrDigit(x))) return Task.FromResult(CreateReturn(false, "Register", "Username cannot contain special characters", "warning"));
            else if (origin.User.Name.Length < 3 || origin.User.Name.Length > 16) return Task.FromResult(CreateReturn(false, "Register", "Username must be between 3 and 16 characters", "warning"));
            else if (origin.User.Password.Length < 5 || origin.User.Password.Length > 20) return Task.FromResult(CreateReturn(false, "Register", "Password must be between 5 and 20 characters", "warning"));
            Data.ReturnInfo returnInfo = Access.Current.CreateUserAsync(origin);
            return Task.FromResult(returnInfo);
        }

        // Password change
        public Task<Data.ReturnInfo> ChangePasswordAsync(ServiceData origin, string password, string confirm) {
            if (!Access.Current.ValidateOrigin(origin)) { origin = new ServiceData(); return Task.FromResult(CreateReturn(false, "Modify", "Could not validate user", "danger")); } 
            else if (password == null) return Task.FromResult(CreateReturn(false, "Register", "Username or password missing", "danger"));
            else if (password != confirm) return Task.FromResult(CreateReturn(false, "Change Password", "Passwords do not match", "warning"));
            else if (password.Length < 5 || password.Length > 20) return Task.FromResult(CreateReturn(false, "Register", "Password must be between 5 and 20 characters", "warning"));
            Data.ReturnInfo returnInfo = Access.Current.ChangePasswordAsync(origin, password);
            return Task.FromResult(returnInfo);
        }

        // Create Return
        public Data.ReturnInfo CreateReturn(bool success, string title = "", string message = "", string type = "") {
            return new Data.ReturnInfo {Success = success, Title = title, Message = message, Type = type};
        }
    }
}
