using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic; // Used for lists

public class HearingTestManager : MonoBehaviour
{
    [Header("Script References")]
    [Tooltip("Drag your 'AudioWaves' script component here")]
    public AudioWaves toneGenerator;
    [Tooltip("Drag your 'UI' script component here")]
    public UI uiScript;

    [Header("UI Elements")]
    [Tooltip("The main slider for 'barely hearable' level")]
    public Slider calibrationSlider;

    [Tooltip("Button to confirm the level and move to the next frequency")]
    public Button confirmButton;

    [Tooltip("Button for 'I still can't hear anything'")]
    public Button noResponseButton;

    [Tooltip("Text to show the user (e.g., 'Calibrating 1000 Hz')")]
    public TMP_Text statusText; // Optional, but very helpful

    [Header("Test Frequencies")]
    public float[] frequenciesToTest = { 1000, 2000, 4000, 8000, 500, 250 };

    // This List will store the final calibration data
    private Dictionary<float, float> calibrationThresholds = new Dictionary<float, float>();
    private int currentFrequencyIndex = 0;

    // ---
    // AWAKE is called once when the script is loaded.
    // We set up listeners here.
    // ---
    void Awake()
    {
        // Set the slider's range to decibels
        calibrationSlider.minValue = -80f;
        calibrationSlider.maxValue = 0f;

        // **This is the fix for your slider**
        // It makes the slider interactive
        calibrationSlider.interactable = true;

        // Add listeners
        calibrationSlider.onValueChanged.AddListener(OnSliderChanged);
        confirmButton.onClick.AddListener(OnConfirmPressed);
        noResponseButton.onClick.AddListener(OnNoResponsePressed);
    }

    // ---
    // ONENABLE is called every time the GameObject is set to active.
    // This is the solution to your "starts on main menu" problem.
    // ---
    void OnEnable()
    {
        Debug.Log("Calibration Panel Enabled. Starting calibration.");

        // Reset and start the process
        currentFrequencyIndex = 0;
        calibrationThresholds.Clear();

        // Start calibration for the first frequency
        LoadFrequency(currentFrequencyIndex);
    }

    // ---
    // ONDISABLE is called every time the GameObject is set to inactive.
    // This stops the sound when you go back to the main menu.
    // ---
    void OnDisable()
    {
        Debug.Log("Calibration Panel Disabled. Stopping tone.");

        // Stop all sound
        if (toneGenerator != null && toneGenerator.audioSource != null)
        {
            toneGenerator.audioSource.Stop();
            toneGenerator.gain = -80; // Set to silent
        }
    }

    // This is called by the slider
    void OnSliderChanged(float newDbValue)
    {
        // The slider's value is the decibel (dBFS) value.
        // Pass this directly to the tone generator.
        toneGenerator.gain = (double)newDbValue;
    }

    // This loads the next frequency to be tested
    void LoadFrequency(int index)
    {
        float freq = frequenciesToTest[index];

        if (statusText != null)
        {
            statusText.text = $"Calibrating: {freq} Hz";
        }

        // Set the frequency
        toneGenerator.frequency = freq;

        // Set the slider to a default starting position (e.g., -20 dB)
        calibrationSlider.value = -60f;
        toneGenerator.gain = -60f;

        // **This is the fix for your 'sound starts randomly' problem**
        // We only play the sound when we are ready to calibrate.
        toneGenerator.audioSource.Play();
    }

    // This is called by the "Confirm" / "Next" button
    void OnConfirmPressed()
    {
        // 1. Save the current slider's value
        float freq = frequenciesToTest[currentFrequencyIndex];
        float threshold_dBFS = calibrationSlider.value;
        calibrationThresholds[freq] = threshold_dBFS;

        Debug.Log($"Threshold for {freq} Hz saved: {threshold_dBFS} dBFS");

        // 2. Move to the next frequency
        currentFrequencyIndex++;
        if (currentFrequencyIndex < frequenciesToTest.Length)
        {
            // Load the next frequency
            LoadFrequency(currentFrequencyIndex);
        }
        else
        {
            // 3. We are finished!
            FinishCalibration();
        }
    }

    // This is called by the "I can't hear it" button
    void OnNoResponsePressed()
    {
        // 1. Save a "No Response" marker (PositiveInfinity)
        float freq = frequenciesToTest[currentFrequencyIndex];
        calibrationThresholds[freq] = float.PositiveInfinity;

        Debug.Log($"No Response (NR) recorded for {freq} Hz");

        // 2. Move to the next frequency
        currentFrequencyIndex++;
        if (currentFrequencyIndex < frequenciesToTest.Length)
        {
            LoadFrequency(currentFrequencyIndex);
        }
        else
        {
            // 3. We are finished!
            FinishCalibration();
        }
    }

    void FinishCalibration()
    {
        Debug.Log("--- CALIBRATION COMPLETE ---");
        toneGenerator.audioSource.Stop();

        if (statusText != null)
        {
            statusText.text = "Calibration Complete!";
        }

        StaticDataAndHelpers.thresholds_dBFS = this.calibrationThresholds;

        // Print results to console
        foreach (var entry in calibrationThresholds)
        {
            Debug.Log($"Result: {entry.Key} Hz at {entry.Value} dBFS");
        }

        uiScript.TestMenuFunc();
    }
}