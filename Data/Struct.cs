using System;
using System.Collections.Generic;

namespace Srs.Data {

    public struct Info {
        public bool Success;
        public string Message;
    }

    public struct Connected {
        public string Name;
        public string Guid;
    }

    public struct Deck {
        public int Id;
        public string Name;
        public string Password;
        public string Owner;
        public List<Card> Cards;
    }

    public struct DeckInfo {
        public int Id;
        public int Popularity;
        public string Name;
        public string Author;
        public int Cards;
    }

    public struct Card {
        public int Id;
        public string Front;
        public string Back;
    }
        
    public struct User {
        public int Id;
        public string Name;
        public string Password;
        public string Email;
    }
}