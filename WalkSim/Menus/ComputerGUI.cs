using System;
using System.Collections.Generic;
using System.Linq;
using GorillaLocomotion;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Serialization;
using WalkSim.WalkSim.Plugin;
using WalkSim.WalkSim.Rigging;
using WalkSim.WalkSim.Tools;

namespace WalkSim.WalkSim.Menus
{
    public class ComputerGUI : MonoBehaviour
    {
        private const float PcRange = 4f;

        public static ComputerGUI instance;

        [FormerlySerializedAs("IsInUse")] public bool isInUse;

        private readonly Dictionary<KeyControl, GorillaKeyboardBindings> buttonMapping =
            new Dictionary<KeyControl, GorillaKeyboardBindings>();

        private readonly Dictionary<GorillaKeyboardBindings, Key> keyMapping =
            new Dictionary<GorillaKeyboardBindings, Key>
            {
                {
                    GorillaKeyboardBindings.A,
                    Key.A
                },
                {
                    GorillaKeyboardBindings.B,
                    Key.B
                },
                {
                    GorillaKeyboardBindings.C,
                    Key.C
                },
                {
                    GorillaKeyboardBindings.D,
                    Key.D
                },
                {
                    GorillaKeyboardBindings.E,
                    Key.E
                },
                {
                    GorillaKeyboardBindings.F,
                    Key.F
                },
                {
                    GorillaKeyboardBindings.G,
                    Key.G
                },
                {
                    GorillaKeyboardBindings.H,
                    Key.H
                },
                {
                    GorillaKeyboardBindings.I,
                    Key.I
                },
                {
                    GorillaKeyboardBindings.J,
                    Key.J
                },
                {
                    GorillaKeyboardBindings.K,
                    Key.K
                },
                {
                    GorillaKeyboardBindings.L,
                    Key.L
                },
                {
                    GorillaKeyboardBindings.M,
                    Key.M
                },
                {
                    GorillaKeyboardBindings.N,
                    Key.N
                },
                {
                    GorillaKeyboardBindings.O,
                    Key.O
                },
                {
                    GorillaKeyboardBindings.P,
                    Key.P
                },
                {
                    GorillaKeyboardBindings.Q,
                    Key.Q
                },
                {
                    GorillaKeyboardBindings.R,
                    Key.R
                },
                {
                    GorillaKeyboardBindings.S,
                    Key.S
                },
                {
                    GorillaKeyboardBindings.T,
                    Key.T
                },
                {
                    GorillaKeyboardBindings.U,
                    Key.U
                },
                {
                    GorillaKeyboardBindings.V,
                    Key.V
                },
                {
                    GorillaKeyboardBindings.W,
                    Key.W
                },
                {
                    GorillaKeyboardBindings.X,
                    Key.X
                },
                {
                    GorillaKeyboardBindings.Y,
                    Key.Y
                },
                {
                    GorillaKeyboardBindings.Z,
                    Key.Z
                },
                {
                    GorillaKeyboardBindings.zero,
                    Key.Digit0
                },
                {
                    GorillaKeyboardBindings.one,
                    Key.Digit1
                },
                {
                    GorillaKeyboardBindings.two,
                    Key.Digit2
                },
                {
                    GorillaKeyboardBindings.three,
                    Key.Digit3
                },
                {
                    GorillaKeyboardBindings.four,
                    Key.Digit4
                },
                {
                    GorillaKeyboardBindings.five,
                    Key.Digit5
                },
                {
                    GorillaKeyboardBindings.six,
                    Key.Digit6
                },
                {
                    GorillaKeyboardBindings.seven,
                    Key.Digit7
                },
                {
                    GorillaKeyboardBindings.eight,
                    Key.Digit8
                },
                {
                    GorillaKeyboardBindings.nine,
                    Key.Digit9
                },
                {
                    GorillaKeyboardBindings.option1,
                    Key.F1
                },
                {
                    GorillaKeyboardBindings.option2,
                    Key.F2
                },
                {
                    GorillaKeyboardBindings.option3,
                    Key.F3
                },
                {
                    GorillaKeyboardBindings.enter,
                    Key.Enter
                },
                {
                    GorillaKeyboardBindings.delete,
                    Key.Backspace
                },
                {
                    GorillaKeyboardBindings.up,
                    Key.UpArrow
                },
                {
                    GorillaKeyboardBindings.down,
                    Key.DownArrow
                }
            };

        private bool inRange;

        private GorillaComputerTerminal[] terminals = Array.Empty<GorillaComputerTerminal>();

        private void Awake()
        {
            instance = this;
            isInUse = false;
        }

        private void Start() => BuildButtonMap();

        private void Update()
        {
            if (!Rig.Instance.Animator)
                return;

            if (Keyboard.current.eKey.wasPressedThisFrame && inRange && !isInUse)
                isInUse = true;

            if (!isInUse) return;

            if (!inRange || Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                isInUse = false;
                return;
            }
            
            foreach (var keyControl in buttonMapping.Keys)
                try
                {
                    if (keyControl == null) Logging.Debug("Key is null");
                    if (!(keyControl is { wasPressedThisFrame: true })) continue;
                    Logging.Debug("Pressed", keyControl.name);
                    GorillaComputer.instance.PressButton(buttonMapping[keyControl]);
                    Sounds.Play(66, 0.5f);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                }
        }

        private void FixedUpdate()
        {
            if (!Rig.Instance.Animator)
                return;

            if (Time.frameCount % 60 == 0) inRange = IsInRange();
            if (!inRange && Time.frameCount % 600 == 0)
                terminals = FindObjectsByType<GorillaComputerTerminal>(FindObjectsInactive.Exclude,
                    FindObjectsSortMode.None);
        }

        private void OnGUI()
        {
            if (!Rig.Instance.Animator)
                return;

            var text = "";
            if (inRange) text = isInUse ? "Press [Escape] to exit" : "Press [E] to use computer";
            GUI.Label(new Rect(20f, 20f, 200f, 200f), text, new GUIStyle
            {
                fontSize = 20,
                normal = new GUIStyleState
                {
                    textColor = Color.white
                }
            });
        }

        private bool IsInRange()
        {
            var position = GTPlayer.Instance.bodyCollider.transform.position;

            return position.Distance(GorillaComputer.instance.transform.position) < PcRange || terminals.Any(gorillaComputerTerminal => position.Distance(gorillaComputerTerminal.transform.position) < PcRange);
        }

        private void BuildButtonMap()
        {
            foreach (var keyValuePair in keyMapping)
                buttonMapping.Add(Keyboard.current[keyValuePair.Value], keyValuePair.Key);
        }
    }
}