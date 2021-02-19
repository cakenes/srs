using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Srs.Access
{
    public class Deck
    {
        
        public static readonly Deck Current = new Deck();
        public Dictionary<int, Data.User> UserList;
        public Dictionary<int, Data.Deck> DeckList;
        public  Dictionary<int, Data.DeckList> DeckInfoList;

        // Run on startup
        public void Initialize() {

        }


        // Load files into dictionarys
        public void LoadDeckList() {

            // Directory missing
            if (!Directory.Exists("decks/")) return;

            // Reset lists
            DeckList = new Dictionary<int, Data.Deck>();

            // Read files
            string[] DeckPaths = Directory.GetFiles("decks/", "*.*");

            // Deserialize file into Deck
            for (int i = 0; i < DeckPaths.Length; i++)
            {
                string jsonArray = File.ReadAllText(DeckPaths[i]);
                Data.Deck fromJson = JsonConvert.DeserializeObject<Data.Deck>(jsonArray);
                DeckList.Add(fromJson.Id, fromJson);
            }
        }

        // Add new or modify Deck
        public void SaveDeck(Data.Deck deck) {
            // Serialize Deck into file
            string toJson = JsonConvert.SerializeObject(deck);
            File.WriteAllText("decks/" + deck.Name, toJson);

            // If editing, remove old entries first
            if (DeckList.ContainsKey(deck.Id)) DeckList.Remove(deck.Id);
            if (DeckInfoList.ContainsKey(deck.Id)) DeckInfoList.Remove(deck.Id);

            // Add Deck and DeckInfo to dictionary
            DeckList.Add(deck.Id, deck);
        }    
    }   
}