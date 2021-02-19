using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Srs.Access
{
    public class Deck
    {
        public static readonly Deck Current = new Deck();
        public List<Data.Deck> DeckList;
        public List<Data.DeckInfo> InfoList;

        public void Initialize() {
            DeckList = new List<Data.Deck>();
            InfoList = new List<Data.DeckInfo>();
            Load();
        }

        public void Load () {
            if (!Directory.Exists("decks/")) return;
            DeckList = new List<Data.Deck>();
            string[] DeckPaths = Directory.GetFiles("decks/", "*.*");
            foreach (string item in DeckPaths)
            {
                string jsonArray = File.ReadAllText(item);
                Data.Deck fromJson = JsonConvert.DeserializeObject<Data.Deck>(jsonArray);
                Data.DeckInfo info = Info(fromJson);
                DeckList.Add(fromJson);
                InfoList.Add(info);
            }
        }

        public bool Add(Data.Deck deck) {
            int index = DeckList.FindIndex(x => x.Name == deck.Name);
            if (index == -1) { Save(deck); return true; }
            return false;
        }

        public void Save(Data.Deck deck) {
            if (deck.Id == -1) {
                deck.Id = DeckList.Count; 
                DeckList.Insert(deck.Id, deck);
                Data.DeckInfo info = Info(deck);
                InfoList.Insert(info.Id, info); }
            else { DeckList[deck.Id] = deck; }
            string toJson = JsonConvert.SerializeObject(deck);
            File.WriteAllText("decks/" + deck.Id, toJson);
        }

        public Data.DeckInfo Info(Data.Deck deck) {
            Data.DeckInfo info = new Data.DeckInfo {Id = deck.Id, Popularity = 666, Name = deck.Name, Author = deck.Name, Cards = deck.Cards.Count};
            if (deck.Password != null) info.Password = true;
            return info;
        }
    }   
}