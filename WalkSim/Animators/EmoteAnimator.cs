using System;
using UnityEngine;
using WalkSim.WalkSim.Plugin;
using WalkSim.WalkSim.Rigging;
using Random = UnityEngine.Random;

namespace WalkSim.WalkSim.Animators
{
    public class EmoteAnimator : AnimatorBase
    {
        public enum Emote
        {
            Wave,
            Point,
            ThumbsUp,
            ThumbsDown,
            Shrug,
            Dance
        }

        public Emote emote;

        private readonly float danceSwitchRate = 1.5f;

        private int dance;

        private Func<HandDriver, float, HandPositionInfo> handPositioner;

        private float lastDanceSwitch;

        private Vector3 startingPosition;

        public override void Animate()
        {
            switch (emote)
            {
                case Emote.Wave:
                    handPositioner = WavePositioner;
                    break;
                case Emote.Point:
                    handPositioner = PointPositioner;
                    break;
                case Emote.ThumbsUp:
                    handPositioner = ThumbsUpPositioner;
                    break;
                case Emote.ThumbsDown:
                    handPositioner = ThumbsDownPositioner;
                    break;
                case Emote.Shrug:
                    handPositioner = ShrugPositioner;
                    break;
                case Emote.Dance:
                    handPositioner = DancePositioner;
                    break;
            }

            AnimateBody();
            AnimateHand(LeftHand);
            AnimateHand(RightHand);
        }

        private void AnimateBody()
        {
            Rig.active = true;
            Rig.useGravity = false;
            var flag = emote == Emote.Dance;
            var flag2 = flag;
            if (flag2)
            {
                var num = Time.time;
                var vector = new Vector3(0f, Mathf.Sin(num * 10f) * 0.1f, 0f);
                Rig.targetPosition = startingPosition + vector;
            }
            else
            {
                Rig.targetPosition = startingPosition;
            }
        }

        private void AnimateHand(HandDriver hand)
        {
            var handPositionInfo = handPositioner(hand, Time.time);
            var flag = !handPositionInfo.Used;
            var flag2 = !flag;
            if (flag2)
            {
                hand.grip = handPositionInfo.Grip;
                hand.trigger = handPositionInfo.Trigger;
                hand.primary = handPositionInfo.Thumb;
                hand.targetPosition = handPositionInfo.Position;
                hand.lookAt = handPositionInfo.LookAt;
                hand.up = handPositionInfo.Up;
            }
        }

        private HandPositionInfo DancePositioner(HandDriver hand, float t)
        {
            var flag = Time.time - lastDanceSwitch > danceSwitchRate;
            var flag2 = flag;
            if (flag2)
            {
                dance = Random.Range(0, 4);
                lastDanceSwitch = Time.time;
            }

            var handPositionInfo = default(HandPositionInfo);
            var num = hand.isLeft ? 0f : 3.1415927f;
            switch (dance)
            {
                case 0:
                {
                    var vector = hand.isLeft
                        ? hand.targetPosition + Head.right + Head.forward
                        : hand.targetPosition - Head.right + Head.forward;
                    var vector2 = new Vector3(hand.isLeft ? -0.2f : 0.2f, -0.3f, 0.2f);
                    vector2.y += Mathf.Sin(t * 10f + num) * 0.1f;
                    var handPositionInfo2 = new HandPositionInfo
                    {
                        Position = Head.TransformPoint(vector2),
                        LookAt = vector,
                        Up = Head.up,
                        Grip = true,
                        Trigger = true,
                        Thumb = true,
                        Used = true
                    };
                    var handPositionInfo3 = handPositionInfo2;
                    handPositionInfo = handPositionInfo3;
                    break;
                }
                case 1:
                {
                    var vector3 = hand.targetPosition + Head.up;
                    var vector4 = new Vector3(hand.isLeft ? -0.2f : 0.2f, -0.2f, 0.3f);
                    vector4.z += Mathf.Sin(t * 10f + num) * 0.1f;
                    var handPositionInfo2 = new HandPositionInfo
                    {
                        Position = Head.TransformPoint(vector4),
                        LookAt = vector3,
                        Up = hand.isLeft ? Head.right : -Head.right,
                        Grip = false,
                        Trigger = false,
                        Thumb = false,
                        Used = true
                    };
                    var handPositionInfo4 = handPositionInfo2;
                    handPositionInfo = handPositionInfo4;
                    break;
                }
                case 2:
                {
                    var vector5 = hand.targetPosition + Head.up;
                    var vector6 = new Vector3(hand.isLeft ? -0.2f : 0.2f, -0.3f, 0.2f);
                    vector6.y += Mathf.Sin(t * 10f + num) * 0.1f;
                    var handPositionInfo2 = new HandPositionInfo
                    {
                        Position = Head.TransformPoint(vector6),
                        LookAt = vector5,
                        Up = hand.isLeft ? Head.right : -Head.right,
                        Grip = true,
                        Trigger = false,
                        Thumb = true,
                        Used = true
                    };
                    var handPositionInfo5 = handPositionInfo2;
                    handPositionInfo = handPositionInfo5;
                    break;
                }
                case 3:
                {
                    var vector7 = hand.targetPosition + Head.up;
                    var vector8 = new Vector3(hand.isLeft ? -0.2f : 0.2f, -0.2f, 0.3f);
                    vector8.x += Mathf.Cos(t * 10f) * 0.1f;
                    vector8.z += Mathf.Sin(t * 10f) * 0.1f;
                    var handPositionInfo2 = new HandPositionInfo
                    {
                        Position = Head.TransformPoint(vector8),
                        LookAt = vector7,
                        Up = hand.isLeft ? Head.right : -Head.right,
                        Grip = true,
                        Trigger = true,
                        Thumb = true,
                        Used = true
                    };
                    var handPositionInfo6 = handPositionInfo2;
                    handPositionInfo = handPositionInfo6;
                    break;
                }
            }

            return handPositionInfo;
        }

