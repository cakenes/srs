using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Srs.Data {

    public class Service {

        public string ConnectionId;

        // User related
        public Task<Data.User> LoginUserAsync(Data.User input) {
            return Task.FromResult(input);
        }

        public Task<Data.User> GetUserDataAsync() {
            return Task.FromResult(new Data.User());
        }

        public Task<List<Data.DeckInfo>> LoadDeckList() {
            return Task.FromResult(new List<Data.DeckInfo>());
        }

        public Task<int> SaveDeck(Data.Deck deck) {
            //Access.Deck.Current

            
            return Task.FromResult(0);
        }

    }

}
