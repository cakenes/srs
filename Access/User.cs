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
            Load();
        }

        public string Login(string name, string password) {
            int index = UserList.FindIndex(x => x.Name == name);
            if (index == -1) return null;
            if (UserList[index].Password == password) { 
                Data.Connected connected = new Data.Connected {User = UserList[index], Guid = Guid.NewGuid().ToString()};
                GuidList.Add(connected);
                return connected.Guid; }
            return null;
        }

        public bool Register(string name, string password) {
            int index = UserList.FindIndex(x => x.Name == name);
            if (index == -1) { 
                Data.User user = new Data.User {Name = name, Password = password, Id = -1};
                Save(user);
                return true; }
            return false;
        }

        public Data.User Name(string guid) {
            int index = GuidList.FindIndex(x => x.Guid == guid);
            if (index != -1) { return GuidList[index].User; }
            return new Data.User();
        }

        public void Save(Data.User user) {
            if (user.Id == -1) { user.Id =  UserList.Count; UserList.Add(user); }
            else { UserList[user.Id] = user; }
            string toJson = JsonConvert.SerializeObject(user);
            File.WriteAllText("users/" + user.Id, toJson);
        }

        public void Load() {
            if (!Directory.Exists("users/")) return;
            UserList = new List<Data.User>();
            string[] UserPaths = Directory.GetFiles("users/", "*.*");
            foreach (string item in UserPaths)
            {
                string jsonArray = File.ReadAllText(item);
                Data.User fromJson = JsonConvert.DeserializeObject<Data.User>(jsonArray);
                UserList.Add(fromJson);
            }
        }
    }
}