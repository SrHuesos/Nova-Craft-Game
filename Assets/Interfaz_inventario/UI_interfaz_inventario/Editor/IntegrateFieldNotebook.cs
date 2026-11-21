using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class IntegrateFieldNotebook
{
    private const string PrefabPath =
        "Assets/Interfaz_inventario/UI_interfaz_inventario/Prefabs/UI/OSB_FieldNotebook.prefab";

    private static readonly string[] ScenePaths =
    {
        "Assets/Scenes/Humedal.unity",
        "Assets/Scenes/JardínOrquideas.unity"
    };

    [MenuItem("Tools/One Step Beyond/Integrate Field Notebook")]
    public static void Integrate()
    {
        GameObject prefab =
            AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);

        if (prefab == null)
        {
            Debug.LogError("No se encontró OSB_FieldNotebook.prefab.");
            return;
        }

        if (prefab.GetComponent<FieldNotebookUI>() == null)
        {
            Debug.LogError("El prefab no tiene FieldNotebookUI en la raíz.");
            return;
        }

        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return;

        Scene previousScene = SceneManager.GetActiveScene();

        foreach (string path in ScenePaths)
        {
            Scene scene = EditorSceneManager.OpenScene(
                path, OpenSceneMode.Additive);

            SceneManager.SetActiveScene(scene);

            bool hasNotebook = false;
            bool hasDiscovery = false;

            foreach (GameObject root in scene.GetRootGameObjects())
            {
                if (root.GetComponentInChildren<FieldNotebookUI>(true) != null)
                    hasNotebook = true;

                if (root.GetComponentInChildren<FieldDiscovery>(true) != null)
                    hasDiscovery = true;
            }

            if (!hasNotebook)
            {
                GameObject instance =
                    (GameObject)PrefabUtility.InstantiatePrefab(prefab, scene);

                if (instance == null)
                {
                    Debug.LogError("No se pudo integrar la libreta en " + path);
                    EditorSceneManager.CloseScene(scene, true);
                    continue;
                }

                instance.name = "OSB_FieldNotebook";
                Debug.Log("Libreta integrada en " + path);
            }
            else
            {
                Debug.Log("La escena ya contiene una libreta: " + path);
            }

            if (!hasDiscovery)
            {
                GameObject manager =
                    new GameObject("FieldDiscovery_Manager");

                SceneManager.MoveGameObjectToScene(manager, scene);
                manager.AddComponent<FieldDiscovery>();

                Debug.Log("FieldDiscovery_Manager creado en " + path);
            }
            else
            {
                Debug.Log("La escena ya contiene FieldDiscovery: " + path);
            }

            EditorSceneManager.MarkSceneDirty(scene);
            bool saved = EditorSceneManager.SaveScene(scene);

            Debug.Log(saved
                ? "ESCENA GUARDADA: " + path
                : "ERROR AL GUARDAR: " + path);

            EditorSceneManager.CloseScene(scene, true);
        }

        if (previousScene.IsValid() && previousScene.isLoaded)
            SceneManager.SetActiveScene(previousScene);

        Debug.Log("Integración de libreta finalizada.");
    }
}
