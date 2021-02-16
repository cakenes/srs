using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Srs.Data {

    public class DataService {

        public Task<User> GetUserDataAsync() {
            return Task.FromResult(new User());
        }

        public Task<List<DeckInfo>> LoadDeckList() {
            return Task.FromResult(new List<DeckInfo>());
        }

    }

}
