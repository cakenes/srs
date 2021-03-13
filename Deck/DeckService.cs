using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Srs {

    public partial class ServiceData {
        public Data.PartialDeck Review = new Data.PartialDeck();
        public Data.DeckFull Create = new Data.DeckFull();
        public void ResetReview() { Review = new Data.PartialDeck(); }
        public void ResetCreate() { Create = new Data.DeckFull(); }
    }

    public partial class Service {

        // Load info list
        public Task<List<Data.DeckInfo>> LoadDeckInfoAsync() {
            return Task.FromResult(Access.Current.InfoList.Values.ToList());
        }

        // Load modify list
        public Task<List<Data.DeckInfo>> CreateModifyListAsync(ServiceData origin) {
            return Task.FromResult(Access.Current.CreateModifyList(origin));
        }

        // Create deck
        public Task<Data.ReturnInfo> CreateDeckAsync(ServiceData origin) {
            Data.ReturnInfo returnInfo;
            if (origin.Create.Name.Length < 6 || origin.Create.Name.Length > 50) returnInfo = CreateReturn(false, "Create Deck", "Name must be between 6 and 50 characters", "warning");
            else if (origin.Create.Cards.Count < 10) returnInfo = CreateReturn(false, "Create Deck", "Must contain atleast 10 cards", "warning");
            else if (!Access.Current.ValidateOrigin(origin)) { origin = new ServiceData(); returnInfo = CreateReturn(false, "Modify", "Could not validate user", "danger"); } 
            else returnInfo = Access.Current.CreateDeckAsync(origin);
            return Task.FromResult(returnInfo);
        }

        // Select modify deck
        public Task<Data.ReturnInfo> SelectModifyDeckAsync(ServiceData origin) {
            Data.ReturnInfo returnInfo;
            Data.DeckFull deckFull = DeckCache.Current.LoadDeck(origin.Create.Name);
            if (deckFull.Author != origin.User.Name) returnInfo = CreateReturn(false, "Modify", "Could not validate user", "danger");
            else if (origin.Create.Name == "") returnInfo = CreateReturn(false, "Modify", "Deck could not be found", "danger");
            else if (!Access.Current.ValidateOrigin(origin)) { origin = new ServiceData(); returnInfo = CreateReturn(false, "Modify", "Could not validate user", "danger"); }
            else { origin.Create = deckFull; origin.Create.Old = deckFull.Name; returnInfo = CreateReturn(true, "Modify", "Success", "success"); }
            return Task.FromResult(returnInfo);
        }

        public Task<Data.ReturnInfo> ModifyDeckAsync(ServiceData origin) {
            Data.ReturnInfo returnInfo;
            if (origin.Create.Name.Length < 6 || origin.Create.Name.Length > 50) returnInfo = CreateReturn(false, "Create Deck", "Name must be between 6 and 50 characters", "warning");
            else if (origin.Create.Cards.Count < 10) returnInfo = CreateReturn(false, "Create Deck", "Must contain atleast 10 cards", "warning");
            else if (origin.Create.Author != origin.User.Name) returnInfo = CreateReturn(false, "Modify", "Could not validate user", "danger");
            else if (!Access.Current.ValidateOrigin(origin)) { origin = new ServiceData(); returnInfo = CreateReturn(false, "Modify", "Could not validate user", "danger"); }
            else { returnInfo = Access.Current.ModifyDeck(origin); origin.ResetCreate(); }
            return Task.FromResult(returnInfo);
        }

        // Select deck for reviewing
        public Task<Data.ReturnInfo> SelectReviewDeckAsync(ServiceData origin, string name) {
            origin.Review = new Data.PartialDeck { Name = name };
            return Task.FromResult(CreateReturn(true, "Review", "Success", "success"));
        }

        // Create deck for reviewing
        public Task<Data.ReturnInfo> CreateReviewDeckAsync(ServiceData origin, float reviewAmount, float reviewPercent) {

            Data.DeckFull fullDeck = DeckCache.Current.LoadDeck(origin.Review.Name);
            Data.User fullUser = UserCache.Current.LoadUser(origin.Review.Name);

            origin.Review.Cards.Clear();

            if (fullDeck.Name == "") return Task.FromResult(CreateReturn(false, "Review", "Deck could not be found", "danger"));

            Random random = new Random(DateTime.Now.Millisecond);
            int oldAmount = (int)MathF.Round(reviewAmount * ((100 - reviewPercent) / 100), 0);
            Dictionary<int,int> oldReview = new Dictionary<int, int>();

            if (fullUser.Review.ContainsKey(origin.Review.Id)) oldReview = fullUser.Review[origin.Review.Id];
            if (oldAmount > oldReview.Count) oldAmount = oldReview.Count;

            // Add old cards 
            for (int i = 0; i < oldAmount * 2; i++) {
                var oldCard = oldReview.ElementAt(random.Next(0, oldReview.Count));
                if (!origin.Review.Cards.ContainsKey(oldCard.Key)) origin.Review.Cards.Add(oldCard.Key, fullDeck.Cards[oldCard.Key]);
                if (origin.Review.Cards.Count >= oldAmount) break;
            }

            // Add new cards
            for (int i = 0; i < (reviewAmount - oldAmount) * 5; i++) {
                var newCard = fullDeck.Cards.ElementAt(random.Next(0, fullDeck.Cards.Count));
                if (!origin.Review.Cards.ContainsKey(newCard.Key)) origin.Review.Cards.Add(newCard.Key, fullDeck.Cards[newCard.Key]);
                if (origin.Review.Cards.Count >= reviewAmount) break;
            }

            return Task.FromResult(CreateReturn(true));
        }

        // Return review deck
        public Task<Data.ReturnInfo> ReviewDeckReturnAsync(ServiceData origin, Dictionary<int,int> reviewReturn) {  
            if (origin.User.Id == null) return Task.FromResult(CreateReturn(false, "Review Done", "You got " + reviewReturn.Values.Where(x => x == 1).Count() + " correct", "success"));

            if (!origin.User.Review.ContainsKey(origin.Review.Id)) origin.User.Review[origin.Review.Id] = new Dictionary<int, int>();

            foreach (var card in reviewReturn) {
                if (!origin.User.Review[origin.Review.Id].ContainsKey(card.Key)) origin.User.Review[origin.Review.Id].Add(card.Key, card.Value);
                else origin.User.Review[origin.Review.Id][card.Key] += card.Value;
                origin.User.Review[origin.Review.Id][card.Key] = Math.Clamp(origin.User.Review[origin.Review.Id][card.Key], 0, 5);
            }

            Data.ReturnInfo returnInfo = Access.Current.ReturnReviewDeck(origin);
            if (returnInfo.Success) return Task.FromResult(CreateReturn(true, "Review Done", "Progress saved, you got " + reviewReturn.Values.Where(x => x == 1).Count() + " correct", "success"));
            return Task.FromResult(returnInfo);
        }
    }
}
