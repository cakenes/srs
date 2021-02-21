using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Srs {

    public partial class Service {

        public Data.DeckFull CompleteDeck;

        // Create review deck
        public Task<Data.PartialDeck> CreateReviewDeck(float reviewAmount, float reviewPercent) {

            Data.PartialDeck reviewDeck = new Data.PartialDeck();
            reviewDeck.Cards = new SortedDictionary<int, Data.Card>();
            Data.DeckFull fullDeck = Access.Current.DeckDict[CompleteDeck.Id];
            List<Data.DoubleInt> reviewOld = new List<Data.DoubleInt>();
            
            if (Access.Current.GuidList[ConnectionId].Review.ContainsKey(CompleteDeck.Id)) reviewOld = Access.Current.GuidList[ConnectionId].Review[CompleteDeck.Id].CardList;

            int oldAmount = (int)MathF.Round(reviewAmount * ((100 - reviewPercent) / 100), 0);
            Random random = new Random(DateTime.Now.Millisecond);

            if (oldAmount > reviewOld.Count) oldAmount = reviewOld.Count;

            // Add old cards 
            for (int i = 0; i < oldAmount * 2; i++)
            {
                var oldCard = reviewOld.ElementAt(random.Next(0, reviewOld.Count));
                if (!reviewDeck.Cards.ContainsKey(oldCard.One)) reviewDeck.Cards.Add(oldCard.One, CompleteDeck.Cards[oldCard.One]);
                if (reviewDeck.Cards.Count == oldAmount) break;
            }

            // Add new cards
            for (int i = 0; i < reviewAmount - oldAmount * 5; i++)
            {
                var newCard = CompleteDeck.Cards.ElementAt(random.Next(0, CompleteDeck.Cards.Count));
                
                if (!reviewDeck.Cards.ContainsKey(newCard.Key)) reviewDeck.Cards.Add(newCard.Key, CompleteDeck.Cards[newCard.Key]);
                if (reviewDeck.Cards.Count == reviewAmount) break;
            }

            return Task.FromResult(reviewDeck);
        }

        // Review Deck
        public Task<Data.ReturnInfo> ReviewDeckAsync(int index) {
            CompleteDeck = Access.Current.DeckDict[index];
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

            return Task.FromResult(new Data.ReturnInfo());
        }

        // Load Info
        public Task<List<Data.DeckInfo>> LoadDeckInfoAsync() {
            return Task.FromResult(Access.Current.InfoList);
        }
    }
}
