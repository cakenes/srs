using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Srs {

    public partial class AccessData {
    }

    public partial class Access {

        public static readonly Access Current = new Access();

        private SortedDictionary<int, Data.DeckInfo> InfoList;
        private SortedDictionary<int, Data.DeckFull> DeckList;

        public void InitializeDeck() {
            DeckList = new SortedDictionary<int, Data.DeckFull>();
            InfoList = new SortedDictionary<int, Data.DeckInfo>();
            LoadDecks();
        }

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
        public Data.ReturnInfo CreateDeck(Data.DeckFull deck) {
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

        // Modify existing deck
        public Data.ReturnInfo ModifyDeck(Data.DeckFull deck, Guid? guid) {
            int index = FindDeck(deck.Name);
            if (index == -1) return CreateReturn(false, "Modify", "Deck not found", "warning"); // Deck not found
            else if (index != deck.Id) return CreateReturn(false, "Modify", "Deck ID doesnt match", "warning"); // Deck id doesnt match the list
            if (!GuidList.ContainsKey(guid)) return CreateReturn(false, "Modify", "User not found", "warning"); // User not in the list
            else if (GuidList[guid].Name != deck.Author) return CreateReturn(false, "Modify", "User not match the author", "warning"); // User does not match

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
    }
}