using System;
using System.Collections.Generic;

namespace Srs.Data {

    public class ReturnInfo {
        public bool Success = false;
        public string Title = "";
        public string Message = "";
        public string Type = "";
    }

    public class User {
        public Guid? Id = null;
        public string Name = "";
        public string Password = "";
        public Dictionary<int, Dictionary<int, int>> Review = new Dictionary<int, Dictionary<int, int>>();
    }
}