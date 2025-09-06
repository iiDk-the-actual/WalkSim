using System;
using System.IO;
using System.Text.RegularExpressions;
using BepInEx;
using UnityEngine;
using WalkSim.WalkSim.Animators;
using WalkSim.WalkSim.Menus;
using WalkSim.WalkSim.Patches;
using WalkSim.WalkSim.Rigging;
using WalkSim.WalkSim.Tools;

namespace WalkSim.WalkSim.Plugin
{
    [BepInPlugin("com.goldentrophy.gorillatag.walksim", "WalkSimulator", "2.0.2")]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin instance;

        private static bool _enabled = true;

        public AnimatorBase walkAnimator;

        public AnimatorBase flyAnimator;

        public AnimatorBase emoteAnimator;

        public AnimatorBase handAnimator;

        public AnimatorBase grabAnimator;

        public ComputerGUI computerGUI;

        public AssetBundle bundle;

        public RadialMenu radialMenu;

        public bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;
                if (value)
                {
                    gameObject.GetOrAddComponent<InputHandler>();
                    gameObject.GetOrAddComponent<Rig>();
                    walkAnimator = gameObject.GetOrAddComponent<WalkAnimator>();
                    flyAnimator = gameObject.GetOrAddComponent<FlyAnimator>();
                    emoteAnimator = gameObject.GetOrAddComponent<EmoteAnimator>();
                    handAnimator = gameObject.GetOrAddComponent<PoseAnimator>();
                    grabAnimator = gameObject.GetOrAddComponent<InteractAnimator>();
                    if (!radialMenu)
                        radialMenu = Instantiate(bundle.LoadAsset<GameObject>("Radial Menu"))
                            .AddComponent<RadialMenu>();
                    computerGUI = gameObject.GetOrAddComponent<ComputerGUI>();
                    walkAnimator.enabled = false;
                    flyAnimator.enabled = false;
                    emoteAnimator.enabled = false;
                    handAnimator.enabled = false;
                    grabAnimator.enabled = false;
                }
                else
                {
                    if (InputHandler.instance != null) InputHandler.instance.Obliterate();
                    if (Rig.Instance != null) Rig.Instance.Obliterate();
                    if (walkAnimator != null) walkAnimator.Obliterate();
                    if (flyAnimator != null) flyAnimator.Obliterate();
                    if (emoteAnimator != null) emoteAnimator.Obliterate();
                    if (handAnimator != null) handAnimator.Obliterate();
                    if (grabAnimator != null) grabAnimator.Obliterate();
                    if (computerGUI != null) computerGUI.Obliterate();
                }
            }
        }

        private void Awake()
        {
            instance = this;
            Logging.Init();
            try
            {
                var text = Paths.ConfigPath + "/BepInEx.cfg";
                var text2 = File.ReadAllText(text);
                text2 = Regex.Replace(text2, "HideManagerGameObject = .+", "HideManagerGameObject = true");
                File.WriteAllText(text, text2);
            }
            catch (Exception ex)
            {
                Logging.Exception(ex);
            }

            bundle = Tools.AssetUtils.LoadAssetBundle("WalkSim/Resources/WalkSimulator");
        }

        private void Start()
        {
            Events.GameInitialized += OnGameInitialized;
        }

        private void OnEnable()
        {
            HarmonyPatches.ApplyHarmonyPatches();
        }

        private void OnDisable()
        {
            HarmonyPatches.RemoveHarmonyPatches();
        }
        
        private void OnGameInitialized(object sender, EventArgs e)
        {
            Enabled = true;
        }
    }
}