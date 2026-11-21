using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public static class CreateFieldNotebookPrefab
{
    private const string ScenePath =
        "Assets/Interfaz_inventario/UI_interfaz_inventario/Scenes/FieldNotebookSandbox.unity";

    private const string PrefabPath =
        "Assets/Interfaz_inventario/UI_interfaz_inventario/Prefabs/UI/OSB_FieldNotebook.prefab";

    [MenuItem("Tools/One Step Beyond/Create Field Notebook Prefab")]
    public static void Create()
    {
        if (AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath) != null)
        {
            Debug.LogError("El prefab ya existe. No se sobrescribió.");
            return;
        }

        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return;

        Scene previousScene = SceneManager.GetActiveScene();
        Scene sandbox = EditorSceneManager.OpenScene(
            ScenePath, OpenSceneMode.Additive);

        GameObject root = null;

        foreach (GameObject item in sandbox.GetRootGameObjects())
        {
            if (item.name == "OSB_UIUX_Root")
            {
                root = item;
                break;
            }
        }

        if (root == null)
        {
            Debug.LogError("No se encontró OSB_UIUX_Root.");
            EditorSceneManager.CloseScene(sandbox, true);
            return;
        }

        Transform panel = root.transform.Find("NotebookPanel");

        if (panel == null)
        {
            Debug.LogError("No se encontró NotebookPanel.");
            EditorSceneManager.CloseScene(sandbox, true);
            return;
        }

        bool wasActive = panel.gameObject.activeSelf;
        panel.gameObject.SetActive(false);

        Directory.CreateDirectory(
            "Assets/Interfaz_inventario/UI_interfaz_inventario/Prefabs/UI");

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);

        panel.gameObject.SetActive(wasActive);

        if (prefab != null)
            Debug.Log("PREFAB CREADO: " + PrefabPath);
        else
            Debug.LogError("No se pudo crear el prefab.");

        EditorSceneManager.CloseScene(sandbox, true);

        if (previousScene.IsValid() && previousScene.isLoaded)
            SceneManager.SetActiveScene(previousScene);
    }
}
