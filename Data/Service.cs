using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Srs.Data {

    public class Service {

        public string ConnectionId;

        // User login
        public Task<Data.Info> LoginUserAsync(string name, string password) {
            if (ConnectionId == null) { return Task.FromResult(new Data.Info {Success = false, Message="Incorrect username / password"}); }
            ConnectionId = Access.User.Current.Login(name,password);
            return Task.FromResult(new Data.Info {Success = true, Message = name});
        }

        // User register
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
