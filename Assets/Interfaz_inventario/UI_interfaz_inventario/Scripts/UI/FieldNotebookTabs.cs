
using UnityEngine;

/// <summary>
/// Controls the tabs inside the field notebook.
/// Keeps discovery content separate from accessibility settings.
/// </summary>
public class FieldNotebookTabs : MonoBehaviour
{
    [Header("Notebook sections")]
    [SerializeField] private GameObject discoveriesSection;
    [SerializeField] private GameObject accessibilitySection;

    [Header("Accessibility menu")]
    [SerializeField] private AccessibilityMenuUI accessibilityMenu;

    private void Start()
    {
        ShowDiscoveries();
    }

    public void ShowDiscoveries()
    {
        if (discoveriesSection != null)
            discoveriesSection.SetActive(true);

        if (accessibilityMenu != null)
            accessibilityMenu.CloseMenu();

        if (accessibilitySection != null)
            accessibilitySection.SetActive(false);
    }

    public void ShowAccessibility()
    {
        if (discoveriesSection != null)
            discoveriesSection.SetActive(false);

        if (accessibilitySection != null)
            accessibilitySection.SetActive(true);

        if (accessibilityMenu != null)
            accessibilityMenu.OpenMenu();
    }
}
