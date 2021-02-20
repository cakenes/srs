using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Srs {
    
    public partial class Access {

        // Add or remove own tag
        public bool SetOwn (Guid guid, int id) {
            // Error
            if (!GuidList.ContainsKey(guid)) return false;
            // Success
            if (GuidList[guid].Own.Contains(id)) GuidList[guid].Own.Remove(id);
            else GuidList[guid].Own.Add(id);
            UpdateUser(guid);
            return true;
        }

        // Add or remove favorite tag
        public bool SetFavorite (Guid guid, int id) {
            // Error
            if (!GuidList.ContainsKey(guid)) return false;
            // Success
            if (GuidList[guid].Favorites.Contains(id)) { GuidList[guid].Favorites.Remove(id); DeckDict[id].RemoveFavorite(); }
            else { GuidList[guid].Favorites.Add(id); DeckDict[id].AddFavorite(); }  
            UpdateUser(guid);
            return true;
        }

        // Add or remove opened tag
        public bool SetOpened (Guid guid, int id) {
            // Error
            if (!GuidList.ContainsKey(guid)) return false;
            // Success
            if (GuidList[guid].Opened.Contains(id)) GuidList[guid].Opened.Remove(id);
            else GuidList[guid].Opened.Add(id);
            UpdateUser(guid);
            return true;
        }

    }
}