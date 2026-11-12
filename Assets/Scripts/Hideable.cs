
using UnityEngine;

public class Hideable : MonoBehaviour
{
    public bool IsHidden { get; private set; }

    public void Hide()
    {
        IsHidden = true;
    }

    public void Reveal()
    {
        IsHidden = false;
    }
}
