using System;
using System.Collections.Generic;

namespace Srs.Data {

    public class DeckFull {
        public int Id = 0;
        public string Name = "";
        public string Old = "";
        public string Password = "";
        public string Author = "";
        public SortedDictionary<int, Card> Cards = new SortedDictionary<int, Card>();
        public int Favorites = 0;
        public void AddFavorite() { Favorites += 1; }
        public void RemoveFavorite() { Favorites -= 1; }
    }

    public class PartialDeck {
        public int Id = 0;
        public string Name = "";
        public SortedDictionary<int, Card> Cards = new SortedDictionary<int, Card>();
    }

    public class Card {
        public int Id = 0;
        public string Front = "";
        public string Back = "";
    }

    public class DeckInfo {
        public int Id = 0;
        public string Name = "";
        public bool Password = false;
        public string Author = "";
        public int Popularity = 0;
        public int Cards = 0;
    }
}