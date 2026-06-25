using UnityEngine;

public class EnemyDeadSoundPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource audio;
    
    public void Play()
    {
        audio.Play();
    }
}
