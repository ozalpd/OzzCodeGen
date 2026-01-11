# OzzCodeGen – AI Coding Instructions

Use this guide to be productive quickly in this repo. Focus on the concrete patterns implemented here (WPF apps + pluggable code-generation engines + XML-serialized models), not generic advice.

## Big Picture
- **Solutions:** The repo hosts two solutions: [OzzGenClassic.sln](OzzGenClassic.sln) (code generation tooling) and [OzzLocalization.sln](OzzLocalization.sln) (vocabulary management).
- **Apps:** WPF fronts in [OzzCodeGen.Wpf](OzzCodeGen.Wpf) and [OzzLocalization.Wpf](OzzLocalization.Wpf). The former loads/edits a `CodeGenProject`, selects `CodeEngines`, and generates artifacts; the latter manages vocabularies.
- **Relationship:** OzzCodeGen is the code generator library with its UI in [OzzCodeGen.Wpf](OzzCodeGen.Wpf). [OzzLocalization](OzzLocalization) (and [OzzLocalization.Wpf](OzzLocalization.Wpf)) is used to create translated strings that OzzCodeGen consumes via its localization/resource engines.
- **Core library:** [OzzCodeGen](OzzCodeGen) contains the domain model (`DataModel`, `EntityDefinition`, `BaseProperty`), provider abstractions (`IModelProvider`), and multiple code engines under [CodeEngines](OzzCodeGen/CodeEngines).
- **Providers:** EF Db-first and an interactive Empty provider live in [OzzCodeGen.Ef](OzzCodeGen.Ef) and [OzzCodeGen/Providers](OzzCodeGen/Providers). Providers produce a `DataModel` from a source (e.g., `.edmx`).
- **Localization:** [OzzLocalization](OzzLocalization) handles XML vocabularies (see [Vocabularies.cs](OzzLocalization/Vocabularies.cs), [Vocabulary.cs](OzzLocalization/Vocabulary.cs)).

