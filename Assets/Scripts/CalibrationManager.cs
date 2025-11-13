using UnityEngine;
using UnityEngine.UI;

public class CalibrationManager : MonoBehaviour
{
    public AudioWaves toneGen; // Drag your ToneGenerator component here
    public Slider calibrationSlider; // Drag your UI Slider here

    // This is the array to save the thresholds
    // e.g., 0=250Hz, 1=500Hz, 2=1000Hz, 3=2k, 4=4k, 5=8k
    public float[] calibrationThresholds = new float[6];

    // The current frequency being calibrated
    private int currentFreqIndex = 2; // Start with 1000 Hz

    void Start()
    {
        // Set the slider's range in code to be safe
        calibrationSlider.minValue = -80f;
        calibrationSlider.maxValue = 0f;

        // Set the slider to a starting value (e.g., -20 dBFS)
        calibrationSlider.value = -20f;

        // IMPORTANT: Add a listener. This calls the 'OnSliderChanged' function
        // every time the user drags the slider.
        calibrationSlider.onValueChanged.AddListener(OnSliderChanged);

        // Start the first tone
        toneGen.frequency = 1000; // or whatever freq is at index 2
        OnSliderChanged(calibrationSlider.value); // Set the initial gain
    }

    // This function is now your "volume" control
    public void OnSliderChanged(float sliderValue)
    {
        // The slider's value *is* the decibel (dBFS) value.
        // Pass this directly to the tone generator.
        toneGen.gain = (double)sliderValue;
    }

    // You will call this from a "Confirm" or "Next" button
    public void LockInThreshold()
    {
        // The user is happy with this level. Save it!
        // The slider's value IS the dB threshold.
        float threshold_dBFS = calibrationSlider.value;

        // Store it
        calibrationThresholds[currentFreqIndex] = threshold_dBFS;

        //... now move to the next frequency ...
    }
}