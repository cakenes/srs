using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Srs {

    public partial class Service {

        public Data.DeckFull ReviewDeck;
        public SortedDictionary<int, Data.DoubleInt> ReviewCards;

        // Review Deck
        public Task<Data.ReturnInfo> ReviewDeckAsync(int index) {
            ReviewDeck = Access.Current.ReviewDeck(index);
            return Task.FromResult(new Data.ReturnInfo {Success = true});
        }

        // Create Deck
        public Task<Data.ReturnInfo> CreateDeckAsync(Data.DeckFull deck) {
            deck.Author = Access.Current.GuidList[ConnectionId].Name;
            Access.Current.CreateDeck(deck);
            return Task.FromResult(new Data.ReturnInfo());
        }

        // Review Cards
        public Task<Data.ReturnInfo> ReviewCardsAsync() {
            ReviewCards = Access.Current.GuidList[ConnectionId].Old;
            return Task.FromResult(new Data.ReturnInfo());
        }

        // Load Info
        public Task<List<Data.DeckInfo>> LoadDeckInfoAsync() {
            return Task.FromResult(Access.Current.InfoList);
        }
    }
}
