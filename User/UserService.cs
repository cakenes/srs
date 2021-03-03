using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Srs {

    public partial class ServiceData {
        public Data.User User = new Data.User { Name = "User" };
        public Guid? UserId;
    }

    public partial class Service {

		// User login
        public async Task<Data.ReturnInfo> LoginUserAsync(ServiceData origin, string name, string password) {
            if (name == null || password == null) return CreateReturn(false, "Login", "Username or password missing", "danger");
            else if (name.Length < 3 || name.Length > 16) return CreateReturn(false, "Login", "Incorrect username or password", "warning");
            else if (password.Length < 5 || password.Length > 20) return CreateReturn(false, "Login", "Incorrect username or password", "warning");
            Data.ReturnInfo returnInfo = await Access.Current.LoginUser(origin, name, password);
            return returnInfo;
        }

        // User logout
        public Task<Data.ReturnInfo> LogoutUserAsync(ServiceData origin) {
            origin = new ServiceData();
            return Task.FromResult(CreateReturn(true, "Logout",  "Logout successful", "success"));
        }

        // Create user
        public async Task<Data.ReturnInfo> CreateUserAsync(string name, string password) {
            if (name == null || password == null) return CreateReturn(false, "Register", "Username or password missing", "danger");
            else if (UserCache.Current.UserExists(name)) return CreateReturn(false, "Register", "Username is already in use", "danger");
            else if (name.Any(x => !char.IsLetterOrDigit(x))) return CreateReturn(false, "Register", "Username cannot contain special characters", "warning");
            else if (name.Length < 3 || name.Length > 16) return CreateReturn(false, "Register", "Username must be between 3 and 16 characters", "warning");
            else if (password.Length < 5 || password.Length > 20) return CreateReturn(false, "Register", "Password must be between 5 and 20 characters", "warning");
            Data.ReturnInfo returnInfo = await Access.Current.CreateUserAsync(name,password);
            return returnInfo;
        }

        // Password change
        public async Task<Data.ReturnInfo> ChangePasswordAsync(ServiceData origin, string password, string confirm) {
            if (password == null) return CreateReturn(false, "Register", "Username or password missing", "danger");
            else if (password != confirm) return CreateReturn(false, "Change Password", "Passwords do not match", "warning");
            else if (password.Length < 5 || password.Length > 20) return CreateReturn(false, "Register", "Password must be between 5 and 20 characters", "warning");
            Data.ReturnInfo returnInfo = await Access.Current.ChangePasswordAsync(origin, password);
            return returnInfo;
        }

        // Create Return
        public Data.ReturnInfo CreateReturn(bool success, string title = "", string message = "", string type = "") {
            return new Data.ReturnInfo {Success = success, Title = title, Message = message, Type = type};
        }
    }
}
