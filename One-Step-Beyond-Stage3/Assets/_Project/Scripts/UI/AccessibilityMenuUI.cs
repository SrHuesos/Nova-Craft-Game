using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Visual controller for the accessibility menu.
/// Designed to be reusable from the game's main menu.
/// </summary>
public class AccessibilityMenuUI : MonoBehaviour
{
    [Header("Menu")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private Image panelBackground;
    [SerializeField] private Text contrastButtonText;
    [SerializeField] private Text audioButtonText;
    [SerializeField] private Text subtitlesButtonText;
    [SerializeField] private Text textSizeValueText;
    [SerializeField] private Text statusText;
    [SerializeField] private Text[] scalableTexts;

    [Header("Optional focus")]
    [SerializeField] private Button firstButton;

    private AccessibilitySettings settings;
    private readonly Dictionary<Text, int> originalFontSizes =
        new Dictionary<Text, int>();

    private void Awake()
    {
        CacheOriginalFontSizes();
        EnsureSettingsExist();
    }

    private void OnEnable()
    {
        EnsureSettingsExist();

        if (settings != null)
            settings.SettingsChanged += RefreshMenu;

        RefreshMenu();
    }

    private void OnDisable()
    {
        if (settings != null)
            settings.SettingsChanged -= RefreshMenu;
    }

    private void EnsureSettingsExist()
    {
        settings = AccessibilitySettings.Instance;

        if (settings == null)
        {
            GameObject settingsObject =
                new GameObject("AccessibilitySettings_Runtime");

            settings = settingsObject.AddComponent<AccessibilitySettings>();
        }
    }

    private void CacheOriginalFontSizes()
    {
        if (scalableTexts == null)
            return;

        foreach (Text label in scalableTexts)
        {
            if (label != null && !originalFontSizes.ContainsKey(label))
                originalFontSizes.Add(label, label.fontSize);
        }
    }

    public void OpenMenu()
    {
        if (menuPanel != null)
            menuPanel.SetActive(true);

        RefreshMenu();
    }

    public void CloseMenu()
    {
        if (menuPanel != null)
            menuPanel.SetActive(false);
    }

    public void ToggleHighContrast()
    {
        if (settings == null)
            return;

        settings.ToggleHighContrast();
        RefreshMenu();
    }

    public void ToggleAudioCues()
    {
        if (settings == null)
            return;

        settings.ToggleAudioCues();
        RefreshMenu();
    }

    public void ToggleSubtitles()
    {
        if (settings == null)
            return;

        settings.ToggleSubtitles();
        RefreshMenu();
    }

    public void IncreaseTextSize()
    {
        if (settings == null)
            return;

        settings.IncreaseTextSize();
        RefreshMenu();
    }

    public void DecreaseTextSize()
    {
        if (settings == null)
            return;

        settings.DecreaseTextSize();
        RefreshMenu();
    }

    public void ResetPreferences()
    {
        if (settings == null)
            return;

        settings.ResetSettings();
        RefreshMenu();
    }

    public void RefreshMenu()
    {
        if (settings == null)
            return;

        if (contrastButtonText != null)
            contrastButtonText.text = settings.HighContrast
                ? "Alto contraste: Activado"
                : "Alto contraste: Desactivado";

        if (audioButtonText != null)
            audioButtonText.text = settings.AudioCuesEnabled
                ? "Señales sonoras: Activadas"
                : "Señales sonoras: Desactivadas";

        if (subtitlesButtonText != null)
            subtitlesButtonText.text = settings.SubtitlesEnabled
                ? "Subtítulos: Activados"
                : "Subtítulos: Desactivados";

        if (textSizeValueText != null)
            textSizeValueText.text =
                Mathf.RoundToInt(settings.TextScale * 100f) + "%";

        ApplyTextScale();
        ApplyContrast();

        if (statusText != null)
            statusText.text = "Preferencias actualizadas.";
    }

    private void ApplyTextScale()
    {
        foreach (KeyValuePair<Text, int> item in originalFontSizes)
        {
            if (item.Key != null)
            {
                item.Key.fontSize = Mathf.RoundToInt(
                    item.Value * settings.TextScale);
            }
        }
    }

    private void ApplyContrast()
    {
        Color backgroundColor = settings.HighContrast
            ? Color.black
            : new Color(0.055f, 0.075f, 0.09f, 0.98f);

        Color textColor = settings.HighContrast
            ? Color.white
            : new Color(0.9f, 0.92f, 0.94f, 1f);

        if (panelBackground != null)
            panelBackground.color = backgroundColor;

        if (scalableTexts != null)
        {
            foreach (Text label in scalableTexts)
            {
                if (label != null)
                    label.color = textColor;
            }
        }
    }
}
