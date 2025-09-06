using GorillaLocomotion;
using UnityEngine;
using WalkSim.WalkSim.Animators;

namespace WalkSim.WalkSim.Rigging
{
    public class Rig : MonoBehaviour
    {
        private const float RaycastLength = 1.3f;

        private const float RaycastRadius = 0.3f;
        public Transform head;

        public Transform body;

        public HeadDriver headDriver;

        public HandDriver leftHand;

        public HandDriver rightHand;

        public Rigidbody rigidbody;

        public Vector3 targetPosition;

        public Vector3 lastNormal;

        public Vector3 lastGroundPosition;

        public bool onGround;

        public bool active;

        public bool useGravity = true;

        private readonly Vector3 raycastOffset = new Vector3(0f, 0.4f, 0f);

        private AnimatorBase animator;

        private float scale = 1f;
        public static Rig Instance { get; private set; }

        public Vector3 SmoothedGroundPosition { get; set; }

        public AnimatorBase Animator
        {
            get => animator;
            set
            {
                if (animator && value != animator) animator.Cleanup();
                animator = value;
                if (animator)
                {
                    animator.enabled = true;
                    animator.Setup();
                }

                leftHand.enabled = animator;
                rightHand.enabled = animator;
                headDriver.enabled = animator;

                if (animator) return;
                leftHand.Reset();
                rightHand.Reset();
            }
        }

        private void Awake()
        {
            Instance = this;
            head = GTPlayer.Instance.headCollider.transform;
            body = GTPlayer.Instance.bodyCollider.transform;
            rigidbody = GTPlayer.Instance.bodyCollider.attachedRigidbody;
            leftHand = new GameObject("WalkSim Left Hand Driver").AddComponent<HandDriver>();
            leftHand.Init(true);
            leftHand.enabled = false;
            rightHand = new GameObject("WalkSim Right Hand Driver").AddComponent<HandDriver>();
            rightHand.Init(false);
            rightHand.enabled = false;
            headDriver = new GameObject("WalkSim Head Driver").AddComponent<HeadDriver>();
            headDriver.enabled = false;
        }

        private void FixedUpdate()
        {
            scale = GTPlayer.Instance.NativeScale;
            SmoothedGroundPosition = Vector3.Lerp(SmoothedGroundPosition, lastGroundPosition, 0.2f);
            OnGroundRaycast();
            if (Animator) Animator.Animate();
            Move();
        }

        private void Move()
        {
            if (!active) return;
            rigidbody.linearVelocity =
                Vector3.Lerp(rigidbody.linearVelocity, (targetPosition - body.position) * 4f, 1f);
            if (!useGravity) rigidbody.AddForce(-Physics.gravity * (rigidbody.mass * scale));
        }

        private void OnGroundRaycast()
        {
            var raycast1 = Physics.Raycast(body.TransformPoint(raycastOffset), Vector3.down, out var raycastHit, RaycastLength * scale,
                GTPlayer.Instance.locomotionEnabledLayers);
            var raycast2 = Physics.SphereCast(body.TransformPoint(raycastOffset), RaycastRadius * scale, Vector3.down, out var raycastHit2, RaycastLength * scale,
                GTPlayer.Instance.locomotionEnabledLayers);
            RaycastHit raycastHit3;
            if (raycast1 && raycast2)
                raycastHit3 = raycastHit.distance <= raycastHit2.distance ? raycastHit : raycastHit2;
            else if (raycast2)
                raycastHit3 = raycastHit2;
            else
            {
                if (!raycast1)
                {
                    onGround = false;
                    return;
                }

                raycastHit3 = raycastHit;
            }

            lastNormal = raycastHit3.normal;
            onGround = true;
            lastGroundPosition = raycastHit3.point;
            lastGroundPosition.x = body.position.x;
            lastGroundPosition.z = body.position.z;
        }
    }
}