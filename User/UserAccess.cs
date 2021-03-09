using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Srs {

    public partial class Access {

        private UserCache userCache = new UserCache();
        private Dictionary<string, Guid?> guidList = new Dictionary<string, Guid?>();

        // Validate user
        public bool ValidateOrigin (ServiceData origin) {
            if (!guidList.ContainsKey(origin.User.Name)) return false;
            if (guidList[origin.User.Name] != origin.User.Id) return false;
            return true;
        }

        // Validate login
        public Data.ReturnInfo ValidateLogin (ServiceData origin) {
            Data.ReturnInfo returnInfo;
            Data.User userLogin = UserCache.Current.LoadUser(origin.User.Name);
            if (userLogin.Name == "") returnInfo = CreateReturn(false, "Login", "Incorrect username or password", "warning");
            else if (userLogin.Password != origin.User.Password) returnInfo =  CreateReturn(false, "Login", "Incorrect username or password", "warning");
            else returnInfo = CreateReturn(true, "Login", "Login successfull", "success");
            return returnInfo;
        }

        // Login user
        public void LoginUser(ServiceData origin) {
            if (guidList.ContainsKey(origin.User.Name)) guidList.Remove(origin.User.Name);
            origin.User = UserCache.Current.LoadUser(origin.User.Name);
            origin.User.Id = Guid.NewGuid();
            guidList.Add(origin.User.Name, origin.User.Id);
        }

        // Logout user
        public Data.ReturnInfo LogoutUser(ServiceData origin) {
            if (guidList.ContainsKey(origin.User.Name)) guidList.Remove(origin.User.Name);
            origin.ResetUser();
            return CreateReturn(true, "Logout",  "Logout successful", "success");
        }

        // Create new user
        public Data.ReturnInfo CreateUserAsync (ServiceData origin) {
            Data.User newUser = new Data.User {Name = origin.User.Name, Password = origin.User.Password, Review = new Dictionary<int, Dictionary<int, int>>()};
            string toFile = JsonConvert.SerializeObject(newUser, Formatting.Indented);
            File.WriteAllText("Db/users/" + newUser.Name, toFile);
            return CreateReturn(true, "Register", "User created successfully", "success");
        }

        // Change user password
        public Data.ReturnInfo ChangePasswordAsync (ServiceData origin, string password) {
          if (!ValidateOrigin(origin)) { origin = new ServiceData { User = new Data.User { Name = "User" }}; return CreateReturn(false, "Create Deck", "Could not validate user", "danger"); }
            else if (origin.User.Password == password) return CreateReturn(false, "Change Password", "Cannot change into current password", "warning");
            origin.User.Password = password;
            string toFile = JsonConvert.SerializeObject(origin.User, Formatting.Indented);
            File.WriteAllText("Db/users/" + origin.User.Name, toFile);
            UserCache.Current.RemoveUser(origin.User.Name);
            origin.User = UserCache.Current.LoadUser(origin.User.Name);
            return CreateReturn(true, "Change Password", "Password successfully changed", "success");
        }

        // Return reviewed deck
        public Data.ReturnInfo ReturnReviewDeck (ServiceData origin) {
            if (!ValidateOrigin(origin)) { origin = new ServiceData { User = new Data.User { Name = "User" }}; return CreateReturn(false, "Review Deck", "Could not validate user", "danger"); }
            string toFile = JsonConvert.SerializeObject(origin.User, Formatting.Indented);
            File.WriteAllText("Db/users/" + origin.User.Name, toFile);
            UserCache.Current.RemoveUser(origin.User.Name);
            origin.User = UserCache.Current.LoadUser(origin.User.Name);
            return CreateReturn(true, "Review Deck", "Success", "success");
       }

        // Create Return
        public Data.ReturnInfo CreateReturn(bool success, string title = "", string message = "", string type = "") {
            return new Data.ReturnInfo {Success = success, Title = title, Message = message, Type = type};
        }
    }
}