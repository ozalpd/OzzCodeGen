# OzzCodeGen & OzzLocalization

OzzCodeGen is a pluggable code generator library with a WPF UI, OzzCodeGen.Wpf. OzzLocalization is a companion library (with its own UI OzzLocalization.Wpf) used to create and manage translated strings that OzzCodeGen can consume during code generation.

## Current Versions
- `OzzCodeGen`: `2.3.2`
- `OzzCodeGen.Wpf`: `2.3.2`
- `OzzLocalization`: `2.1.6`
- `OzzLocalization.Wpf`: `2.1.6`

## Changelog

See [`CHANGELOG.md`](CHANGELOG.md) for release history.

## Latest Highlights (2.3.4)
- Added `CanDeleteAsync` to generated SQLite repositories for referential integrity checks before deletion.
- Add async `AnyByForeignKey` methods for each foreign key in `CSharpSqliteRepositoryTemplate`.
- Replaced `StorageEntitySetting.ForeignTables` property with a `GetForeignTables()` method to fix an issue of template `CreateMsSqlDb` and to ensure up-to-date foreign key resolution. Added XML docs to `GetForeignTables()` method for clarity.
- Refactored repository initialization to support both autoloaded and externally referenced repositories. Improved code generation for parameter lists, update statements, and column arrays.

## Latest Highlights (2.3.3)
- Introduce `WpfDialogServcTemplate` for generating dialog service interfaces and implementations for entity create/edit dialogs.
- Introduce `WpfCommandTemplate` for generating MVVM command classes (Create/Edit) in the WPF engine, including constructor wiring for view models, dialog services, and foreign lookups.
- Added `MvvmTemplate` enum and entity setting helpers for template-specific naming.
- Added properties `CommandVmTypeName` and `CommandVmNamespace` to `BaseMvvmEntitySetting<TPropertySetting>` for specifying ViewModel type name in generated command classes.
- Centralized accessibility logic with `GetAccessibility()` and standardized `IsPublic` property. Updated `.csproj` and performed related cleanup.
- Enhances the ViewModel template to generate fields, collections, and async loading methods for foreign lookup entities.
- Added `GetPreselectProperties()` to `WpfMvvmEntitySetting` for ordered preselect property retrieval.
- Renamed the WPF ViewModel template class and related files from `CSharpWpfViewModelTemplate` to `WpfViewModelTemplate`. Updated all references, constructors, and .csproj entries to use the new, shorter name.

## Latest Highlights (2.3.2)
- Renamed `ServiceFolder`/`ServiceNamespaceName` to `LookupFolder`/`LookupNamespaceName` in `BaseMvvmCodeEngine` for clarity.
- Added `PutLookupInInfra` property to control whether lookup services are placed in the infrastructure folder.
- Introduced `RepoContractNamespaceName` for explicit repository contract namespace configuration.
- Updated `CSharpLookupServiceTemplate` and `WpfMvvmEngineUI` bindings to use the new properties.
- Improved namespace logic for lookup service output based on infrastructure placement.
- Bumped `OzzCodeGen` and `OzzCodeGen.Wpf` to `2.3.2`.

## Latest Highlights (2.3.1)
- Introduced `CsDbRepository` base classes for repository code generation and refactored SQLite repository generation to use the new abstractions.
- Added support for generating repository contracts into separate folders and namespaces.
- Updated repository-related WPF UIs and templates to clarify infrastructure and contract settings.
- Improved namespace handling and applied code cleanup across repository code generation.
- Removed obsolete MVVM base class template.
- Bumped `OzzCodeGen` and `OzzCodeGen.Wpf` to `2.3.1`.

