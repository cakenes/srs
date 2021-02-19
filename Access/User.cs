using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Srs.Access
{
    public class User
    {
        public static readonly User Current = new User();

        public List<Data.User> UserList;
        public List<Data.Connected> GuidList;

        public void Initialize() {
            UserList = new List<Data.User>();
            GuidList = new List<Data.Connected>();
        }

        public string Login(string name, string password) {
            int index = UserList.FindIndex(x => x.Name == name);
            if (index == -1) return null;
            if (UserList[index].Password == password) { 
                Data.Connected connected = new Data.Connected {Name = UserList[index].Name, Guid = Guid.NewGuid().ToString()};
                GuidList.Add(connected);
                return connected.Guid; }
            return null;
        }

        public bool Register(string name, string password) {
            int index = UserList.FindIndex(x => x.Name == name);
            if (index == -1) { 
                Data.User user = new Data.User {Name = name, Password = password, Id = -1};
                SaveUser(user);
                return true; }
            return false;
        }

        public string Name(string guid) {
            int index = GuidList.FindIndex(x => x.Guid == guid);
            if (index != -1) { return GuidList[index].Name; }
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