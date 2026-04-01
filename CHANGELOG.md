# Changelog

All notable changes to this project will be documented in this file.
The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

## [2.2.4] - 2026-04-01

### Added
- Added a SQLite metadata repository T4 template using a singleton pattern and `IMetadataRepository` interface.

### Changed
- Bumped `OzzCodeGen` version to `2.2.4`.
- Bumped `OzzCodeGen.Wpf` version to `2.2.4`.
- Refactored SQLite base repository template naming for consistency.
- Updated `CsSqliteRepository` engine output flow to generate both base and metadata repositories.
- Updated `.csproj` template wiring for new/renamed SQLite repository template files and outputs.
- Improved SQLite template formatting and XML documentation comments.
- Updated README to point to `CHANGELOG.md` for release history.
- Kept `OzzLocalization` version at `2.1.6` because there were no changes in this release.
- Kept `OzzLocalization.Wpf` version at `2.1.6` because there were no changes in this release.

## [2.2.3] - 2026-04-01

### Changed
- Enhanced `CsSqliteRepository` engine with per-entity DDL file, seed file, and order-by clause support.
- Exposed `ModelClassCodeEngine` from the SQLite repository engine for validator/template integration scenarios.
- Refactored repository mapping logic for stronger type safety and improved enum/null handling.
- Updated SQLite repository templates to use the new entity settings and improved mapping behavior.
- Added helper methods for `DisplayOrder`/`IsActive` detection and for repository/foreign-key column selection.
- Improved SQLite repository engine UI with new fields, clearer layout, and better property-grid resizing behavior.
- Updated default file-name behavior and hidden-column setup for clearer UI/editing flow.
- Bumped `OzzCodeGen` version to `2.2.3`.
- Bumped `OzzCodeGen.Wpf` version to `2.2.3`.
- Kept `OzzLocalization` version at `2.1.6` because there were no changes in this release.
- Kept `OzzLocalization.Wpf` version at `2.1.6` because there were no changes in this release.

### Notes
- This release focuses on flexibility, maintainability, and UX improvements for C# SQLite repository generation.

## [2.2.2] - 2026-03-31

### Added
- Added T4-generated SQLite repository template files under `OzzCodeGen/CodeEngines/CsSqliteRepository/Templates/`.
- Introduced `BaseCSharpPropertySetting` to centralize shared C# property type logic and engine access.

### Changed
- Refactored code generation engines to use engine-specific entity and property settings instead of the generic `EntitySetting` and `PropertySetting` types.
- Refactored `CSharpSqliteRepositoryEngine` to inherit directly from `BaseCodeEngine`, improve property synchronization, always generate its base repository class, and update default folders and namespaces.
- Updated code engine UIs and XAML for dynamic enum binding, adjusted column visibility, and related field renaming.
- Updated `BaseCodeEngine` to require `CreateEntitySetting()` and `ProjectTypeName`, and implemented those contracts across engines.
- Added explicit `GetTemplateList()` implementations to engines for clearer template selection.
- Updated Windows target frameworks to `net10.0-windows10.0.19041.0`.
- Bumped `OzzCodeGen` version to `2.2.2`.
- Bumped `OzzCodeGen.Wpf` version to `2.2.2`.
- Kept `OzzLocalization` version at `2.1.6` because there were no changes in this release.
- Kept `OzzLocalization.Wpf` version at `2.1.6` because there were no changes in this release.

### Notes
- The new SQLite repository `.tt` files are scaffolded and intentionally still close to empty while template work continues.
- Includes miscellaneous bug fixes, code cleanup, and maintainability improvements.

## [2.2.1] - 2026-03-30

### Changed
- Improved the `CsSqliteRepository` engine to resolve table names from the `SqliteScriptsEngine` when available, enabling accurate repository method generation.
- `SqliteRepositoryEntitySetting.TableName` now retrieves the table mapping from the related `StorageEntitySetting` if the SQLite Scripts engine is active.
- Bumped `OzzCodeGen` version to `2.2.1`.
- Bumped `OzzCodeGen.Wpf` version to `2.2.1`.
- Kept `OzzLocalization` version at `2.1.6` because there were no changes in this release.
- Kept `OzzLocalization.Wpf` version at `2.1.6` because there were no changes in this release.

