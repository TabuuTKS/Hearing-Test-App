using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class AudiometryTest : MonoBehaviour
{
    [Header("Script References")]
    [Tooltip("Drag your 'AudioWaves' (ToneGenerator) script here")]
    public AudioWaves toneGenerator;

    [Header("UI Elements")]
    public Button heardButton;
    public TMP_Text statusText;

    // This is where the test results will be stored
    // (e.g., 1000Hz = 20dB HL, 2000Hz = 15dB HL)
    private Dictionary<float, int> audiogramResults = new Dictionary<float, int>();

    // This is the calibration data we will load
    private Dictionary<float, float> calibrationData;

    // --- Internal Test Logic Variables ---
    private int currentFreqIndex = 0;
    private int currentTestHL = 20; // Start test at 20 dB HL
    private int ascendingResponses = 0;
    private bool isWaitingForResponse = false;
    private Coroutine responseTimer;

    // The frequencies to test (MUST match calibration)
    private float[] frequenciesToTest = { 1000, 2000, 4000, 8000, 500, 250 };


    void OnEnable()
    {
        // 1. GET THE CALIBRATION DATA!
        if (StaticDataAndHelpers.thresholds_dBFS == null)
        {
            statusText.text = "ERROR: Calibration data not found!";
            // You should probably send the user back to the main menu here
            return;
        }

        calibrationData = StaticDataAndHelpers.thresholds_dBFS;

        // 2. Setup the test
        heardButton.onClick.AddListener(OnUserHeard);
        audiogramResults.Clear();

        // 3. Start the test
        StartCoroutine(RunTest());
    }

    void OnDisable()
    {
        // Stop all sound when this panel is hidden
        if (toneGenerator != null)
        {
            toneGenerator.audioSource.Stop();
        }
        StopAllCoroutines();
    }

    // This is the main test loop
    IEnumerator RunTest()
    {
        for (currentFreqIndex = 0; currentFreqIndex < frequenciesToTest.Length; currentFreqIndex++)
        {
            float freq = frequenciesToTest[currentFreqIndex];
            statusText.text = $"Testing: {freq} Hz";

            // Check if this frequency was calibrated
            if (!calibrationData.ContainsKey(freq) || float.IsPositiveInfinity(calibrationData[freq]))
            {
                Debug.Log($"Skipping {freq} Hz (No Response in calibration).");
                audiogramResults[freq] = -1; // -1 means "NR"
                continue; // Skip to next frequency
            }

            // Reset logic for this frequency
            currentTestHL = 20; // Start at 20 dB HL
            ascendingResponses = 0;

            // Run the "10-down, 5-up" logic
            while (true)
            {
                // *** THIS IS THE MOST IMPORTANT LINE ***
                // It combines your calibration data with the test level (HL)
                // to get the final dBFS value to play.
                float zero_dB_HL = calibrationData[freq]; // e.g., -55 dBFS
                float testLevel_dBFS = zero_dB_HL + currentTestHL; // e.g., -55 + 20 = -35 dBFS
                                                                   // ***

                // Safety Check: Don't play louder than 0 dBFS
                if (testLevel_dBFS > 0)
                {
                    testLevel_dBFS = 0;
                    // If we're at max volume, we can't test further
                    Debug.LogWarning($"Reached max volume for {freq} Hz.");
                    audiogramResults[freq] = -1; // Mark as No Response
                    break; // Stop testing this frequency
                }

                // 1. SET TONE
                toneGenerator.frequency = freq;
                toneGenerator.gain = testLevel_dBFS;

                // 2. PLAY TONE
                toneGenerator.audioSource.Play();
                yield return new WaitForSeconds(1.0f); // Play for 1 second
                toneGenerator.audioSource.Stop();

                // 3. WAIT FOR RESPONSE
                isWaitingForResponse = true;
                responseTimer = StartCoroutine(ResponseTimer());
                while (isWaitingForResponse)
                {
                    yield return null;
                }

                // 4. CHECK FOR THRESHOLD
                if (ascendingResponses >= 2)
                {
                    Debug.Log($"Threshold for {freq} Hz found: {currentTestHL} dB HL");
                    audiogramResults[freq] = currentTestHL;
                    break; // Found it! Move to next frequency
                }
            }
        }

        FinishTest();
    }

    IEnumerator ResponseTimer()
    {
        yield return new WaitForSeconds(2.0f); // Wait 2 seconds
        if (isWaitingForResponse)
        {
            isWaitingForResponse = false;
            OnUserMissed();
        }
    }

    void OnUserHeard()
    {
        if (!isWaitingForResponse) return;
        isWaitingForResponse = false;
        StopCoroutine(responseTimer);

        ascendingResponses++; // Record positive response
        currentTestHL -= 10; // 10-DOWN
    }

    void OnUserMissed()
    {
        ascendingResponses = 0; // Reset
        currentTestHL += 5; // 5-UP
    }

    void FinishTest()
    {
        Debug.Log("--- TEST COMPLETE ---");
        statusText.text = "Test Complete! See console for results.";

        // This is your final audiogram!
        foreach (var result in audiogramResults)
        {
            // A result of -1 means "No Response"
            string threshold = (result.Value == -1) ? "No Response" : result.Value + " dB HL";
            Debug.Log($"Result: {result.Key} Hz = {threshold}");
        }

        // Now you can move to a "Results Panel"
    }
}