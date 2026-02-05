using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VContainer;
using VContainer.Unity;
using System.Runtime.InteropServices;
using Unity.VisualScripting;

/// <summary>
/// Manages UI screens using dependency injection for instantiation.
/// Handles screen lifecycle, stack management, and Addressables loading.
/// </summary>
public class ScreenManager : MonoBehaviour, IScreenManager, IStartable
{
    [SerializeField] private Transform _screenContainer;
    [SerializeField] private ScreenStackManager _screenStack;

    private ScreenFactory _screenFactory;
    private ScreenPrefabProvider _prefabProvider;
    private Dictionary<ScreenType, ScreenPresenter> _activeScreens = new Dictionary<ScreenType, ScreenPresenter>();
    
    private bool _isLoaded;
    
    /// <summary>
    /// Returns true when all screen prefabs have been loaded from Addressables.
    /// </summary>
    public bool IsLoaded => _isLoaded;

    /// <summary>
    /// VContainer dependency injection constructor.
    /// </summary>
    /// <param name="screenFactory">Factory for creating screen instances</param>
    /// <param name="prefabProvider">Provider for loading screen prefabs</param>
    [Inject]
    public void Construct(ScreenFactory screenFactory, ScreenPrefabProvider prefabProvider)
    {
        _screenFactory = screenFactory;
        _prefabProvider = prefabProvider;
    }

    /// <summary>
    /// Ensures ScreenStackManager component exists.
    /// </summary>
    private void Awake()
    {
        if (_screenStack == null)
        {
            _screenStack = GetComponent<ScreenStackManager>();
            if (_screenStack == null)
            {
                _screenStack = gameObject.AddComponent<ScreenStackManager>();
            }
        }
    }

    /// <summary>
    /// VContainer lifecycle method. Loads all screen prefabs from Addressables asynchronously.
    /// </summary>
    async void IStartable.Start()
    {        
        try 
        {
            await _prefabProvider.LoadPrefabs();
            _isLoaded = true;
            // Debug.Log("[ScreenManager] Prefabs loaded successfully");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[ScreenManager] Error loading prefabs: {e}");
        }
    }

    /// <summary>
    /// Coroutine that waits until all screen prefabs are loaded.
    /// Used by GameManager to ensure UI is ready before transitioning to menu.
    /// </summary>
    /// <returns>Coroutine enumerator</returns>
    public IEnumerator WaitForScreens()
    {
        while (!_isLoaded)
        {
            yield return null;
        }
    }

    /// <summary>
    /// Shows a screen and pushes it onto the screen stack.
    /// Reuses existing screen instance if already created.
    /// </summary>
    /// <param name="screenName">Screen to display</param>
    public void ShowScreen(ScreenType screenName)
    {        
        // Reuse existing screen instance if already created
        if (_activeScreens.ContainsKey(screenName))
        {
            _activeScreens[screenName].Setup();
            _screenStack.PushScreen(_activeScreens[screenName]);
            return;
        }

        // Create new screen instance via factory
        ScreenPresenter presenter = _screenFactory.CreateScreen(screenName, _screenContainer);
        
        if (presenter == null)
        {
            Debug.LogError($"Failed to create screen: {screenName}");
            return;
        }

        _activeScreens[screenName] = presenter;
        presenter.Setup();
        _screenStack.PushScreen(presenter);
    }

    /// <summary>
    /// Hides the specified screen by popping it from the stack.
    /// </summary>
    /// <param name="screenName">Screen to hide</param>
    public void HideScreen(ScreenType screenName)
    {
        if (_activeScreens.TryGetValue(screenName, out ScreenPresenter controller))
        {
            _screenStack.PopScreen();
        }
        else
        {
            Debug.LogWarning($"Failed to HideScren, Attempted to hide inactive screen: {screenName}");
        }
    }

    /// <summary>
    /// Goes back to the previous screen by popping the current screen from the stack.
    /// </summary>
    public void GoBack()
    {
        if (_screenStack != null)
        {
            _screenStack.PopScreen();
        }
    }

    /// <summary>
    /// Clears the screen stack and destroys all active screen GameObjects.
    /// Used when transitioning to a completely different game state.
    /// </summary>
    public void HideAllScreens()
    {
        if (_screenStack != null)
        {
            _screenStack.ClearStack();
        }
        
        // Clean up active screens even if stack is null
        foreach (ScreenPresenter screen in _activeScreens.Values)
        {
            if (screen != null)
            {
                screen.Destroy();
            }
        }
        _activeScreens.Clear();
    }

    /// <summary>
    /// Cleanup when ScreenManager is destroyed. Unloads Addressables.
    /// </summary>
    private void OnDestroy()
    {
        if (this.enabled && this.gameObject.activeInHierarchy)
        {
            HideAllScreens();
            _prefabProvider.UnloadPrefabs();
        }
    }
}