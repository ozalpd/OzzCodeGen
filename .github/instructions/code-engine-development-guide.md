# Code Engine Development Guide

Use this guide when adding a new code engine or making substantial changes to an existing one. It is a practical checklist for this repository, not a generic template-authoring guide.

For stable repo-wide rules, see `.github/copilot-instructions.md` first.

## Goals
- Make engine creation and loading work consistently.
- Keep engine settings XML-safe and round-trip safe.
- Follow the existing WPF host integration pattern.
- Keep generated output relative to the saved project.
- Match the folder, naming, and template patterns already used in `OzzCodeGen`.

## Good Reference Implementations
Start by comparing with one or more existing engines:
- `OzzCodeGen/CodeEngines/CsModelClass/CSharpModelClassCodeEngine.cs`
- `OzzCodeGen/CodeEngines/CsSqliteRepository/CSharpSqliteRepositoryEngine.cs` ← repository engine reference
- `OzzCodeGen/CodeEngines/WpfMvvm/WpfMvvmCodeEngine.cs`
- `OzzCodeGen/CodeEngines/Localization/ResxEngine.cs`
- `OzzCodeGen/CodeEngines/Metadata/MetadataCodeEngine.cs`
- `OzzCodeGen/CodeEngines/TechDocument/TechDocumentEngine.cs`

## Naming and Placement Checklist
- Create the engine under `OzzCodeGen/CodeEngines/<EngineName>/`.
- Prefer folder and namespace names that are as short as possible while still understandable. For example, prefer `CsSqliteRepository` over `CSharpSqliteRepository`.
- Put the main engine class in that folder.
- Put WPF engine UI under `OzzCodeGen/CodeEngines/<EngineName>/UI/`.
- Name the XAML code-behind file to match the XAML file exactly, for example `WpfMvvmEngineUI.xaml.cs` not `WpfMvvmEngineUI.cs`, so Visual Studio nests it correctly under the XAML file.
- Put templates under `OzzCodeGen/CodeEngines/<EngineName>/Templates/`.
- Prefer language-first template names such as `CSharpModelClassTemplate` or `TypeScriptModelClassTemplate`.
- Prefix base classes with `Base` when introducing reusable engine-specific abstractions.
- When an engine can share generated infrastructure with future engines (for example MVVM base/contracts), place that output in a dedicated folder setting (for example `InfrastructureFolder`) instead of mixing it with platform-specific View/ViewModel outputs.

## Required Engine Registration Checklist
A new engine is not complete until all of the following are wired:

1. Add an engine ID constant in `OzzCodeGen/CodeEngines/EngineTypes.cs`.
2. Update `EngineTypes.GetInstance()` to return a new engine instance for that ID.
3. Update `EngineTypes.OpenFile()` to load the engine settings file for that ID.
4. Add a static `DefaultFileName` on the engine class.
5. Implement a static `OpenFile(string fileName)` method on the engine class.
6. Make sure the engine can be added to a project and reopened from a saved `.OzzGen` project.

If `GetInstance()` is wired but `OpenFile()` is not, project load will fail for saved projects.

## Engine Class Checklist
A typical engine class should provide these members or equivalent behavior:
- `EngineId`
- `ProjectTypeName`
- `GetDefaultFileName()`
- `GetDefaultTargetFolder()`
- `OpenFile(string fileName)`
- `RefreshFromProject(bool cleanRemovedItems)`
- `RenderSelectedTemplate()`
- `RenderAllTemplates()` when applicable
- `GetUiControl()`

Most new engines should inherit from an existing base class when possible instead of starting directly from `BaseCodeEngine`.

