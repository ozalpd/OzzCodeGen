# OzzCodeGen – AI Coding Instructions

Use this guide to be productive quickly in this repo. Focus on the concrete patterns implemented here (WPF apps + pluggable code-generation engines + XML-serialized models), not generic advice.

## Big Picture
- **Solution:** The repo contains a single solution [OzzCodeGen.sln](OzzCodeGen.sln) with both code generation tooling and vocabulary management.
- **Apps:** WPF fronts in [OzzCodeGen.Wpf](OzzCodeGen.Wpf) and [OzzLocalization.Wpf](OzzLocalization.Wpf). The former loads/edits a `CodeGenProject`, selects `CodeEngines`, and generates artifacts; the latter manages vocabularies.
- **Relationship:** OzzCodeGen is the code generator library with its UI in [OzzCodeGen.Wpf](OzzCodeGen.Wpf). [OzzLocalization](OzzLocalization) (and [OzzLocalization.Wpf](OzzLocalization.Wpf)) is used to create translated strings that OzzCodeGen consumes via its localization/resource engines.
- **Core library:** [OzzCodeGen](OzzCodeGen) contains the domain model (`DataModel`, `EntityDefinition`, `BaseProperty`), provider abstractions (`IModelProvider`), and multiple code engines under [CodeEngines](OzzCodeGen/CodeEngines).
- **Providers:** An interactive Empty provider lives in [OzzCodeGen/Providers](OzzCodeGen/Providers). Providers produce a `DataModel` from a source. The EF Database-First provider (`OzzCodeGen.Ef`) has been removed from the solution.
- **Localization:** [OzzLocalization](OzzLocalization) handles XML vocabularies (see [Vocabularies.cs](OzzLocalization/Vocabularies.cs), [Vocabulary.cs](OzzLocalization/Vocabulary.cs)).
- **OzzUtils:** Shared utilities library (e.g., common extensions, helpers) consumed by both code generation and localization projects.

