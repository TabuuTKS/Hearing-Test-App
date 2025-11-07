using UnityEngine;

public class AudioWaves : MonoBehaviour
{
    public static AudioWaves instance;

    const float DURATION = 2.0f; // Seconds
    const int SAMPLE_RATE = 44100; //Hz

    [Header("Basic Wave Details")]
    [Range(125, 16000)] [SerializeField] float Freequency = 20f; //Hz
    [Range(0f,1f)] [SerializeField] float amplitude = 0.5f; // 0 to 1

    public AudioSource audioSource { get; private set; }
    public AudioClip wave {get; private set; }


    private AudioClip CreateAudioWave(float freq, float amp)
    {
        int SampleCount = (int)(SAMPLE_RATE * DURATION);
        float[] Samples = new float[SampleCount];

        for (int i = 0; i < SampleCount; i++)
        {
            Samples[i] = amp * Mathf.Sin(2 * Mathf.PI * freq * i / DURATION);
        }

        AudioClip clip = AudioClip.Create("wave", SampleCount, 1, SAMPLE_RATE, false);
        clip.SetData(Samples, 0);
        return clip;
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        wave = CreateAudioWave(Freequency, amplitude);
        instance = this;
    }
}
