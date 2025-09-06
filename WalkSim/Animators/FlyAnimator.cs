using GorillaLocomotion;
using UnityEngine;
using UnityEngine.InputSystem;
using WalkSim.WalkSim.Plugin;
using WalkSim.WalkSim.Rigging;
using WalkSim.WalkSim.Tools;

namespace WalkSim.WalkSim.Animators
{
    public class FlyAnimator : AnimatorBase
    {
        private const float MaxSpeed = 5f;

        private const float MinSpeed = 0f;
        private int layersBackup;

        private bool noClipActive;

        private float speed = 1f;

        private void Awake() => layersBackup = GTPlayer.Instance.locomotionEnabledLayers;

        private void Update()
        {
            speed += Mouse.current.scroll.ReadValue().y / 50f;
            speed = Mathf.Clamp(speed, MinSpeed, MaxSpeed);

            if (!Keyboard.current.nKey.wasPressedThisFrame) return;
            noClipActive = !noClipActive;
            GTPlayer.Instance.locomotionEnabledLayers = noClipActive ? 536870912 : layersBackup;
            GTPlayer.Instance.headCollider.isTrigger = noClipActive;
            GTPlayer.Instance.bodyCollider.isTrigger = noClipActive;
        }

        public override void Animate()
        {
            AnimateBody();
            AnimateHands();
        }

        private void AnimateBody()
        {
            Rig.active = true;
            Rig.useGravity = false;
            Rig.targetPosition = Body.TransformPoint(InputHandler.inputDirection * speed);
        }

        private void AnimateHands()
        {
            LeftHand.followRate = RightHand.followRate = Extensions.Map(speed, MinSpeed, MaxSpeed, 0f, 1f);
            LeftHand.targetPosition = LeftHand.DefaultPosition;
            RightHand.targetPosition = RightHand.DefaultPosition;
            LeftHand.lookAt = LeftHand.targetPosition + Body.forward;
            RightHand.lookAt = RightHand.targetPosition + Body.forward;
            LeftHand.up = Body.right;
            RightHand.up = -Body.right;
        }

        public override void Cleanup()
        {
            base.Cleanup();
            LeftHand.followRate = RightHand.followRate = 0.1f;
            GTPlayer.Instance.locomotionEnabledLayers = layersBackup;
        }

        public override void Setup()
        {
            HeadDriver.instance.LockCursor = true;
            HeadDriver.instance.turn = true;
        }
    }
}