## Architectural Patterns
- **Project orchestration:** `CodeGenProject` is the central state holder; engines attach to it and react to changes.
  - See [CodeGenProject.cs](OzzCodeGen/CodeGenProject.cs#L12-L25) for engine list and [GetCodeEngine](OzzCodeGen/CodeGenProject.cs#L306-L309).
  - `TargetFolder` is relative to the saved project file; resolved via [TargetSolutionDir](OzzCodeGen/CodeGenProject.cs#L121-L139).
- **Target platform:** `CodeGenProject.TargetPlatform` is a `TargetDotNetPlatform` enum (defined in [_enums.cs](OzzCodeGen/_enums.cs)) with two values:
  - `DotNetFramework` – classic .NET Framework output (default for backward compatibility).
  - `ModernDotNet` – modern .NET (.NET 6+) output, enabling nullable reference type annotations and other modern conventions.
  - Engines check `Project.TargetPlatform` to adapt generated code (e.g., `MetadataPropertySetting.GetTypeName()` appends `?` for nullable reference types only when targeting `ModernDotNet`).
  - The WPF UI exposes this via a `ComboBox` bound to `{StaticResource TargetPlatformValues}` in [MainWindow.xaml](OzzCodeGen.Wpf/MainWindow.xaml).
- **Data model:** `DataModel` is an `ObservableCollection<EntityDefinition>` with move/reorder helpers and XML (de)serialization (see [DataModel.cs](OzzCodeGen/DataModel.cs#L1-L22), [DataModel.cs](OzzCodeGen/DataModel.cs#L60-L97)).
- **Pluggable engines:** Engine IDs are centralized in [EngineTypes.cs](OzzCodeGen/CodeEngines/EngineTypes.cs); the WPF UI binds to these IDs and injects engine-specific UIs.
  - Active IDs: `CS_Model_Class_Generator`, `CS_Sqlite_Repository_Generator`, `Metadata_Class_Generator`, `AspNetMvc_Controller_View_Generator`, `T-Sql_Scripts_Generator`, `Sqlite_Scripts_Generator`, `Localization_Resource_Generator`, `EF_Technical_Document`.
  - `CsModelClass` is the primary C# model-class path; `CsSqliteRepository` generates C# SQLite repository classes (including DDL/seed/order settings and improved mapping behavior); `Metadata_Class_Generator` remains for compatibility/legacy project loading.
  - Engines now prefer engine-specific entity/property settings instead of generic `EntitySetting` and `PropertySetting` types.
  - Removed engines (throw `NotImplementedException` on load): `EF_DatabaseFirst_DataLayer`, `ObjectiveC_Code_Generator`, `Android_Code_Generator`.
  - The WPF app injects `Project.CurrentCodeEngine.UiControl` into the layout (see [MainWindow.xaml.cs](OzzCodeGen.Wpf/MainWindow.xaml.cs#L194-L209)).
- **Model providers:** Implement `IModelProvider` ([IModelProvider.cs](OzzCodeGen/Providers/IModelProvider.cs)), returning/refreshing a `DataModel`.
  - Empty provider discovers `.OzzGen` templates under `Defaults/` and opens an interactive dialog (see [EmptyModel.cs](OzzCodeGen/Providers/EmptyModel.cs#L78-L112), [EmptyModel.cs](OzzCodeGen/Providers/EmptyModel.cs#L116-L167)).
- **Templates & T4:** Many engine templates are `.tt`-backed with `*.part.cs` companions; the `.csproj` wires `DependentUpon` to keep generated pieces grouped (see [OzzCodeGen.csproj](OzzCodeGen/OzzCodeGen.csproj#L25-L112)).
  - C# model-class templates live under `CodeEngines/CsModelClass/Templates/` and share behavior via `BaseCSharpModelClassTemplate` + engine/settings base classes.
  - SQLite repository templates under `CodeEngines/CsSqliteRepository/Templates/` are active and evolving; keep mappings aligned with entity settings and storage-engine metadata.

## Developer Workflows
- **Build:** Uses .NET 10 SDK.
  - Restore and build from any terminal:
    - `dotnet restore OzzCodeGen.sln`
    - `dotnet build OzzCodeGen.sln -c Debug`
  - WPF startup projects: set [OzzCodeGen.Wpf](OzzCodeGen.Wpf) or [OzzLocalization.Wpf](OzzLocalization.Wpf) as Startup.
- **Run (CodeGen):** Launch `OzzCodeGen.Wpf`. Create/open a project (`*.OzzGen`), pick the Empty Model Provider, then add one or more Engines. Use the Refresh button to sync the model (see [MainWindow.xaml.cs](OzzCodeGen.Wpf/MainWindow.xaml.cs#L113-L129), [MainWindow.xaml.cs](OzzCodeGen.Wpf/MainWindow.xaml.cs#L239-L247)).
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
- In the engine UI: set `TargetFolder` (default `$"{Project.TargetFolder}\\{Project.Name}.i18n"`) and `VocabularyFolder` relative to the project file.
- `SingleResx` combined output uses `SingleResxFilename` with default value `LocalizedStrings`.
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
- **Provider UX:** `EmptyModel` uses a WinForms dialog; sets `CodeGenProject.ModelProvider` and feeds `DataModel` back to the project.
- **Serialization:** Project, data model, and vocabularies use XML serializers. Saving a project triggers engine-bound file saves (see [CodeGenProject.SaveBoundFiles](OzzCodeGen/CodeGenProject.cs#L220-L231)).
- **Defaults discovery:** The empty provider scans `Defaults/` recursively for `.OzzGen` files; the WPF app prompts for the folder if missing (see [MainWindow.xaml.cs](OzzCodeGen.Wpf/MainWindow.xaml.cs#L153-L173), [EmptyModel.cs](OzzCodeGen/Providers/EmptyModel.cs#L141-L167)).

## Naming Conventions
- **Base class naming:** Prefix base classes with 'Base' (e.g., `BaseModelClassPropertySetting`, `BaseModelClassCodeEngine`) to group related base classes together in Solution Explorer for improved readability.
- **Language-first template naming:** Template classes use a language-first convention so the target language is immediately obvious and the pattern scales to future languages:
  - `Base{Language}{Purpose}Template` – e.g., `BaseCSharpModelClassTemplate`
  - `{Language}{Purpose}Template` – e.g., `CSharpModelClassTemplate`, `TypeScriptModelClassTemplate`
  - `{Language}{Purpose}ValidatorTemplate` – e.g., `CSharpValidatorTemplate`, `RustValidatorTemplate`
- **Folder and namespace naming:** Prefer folder and namespace names that are as short as possible while still understandable; for example, prefer `CsSqliteRepository` over `CSharpSqliteRepository`.

## Extending This Repo
- **Add a model provider:** Implement `IModelProvider`, wire UI selection, and set `CodeGenProject.ModelProvider`. Provide `SelectSource()` for source picking and `RefreshDataModel()` for schema sync.
- **Add a code engine:** Create a `BaseCodeEngine` subclass with `EngineId`, `ProjectTypeName`, `CreateEntitySetting()`, `DefaultFileName`, `OpenFile()`, `RefreshFromProject()`, `GetTemplateList()`, and a `UiControl` user control under `CodeEngines/<Engine>/UI`. Register in [EngineTypes.cs](OzzCodeGen/CodeEngines/EngineTypes.cs).
- **Use templates:** Back templates with `.tt` and `*.part.cs`; align with existing `DependentUpon` usage in [OzzCodeGen.csproj](OzzCodeGen/OzzCodeGen.csproj).

### Code Engine Essentials
- When adding or substantially modifying a code engine, follow `.github/instructions/code-engine-development-guide.md`.
- Add the engine ID and wire both `EngineTypes.GetInstance()` and `EngineTypes.OpenFile()` so creation and project load both work.
- Prefer engine-specific entity/property settings instead of generic shared setting classes.
- Keep runtime-only state (`UserControl`, cached selections, computed directories, engine cross-references) marked with `[XmlIgnore]` and `[JsonIgnore]` so project settings round-trip cleanly.
- Expose engine-specific WPF UI through `UiControl`/`GetUiControl()` and rely on the host to inject it into `MainWindow`.
- Implement `GetTemplateList()` explicitly so template selection is clear even when the engine currently has only one effective output path.
- Keep output paths relative to `CodeGenProject.TargetSolutionDir` by using `TargetFolder`/`TargetDirectory` patterns instead of absolute persisted paths.
- If the engine uses T4 templates, include the `.tt` and generated companions in `OzzCodeGen.csproj` with the same `DependentUpon` pattern used by existing engines. Scaffolded `.tt` files are acceptable while generation behavior is still being completed.

## Testing Strategy
- **Manual verification:** Since automated tests are not present, verify changes via the WPF UIs.
- **Key workflows to test:**
  - Create a new project with the Empty Model Provider.
  - Add/remove engines and verify settings persist across save/load.
  - Modify data model and trigger engine refresh; confirm outputs update.
  - Test end-to-end: vocabulary creation → ResxEngine render → `.resx` generation.
  - Edge cases: empty models, large schemas, special characters in names, relative path resolution.
- **Regression checks:** After changes, run both WPF apps and exercise the main use cases in **Quick Start (Localization → ResxEngine)**.

## Troubleshooting
- **Project fails to load:** Check XML format in `.OzzGen` file. Ensure all referenced model provider paths are valid. See `CodeGenProject.OpenFile()`.
- **Model provider refresh fails:** For Empty, ensure `Defaults/` folder exists. Check Output window for detailed errors.
- **Engine output not appearing:** Verify `TargetFolder` resolves to an accessible directory (relative to `TargetSolutionDir`). Check engine's `RefreshFromProject()` and template selection. Review engine-specific logs in Output window.
- **Template generation error:** Inspect `.tt` file for syntax issues. Verify all referenced properties exist on the model. Manually regenerate via **Run Custom Tool**.
- **Serialization roundtrip fails:** Compare saved XML with schema expectations. Check for uninitialized collections or nested objects with null defaults.
- **BuildInfo.Date not updating:** Right-click `BuildInfo.tt` → **Run Custom Tool**, or rebuild the entire project.

## Notes
- Tests are not present; rely on manual verification via WPF apps.
- Project files use SDK-style `.csproj` format targeting .NET 10. Current Windows projects target `net10.0-windows10.0.19041.0`; assembly metadata (`Version`, `Copyright`, `Company`, `Product`, `Description`) is declared directly in each `.csproj`.
- Current version alignment:
  - `OzzCodeGen` and `OzzCodeGen.Wpf`: `2.2.2`
  - `OzzLocalization` and `OzzLocalization.Wpf`: `2.1.6`
- Versioning policy:
  - `OzzLocalization` and `OzzLocalization.Wpf` are expected to change infrequently and should normally advance with small monotonic patch increments (for example `2.1.6` -> `2.1.7` -> `2.1.8`) unless there is a real feature-driven or breaking-change reason to change minor or major versions.
- `AspNetMvc` templates have been updated for Bootstrap 5 compatibility, but not all templates have been fully checked yet.
- Existing template code should remain compatible with Font Awesome 4.7.0 until a later modernization pass.
- `OzzCodeGen.Wpf` UI icons were migrated from legacy PNG resources to Bootstrap icon path resources (`Resources/BootstrapIcons.xaml`).
- Bootstrap icon pack version in repo: **v1.13.1** (source: https://icons.getbootstrap.com, repository: https://github.com/twbs/icons, license: MIT).

## Code Engine Development Guide
For detailed checklists and guidelines on developing code engines, refer to `.github/instructions/code-engine-development-guide.md`. Keep essential stable rules in this document for quick reference.