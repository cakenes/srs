using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Srs {

    public partial class Service {

        public Data.DeckFull Deck = new Data.DeckFull {Cards = new SortedDictionary<int, Data.Card>()};
    
        public Task<Data.PartialDeck> ReviewDeckCreateAsync(float reviewAmount, float reviewPercent) {
            // Initialize
            Data.PartialDeck partialDeck = new Data.PartialDeck {Cards = new SortedDictionary<int, Data.Card>()};
            Data.User reviewUser = Access.Current.GuidList[ConnectionId];
            List<Data.DoubleInt> reviewOld = new List<Data.DoubleInt>();
            Random random = new Random(DateTime.Now.Millisecond);
            int oldAmount = (int)MathF.Round(reviewAmount * ((100 - reviewPercent) / 100), 0);

            if (reviewUser.Review.ContainsKey(Deck.Id)) reviewOld = reviewUser.Review[Deck.Id].Cards;
            if (oldAmount > reviewOld.Count) oldAmount = reviewOld.Count;


            // Add old cards 
            for (int i = 0; i < oldAmount * 2; i++) {
                var oldCard = reviewOld.ElementAt(random.Next(0, reviewOld.Count));
                if (!partialDeck.Cards.ContainsKey(oldCard.One)) partialDeck.Cards.Add(oldCard.One, Deck.Cards[oldCard.One]);
                if (partialDeck.Cards.Count == oldAmount) break;
            }

            // Add new cards
            for (int i = 0; i < reviewAmount - oldAmount * 5; i++) {
                var newCard = Deck.Cards.ElementAt(random.Next(0, Deck.Cards.Count));
                if (!partialDeck.Cards.ContainsKey(newCard.Key)) partialDeck.Cards.Add(newCard.Key, Deck.Cards[newCard.Key]);
                if (partialDeck.Cards.Count == reviewAmount) break;
            }

            return Task.FromResult(partialDeck);
        }

        // Review Deck
        public Task<Data.ReturnInfo> ReviewDeckAsync(int index) {
            Deck = Access.Current.DeckDict[index];
            return Task.FromResult(new Data.ReturnInfo {Success = true});
        }

        // Create Deck
        public Task<Data.ReturnInfo> DeckCreateAsync(Data.DeckFull deck) {
            deck.Author = Access.Current.GuidList[ConnectionId].Name;
            Access.Current.CreateDeck(deck);
            return Task.FromResult(new Data.ReturnInfo());
        }

        // Review Cards
        public Task<Data.ReturnInfo> ReviewCardsAsync() {

            return Task.FromResult(new Data.ReturnInfo());
        }

        // Load Info
        public Task<List<Data.DeckInfo>> LoadDeckInfoAsync() {
            return Task.FromResult(Access.Current.InfoList);
        }
    }
}
