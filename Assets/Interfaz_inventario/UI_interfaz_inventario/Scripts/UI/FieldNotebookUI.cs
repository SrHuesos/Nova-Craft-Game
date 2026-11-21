
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class FieldNotebookUI : MonoBehaviour
{
    [Header("Notebook")]
    [SerializeField] private GameObject notebookPanel;

    [Header("Accessibility")]
    [SerializeField] private Selectable firstSelectedControl;

    public bool IsOpen { get; private set; }

    private void Start()
    {
        SetNotebookOpen(false);
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.bKey.wasPressedThisFrame)
        {
            if (InputFieldIsFocused())
                return;

            SetNotebookOpen(!IsOpen);
        }

        if (IsOpen && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            SetNotebookOpen(false);
        }
    }

    public void SetNotebookOpen(bool open)
    {
        IsOpen = open;

        if (notebookPanel != null)
            notebookPanel.SetActive(open);

        if (open && firstSelectedControl != null &&
            EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(
                firstSelectedControl.gameObject
            );
        }
        else if (!open && EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        Debug.Log(open
            ? "Field notebook opened."
            : "Field notebook closed.");
    }

    private bool InputFieldIsFocused()
    {
        if (EventSystem.current == null)
            return false;

        GameObject selected =
            EventSystem.current.currentSelectedGameObject;

        if (selected == null)
            return false;

        return selected.GetComponent<InputField>() != null;
    }
}
