using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.UI;

public static class AccessibilityMenuBuilder
{
    private const string OutputFolder = "Assets/_Project/Prefabs/UI";
    private const string OutputPath =
        OutputFolder + "/OSB_AccessibilityMenu.prefab";

    [MenuItem("Tools/One Step Beyond/Create Accessibility Menu Prefab")]
    public static void CreateAccessibilityMenuPrefab()
    {
        EnsureFolderExists();

        GameObject root = new GameObject(
            "OSB_AccessibilityMenu",
            typeof(RectTransform),
            typeof(AccessibilityMenuUI));

        RectTransform rootRect = root.GetComponent<RectTransform>();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;

        GameObject panel = CreateUIObject(
            "AccessibilityPanel", root.transform);

        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.sizeDelta = new Vector2(820, 780);

        Image background = panel.AddComponent<Image>();
        background.color = new Color(0.055f, 0.075f, 0.09f, 1f);

        Text title = CreateText(
            panel.transform, "ACCESIBILIDAD",
            new Vector2(0, 320), new Vector2(700, 65),
            36, FontStyle.Bold, Color.white, TextAnchor.MiddleCenter);

        Text description = CreateText(
            panel.transform,
            "Personaliza la experiencia de juego",
            new Vector2(0, 275), new Vector2(700, 40),
            22, FontStyle.Normal, Color.white, TextAnchor.MiddleCenter);

        Button contrastButton = CreateButton(
            panel.transform, "Alto contraste: Desactivado",
            new Vector2(0, 190));

        Button audioButton = CreateButton(
            panel.transform, "Señales sonoras: Activadas",
            new Vector2(0, 110));

        Button subtitlesButton = CreateButton(
            panel.transform, "Subtítulos: Activados",
            new Vector2(0, 30));

        Text textSizeLabel = CreateText(
            panel.transform, "Tamaño del texto",
            new Vector2(0, -55), new Vector2(500, 40),
            23, FontStyle.Bold, Color.white, TextAnchor.MiddleCenter);

        Button decreaseButton = CreateButton(
            panel.transform, "A−",
            new Vector2(-115, -115), new Vector2(100, 58));

        Text textSizeValue = CreateText(
            panel.transform, "100%",
            new Vector2(0, -115), new Vector2(130, 58),
            24, FontStyle.Bold, Color.white, TextAnchor.MiddleCenter);

        Button increaseButton = CreateButton(
            panel.transform, "A+",
            new Vector2(115, -115), new Vector2(100, 58));

        Button resetButton = CreateButton(
            panel.transform, "Restaurar valores predeterminados",
            new Vector2(0, -205));

        Text status = CreateText(
            panel.transform, "Configura tus preferencias.",
            new Vector2(0, -270), new Vector2(700, 38),
            18, FontStyle.Normal, Color.white, TextAnchor.MiddleCenter);

        Button closeButton = CreateButton(
            panel.transform, "Cerrar",
            new Vector2(0, -335));

        AccessibilityMenuUI controller =
            root.GetComponent<AccessibilityMenuUI>();

        UnityEventTools.AddPersistentListener(
            contrastButton.onClick, controller.ToggleHighContrast);

        UnityEventTools.AddPersistentListener(
            audioButton.onClick, controller.ToggleAudioCues);

        UnityEventTools.AddPersistentListener(
            subtitlesButton.onClick, controller.ToggleSubtitles);

        UnityEventTools.AddPersistentListener(
            decreaseButton.onClick, controller.DecreaseTextSize);

        UnityEventTools.AddPersistentListener(
            increaseButton.onClick, controller.IncreaseTextSize);

        UnityEventTools.AddPersistentListener(
            resetButton.onClick, controller.ResetPreferences);

        UnityEventTools.AddPersistentListener(
            closeButton.onClick, controller.CloseMenu);

        SerializedObject so = new SerializedObject(controller);

        so.FindProperty("menuPanel").objectReferenceValue = panel;
        so.FindProperty("panelBackground").objectReferenceValue = background;
        so.FindProperty("contrastButtonText").objectReferenceValue =
            contrastButton.GetComponentInChildren<Text>();
        so.FindProperty("audioButtonText").objectReferenceValue =
            audioButton.GetComponentInChildren<Text>();
        so.FindProperty("subtitlesButtonText").objectReferenceValue =
            subtitlesButton.GetComponentInChildren<Text>();
        so.FindProperty("textSizeValueText").objectReferenceValue = textSizeValue;
        so.FindProperty("statusText").objectReferenceValue = status;
        so.FindProperty("firstButton").objectReferenceValue = contrastButton;

        Text[] scalableTexts =
        {
            title, description,
            contrastButton.GetComponentInChildren<Text>(),
            audioButton.GetComponentInChildren<Text>(),
            subtitlesButton.GetComponentInChildren<Text>(),
            textSizeLabel, textSizeValue,
            decreaseButton.GetComponentInChildren<Text>(),
            increaseButton.GetComponentInChildren<Text>(),
            resetButton.GetComponentInChildren<Text>(),
            status, closeButton.GetComponentInChildren<Text>()
        };

        SerializedProperty textsProperty =
            so.FindProperty("scalableTexts");

        textsProperty.arraySize = scalableTexts.Length;

        for (int i = 0; i < scalableTexts.Length; i++)
        {
            textsProperty.GetArrayElementAtIndex(i).objectReferenceValue =
                scalableTexts[i];
        }

        so.ApplyModifiedPropertiesWithoutUndo();

        // Keep the root active so a main-menu button can call OpenMenu.
        // Only the visible panel starts hidden.
        panel.SetActive(false);

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, OutputPath);

