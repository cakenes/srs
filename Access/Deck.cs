using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Srs{

    public partial class Access {

        public static readonly Access Current = new Access();

        public SortedDictionary<int, Data.Deck> DeckDict;
        public List<Data.DeckInfo> InfoList;

        public void InitializeDeck() {
            DeckDict = new SortedDictionary<int, Data.Deck>();
            InfoList = new List<Data.DeckInfo>();
            LoadDecks();
        }

        // Load decks to memory
        public void LoadDecks() {
            // Error
            if (!Directory.Exists("decks/")) return;

            // Success
            DeckDict = new SortedDictionary<int, Data.Deck>();
            string[] DeckPaths = Directory.GetFiles("decks/", "*.*");
            foreach (string item in DeckPaths) {
                string jsonArray = File.ReadAllText(item);
                Data.Deck fromJson = JsonConvert.DeserializeObject<Data.Deck>(jsonArray);
                DeckDict.Add(fromJson.Id, fromJson);
                InfoList.Add(NewInfo(fromJson));
            }
        }

        // Create new deck
        public bool CreateDeck(Data.Deck deck) {
            // Error
            int index = FindDeck(deck.Name);
            if (index != -1) return false; // Name exists

            // Success, Empy list check
            if (DeckDict.Count == 0) deck.Id = 0;
            else deck.Id = DeckDict.Keys.Last() + 1;
            // Create Deck
            DeckDict.Add(deck.Id, deck);
            InfoList.Add(NewInfo(deck));
            string toJson = JsonConvert.SerializeObject(deck);
            File.WriteAllText("decks/" + deck.Name, toJson);
            return true;
        }

        // Create info data
        public Data.DeckInfo NewInfo(Data.Deck deck) {
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