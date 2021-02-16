using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Srs.Data;

namespace Srs
{
    public class DataAccess
    {
        public static DataAccess Current = new DataAccess();
    
        public Dictionary<int, User> UserList;
        public Dictionary<int, Deck> DeckList;
        public  Dictionary<int, DeckInfo> DeckInfoList;

        // Load files into dictionarys
        public void LoadDeckList() {

            // Directory missing
            if (!Directory.Exists("Decks/")) return;
            if (!Directory.Exists("Users/")) return;

            // Reset lists
            DeckList = new Dictionary<int, Deck>();
            UserList = new Dictionary<int, User>();

            // Read files
            string[] DeckPaths = Directory.GetFiles("Decks/", "*.*");
            string[] UserPaths = Directory.GetFiles("Users/", "*.*");

            // Deserialize file into Deck
            for (int i = 0; i < DeckPaths.Length; i++)
            {
                string jsonArray = File.ReadAllText(DeckPaths[i]);
                Deck fromJson = JsonConvert.DeserializeObject<Deck>(jsonArray);
                DeckList.Add(fromJson.Id, fromJson);
            }

            // Deserialize file into User
            for (int i = 0; i < UserPaths.Length; i++)
            {
                string jsonArray = File.ReadAllText(UserPaths[i]);
                User fromJson = JsonConvert.DeserializeObject<User>(jsonArray);
                UserList.Add(fromJson.Id, fromJson);
            }
        }

        // Add new or modify Deck
        public void SaveDeck(Deck deck) {
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
        public void SaveUser(User user) {
            // Serialize User into file
            string toJson = JsonConvert.SerializeObject(user);
            File.WriteAllText("Users/" + user.Name, toJson);

            // If editing, remove old user first
            if (UserList.ContainsKey(user.Id)) UserList.Remove(user.Id);

            // Add User to dictionary
            UserList.Add(user.Id, user);
        }        

        // Create DeckInfo from Deck
        public DeckInfo CreateDeckInfo(Deck deck) {
            return new DeckInfo { 
                Id = deck.Id, Name = deck.Name, Auhors = deck.Auhors, Cards = deck.Cards.Count, Favorites = deck.Favorites
            };
        }
    }   
}