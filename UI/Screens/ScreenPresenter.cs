using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.Profiling;

/// <summary>
/// Base class for all screen controllers in the UI system.
/// Handles basic screen lifecycle and UI component management.
/// </summary>
public abstract class ScreenPresenter
{
    public ScreenView View { get; set; }
    
    public bool IsVisible { get; private set; }
    [NonSerialized] public ScreenType ScreenName;

    /// <summary>
    /// Called when the screen is first instantiated.
    /// </summary>
    public virtual void OnInstantiated()
    {

    }
    
    /// <summary>
    /// Sets up the screen for use. Call this when the screen is first shown.
    /// </summary>
    public virtual void Setup()
    {
        
    }

    /// <summary>
    /// Cleans up the screen before it's destroyed or hidden.
    /// </summary>
    public virtual void Cleanup()
    {
        Show(false);
    }

    /// <summary>
    /// Shows or hides the screen and its UI components.
    /// </summary>
    public virtual void Show(bool show)
    {
        if (IsVisible == show) return; // Prevent redundant calls
        
        IsVisible = show;
        View.Show(show);
    }

    /// <summary>
    /// Destroys the screen and releases resources.
    /// </summary>
    public virtual void Destroy()
    {
        
    }

}

