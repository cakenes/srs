using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;

namespace Srs {

    public class DeckCache {

        public static readonly DeckCache Current = new DeckCache();

        public int PooledMax = 1000 ;  // Hard cap to number of objects in Dictionary, will replace forcefully.
        public double PooledExpiration = 1;  // Time in hours to remove pooled objects.

        public Dictionary<string, Data.DeckFull> DeckList = new Dictionary<string, Data.DeckFull>();
        private Dictionary<string, DateTime> ExpirationList = new Dictionary<string, DateTime>();

        public async Task<Data.DeckFull> LoadDeck(string name = "") {

            Data.DeckFull deckReturn;
            DateTime deckExpire = DateTime.Now.AddHours(PooledExpiration);

            if (DeckList.ContainsKey(name)) {
                deckReturn = DeckList[name];
                ExpirationList[name] = DateTime.Now;
                return deckReturn;
            }

            if (File.Exists("Db/decks/" + name)) {
                string fileRead = await File.ReadAllTextAsync("Db/decks/" + name);
                deckReturn =  JsonConvert.DeserializeObject<Data.DeckFull>(fileRead);

                if (DeckList.Count >= PooledMax) {
                    string removeName = ExpirationList.OrderBy(x => x.Value).FirstOrDefault().Key;
                    DeckList.Remove(removeName);
                    ExpirationList.Remove(removeName);
                }

                DeckList.Add(deckReturn.Name, deckReturn);
                ExpirationList.Add(deckReturn.Name, deckExpire);
                return deckReturn;
            }

            deckReturn = new Data.DeckFull { Cards = new SortedDictionary<int, Data.Card>() };
            return deckReturn;
        }

        public bool DeckExists(string name = "") {
            if (DeckList.ContainsKey(name)) return true;
            if (File.Exists("Db/decks/" + name)) return true;
            return false;
        }

        public void RemoveDeck(string name = "") {
            if (ExpirationList.ContainsKey(name)) ExpirationList.Remove(name);
            if (DeckList.ContainsKey(name)) DeckList.Remove(name);
        }

        public void CleanUp (int delay, CancellationToken token) {
            Task.Run(async () => { 
                while (!token.IsCancellationRequested) {
                    foreach (var item in ExpirationList.Where(x => x.Value <= DateTime.Now)) {
                        DeckList.Remove(item.Key);
                        ExpirationList.Remove(item.Key);
                    }
                    await Task.Delay(delay);
                }
            }, token);
        }
    }
}