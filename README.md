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



