using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FieldNotebookContent : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Text categoryTitle;
    [SerializeField] private Text discoveryListText;
    [SerializeField] private Text statusText;

    [Header("Accessibility")]
    [SerializeField] private Button firstCategoryButton;

    private FieldDiscoveryCategory? selectedCategory;
    private FieldDiscovery subscribedManager;

    private void OnEnable()
    {
        SubscribeToDiscoveries();
        RefreshContent();
    }

    private void OnDisable()
    {
        UnsubscribeFromDiscoveries();
    }

    private void SubscribeToDiscoveries()
    {
        FieldDiscovery manager = FieldDiscovery.Instance;

        if (manager == null || subscribedManager == manager)
            return;

        UnsubscribeFromDiscoveries();
        subscribedManager = manager;
        subscribedManager.DiscoveriesChanged += RefreshContent;
    }

    private void UnsubscribeFromDiscoveries()
    {
        if (subscribedManager != null)
            subscribedManager.DiscoveriesChanged -= RefreshContent;

        subscribedManager = null;
    }

    public void ShowAll()
    {
        selectedCategory = null;
        RefreshContent();
    }

    public void ShowSpecies()
    {
        ShowCategory(FieldDiscoveryCategory.Especie);
    }

    public void ShowEcosystems()
    {
        ShowCategory(FieldDiscoveryCategory.Ecosistema);
    }

    public void ShowClues()
    {
        ShowCategory(FieldDiscoveryCategory.Pista);
    }

    public void ShowPhotographs()
    {
        ShowCategory(FieldDiscoveryCategory.Fotografia);
    }

    private void ShowCategory(FieldDiscoveryCategory category)
    {
        selectedCategory = category;
        RefreshContent();
    }

    public void RefreshContent()
    {
        if (categoryTitle == null || discoveryListText == null)
            return;

        categoryTitle.text = selectedCategory.HasValue
            ? GetCategoryName(selectedCategory.Value)
            : "Todos los descubrimientos";

        FieldDiscovery manager = FieldDiscovery.Instance;

        if (manager == null || manager.Discoveries.Count == 0)
        {
            discoveryListText.text =
                "Aún no hay descubrimientos.\n" +
                "Explora el entorno y utiliza F para interactuar.";

            if (statusText != null)
                statusText.text = "0 descubrimientos registrados.";

            return;
        }

        List<string> lines = new List<string>();
        int total = 0;

        foreach (FieldDiscoveryEntry entry in manager.Discoveries)
        {
            if (selectedCategory.HasValue &&
                entry.category != selectedCategory.Value)
                continue;

            total++;

            lines.Add(
                entry.title + "\n" +
                entry.description + "\n" +
                "Categoría: " + GetCategoryName(entry.category));
        }

        discoveryListText.text = total == 0
            ? "Todavía no hay elementos en esta categoría."
            : string.Join("\n\n--------------------\n\n", lines);

        if (statusText != null)
        {
            statusText.text = total == 1
                ? "1 descubrimiento mostrado."
                : total + " descubrimientos mostrados.";
        }
    }

    private string GetCategoryName(FieldDiscoveryCategory category)
    {
        switch (category)
        {
            case FieldDiscoveryCategory.Especie:
                return "Especies";
            case FieldDiscoveryCategory.Ecosistema:
                return "Ecosistemas";
            case FieldDiscoveryCategory.Pista:
                return "Pistas";
            case FieldDiscoveryCategory.Fotografia:
                return "Fotografías";
            default:
                return "Otros";
        }
    }
}
