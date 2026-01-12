# OzzCodeGen & OzzLocalization

OzzCodeGen is a pluggable code generator library with a WPF UI, OzzCodeGen.Wpf. OzzLocalization is a companion library (with its own UI OzzLocalization.Wpf) used to create and manage translated strings that OzzCodeGen can consume during code generation.

## Components
- **OzzCodeGen**: Core library with domain model (`DataModel`, `EntityDefinition`, `BaseProperty`), provider abstractions (`IModelProvider`), and multiple code engines under `OzzCodeGen/CodeEngines`.
- **OzzCodeGen.Wpf**: WPF application for creating/opening a `CodeGenProject`, selecting a Model Provider (EF or Empty), adding Code Engines, and generating artifacts.
- **OzzLocalization**: Library that manages XML vocabularies (see `OzzLocalization/Vocabularies.cs`, `OzzLocalization/Vocabulary.cs`). Provides `vocabulary.??.xml` files consumed by resource-related engines.
- **OzzLocalization.Wpf**: WPF application for editing and organizing vocabularies used by OzzCodeGen.

## Solutions
- **OzzGenClassic.sln**: Code generation tooling (OzzCodeGen + OzzCodeGen.Wpf + engines/providers).
- **OzzLocalization.sln**: Vocabulary management (OzzLocalization + OzzLocalization.Wpf).

## Build
Use .NET 10 SDK.

```bat
dotnet restore OzzGenClassic.sln
dotnet build OzzGenClassic.sln -c Debug
dotnet restore OzzLocalization.sln
dotnet build OzzLocalization.sln -c Debug
```

## Run
- **CodeGen UI**: Start `OzzCodeGen.Wpf`. Create/open a project (`*.OzzGen`), choose a Model Provider, add engines, then generate outputs.
- **Localization UI**: Start `OzzLocalization.Wpf`. Edit `vocabulary.??.xml` files (default `notr`) and save; engines in OzzCodeGen can use these strings.

## Quick Start
1. Build both solutions (see Build section).
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
