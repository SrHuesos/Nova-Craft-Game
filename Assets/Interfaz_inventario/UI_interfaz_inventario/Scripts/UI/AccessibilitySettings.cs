using System;
using UnityEngine;

/// <summary>
/// Stores accessibility preferences for the One Step Beyond UI.
/// Preferences are saved locally and can be read by other UI systems.
/// </summary>
public class AccessibilitySettings : MonoBehaviour
{
    public static AccessibilitySettings Instance { get; private set; }

    public event Action SettingsChanged;

    public bool HighContrast { get; private set; }
    public bool AudioCuesEnabled { get; private set; }
    public bool SubtitlesEnabled { get; private set; }
    public float TextScale { get; private set; }

    private const string ContrastKey = "OSB_Accessibility_HighContrast";
    private const string AudioKey = "OSB_Accessibility_AudioCues";
    private const string SubtitlesKey = "OSB_Accessibility_Subtitles";
    private const string TextScaleKey = "OSB_Accessibility_TextScale";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        LoadSettings();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void ToggleHighContrast()
    {
        HighContrast = !HighContrast;
        SaveSettings();
    }

    public void ToggleAudioCues()
    {
        AudioCuesEnabled = !AudioCuesEnabled;
        SaveSettings();
    }

    public void ToggleSubtitles()
    {
        SubtitlesEnabled = !SubtitlesEnabled;
        SaveSettings();
    }

    public void IncreaseTextSize()
    {
        SetTextScale(TextScale + 0.1f);
    }

    public void DecreaseTextSize()
    {
        SetTextScale(TextScale - 0.1f);
    }

    public void ResetSettings()
    {
        HighContrast = false;
        AudioCuesEnabled = true;
        SubtitlesEnabled = true;
        TextScale = 1f;
        SaveSettings();
    }

    private void SetTextScale(float value)
    {
        TextScale = Mathf.Clamp(value, 0.8f, 1.5f);
        SaveSettings();
    }

    private void LoadSettings()
    {
        HighContrast = PlayerPrefs.GetInt(ContrastKey, 0) == 1;
        AudioCuesEnabled = PlayerPrefs.GetInt(AudioKey, 1) == 1;
        SubtitlesEnabled = PlayerPrefs.GetInt(SubtitlesKey, 1) == 1;
        TextScale = PlayerPrefs.GetFloat(TextScaleKey, 1f);

        TextScale = Mathf.Clamp(TextScale, 0.8f, 1.5f);
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetInt(ContrastKey, HighContrast ? 1 : 0);
        PlayerPrefs.SetInt(AudioKey, AudioCuesEnabled ? 1 : 0);
        PlayerPrefs.SetInt(SubtitlesKey, SubtitlesEnabled ? 1 : 0);
        PlayerPrefs.SetFloat(TextScaleKey, TextScale);
        PlayerPrefs.Save();

        SettingsChanged?.Invoke();
        Debug.Log("Accessibility preferences saved.");
    }
}
