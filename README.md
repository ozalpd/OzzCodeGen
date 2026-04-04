# OzzCodeGen & OzzLocalization

OzzCodeGen is a pluggable code generator library with a WPF UI, OzzCodeGen.Wpf. OzzLocalization is a companion library (with its own UI OzzLocalization.Wpf) used to create and manage translated strings that OzzCodeGen can consume during code generation.

## Current Versions
- `OzzCodeGen`: `2.2.10`
- `OzzCodeGen.Wpf`: `2.2.10`
- `OzzLocalization`: `2.1.6`
- `OzzLocalization.Wpf`: `2.1.6`

## Changelog

See [`CHANGELOG.md`](CHANGELOG.md) for release history.

## Latest Highlights (2.2.10)
- Added `AutoLoad` to SQLite repository property settings and exposed it in the WPF UI.
- Generated constructors with repository dependencies for autoloaded navigation types.
- Generated preload logic for related entities in `GetAllAsync` and `GetBy*Async` methods.
- Added generated `Load*Async` methods for autoloaded navigation properties.
- Updated generated `GetBy*Async` methods to accept nullable keys and return `null` when no key value is provided.
- Added and used helper methods for type checks, safe value expressions, and foreign key navigation.
- Reordered and resized SQLite repository property-grid columns, plus minor UI color improvements.
- Refactored property change notifications to use `nameof(...)`.

## Latest Highlights (2.2.9)
- Added `SingleColumnUpdate` to SQLite repository property settings and exposed it in the WPF UI.
- Generated `Update{ColumnName}Async` methods in SQLite repositories and repository interfaces for marked columns.
- Refactored `WriteColumnsAndParameters` to support custom value expressions.
- Added `DeleteAsync` method generation for all SQLite repositories.
- Generated partial `OnCreated` and `OnUpdated` methods for repository extensibility.
- Improved the SQLite repository property settings UI with a new side panel and minor style tweaks.

## Latest Highlights (2.2.8)
- Added explicit `CreatedAt`/`UpdatedAt` timestamp support to generated SQLite repositories.
- Added `CreatedAtName` and `UpdatedAtName` configuration to SQLite repository entity settings, with matching UI fields.
- Generated repository templates now set configured timestamp columns to `DateTime.UtcNow` on insert/update and avoid updating `CreatedAt`.
- Added `CheckIfAltered` to SQLite repository property settings for generated update checks.
- Expanded the SQLite repository UI to support timestamp fields and the `Overwrite Existing Files` option.
- Refactored update and unique-constraint logic for clarity and correctness.
- Fixed backing-field usage in `StorageColumnSetting`.

## Latest Highlights (2.2.7)
- Added `UpdateAsync` support to generated SQLite repository templates.
- Generated update methods now include unique-constraint checks, change detection, and selective column updates.
- Refactored repository parameter-writing logic for better clarity and reuse.
- Improved maintainability and correctness of generated SQLite repository code.

## UI Icons
- Icon set: **Bootstrap Icons v1.13.1**
- Source: https://icons.getbootstrap.com
- Repository: https://github.com/twbs/icons
- License: **MIT**
- Geometry resources are stored in `OzzCodeGen/Resources/BootstrapIcons.xaml`.

