using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundRandomPlayer : MonoBehaviour
{
    public AudioClip[] Clips;

    AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    [SerializeField] private bool OnStartPlay;

    private void OnEnable()
    {
        if (OnStartPlay)
        {
            SoundPlay();
        }
    }

    int lastIndex = -1;

    public void SoundPlay()
    {
        source.Stop();
        int index;

        if (Clips.Length == 1)
        {
            index = 0;
        }
        else
        {
            do
            {
                index = Random.Range(0, Clips.Length);
            }
            while (index == lastIndex);
        }

        lastIndex = index;

        source.PlayOneShot(Clips[index]);
    }

    public void Stop()
    {
        source.Stop();
    }
}
