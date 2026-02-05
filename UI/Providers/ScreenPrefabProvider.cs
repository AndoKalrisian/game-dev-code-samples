using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;
using System;

/// <summary>
/// Loads screen prefabs from Addressables asynchronously.
/// Caches loaded prefabs for efficient reuse.
/// </summary>
public class ScreenPrefabProvider
{
    private readonly Dictionary<ScreenType, GameObject> _prefabs = new Dictionary<ScreenType, GameObject>();

    /// <summary>
    /// Loads all screen prefabs from Addressables using the "Screens/{ScreenName}" naming convention.
    /// Runs asynchronously to avoid blocking the main thread.
    /// </summary>
    public async Task LoadPrefabs()
    {
        foreach (ScreenType screenName in Enum.GetValues(typeof(ScreenType)))
        {
            if (screenName == ScreenType.None)
                continue;
            
            try
            {
                // Load prefab from Addressables using naming convention
                var loadOp = Addressables.LoadAssetAsync<GameObject>($"Screens/{screenName}");
                var prefab = await loadOp.Task;
                
                if (prefab != null)
                {
                    _prefabs[screenName] = prefab;
                }
                else
                {
                    Debug.LogError($"[ScreenPrefabProvider] Prefab loaded but was null for {screenName}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[ScreenPrefabProvider] Failed to load prefab for {screenName}: {e.Message}\nStack trace: {e.StackTrace}");
            }
        }
    }

    /// <summary>
    /// Retrieves a cached prefab for the specified screen.
    /// </summary>
    /// <param name="screenName">Screen to get prefab for</param>
    /// <returns>Prefab GameObject or null if not loaded</returns>
    public GameObject GetPrefab(ScreenType screenName)
    {
        if (_prefabs.TryGetValue(screenName, out var prefab))
        {
            return prefab;
        }
        Debug.LogError($"Prefab for {screenName} not loaded");
        return null;
    }

    /// <summary>
    /// Releases all loaded prefabs from Addressables memory.
    /// Called when ScreenManager is destroyed.
    /// </summary>
    public void UnloadPrefabs()
    {
        foreach (var prefab in _prefabs.Values)
        {
            Addressables.Release(prefab);
        }
        _prefabs.Clear();
    }
}