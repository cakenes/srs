using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Srs{

    public partial class Access {

        public static readonly Access Current = new Access();

        public List<Data.DeckInfo> InfoList;
        public SortedDictionary<int, Data.DeckFull> DeckDict;
        public SortedDictionary<int, Data.PartialDeck> PartialDictionary;

        public void InitializeDeck() {
            InfoList = new List<Data.DeckInfo>();
            DeckDict = new SortedDictionary<int, Data.DeckFull>();
            LoadDecks();
        }

        // Load decks to memory
        public void LoadDecks() {
            // Error
            if (!Directory.Exists("decks/")) return;

            // Success, loop thrue files
            DeckDict = new SortedDictionary<int, Data.DeckFull>();
            string[] DeckPaths = Directory.GetFiles("decks/", "*.*");
            foreach (string item in DeckPaths) {
                string jsonArray = File.ReadAllText(item);
                Data.DeckFull fromJson = JsonConvert.DeserializeObject<Data.DeckFull>(jsonArray);
                DeckDict.Add(fromJson.Id, fromJson);
                InfoList.Add(NewInfo(fromJson));
            }
        }

        // Create new deck
        public bool CreateDeck(Data.DeckFull deck) {
            // Error
            int index = FindDeck(deck.Name);
            if (index != -1) return false; // Name exists

            // Success, set deck.Id
            if (DeckDict.Count == 0) deck.Id = 1;
            else deck.Id = DeckDict.Keys.Last() + 1;

            // Create Deck
            DeckDict.Add(deck.Id, deck);
            InfoList.Add(NewInfo(deck));
            string toJson = JsonConvert.SerializeObject(deck);
            File.WriteAllText("decks/" + deck.Name, toJson);
            return true;
        }

        // Create info data
        public Data.DeckInfo NewInfo(Data.DeckFull deck) {
            Data.DeckInfo info = new Data.DeckInfo {Id = deck.Id, Popularity = 666, Name = deck.Name, Author = deck.Author, Cards = deck.Cards.Count};
            if (deck.Password != null) info.Password = true;
            return info;
        }

        // Return name index, if not found return -1
        private int FindDeck (string input) {
            foreach (var item in DeckDict) {  if (item.Value.Name.ToLower() == input.ToLower()) return item.Key; }
            return -1;
        }

    }
}