using UnityEngine;

public class UI : MonoBehaviour
{
    public void PlaySound()
    {
        if (AudioWaves.instance.wave != null)
        {
            AudioWaves.instance.audioSource.clip = AudioWaves.instance.wave;
            AudioWaves.instance.audioSource.Play();
        }
        else
        {
            Debug.LogError("Clip Isn't Created");
        }
    }

    public void StopSound()
    {
        if (AudioWaves.instance.audioSource.isPlaying)
        {
            AudioWaves.instance.audioSource.Stop();
        }
        else
        {
            Debug.LogError("Sound isn't Playing right now");
        }
    }
}
