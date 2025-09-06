using System.Reflection;
using HarmonyLib;

namespace WalkSim.WalkSim.Plugin
{
    public static class HarmonyPatches
    {
        public const string InstanceId = "com.kylethescientist.gorillatag.walksimulator";

        private static Harmony _instance;
        private static bool IsPatched { get; set; }

        internal static void ApplyHarmonyPatches()
        {
            if (IsPatched) return;
            _instance ??= new Harmony("com.kylethescientist.gorillatag.walksimulator");
            _instance.PatchAll(Assembly.GetExecutingAssembly());
            IsPatched = true;
        }

        internal static void RemoveHarmonyPatches()
        {
            if (_instance == null || !IsPatched) return;
            _instance.UnpatchSelf();
            IsPatched = false;
        }
    }
}