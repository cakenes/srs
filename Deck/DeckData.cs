using System;
using System.Collections.Generic;

namespace Srs.Data {

    public struct DeckFull {
        public int Id;
        public string Name;
        public string Password;
        public string Author;
        public SortedDictionary<int, Card> Cards;
        public int Favorites;
        public void AddFavorite() { Favorites += 1; }
        public void RemoveFavorite() { Favorites -= 1; }
    }

    public struct PartialDeck {
        public int Id;
        public string Name;
        public SortedDictionary<int, Card> Cards;
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
}