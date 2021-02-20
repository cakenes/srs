using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Srs.Data {

    public class Service {

        public Guid? ConnectionId;

        // User login
        public Task<Data.Info> LoginUserAsync(string name, string password) {
            ConnectionId = Access.User.Current.Login(name,password);
            if (ConnectionId == null) { return Task.FromResult(new Data.Info {Success = false, Message="Incorrect username / password"}); }
            return Task.FromResult(new Data.Info {Success = true, Message = name});
        }

        // Create User
        public Task<Data.Info> CreateUserAsync(string name, string password) {
            if (Access.User.Current.Create(name,password)) { return Task.FromResult(new Data.Info {Success = true, Message = "User created"}); }
            else { return Task.FromResult(new Data.Info {Success = false, Message = "User already exists"}); }
        }

        // Load Deck

        // Create Deck
        public Task<Data.Info> CreateDeckAsync(Data.Deck deck) {
            deck.Author = Access.User.Current.GuidList[ConnectionId].Name;
            Access.Deck.Current.Create(deck);
            return Task.FromResult(new Data.Info());
        }

        // Load Info
        public Task<List<Data.DeckInfo>> LoadDeckInfoAsync() {
            return Task.FromResult(Access.Deck.Current.InfoList);
        }
    }
}
