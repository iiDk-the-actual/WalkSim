using UnityEngine;
using UnityEngine.InputSystem;
using WalkSim.WalkSim.Animators;
using WalkSim.WalkSim.Menus;
using WalkSim.WalkSim.Patches;
using WalkSim.WalkSim.Rigging;
using CommonUsages = UnityEngine.XR.CommonUsages;

namespace WalkSim.WalkSim.Plugin
{
    public class InputHandler : MonoBehaviour
    {
        public static InputHandler instance;

        public static Vector3 inputDirection;

        public static Vector3 inputDirectionNoY;

        private static string _deviceName = "";

        private void Awake()
        {
            instance = this;
            ValidateDevices();
        }

        private void Update()
        {
            try
            {
                if (!ComputerGUI.instance) return;
                if (!Plugin.instance.Enabled || ComputerGUI.instance.isInUse) return;
                GetInputDirection();
                if (Keyboard.current.escapeKey.wasPressedThisFrame)
                    HeadDriver.instance.LockCursor = !HeadDriver.instance.LockCursor;
                if (Keyboard.current.cKey.wasPressedThisFrame) HeadDriver.instance.ToggleCam();
                Plugin.instance.radialMenu.enabled = Keyboard.current.tabKey.isPressed;
                Plugin.instance.radialMenu.gameObject.SetActive(Keyboard.current.tabKey.isPressed);
                if (Keyboard.current.digit1Key.wasPressedThisFrame) EnableEmote(EmoteAnimator.Emote.Wave);
                if (Keyboard.current.digit2Key.wasPressedThisFrame) EnableEmote(EmoteAnimator.Emote.Point);
                if (Keyboard.current.digit3Key.wasPressedThisFrame) EnableEmote(EmoteAnimator.Emote.ThumbsUp);
                if (Keyboard.current.digit4Key.wasPressedThisFrame) EnableEmote(EmoteAnimator.Emote.ThumbsDown);
                if (Keyboard.current.digit5Key.wasPressedThisFrame) EnableEmote(EmoteAnimator.Emote.Shrug);
                if (Keyboard.current.digit6Key.wasPressedThisFrame) EnableEmote(EmoteAnimator.Emote.Dance);
            }
            catch
            {
                // ignored
            }
        }

        private void EnableEmote(EmoteAnimator.Emote emote)
        {
            var emoteAnimator = Plugin.instance.emoteAnimator as EmoteAnimator;
            Rig.Instance.Animator = emoteAnimator;
            if (emoteAnimator) emoteAnimator.emote = emote;
        }

        private void GetInputDirection()
        {
            var vector = KeyboardInput();
            if (vector.magnitude > 0f)
            {
                inputDirection = vector.normalized;
                inputDirectionNoY = vector;
                inputDirectionNoY.y = 0f;
            }
            else
            {
                var leftControllerDevice = ControllerInputPoller.instance.leftControllerDevice;
                var rightControllerDevice = ControllerInputPoller.instance.rightControllerDevice;
                leftControllerDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out var vector2);
                rightControllerDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out var vector3);
                inputDirection = new Vector3(vector2.x, vector3.y, vector2.y).normalized;
                inputDirectionNoY = new Vector3(vector2.x, 0f, vector2.y);
            }

            inputDirectionNoY.Normalize();
        }

        private Vector3 KeyboardInput()
        {
            var num = 0f;
            var num2 = 0f;
            var num3 = 0f;
            if (Keyboard.current.aKey.isPressed) num -= 1f;
            if (Keyboard.current.dKey.isPressed) num += 1f;
            if (Keyboard.current.sKey.isPressed) num2 -= 1f;
            if (Keyboard.current.wKey.isPressed) num2 += 1f;
            if (Keyboard.current.ctrlKey.isPressed) num3 -= 1f;
            if (Keyboard.current.spaceKey.isPressed) num3 += 1f;
            return new Vector3(num, num3, num2);
        }

        private void ValidateDevices()
        {
            Events.RoomJoined += delegate
            {
                if (Rig.Instance.enabled)
                    while (Application.isFocused)
                        if (ControllerInputPoller.instance.controllerType.Equals(_deviceName))
                            _deviceName = ControllerInputPoller.instance.controllerType.ToString();
            };
        }
    }
}