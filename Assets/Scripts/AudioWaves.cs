using UnityEngine;

// We add LineRenderer to the requirements
[RequireComponent(typeof(AudioSource))]
public class AudioWaves : MonoBehaviour
{
    [Header("Tone Settings")]
    [Range(125f, 16000f)]
    public double frequency = 1000.0;

    [Range(-80f, 0f)]
    public double gain = -20.0;

    [Tooltip("-1 = Left, 0 = Center, 1 = Right")]
    [Range(-1f, 1f)]
    public float pan = 0f;

    public AudioSource audioSource { get; private set; }

    private double phase;
    private double increment;
    private double sampling_frequency;
    private double currentAmplitude;

    // --- NEW VISUALIZATION ---
    [Header("Visualization Settings")]
    public int resolution = 200; // Number of points on the line
    public float waveWidth = 10f;  // How wide the wave is drawn
    public float waveHeight = 2f;  // How tall the wave is drawn
    public float waveSpeed = 2f;   // How fast the wave animates
    [SerializeField] LineRenderer lineRenderer;

    private bool isVisualizing = false;
    // --- END NEW VISUALIZATION ---


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        sampling_frequency = AudioSettings.outputSampleRate;
        audioSource.playOnAwake = false;

        // --- NEW ---
        // Get and configure the LineRenderer
        lineRenderer.positionCount = resolution;
        lineRenderer.useWorldSpace = false; // Draw relative to this GameObject
        lineRenderer.widthMultiplier = 0.1f; // Give the line some thickness
        lineRenderer.gameObject.SetActive(false); // Start hidden
        // --- END NEW ---
    }

    void Update()
    {
        currentAmplitude = Mathf.Pow(10f, (float)gain / 20f);

        // --- NEW ---
        // This block checks if we should be drawing the wave
        // It's "playing" if the source is playing AND the gain is above the minimum
        bool isPlaying = audioSource.isPlaying && gain > -80.0f;

        if (isPlaying)
        {
            if (!isVisualizing)
            {
                // Just started, show the renderer
                isVisualizing = true;
                lineRenderer.gameObject.SetActive(true);
            }
            // Update the wave drawing every frame
            DrawSineWave();
        }
        else
        {
            if (isVisualizing)
            {
                // Just stopped, hide the renderer
                isVisualizing = false;
                lineRenderer.gameObject.SetActive(false);
                lineRenderer.positionCount = 0; // Clear the line
            }
        }
        // --- END NEW ---
    }

    // --- (OnAudioFilterRead is unchanged) ---
    void OnAudioFilterRead(float[] data, int channels)
    {
        increment = frequency * 2.0 * Mathf.PI / sampling_frequency;

        for (int i = 0; i < data.Length; i += channels)
        {
            phase += increment;

            float sample = (float)(currentAmplitude * Mathf.Sin((float)phase));

            // Calculate left/right volumes based on pan
            float leftVol = (pan <= 0) ? 1.0f : 1.0f - pan;
            float rightVol = (pan >= 0) ? 1.0f : 1.0f + pan;

            // Apply the sample to the correct channels
            if (channels > 0)
                data[i] = sample * leftVol; // Left channel
            if (channels > 1)
                data[i + 1] = sample * rightVol; // Right channel

            if (phase > (Mathf.PI * 2))
            {
                phase -= (Mathf.PI * 2);
            }
        }
    }

    // --- NEW ---
    /// <summary>
    /// Updates the LineRenderer to show a moving sine wave.
    /// Copied from SineWaveVisualizer.cs
    /// </summary>
    void DrawSineWave()
    {
        if (lineRenderer.positionCount != resolution)
        {
            lineRenderer.positionCount = resolution; // Ensure we have the right number of points
        }

        // This creates the "movement" effect by offsetting the phase by Time.time
        // We use (float)frequency, which is the class variable
        float timePhase = Time.time * (float)frequency * waveSpeed * 0.01f;

        for (int i = 0; i < resolution; i++)
        {
            float x = (float)i / (resolution - 1); // Normalized x (0 to 1)

            // This determines how many full waves are visible
            float cycles = 3f;

            // Calculate the y position of the sine wave
            float y = Mathf.Sin((x * cycles * 2 * Mathf.PI) + timePhase);

            // Scale to our visual coordinates
            float xPos = x * waveWidth - (waveWidth / 2f); // Center the wave horizontally
            float yPos = y * waveHeight;

            lineRenderer.SetPosition(i, new Vector3(xPos, yPos, 0));
        }
    }
    // --- END NEW ---
}