## Prerequisites
- **.NET 10 SDK** – Download from [dotnet.microsoft.com](https://dotnet.microsoft.com/download/dotnet/10.0)
- **Visual Studio 2026** (or later) with WPF and C# workloads, or **Visual Studio Code** with C# extensions
- **Git** (optional, for cloning the repository)

## Components

```
OzzUtils (Shared Utilities)
    ↓
OzzLocalization ←→ OzzLocalization.Wpf
    ↓
OzzCodeGen ←→ OzzCodeGen.Wpf
    ├── CodeEngines (Pluggable generators)
    └── Providers (Empty)
```

- **OzzUtils**: Shared utilities library (extensions, helpers) consumed by code generation and localization projects.
- **OzzCodeGen**: Core library with domain model (`DataModel`, `EntityDefinition`, `BaseProperty`), provider abstractions (`IModelProvider`), and multiple code engines under `OzzCodeGen/CodeEngines`.
- **OzzCodeGen.Wpf**: WPF application for creating/opening a `CodeGenProject`, selecting a Model Provider, adding Code Engines, and generating artifacts.
- **OzzLocalization**: Library that manages XML vocabularies (see `OzzLocalization/Vocabularies.cs`, `OzzLocalization/Vocabulary.cs`). Provides `vocabulary.??.xml` files consumed by resource-related engines.
- **OzzLocalization.Wpf**: WPF application for editing and organizing vocabularies used by OzzCodeGen.

## Solutions
- **OzzCodeGen.sln**: Complete tooling solution containing code generation and vocabulary management (all projects listed above).

## Build
Use .NET 10 SDK. Main Windows projects target `net10.0-windows10.0.19041.0`.

```bat
 dotnet restore OzzCodeGen.sln
 dotnet build OzzCodeGen.sln -c Debug
```

## Run
- **CodeGen UI**: Start `OzzCodeGen.Wpf`. Create/open a project (`*.OzzGen`), choose a Model Provider, add engines, then generate outputs.
- **Localization UI**: Start `OzzLocalization.Wpf`. Edit `vocabulary.??.xml` files (default `notr`) and save; engines in OzzCodeGen can use these strings.

## Development Setup

### IDE Setup
- **Visual Studio**: Open `OzzCodeGen.sln`, then set a WPF project as Startup (right-click project → **Set as Startup Project**).
- **VS Code**: Open the workspace folder; use the C# extension for IntelliSense and debugging.

### Local Development Workflow
1. Clone the repository: `git clone https://github.com/ozalpd/OzzCodeGen.git`
2. Open `OzzCodeGen.sln` in your IDE
3. Restore and build: `dotnet restore && dotnet build`
4. Set `OzzCodeGen.Wpf` or `OzzLocalization.Wpf` as Startup Project
5. Press **F5** to run or **Ctrl+F5** to run without debugging

### Adding a Custom Engine
1. Create a new folder under `OzzCodeGen/CodeEngines/<YourEngineName>/`.
2. Create a `BaseCodeEngine` subclass with:
   - `EngineId` property (unique identifier)
   - `ProjectTypeName` property
   - `DefaultFileName` for saving engine state
   - `CreateEntitySetting()` and `OpenFile()` for persistence and settings creation
   - `RefreshFromProject()` to sync with `CodeGenProject`
   - `GetTemplateList()` for explicit template selection
   - `UiControl` property exposing a WPF `UserControl` for UI
3. Prefer engine-specific entity and property settings instead of generic `EntitySetting` and `PropertySetting` types.
4. Add UI folder: `CodeEngines/<YourEngineName>/UI/` with your control.
5. Register the engine in `OzzCodeGen/CodeEngines/EngineTypes.cs`:
   - Add a new case in `GetInstance()` method
   - Map the ID in the `OpenFile()` method
6. For templates, use `.tt` (T4 template) + `*.part.cs` pattern and wire `DependentUpon` in `.csproj`. In-progress template scaffolds are acceptable while the engine contract is still being shaped.

### Adding a Custom Model Provider
1. Implement `IModelProvider` interface (see `OzzCodeGen/Providers/IModelProvider.cs`)
2. Implement `SelectSource()` for source selection UI
3. Implement `RefreshDataModel()` to return/update a `DataModel`
4. Wire provider selection in `MainWindow.xaml.cs` or provider dialog

### Template Development
- Templates are `.tt` (T4) files with accompanying `*.part.cs` files
- Example: `CodeEngines/Localization/Templates/SingleResx.tt` + `SingleResx.part.cs`
- **Regenerate manually**: Right-click `.tt` → **Run Custom Tool**
- **Automatic regeneration**: Preprocessed at build time via `.csproj` configuration

## Quick Start
1. Build the solution (see Build section).
2. Launch `OzzLocalization.Wpf` and create `vocabulary.notr.xml` under a folder next to your project file.
3. Launch `OzzCodeGen.Wpf` and create a new project:
	- Pick a Model Provider (e.g., Empty).
	- Add engines via the plus button, e.g., `Localization_Resource_Generator`.
	- Save the project (`.OzzGen`) to establish `TargetSolutionDir`.
4. In the `Localization_Resource_Generator` UI:
	- Set `TargetFolder` (default `$"{Project.TargetFolder}\\{Project.Name}.i18n"`).
	- Set `VocabularyFolder` to the folder containing `vocabulary.??.xml` (relative to the project file).
	- Choose `SingleResx` for a combined resource or per-entity resources.
	- Click Render to generate `.resx` files under `TargetSolutionDir/TargetFolder`.

## Localization Flow Example (ResxEngine)
- Engine ID: `Localization_Resource_Generator` (see `OzzCodeGen/CodeEngines/EngineTypes.cs`).
- Vocabulary discovery: ResxEngine loads vocabularies from `VocabularyDir` resolved from `CodeGenProject.SavedFileName` + `VocabularyFolder` (see `ResxEngine.VocabularyDir`).
- Outputs:
  - Default target folder is `$"{Project.TargetFolder}\\{Project.Name}.i18n"` (see `ResxEngine.GetDefaultTargetFolder`).
  - Default combined resource base name is `SingleResxFilename = "LocalizedStrings"`.
  - Generates one `.resx` per culture code for each entity or a single combined file when `SingleResx` is enabled (see `ResxEngine.RenderSelectedTemplate`).
- Optional: Set `SaveWithVocabularies` to duplicate `vocabulary.??.xml` into the target directory on save (see `ResxEngine.SaveToFile`).

### Output Naming Examples
- **SingleResx enabled:** `LocalizedStrings.notr.resx`, `LocalizedStrings.tr.resx` (default file base is `SingleResxFilename` → `LocalizedStrings`).
- **Per-entity resources:** `CustomerString.notr.resx`, `OrderString.tr.resx` (default base from `GetDefaultTargetFile` → `${entity.Name}String`).

## Target Platform

`CodeGenProject.TargetPlatform` controls the target .NET platform for generated code via the `TargetDotNetPlatform` enum (defined in `OzzCodeGen/_enums.cs`):

| Value | Description |
|---|---|
| `DotNetFramework` | Classic .NET Framework output (default for backward compatibility). |
| `ModernDotNet` | Modern .NET (.NET 6+) output — enables nullable reference type annotations (`?` suffix) and other modern conventions. |

Engines check `Project.TargetPlatform` to adapt generated code. For example, `MetadataPropertySetting.GetTypeName()` appends `?` for nullable reference types only when targeting `ModernDotNet`. The WPF UI exposes this via a **Target Platform** ComboBox in the toolbar.

## Code Engines

| Engine ID | Description | Status |
|---|---|---|
| `CS_Model_Class_Generator` | Generates C# model classes (primary path) using `CSharpModelClassTemplate`/`BaseCSharpModelClassTemplate`. | ✅ Active |
| `CS_Sqlite_Repository_Generator` | Generates C# SQLite repository classes using the `CsSqliteRepository` engine stack. | ✅ Active |
| `Metadata_Class_Generator` | Generates metadata/validation attribute classes (legacy compatibility path). | ✅ Active |
| `AspNetMvc_Controller_View_Generator` | Generates ASP.NET MVC controllers and Razor views. | ✅ Active |
| `T-Sql_Scripts_Generator` | Generates T-SQL DDL scripts. | ✅ Active |
| `Sqlite_Scripts_Generator` | Generates SQLite DDL scripts. | ✅ Active |
| `Localization_Resource_Generator` | Generates `.resx` resource files from XML vocabularies. | ✅ Active |
| `EF_Technical_Document` | Generates technical documentation from the data model. | ✅ Active |
| `EF_DatabaseFirst_DataLayer` | EF Database-First data layer generation. | ❌ Removed |
| `ObjectiveC_Code_Generator` | Objective-C code generation. | ❌ Removed |
| `Android_Code_Generator` | Android/Java code generation. | ❌ Removed |

## Migration Note: `CS_Model_Class_Generator` vs `Metadata_Class_Generator`

- Use `CS_Model_Class_Generator` for **new projects**. It is the primary and actively evolved C# model-class generation path.
- Keep `Metadata_Class_Generator` for **existing/legacy projects** that already depend on metadata engine IDs/settings and serialized project compatibility.
- Both engines remain loadable, but migration should be incremental:
  1. Create new outputs with `CS_Model_Class_Generator`.
  2. Keep `Metadata_Class_Generator` enabled for old outputs/settings until manual validation is complete.
  3. Remove legacy engine usage only after save/load and generated output parity checks pass.

## Key Patterns
- Engine IDs and registration: see `OzzCodeGen/CodeEngines/EngineTypes.cs`.
- Model-class primary stack:
  - `OzzCodeGen/CodeEngines/CsModelClass/CSharpModelClassCodeEngine.cs`
  - `OzzCodeGen/CodeEngines/CsModelClass/BaseModelClassCodeEngine.cs`
  - `OzzCodeGen/CodeEngines/CsModelClass/Templates/CSharpModelClassTemplate.tt`
  - `OzzCodeGen/CodeEngines/CsModelClass/Templates/BaseCSharpModelClassTemplate.tt`
  - `OzzCodeGen/CodeEngines/CsModelClass/Templates/BaseCSharpModelClassTemplate.part.cs`
- SQLite repository stack:
  - `OzzCodeGen/CodeEngines/CsSqliteRepository/CSharpSqliteRepositoryEngine.cs`
  - `OzzCodeGen/CodeEngines/CsSqliteRepository/Templates/BaseCSharpSqliteRepositoryTemplate.tt`
  - `OzzCodeGen/CodeEngines/CsSqliteRepository/Templates/BaseCSharpSqliteRepositoryTemplate.part.cs`
  - `OzzCodeGen/CodeEngines/CsSqliteRepository/Templates/CSharpSqliteBaseRepositoryTemplate.tt`
  - `OzzCodeGen/CodeEngines/CsSqliteRepository/Templates/CSharpSqliteBaseRepositoryTemplate.part.cs`
  - `OzzCodeGen/CodeEngines/CsSqliteRepository/Templates/CSharpSqliteRepositoryTemplate.tt`
  - `OzzCodeGen/CodeEngines/CsSqliteRepository/Templates/CSharpSqliteRepositoryTemplate.part.cs`
- Metadata compatibility stack:
  - `OzzCodeGen/CodeEngines/Metadata/MetadataCodeEngine.cs`
- Project orchestration: see `OzzCodeGen/CodeGenProject.cs`.
- Target platform enum: see `OzzCodeGen/_enums.cs` (`TargetDotNetPlatform`).
- Data model serialization and helpers: see `OzzCodeGen/DataModel.cs`.
- Model provider implementation (Empty): see `OzzCodeGen/Providers/EmptyModel.cs`.
- Vocabulary loading/saving: see `OzzLocalization/Vocabularies.cs` and `OzzLocalization/Vocabulary.cs`.

## Troubleshooting

### Build Issues
- **Restore fails**: Ensure .NET 10 SDK is installed (`dotnet --version`).
- **NuGet errors**: Clear cache: `dotnet nuget locals all --clear` and retry.

### Runtime Issues
- **Project fails to load**: Check `.OzzGen` file is valid XML. Verify model provider paths are accessible.
- **Model provider refresh fails**: For Empty, verify `Defaults/` folder exists.
- **Engine output not appearing**: Check `TargetFolder` is accessible. Verify engine template is selected. Review Output window logs.
- **BuildInfo.Date not updating**: Right-click `BuildInfo.tt` → **Run Custom Tool** or rebuild solution.

For deeper troubleshooting, see [`.github/copilot-instructions.md`](.github/copilot-instructions.md#troubleshooting).
