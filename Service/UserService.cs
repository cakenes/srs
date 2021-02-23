using System;
using System.Threading.Tasks;

namespace Srs {

    public partial class Service {

		public Guid? ConnectionId;

		// User login
        public Task<Data.ReturnInfo> LoginUserAsync(string name, string password) {
            ConnectionId = Access.Current.LoginUser(name,password);
            if (ConnectionId == null) { return Task.FromResult(new Data.ReturnInfo {Success = false, Message="Incorrect username / password"}); }
            return Task.FromResult(new Data.ReturnInfo {Success = true, Message = name});
        }

        // Create user
        public Task<Data.ReturnInfo> CreateUserAsync(string name, string password) {
            if (Access.Current.CreateUser(name,password)) { return Task.FromResult(new Data.ReturnInfo {Success = true, Message = "User created"}); }
            else { return Task.FromResult(new Data.ReturnInfo {Success = false, Message = "User already exists"}); }
        }
    }
}
