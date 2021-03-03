using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Srs {

    public partial class ServiceData {
        public Data.PartialDeck Review;
        public Data.DeckFull Create;
        public Data.DeckFull Old;
    }

    public partial class Service {

        // Load info list
        public Task<List<Data.DeckInfo>> LoadDeckInfoAsync() {
            return Task.FromResult(Access.Current.InfoList.Values.ToList());
        }

        // Load modify list
        public Task<List<Data.DeckInfo>> LoadModifyListAsync(ServiceData origin) {
            return Task.FromResult(Access.Current.CreateModifyList(origin.User.Name));
        }

        // Create deck
        public async Task<Data.ReturnInfo> CreateDeckAsync(ServiceData origin) {
            if (origin.Create.Name.Length < 6 || origin.Create.Name.Length > 50) return CreateReturn(false, "Create Deck", "Name must be between 6 and 50 characters", "warning");



            if (!Access.Current.ValidateOrigin(origin)) { origin = new ServiceData(); return CreateReturn(false, "Modify", "Could not validate user", "danger"); } 
            // VALIDATE EVERYTHING.

            Data.ReturnInfo info = await Access.Current.CreateDeckAsync(origin);
            return info;
        }

        // Select modify deck
        public async Task<Data.ReturnInfo> SelectModifyDeckAsync(ServiceData origin, string name) {
            if (!Access.Current.ValidateOrigin(origin)) { origin = new ServiceData(); return CreateReturn(false, "Modify", "Could not validate user", "danger"); }  

            origin.Create = await DeckCache.Current.LoadDeck(name);
            origin.Old = origin.Create;

            if (origin.Create.Name == "") return CreateReturn(false, "Modify", "Deck could not be found", "danger");
            return CreateReturn(true, "Modify", "Success", "success");
        }

        public async Task<Data.ReturnInfo> ModifyDeckAsync(ServiceData origin) {



            if (!Access.Current.ValidateOrigin(origin)) { origin = new ServiceData(); return CreateReturn(false, "Modify", "Could not validate user", "danger"); } 

            // VALIDATE EVERYTHING.

            Data.ReturnInfo info = await Access.Current.ModifyDeck(origin);
            return info;
        }

        // Select deck for reviewing
        public Task<Data.ReturnInfo> SelectReviewDeckAsync(ServiceData origin, string name) {
            origin.Review = new Data.PartialDeck { Name = name };
            return Task.FromResult(CreateReturn(true, "Review", "Success", "success"));
        }

        // Create deck for reviewing
        public async Task<Data.ReturnInfo> CreateReviewDeckAsync(ServiceData origin, float reviewAmount, float reviewPercent) {

            Data.DeckFull fullDeck = await DeckCache.Current.LoadDeck(origin.Review.Name);
            Data.User fullUser = await UserCache.Current.LoadUser(origin.Review.Name);

            origin.Review = new Data.PartialDeck {Cards = new SortedDictionary<int, Data.Card>()};

            if (fullDeck.Name == null) return CreateReturn(false, "Review", "Deck could not be found", "danger");

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

            return CreateReturn(true, "Review", "Success", "success");
        }

        // Return review deck
        public async Task<Data.ReturnInfo> ReviewDeckReturnAsync(ServiceData origin, Dictionary<int,int> reviewReturn) {  
            if (origin.UserId == null) return CreateReturn(false, "Review Done", "You got " + reviewReturn.Values.Where(x => x == 1).Count() + " correct", "success");

            if (!origin.User.Review.ContainsKey(origin.Review.Id)) origin.User.Review[origin.Review.Id] = new Dictionary<int, int>();

            foreach (var card in reviewReturn) {
                if (!origin.User.Review[origin.Review.Id].ContainsKey(card.Key)) origin.User.Review[origin.Review.Id].Add(card.Key, card.Value);
                else origin.User.Review[origin.Review.Id][card.Key] += card.Value;
                origin.User.Review[origin.Review.Id][card.Key] = Math.Clamp(origin.User.Review[origin.Review.Id][card.Key], 0, 5);
            }

            Data.ReturnInfo returnInfo = await Access.Current.ReturnReviewDeck(origin);
            if (returnInfo.Success) return CreateReturn(true, "Review Done", "Progress saved, you got " + reviewReturn.Values.Where(x => x == 1).Count() + " correct", "success");
            return returnInfo;
        }
    }
}
