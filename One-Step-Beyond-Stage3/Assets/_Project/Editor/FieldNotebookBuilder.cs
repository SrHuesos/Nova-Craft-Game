using System.IO;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public static class FieldNotebookBuilder
{
    private const string OutputFolder = "Assets/_Project/Scenes";
    private const string OutputScene = OutputFolder + "/FieldNotebookSandbox.unity";

    [MenuItem("Tools/One Step Beyond/Create Notebook Sandbox")]
    public static void CreateNotebookSandbox()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return;

        Directory.CreateDirectory(OutputFolder);

        Scene scene = EditorSceneManager.NewScene(
            NewSceneSetup.EmptyScene, NewSceneMode.Single);

        SceneManager.SetActiveScene(scene);

        GameObject manager = new GameObject("FieldDiscovery_Manager");
        manager.AddComponent<FieldDiscovery>();

        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<StandaloneInputModule>();

        GameObject canvasObject = new GameObject(
            "OSB_UIUX_Root",
            typeof(RectTransform),
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster),
            typeof(FieldNotebookUI));

        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        GameObject panel = CreateUIObject("NotebookPanel", canvasObject.transform);
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.sizeDelta = new Vector2(1200, 800);

        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0.055f, 0.075f, 0.09f, 0.98f);

        CreateText(panel.transform, "LIBRETA DE CAMPO",
            new Vector2(0, 325), new Vector2(1050, 70),
            42, FontStyle.Bold, Color.white, TextAnchor.MiddleCenter);

        CreateText(panel.transform, "Consulta tus descubrimientos",
            new Vector2(0, 275), new Vector2(1000, 40),
            23, FontStyle.Normal, new Color(0.85f, 0.9f, 0.92f),
            TextAnchor.MiddleCenter);

        FieldNotebookContent content = panel.AddComponent<FieldNotebookContent>();

        Button allButton = CreateButton(panel.transform, "Todos", new Vector2(-450, 210));
        Button speciesButton = CreateButton(panel.transform, "Especies", new Vector2(-225, 210));
        Button ecosystemsButton = CreateButton(panel.transform, "Ecosistemas", new Vector2(0, 210));
        Button cluesButton = CreateButton(panel.transform, "Pistas", new Vector2(225, 210));
        Button photosButton = CreateButton(panel.transform, "Fotografías", new Vector2(450, 210));

        UnityEventTools.AddPersistentListener(allButton.onClick, content.ShowAll);
        UnityEventTools.AddPersistentListener(speciesButton.onClick, content.ShowSpecies);
        UnityEventTools.AddPersistentListener(ecosystemsButton.onClick, content.ShowEcosystems);
        UnityEventTools.AddPersistentListener(cluesButton.onClick, content.ShowClues);
        UnityEventTools.AddPersistentListener(photosButton.onClick, content.ShowPhotographs);

        Text categoryTitle = CreateText(panel.transform, "Todos los descubrimientos",
            new Vector2(0, 140), new Vector2(1000, 55),
            30, FontStyle.Bold, Color.white, TextAnchor.MiddleCenter);

        Text listText = CreateText(panel.transform, 
            "Aún no hay descubrimientos.\nExplora el entorno y utiliza F para interactuar.",
            new Vector2(0, -20), new Vector2(1000, 340),
            24, FontStyle.Normal, Color.white, TextAnchor.UpperLeft);

        listText.horizontalOverflow = HorizontalWrapMode.Wrap;
        listText.verticalOverflow = VerticalWrapMode.Overflow;

        Text statusText = CreateText(panel.transform, "0 descubrimientos registrados.",
            new Vector2(0, -345), new Vector2(1000, 45),
            20, FontStyle.Bold, new Color(0.75f, 0.9f, 0.8f),
            TextAnchor.MiddleCenter);

        SerializedObject contentSO = new SerializedObject(content);
        contentSO.FindProperty("categoryTitle").objectReferenceValue = categoryTitle;
        contentSO.FindProperty("discoveryListText").objectReferenceValue = listText;
        contentSO.FindProperty("statusText").objectReferenceValue = statusText;
        contentSO.FindProperty("firstCategoryButton").objectReferenceValue = allButton;
        contentSO.ApplyModifiedPropertiesWithoutUndo();

        SerializedObject notebookSO = new SerializedObject(
            canvasObject.GetComponent<FieldNotebookUI>());
        notebookSO.FindProperty("notebookPanel").objectReferenceValue = panel;
        notebookSO.FindProperty("firstSelectedControl").objectReferenceValue = allButton;
        notebookSO.ApplyModifiedPropertiesWithoutUndo();

        panel.SetActive(false);

        EditorSceneManager.MarkSceneDirty(scene);
        bool saved = EditorSceneManager.SaveScene(scene, OutputScene);

        Selection.activeGameObject = canvasObject;

        if (saved)
            Debug.Log("Notebook sandbox created at " + OutputScene);
        else
            Debug.LogError("Could not save the notebook sandbox scene.");
    }

    private static GameObject CreateUIObject(string objectName, Transform parent)
    {
        GameObject obj = new GameObject(
            objectName, typeof(RectTransform));
        obj.transform.SetParent(parent, false);

        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;

        return obj;
    }

    private static Text CreateText(
        Transform parent, string value, Vector2 position, Vector2 size,
        int fontSize, FontStyle style, Color color, TextAnchor alignment)
    {
        GameObject obj = CreateUIObject("Text_" + value.Replace(" ", "_"), parent);
        RectTransform rect = obj.GetComponent<RectTransform>();
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

    private static Button CreateButton(Transform parent, string label, Vector2 position)
    {
        GameObject obj = CreateUIObject("Button_" + label, parent);
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(205, 64);

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

        CreateText(obj.transform, label, Vector2.zero, new Vector2(190, 54),
            20, FontStyle.Bold, Color.white, TextAnchor.MiddleCenter);

        return button;
    }
}


