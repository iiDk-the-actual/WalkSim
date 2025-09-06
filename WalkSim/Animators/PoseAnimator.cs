using GorillaLocomotion;
using UnityEngine;
using UnityEngine.InputSystem;
using WalkSim.WalkSim.Rigging;

namespace WalkSim.WalkSim.Animators
{
    public class PoseAnimator : AnimatorBase
    {
        private Vector3 eulerAngles;

        private Vector3 lookAtLeft = Vector3.forward;

        private Vector3 lookAtRight = Vector3.forward;

        private HandDriver main;

        private Vector3 offsetLeft;

        private Vector3 offsetRight;

        private HandDriver secondary;

        private float zRotationLeft;

        private float zRotationRight;

        private void Update()
        {
            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                main = secondary;
                secondary = main;
            }

            if (Keyboard.current.rKey.isPressed)
                RotateHand();
            else
                PositionHand();
            var vector = main.isLeft ? lookAtLeft : lookAtRight;
            var num = main.isLeft ? zRotationLeft : zRotationRight;
            var num2 = (float)(main.isLeft ? -1 : 1);
            main.up = Quaternion.AngleAxis(num * num2, vector) * Head.up;
            main.trigger = Mouse.current.leftButton.isPressed;
            main.grip = Mouse.current.rightButton.isPressed;
            main.primary = Mouse.current.backButton.isPressed || Keyboard.current.leftBracketKey.isPressed;
            main.secondary = Mouse.current.forwardButton.isPressed || Keyboard.current.rightBracketKey.isPressed;
        }

        public override void Animate()
        {
            Rig.headDriver.turn = false;
            AnimateBody();
            AnimateHands();
        }

        private void RotateHand()
        {
            eulerAngles.x = eulerAngles.x - Mouse.current.delta.value.y / 10f;
            if (eulerAngles.x > 180f) eulerAngles.x = eulerAngles.x - 360f;
            eulerAngles.x = Mathf.Clamp(eulerAngles.x, -85f, 85f);
            eulerAngles.y = eulerAngles.y + Mouse.current.delta.value.x / 10f;
            if (eulerAngles.y > 180f) eulerAngles.y = eulerAngles.y - 360f;
            eulerAngles.y = Mathf.Clamp(eulerAngles.y, -85f, 85f);
            if (main.isLeft)
            {
                lookAtLeft = Quaternion.Euler(eulerAngles) * Head.forward;
                zRotationLeft += Mouse.current.scroll.ReadValue().y / 5f;
            }
            else
            {
                lookAtRight = Quaternion.Euler(eulerAngles) * Head.forward;
                zRotationRight += Mouse.current.scroll.ReadValue().y / 5f;
            }
        }

        private void PositionHand()
        {
            var vector = main.isLeft ? offsetLeft : offsetRight;
            vector.z += Mouse.current.scroll.ReadValue().y / 50f;
            if (Keyboard.current.upArrowKey.wasPressedThisFrame) vector.z += 0.1f;
            if (Keyboard.current.downArrowKey.wasPressedThisFrame) vector.z -= 0.1f;
            vector.z = Mathf.Clamp(vector.z, -0.25f, 0.75f);
            vector.x += Mouse.current.delta.ReadValue().x / 1000f;
            vector.x = Mathf.Clamp(vector.x, -0.5f, 0.5f);
            vector.y += Mouse.current.delta.ReadValue().y / 1000f;
            vector.y = Mathf.Clamp(vector.y, -0.5f, 0.5f);
            if (main.isLeft)
                offsetLeft = vector;
            else
                offsetRight = vector;
        }

        private void AnimateBody()
        {
            Rig.active = true;
            Rig.useGravity = false;
            Rig.targetPosition = Body.position;
        }

        private void AnimateHands()
        {
            main.targetPosition = Body.TransformPoint(new Vector3((main.isLeft ? -1 : 1) * 0.2f, 0.1f, 0.3f) + (main.isLeft ? offsetLeft : offsetRight));
            main.lookAt = main.targetPosition + (main.isLeft ? lookAtLeft : lookAtRight);
            main.hideControllerTransform = false;
        }

        public override void Setup()
        {
            base.Start();
            HeadDriver.instance.LockCursor = true;
            main = RightHand;
            secondary = LeftHand;
            offsetLeft = Vector3.zero;
            lookAtLeft = Head.forward;
            offsetRight = Vector3.zero;
            lookAtRight = Head.forward;
            secondary.targetPosition = secondary.DefaultPosition + Vector3.up * (0.2f * GTPlayer.Instance.NativeScale);
            secondary.lookAt = secondary.targetPosition + Head.forward;
            secondary.up = Body.right * (main.isLeft ? -1 : 1);
        }
    }
}