using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Srs {

    public partial class Access {
        
        public static readonly Access Current = new Access();

        public Dictionary<string, Data.DeckInfo> InfoList = new Dictionary<string, Data.DeckInfo>();

        // Populate InfoList
        public async void InitializeInfo() {
            if (!Directory.Exists("Db/decks/")) return;
            InfoList = new Dictionary<string, Data.DeckInfo>();
            string[] filePaths = Directory.GetFiles("Db/decks/", "*.*");

            foreach (string path in filePaths)
            {
                string fromFile = await File.ReadAllTextAsync(path);
                Data.DeckFull fromText = JsonConvert.DeserializeObject<Data.DeckFull>(fromFile);
                InfoList.Add(fromText.Name, CreateDeckInfo(fromText));
            }
        }

        // Create new deck
        public Data.ReturnInfo CreateDeckAsync(ServiceData origin) {
            if (!ValidateOrigin(origin)) return CreateReturn(false, "Logout", "Could not validate user", "danger");
            origin.Create.Author = origin.User.Name;
            string toFile = JsonConvert.SerializeObject(origin.Create, Formatting.Indented);
            File.WriteAllText("Db/decks/" + origin.Create.Name, toFile);
            InfoList.Add(origin.Create.Name, CreateDeckInfo(origin.Create));
            return CreateReturn(true,"Create Deck", "Deck successfully created", "success");
        }

        // Modify existing deck
        public Data.ReturnInfo ModifyDeck(ServiceData origin) {
            if (!ValidateOrigin(origin)) return CreateReturn(false, "Logout", "Could not validate user", "danger");
            string toFile = JsonConvert.SerializeObject(origin.Create, Formatting.Indented);
            File.WriteAllText("Db/decks/" + origin.Create.Name, toFile);
            InfoList.Add(origin.Create.Name, CreateDeckInfo(origin.Create));
            // Remove Old
            InfoList.Remove(origin.Create.Old);
            DeckCache.Current.RemoveDeck(origin.Create.Old);
            File.Delete("Db/decks/" + origin.Create.Old);
            return CreateReturn(true, "Modify", "Deck successfully modified", "success");
        }

        // Generate modifyable list
        public List<Data.DeckInfo> CreateModifyList(ServiceData origin) {
            List<Data.DeckInfo> returnList = InfoList.Values.Where(x => x.Author == origin.User.Name).ToList();
            return returnList;
        }

        // Generate DeckInfo
        public Data.DeckInfo CreateDeckInfo(Data.DeckFull deck) {
            Data.DeckInfo deckInfo = new Data.DeckInfo {Id = deck.Id, Popularity = 0, Name = deck.Name, Author = deck.Author, Cards = deck.Cards.Count};
            if (deck.Password != "") deckInfo.Password = true;
            return deckInfo;
        }
    }
}