using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Srs.Data {

    public class Service {

        public string ConnectionId;

        // User login
        public Task<Data.Info> LoginUserAsync(string name, string password) {
            ConnectionId = Access.User.Current.Login(name,password);
            if (ConnectionId == null) { return Task.FromResult(new Data.Info {Success = false, Message="Incorrect username / password"}); }
            return Task.FromResult(new Data.Info {Success = true, Message = name});
        }

        // User register
        public Task<Data.Info> RegisterUserAsync(string name, string password) {
            if (Access.User.Current.Register(name,password)) { return Task.FromResult(new Data.Info {Success = true, Message = "User created"}); }
            else { return Task.FromResult(new Data.Info {Success = false, Message = "User already exists"}); }
        }

        // Load Deck
        // Load Info
        public Task<List<Data.DeckInfo>> LoadDeckInfoAsync() {
            if (Access.Deck.Current.InfoList == null) return Task.FromResult(new List<Data.DeckInfo>());
            return Task.FromResult(Access.Deck.Current.InfoList);
        }

        // Save Deck
        public Task<Data.Info> CreateDeckAsync(Data.Deck deck) {
            Access.Deck.Current.Save(deck);
            return Task.FromResult(new Data.Info());
        }

        public Task<Data.Info> EditDeckAsync(Data.Deck deck, int index) {
            Access.Deck.Current.Edit(deck, index);
            return Task.FromResult(new Data.Info());
        }
    }
}
