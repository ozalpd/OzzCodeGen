# OzzCodeGen – AI Coding Instructions

Use this guide to be productive quickly in this repo. Focus on the concrete patterns implemented here (WPF apps + pluggable code-generation engines + XML-serialized models), not generic advice.

## Big Picture
- **Solution:** The repo contains a single solution [OzzCodeGen.sln](OzzCodeGen.sln) with both code generation tooling and vocabulary management.
- **Apps:** WPF fronts in [OzzCodeGen.Wpf](OzzCodeGen.Wpf) and [OzzLocalization.Wpf](OzzLocalization.Wpf). The former loads/edits a `CodeGenProject`, selects `CodeEngines`, and generates artifacts; the latter manages vocabularies.
- **Relationship:** OzzCodeGen is the code generator library with its UI in [OzzCodeGen.Wpf](OzzCodeGen.Wpf). [OzzLocalization](OzzLocalization) (and [OzzLocalization.Wpf](OzzLocalization.Wpf)) is used to create translated strings that OzzCodeGen consumes via its localization/resource engines.
- **Core library:** [OzzCodeGen](OzzCodeGen) contains the domain model (`DataModel`, `EntityDefinition`, `BaseProperty`), provider abstractions (`IModelProvider`), and multiple code engines under [CodeEngines](OzzCodeGen/CodeEngines).
- **Providers:** EF Db-first and an interactive Empty provider live in [OzzCodeGen.Ef](OzzCodeGen.Ef) and [OzzCodeGen/Providers](OzzCodeGen/Providers). Providers produce a `DataModel` from a source (e.g., `.edmx`).
- **Localization:** [OzzLocalization](OzzLocalization) handles XML vocabularies (see [Vocabularies.cs](OzzLocalization/Vocabularies.cs), [Vocabulary.cs](OzzLocalization/Vocabulary.cs)).
- **OzzUtils:** Shared utilities library (e.g., common extensions, helpers) consumed by both code generation and localization projects.

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
- **Build:** Uses .NET 10 SDK.
  - Restore and build from any terminal:
    - `dotnet restore OzzCodeGen.sln`
    - `dotnet build OzzCodeGen.sln -c Debug`
  - WPF startup projects: set [OzzCodeGen.Wpf](OzzCodeGen.Wpf) or [OzzLocalization.Wpf](OzzLocalization.Wpf) as Startup.
