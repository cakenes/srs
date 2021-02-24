using System;
using System.Threading.Tasks;

namespace Srs {

    public partial class Service {

		public Guid? ConnectionId;

		// User login
        public Task<Data.ReturnInfo> LoginUserAsync(string name, string password) {
            if (name == null || password == null) CreateReturn(false, "What are you doing?", "warning");
            else if (name.Length < 3) CreateReturn(false, "Incorrect username", "warning");
            else if (password.Length < 3) CreateReturn(false, "Incorrect password", "warning");
            Data.ReturnInfo info = Access.Current.LoginUser(name,password);
            if (!info.Success) return Task.FromResult(info);
            ConnectionId = new Guid(info.Message);
            return Task.FromResult(CreateReturn(true, name, "success"));
        }

        // User logout
        public Task<Data.ReturnInfo> LogoutUserAsync() {
            ConnectionId = null;
            return Task.FromResult(CreateReturn(true, "Logout successful", "success"));
        }

        // Create user
        public Task<Data.ReturnInfo> CreateUserAsync(string name, string password) {
            if (Access.Current.CreateUser(name,password)) { return Task.FromResult(new Data.ReturnInfo {Success = true, Message = "User created"}); }
            else { return Task.FromResult(new Data.ReturnInfo {Success = false, Message = "User already exists"}); }
        }

        // Create Return
        public Data.ReturnInfo CreateReturn(bool success, string message, string type) {
            return new Data.ReturnInfo {Success = success, Message = message, Type = type};
        }

    }
}
