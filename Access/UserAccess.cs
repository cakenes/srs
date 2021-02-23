using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

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
        public bool UpdateUser(Guid? guid) {
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
            if (name == null || password == null) return false;
            if (name.Length < 3) return false; //Name too short
            if (password.Length < 3) return false; //Password too short
            if (FindUser(name) != -1) return false; //Name exists
            
            // Initialize user data
            Data.User user = new Data.User {Name = name, Password = password, Review = new Dictionary<int, Dictionary<int, int>>()};

            // Success, set deck.Id
            if (UserDict.Count == 0) user.Id = 1;
            else user.Id = UserDict.Keys.Last() + 1;
            
            // Create User
            UserDict.Add(user.Id, user);
            string toJson = JsonConvert.SerializeObject(user);
            File.WriteAllText("users/" + user.Name, toJson);
            return true;
        }

        public Data.User GetUser (Guid? guid) {
            if (guid == null) return new Data.User { Review = new Dictionary<int, Dictionary<int, int>>()};
            if (!GuidList.ContainsKey(guid)) return new Data.User { Review = new Dictionary<int, Dictionary<int, int>>()};
            return GuidList[guid];
        }

        // Return name index, if not found return -1
        private int FindUser (string input) {
            foreach (var item in UserDict) {  if (item.Value.Name.ToLower() == input.ToLower()) return item.Key; }
            return -1;
        }

        // Add or remove own tag
        public bool SetOwn (Guid guid, int id) {
            // Error
            if (!GuidList.ContainsKey(guid)) return false;
            // Success
            if (GuidList[guid].Own.Contains(id)) GuidList[guid].Own.Remove(id);
            else GuidList[guid].Own.Add(id);
            UpdateUser(guid);
            return true;
        }

        // Add or remove favorite tag
        public bool SetFavorite (Guid guid, int id) {
            // Error
            if (!GuidList.ContainsKey(guid)) return false;
            // Success
            if (GuidList[guid].Favorites.Contains(id)) { GuidList[guid].Favorites.Remove(id); DeckDict[id].RemoveFavorite(); }
            else { GuidList[guid].Favorites.Add(id); DeckDict[id].AddFavorite(); }  
            UpdateUser(guid);
            return true;
        }

        // Add or remove opened tag
        public bool SetOpened (Guid guid, int id) {
            // Error
            if (!GuidList.ContainsKey(guid)) return false;
            // Success
            if (GuidList[guid].Opened.Contains(id)) GuidList[guid].Opened.Remove(id);
            else GuidList[guid].Opened.Add(id);
            UpdateUser(guid);
            return true;
        }
    }
}