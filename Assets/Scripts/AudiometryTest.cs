using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public enum TestEar { Left, Right, Done }

public class AudiometryTest : MonoBehaviour
{
    [Header("Script References")]
    public AudioWaves toneGenerator;
    public UI uiManager;

    [Header("UI Elements")]
    public Button heardButton;
    public Button noResponseButton;
    public TMP_Text statusText;

    private Dictionary<float, int> audiogramResultsLeft = new Dictionary<float, int>();
    private Dictionary<float, int> audiogramResultsRight = new Dictionary<float, int>();
    private TestEar currentEar = TestEar.Left;

    private Dictionary<float, float> calibrationData;
    private int currentFreqIndex = 0;
    private int currentTestHL = 20;
    private bool isWaitingForResponse = false;
    private Coroutine responseTimer;
    private float[] frequenciesToTest = { 1000, 2000, 4000, 8000, 500, 250 };
    private int maxTestHL = 100;

    // --- NEW ---
    // This flag will be set to true by OnUserHeard() to break the loop.
    private bool isFrequencyDone = false;

    void OnEnable()
    {
        if (StaticDataAndHelpers.thresholds_dBFS == null)
        {
            statusText.text = "ERROR: Calibration data not found!";
            return;
        }

        calibrationData = StaticDataAndHelpers.thresholds_dBFS;

        heardButton.onClick.AddListener(OnUserHeard);
        noResponseButton.onClick.AddListener(OnUserDidNotHear);

        StartCoroutine(RunFullTestBattery());
    }

    void OnDisable()
    {
        if (toneGenerator != null) { toneGenerator.audioSource.Stop(); }
        StopAllCoroutines();
        if (heardButton != null) heardButton.onClick.RemoveListener(OnUserHeard);
        if (noResponseButton != null) noResponseButton.onClick.RemoveListener(OnUserDidNotHear);
    }

    IEnumerator RunFullTestBattery()
    {
        // 1. TEST LEFT EAR
        currentEar = TestEar.Left;
        toneGenerator.pan = -1.0f;
        audiogramResultsLeft.Clear();
        statusText.text = "Testing: LEFT EAR";
        yield return new WaitForSeconds(2.0f);
        yield return StartCoroutine(RunTestForCurrentEar());

        // 2. TEST RIGHT EAR
        currentEar = TestEar.Right;
        toneGenerator.pan = 1.0f;
        audiogramResultsRight.Clear();
        statusText.text = "Testing: RIGHT EAR";
        yield return new WaitForSeconds(2.0f);
        yield return StartCoroutine(RunTestForCurrentEar());

        // 3. FINISH
        currentEar = TestEar.Done;
        FinishTest();
    }

    // --- LOGIC UPDATED ---
    // This loop now only breaks when isFrequencyDone = true
    IEnumerator RunTestForCurrentEar()
    {
        for (currentFreqIndex = 0; currentFreqIndex < frequenciesToTest.Length; currentFreqIndex++)
        {
            float freq = frequenciesToTest[currentFreqIndex];
            statusText.text = $"Testing {currentEar} Ear: {freq} Hz";

            if (!calibrationData.ContainsKey(freq) || float.IsPositiveInfinity(calibrationData[freq]))
            {
                RecordResult(-1);
                continue;
            }

            currentTestHL = 20;
            isFrequencyDone = false; // Reset the flag for the new frequency

            while (!isFrequencyDone) // Loop until "Heard" is pressed
            {
                float zero_dB_HL = calibrationData[freq];
                float testLevel_dBFS = zero_dB_HL + currentTestHL;

                // Check 1: Clinical limit
                if (currentTestHL > maxTestHL)
                {
                    RecordResult(-1);
                    isFrequencyDone = true; // Break the loop
                    continue; // Skip the rest of this loop iteration
                }

                // Check 2: Physical limit
                if (testLevel_dBFS > 0)
                {
                    testLevel_dBFS = 0;
                    RecordResult(-1);
                    isFrequencyDone = true; // Break the loop
                    continue; // Skip the rest of this loop iteration
                }

                toneGenerator.frequency = freq;
                toneGenerator.gain = testLevel_dBFS;

                toneGenerator.audioSource.Play();
                yield return new WaitForSeconds(1.0f);
                toneGenerator.audioSource.Stop();

                isWaitingForResponse = true;
                responseTimer = StartCoroutine(ResponseTimer());
                while (isWaitingForResponse) { yield return null; }

                // The loop will now repeat at a new HL if "No Response" was pressed,
                // or it will exit if "Heard" was pressed (isFrequencyDone = true)
            }
        }
    }

    void RecordResult(int threshold)
    {
        float freq = frequenciesToTest[currentFreqIndex];
        if (currentEar == TestEar.Left)
        {
            audiogramResultsLeft[freq] = threshold;
        }
        else
        {
            audiogramResultsRight[freq] = threshold;
        }
    }

    IEnumerator ResponseTimer()
    {
        yield return new WaitForSeconds(2.0f);
        if (isWaitingForResponse)
        {
            isWaitingForResponse = false;
            OnUserMissed(); // Treat timer running out as "No Response"
        }
    }

    // --- LOGIC UPDATED ---
    void OnUserHeard()
    {
        if (!isWaitingForResponse) return;
        isWaitingForResponse = false;
        StopCoroutine(responseTimer);

        // 1. Record the threshold at the *current* level
        RecordResult(currentTestHL);

        // 2. Set the flag to true to break the 'while' loop
        isFrequencyDone = true;
    }

    public void OnUserDidNotHear()
    {
        if (!isWaitingForResponse) return;
        isWaitingForResponse = false;
        StopCoroutine(responseTimer);

        OnUserMissed();
    }

    // --- LOGIC UPDATED ---
    void OnUserMissed()
    {
        // "No Response" simply increases the level for the next loop.
        // It no longer resets any counters.
        currentTestHL += 5;
    }

    void FinishTest()
    {
        Debug.Log("--- TEST COMPLETE ---");
        statusText.text = "Test Complete!";
        toneGenerator.pan = 0f;

        StaticDataAndHelpers.audiogramResultsLeft = this.audiogramResultsLeft;
        StaticDataAndHelpers.audiogramResultsRight = this.audiogramResultsRight;

        if (uiManager != null)
        {
            uiManager.ShowResultsMenu();
        }
    }
}
