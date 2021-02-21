using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Srs.Data;

namespace Srs
{
    public partial class Access {

        public SortedDictionary<int, Data.User> UserDict;
        public Dictionary<Guid?,Data.User> GuidList;

        public void InitializeUser() {
            UserDict = new SortedDictionary<int, Data.User>();
            GuidList = new Dictionary<Guid?, Data.User>();
            LoadUsers();
        }

        // Load users to memory
        public void LoadUsers() {
            // Error
            if (!Directory.Exists("users/")) return;

            // Success
            UserDict = new SortedDictionary<int, Data.User>();
            string[] UserPaths = Directory.GetFiles("users/", "*.*");
            foreach (string item in UserPaths) {
                string jsonArray = File.ReadAllText(item);
                Data.User fromJson = JsonConvert.DeserializeObject<Data.User>(jsonArray);
                UserDict.Add(fromJson.Id, fromJson);
            }
        }

        // Login user
        public Guid? LoginUser(string name, string password) {
            // Error
            int index = FindUser(name);
            if (index == -1) return null; //Name doesn't exist
            else if (UserDict[index].Password != password) return null; //Incorrect password
            
            // Success
            Guid guid = Guid.NewGuid();
            GuidList.Add(guid, UserDict[index]);
            return guid;
        }

        // Update user file
        public bool UpdateUser(Guid guid) {
            // Error
            if (!GuidList.ContainsKey(guid)) return false;

            // Success
            string toJson = JsonConvert.SerializeObject(GuidList[guid]);
            File.WriteAllText("users/" + GuidList[guid].Name, toJson);
            return true;
        }

        // Create new user
        public bool CreateUser(string name, string password) {
            // Error
            int index = FindUser(name);
            if (index != -1) return false; //Name exists
            
            // Success
            Data.User user = new Data.User {Name = name, Password = password};
            // Empty list check
            if (UserDict.Count == 0) user.Id = 0;
            else user.Id = UserDict.Keys.Last() + 1;
            // Create User
            UserDict.Add(user.Id, user);
            string toJson = JsonConvert.SerializeObject(user);
            File.WriteAllText("users/" + user.Name, toJson);
            return true;
        }

        // Create user data
        private Data.User NewUser(string name, string password) {
            return new Data.User {Name = name, Password = password};
        }

        // Return name index, if not found return -1
        private int FindUser (string input) {
            foreach (var item in UserDict) {  if (item.Value.Name.ToLower() == input.ToLower()) return item.Key; }
            return -1;
        }
    }
}