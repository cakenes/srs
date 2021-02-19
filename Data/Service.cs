using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Srs.Data {

    public class Service {

        public string ConnectionId;

        // User related
        public Task<Data.Info> LoginUserAsync(string name, string password) {
            ConnectionId = Access.User.Current.Login(name,password);
            if (ConnectionId == null) { return Task.FromResult(new Data.Info {Success = false, Message="Incorrect username / password"}); }
            else {
                return Task.FromResult(
                    new Data.Info {
                        Success = true,
                        Message = Access.User.Current.UserName(ConnectionId)
                    }
                );
            }
        }

        public Task<Data.Info> RegisterUserAsync(string name, string password) {
            if (Access.User.Current.Register(name,password)) {
                return Task.FromResult(
                    new Data.Info {
                        Success = true,
                        Message = "User created"
                    }
                );
            }
            
            else {
                return Task.FromResult(
                    new Data.Info {
                        Success = false,
                        Message = "User already exists"
                    }
                );
            }
        }
    }
}
