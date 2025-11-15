using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;

public class RiskFactor
{
    public string factor;
    public string severity;
    public string description;
}

public class ResultsManager : MonoBehaviour
{
    [Header("Manager Reference")]
    public UI uiManager;

    [Header("UI Text Elements")]
    public TMP_Text hearingStatusText;
    public TMP_Text averageThresholdText;
    public TMP_Text riskFactorsText;
    public TMP_Text recommendationsText;

    [Header("UI Buttons")]
    public Button downloadReportButton;
    public Button retestButton;
    public Button menuButton;
    public TMP_Text downloadStatusText;

    [Header("Audiogram Chart")]
    public RectTransform audiogramChartArea;

    [Tooltip("UI Images for LEFT ear dots: 250, 500, 1k, 2k, 4k, 8k")]
    public List<RectTransform> audiogramDotsLeft; // 6 dots

    [Tooltip("UI Images for RIGHT ear dots: 250, 500, 1k, 2k, 4k, 8k")]
    public List<RectTransform> audiogramDotsRight; // 6 dots

    // --- Private Data ---
    private PatientHistory history;
    private Dictionary<float, int> resultsLeft;
    private Dictionary<float, int> resultsRight;
    private float[] frequencies = { 250, 500, 1000, 2000, 4000, 8000 };
    private string hearingStatus = "NORMAL";
    private float averageThreshold = 0;
    private List<RiskFactor> currentRisks = new List<RiskFactor>();
    private List<string> currentRecs = new List<string>();

    // This MUST match the maxTestHL in AudiometryTest.cs
    private int maxTestHL = 100;


    void Start()
    {
        if (downloadReportButton != null)
            downloadReportButton.onClick.AddListener(OnDownloadReport);
        if (retestButton != null && uiManager != null)
            retestButton.onClick.AddListener(uiManager.Retest);
        if (menuButton != null && uiManager != null)
            menuButton.onClick.AddListener(uiManager.ReturnToMainMenu);
    }

    public void DisplayResults()
    {
        this.history = StaticDataAndHelpers.patientHistory;
        this.resultsLeft = StaticDataAndHelpers.audiogramResultsLeft;
        this.resultsRight = StaticDataAndHelpers.audiogramResultsRight;

        if (this.history == null || this.resultsLeft == null || this.resultsRight == null)
        {
            hearingStatusText.text = "ERROR: Data not found.";
            return;
        }

        averageThreshold = CalculateAverageThreshold(); // Avg of both ears
        hearingStatus = GetHearingStatus(averageThreshold);
        currentRisks = GetRiskFactors();
        currentRecs = GetRecommendations(averageThreshold, currentRisks);

        hearingStatusText.text = $"YOUR HEARING IS: {hearingStatus}";
        averageThresholdText.text = $"Average threshold: {averageThreshold:F1} dB HL";

        StringBuilder sbRisks = new StringBuilder();
        foreach (var risk in currentRisks)
        {
            sbRisks.AppendLine($"<b>[{risk.severity.ToUpper()}] {risk.factor}</b>");
            sbRisks.AppendLine($"<size=24><i>{risk.description}</i></size>\n");
        }
        riskFactorsText.text = currentRisks.Count > 0 ? sbRisks.ToString() : "No significant risk factors identified.";

        StringBuilder sbRecs = new StringBuilder();
        foreach (var rec in currentRecs)
        {
            sbRecs.AppendLine($"• {rec}");
        }
        recommendationsText.text = sbRecs.ToString();

        // Plot BOTH sets of dots
        PlotAudiogram(resultsLeft, audiogramDotsLeft);
        PlotAudiogram(resultsRight, audiogramDotsRight);

        if (downloadStatusText != null) { downloadStatusText.text = ""; }
    }

    // --- THIS FUNCTION CONTAINS THE BUG FIX ---
    float CalculateAverageThreshold()
    {
        float total = 0;
        int count = 0;

        // Average LEFT ear
        foreach (var freq in frequencies)
        {
            if (resultsLeft.ContainsKey(freq))
            {
                if (resultsLeft[freq] == -1)
                {
                    // "No Response" counts as the max testable volume
                    total += maxTestHL;
                }
                else
                {
                    // Valid threshold found
                    total += resultsLeft[freq];
                }
                count++;
            }
        }
        // Average RIGHT ear
        foreach (var freq in frequencies)
        {
            if (resultsRight.ContainsKey(freq))
            {
                if (resultsRight[freq] == -1)
                {
                    // "No Response" counts as the max testable volume
                    total += maxTestHL;
                }
                else
                {
                    // Valid threshold found
                    total += resultsRight[freq];
                }
                count++;
            }
        }

        return (count == 0) ? 0 : (total / count);
    }

