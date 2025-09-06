using UnityEngine;
using WalkSim.WalkSim.Plugin;
using WalkSim.WalkSim.Rigging;

namespace WalkSim.WalkSim.Animators
{
    public abstract class AnimatorBase : MonoBehaviour
    {
        protected Transform Body;

        protected Transform Head;

        protected HandDriver LeftHand;

        protected Rig Rig;

        protected HandDriver RightHand;

        protected Rigidbody Rigidbody;

        protected virtual void Start()
        {
            Logging.Debug("==START==");
            Rig = Rig.Instance;
            Body = Rig.body;
            Head = Rig.head;
            Rigidbody = Rig.rigidbody;
            LeftHand = Rig.leftHand;
            RightHand = Rig.rightHand;
        }

        public abstract void Setup();

        public virtual void Cleanup()
        {
            enabled = false;
            Rig.active = false;
            Rig.useGravity = true;
            Rig.headDriver.turn = true;
            LeftHand.Reset();
            RightHand.Reset();
        }

        public abstract void Animate();
    }
}