using GorillaLocomotion;
using UnityEngine;
using UnityEngine.InputSystem;
using WalkSim.WalkSim.Plugin;
using WalkSim.WalkSim.Rigging;
using WalkSim.WalkSim.Tools;

namespace WalkSim.WalkSim.Animators
{
    public class WalkAnimator : AnimatorBase
    {
        private bool hasJumped;

        private float height = 0.2f;

        private float jumpTime;

        private bool onJumpCooldown;

        private float targetHeight;

        private float walkCycleTime;

        private bool IsSprinting => Keyboard.current.leftShiftKey.isPressed;

        private bool NotMoving => InputHandler.inputDirectionNoY == Vector3.zero;

        private void Update()
        {
            if (Plugin.Plugin.instance.Enabled)
            {
                if (!hasJumped && Rig.onGround && Keyboard.current.spaceKey.wasPressedThisFrame)
                {
                    hasJumped = true;
                    onJumpCooldown = true;
                    jumpTime = Time.time;
                    Rig.active = false;
                    Rigidbody.AddForce(Vector3.up * 6.5f, ForceMode.Impulse);
                }

                if ((hasJumped && !Rig.onGround) || Time.time - jumpTime > 1f) onJumpCooldown = false;
                if (Rig.onGround && !onJumpCooldown) hasJumped = false;
            }
        }

        public override void Animate()
        {
            MoveBody();
            AnimateHands();
        }

        public void MoveBody()
        {
            Rig.active = Rig.onGround && !hasJumped;
            Rig.useGravity = !Rig.onGround;
            if (!Rig.onGround) return;
            float num;
            float num2;
            float num3;

            if (NotMoving)
            {
                num = 0.5f;
                num2 = 0.55f;
                num3 = Time.time * 3.1415927f * 2f;
            }
            else
            {
                num = 0.3f;
                num2 = 0.8f;
                num3 = walkCycleTime * 3.1415927f * 2f;
            }

            if (Keyboard.current.ctrlKey.isPressed)
            {
                num -= 0.3f;
                num2 -= 0.3f;
            }

            targetHeight = Extensions.Map(Mathf.Sin(num3), -1f, 1f, num, num2);
            height = targetHeight;
            var vector = Rig.lastGroundPosition + Vector3.up * (height * GTPlayer.Instance.NativeScale);
            var vector2 = Body.TransformDirection(InputHandler.inputDirectionNoY);
            vector2.y = 0f;
            if (Vector3.Dot(Rig.lastNormal, Vector3.up) > 0.3f)
                vector2 = Vector3.ProjectOnPlane(vector2, Rig.lastNormal);
            vector2 *= GTPlayer.Instance.NativeScale;
            vector += vector2 * (IsSprinting ? GTPlayer.Instance.maxJumpSpeed * 2f : GTPlayer.Instance.maxJumpSpeed) /
                      10f;
            Rig.targetPosition = vector;
        }

        private void AnimateHands()
        {
            LeftHand.lookAt = LeftHand.targetPosition + Body.forward;
            RightHand.lookAt = RightHand.targetPosition + Body.forward;
            LeftHand.up = Body.right;
            RightHand.up = -Body.right;
            if (!Rig.onGround)
            {
                LeftHand.grounded = false;
                RightHand.grounded = false;
                var vector = Vector3.up * (0.2f * GTPlayer.Instance.NativeScale);
                LeftHand.targetPosition = LeftHand.DefaultPosition;
                RightHand.targetPosition = RightHand.DefaultPosition + vector;
                return;
            }

            UpdateHitInfo(LeftHand);
            UpdateHitInfo(RightHand);
            if (NotMoving)
            {
                LeftHand.targetPosition = LeftHand.hit;
                RightHand.targetPosition = RightHand.hit;
                return;
            }

            if (!LeftHand.grounded && !RightHand.grounded)
            {
                LeftHand.grounded = true;
                LeftHand.lastSnap = LeftHand.hit;
                LeftHand.targetPosition = LeftHand.hit;
                RightHand.lastSnap = RightHand.hit;
                RightHand.targetPosition = RightHand.hit;
            }

            AnimateHand(LeftHand, RightHand);
            AnimateHand(RightHand, LeftHand);
        }

        private void UpdateHitInfo(HandDriver hand)
        {
            var smoothedGroundPosition = Rig.SmoothedGroundPosition;
            var lastNormal = Rig.lastNormal;
            var vector = Body.TransformDirection(InputHandler.inputDirectionNoY * Extensions.Map(
                Mathf.Abs(Vector3.Dot(InputHandler.inputDirectionNoY, Vector3.forward)), 0f, 1f,
                0.4f, 0.5f));
            vector.y = 0f;
            vector *= GTPlayer.Instance.NativeScale;
            if (!Physics.Raycast(
                    Vector3.ProjectOnPlane(hand.DefaultPosition - smoothedGroundPosition + vector, lastNormal) +
                    (smoothedGroundPosition + lastNormal * (0.3f * GTPlayer.Instance.NativeScale)), -lastNormal,
                    out var raycastHit, 0.5f * GTPlayer.Instance.NativeScale,
                    GTPlayer.Instance.locomotionEnabledLayers))
            {
                if (NotMoving) hand.targetPosition = hand.DefaultPosition;
            }
            else
            {
                hand.hit = raycastHit.point;
                hand.normal = raycastHit.normal;
                hand.lookAt = hand.transform.position + Body.forward;
            }
        }

        private void AnimateHand(HandDriver hand, HandDriver otherHand)
        {
            var num3 = Extensions.Map(Mathf.Abs(Vector3.Dot(InputHandler.inputDirectionNoY, Vector3.forward)), 0f, 1f,
                0.5f, 1.25f);
            num3 *= Extensions.Map(Vector3.Dot(Rig.lastNormal, Vector3.up), 0f, 1f, 0.1f, 0.6f) *
                    GTPlayer.Instance.NativeScale;
            var num5 = otherHand.hit.Distance(otherHand.lastSnap) / num3;
            if (otherHand.grounded && num5 >= 1f)
            {
                hand.targetPosition = hand.hit;
                hand.lastSnap = hand.hit;
                hand.grounded = true;
                otherHand.grounded = false;
            }
            else if (otherHand.grounded)
            {
                walkCycleTime = num5;
                hand.targetPosition = Vector3.Slerp(hand.lastSnap, hand.hit, walkCycleTime);
                hand.targetPosition += hand.normal * (0.2f * GTPlayer.Instance.NativeScale * Mathf.Sin(walkCycleTime));
                hand.grounded = false;
            }

            if (hand.targetPosition.Distance(hand.DefaultPosition) > 1f) hand.targetPosition = hand.DefaultPosition;
        }

        public override void Setup()
        {
            HeadDriver.instance.LockCursor = true;
            HeadDriver.instance.turn = true;
        }
    }
}