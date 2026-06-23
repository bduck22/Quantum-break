using System.Collections;
using UnityEngine;

public class PlayerHitEffect : MonoBehaviour
{
    public GameObject HitIamge;
    public void OnHit()
    {
        Time.timeScale = 0.075f;
        HitIamge.SetActive(true);
    }

    public void EndHit()
    {
        HitIamge.SetActive(false);
        if (Time.timeScale == 0.075f)
        {
            Time.timeScale = 1;
        }
    }
}