## Latest Highlights (2.3.0)
- Added new `Wpf_Mvvm_View_ViewModel_Generator` (`WpfMvvmCodeEngine`) to generate WPF Create/Edit Views, ViewModels, Commands, and per-entity Lookup Services.
- Added `CSharpLookupServiceTemplate.tt` for auto-generating lookup service interfaces and implementations with design-time/test stubs.
- Added detailed XML documentation generation for generated lookup service interfaces and classes.
- Refactored lookup-service template flow to use explicit `TemplateType` branches.
- Lookup-service design-time classes are now generated as `partial` (not `sealed`) for extensibility.
- Refined lookup-service runtime namespace imports so usings are emitted only when needed.
- Improved lookup-service overwrite behavior to respect per-entity generation flags.
- Lookup-service interface and design-time class generation now always runs for lookup services.
- Added `RepositoryInstanceName` to `BaseMvvmEntitySetting` for consistent DI naming in generated services.
- Added MVVM infrastructure generation support (`InfrastructureFolder`) for shared base/contracts output, intentionally platform-agnostic for future reuse (e.g., MAUI).
- Refactored infra/runtime folder handling: when `InfrastructureFolder` is empty, base classes/interfaces render to target ViewModels/Services folders; when set, they render to `InfrastructureFolder/ViewModels` and `InfrastructureFolder/Services`.
- Made `TargetInfrastructureDirectory` virtual in `BaseAppInfraCodeEngine` and centralized its path-selection logic.
- Added `GetNamespace()` to templates for correct infrastructure/runtime namespace selection.
- Updated `DefaultUsingNamespaceList()` to include contracts namespace when infrastructure output is separated.
- Adjusted `WpfMvvmCodeEngine` to write infrastructure/runtime files into the correct output directories.
- Added `GetFolderToNamespace` helper and cleaned up usings in `CsClassBase.part.cs`.
- Added `RenderBaseViewModels` to generate all base ViewModel classes: `AbstractViewModel`, `AbstractDataErrorInfoVM`, `AbstractCreateEditVM`, and `AbstractCollectionVM`.
- Added `CSharpWpfBaseVmTemplate.tt` and generated `.cs`/`.part.cs` files for base ViewModel and `IIsDirty` interface generation.
- Added `DefaultValue`, `FormatDefaultValue()`, `IsReadOnlyInCreate`/`IsReadOnlyInEdit`, and `ValueConstraint` to MVVM entity/property settings.
- Added `GenerateLookupService` to `BaseMvvmEntitySetting` for per-entity lookup-service generation control.
- Added `ServiceNamespaceName` and `ServiceFolder` to `BaseMvvmCodeEngine` for service output namespace/folder configuration.
- Added namespace/subfolder helpers and repository name support to MVVM engine settings and `WpfMvvmEngineUI`.
- Extended `BaseCSharpWpfMvvmTemplate` with `isInterface` parameter support for flexible interface/implementation generation.
- Rewrote WPF ViewModel T4 template with improved default-value and read-only logic; removed old `.cs`-based templates.
- Changed `IsForeignKey` from a method to a property; refactored `BasePropertySetting` to use expression-bodied type-check properties.
- Improved visual consistency across WPF engine UIs by standardizing button/DataGrid backgrounds, reducing control heights, and simplifying folder labels.
- Updated `AbstractEngineUI` to shorten "ViewModel" column headers to "VM".
- Enhanced `WpfMvvmEngineUI` with per-entity View/ViewModel toggles and improved property grid layout.
- Updated DataGrid hidden columns and widths in WPF MVVM and SQLite repository UIs.
- Updated `.csproj` to include the new WPF base ViewModel and lookup service templates and generated files.
- Default namespace logic for infrastructure/contracts now consistently uses `Project.NamespaceName`.
- Minor parameter refactoring in `CSharpWpfViewModelTemplate` for consistency and clarity.
- Fixed infrastructure directory resolution, infra/runtime namespace separation edge cases, and variable naming issues.
- Bumped `OzzCodeGen` and `OzzCodeGen.Wpf` to `2.3.0`.

## Latest Highlights (2.2.23)
- Replaced `GetNullableDecimal` with `GetDecimalFromInteger` and `GetDecimalFromText` helpers in `CSharpSqliteExtensionsTemplate` for clearer decimal mapping.
- Refactored `GetMappingExpression` to use new decimal helpers and accept a `needsComma` parameter for improved formatting.
- Updated SQLite repository templates to use improved mapping and formatting logic throughout.
- No business logic changes; generated code is clearer and more robust for decimal handling.
- Bumped `OzzCodeGen` and `OzzCodeGen.Wpf` to `2.2.23`.

