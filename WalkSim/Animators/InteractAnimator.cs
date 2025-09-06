using System.Collections;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.InputSystem;
using WalkSim.WalkSim.Rigging;
using WalkSim.WalkSim.Tools;

namespace WalkSim.WalkSim.Animators
{
    public class InteractAnimator : AnimatorBase
    {
        private enum State
        {
            Idle,
            Wait,
            Button
        }

        private Transform reticle;

        private State state;

        protected override void Start()
        {
            base.Start();
            reticle = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
            reticle.localScale = Vector3.one * 0.001f;
            reticle.GetComponent<MeshRenderer>().material.color = Color.white;
            reticle.GetComponent<MeshRenderer>().material.shader = Shader.Find("GorillaTag/UberShader");
            reticle.GetComponent<SphereCollider>().enabled = false;
            reticle.transform.SetParent(Camera.main.transform);
            reticle.transform.localPosition = Vector3.forward * 0.1f;
            reticle.gameObject.SetActive(false);
        }

        private void Update()
        {
            var flag = state > State.Idle;
            var flag2 = !flag;
            if (flag2)
            {
                var isPressed = Mouse.current.leftButton.isPressed;
                var flag3 = isPressed;
                if (flag3)
                {
                    state = State.Wait;
                    StartCoroutine(Raycast(LeftHand));
                }
                else
                {
                    var isPressed2 = Mouse.current.rightButton.isPressed;
                    var flag4 = isPressed2;
                    if (flag4)
                    {
                        state = State.Wait;
                        StartCoroutine(Raycast(RightHand));
                    }
                }
            }
        }

        public override void Animate()
        {
            reticle.gameObject.SetActive(true);
            HeadDriver.instance.FirstPerson = true;
            AnimateBody();
            AnimateHands();
        }

        private void AnimateBody()
        {
            Rig.active = true;
            Rig.useGravity = false;
            Rig.targetPosition = Body.position;
        }

        private IEnumerator Raycast(HandDriver main)
        {
            var ray = new Ray(Camera.main.transform.position, reticle.position - Camera.main.transform.position);
            var buttonLayer = LayerMask.GetMask("GorillaInteractable", "TransparentFX");
            var hits = Physics.RaycastAll(ray, 0.82f, buttonLayer);
            foreach (var hit in hits)
            {
                var flag = hit.transform.GetComponent<GorillaPressableButton>() ||
                           hit.transform.GetComponent<GorillaKeyboardButton>() ||
                           hit.transform.GetComponent<GorillaPlayerLineButton>() ||
                           hit.transform.name.ToLower().Contains("button");
                var flag2 = flag;
                if (flag2) yield return PressButton(main, hit.point - Camera.main.transform.forward * 0.05f);
            }

            state = State.Idle;
            yield break;
        }

        private IEnumerator PressButton(HandDriver hand, Vector3 targetPosition)
        {
            state = State.Button;
            hand.grip = true;
            hand.targetPosition = reticle.position;
            hand.lookAt = targetPosition;
            hand.up = hand.isLeft ? Head.right : -Head.right;
            yield return new WaitForSeconds(0.1f);
            hand.targetPosition = targetPosition;
            while (hand.transform.position.Distance(targetPosition) > 0.05f) yield return new WaitForFixedUpdate();
            hand.targetPosition = reticle.position + Camera.main.transform.forward * 0.05f;
            yield return new WaitForSeconds(0.1f);
            hand.targetPosition = hand.DefaultPosition;
            state = State.Idle;
            yield break;
        }

        private void AnimateHands()
        {
            var flag = state == State.Idle;
            var flag2 = flag;
            if (flag2)
            {
                LeftHand.targetPosition = LeftHand.DefaultPosition;
                RightHand.targetPosition = RightHand.DefaultPosition;
                LeftHand.lookAt = LeftHand.targetPosition + Head.forward;
                RightHand.lookAt = RightHand.targetPosition + Head.forward;
                LeftHand.up = Head.right;
                RightHand.up = -Head.right;
            }
        }

        public override void Setup()
        {
            HeadDriver.instance.LockCursor = true;
        }

        public override void Cleanup()
        {
            base.Cleanup();
            reticle.gameObject.SetActive(false);
            state = State.Idle;
            StopAllCoroutines();
        }

        private void OnDestory()
        {
            Destroy(reticle.gameObject);
        }
    }
}