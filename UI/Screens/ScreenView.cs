using UnityEngine;
using UnityEngine.UI;

public abstract class ScreenView : MonoBehaviour
{
    protected Canvas Canvas;
    protected CanvasScaler CanvasScaler;
    protected GraphicRaycaster GraphicRaycaster;

    [SerializeField] private bool _isVisible = true;
    [SerializeField] private bool _isInteractable = true;

    protected virtual void Awake()
    {
        Canvas = GetComponent<Canvas>();
        CanvasScaler = GetComponent<CanvasScaler>();
        GraphicRaycaster = GetComponent<GraphicRaycaster>();
    }

    protected virtual void Update()
    {
        if (!_isVisible) return;

        OnVisibleUpdate();
    }

    protected virtual void OnVisibleUpdate()
    {
        // Child classes override this if they want to do per-frame logic
    }

    /// <summary>
    /// Whether the view is currently visible.
    /// </summary>
    public bool IsVisible 
    {
        get => _isVisible;
        protected set => _isVisible = value;
    }

    /// <summary>
    /// Whether the view can receive user input.
    /// </summary>
    public bool IsInteractable
    {
        get => _isInteractable;
        protected set => _isInteractable = value;
    }

    /// <summary>
    /// Shows or hides the screen and its UI components.
    /// </summary>
    public virtual void Show(bool show)
    {
        if (IsVisible == show) return; // Prevent redundant calls
        
        IsVisible = show;
        
        Canvas?.gameObject.SetActive(show);
        if (CanvasScaler != null) CanvasScaler.enabled = show;
        if (GraphicRaycaster != null) GraphicRaycaster.enabled = show;
    }
}