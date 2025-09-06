using GorillaLocomotion;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using WalkSim.WalkSim.Plugin;

namespace WalkSim.WalkSim.Rigging
{
    public class HeadDriver : MonoBehaviour
    {
        public static HeadDriver instance;

        public Transform thirpyTarget;

        public Transform head;

        public GameObject cameraObject;

        public bool turn = true;

        private readonly Vector3 offset = new Vector3(0f, 0f, 0f);

        private bool lockCursor;

        private Camera overrideCam;

        public bool LockCursor
        {
            get => lockCursor;
            set
            {
                lockCursor = value;
                var cursor = lockCursor;
                if (cursor)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }
        }

        public bool FirstPerson
        {
            get => overrideCam.enabled;
            set => overrideCam.enabled = value;
        }

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            var cinemachine3RdPersonFollow = FindFirstObjectByType<Cinemachine3rdPersonFollow>();
#pragma warning restore CS0618 // Type or member is obsolete
            thirpyTarget = cinemachine3RdPersonFollow.VirtualCamera.Follow;
            var componentInParent = cinemachine3RdPersonFollow.gameObject.GetComponentInParent<Camera>();
            cameraObject = Plugin.Plugin.instance.bundle.LoadAsset<GameObject>("Override Camera");
            cameraObject = Instantiate(cameraObject);
            overrideCam = cameraObject.GetComponent<Camera>();
            overrideCam.nearClipPlane = componentInParent.nearClipPlane;
            overrideCam.farClipPlane = componentInParent.farClipPlane;
            overrideCam.cullingMask = componentInParent.cullingMask;
            overrideCam.depth = componentInParent.depth + 1f;
            overrideCam.targetDisplay = componentInParent.targetDisplay;
            overrideCam.fieldOfView = 90f;
            overrideCam.enabled = false;
        }

        private void LateUpdate()
        {
            cameraObject.transform.position = GTPlayer.Instance.headCollider.transform.TransformPoint(offset);
            cameraObject.transform.forward = head.forward;
            if (!turn) return;
            GTPlayer.Instance.Turn(Mouse.current.delta.value.x / 10f);
            var eulerAngles = GorillaTagger.Instance.offlineVRRig.headConstraint.eulerAngles;
            eulerAngles.x -= Mouse.current.delta.value.y / 10f;
            if (eulerAngles.x > 180f) eulerAngles.x -= 360f;
            eulerAngles.x = Mathf.Clamp(eulerAngles.x, -60f, 60f);
            GorillaTagger.Instance.offlineVRRig.headConstraint.eulerAngles = eulerAngles;
            eulerAngles.y += 90f;
            thirpyTarget.localEulerAngles = new Vector3(eulerAngles.x, 0f, 0f);
            GTPlayer.Instance.headCollider.transform.localEulerAngles = new Vector3(eulerAngles.x, 0f, 0f);
        }

        private void OnEnable()
        {
            Logging.Debug("Enabled");
            if (Rig.Instance.Animator == null) return;
            LockCursor = true;
            OverrideHeadMovement();
        }

        private void OnDisable()
        {
            if (head)
            {
                LockCursor = false;
                GorillaTagger.Instance.offlineVRRig.head.rigTarget = head;
            }
        }

        private void OverrideHeadMovement()
        {
            head = GorillaTagger.Instance.offlineVRRig.head.rigTarget;
        }

        internal void ToggleCam()
        {
            overrideCam.enabled = !overrideCam.enabled;
        }
    }
}