## [2.2.0] - 2026-03-30

### Added
- Added the `CS_Sqlite_Repository_Generator` code engine for generating C# SQLite repository classes.
- Added the `CsSqliteRepository` engine stack under `OzzCodeGen/CodeEngines/CsSqliteRepository/` with engine, UI, and template files.

### Changed
- Standardized the new SQLite repository engine folder naming to the shorter `CsSqliteRepository` form.
- Moved the AI-focused code engine guide from the repo root to `.github/instructions/code-engine-development-guide.md`.
- Updated AI instruction references to point to the new guide location.
- Bumped `OzzCodeGen` version to `2.2.0`.
- Bumped `OzzCodeGen.Wpf` version to `2.2.0`.
- Kept `OzzLocalization` version at `2.1.6` because there were no changes in this release.
- Kept `OzzLocalization.Wpf` version at `2.1.6` because there were no changes in this release.

## [2.1.6] - 2026-03-30

### Changed
- Renamed and reorganized the model-class code generation engine to `CsModelClass` to make its C#-specific purpose explicit.
- Updated namespaces, class names, engine ID, templates, UI wiring, and project file references to use `CSharpModelClassCodeEngine`.
- Updated metadata engine dependencies to follow the new engine structure.
- Bumped `OzzCodeGen` version to `2.1.6`.
- Bumped `OzzCodeGen.Wpf` version to `2.1.6`.
- Bumped `OzzLocalization` version to `2.1.6`.
- Bumped `OzzLocalization.Wpf` version to `2.1.6`.
- No functional changes; this is a structural and clarity-focused refactor.

## [2.1.5] - 2026-03-26

### Changed
- Renamed `BaseModelClassTemplate` to `BaseCSharpModelClassTemplate`.
- Renamed `ModelClassTemplate` to `CSharpModelClassTemplate` (`.tt`, `.part.cs`, and generated `.cs`).
- Renamed `ValidatorTemplate` to `CSharpValidatorTemplate` (`.tt`, `.part.cs`, and generated `.cs`).
- Updated all references, constructors, and `.csproj` entries accordingly.
- Updated code comments in generated files for clarity.
- Reformatted `MainWindow` title string.
- Bumped `OzzCodeGen` version to `2.1.5`.
- Bumped `OzzCodeGen.Wpf` version to `2.1.5`.
- Bumped `OzzLocalization` version to `2.1.5`.
- Bumped `OzzLocalization.Wpf` version to `2.1.5`.
- No functional changes; focused on code organization and language-first naming consistency.

## [2.1.4] - 2026-03-25

### Changed
- Bumped `OzzCodeGen` version to `2.1.4`.
- Bumped `OzzCodeGen.Wpf` version to `2.1.4`.
- Bumped `OzzLocalization` version to `2.1.4`.
- Bumped `OzzLocalization.Wpf` version to `2.1.4`.
- Updated `AspNetMvc` templates for Bootstrap 5 compatibility.
- Kept existing Font Awesome 4.7.0-compatible template code.
- Template compatibility checks are still in progress for the remaining templates.

## [2.1.3] - 2026-03-25

### Added
- Added `ModelValidator` generation template to `ModelClassCodeEngine`.

### Changed
- Bumped `OzzCodeGen` version to `2.1.3`.
- Bumped `OzzCodeGen.Wpf` version to `2.1.3`.
- Bumped `OzzLocalization` version to `2.1.3`.
- Bumped `OzzLocalization.Wpf` version to `2.1.3`.
- Bound new properties to `ModelClassEngineUI.xaml`.

## [2.1.2] - 2026-03-24

