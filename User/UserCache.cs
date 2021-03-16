using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;

namespace Srs {

    public class UserCache {

        public static readonly UserCache Current = new UserCache();

        public int CacheMax = 1000;  // Hard cap to number of objects in Dictionary, will replace forcefully.
        public double CacheExp = 1;  // Time in hours to remove cached objects.

        public Dictionary<string, Data.User> UserList = new Dictionary<string, Data.User>();
        private Dictionary<string, DateTime> ExpirationList = new Dictionary<string, DateTime>();

        public Data.User LoadUser(string name = "") {

            Data.User userReturn;
            DateTime userExpire = DateTime.Now.AddHours(CacheExp);

            if (UserList.ContainsKey(name)) {
                userReturn = UserList[name];
                ExpirationList[name] = userExpire;
                return userReturn;
            }

            if (File.Exists("Db/users/" + name)) {
                string fileRead = File.ReadAllText("Db/users/" + name);
                userReturn =  JsonConvert.DeserializeObject<Data.User>(fileRead);

                if (UserList.Count >= CacheMax) {
                    string removeName = ExpirationList.OrderBy(x => x.Value).FirstOrDefault().Key;
                    UserList.Remove(removeName);
                    ExpirationList.Remove(removeName);
                }

                UserList.Add(userReturn.Name, userReturn);
                ExpirationList.TryAdd(userReturn.Name, userExpire);
                return userReturn;
            }

            userReturn = new Data.User { Review = new Dictionary<int, Dictionary<int, int>>() };
            return userReturn;
        }

        public void RemoveUser(string name = "") {
            if (ExpirationList.ContainsKey(name)) ExpirationList.Remove(name);
            if (UserList.ContainsKey(name)) UserList.Remove(name);
        }

        public bool UserExists(string name = "") {
            if (UserList.ContainsKey(name)) return true;
            if (File.Exists("Db/users/" + name)) return true;
            return false;
        }

        public void CleanUp (int delay, CancellationToken token) {
            Task.Run(async () => { 
                while (!token.IsCancellationRequested) {
                    foreach (var item in ExpirationList.Where(x => x.Value <= DateTime.Now)) {
                        UserList.Remove(item.Key);
                        ExpirationList.Remove(item.Key);
                    }
                    await Task.Delay(delay);
                }
            }, token);
        }
    }
}