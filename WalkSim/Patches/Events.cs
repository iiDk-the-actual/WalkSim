using System;
using System.Diagnostics;

namespace WalkSim.WalkSim.Patches
{
    public class Events
    {
        [field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public static event EventHandler<RoomJoinedArgs> RoomJoined;

        [field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public static event EventHandler<RoomJoinedArgs> RoomLeft;

        [field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public static event EventHandler GameInitialized;

        public virtual void TriggerRoomJoin(RoomJoinedArgs e)
        {
            var roomJoined = RoomJoined;
            if (roomJoined != null) roomJoined.SafeInvoke(this, e);
        }

        public virtual void TriggerRoomLeft(RoomJoinedArgs e)
        {
            var roomLeft = RoomLeft;
            roomLeft?.SafeInvoke(this, e);
        }

        public virtual void TriggerGameInitialized()
        {
            var gameInitialized = GameInitialized;
            gameInitialized?.SafeInvoke(this, EventArgs.Empty);
        }

        public class RoomJoinedArgs : EventArgs
        {
            public bool IsPrivate { get; set; }

            public string Gamemode { get; set; }
        }
    }
}