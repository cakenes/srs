using System;
using System.Collections.Generic;

namespace Srs.Data {

    public struct Info {
        public bool Success;
        public string Message;
    }

    public struct Connected {
        public User User;
        public string Guid;
    }

    public struct Deck {
        public int Id;
        public string Name;
        public string Password;
        public string Author;
        public List<Card> Cards;
    }

    public struct Card {
        public int Id;
        public string Front;
        public string Back;
    }

    public struct DeckInfo {
        public int Id;
        public string Name;
        public bool Password;
        public string Author;
        public int Popularity;
        public int Cards;
    }
        
    public struct User {
        public int Id;
        public string Name;
        public string Password;
    }
}