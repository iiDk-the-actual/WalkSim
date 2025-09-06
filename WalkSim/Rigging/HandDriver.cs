using System;
using System.Collections;
using GorillaLocomotion;
using UnityEngine;
using WalkSim.WalkSim.Patches;
using WalkSim.WalkSim.Plugin;

namespace WalkSim.WalkSim.Rigging
{
    public class HandDriver : MonoBehaviour
    {
        public bool grip;

        public bool trigger;

        public bool primary;

        public bool secondary;

        public bool isLeft;

        public bool grounded;

        public bool hideControllerTransform = true;

        public Vector3 targetPosition;

        public Vector3 lookAt;

        public Vector3 up;

        public float followRate = 0.1f;

        public Vector3 hit;

        public Vector3 lastSnap;

        public Vector3 normal;

        private Transform body;

        private Transform controller;

        private Vector3 defaultOffset;

        private VRMap handMap;

        public Vector3 DefaultPosition => body.TransformPoint(defaultOffset);

        public void Reset()
        {
            grip = trigger = primary = false;
            targetPosition = DefaultPosition;
            lookAt = targetPosition + body.forward;
            up = isLeft ? body.right : -body.right;
            hideControllerTransform = true;
            grounded = false;
        }

        private void FixedUpdate()
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, followRate);
            transform.LookAt(lookAt, up);
            controller.position = hideControllerTransform ? body.position : transform.position;
            if (isLeft)
            {
                FingerPatch.forceLeftGrip = grip;
                FingerPatch.forceLeftPrimary = primary;
                FingerPatch.forceLeftSecondary = secondary;
                FingerPatch.forceLeftTrigger = trigger;
            }
            else
            {
                FingerPatch.forceRightGrip = grip;
                FingerPatch.forceRightPrimary = primary;
                FingerPatch.forceRightSecondary = secondary;
                FingerPatch.forceRightTrigger = trigger;
            }
        }

        private void OnEnable()
        {
            if (!body || Rig.Instance.Animator == null) return;
            transform.position = DefaultPosition;
            targetPosition = DefaultPosition;
            try
            {
                handMap.overrideTarget = transform;
            }
            catch (Exception ex)
            {
                Logging.Exception(ex);
            }
        }

        public void Init(bool initIsLeft)
        {
            isLeft = initIsLeft;
            defaultOffset = new Vector3(initIsLeft ? -0.25f : 0.25f, -0.45f, 0.2f);
            handMap = initIsLeft
                ? GorillaTagger.Instance.offlineVRRig.leftHand
                : GorillaTagger.Instance.offlineVRRig.rightHand;
            body = Rig.Instance.body;
            controller = initIsLeft
                ? GTPlayer.Instance.leftControllerTransform
                : GTPlayer.Instance.rightControllerTransform;
            handMap = initIsLeft
                ? GorillaTagger.Instance.offlineVRRig.leftHand
                : GorillaTagger.Instance.offlineVRRig.rightHand;
            transform.position = DefaultPosition;
            targetPosition = DefaultPosition;
            lastSnap = DefaultPosition;
            hit = DefaultPosition;
            up = Vector3.up;
        }

        // ReSharper disable once UnusedMember.Local
        private IEnumerator Disable(Action<HandDriver> onDisable)
        {
            transform.position = DefaultPosition;
            yield return new WaitForSeconds(0.1f);
            handMap.overrideTarget = null;
            enabled = false;
            onDisable?.Invoke(this);
        }
    }
}