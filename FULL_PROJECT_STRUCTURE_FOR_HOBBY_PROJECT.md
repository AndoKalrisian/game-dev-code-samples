
# Project Directory Structure for Hobby Project

## Root Project Structure

```
sse-ss-proto/
├── Assets/
│   ├── _Project/                           # Main project folder (all custom content)
│   ├── AddressableAssetsData/              # Addressables system data and catalogs
│   ├── Data/                               # ScriptableObject data assets
│   ├── Plugins/                            # Third-party plugins (DOTween, etc.)
│   ├── Resources/                          # Unity Resources folder (legacy loading)
│   ├── Scenes/                             # Additional scenes (if any)
│   ├── StreamingAssets/                    # Files loaded at runtime
│   └── TextMesh Pro/                       # TextMesh Pro package assets
└── Packages/                               # Unity Package Manager dependencies
```

---

## Assets/_Project/ - Main Project Folder

```
_Project/
├── Art/
│   ├── Animations/                         # Animation clips and controllers
│   ├── Fonts/                              # Font assets for UI
│   └── Sprites/                            # 2D sprite assets and atlases
│
├── Audio/
│   ├── MainAudioMixer.mixer                # Master audio mixer configuration
│   ├── Music/                              # Background music tracks
│   ├── SFX/                                # Sound effects (impacts, UI sounds, etc.)
│   ├── SoundEvents/                        # Audio event ScriptableObjects
│   └── Voice/                              # Voice over files
│
├── Editor/
│   ├── BuildProfiler.cs                    # Build size profiling tool
│   ├── GameManagerDebugWindow.cs           # Custom editor window for GameManager
│   ├── ProfileViewer.cs                    # Performance profiling viewer
│   └── SaveDataEditorWindow.cs             # Editor tool for save data inspection
│
├── Prefabs/
│   ├── Drops/                              # Currency and modifier drop prefabs
│   ├── Enemies/                            # Enemy entity prefabs
│   ├── Environment/                        # Background and environment prefabs
│   ├── Player/                             # Player-related prefabs
│   └── UI/                                 # UI screen and component prefabs
│
├── Resources/
│   └── (Unity Resources)                   # Assets loaded via Resources.Load()
│
├── Scenes/
│   ├── Boot.unity                          # Bootstrap/initialization scene
│   └── test.unity                          # Testing/development scene
│
├── Scripts/                                # All C# source code (see detailed breakdown below)
│
├── Settings/
│   ├── GameConfig/                         # Game configuration ScriptableObjects
│   └── InputSystem/                        # Input System action maps and settings
│
├── ThirdParty/                             # Third-party assets and integrations
│
└── SceneLoadProfiler.cs                    # Scene loading performance profiler
```

---

## Scripts/ - Detailed Breakdown

### Architecture Overview
- **Pattern**: Model-View-Presenter (MVP) for UI
- **DI Framework**: VContainer for dependency injection
- **State Management**: Services layer with reactive state objects

