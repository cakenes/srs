using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Srs.Data;

public class DataAccess
{
    public static Dictionary<int, User> UserList;
    public static Dictionary<int, Deck> DeckList;
    public static Dictionary<int, DeckList> ClientDeckList;

    public static void LoadDeckList() {
        // Directory missing
        if (!Directory.Exists("Decks/")) return;
        if (!Directory.Exists("Users/"))

        // Reset lists
        DeckList = new Dictionary<int, Deck>();
        UserList = new Dictionary<int, User>();

        string[] DeckPaths = Directory.GetFiles("Decks/", "*.*");
        string[] UserPaths = Directory.GetFiles("Users/", "*.*");

        for (int i = 0; i < DeckPaths.Length; i++)
        {
            string jsonArray = File.ReadAllText(DeckPaths[i]);
            Deck fromJson = JsonConvert.DeserializeObject<Deck>(jsonArray);
            DeckList.Add(fromJson.Id, fromJson);
        }

        for (int i = 0; i < UserPaths.Length; i++)
        {
            string jsonArray = File.ReadAllText(UserPaths[i]);
            User fromJson = JsonConvert.DeserializeObject<User>(jsonArray);
            UserList.Add(fromJson.Id, fromJson);
        }

    }

    public static void SaveDeck(Deck deck) {
        // Save to file
        string toJson = JsonConvert.SerializeObject(deck);
        File.WriteAllText("Decks/" + deck.Name, toJson);
        
        // Editing
        if (DeckList.ContainsKey(deck.Id)) DeckList.Remove(deck.Id);
        if (ClientDeckList.ContainsKey(deck.Id)) ClientDeckList.Remove(deck.Id);

        // Add
        DeckList.Add(deck.Id, deck);
    }
}