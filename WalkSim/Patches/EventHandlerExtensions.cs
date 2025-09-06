using System;
using WalkSim.WalkSim.Plugin;

namespace WalkSim.WalkSim.Patches
{
    public static class EventHandlerExtensions
    {
        public static void SafeInvoke(this EventHandler handler, object sender, EventArgs e)
        {
            foreach (var @delegate in handler?.GetInvocationList()!)
            {
                var eventHandler = (EventHandler)@delegate;
                try
                {
                    if (eventHandler != null) eventHandler(sender, e);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                }
            }
        }

        public static void SafeInvoke<T>(this EventHandler<T> handler, object sender, T e) where T : EventArgs
        {
            foreach (var @delegate in handler?.GetInvocationList()!)
            {
                var eventHandler = (EventHandler<T>)@delegate;
                try
                {
                    if (eventHandler != null) eventHandler(sender, e);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                }
            }
        }
    }
}