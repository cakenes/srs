using System;
using System.Collections.Generic;

namespace Srs.Data {

    public struct ReturnInfo {
        public bool Success;
        public string Title;
        public string Message;
        public string Type;
    }

    public struct User {
        public int Id;
        public string Name;
        public string Password;
        // Deck related
        public List<int> Own;
        public List<int> Opened;
        public List<int> Favorites;
        // Old reviews
        public Dictionary<int, Dictionary<int, int>> Review;
    }
}