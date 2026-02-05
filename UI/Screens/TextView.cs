using UnityEngine;
using TMPro;

/// <summary>
/// Manages a TextMeshPro UI element with visibility control.
/// </summary>
[RequireComponent(typeof(TMP_Text))]
public class TextView : MonoBehaviour
{
    private TMP_Text _text;

    /// <summary>
    /// Gets or sets the text content.
    /// </summary>
    public string Text
    {
        get => _text.text;
        set => _text.SetText(value);
    }

    /// <summary>
    /// Initializes the text component.
    /// </summary>
    protected virtual void Awake()
    {
        _text = GetComponent<TMP_Text>();
    }

    /// <summary>
    /// Shows or hides the text by enabling/disabling the component.
    /// </summary>
    /// <param name="show">Whether to show or hide the text</param>
    public virtual void Show(bool show)
    {
        _text.enabled = show;
    }
}
