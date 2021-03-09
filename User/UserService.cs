using System;
using System.Linq;
using System.Threading.Tasks;

namespace Srs {

    public partial class ServiceData {
        public Data.User User = new Data.User();
        public void ResetUser() { User = new Data.User(); }
    }

    public partial class Service {

		// User login
        public Task<Data.ReturnInfo> LoginUserAsync(ServiceData origin) {
            Data.ReturnInfo returnInfo;
            if (origin.User.Name == null || origin.User.Password == null) returnInfo = CreateReturn(false, "Login", "Username or password missing", "danger");
            else if (origin.User.Name.Length < 3 || origin.User.Name.Length > 16) returnInfo = CreateReturn(false, "Login", "Incorrect username or password", "warning");
            else if (origin.User.Password.Length < 5 || origin.User.Password.Length > 20) returnInfo = CreateReturn(false, "Login", "Incorrect username or password", "warning");
            returnInfo = Access.Current.ValidateLogin(origin);
            if (returnInfo.Success == true) Access.Current.LoginUser(origin);
            return Task.FromResult(returnInfo);
        }

        // User logout
        public Task<Data.ReturnInfo> LogoutUserAsync(ServiceData origin) {
            Data.ReturnInfo returnInfo;
            if (origin.User.Name == "" || origin.User.Id == null) returnInfo = CreateReturn(false, "Logout", "You were never logged in", "danger");
            else returnInfo = Access.Current.LogoutUser(origin);
            return Task.FromResult(returnInfo);
        }

        // Create user
        public Task<Data.ReturnInfo> CreateUserAsync(ServiceData origin) {
            Data.ReturnInfo returnInfo;
            if (origin.User.Name == null || origin.User.Password == null) returnInfo = CreateReturn(false, "Register", "Username or password missing", "danger");
            else if (UserCache.Current.UserExists(origin.User.Name)) returnInfo = CreateReturn(false, "Register", "Username is already in use", "danger");
            else if (origin.User.Name.Any(x => !char.IsLetterOrDigit(x))) returnInfo = CreateReturn(false, "Register", "Username cannot contain special characters", "warning");
            else if (origin.User.Name.Length < 3 || origin.User.Name.Length > 16) returnInfo = CreateReturn(false, "Register", "Username must be between 3 and 16 characters", "warning");
            else if (origin.User.Password.Length < 5 || origin.User.Password.Length > 20) returnInfo = CreateReturn(false, "Register", "Password must be between 5 and 20 characters", "warning");
            else returnInfo = Access.Current.CreateUserAsync(origin);
            return Task.FromResult(returnInfo);
        }

        // Password change
        public Task<Data.ReturnInfo> ChangePasswordAsync(ServiceData origin, string password, string confirm) {
            Data.ReturnInfo returnInfo;
            if (password == null) returnInfo = CreateReturn(false, "Register", "Username or password missing", "danger");
            else if (password != confirm) returnInfo = CreateReturn(false, "Change Password", "Passwords do not match", "warning");
            else if (password.Length < 5 || password.Length > 20) returnInfo = CreateReturn(false, "Register", "Password must be between 5 and 20 characters", "warning");
            else if (!Access.Current.ValidateOrigin(origin)) { origin = new ServiceData(); returnInfo = CreateReturn(false, "Modify", "Could not validate user", "danger"); } 
            returnInfo = Access.Current.ChangePasswordAsync(origin, password);
            return Task.FromResult(returnInfo);
        }

        // Create Return
        public Data.ReturnInfo CreateReturn(bool success, string title = "", string message = "", string type = "") {
            return new Data.ReturnInfo {Success = success, Title = title, Message = message, Type = type};
        }
    }
}
