using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Srs
{
    public partial class Access {

        private SortedDictionary<int, Data.User> UserList;
        private Dictionary<Guid?,Data.User> GuidList;

        public void InitializeUser() {
            UserList = new SortedDictionary<int, Data.User>();
            GuidList = new Dictionary<Guid?, Data.User>();
            LoadUsers();
        }

        // Load users to memory
        public void LoadUsers() {
            // Error
            if (!Directory.Exists("users/")) return;

            // Success
            UserList = new SortedDictionary<int, Data.User>();
            string[] UserPaths = Directory.GetFiles("users/", "*.*");
            foreach (string item in UserPaths) {
                string jsonArray = File.ReadAllText(item);
                Data.User fromJson = JsonConvert.DeserializeObject<Data.User>(jsonArray);
                UserList.Add(fromJson.Id, fromJson);
            }
        }

        // Login user
        public Data.ReturnInfo LoginUser(string name, string password) {
            // Error
            int index = FindUser(name);
            if (index == -1) return CreateReturn(false, "Username does not exist", "danger"); //Name doesn't exist
            else if (UserList[index].Password != password) return CreateReturn(false, "Incorrect password", "danger"); //Incorrect password
            
            // Success
            Guid guid = Guid.NewGuid();
            GuidList.Add(guid, UserList[index]);
            return CreateReturn(true, guid.ToString(), "success");
        }

        // Update user file
        public bool UpdateUser(Guid? guid) {
            if (!GuidList.ContainsKey(guid)) return false;

            // Success
            string toJson = JsonConvert.SerializeObject(GuidList[guid]);
            File.WriteAllText("users/" + GuidList[guid].Name, toJson);
            return true;
        }

        // Create new user
        public Data.ReturnInfo CreateUser(string name, string password) {
            if (FindUser(name) != -1) return CreateReturn(false, "Username is taken", "warning");
            
            // Initialize user data
            Data.User user = new Data.User {Name = name, Password = password, Review = new Dictionary<int, Dictionary<int, int>>()};

            // Success, set deck.Id
            if (UserList.Count == 0) user.Id = 1;
            else user.Id = UserList.Keys.Last() + 1;
            
            // Create User
            UserList.Add(user.Id, user);
            string toJson = JsonConvert.SerializeObject(user);
            File.WriteAllText("users/" + user.Name, toJson);
            return CreateReturn(true, "User created successfully", "success");
        }

        public Data.ReturnInfo ChangePassword(Guid? guid, string current, string password) {
            if (!GuidList.ContainsKey(guid)) return CreateReturn(false, "User not found", "danger");
            else if (GuidList[guid].Password != current) return CreateReturn(false, "Current password incorrect", "warning");
            else { Data.User user = GuidList[guid]; user.Password = password; GuidList[guid] = user; }
            return CreateReturn(true, "Password changed successfully", "success");
        }

        public Data.ReturnInfo CreateReturn(bool success, string message, string type) {
            return new Data.ReturnInfo {Success = success, Message = message, Type = type};
        }

        public Data.User GetUser (Guid? guid) {
            if (guid == null) return new Data.User { Review = new Dictionary<int, Dictionary<int, int>>()};
            if (!GuidList.ContainsKey(guid)) return new Data.User { Review = new Dictionary<int, Dictionary<int, int>>()};
            return GuidList[guid];
        }

        public string GetUserName (Guid? guid) {
            if (guid == null) return "User";
            if (!GuidList.ContainsKey(guid)) return "User";
            return GuidList[guid].Name;
        }

        // Return name index, if not found return -1
        private int FindUser (string input) {
            foreach (var item in UserList) {  if (item.Value.Name.ToLower() == input.ToLower()) return item.Key; }
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
            if (GuidList[guid].Favorites.Contains(id)) { GuidList[guid].Favorites.Remove(id); DeckList[id].RemoveFavorite(); }
            else { GuidList[guid].Favorites.Add(id); DeckList[id].AddFavorite(); }  
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