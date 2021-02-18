using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Srs.Access
{
    public class User
    {

        public static User Current = new User();
        public List<Data.User> UserList;

        public void ValidateUser(Data.User oldUser, Data.User newUser)
        {

        }

        public void SaveUser(Data.User user) {
            // Serialize object into file
            string toJson = JsonConvert.SerializeObject(user);
            File.WriteAllText("Users/" + user.Id, toJson);

            // Update list, remove old entries first
            if (user.Id == -1) UserList.Add(user);
            else {
                UserList.RemoveAt(user.Id);
                UserList.Insert(user.Id, user);
            }
            
        }

        public void LoadUserList() {
            
            // Directory missing ?
            if (!Directory.Exists("Users/")) return;

            // Reset list
            UserList = new List<Data.User>();

            // Read files
            string[] UserPaths = Directory.GetFiles("Users/", "*.*");

            // Deserialize files into objects.
            foreach (string item in UserPaths)
            {
                string jsonArray = File.ReadAllText(item);
                Data.User fromJson = JsonConvert.DeserializeObject<Data.User>(jsonArray);
                UserList.Insert(fromJson.Id, fromJson);
            }
        }

        public void Initialize() {

        }
    }
}