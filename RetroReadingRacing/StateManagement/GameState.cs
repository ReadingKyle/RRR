using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroReadingRacing.StateManagement
{
    public class GameState
    {
        public TimeSpan TrackOneHighScore { get; set; }
        public TimeSpan TrackTwoHighScore { get; set; }
        public TimeSpan TrackThreeHighScore { get; set; }
        public TimeSpan TrackFourHighScore { get; set; }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static GameState Deserialize(string serializedState)
        {
            return JsonConvert.DeserializeObject<GameState>(serializedState);
        }
    }
}
