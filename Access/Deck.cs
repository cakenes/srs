using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Srs.Access
{
    public class Deck
    {
        public static Deck Current = new Deck();
    
        public Dictionary<int, Data.User> UserList;
        public Dictionary<int, Data.Deck> DeckList;
        public  Dictionary<int, Data.DeckInfo> DeckInfoList;

        // Load files into dictionarys
        public void LoadDeckList() {

            // Directory missing
            if (!Directory.Exists("Decks/")) return;
            if (!Directory.Exists("Users/")) return;

            // Reset lists
            DeckList = new Dictionary<int, Data.Deck>();
            UserList = new Dictionary<int, Data.User>();

            // Read files
            string[] DeckPaths = Directory.GetFiles("Decks/", "*.*");
            string[] UserPaths = Directory.GetFiles("Users/", "*.*");

            // Deserialize file into Deck
            for (int i = 0; i < DeckPaths.Length; i++)
            {
                string jsonArray = File.ReadAllText(DeckPaths[i]);
                Data.Deck fromJson = JsonConvert.DeserializeObject<Data.Deck>(jsonArray);
                DeckList.Add(fromJson.Id, fromJson);
            }

            // Deserialize file into User
            for (int i = 0; i < UserPaths.Length; i++)
            {
                string jsonArray = File.ReadAllText(UserPaths[i]);
                Data.User fromJson = JsonConvert.DeserializeObject<Data.User>(jsonArray);
                UserList.Add(fromJson.Id, fromJson);
            }
        }

        // Add new or modify Deck
        public void SaveDeck(Data.Deck deck) {
            // Serialize Deck into file
            string toJson = JsonConvert.SerializeObject(deck);
            File.WriteAllText("Decks/" + deck.Name, toJson);

            // If editing, remove old entries first
            if (DeckList.ContainsKey(deck.Id)) DeckList.Remove(deck.Id);
            if (DeckInfoList.ContainsKey(deck.Id)) DeckInfoList.Remove(deck.Id);

            // Add Deck and DeckInfo to dictionary
            DeckList.Add(deck.Id, deck);
            DeckInfoList.Add(deck.Id, CreateDeckInfo(deck));
        }

        // Add new or modify User
        public void SaveUser(Data.User user) {
            // Serialize User into file
            string toJson = JsonConvert.SerializeObject(user);
            File.WriteAllText("Users/" + user.Name, toJson);

            // If editing, remove old user first
            if (UserList.ContainsKey(user.Id)) UserList.Remove(user.Id);

            // Add User to dictionary
            UserList.Add(user.Id, user);
        }        

        // Create DeckInfo from Deck
        public Data.DeckInfo CreateDeckInfo(Data.Deck deck) {
            return new Data.DeckInfo { 
                Id = deck.Id, Name = deck.Name, Auhors = deck.Auhors, Cards = deck.Cards.Count, Favorites = deck.Favorites
            };
        }
    }   
}