        private HandPositionInfo ThumbsDownPositioner(HandDriver hand, float _)
        {
            var vector = hand.isLeft ? hand.targetPosition + Head.right : hand.targetPosition - Head.right;
            return new HandPositionInfo
            {
                Position = Head.TransformPoint(new Vector3(hand.isLeft ? -0.2f : 0.2f, 0f, 0.4f)),
                LookAt = vector,
                Up = -Head.up,
                Grip = true,
                Trigger = true,
                Used = true
            };
        }

        private HandPositionInfo ThumbsUpPositioner(HandDriver hand, float _)
        {
            var vector = hand.isLeft ? hand.targetPosition + Head.right : hand.targetPosition - Head.right;
            return new HandPositionInfo
            {
                Position = Head.TransformPoint(new Vector3(hand.isLeft ? -0.2f : 0.2f, 0f, 0.4f)),
                LookAt = vector,
                Up = Head.up,
                Grip = true,
                Trigger = true,
                Used = true
            };
        }

        private HandPositionInfo ShrugPositioner(HandDriver hand, float _)
        {
            var vector = hand.isLeft
                ? hand.targetPosition - Head.right + Head.forward
                : hand.targetPosition + Head.right + Head.forward;
            return new HandPositionInfo
            {
                Position = Body.TransformPoint(new Vector3(hand.isLeft ? -0.4f : 0.4f, 0f, 0.2f)),
                LookAt = vector,
                Up = -Head.forward,
                Used = true
            };
        }

        private HandPositionInfo PointPositioner(HandDriver hand, float __)
        {
            var isLeft = hand.isLeft;
            var flag = isLeft;
            HandPositionInfo handPositionInfo;
            if (flag)
                handPositionInfo = new HandPositionInfo
                {
                    Used = true,
                    Position = hand.DefaultPosition,
                    LookAt = hand.targetPosition + Head.forward,
                    Up = Head.right
                };
            else
                handPositionInfo = new HandPositionInfo
                {
                    Position = Head.TransformPoint(new Vector3(0.25f, 0f, 0.7f)),
                    LookAt = hand.targetPosition + Head.forward,
                    Up = -Head.right,
                    Grip = true,
                    Trigger = false,
                    Used = true
                };
            return handPositionInfo;
        }

        private HandPositionInfo WavePositioner(HandDriver hand, float time)
        {
            var isLeft = hand.isLeft;
            var flag = isLeft;
            HandPositionInfo handPositionInfo;
            if (flag)
            {
                handPositionInfo = new HandPositionInfo
                {
                    Used = true,
                    Position = hand.DefaultPosition,
                    LookAt = hand.targetPosition + Head.forward,
                    Up = Head.right
                };
            }
            else
            {
                var vector = new Vector3(0.25f, 0f, 0.2f);
                var num = 0.25f;
                var num2 = 0.25f;
                var num3 = 5f;
                var num4 = Mathf.Sin(Time.time * num3);
                var num5 = Mathf.Cos(Time.time * num3);
                var vector2 = Vector3.zero;
                vector2.x = num4 * num;
                vector2.y = Mathf.Abs(num5) * num2;
                vector2 += vector;
                var vector3 = hand.targetPosition - Head.TransformPoint(vector - Vector3.up * 0.25f);
                handPositionInfo = new HandPositionInfo
                {
                    Position = Head.TransformPoint(vector2),
                    LookAt = hand.targetPosition + vector3,
                    Up = -Head.right,
                    Used = true
                };
            }

            return handPositionInfo;
        }

        public override void Setup()
        {
            Logging.Debug("===SETUP===");
            base.Start();
            startingPosition = Body.position;
        }

        private struct HandPositionInfo
        {
            public Vector3 Position;

            public Vector3 LookAt;

            public Vector3 Up;

            public bool Grip;

            public bool Trigger;

            public bool Thumb;

            public bool Used;
        }
    }
}