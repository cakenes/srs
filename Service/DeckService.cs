using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Srs {

    public partial class ServiceData {

        public Data.DeckFull Review = new Data.DeckFull {Cards = new SortedDictionary<int, Data.Card>()};
        public Data.DeckFull Create = new Data.DeckFull {Cards = new SortedDictionary<int, Data.Card>()};

    }

    public partial class Service {

        // Create review deck
        public Task<Data.PartialDeck> CreateReviewDeckAsync(ServiceData origin, float reviewAmount, float reviewPercent) {
            // Initialize
            Data.PartialDeck partialDeck = new Data.PartialDeck {Cards = new SortedDictionary<int, Data.Card>()};
            Data.User reviewUser = Access.Current.GetUser(origin.UserId);
            Dictionary<int,int> reviewOld = new Dictionary<int, int>();
            Random random = new Random(DateTime.Now.Millisecond);
            int oldAmount = (int)MathF.Round(reviewAmount * ((100 - reviewPercent) / 100), 0);

            if (reviewUser.Review.ContainsKey(origin.Review.Id)) reviewOld = reviewUser.Review[origin.Review.Id];
            if (oldAmount > reviewOld.Count) oldAmount = reviewOld.Count;

            // Add old cards 
            for (int i = 0; i < oldAmount * 2; i++) {
                var oldCard = reviewOld.ElementAt(random.Next(0, reviewOld.Count));
                if (!partialDeck.Cards.ContainsKey(oldCard.Key)) partialDeck.Cards.Add(oldCard.Key, origin.Review.Cards[oldCard.Key]);
                if (partialDeck.Cards.Count >= oldAmount) break;
            }

            // Add new cards
            for (int i = 0; i < (reviewAmount - oldAmount) * 5; i++) {
                var newCard = origin.Review.Cards.ElementAt(random.Next(0, origin.Review.Cards.Count));
                if (!partialDeck.Cards.ContainsKey(newCard.Key)) partialDeck.Cards.Add(newCard.Key, origin.Review.Cards[newCard.Key]);
                if (partialDeck.Cards.Count >= reviewAmount) break;
            }

            return Task.FromResult(partialDeck);
        }

        // Select deck
        public Task<Data.ReturnInfo> SelectReviewDeckAsync(ServiceData origin, int index) {
            origin.Review = Access.Current.GetDeckList(index);
            if (origin.Review.Id == 0) return Task.FromResult(CreateReturn(false, "Deck could not be found", "danger")); 
            return Task.FromResult(CreateReturn(true, "Success", "success"));
        }

        // Select modify deck
        public Task<Data.ReturnInfo> SelectModifyDeckAsync(ServiceData origin, int index) {
            origin.Create = Access.Current.GetDeckList(index);
            if (origin.Review.Id == 0) return Task.FromResult(CreateReturn(false, "Deck could not be found", "danger")); 
            return Task.FromResult(CreateReturn(true, "Success", "success"));
        }

        // Create deck
        public Task<Data.ReturnInfo> CreateDeckAsync(ServiceData origin, Data.DeckFull deck) {
            deck.Author = Access.Current.GetUser(origin.UserId).Name;
            Data.ReturnInfo info = Access.Current.CreateDeck(deck);
            return Task.FromResult(info);
        }

        // Modify deck
        public Task<Data.ReturnInfo> ModifyDeckAsync(ServiceData origin, Data.DeckFull deck) {
            Data.ReturnInfo info = Access.Current.ModifyDeck(deck, origin.UserId);
            return Task.FromResult(info);
        }

        // Return review deck
        public Task<Data.ReturnInfo> ReviewDeckReturnAsync(ServiceData origin, Dictionary<int,int> reviewReturn) {  
            if (origin.UserId == null) return Task.FromResult(CreateReturn(false, "Review Done! \n Not logged in, progress wont be saved", "warning"));
            Data.User reviewUser = Access.Current.GetUser(origin.UserId);

            if (!reviewUser.Review.ContainsKey(origin.Review.Id)) reviewUser.Review[origin.Review.Id] = new Dictionary<int, int>();

            foreach (var card in reviewReturn)
            {
                if (!reviewUser.Review[origin.Review.Id].ContainsKey(card.Key)) reviewUser.Review[origin.Review.Id].Add(card.Key, card.Value);
                else reviewUser.Review[origin.Review.Id][card.Key] += card.Value;
                reviewUser.Review[origin.Review.Id][card.Key] = Math.Clamp(reviewUser.Review[origin.Review.Id][card.Key], 0, 5);
            }

            Access.Current.UpdateUser(origin.UserId);
            return Task.FromResult(CreateReturn(true, "Done! Progress saved", "success"));
        }

        // Load info
        public Task<List<Data.DeckInfo>> LoadDeckInfoAsync() {
            return Task.FromResult(Access.Current.GetInfoList());
        }

        // Load modify info
        public Task<List<Data.DeckInfo>> LoadModifyInfoAsync(ServiceData origin) {
            return Task.FromResult(Access.Current.GetModifyList(origin.UserId));
        }
    }
}
