using System.Collections.Generic;
using GorillaNetworking;
using Photon.Pun;

namespace WalkSim.WalkSim.Patches
{
    public class UtillaNetworkController : MonoBehaviourPunCallbacks
    {
        public static Events events = new Events();

        private Events.RoomJoinedArgs lastRoom;

        public override void OnJoinedRoom()
        {
            var flag = false;
            var text = "";
            if (PhotonNetwork.CurrentRoom != null)
            {
                var currentRoom = PhotonNetwork.NetworkingClient.CurrentRoom;
                flag = !currentRoom.IsVisible || currentRoom.CustomProperties.ContainsKey("Description");
                if (currentRoom.CustomProperties.TryGetValue("gameMode", out var obj)) text = obj as string;
            }

            var text2 = "";
            foreach (var keyValuePair in new Dictionary<string, string>
                     {
                         { "INFECTION", "INFECTION" },
                         { "CASUAL", "CASUAL" },
                         { "HUNT", "HUNT" },
                         { "BATTLE", "PAINTBRAWL" }
                     })
            {
                if (text.Contains(keyValuePair.Key))
                {
                    text2 = keyValuePair.Value;
                    break;
                }
            }

            GorillaComputer.instance.currentGameModeText.Value = "CURRENT MODE\n" + text2;
            var roomJoinedArgs = new Events.RoomJoinedArgs
            {
                IsPrivate = flag,
                Gamemode = text
            };
            events.TriggerRoomJoin(roomJoinedArgs);
            lastRoom = roomJoinedArgs;
        }

        public override void OnLeftRoom()
        {
            if (lastRoom != null)
            {
                events.TriggerRoomLeft(lastRoom);
                lastRoom = null;
            }

            GorillaComputer.instance.currentGameModeText.Value = "CURRENT MODE\n-NOT IN ROOM-";
        }
    }
}