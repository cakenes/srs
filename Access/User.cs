using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Srs.Access
{
    public class User
    {

        public static readonly User Current = new User();

        public List<Data.User> UserList = new List<Data.User>();
        public Dictionary<string, Data.User> Connected = new Dictionary<string, Data.User>();

        public string Login(string name, string password) {
            int index = UserList.FindIndex(x => x.Name == name);
            if (index == -1) return null;
            if (UserList[index].Password == password) { string guid = Guid.NewGuid().ToString(); Connected.Add(guid, UserList[index]); return guid; }
            return null;
        }

        public bool Register(string name, string password) {
            LoadUserList();
            int index = UserList.FindIndex(x => x.Name == name);
            if (index == -1) { SaveUser(new Data.User {Name = name, Password = password, Id = -1}); return true; }
            return false;
        }

        // OLD
        public string UserName(string guid) {
            if (Connected.ContainsKey(guid)) {
                return Connected[guid].Name;
            }
            
            return null;
        }

        public void SaveUser(Data.User user) {

            // Add or update list
            if (user.Id == -1) { user.Id =  UserList.Count; UserList.Add(user); }
            else { UserList[user.Id] = user; }

            // Serialize object into file
            string toJson = JsonConvert.SerializeObject(user);
            File.WriteAllText("users/" + user.Id, toJson);
        }

        public void LoadUserList() {
            
            // Directory missing ?
            if (!Directory.Exists("users/")) return;

            UserList = new List<Data.User>();
            string[] UserPaths = Directory.GetFiles("users/", "*.*");

            // Deserialize files into objects.
            foreach (string item in UserPaths)
            {
                string jsonArray = File.ReadAllText(item);
                Data.User fromJson = JsonConvert.DeserializeObject<Data.User>(jsonArray);
                UserList.Insert(fromJson.Id, fromJson);
            }
        }
    }
}