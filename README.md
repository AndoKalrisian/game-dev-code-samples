# Game Development Code Samples

A collection of production-quality C# code samples from a hobby game project.

## Project Overview

This repository showcases the core systems from a game built with **Unity**, featuring:
- **Modular Architecture** with clear separation of concerns
- **Dependency Injection** using VContainer for loose coupling
- **Model-View-Presenter (MVP)** pattern for UI systems
- **Scalable Service Layer** for business logic
- **Reactive State Management** with event-driven updates
- **Comprehensive Documentation** with XML comments throughout

## Key Features

### 🏗️ Architecture Highlights

#### Dependency Injection (VContainer)
- Centralized configuration in `GameLifetimeScope`
- Singleton services for game state management
- Automatic constructor injection for clean code
- Lifecycle management for game systems

#### Data Models & State Management
- **Configuration Layer**: Immutable `Config` classes (ScriptableObjects)
- **Runtime State Layer**: Mutable state objects tracking gameplay
- **Clear Separation**: Configs define rules, State tracks progress
- Examples: `CurrencyConfig`/`CurrencyState`, `PlayerConfig`/`PlayerState`

#### Services Layer
**CurrencyService** - Currency transaction management
- Stateless transaction processing
- Affordability validation
- Event-driven notifications
- Supports multiple currency types

**GameplayModifierService** - Dynamic power-up system
- Random modifier selection with repetition prevention
- Stat modifier application and tracking
- Session-persistent modifier history
- Smart allocation strategies for variety

**UpgradesService** - Progression system
- Upgrade purchase and activation
- Stat calculation and application
- Persistent upgrade state management

#### UI System (MVP Pattern)
**Base Components**:
- `ScreenPresenter` - Base controller for all screens
- `ScreenView` - Base view for UI rendering
- `ScreenManager` - Screen lifecycle and visibility management
- `ScreenStackManager` - Navigation and back-stack handling

**Example Screens**:
- `HUDPresenter`/`HUDView` - In-game heads-up display
- `GameOverPresenter`/`GameOverView` - Session end screen
- `CurrencyBarPresenter`/`CurrencyBarView` - UI component example

**Key Features**:
- Presenter handles logic, View handles rendering
- Prefab-based screen instantiation via `ScreenPrefabProvider`
- Stack-based navigation with back button support
- Component reusability (e.g., `CurrencyBar` used across screens)

## Project Structure

```
Scripts/
├── DataModels/              # Config and state classes
│   ├── *Config.cs          # Immutable configuration (ScriptableObjects)
│   └── *State.cs           # Mutable runtime state
│
├── Services/                # Business logic layer
│   ├── CurrencyService.cs
│   ├── GameplayModifierService.cs
│   └── UpgradesService.cs
│
└── UI/                      # User interface (MVP pattern)
    ├── Managers/           # Screen management and navigation
    ├── Providers/          # Factory and asset loading
    └── Screens/            # Screen controllers and views
        ├── Components/     # Reusable UI components
        ├── GameOver/       # Game over screen
        └── HUD/            # In-game display
```

## Code Quality & Patterns

### Best Practices Demonstrated

✅ **SOLID Principles**
- Single Responsibility: Each class has one reason to change
- Dependency Inversion: Dependencies injected via DI container
- Interface Segregation: Focused, minimal interfaces

✅ **Design Patterns**
- Dependency Injection for decoupling
- Service Locator pattern (via VContainer)
- MVP for UI testability
- State pattern in services
- Factory pattern in `ScreenPrefabProvider`

✅ **Documentation**
- XML documentation comments on public members
- Clear method descriptions with parameters
- Architecture patterns explained inline

✅ **Code Organization**
- Logical folder structure by responsibility
- Consistent naming conventions
- Reusable base classes for common functionality
- Separation of concerns across layers

### Example: Currency System

```csharp
// Service layer (stateless business logic)
public class CurrencyService
{
    public event Action<CurrencyType, int, int> OnCurrencyTransaction;
    
    public bool CanAfford(int cost, CurrencyType type) { ... }
    public void Spend(int amount, CurrencyType type) { ... }
    public void Earn(int amount, CurrencyType type) { ... }
}

// State layer (reactive state)
public class CurrencyState
{
    public int GetBalance(CurrencyType type) { ... }
    public void AdjustBalance(CurrencyType type, int amount) { ... }
}

// UI presentation layer
public class CurrencyBarPresenter : MonoBehaviour
{
    // Subscribes to service events and updates view
    private void OnCurrencyChanged(CurrencyType type, int amount, int newTotal)
    {
        _view.UpdateDisplay(type, newTotal);
    }
}
```

### Example: MVP Screen Pattern

```csharp
public class GameOverPresenter : ScreenPresenter
{
    private GameOverView _view;
    private GameSessionState _sessionState;
    
    public override void Setup()
    {
        _view.OnRestartClicked += HandleRestartClicked;
        _view.OnMenuClicked += HandleMenuClicked;
    }
    
    private void HandleRestartClicked()
    {
        // Handle logic in presenter, delegate rendering to view
    }
}

public class GameOverView : ScreenView
{
    public event Action OnRestartClicked;
    
    // Only handles UI rendering and input forwarding
    public void DisplayStats(int kills, float time, int score)
    {
        _killsText.text = kills.ToString();
        _timeText.text = FormatTime(time);
    }
}
```

## Technologies & Dependencies

- **Unity** - Game engine
- **C# 8+** - Language
- **VContainer** - Dependency injection framework
- **DOTween** - Animation library
- **TextMesh Pro** - UI text rendering
- **Unity Input System** - Input handling

## Key Takeaways for Employers

### Architectural Competence
- Understand and implement proven design patterns
- Design scalable systems with clear separation of concerns
- Apply SOLID principles for maintainability

### Software Engineering
- Professional code organization and structure
- Comprehensive documentation and comments
- Event-driven architecture for loose coupling
- Reactive state management patterns

### Problem Solving
- Currency system handling multiple transaction types
- Modifier system preventing poor UX through smart selection
- UI navigation with stack-based back button handling
- Performance optimization through reusable buffers

### Production Readiness
- Error handling and validation
- Extensible base classes for rapid feature development
- Scalable to larger team environments
- Testable code through dependency injection

## Usage

This is a reference codebase demonstrating architectural patterns. Key files to review:

1. **Architecture Overview**: `PROJECT_STRUCTURE_FOR_HOBBY_PROJECT.md`
2. **DI Configuration**: `Core/DI/GameLifetimeScope.cs`
3. **Service Layer**: `Services/` folder
4. **UI Patterns**: `UI/Screens/ScreenPresenter.cs` and implementations
5. **State Management**: `DataModels/` folder

## Notes

- This is a **code samples repository**, not a standalone game
- Includes only the most instructive systems and patterns
- Full game implementation would include additional systems (enemies, weapons, effects, etc.)
- All code follows professional documentation and naming standards

---

**Created for portfolio review** | Demonstrates enterprise-level code quality in game development context
