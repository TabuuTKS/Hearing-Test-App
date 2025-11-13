using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioWaves : MonoBehaviour
{
    // Removed 'instance' - your main test manager should hold a direct 
    // reference to this component to control it.

    [Header("Tone Settings")]
    [Tooltip("Frequency in Hz (e.g., 250, 500, 1000)")]
    [Range(125f, 16000f)]
    public double frequency = 1000.0; // Note: Fixed typo 'Freequency'

    [Tooltip("Gain in decibels (dBFS). -80 is silent, 0 is max volume.")]
    [Range(-80f, 0f)]
    public double gain = -20.0; // This REPLACES your 'amplitude'

    // Public reference to the AudioSource
    public AudioSource audioSource { get; private set; }

    // --- Internal variables for live generation ---
    private double phase;
    private double increment;
    private double sampling_frequency;

    // This is the linear amplitude (0-1) calculated from 'gain'
    private double currentAmplitude;

    // We no longer pre-create a clip.
    // Removed: wave, DURATION, SAMPLE_RATE constant

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        // Get the device's sample rate automatically
        sampling_frequency = AudioSettings.outputSampleRate;

        // Ensure the AudioSource doesn't play on its own
        audioSource.playOnAwake = false;

        // We no longer create a clip, so this is gone:
        // wave = CreateAudioWave(Freequency, amplitude);
    }

    // The CreateAudioWave function is removed, as we now 
    // use OnAudioFilterRead for live generation.

    // Update is called once per frame
    void Update()
    {
        // This is the key formula:
        // It converts your 'gain' (in dB) into the 'currentAmplitude' (a 0-1 value)
        // that the audio system needs. Your slider will control the 'gain' variable.
        currentAmplitude = Mathf.Pow(10f, (float)gain / 20f);
    }

    /// <summary>
    /// This function is called by Unity's audio system to get the next
    /// block of audio samples. It generates the sine wave live.
    /// </summary>
    void OnAudioFilterRead(float[] data, int channels)
    {
        // Calculate the step for the next audio sample's phase
        increment = frequency * 2.0 * Mathf.PI / sampling_frequency;

        for (int i = 0; i < data.Length; i += channels)
        {
            phase += increment;

            // Generate the sine wave sample
            data[i] = (float)(currentAmplitude * Mathf.Sin((float)phase));

            // Copy to other channels (for stereo)
            if (channels == 2)
            {
                data[i + 1] = data[i];
            }

            // Keep phase in a manageable range (wraps around 2*PI)
            if (phase > (Mathf.PI * 2))
            {
                phase -= (Mathf.PI * 2);
            }
        }
    }
}