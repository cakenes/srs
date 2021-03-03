using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;

namespace Srs {

    public partial class Access {

        private UserCache userCache = new UserCache();
        private Dictionary<string, Guid?> guidList = new Dictionary<string, Guid?>();

        // Validate user
        public bool ValidateOrigin (ServiceData origin) {
            if (!guidList.ContainsKey(origin.User.Name)) return false;
            if (guidList[origin.User.Name] != origin.UserId) return false;
            return true;
        }

        // Login user
        public async Task<Data.ReturnInfo> LoginUser(ServiceData origin, string name, string password) {
            Data.User userLogin = await UserCache.Current.LoadUser(name);
            if (userLogin.Name == null) return CreateReturn(false, "Login", "User does not exist", "warning");
            else if (userLogin.Password != password) return CreateReturn(false, "Login", "Incorrect password", "warning");
            else { Guid guid = Guid.NewGuid(); origin.User = userLogin; origin.UserId = guid; guidList.Add(userLogin.Name, guid); }
            return CreateReturn(true, "Login", "Login successfull", "success");
        }

        // Create new user
        public async Task<Data.ReturnInfo> CreateUserAsync (string name, string password) {
            Data.User newUser = new Data.User {Name = name, Password = password, Review = new Dictionary<int, Dictionary<int, int>>()};
            string toFile = JsonConvert.SerializeObject(newUser, Formatting.Indented);
            await File.WriteAllTextAsync("Db/users/" + name, toFile);
            return CreateReturn(true, "Register", "User created successfully", "success");
        }

        // Change user password
        public async Task<Data.ReturnInfo> ChangePasswordAsync (ServiceData origin, string password) {
          if (!ValidateOrigin(origin)) { origin = new ServiceData { User = new Data.User { Name = "User" }}; return CreateReturn(false, "Create Deck", "Could not validate user", "danger"); }
            else if (origin.User.Password == password) return CreateReturn(false, "Change Password", "Cannot change into current password", "warning");
            origin.User.Password = password;
            string toFile = JsonConvert.SerializeObject(origin.User, Formatting.Indented);
            await File.WriteAllTextAsync("Db/users/" + origin.User.Name, toFile);
            UserCache.Current.RemoveUser(origin.User.Name);
            origin.User = await UserCache.Current.LoadUser(origin.User.Name);
            return CreateReturn(true, "Change Password", "Deck successfully modified", "success");
        }

        // Return reviewed deck
        public async Task<Data.ReturnInfo> ReturnReviewDeck (ServiceData origin) {
            if (!ValidateOrigin(origin)) { origin = new ServiceData { User = new Data.User { Name = "User" }}; return CreateReturn(false, "Review Deck", "Could not validate user", "danger"); }
            string toFile = JsonConvert.SerializeObject(origin.User, Formatting.Indented);
            await File.WriteAllTextAsync("Db/users/" + origin.User.Name, toFile);
            UserCache.Current.RemoveUser(origin.User.Name);
            origin.User = await UserCache.Current.LoadUser(origin.User.Name);
            return CreateReturn(true, "Review Deck", "Success", "success");
       }

        // Create Return
        public Data.ReturnInfo CreateReturn(bool success, string title = "", string message = "", string type = "") {
            return new Data.ReturnInfo {Success = success, Title = title, Message = message, Type = type};
        }

/*

//////////////////////////////////////


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
            if (index == -1) return CreateReturn(false, "Login", "Username does not exist", "danger"); //Name doesn't exist
            else if (UserList[index].Password != password) return CreateReturn(false, "Login", "Incorrect password", "danger"); //Incorrect password
            
            // Success
            Guid guid = Guid.NewGuid();
            GuidList.Add(guid, UserList[index]);
            return CreateReturn(true, "Login", guid.ToString(), "success");
        }

        // Update user file
        public bool UpdateUser(Guid? guid) {
            if (!GuidList.ContainsKey(guid)) return false;

            // Success
            string toJson = JsonConvert.SerializeObject(GuidList[guid], Formatting.Indented);
            File.WriteAllText("users/" + GuidList[guid].Id + "-" + GuidList[guid].Name, toJson);
            return true;
        }

        public async Task<Data.ReturnInfo> LoginUserAsync (string name, string password) {
            if (!userCache.UserExists(name)) return CreateReturn(false, "Login", "Username does not exist", "danger");

            Data.User loginUser = await userCache.LoadUser(name);
            if (loginUser.Password != password) return CreateReturn(false, "Login", "Incorrect password", "danger");

            Guid guid = Guid.NewGuid();
            GuidList.Add(guid, loginUser);
            return CreateReturn(true, "Login", guid.ToString(), "success");
        }

        public async Task<Data.ReturnInfo> CreateUserAsync (string name, string password) {
            if (userCache.UserExists(name)) return CreateReturn(false, "Register", "Username is taken", "warning");

            Data.User newUser = new Data.User {Name = name, Password = password, Review = new Dictionary<int, Dictionary<int, int>>()};

            string toFile = JsonConvert.SerializeObject(newUser, Formatting.Indented);
            await File.WriteAllTextAsync("users/" + newUser.Name, toFile);

            return CreateReturn(true, "Register", "User created successfully", "success");
        }


        // Create new user
        public Data.ReturnInfo CreateUser(string name, string password) {
            if (FindUser(name) != -1) return CreateReturn(false, "Register", "Username is taken", "warning");
            
            // Initialize user data
            Data.User user = new Data.User {Name = name, Password = password, Review = new Dictionary<int, Dictionary<int, int>>()};

            // Success, set deck.Id
            if (UserList.Count == 0) user.Id = 1;
            else user.Id = UserList.Keys.Last() + 1;
            
            // Create User
            UserList.Add(user.Id, user);
            string toJson = JsonConvert.SerializeObject(user, Formatting.Indented);
            File.WriteAllText("users/" + user.Name, toJson);
            return CreateReturn(true, "Register", "User created successfully", "success");
        }

        public Data.ReturnInfo ChangePassword(Guid? guid, string current, string password) {
            if (!GuidList.ContainsKey(guid)) return CreateReturn(false, "Change Password", "User not found", "danger");
            else if (GuidList[guid].Password != current) return CreateReturn(false, "Change Password", "Current password incorrect", "warning");
            else { Data.User user = GuidList[guid]; user.Password = password; GuidList[guid] = user; UpdateUser(guid); }
            return CreateReturn(true, "Change Password", "Password changed successfully", "success");
        }

        public Data.ReturnInfo CreateReturn(bool success, string title, string message, string type) {
            return new Data.ReturnInfo {Success = success, Title = title, Message = message, Type = type};
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
        
*/

    }
}