Available base classes:
- `BaseModelClassCodeEngine` – for engines that generate C# model/metadata classes with property-level settings.
- `BaseAppInfraCodeEngine` – for app-language engines (for example C#/TypeScript style generators) that need shared `InfrastructureFolder`/`TargetInfrastructureDirectory` behavior.
- `BaseMvvmCodeEngine` – for engines that generate MVVM views, ViewModels, and commands. Platform-agnostic; extend for WPF, MAUI, etc.
- `BaseCodeEngine` – use directly only when no specialized base fits.

Use `BaseAppInfraCodeEngine` when the engine generates shared app-layer infrastructure output. Avoid it for engines that do not generate app-layer infrastructure (for example `CSharpModelClassCodeEngine`, `ResxEngine`, and SQL script engines).

For repository engines, build on the shared abstractions in `OzzCodeGen/CodeEngines/CsDbRepository/` rather than starting from scratch. `CSharpSqliteRepositoryEngine` derives from `BaseAppInfraCodeEngine` and uses `CsDbRepository` base classes as its foundation — study it before adding a new repository engine.

## Serialization Checklist
Project and engine settings are XML-serialized. Treat serialization as a first-class requirement.

Persist only stable settings. Mark runtime-only or computed members with both attributes:
- `[XmlIgnore]`
- `[JsonIgnore]`

Typical runtime-only members that should not be serialized:
- `UserControl` instances
- cached engine references
- filtered or computed collections
- selected UI items
- computed absolute directories like `TargetDirectory`

Typical persisted members:
- feature toggles
- namespace settings
- folder names relative to the project
- overwrite flags
- per-entity and per-property settings

After adding new persisted properties, verify save and reopen still round-trip correctly.

## Output Path Checklist
Do not persist absolute output paths.

Use the existing pattern:
- persist a relative `TargetFolder`
- compute the absolute directory from `CodeGenProject.TargetSolutionDir`
- render into `TargetDirectory`

If the engine needs an additional output location, follow the same relative-folder pattern used by `ValidatorFolder` and `TargetValidatorDirectory` in `CSharpModelClassCodeEngine`.

For shared generated infrastructure (for example MVVM base classes/contracts), keep a separate relative folder setting (for example `InfrastructureFolder`) and document the intent clearly. This folder is intentionally platform-agnostic so the generated base/contracts can be reused by future engines (for example, MAUI) with minimal duplication.

For repository engines, if contracts (interfaces) are generated separately from implementations, use a dedicated relative folder setting (for example `ContractsFolder`) and derive its namespace consistently. Keep folder/namespace resolution logic centralized — do not scatter it across templates, engine, and UI independently.

## WPF UI Integration Checklist
The host expects engines to expose a `UserControl` through `UiControl` and `GetUiControl()`.

Checklist:
- Create a `UserControl` under `CodeEngines/<EngineName>/UI/`.
- Set the control's `CodeEngine` property or data context using the existing engine pattern.
- Cache the control instance in the engine when the engine expects UI state to stay alive while switching selections.
- Keep host integration simple; `MainWindow` injects `Project.CurrentCodeEngine.UiControl` into the layout.
- If the engine needs a separate settings dialog, implement `GetSettingsDlgUI()`.

Use existing engines as the contract source rather than inventing a new host integration pattern.

## Data Model and Refresh Checklist
When the project data model changes, engine settings must stay in sync.

Checklist:
- Reuse `BaseCodeEngine.RefreshFromProject()` behavior or the engine-specific base class behavior whenever possible.
- Ensure new entities receive default settings.
- Ensure removed entities or properties are cleaned up when `cleanRemovedItems` is enabled.
- Rebind engine-owned collections and raise property change notifications where the UI depends on them.
- If the engine exposes filtered views such as `Entities`, keep the UI-facing property separate from the persisted backing collection when needed.
- If the engine depends on metadata from another engine (e.g., `CsSqliteRepository` reads table/key info from `SqliteScriptsEngine`), cache the reference with `[XmlIgnore]` and `[JsonIgnore]` and use `Project.GetCodeEngine()` to resolve it lazily.

For model-class-style engines, study `BaseModelClassCodeEngine` before introducing new sync logic.
For MVVM-style engines (views, ViewModels, commands), study `BaseMvvmCodeEngine` and `WpfMvvmCodeEngine` before introducing new sync logic.

## Template Checklist
If the engine generates files through templates:
- Place templates in `CodeEngines/<EngineName>/Templates/`.
- Reuse existing base template classes if possible.
- Keep file naming logic inside the template or engine consistently.
- If using T4, include both the `.tt` and generated companion files in `OzzCodeGen/OzzCodeGen.csproj`.
- Match the existing `DependentUpon` pattern so files stay grouped in Solution Explorer.
- Verify the generated file name returned by the template matches the engine output expectations.

## Project File Checklist
When adding new engine files, review `OzzCodeGen/OzzCodeGen.csproj` for any required entries.

Common cases:
- T4 template inclusion
- generated companion file inclusion
- `DependentUpon` metadata
- embedded resources or XAML-related entries if the engine UI introduces new assets

Not every new C# file requires explicit project updates, but T4-related additions usually do.

## Manual Verification Checklist
Because automated tests are not present, complete this checklist manually:

1. Build the solution.
2. Open `OzzCodeGen.Wpf`.
3. Create or open a project.
4. Add the engine.
5. Confirm the engine appears in the code engine list.
6. Confirm the engine UI loads when selected.
7. Save the project.
8. Close and reopen the project.
9. Confirm the engine reloads through `EngineTypes.OpenFile()` without errors.
10. Refresh the data model if the engine depends on entities or properties.
11. Render output.
12. Confirm files are written under the expected folder relative to `TargetSolutionDir`.
13. If the engine uses extra folders or validators, verify those derived directories too.
14. If the engine uses T4, run **Run Custom Tool** when needed and confirm generated companions are current.

## Recommended Implementation Order
Use this order to reduce rework:

1. Create the engine folder structure.
2. Add the engine class skeleton.
3. Register the engine in `EngineTypes.GetInstance()` and `EngineTypes.OpenFile()`.
4. Add persisted settings and mark runtime-only members with ignore attributes.
5. Add the WPF UI control.
6. Add template classes and file naming.
7. Update `OzzCodeGen.csproj` if T4 or generated companions are involved.
8. Build.
9. Run the WPF app and verify add, save, reopen, refresh, and render.

## Common Pitfalls
- Adding an engine ID but forgetting `OpenFile()` wiring.
- Persisting absolute paths instead of relative folders.
- Forgetting `[XmlIgnore]` and `[JsonIgnore]` on `UserControl` or computed properties.
- Introducing a UI control that is not cached when the engine expects selection state to persist.
- Adding templates without updating `OzzCodeGen.csproj` for T4-related files.
- Changing engine settings without checking save/reopen behavior.
- Replacing established base-class refresh logic with custom code unnecessarily.
- For repository engines: bypassing `CsDbRepository` shared base classes and duplicating helper logic instead.

## Short Pre-PR Checklist
Before considering engine work done, confirm:
- engine can be created
- engine can be saved
- engine can be reopened
- UI appears correctly
- refresh works
- render works
- output path is relative
- serialization round-trips cleanly
- build succeeds
