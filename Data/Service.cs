using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Srs.Data {

    public class Service {

        public Guid? ConnectionId;
        public Deck ReviewDeck;
        public SortedDictionary<int, Data.CardInfo> ReviewCards;

        // User login
        public Task<Data.Info> LoginUserAsync(string name, string password) {
            ConnectionId = Access.Current.LoginUser(name,password);
            if (ConnectionId == null) { return Task.FromResult(new Data.Info {Success = false, Message="Incorrect username / password"}); }
            return Task.FromResult(new Data.Info {Success = true, Message = name});
        }

        // Create User
        public Task<Data.Info> CreateUserAsync(string name, string password) {
            if (Access.Current.CreateUser(name,password)) { return Task.FromResult(new Data.Info {Success = true, Message = "User created"}); }
            else { return Task.FromResult(new Data.Info {Success = false, Message = "User already exists"}); }
        }

        // Review Deck
        public Task<Data.Info> ReviewDeckAsync(int index) {
            ReviewDeck = Access.Current.ReviewDeck(index);
            return Task.FromResult(new Data.Info {Success = true});
        }

        // Create Deck
        public Task<Data.Info> CreateDeckAsync(Data.Deck deck) {
            deck.Author = Access.Current.GuidList[ConnectionId].Name;
            Access.Current.CreateDeck(deck);
            return Task.FromResult(new Data.Info());
        }

        // Review Cards
        public Task<Data.Info> ReviewCardsAsync() {
            ReviewCards = Access.Current.GuidList[ConnectionId].Old;
            return Task.FromResult(new Data.Info());
        }

        // Load Info
        public Task<List<Data.DeckInfo>> LoadDeckInfoAsync() {
            return Task.FromResult(Access.Current.InfoList);
        }
    }
}
