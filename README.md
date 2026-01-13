# OzzCodeGen & OzzLocalization

OzzCodeGen is a pluggable code generator library with a WPF UI, OzzCodeGen.Wpf. OzzLocalization is a companion library (with its own UI OzzLocalization.Wpf) used to create and manage translated strings that OzzCodeGen can consume during code generation.

## Prerequisites
- **.NET 10 SDK** – Download from [dotnet.microsoft.com](https://dotnet.microsoft.com/download/dotnet/10.0)
- **Visual Studio 2022** (or later) with WPF and C# workloads, or **Visual Studio Code** with C# extensions
- **Git** (optional, for cloning the repository)

## Components

```
OzzUtils (Shared Utilities)
    ↓
OzzLocalization ←→ OzzLocalization.Wpf
    ↓
OzzCodeGen ←→ OzzCodeGen.Wpf
    ├── CodeEngines (Pluggable generators)
    ├── Providers (EF Db-first, Empty)
    └── OzzCodeGen.Ef (EF integration)
```

- **OzzUtils**: Shared utilities library (extensions, helpers) consumed by code generation and localization projects.
- **OzzCodeGen**: Core library with domain model (`DataModel`, `EntityDefinition`, `BaseProperty`), provider abstractions (`IModelProvider`), and multiple code engines under `OzzCodeGen/CodeEngines`.
- **OzzCodeGen.Wpf**: WPF application for creating/opening a `CodeGenProject`, selecting a Model Provider (EF or Empty), adding Code Engines, and generating artifacts.
- **OzzLocalization**: Library that manages XML vocabularies (see `OzzLocalization/Vocabularies.cs`, `OzzLocalization/Vocabulary.cs`). Provides `vocabulary.??.xml` files consumed by resource-related engines.
- **OzzLocalization.Wpf**: WPF application for editing and organizing vocabularies used by OzzCodeGen.

## Solutions
- **OzzCodeGen.sln**: Complete tooling solution containing code generation and vocabulary management (all projects listed above).

## Build
Use .NET 10 SDK.

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
1. Create a new folder under `OzzCodeGen/CodeEngines/<YourEngineName>/`
2. Create a `BaseCodeEngine` subclass with:
   - `EngineId` property (unique identifier)
   - `DefaultFileName` for saving engine state
   - `OpenFile()` and `SaveToFile()` for persistence
   - `RefreshFromProject()` to sync with `CodeGenProject`
   - `UiControl` property exposing a WPF `UserControl` for UI
3. Add UI folder: `CodeEngines/<YourEngineName>/UI/` with your control
4. Register the engine in `OzzCodeGen/CodeEngines/EngineTypes.cs`:
   - Add a new case in `GetInstance()` method
   - Map the ID in the `OpenFile()` method
5. For templates, use `.tt` (T4 template) + `*.part.cs` pattern and wire `DependentUpon` in `.csproj`

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
	- Pick a Model Provider (e.g., Empty or EF `.edmx`).
	- Add engines via the plus button, e.g., `Localization_Resource_Generator`.
	- Save the project (`.OzzGen`) to establish `TargetSolutionDir`.
4. In the `Localization_Resource_Generator` UI:
	- Set `TargetFolder` (default `App_GlobalResources`).
	- Set `VocabularyFolder` to the folder containing `vocabulary.??.xml` (relative to the project file).
	- Choose `SingleResx` for a combined resource or per-entity resources.
	- Click Render to generate `.resx` files under `TargetSolutionDir/TargetFolder`.

## Localization Flow Example (ResxEngine)
- Engine ID: `Localization_Resource_Generator` (see `OzzCodeGen/CodeEngines/EngineTypes.cs`).
- Vocabulary discovery: ResxEngine loads vocabularies from `VocabularyDir` resolved from `CodeGenProject.SavedFileName` + `VocabularyFolder` (see `ResxEngine.VocabularyDir`).
- Outputs:
  - Default target folder is `App_GlobalResources` (see `ResxEngine.GetDefaultTargetFolder`).
  - Generates one `.resx` per culture code for each entity or a single combined file when `SingleResx` is enabled (see `ResxEngine.RenderSelectedTemplate`).
- Optional: Set `SaveWithVocabularies` to duplicate `vocabulary.??.xml` into the target directory on save (see `ResxEngine.SaveToFile`).

### Output Naming Examples
- **SingleResx enabled:** `EntityStrings.notr.resx`, `EntityStrings.tr.resx` (default file base is `SingleResxFilename` → `EntityStrings`).
- **Per-entity resources:** `CustomerString.notr.resx`, `OrderString.tr.resx` (default base from `GetDefaultTargetFile` → `${entity.Name}String`).

## Key Patterns
- Engine IDs and registration: see `OzzCodeGen/CodeEngines/EngineTypes.cs`.
- Project orchestration: see `OzzCodeGen/CodeGenProject.cs`.
- Data model serialization and helpers: see `OzzCodeGen/DataModel.cs`.
- EF Db-first provider: see `OzzCodeGen.Ef/Ef5.Provider.cs`.
- Vocabulary loading/saving: see `OzzLocalization/Vocabularies.cs` and `OzzLocalization/Vocabulary.cs`.

## Troubleshooting

### Build Issues
- **Restore fails**: Ensure .NET 10 SDK is installed (`dotnet --version`).
- **NuGet errors**: Clear cache: `dotnet nuget locals all --clear` and retry.

### Runtime Issues
- **Project fails to load**: Check `.OzzGen` file is valid XML. Verify model provider paths are accessible.
- **Model provider refresh fails**: For EF, ensure `.edmx` file exists and is valid. For Empty, verify `Defaults/` folder exists.
- **Engine output not appearing**: Check `TargetFolder` is accessible. Verify engine template is selected. Review Output window logs.
- **BuildInfo.Date not updating**: Right-click `BuildInfo.tt` → **Run Custom Tool** or rebuild solution.

For deeper troubleshooting, see [`.github/copilot-instructions.md`](.github/copilot-instructions.md#troubleshooting).