```
Scripts/
│
├── Audio/
│   ├── AudioManager.cs                     # Audio system manager (SFX, music, mixing)
│   └── SoundEventData.cs                   # ScriptableObject for sound event configuration
│
├── Core/
│   ├── DI/
│   │   └── GameLifetimeScope.cs            # VContainer DI container configuration
│   └── GameManager.cs                      # Core game state machine and flow controller
│
├── DataModels/                             # Data structures (configs and runtime state)
│   ├── BaseEnemyConfig.cs                  # Enemy configuration ScriptableObject
│   ├── CurrencyConfig.cs                   # Currency type configuration
│   ├── CurrencyState.cs                    # Runtime currency amounts
│   ├── EnemySpawnerConfig.cs               # Enemy spawn wave configuration
│   ├── GameplayModifierItemConfig.cs       # Individual modifier configuration
│   ├── GameplayModifierItemList.cs         # Collection of all modifiers
│   ├── GameSessionState.cs                 # Current session runtime data
│   ├── LevelConfig.cs                      # Level progression configuration
│   ├── PlayerConfig.cs                     # Player base stats configuration
│   ├── PlayerState.cs                      # Runtime player stats
│   ├── RarityType.cs                       # Item rarity enum
│   ├── SaveLoadProgressionData.cs          # Persistent progression data structure
│   ├── SaveLoadSessionData.cs              # Session save data structure
│   ├── ShieldConfig.cs                     # Shield configuration
│   ├── ShieldState.cs                      # Runtime shield state
│   ├── StatModifier.cs                     # Stat modification data
│   ├── UpgradeItemConfig.cs                # Individual upgrade configuration
│   ├── UpgradeItemsDataList.cs             # Collection of all upgrades
│   ├── UpgradeItemState.cs                 # Runtime upgrade state
│   ├── WeaponConfig.cs                     # Weapon configuration
│   └── WeaponState.cs                      # Runtime weapon state
│
├── Gameplay/                               # Core gameplay systems
│   ├── Drops/
│   │   ├── CurrencyDrop.cs                 # Base currency drop behavior
│   │   ├── GameModifierDrop.cs             # Game modifier drop behavior
│   │   ├── HardCurrencyDrop.cs             # Premium currency drop
│   │   └── SoftCurrencyDrop.cs             # Standard currency drop
│   │
│   ├── Enemy/
│   │   ├── Movement/                       # Movement strategy implementations
│   │   │   ├── IEnemyMovement.cs           # Movement strategy interface
│   │   │   ├── LinearMovement.cs           # Straight-line movement
│   │   │   ├── GravityWellMovement.cs      # Gravity well attraction pattern
│   │   │   ├── BossBouncingMovement.cs     # Boss bouncing pattern
│   │   │   └── GravityWellBouncingMovement.cs  # Gravity well with bouncing
│   │   │
│   │   ├── BaseEnemy.cs                    # Abstract base class for all enemies
│   │   ├── BaseEnemyMover.cs               # Base enemy movement controller
│   │   ├── BossEnemy.cs                    # Boss enemy implementation
│   │   ├── BossMover.cs                    # Boss-specific movement controller
│   │   ├── EnemyHitPointsCalculator.cs     # Enemy HP scaling calculations
│   │   ├── EnemyMovementExample.cs         # Example/reference movement
│   │   ├── EnemyMover.cs                   # Standard enemy movement controller
│   │   ├── EnemySpawner.cs                 # Enemy wave spawning system
│   │   ├── GameModifierEnemy.cs            # Enemy that drops modifiers
│   │   ├── HardCurrencyEnemy.cs            # Premium currency enemy
│   │   └── SoftCurrencyEnemy.cs            # Standard currency enemy
│   │
│   ├── GameBg.cs                           # Background parallax/scrolling system
│   ├── Level.cs                            # Level controller
│   ├── LevelGenerator.cs                   # Procedural level generation
│   ├── PlayerReticle.cs                    # Player targeting and shooting system
│   └── PlayerVisual.cs                     # Player visual representation
│
├── Services/                               # Business logic layer (stateless services)
│   ├── CurrencyService.cs                  # Currency transactions and validation
│   ├── GameplayModiferService.cs           # Game modifier application logic
│   ├── SaveService.cs                      # Save/load with encryption
│   └── UpgradesService.cs                  # Upgrade purchase and application
│
├── Types/                                  # Enums and type definitions
│   ├── CurrencyType.cs                     # Currency type enum
│   ├── EnemyType.cs                        # Enemy type enum
│   ├── GameModifierId.cs                   # Game modifier identifier enum
│   ├── GameModifierType.cs                 # Game modifier category enum
│   ├── ScreenType.cs                       # UI screen identifier enum
│   ├── SfxType.cs                          # Sound effect type enum
│   ├── SSEValueType.cs                     # SSE value type enum
│   ├── UpgradeAlgorithmType.cs             # Upgrade calculation algorithm enum
│   ├── UpgradeId.cs                        # Upgrade identifier enum
│   └── UpgradeType.cs                      # Upgrade category enum
│
├── UI/                                     # User interface (MVP pattern)
│   ├── Context/
│   │   └── UIContextRegistry.cs            # Registry for UI data binding contexts
│   │
│   ├── Factories/
│   │   └── ScreenFactory.cs                # Factory for instantiating UI screens
│   │
│   ├── Interfaces/
│   │   └── IScreenManager.cs               # Screen manager interface
│   │
│   ├── Managers/
│   │   ├── ScreenManager.cs                # Screen lifecycle and transition manager
│   │   └── ScreenStackManager.cs           # Screen navigation stack manager
│   │
│   ├── Providers/
│   │   └── ScreenPrefabProvider.cs         # Addressables-based screen prefab loader
│   │
│   ├── Screens/                            # UI screens (MVP pattern: View + Presenter)
│   │   ├── Components/                     # Reusable UI components
│   │   │   ├── CurrencyBarPresenter.cs     # Currency bar logic
│   │   │   └── CurrencyBarView.cs          # Currency bar UI
│   │   │
│   │   ├── GameModifierOverlay/
│   │   │   ├── GameModifierCardPresenter.cs
│   │   │   ├── GameModifierCardView.cs
│   │   │   ├── GameModifierOverlayPresenter.cs
│   │   │   └── GameModifierOverlayView.cs
│   │   │
│   │   ├── GameOver/
│   │   │   ├── GameOverPresenter.cs
│   │   │   ├── GameOverUIContext.cs
│   │   │   └── GameOverView.cs
│   │   │
│   │   ├── HUD/
│   │   │   ├── HUDPresenter.cs
│   │   │   └── HUDView.cs
│   │   │
│   │   ├── MainMenu/
│   │   │   ├── MainMenuPresenter.cs
│   │   │   └── MainMenuView.cs
│   │   │
│   │   ├── PauseGameOverlay/
│   │   │   ├── PauseGameOverlayPresenter.cs
│   │   │   └── PauseGameOverlayView.cs
│   │   │
│   │   ├── ResumeGameOverlay/
│   │   │   ├── ResumeGameOverlayPresenter.cs
│   │   │   └── ResumeGameOverlayView.cs
│   │   │
│   │   ├── Upgrades/
│   │   │   ├── TabContentView.cs
│   │   │   ├── TabView.cs
│   │   │   ├── UpgradeItemPresenter.cs
│   │   │   ├── UpgradeItemView.cs
│   │   │   ├── UpgradesPresenter.cs
│   │   │   └── UpgradesView.cs
│   │   │
│   │   ├── ButtonView.cs                   # Base button component
│   │   ├── ElementView.cs                  # Base UI element component
│   │   ├── ImageView.cs                    # Image component wrapper
│   │   ├── ScreenPresenter.cs              # Base screen presenter
│   │   ├── ScreenView.cs                   # Base screen view
│   │   └── TextView.cs                     # Text component wrapper
│   │
│   └── Utils/                              # Utils
│       └── StringUtils.cs                  # String formatting utilities
│
└── Utils/                                  # [EMPTY] General utility functions
```

---

## Key Architectural Patterns

### 1. **Model-View-Presenter (MVP)**
- **View**: MonoBehaviour with UI component references
- **Presenter**: Pure C# class with business logic
- **Model**: Data state objects and services

### 2. **Dependency Injection (VContainer)**
- Services registered as singletons in `GameLifetimeScope.cs`
- Constructor injection for dependencies
- Supports testing and loose coupling

### 3. **Strategy Pattern**
- `IEnemyMovement` interface with multiple implementations
- Runtime swappable behaviors
- Open/closed principle compliance

### 4. **Service Layer**
- Stateless services: `CurrencyService`, `SaveService`, `UpgradesService`
- State objects: `PlayerState`, `WeaponState`, `CurrencyState`
- Clear separation of concerns

### 5. **Addressables**
- Asynchronous asset loading via `ScreenPrefabProvider`
- Memory-efficient resource management
- Runtime content delivery ready




