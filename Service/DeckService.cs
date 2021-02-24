using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Srs {

    public partial class Service {

        public Data.DeckFull Deck = new Data.DeckFull {Cards = new SortedDictionary<int, Data.Card>()};
    
        // Create review deck
        public Task<Data.PartialDeck> CreateReviewDeckAsync(float reviewAmount, float reviewPercent) {
            // Initialize
            Data.PartialDeck partialDeck = new Data.PartialDeck {Cards = new SortedDictionary<int, Data.Card>()};
            Data.User reviewUser = Access.Current.GetUser(ConnectionId);
            Dictionary<int,int> reviewOld = new Dictionary<int, int>();
            Random random = new Random(DateTime.Now.Millisecond);
            int oldAmount = (int)MathF.Round(reviewAmount * ((100 - reviewPercent) / 100), 0);

            if (reviewUser.Review.ContainsKey(Deck.Id)) reviewOld = reviewUser.Review[Deck.Id];
            if (oldAmount > reviewOld.Count) oldAmount = reviewOld.Count;

            // Add old cards 
            for (int i = 0; i < oldAmount * 2; i++) {
                var oldCard = reviewOld.ElementAt(random.Next(0, reviewOld.Count));
                if (!partialDeck.Cards.ContainsKey(oldCard.Key)) partialDeck.Cards.Add(oldCard.Key, Deck.Cards[oldCard.Key]);
                if (partialDeck.Cards.Count >= oldAmount) break;
            }

            // Add new cards
            for (int i = 0; i < reviewAmount - oldAmount * 5; i++) {
                var newCard = Deck.Cards.ElementAt(random.Next(0, Deck.Cards.Count));
                if (!partialDeck.Cards.ContainsKey(newCard.Key)) partialDeck.Cards.Add(newCard.Key, Deck.Cards[newCard.Key]);
                if (partialDeck.Cards.Count >= reviewAmount) break;
            }

            return Task.FromResult(partialDeck);
        }

        // Select deck
        public Task<Data.ReturnInfo> SelectReviewDeckAsync(int index) {
            Deck = Access.Current.GetDeckList(index);
            if (Deck.Id == 0) return Task.FromResult(CreateReturn(false, "Deck could not be found", "danger")); 
            return Task.FromResult(CreateReturn(true, "Success", "success"));
        }

        // Create deck
        public Task<Data.ReturnInfo> CreateDeckAsync(Data.DeckFull deck) {
            deck.Author = Access.Current.GetUser(ConnectionId).Name;
            Data.ReturnInfo info = Access.Current.CreateDeck(deck);
            return Task.FromResult(info);
        }


        // Return review deck
        public Task<Data.ReturnInfo> ReviewDeckReturnAsync(Dictionary<int,int> reviewReturn) {  
            if (ConnectionId == null) return Task.FromResult(CreateReturn(false, "Not logged in, progress wont be saved", "warning")); // Not logged in
            Data.User reviewUser = Access.Current.GetUser(ConnectionId);

            if (!reviewUser.Review.ContainsKey(Deck.Id)) reviewUser.Review[Deck.Id] = new Dictionary<int, int>();

            foreach (var card in reviewReturn)
            {
                if (!reviewUser.Review[Deck.Id].ContainsKey(card.Key)) reviewUser.Review[Deck.Id].Add(card.Key, card.Value);
                else reviewUser.Review[Deck.Id][card.Key] += card.Value;
                if (reviewUser.Review[Deck.Id][card.Key] == 6) reviewUser.Review[Deck.Id][card.Key] = 5;
                if (reviewUser.Review[Deck.Id][card.Key] == -1) reviewUser.Review[Deck.Id][card.Key] = 0;
            }

            Access.Current.UpdateUser(ConnectionId);
            return Task.FromResult(CreateReturn(true, "Done! Progress saved", "success"));
        }

        // Load Info
        public Task<List<Data.DeckInfo>> LoadDeckInfoAsync() {
            return Task.FromResult(Access.Current.GetInfoList());
        }
    }
}