## Latest Highlights (2.2.22)
- Added paged query method generation (`GetPagedAsync`) to C# SQLite repository code generation with per-entity query parameters.
- Added per-entity WPF UI toggle for paged query generation (`GeneratePaged` property).
- Refactored SQLite repository templates with modular autoloading and parameter handling for improved flexibility.
- Enhanced extensibility for future search and filter features.
- Bumped `OzzCodeGen` and `OzzCodeGen.Wpf` to `2.2.22`.

## Latest Highlights (2.2.21)
- Added `IsDouble` and `IsFloat` properties to `BasePropertySetting` for fine-grained numeric type checks.
- Added `IsInteger` and `IsText` properties to `StorageColumnSetting` for clearer column-type detection.
- Expanded `SqliteExtensions` with overloads for `DateTime`, `bool`, integer, and string parameters (nullable and non-nullable).
- Enhanced `SqliteRepositoryPropertySetting` with new column type helpers and improved documentation.
- Renamed `IsTypeIntNumeric` → `IsTypeIntegerNumeric` and updated all usages for consistency.
- Refactored SQLite repository templates to use new type-safe `SqliteCommand` extension methods.
- Improved decimal-to-integer and decimal-to-text conversion handling in generated repositories.
- Bumped `OzzCodeGen` and `OzzCodeGen.Wpf` to `2.2.21`.

## Latest Highlights (2.2.20)
- Added `SqliteExtensions` for reusable nullable and scaled decimal SQLite parameter handling in generated repositories.
- Added `DecimalToIntegerScale` to SQLite repository property settings and exposed it in the WPF UI.
- Added generation of a `DecimalToIntegerScale` struct for entities with decimal columns.
- Replaced `AddNullableTextParameter` usage in repository templates and generated code with SQLite extension methods.
- Renamed `IsDecimalNumeric`/`IsIntNumeric` to `IsFractionalNumeric`/`IsIntegerNumeric` for clearer intent.
- Bumped `OzzCodeGen` and `OzzCodeGen.Wpf` to `2.2.20`.

## UI Icons
- Icon set: **Bootstrap Icons v1.13.1**
- Source: https://icons.getbootstrap.com
- Repository: https://github.com/twbs/icons
- License: **MIT**
- Geometry resources are stored in `OzzCodeGen/Resources/BootstrapIcons.xaml`.

## Prerequisites
- **.NET 10 SDK** – Download from [dotneteurope.com](https://dotnet.microsoft.com/download/dotnet/10.0)
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
| `CS_Sqlite_Repository_Generator` | Generates C# SQLite repository classes using the `CsSqliteRepository` engine stack, including `SqliteExtensions`-based nullable/scaled decimal handling and per-property `DecimalToIntegerScale` support. | ✅ Active |
| `Wpf_Mvvm_View_ViewModel_Generator` | Generates WPF MVVM Create/Edit Views, ViewModels, and Commands; includes `InfrastructureFolder` output. This folder is intentionally platform-agnostic so the generated base/contracts can be reused by future engines (for example, MAUI) with minimal duplication. | ✅ Active |
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
- WPF MVVM stack (shared base in `Mvvm/`, WPF-specific in `WpfMvvm/`):
  - `OzzCodeGen/CodeEngines/Mvvm/BaseMvvmCodeEngine.cs` (platform-agnostic base)
  - `OzzCodeGen/CodeEngines/Mvvm/BaseMvvmEntitySetting.cs`
  - `OzzCodeGen/CodeEngines/Mvvm/BaseMvvmPropertySetting.cs`
  - `OzzCodeGen/CodeEngines/WpfMvvm/WpfMvvmCodeEngine.cs`
  - `OzzCodeGen/CodeEngines/WpfMvvm/Templates/WpfMvvmViewXamlTemplate.cs`
  - `OzzCodeGen/CodeEngines/WpfMvvm/Templates/WpfMvvmViewModelTemplate.cs`
  - `OzzCodeGen/CodeEngines/WpfMvvm/Templates/WpfMvvmCommandsTemplate.cs`
  - `OzzCodeGen/CodeEngines/WpfMvvm/Templates/MvvmBaseClassesTemplate.cs` (infrastructure)
  - `OzzCodeGen/CodeEngines/WpfMvvm/Templates/MvvmContractsTemplate.cs` (infrastructure)
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

For deeper troubleshooting, see [`.github/copilot-instructions.md`](.github/copilot-instructions.md#troubleshooting).
