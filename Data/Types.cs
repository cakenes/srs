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

    public struct UserDeck {
        public int Id;
        public List<Card> Known;
    }

    public struct Deck {
        public int Id;
        public string Name;
        public string Password;
        public string Author;
        public SortedDictionary<int, Card> Cards;
        // Statistics
        public int Favorites;

        public void AddFavorite() { Favorites += 1; }
        public void RemoveFavorite() { Favorites -= 1; }
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
        // Deck related
        public List<int> Own;
        public List<int> Opened;
        public List<int> Favorites;
        // Card related (int = deckId)
        public SortedDictionary<int, Card> BoxOne;
        public SortedDictionary<int, Card> BoxTwo;
        public SortedDictionary<int, Card> BoxThree;
        public SortedDictionary<int, Card> BoxFour;
        public SortedDictionary<int, Card> BoxFive;
    }
}