        Object.DestroyImmediate(root);

        if (prefab != null)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Selection.activeObject = prefab;
            Debug.Log("Accessibility prefab created: " + OutputPath);
        }
        else
        {
            Debug.LogError("Failed to create the accessibility menu prefab.");
        }
    }

    private static void EnsureFolderExists()
    {
        if (!AssetDatabase.IsValidFolder("Assets/_Project"))
        {
            AssetDatabase.CreateFolder("Assets", "_Project");
        }

        if (!AssetDatabase.IsValidFolder("Assets/_Project/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets/_Project", "Prefabs");
        }

        if (!AssetDatabase.IsValidFolder(OutputFolder))
        {
            AssetDatabase.CreateFolder("Assets/_Project/Prefabs", "UI");
        }
    }

    private static GameObject CreateUIObject(string objectName, Transform parent)
    {
        GameObject obj = new GameObject(
            objectName, typeof(RectTransform));

        obj.transform.SetParent(parent, false);
        return obj;
    }

    private static Text CreateText(
        Transform parent, string value, Vector2 position, Vector2 size,
        int fontSize, FontStyle style, Color color, TextAnchor alignment)
    {
        GameObject obj = CreateUIObject("Text_" + value.Replace(" ", "_"), parent);

        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        Text text = obj.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.text = value;
        text.fontSize = fontSize;
        text.fontStyle = style;
        text.color = color;
        text.alignment = alignment;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.raycastTarget = false;

        return text;
    }

    private static Button CreateButton(
        Transform parent, string label, Vector2 position,
        Vector2? size = null)
    {
        GameObject obj = CreateUIObject("Button_" + label, parent);

        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size ?? new Vector2(580, 62);

        Image image = obj.AddComponent<Image>();
        image.color = new Color(0.12f, 0.28f, 0.32f, 1f);

        Button button = obj.AddComponent<Button>();
        button.targetGraphic = image;

        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.12f, 0.28f, 0.32f, 1f);
        colors.highlightedColor = new Color(0.22f, 0.42f, 0.46f, 1f);
        colors.pressedColor = new Color(0.08f, 0.18f, 0.22f, 1f);
        colors.selectedColor = colors.highlightedColor;
        button.colors = colors;

        CreateText(
            obj.transform, label, Vector2.zero,
            new Vector2(rect.sizeDelta.x - 20, rect.sizeDelta.y - 8),
            21, FontStyle.Bold, Color.white, TextAnchor.MiddleCenter);

        return button;
    }
}
