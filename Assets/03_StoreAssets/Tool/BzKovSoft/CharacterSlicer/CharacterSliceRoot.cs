using UnityEngine;

namespace BzKovSoft.CharacterSlicer
{
    public class CharacterSliceRoot : MonoBehaviour
    {
        public Transform Root;

        public Transform GetRoot()
        {
            if (Root != null)
            {
                return Root;
            }

            return transform;
        }
    }
}