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

        // List index
        public int Id;

        // Generic
        public string Name;
        public string Password;
        public string Owner;

        // Cards
        public List<Card> Cards;
    }

    public struct DeckList {

        // List index
        public int Id;

        // Generic
        public string Name;
    }

    public struct Card {

        // List index
        public int Id;

        // Generic
        public string Front;
        public string Back;
    }
        
    public struct User {

        // List index
        public int Id;

        // Generic
        public string Name;
        public string Password;
        public string Email;
    }
}