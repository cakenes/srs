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
        public Dictionary<int, Dictionary<int, int>> Review;
    }
}