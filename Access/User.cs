using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Srs.Access
{
    public class User
    {
        public static readonly User Current = new User();

        public SortedDictionary<int, Data.User> UserDict;
        public Dictionary<Guid?,Data.User> GuidList;

        public void Initialize() {
            UserDict = new SortedDictionary<int, Data.User>();
            GuidList = new Dictionary<Guid?, Data.User>();
            Load();
        }

        // Load users to memory
        public void Load() {
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
        public Guid? Login(string name, string password) {
            // Error
            int index = FindName(name);
            if (index == -1) return null; //Name doesn't exist
            else if (UserDict[index].Password != password) return null; //Incorrect password
            
            // Success
            Guid guid = Guid.NewGuid();
            GuidList.Add(guid, UserDict[index]);
            return guid;
        }

        // Update user file
        public bool Update(Guid guid) {
            // Error
            if (!GuidList.ContainsKey(guid)) return false;
            
            // Success
            string toJson = JsonConvert.SerializeObject(GuidList[guid]);
            File.WriteAllText("users/" + GuidList[guid].Name, toJson);
            return true;
        }

        // Create new user
        public bool Create(string name, string password) {
            // Error
            int index = FindName(name);
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
        private int FindName (string input) {
            foreach (var item in UserDict) {  if (item.Value.Name.ToLower() == input.ToLower()) return item.Key; }
            return -1;
        }

        // Add or remove own
        public bool Own (Guid guid, int id) {
            if (!GuidList.ContainsKey(guid)) return false;
            if (GuidList[guid].Own.Contains(id)) GuidList[guid].Own.Remove(id);
            else GuidList[guid].Own.Remove(id);
            Update(guid);
            return true;
        }

        // Add or remove favorite
        public bool Favorite (Guid guid, int id) {
            if (!GuidList.ContainsKey(guid)) return false;
            if (GuidList[guid].Favorites.Contains(id)) GuidList[guid].Favorites.Remove(id);
            else GuidList[guid].Favorites.Remove(id);
            Update(guid);
            return true;
        }

        // Add or remove own
        public bool Opened (Guid guid, int id) {
            if (!GuidList.ContainsKey(guid)) return false;
            if (GuidList[guid].Opened.Contains(id)) GuidList[guid].Opened.Remove(id);
            else GuidList[guid].Opened.Remove(id);
            Update(guid);
            return true;
        }


    }
}