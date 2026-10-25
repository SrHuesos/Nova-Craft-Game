using System;
using System.Collections.Generic;
using UnityEngine;

public enum FieldDiscoveryCategory
{
    Especie,
    Ecosistema,
    Pista,
    Fotografia
}

[Serializable]
public class FieldDiscoveryEntry
{
    public string id;
    public string title;
    [TextArea(2, 5)] public string description;
    public FieldDiscoveryCategory category;

    public FieldDiscoveryEntry(
        string id,
        string title,
        string description,
        FieldDiscoveryCategory category)
    {
        this.id = id;
        this.title = title;
        this.description = description;
        this.category = category;
    }
}

public class FieldDiscovery : MonoBehaviour
{
    public static FieldDiscovery Instance { get; private set; }

    [SerializeField]
    private List<FieldDiscoveryEntry> discoveries =
        new List<FieldDiscoveryEntry>();

    public IReadOnlyList<FieldDiscoveryEntry> Discoveries
    {
        get { return discoveries; }
    }

    public event Action DiscoveriesChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning(
                "There is already a FieldDiscovery manager in the scene.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public bool AddDiscovery(
        string id,
        string title,
        string description,
        FieldDiscoveryCategory category)
    {
        if (string.IsNullOrWhiteSpace(id) ||
            string.IsNullOrWhiteSpace(title))
        {
            Debug.LogWarning(
                "A discovery needs a valid ID and title.");
            return false;
        }

        foreach (FieldDiscoveryEntry entry in discoveries)
        {
            if (entry.id == id)
            {
                Debug.Log("Already discovered: " + title);
                return false;
            }
        }

        FieldDiscoveryEntry newEntry =
            new FieldDiscoveryEntry(
                id, title, description, category);

        discoveries.Add(newEntry);

        Debug.Log("New field discovery: " + title);
        DiscoveriesChanged?.Invoke();

        return true;
    }
}
