using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using WalkSim.WalkSim.Animators;
using WalkSim.WalkSim.Plugin;
using WalkSim.WalkSim.Rigging;

namespace WalkSim.WalkSim.Menus
{
    public class RadialMenu : MonoBehaviour
    {
        private bool cursorWasLocked;

        private List<Icon> icons;

        private AnimatorBase selectedAnimator;

        private bool wasTurning;

        private void Awake()
        {
            icons = new List<Icon>
            {
                new Icon
                {
                    Image = transform.Find("Icons/Walk").GetComponent<Image>(),
                    Direction = Vector2.up,
                    Animator = Plugin.Plugin.instance.walkAnimator
                },
                new Icon
                {
                    Image = transform.Find("Icons/Interact").GetComponent<Image>(),
                    Direction = Vector2.left,
                    Animator = Plugin.Plugin.instance.grabAnimator
                },
                new Icon
                {
                    Image = transform.Find("Icons/Pose").GetComponent<Image>(),
                    Direction = Vector2.down,
                    Animator = Plugin.Plugin.instance.handAnimator
                },
                new Icon
                {
                    Image = transform.Find("Icons/Fly").GetComponent<Image>(),
                    Direction = Vector2.right,
                    Animator = Plugin.Plugin.instance.flyAnimator
                }
            };
        }

        private void Update()
        {
            if (!((Mouse.current.position.value - new Vector2(Screen.width / 2f, Screen.height / 2f)).magnitude <
                  Screen.width / 20f))
            {
                var icon = default(Icon);
                var closestDistance = 500f;
                var closestIcons = icons.Where(icon2 => Vector2.Distance(
                    Mouse.current.position.value - new Vector2(Screen.width / 2f, Screen.height / 2f),
                    icon2.Direction) < closestDistance);
                
                foreach (var icon2 in closestIcons)
                {
                    icon = icon2;
                    closestDistance = Vector2.Distance(
                        Mouse.current.position.value - new Vector2(Screen.width / 2f, Screen.height / 2f),
                        icon2.Direction);
                }

                selectedAnimator = icon.Animator;
                foreach (var icon3 in icons)
                {
                    icon3.Image.color = icon3.Equals(icon) ? Color.white : Color.gray;
                    icon3.Image.transform.localScale = Vector3.one * (icon3.Equals(icon) ? 1.5f : 1f);
                }
            }
        }

        private void OnEnable()
        {
            cursorWasLocked = HeadDriver.instance.LockCursor;
            wasTurning = HeadDriver.instance.turn;
            HeadDriver.instance.LockCursor = false;
            HeadDriver.instance.turn = false;
        }

        private void OnDisable()
        {
            Logging.Debug("RadialMenu disabled");
            HeadDriver.instance.LockCursor = cursorWasLocked;
            HeadDriver.instance.turn = wasTurning;
            Rig.Instance.Animator = selectedAnimator;
            Logging.Debug("--Finished");
        }

        public struct Icon : IEquatable<Icon>
        {
            public Image Image;

            public Vector2 Direction;

            public AnimatorBase Animator;

            public bool Equals(Icon other)
            {
                return Equals(Image, other.Image) && Direction.Equals(other.Direction) &&
                       Equals(Animator, other.Animator);
            }

            public override bool Equals(object obj)
            {
                return obj is Icon other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Image, Direction, Animator);
            }
        }
    }
}