### Changed
- Bumped `OzzCodeGen` version to `2.1.2`.
- Bumped `OzzCodeGen.Wpf` version to `2.1.2`.
- Bumped `OzzLocalization` version to `2.1.2`.
- Bumped `OzzLocalization.Wpf` version to `2.1.2`.
- Renamed `ErrorStrings.MaxLength` to `MaxStringLength` for better clarity in resource naming.

## [2.1.1] - 2026-03-23

### Changed
- Bumped `OzzCodeGen` version to `2.1.1`.
- Bumped `OzzCodeGen.Wpf` version to `2.1.1`.
- Bumped `OzzLocalization` version to `2.1.1`.
- Bumped `OzzLocalization.Wpf` version to `2.1.1`.
- Added `IsImmutable` property to `SimpleProperty`.
- Moved `IsStoreGenerated` property from `BaseProperty` to `SimpleProperty`.
- Updated `NewPropertyDialog` UI and bindings for `IsImmutable` and `IsStoreGenerated`.

### Removed
- Removed `UiVisible`, `SourceTypeName`, and `Editable` properties from `BaseProperty`.

## [2.1.0] - 2026-03-22

### Changed
- Bumped `OzzCodeGen` version to `2.1.0`.
- Bumped `OzzCodeGen.Wpf` version to `2.1.0`.
- Bumped `OzzLocalization` version to `2.1.0`.
- Bumped `OzzLocalization.Wpf` version to `2.1.0`.
- Updated `OzzCodeGen.Wpf` UI toolbar/menu icon usage to Bootstrap icon path resources (**Bootstrap Icons v1.13.1**).
- Updated `OzzLocalization.Wpf` UI toolbar icon usage to Bootstrap icon path resources (**Bootstrap Icons v1.13.1**).

### Removed
- Removed legacy PNG toolbar/menu icon resources from `OzzCodeGen.Wpf`.
- Removed legacy PNG toolbar icon resources from `OzzLocalization.Wpf`.

## [2.0.0] - 2026-03-20

### Changed
- Migrated all projects from .NET Framework to **.NET 10**
- Converted all project files to **SDK-style `.csproj`** format
- Removed legacy `Properties/AssemblyInfo.cs` from all projects; assembly metadata now generated by the SDK via `.csproj` properties
- Removed legacy `Properties/Settings.settings` and `Settings.Designer.cs` (were empty) from WPF apps
- Removed legacy `<ProjectGuid>` elements from all project files
- Removed `<GenerateAssemblyInfo>false</GenerateAssemblyInfo>` from all project files
- Removed stale `<Service Include="{508349b6-...}" />` (Roslyn 2013 opt-in) from project files
- Removed `<OutputType>Library</OutputType>` from library projects (SDK default)
- Simplified asset resource includes to wildcard `<Resource Include="Assets\**\*.*" />` in WPF projects
- Added version metadata (`Version`, `AssemblyVersion`, `FileVersion`, `InformationalVersion`) and product info (`Copyright`, `Company`, `Product`, `Description`) to all project files
- Replaced `BuildInfo.tt` / `BuildInfo.cs` T4 build-date mechanism with assembly version read via reflection (`Assembly.GetExecutingAssembly().GetName().Version`) in `MainWindow.DefaultTitle`

### Removed
- `OzzCodeGen.Ef` project (EF Database-First provider) removed from solution

## [1.0.0] - 2012-01-01

### Added
- Initial release targeting .NET Framework
- `OzzCodeGen` core library with pluggable code engine architecture
- `OzzCodeGen.Wpf` WPF application for project management and code generation
- `OzzLocalization` vocabulary management library
- `OzzLocalization.Wpf` WPF application for editing XML vocabularies
- `OzzUtils` shared utilities library
- Code engines: `Model_Class_Generator`, `Metadata_Class_Generator`, `AspNetMvc_Controller_View_Generator`, `T-Sql_Scripts_Generator`, `Sqlite_Scripts_Generator`, `Localization_Resource_Generator`, `EF_Technical_Document`
- Model providers: Empty (interactive), EF Database-First (`.edmx`)
- T4 template system for all code generation engines
- XML-serialized `.OzzGen` project file format
