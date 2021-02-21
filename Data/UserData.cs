using System;
using System.Collections.Generic;

namespace Srs.Data {

    public struct ReturnInfo {
        public bool Success;
        public string Message;
    }

    public struct Connected {
        public User User;
        public string Guid;
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
        public SortedDictionary<int, DoubleInt> Old;
    }
}