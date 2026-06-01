using System.Collections;
using UnityEngine;

public class PlayerHitEffect : MonoBehaviour
{
    public void OnHit()
    {
        Time.timeScale = 0.075f;
    }

    public void EndHit()
    {
        if (Time.timeScale == 0.075f)
        {
            Time.timeScale = 1;
        }
    }
}
