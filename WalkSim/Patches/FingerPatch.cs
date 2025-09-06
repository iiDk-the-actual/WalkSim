﻿using HarmonyLib;

namespace WalkSim.WalkSim.Patches
{
    [HarmonyPatch(typeof(ControllerInputPoller), "Update")]
    public class FingerPatch
    {
        public static bool forceLeftGrip;

        public static bool forceRightGrip;

        public static bool forceLeftTrigger;

        public static bool forceRightTrigger;

        public static bool forceLeftPrimary;

        public static bool forceRightPrimary;

        public static bool forceLeftSecondary;

        public static bool forceRightSecondary;

        private static void Postfix(ControllerInputPoller __instance)
        {
            if (Plugin.Plugin.instance.Enabled)
            {
                if (forceLeftGrip)
                {
                    __instance.leftControllerGripFloat = 1f;
                    __instance.leftGrab = true;
                    __instance.leftGrabRelease = false;
                }

                if (forceRightGrip)
                {
                    __instance.rightControllerGripFloat = 1f;
                    __instance.rightGrab = true;
                    __instance.rightGrabRelease = false;
                }

                if (forceLeftTrigger) __instance.leftControllerIndexFloat = 1f;
                if (forceRightTrigger) __instance.rightControllerIndexFloat = 1f;
                if (forceLeftPrimary) __instance.leftControllerPrimaryButton = true;
                if (forceRightPrimary) __instance.rightControllerPrimaryButton = true;
                if (forceLeftSecondary) __instance.leftControllerSecondaryButton = true;
                if (forceRightSecondary) __instance.rightControllerSecondaryButton = true;
            }
        }
    }
}