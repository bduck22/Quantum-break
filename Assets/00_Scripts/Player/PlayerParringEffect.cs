using System.Collections;
using UnityEngine;

public class PlayerParringEffect : MonoBehaviour
{
    bool OnParried;
    public void OnParring()
    {
        if (!OnParried)
        {
            OnParried = true;
        }
    }
}
