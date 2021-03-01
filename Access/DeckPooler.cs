using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;

namespace Srs {

    public class DeckPooler {

        public int PooledMax = 100 ;  // Hard cap to number of objects in Dictionary, will replace forcefully.
        public double PooledExpiration = 1;  // Time in hours to remove pooled objects.

        public Dictionary<int, Data.DeckFull> DeckList = new Dictionary<int, Data.DeckFull>();
        private Dictionary<int, DateTime> ExpirationList = new Dictionary<int, DateTime>();

        public async Task<Data.DeckFull> LoadDeck(int index = 0) {

            Data.DeckFull deckReturn;
            DateTime deckExpire = DateTime.Now.AddHours(PooledExpiration);

            if (DeckList.ContainsKey(index)) {
                deckReturn = DeckList[index];
                ExpirationList[index] = DateTime.Now;
                return deckReturn;
            }

            string[] filePaths = Directory.GetFiles("decks/", index + "-*");

            if (filePaths.Length >= 1) {
                string fileRead = await File.ReadAllTextAsync(filePaths[0]);
                deckReturn =  JsonConvert.DeserializeObject<Data.DeckFull>(fileRead);

                if (DeckList.Count >= PooledMax) {
                    int removeIndex = ExpirationList.OrderBy(x => x.Value).FirstOrDefault().Key;
                    DeckList.Remove(removeIndex);
                    ExpirationList.Remove(removeIndex);
                }

                DeckList.Add(deckReturn.Id, deckReturn);
                ExpirationList.Add(deckReturn.Id, deckExpire);
                return deckReturn;
            }

            deckReturn = new Data.DeckFull { Cards = new SortedDictionary<int, Data.Card>() };
            return deckReturn;

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