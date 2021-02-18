using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Srs.Data {

    public struct User {

        // Index
        public int Id;

        // Generic
        public string Name;
        public string Password;
        public string Email;

        // Decks
        public List<DeckStats> Decks;

        // Statistics
        public DateTime Created;
    }

    public struct Card {

        public int Id;

        // Generic
        public string Front;
        public string Back;

        // Allow small typos
        // not yet implemented, might move to user settings
        public bool AllowTypos;

        // Multi choice instead of typing, not yet implemented
        public bool Choice;
        public bool Random;
        public string FrontOne;
        public string FrontTwo;
        public string BackOne;
        public string BackTwo;

        // Statistics
        public int Tries;
        public int Correct;
    }

    public struct Deck {
        // Unique Id
        public int Id;

        // Generic
        public string Name;
        public string Password;
        public User Auhors;

        // Cards
        public List<Card> Cards;

        // Statistics
        public DateTime Created;
        public DateTime Modified;
        public int Favorites;
    }

    public struct DeckInfo {
        // Id used in Deck
        public int Id;

        // Generic
        public string Name;
        public User Auhors;

        // Statistics
        public int Cards;
        public int Favorites;
    }

    public struct DeckStats {
        // Id used in Deck
        public int Id;

        // 
        public bool Own;
        public bool Favorite;
        public int Tries;
        public int Correct;
        List<CardStats> Cards;
    }

    // Tracking card progress inside of deck progress
    public struct CardStats {
        public int Id;
        public int Level;
        public int Tries;
        public int Correct;
    }

}
