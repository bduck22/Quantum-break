using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerForwardEffectController : MonoBehaviour
{
    public VisualEffect vfx;
    public float duration;

    private void Start()
    {
        vfx.gameObject.SetActive(true);
        vfx.Stop();
    }

    public void VfxPlay()
    {
        StartCoroutine(Playduration());
    }

    IEnumerator Playduration()
    {
        vfx.Play();

        yield return new WaitForSeconds(duration);

        vfx.Stop();
    }
}