    // This function now plots one set of dots at a time
    void PlotAudiogram(Dictionary<float, int> results, List<RectTransform> dots)
    {
        if (audiogramChartArea == null || dots == null || dots.Count != frequencies.Length)
        {
            Debug.LogWarning("Audiogram UI not fully set up. Skipping plot.");
            return;
        }

        float chartHeight = audiogramChartArea.rect.height;
        float maxDB = 100f;
        float minDB = 0f;

        for (int i = 0; i < frequencies.Length; i++)
        {
            float freq = frequencies[i];
            if (results.ContainsKey(freq) && results[freq] != -1)
            {
                float dbValue = results[freq];
                float y_percent = 1.0f - Mathf.InverseLerp(minDB, maxDB, dbValue);

                // This calculation is improved for center pivot
                float y_pos = (y_percent - 0.5f) * chartHeight;

                dots[i].anchoredPosition = new Vector2(dots[i].anchoredPosition.x, y_pos);
                dots[i].gameObject.SetActive(true);
            }
            else
            {
                // Hide the dot if result is -1 (No Response)
                dots[i].gameObject.SetActive(false);
            }
        }
    }

    // This is called by the `downloadReportButton`
    public void OnDownloadReport()
    {
        Debug.Log("Generating report...");

        // 1. Build the report string
        string report = BuildReportString();

        // 2. Define a file path
        string fileName = $"Hearing_Test_{history.name.Replace(" ", "_")}_{DateTime.Now:yyyy-MM-dd}.txt";
        string filePath = Path.Combine(Application.persistentDataPath, fileName);

        try
        {
            // 3. Save the file
            File.WriteAllText(filePath, report);

            // 4. Notify the user
            Debug.Log($"Report saved to: {filePath}");
            if (downloadStatusText != null)
            {
                downloadStatusText.text = $"Report saved to:\n{filePath}";
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save report: {e.Message}");
            if (downloadStatusText != null)
            {
                downloadStatusText.text = "Error: Could not save report.";
            }
        }
    }

    // Added L/R results to the report
    private string BuildReportString()
    {
        StringBuilder report = new StringBuilder();
        string divider = "???????????????????????????????????????????????????????\n";

        report.AppendLine(divider);
        report.AppendLine("HEARING TEST REPORT");
        report.AppendLine(divider);
        report.AppendLine("\nPATIENT INFORMATION");
        report.AppendLine("???????????????????????????????????????????????????????");
        report.AppendLine($"Name:           {history.name}");
        report.AppendLine($"Age:            {history.age}");
        report.AppendLine($"Gender:         {history.gender}");
        report.AppendLine($"Test Date:      {DateTime.Now:MMMM dd, yyyy}");
        report.AppendLine($"\n{divider}");
        report.AppendLine("TEST RESULTS SUMMARY");
        report.AppendLine(divider);
        report.AppendLine($"Hearing Status:       {hearingStatus}");
        report.AppendLine($"Average Threshold:    {averageThreshold:F1} dB HL\n");

        report.AppendLine("\nDETAILED AUDIOGRAM DATA");
        report.AppendLine("???????????????????????????????????????????????????????");

        report.AppendLine("\nLEFT EAR:");
        foreach (float freq in frequencies)
        {
            string thresholdStr = "No Response";
            if (resultsLeft.ContainsKey(freq) && resultsLeft[freq] != -1)
                thresholdStr = $"{resultsLeft[freq]} dB HL";
            report.AppendLine($"  {freq} Hz: {thresholdStr}");
        }

        report.AppendLine("\nRIGHT EAR:");
        foreach (float freq in frequencies)
        {
            string thresholdStr = "No Response";
            if (resultsRight.ContainsKey(freq) && resultsRight[freq] != -1)
                thresholdStr = $"{resultsRight[freq]} dB HL";
            report.AppendLine($"  {freq} Hz: {thresholdStr}");
        }

        if (currentRisks.Count > 0)
        {
            report.AppendLine($"\n{divider}");
            report.AppendLine("IDENTIFIED RISK FACTORS");
            report.AppendLine(divider);
            foreach (var risk in currentRisks)
            {
                report.AppendLine($"• [{risk.severity.ToUpper()}] {risk.factor}");
                report.AppendLine($"   <i>{risk.description}</i>\n");
            }
        }

        if (currentRecs.Count > 0)
        {
            report.AppendLine($"\n{divider}");
            report.AppendLine("PERSONALIZED RECOMMENDATIONS");
            report.AppendLine(divider);
            foreach (var rec in currentRecs)
            {
                report.AppendLine($"• {rec}");
            }
        }

        report.AppendLine($"\n{divider}");
        report.AppendLine("IMPORTANT NOTICE");
        report.AppendLine(divider);
        report.AppendLine("This is a screening test and not a substitute for a professional audiological evaluation.");

        return report.ToString();
    }

    string GetHearingStatus(float avg)
    {
        if (avg < 25) return "NORMAL";
        if (avg < 40) return "MILD LOSS";
        if (avg < 60) return "MODERATE LOSS";
        return "SIGNIFICANT LOSS";
    }

    List<RiskFactor> GetRiskFactors()
    {
        var risks = new List<RiskFactor>();
        if (history.age >= 60)
        {
            risks.Add(new RiskFactor { factor = "Age-related hearing loss (Presbycusis)", severity = "moderate", description = "Natural hearing decline typically begins around age 60" });
        }
        if (history.noiseExposure.occupational || history.noiseExposure.military || (history.noiseExposure.recreational && (history.noiseExposure.duration == "5-10" || history.noiseExposure.duration == "10+")))
        {
            risks.Add(new RiskFactor { factor = "Noise-induced hearing loss", severity = "high", description = "Prolonged exposure to loud noise is a major risk factor" });
        }
        if (history.medicalConditions.diabetes)
        {
            risks.Add(new RiskFactor { factor = "Diabetes", severity = "moderate", description = "Diabetes can damage blood vessels in the inner ear" });
        }
        if (history.medicalConditions.cardiovascular || history.medicalConditions.hypertension)
        {
            risks.Add(new RiskFactor { factor = "Cardiovascular disease", severity = "moderate", description = "Reduced blood flow can affect hearing" });
        }
        if (history.ototoxicMedications.current)
        {
            risks.Add(new RiskFactor { factor = "Current ototoxic medications", severity = "high", description = "Some medications can cause temporary or permanent hearing damage" });
        }
        if (history.earHistory.suddenHearingLoss)
        {
            risks.Add(new RiskFactor { factor = "Previous sudden hearing loss", severity = "high", description = "Requires immediate medical attention and monitoring" });
        }
        if (history.earHistory.infections == "chronic")
        {
            risks.Add(new RiskFactor { factor = "Chronic ear infections", severity = "moderate", description = "Can cause conductive hearing loss" });
        }
        if (history.symptoms.tinnitus && (history.symptoms.tinnitusFrequency == "frequently" || history.symptoms.tinnitusFrequency == "constant"))
        {
            risks.Add(new RiskFactor { factor = "Persistent tinnitus", severity = "moderate", description = "Often associated with hearing loss" });
        }
        if (history.symptoms.dizziness)
        {
            risks.Add(new RiskFactor { factor = "Balance problems", severity = "moderate", description = "May indicate inner ear dysfunction" });
        }
        if (history.familyHistory)
        {
            risks.Add(new RiskFactor { factor = "Family history", severity = "low", description = "Genetic predisposition to hearing loss" });
        }
        return risks;
    }

    List<string> GetRecommendations(float avg, List<RiskFactor> risks)
    {
        var recs = new List<string>();
        bool hasHighRisk = risks.Exists(r => r.severity == "high");

        if (avg >= 40)
        {
            recs.Add("Consult an audiologist or ENT specialist for comprehensive evaluation");
            recs.Add("Consider hearing aid evaluation");
        }
        else if (avg >= 25)
        {
            recs.Add("Schedule follow-up hearing test in 6-12 months");
            recs.Add("Consult with an audiologist for professional assessment");
        }

        if (history.noiseExposure.occupational || history.noiseExposure.recreational)
        {
            recs.Add("Use hearing protection (earplugs/earmuffs) in loud environments");
        }
        if (history.medicalConditions.diabetes || history.medicalConditions.cardiovascular)
        {
            recs.Add("Manage underlying health conditions to protect hearing");
        }
        if (history.ototoxicMedications.current)
        {
            recs.Add("Discuss hearing monitoring plan with prescribing physician");
        }
        if (history.earHistory.suddenHearingLoss)
        {
            recs.Add("Seek immediate medical attention for any new sudden hearing changes");
        }
        if (history.symptoms.earPain || history.symptoms.earFullness)
        {
            recs.Add("Consult ENT specialist to rule out infection or blockage");
        }
        if (avg < 25 && !hasHighRisk)
        {
            recs.Add("Continue protecting your hearing from loud noise exposure");
            recs.Add("Have your hearing tested every 3-5 years");
        }
        return recs;
    }
}