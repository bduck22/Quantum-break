using UnityEngine;

namespace BzKovSoft.ObjectSlicer
{
    public static class RigidbodyCompat
    {
        public static Vector3 GetLinearVelocity(Rigidbody rigidbody)
        {
#if UNITY_6000_0_OR_NEWER
            return rigidbody.linearVelocity;
#else
            return rigidbody.velocity;
#endif
        }

        public static void SetLinearVelocity(Rigidbody rigidbody, Vector3 value)
        {
#if UNITY_6000_0_OR_NEWER
            rigidbody.linearVelocity = value;
#else
            rigidbody.velocity = value;
#endif
        }
    }
}