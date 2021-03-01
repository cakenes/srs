using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;

namespace Srs {

    public class UserPooler {

        public int PooledMax = 100;  // Hard cap to number of objects in Dictionary, will replace forcefully.
        public double PooledExpiration = 1;  // Time in hours to remove pooled objects.

        public Dictionary<string, Data.User> UserList;
        private Dictionary<string, DateTime> ExpirationList;

        public async Task<Data.User> LoadUser(string name = "") {

            Data.User userReturn;
            DateTime userExpire = DateTime.Now.AddHours(PooledExpiration);

            if (UserList.ContainsKey(name)) {
                userReturn = UserList[name];
                ExpirationList[name] = userExpire;
                return userReturn;
            }

            string[] filePaths = Directory.GetFiles("users/", name);

            if (filePaths.Length >= 1) {
                string fileRead = await File.ReadAllTextAsync(filePaths[0]);
                userReturn =  JsonConvert.DeserializeObject<Data.User>(fileRead);

                if (UserList.Count >= PooledMax) {
                    string removeName = ExpirationList.OrderBy(x => x.Value).FirstOrDefault().Key;
                    UserList.Remove(removeName);
                    ExpirationList.Remove(removeName);
                }

                UserList.Add(userReturn.Name, userReturn);
                ExpirationList.Add(userReturn.Name, userExpire);
                return userReturn;
            }

            userReturn = new Data.User { Review = new Dictionary<int, Dictionary<int, int>>() };
            return userReturn;
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