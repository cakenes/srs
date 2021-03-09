using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Srs {

    public partial class Access {
        
        public static readonly Access Current = new Access();

        public Dictionary<string, Data.DeckInfo> InfoList;

        private Data.DeckInfo tempInfo;

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
          if (!ValidateOrigin(origin)) { origin = new ServiceData { User = new Data.User { Name = "User" }}; return CreateReturn(false, "Create Deck", "Could not validate user", "danger"); }
            origin.Create.Author = origin.User.Name;
            string toFile = JsonConvert.SerializeObject(origin.Create, Formatting.Indented);
            File.WriteAllText("Db/decks/" + origin.Create.Name, toFile);
            InfoList.Add(origin.Create.Name, CreateDeckInfo(origin.Create));
            return CreateReturn(true,"Create Deck", "Deck successfully created", "success");
        }

        // Modify existing deck
        public Data.ReturnInfo ModifyDeck(ServiceData origin) {
            if (!ValidateOrigin(origin)) { origin = new ServiceData { User = new Data.User { Name = "User" }}; return CreateReturn(false, "Create Deck", "Could not validate user", "danger"); } 
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
            tempInfo = new Data.DeckInfo {Id = deck.Id, Popularity = 0, Name = deck.Name, Author = deck.Author, Cards = deck.Cards.Count};
            if (deck.Password != "") tempInfo.Password = true;
            return tempInfo;
        }

/*

        // Load decks to memory
        public void LoadDecks() {
            // Error
            if (!Directory.Exists("decks/")) return;

            // Success, loop thrue files
            DeckList = new SortedDictionary<int, Data.DeckFull>();
            string[] DeckPaths = Directory.GetFiles("decks/", "*.*");
            foreach (string item in DeckPaths) {
                string jsonArray = File.ReadAllText(item);
                Data.DeckFull fromJson = JsonConvert.DeserializeObject<Data.DeckFull>(jsonArray);
                DeckList.Add(fromJson.Id, fromJson);
                InfoList.Add(fromJson.Id, NewInfo(fromJson));
            }
        }

        // Create new deck
        public Data.ReturnInfo CreateDeckNew(Data.DeckFull deck) {
            int index = FindDeck(deck.Name);
            if (index != -1) return CreateReturn(false, "Create Deck", "Name is already in use", "warning"); // Name exists

            // Success, set deck.Id
            if (DeckList.Count == 0) deck.Id = 1;
            else deck.Id = DeckList.Keys.Last() + 1;

            // Create Deck
            DeckList.Add(deck.Id, deck);
            InfoList.Add(deck.Id, NewInfo(deck));
            string toJson = JsonConvert.SerializeObject(deck, Formatting.Indented);
            File.WriteAllText("decks/" + deck.Id + "-" + deck.Name, toJson);
            return CreateReturn(true,"Create Deck", "Deck successfully created", "success");
        }

        // Create new deck
        public Data.ReturnInfo CreateDeck(Data.DeckFull deck) {
            if (DeckExists(deck.Name)) return CreateReturn(false, "Create Deck", "Name is already in use", "warning"); // Name exists

            // Success, set deck.Id
            if (DeckList.Count == 0) deck.Id = 1;
            else deck.Id = DeckList.Keys.Last() + 1;

            // Create Deck
            DeckList.Add(deck.Id, deck);
            InfoList.Add(deck.Id, NewInfo(deck));
            string toJson = JsonConvert.SerializeObject(deck, Formatting.Indented);
            File.WriteAllText("decks/" + deck.Id + "-" + deck.Name, toJson);
            return CreateReturn(true,"Create Deck", "Deck successfully created", "success");
        }

        // Modify existing deck
        public Data.ReturnInfo ModifyDeckd(Data.DeckFull deck, Guid? guid) {
            if (!DeckList.ContainsKey(deck.Id)) return CreateReturn(false, "Modify", "Deck not found", "warning"); // Deck not found
            else if (!GuidList.ContainsKey(guid)) return CreateReturn(false, "Modify", "User not found", "warning"); // User not in the list
            else if (GuidList[guid].Name != deck.Author) return CreateReturn(false, "Modify", "User not match the author", "warning"); // User does not match

            // Remove old file
            File.Delete("decks/" + deck.Id + "-" + DeckList[deck.Id]);

            // Success
            InfoList[deck.Id] = NewInfo(deck);
            DeckList[deck.Id] = deck;
            string toJson = JsonConvert.SerializeObject(deck, Formatting.Indented);
            File.WriteAllText("decks/" + deck.Id + "-" + deck.Name, toJson);
            return CreateReturn(true, "Modify", "Deck successfully modified", "success");
        }

        public Data.DeckFull GetDeckList(int index) {
            if (!DeckList.ContainsKey(index)) return new Data.DeckFull();
            return DeckList[index];
        }

        public List<Data.DeckInfo> GetInfoList() {
            if (InfoList.Count == 0) return new List<Data.DeckInfo>();
            return InfoList.Values.ToList();
        }

        public List<Data.DeckInfo> GetModifyList(Guid? guid) {
            if (!GuidList.ContainsKey(guid)) return new List<Data.DeckInfo>();
            List<Data.DeckInfo> returnList = InfoList.Values.Where(x => x.Author == GuidList[guid].Name).ToList();
            return returnList;
        }  

        // Create info data
        public Data.DeckInfo NewInfo(Data.DeckFull deck) {
            Data.DeckInfo info = new Data.DeckInfo {Id = deck.Id, Popularity = 0, Name = deck.Name, Author = deck.Author, Cards = deck.Cards.Count};
            if (deck.Password != null) info.Password = true;
            return info;
        }

        // Return name index, if not found return -1
        private int FindDeck (string input) {
            foreach (var item in DeckList) {  if (item.Value.Name.ToLower() == input.ToLower()) return item.Key; }
            return -1;
        }

*/

    }
}