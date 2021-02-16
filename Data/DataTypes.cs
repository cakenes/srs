using System;
using System.Collections.Generic;

namespace Srs.Data {

    public struct User {

        public int Id;

        // Generic
        public string Name;
        public string Password;
        public string Email;

        // Decks
        public List<Deck> Own;
        public List<Deck> Favorite;
        public List<Deck> InProgress;

        // Statistics
        public DateTime Created;
    }

    public struct Card {

        // Generic
        public string Front;
        public string Back;

        // Allow small typos
        public bool AllowTypos;

        // Multi choice instead of typing
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

        public int Id;

        // Generic
        public string Name;
        public string Auhors;

        // Cards
        public Dictionary<int, Card> Cards;

        // Statistics
        public DateTime Created;
        public DateTime Modified;
    }

    public struct DeckList {

        public int Id;

        // Generic
        public string Name;
        public string Auhors;

        // Statistics
        public int Cards;
        public int Favorites;
    }

}
