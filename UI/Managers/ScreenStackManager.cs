using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages a stack of UI screens for navigation history.
/// Handles push/pop operations and ensures only the top screen is visible.
/// </summary>
public class ScreenStackManager : MonoBehaviour
{
    private readonly Stack<ScreenPresenter> _screenStack = new Stack<ScreenPresenter>();
    
    /// <summary>
    /// Returns the ScreenName of the currently visible screen (top of stack).
    /// Returns None if stack is empty.
    /// </summary>
    public ScreenType CurrentScreenName
    {
        get
        {
            if (_screenStack.Count > 0)
            {
                return _screenStack.Peek().ScreenName;
            }
            else
            {
                Debug.LogWarning("ScreenStackManager: No screens in stack, returning default ScreenName.MainMenu");
                return ScreenType.None;
            }
        }
    }

    /// <summary>
    /// Pushes a new screen onto the stack and shows it.
    /// Hides the previously visible screen.
    /// </summary>
    /// <param name="screen">Screen to push onto stack</param>
    public void PushScreen(ScreenPresenter screen)
    {
        if (_screenStack.Count > 0)
        {
            ScreenPresenter currentScreen = _screenStack.Peek();
            currentScreen.Show(false);
        }

        _screenStack.Push(screen);
        screen.Show(true);
    }

    /// <summary>
    /// Pops the current screen from the stack and shows the previous screen.
    /// </summary>
    public void PopScreen()
    {
        if (_screenStack.Count > 0)
        {
            ScreenPresenter currentScreen = _screenStack.Pop();
            currentScreen.Show(false);

            if (_screenStack.Count > 0)
            {
                ScreenPresenter previousScreen = _screenStack.Peek();
                previousScreen.Show(true);
            }
        }
    }

    /// <summary>
    /// Pops all screens except the root (bottom) screen and shows it.
    /// Used to return to main menu from nested screens.
    /// </summary>
    public void PopToRoot()
    {
        while (_screenStack.Count > 1)
        {
            ScreenPresenter screen = _screenStack.Pop();
            screen.Show(false);
        }

        if (_screenStack.Count > 0)
        {
            ScreenPresenter rootScreen = _screenStack.Peek();
            rootScreen.Show(true);
        }
    }

    /// <summary>
    /// Removes all screens from the stack and hides them.
    /// Used when transitioning to a completely new screen context.
    /// </summary>
    public void ClearStack()
    {
        while (_screenStack.Count > 0)
        {
            ScreenPresenter screen = _screenStack.Pop();
            screen.Show(false);
        }
    }
}