- **Run (CodeGen):** Launch `OzzCodeGen.Wpf`. Create/open a project (`*.OzzGen`), pick a Model Provider (EF or Empty), then add one or more Engines. For EF, select an `.edmx` via provider dialog; use the Refresh button to sync schema (see [MainWindow.xaml.cs](OzzCodeGen.Wpf/MainWindow.xaml.cs#L113-L129), [MainWindow.xaml.cs](OzzCodeGen.Wpf/MainWindow.xaml.cs#L239-L247)).
- **Run (Localization):** Launch `OzzLocalization.Wpf` and edit `vocabulary.??.xml` files (default `notr`). See [Vocabularies.cs](OzzLocalization/Vocabularies.cs#L6-L20) for naming and [OpenVocabularies](OzzLocalization/Vocabularies.cs#L64-L95).
- **Generated outputs:** Engines persist their own settings/files next to the project and write artifacts under `CodeGenProject.TargetFolder` (default `..\Generated Codes`, resolved via [TargetSolutionDir](OzzCodeGen/CodeGenProject.cs#L121-L139)).

### T4 Templates & Build Info
- **BuildInfo pattern:** [BuildInfo.tt](OzzCodeGen.Wpf/BuildInfo.tt) auto-generates [BuildInfo.cs](OzzCodeGen.Wpf/BuildInfo.cs) with `BuildInfo.Date` (used in main window title).
  - T4 files are preprocessed at build time; the `.csproj` declares [DependentUpon](OzzCodeGen.Wpf/OzzCodeGen.Wpf.csproj) relationships so IDE groups `.tt` with generated `.cs`.
  - Manual regeneration in Visual Studio: right-click `.tt` file → **Run Custom Tool**.
  - If changes don't appear: rebuild solution or delete/recreate the generated file.
- **Template examples:** Engine templates under `CodeEngines/<Engine>/Templates/` follow the same `.tt` + `*.part.cs` pattern for maintainability.

### Quick Start (Localization → ResxEngine)
- Build the solution, then create `vocabulary.notr.xml` in a folder alongside your `.OzzGen` project file (use [OzzLocalization.Wpf](OzzLocalization.Wpf)).
- In [OzzCodeGen.Wpf](OzzCodeGen.Wpf): create a project, add `Localization_Resource_Generator`, and save the project to establish `TargetSolutionDir`.
- In the engine UI: set `TargetFolder` (default `App_GlobalResources`) and `VocabularyFolder` relative to the project file.
- Choose `SingleResx` for combined resources or per-entity; render to generate `.resx` under `TargetSolutionDir/TargetFolder` (see [ResxEngine.cs](OzzCodeGen/CodeEngines/Localization/ResxEngine.cs)).

## Engine Lifecycle
- **Instantiation:** `EngineTypes.GetInstance()` creates an engine from its ID; engines store settings in XML files next to the project.
- **Refresh:** When the data model changes, `RefreshFromProject()` is called on each active engine to sync state from the `CodeGenProject`.
- **Template rendering:** Engines expose template selection and rendering via their UI control (`UiControl`). Templates execute with access to the current `DataModel` and engine settings.
- **Output generation:** Rendered output is written to `CodeGenProject.TargetFolder`. Engines control file naming, encoding, and organization.
- **Settings persistence:** Engine state (UI selections, paths, flags) is saved via `SaveToFile()` when the project is saved (see [CodeGenProject.SaveBoundFiles](OzzCodeGen/CodeGenProject.cs#L220-L231)).

## XML Serialization Patterns
- **Serialization attributes:** Domain classes (`DataModel`, `EntityDefinition`, `BaseProperty`, `Vocabulary`) use `[XmlRoot]`, `[XmlElement]`, `[XmlAttribute]` for controlled serialization.
- **Custom logic:** Classes may implement `XmlSerializationHelper` methods or override serialization for nested structures (e.g., `EntityDefinition.Properties` as a collection).
- **Versioning:** Serialized XML is version-agnostic; new optional properties default to their type's default value if missing on deserialization.
- **Round-trip:** Ensure read/write cycles preserve data. Test by saving, closing, and reopening a project to verify no loss.

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

## Testing Strategy
- **Manual verification:** Since automated tests are not present, verify changes via the WPF UIs.
- **Key workflows to test:**
  - Create a new project with each Model Provider (Empty, EF).
  - Add/remove engines and verify settings persist across save/load.
  - Modify data model and trigger engine refresh; confirm outputs update.
  - Test end-to-end: vocabulary creation → ResxEngine render → `.resx` generation.
  - Edge cases: empty models, large schemas, special characters in names, relative path resolution.
- **Regression checks:** After changes, run both WPF apps and exercise the main use cases in **Quick Start (Localization → ResxEngine)**.

## Troubleshooting
- **Project fails to load:** Check XML format in `.OzzGen` file. Ensure all referenced model provider paths are valid. See `CodeGenProject.OpenFile()`.
- **Model provider refresh fails:** For EF, verify `.edmx` path is accessible. For Empty, ensure `Defaults/` folder exists. Check Output window for detailed errors.
- **Engine output not appearing:** Verify `TargetFolder` resolves to an accessible directory (relative to `TargetSolutionDir`). Check engine's `RefreshFromProject()` and template selection. Review engine-specific logs in Output window.
- **Template generation error:** Inspect `.tt` file for syntax issues. Verify all referenced properties exist on the model. Manually regenerate via **Run Custom Tool**.
- **Serialization roundtrip fails:** Compare saved XML with schema expectations. Check for uninitialized collections or nested objects with null defaults.
- **BuildInfo.Date not updating:** Right-click `BuildInfo.tt` → **Run Custom Tool**, or rebuild the entire project.

## Notes
- Tests are not present; rely on manual verification via WPF apps.
- Project files use SDK-style `.csproj` format with .NET 10 as the target framework.
- OzzUtils is a shared dependency across all projects; changes there may require rebuild of dependent projects.

If any section is unclear or missing (e.g., a specific engine's output layout or provider dialogs), tell me which part you want expanded and I'll iterate.