## Architectural Patterns
- **Project orchestration:** `CodeGenProject` is the central state holder; engines attach to it and react to changes.
  - See [CodeGenProject.cs](OzzCodeGen/CodeGenProject.cs#L12-L25) for engine list and [GetCodeEngine](OzzCodeGen/CodeGenProject.cs#L306-L309).
  - `TargetFolder` is relative to the saved project file; resolved via [TargetSolutionDir](OzzCodeGen/CodeGenProject.cs#L121-L139).
- **Data model:** `DataModel` is an `ObservableCollection<EntityDefinition>` with move/reorder helpers and XML (de)serialization (see [DataModel.cs](OzzCodeGen/DataModel.cs#L1-L22), [DataModel.cs](OzzCodeGen/DataModel.cs#L60-L97)).
- **Pluggable engines:** Engine IDs are centralized in [EngineTypes.cs](OzzCodeGen/CodeEngines/EngineTypes.cs); the WPF UI binds to these IDs and injects engine-specific UIs.
  - Example IDs: `AspNetMvc_Controller_View_Generator`, `EF_DatabaseFirst_DataLayer`, `T-Sql_Scripts_Generator`, `Sqlite_Scripts_Generator`.
  - The WPF app injects `Project.CurrentCodeEngine.UiControl` into the layout (see [MainWindow.xaml.cs](OzzCodeGen.Wpf/MainWindow.xaml.cs#L194-L209)).
- **Model providers:** Implement `IModelProvider` ([IModelProvider.cs](OzzCodeGen/Providers/IModelProvider.cs)), returning/refreshing a `DataModel`.
  - EF provider loads from `.edmx`, maps EF types and navigation, and marks keys/non-nullables; see [Ef5.Provider.cs](OzzCodeGen.Ef/Ef5.Provider.cs#L34-L63), [Ef5.Provider.cs](OzzCodeGen.Ef/Ef5.Provider.cs#L68-L108), [Ef5.Provider.cs](OzzCodeGen.Ef/Ef5.Provider.cs#L146-L196).
  - Empty provider discovers `.OzzGen` templates under `Defaults/` and opens an interactive dialog (see [EmptyModel.cs](OzzCodeGen/Providers/EmptyModel.cs#L78-L112), [EmptyModel.cs](OzzCodeGen/Providers/EmptyModel.cs#L116-L167)).
- **Templates & T4:** Many engine templates are `.tt`-backed with `*.part.cs` companions; the `.csproj` wires `DependentUpon` to keep generated pieces grouped (see [OzzCodeGen.csproj](OzzCodeGen/OzzCodeGen.csproj#L25-L112)).

## Developer Workflows
- **Build:** Uses .NET Framework 4.8 and `packages.config` restore.
  - Restore and build from a Developer Command Prompt:
    - `nuget restore OzzGenClassic.sln`
    - `MSBuild.exe OzzGenClassic.sln /p:Configuration=Debug`
  - WPF startup projects: set [OzzCodeGen.Wpf](OzzCodeGen.Wpf) or [OzzLocalization.Wpf](OzzLocalization.Wpf) as Startup.
- **Run (CodeGen):** Launch `OzzCodeGen.Wpf`. Create/open a project (`*.OzzGen`), pick a Model Provider (EF or Empty), then add one or more Engines. For EF, select an `.edmx` via provider dialog; use the Refresh button to sync schema (see [MainWindow.xaml.cs](OzzCodeGen.Wpf/MainWindow.xaml.cs#L113-L129), [MainWindow.xaml.cs](OzzCodeGen.Wpf/MainWindow.xaml.cs#L239-L247)).
- **Run (Localization):** Launch `OzzLocalization.Wpf` and edit `vocabulary.??.xml` files (default `notr`). See [Vocabularies.cs](OzzLocalization/Vocabularies.cs#L6-L20) for naming and [OpenVocabularies](OzzLocalization/Vocabularies.cs#L64-L95).
- **Generated outputs:** Engines persist their own settings/files next to the project and write artifacts under `CodeGenProject.TargetFolder` (default `..\Generated Codes`, resolved via [TargetSolutionDir](OzzCodeGen/CodeGenProject.cs#L121-L139)).
- **Version info:** `BuildInfo.tt` generates `BuildInfo.cs`; the main window title uses `BuildInfo.Date` (see [MainWindow.xaml.cs](OzzCodeGen.Wpf/MainWindow.xaml.cs#L23-L36)).

### Quick Start (Localization → ResxEngine)
- Build both solutions, then create `vocabulary.notr.xml` in a folder alongside your `.OzzGen` project file (use [OzzLocalization.Wpf](OzzLocalization.Wpf)).
- In [OzzCodeGen.Wpf](OzzCodeGen.Wpf): create a project, add `Localization_Resource_Generator`, and save the project to establish `TargetSolutionDir`.
- In the engine UI: set `TargetFolder` (default `App_GlobalResources`) and `VocabularyFolder` relative to the project file.
- Choose `SingleResx` for combined resources or per-entity; render to generate `.resx` under `TargetSolutionDir/TargetFolder` (see [ResxEngine.cs](OzzCodeGen/CodeEngines/Localization/ResxEngine.cs)).

## Conventions & Integration Points
- **Engine ID-first design:** UI and persistence use engine IDs; adding an engine requires updating [EngineTypes.GetInstance](OzzCodeGen/CodeEngines/EngineTypes.cs#L15-L60) and `OpenFile` mapping ([EngineTypes.cs](OzzCodeGen/CodeEngines/EngineTypes.cs#L62-L135)).
- **Engine UI contract:** Engines expose a WPF `UserControl` via `UiControl` and an optional settings dialog via `GetSettingsDlgUI()`; the host injects the control (see [MainWindow.xaml.cs](OzzCodeGen.Wpf/MainWindow.xaml.cs#L194-L209)).
- **Provider UX:** `EmptyModel` uses a WinForms dialog, `Ef5` uses file pickers; both set `CodeGenProject.ModelProvider` and feed `DataModel` back to the project.
- **Serialization:** Project, data model, and vocabularies use XML serializers. Saving a project triggers engine-bound file saves (see [CodeGenProject.SaveBoundFiles](OzzCodeGen/CodeGenProject.cs#L220-L231)).
- **Defaults discovery:** The empty provider scans `Defaults/` recursively for `.OzzGen` files; the WPF app prompts for the folder if missing (see [MainWindow.xaml.cs](OzzCodeGen.Wpf/MainWindow.xaml.cs#L153-L173), [EmptyModel.cs](OzzCodeGen/Providers/EmptyModel.cs#L141-L167)).

## Extending This Repo
- **Add a model provider:** Implement `IModelProvider`, wire UI selection, and set `CodeGenProject.ModelProvider`. Provide `SelectSource()` for source picking and `RefreshDataModel()` for schema sync.
- **Add a code engine:** Create a `BaseCodeEngine` subclass with `EngineId`, `DefaultFileName`, `OpenFile()`, `RefreshFromProject()`, and a `UiControl` user control under `CodeEngines/<Engine>/UI`. Register in [EngineTypes.cs](OzzCodeGen/CodeEngines/EngineTypes.cs).
- **Use templates:** Back templates with `.tt` and `*.part.cs`; align with existing `DependentUpon` usage in [OzzCodeGen.csproj](OzzCodeGen/OzzCodeGen.csproj).

## Notes
- Tests are not present; rely on manual verification via WPF apps.
- NuGet uses `packages.config` + `packages/` folder; prefer `nuget restore` over `msbuild /t:Restore` for compatibility.

If any section is unclear or missing (e.g., a specific engine’s output layout or provider dialogs), tell me which part you want expanded and I’ll iterate. 