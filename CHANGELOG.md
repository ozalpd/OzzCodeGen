# Changelog

All notable changes to this project will be documented in this file.
The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

## [2.2.12] - 2026-04-06

### Changed
- Bumped `OzzCodeGen` version to `2.2.12`.
- Bumped `OzzCodeGen.Wpf` version to `2.2.12`.
- Increased `MainWindow` height from `640` to `832` for improved working space.
- Restructured the **Entities** tab layout in `MainWindow.xaml` for better usability.
- Adjusted Entities-tab column definitions to allocate more space to entity/property grids.
- Moved entity and property `DataGrid` regions into two-column `Grid` containers.
- Increased `DataGrid` column widths for better readability.
- Removed redundant `StackPanel` containers and obsolete row-definition usage in the Entities layout.
- Increased max width limits for main Entities-tab grids to `1280`.

## [2.2.11] - 2026-04-05

### Added
- Added generated `GetByForeignKey` methods for each foreign-key column in SQLite repositories.

### Changed
- Bumped `OzzCodeGen` version to `2.2.11`.
- Bumped `OzzCodeGen.Wpf` version to `2.2.11`.
- Refactored C# SQLite repository code generation templates to modularize method generation using helper methods.
- SQLite repositories now generate `GetByForeignKey` methods in addition to `GetAll`, `GetByPKey`, and `GetByUnique`.
- Renamed enum value `GetByUniqueIndex` to `GetByUnique` for clarity.
- Improved maintainability and support for more flexible querying in generated SQLite repositories.
- Kept `OzzLocalization` version at `2.1.6` because there were no changes in this release.
- Kept `OzzLocalization.Wpf` version at `2.1.6` because there were no changes in this release.

## [2.2.10] - 2026-04-04

### Added
- Added `AutoLoad` to SQLite repository property settings and exposed it in the WPF UI.
- Added constructor generation with repository dependencies for autoloaded navigation types.
- Added preload support for related entities in generated `GetAllAsync` and `GetBy*Async` methods.
- Added generation of `Load*Async` methods for autoloaded navigation properties.
- Added helper methods for type checks, safe value expressions, and foreign key navigation.

### Changed
- Bumped `OzzCodeGen` version to `2.2.10`.
- Bumped `OzzCodeGen.Wpf` version to `2.2.10`.
- Updated `GetBy*Async` method generation to accept nullable keys and return `null` when key values are not provided.
- Updated generated code to use safe value expressions for nullable foreign keys and nullable types.
- Reordered and resized SQLite repository property-grid columns in the UI.
- Improved UI colors with minor styling adjustments.
- Refactored property change notifications to use `nameof(...)`.
- Kept `OzzLocalization` version at `2.1.6` because there were no changes in this release.
- Kept `OzzLocalization.Wpf` version at `2.1.6` because there were no changes in this release.

## [2.2.9] - 2026-04-03

### Added
- Added `SingleColumnUpdate` to SQLite repository property settings and exposed it in the WPF UI.
- Added generation of `Update{ColumnName}Async` methods in SQLite repositories and repository interfaces for marked columns.
- Added generated `DeleteAsync` methods to all SQLite repositories.
- Added partial `OnCreated` and `OnUpdated` methods to generated repositories for extensibility.

### Changed
- Bumped `OzzCodeGen` version to `2.2.9`.
- Bumped `OzzCodeGen.Wpf` version to `2.2.9`.
- Refactored `WriteColumnsAndParameters` to support custom value expressions.
- Improved the SQLite repository property settings UI with a new side panel and minor style tweaks.
- Kept `OzzLocalization` version at `2.1.6` because there were no changes in this release.
- Kept `OzzLocalization.Wpf` version at `2.1.6` because there were no changes in this release.

## [2.2.8] - 2026-04-03

### Added
- Added `CreatedAtName` and `UpdatedAtName` properties to SQLite repository entity settings, with corresponding UI fields for configuration.
- Added `CheckIfAltered` to SQLite repository property settings for generated update/change-detection checks.

### Changed
- Bumped `OzzCodeGen` version to `2.2.8`.
- Bumped `OzzCodeGen.Wpf` version to `2.2.8`.
- Updated SQLite repository templates and generated code to set configured `CreatedAt`/`UpdatedAt` columns to `DateTime.UtcNow` during insert/update operations.
- Prevented generated repository updates from modifying configured `CreatedAt` columns.
- Expanded the SQLite repository UI to support timestamp field configuration and the `Overwrite Existing Files` option.
- Refactored update and unique-constraint logic for better clarity and correctness.
- Improved timestamp handling and overall robustness of generated SQLite repository code.
- Kept `OzzLocalization` version at `2.1.6` because there were no changes in this release.
- Kept `OzzLocalization.Wpf` version at `2.1.6` because there were no changes in this release.

### Fixed
- Fixed backing-field usage in `StorageColumnSetting`.


## [2.2.7] - 2026-04-03

### Added
- Added `UpdateAsync` method generation to SQLite repository templates.

### Changed
- Bumped `OzzCodeGen` version to `2.2.7`.
- Bumped `OzzCodeGen.Wpf` version to `2.2.7`.
- Generated repository `UpdateAsync` methods now include unique-constraint checks, change detection, and selective column updates.
- Refactored parameter-writing logic for clarity and reuse across generated repository methods.
- Improved maintainability and correctness of generated SQLite repository code.
- Kept `OzzLocalization` version at `2.1.6` because there were no changes in this release.
- Kept `OzzLocalization.Wpf` version at `2.1.6` because there were no changes in this release.

## [2.2.6] - 2026-04-02

### Added
- Introduced `BaseCSharpEntitySetting<T>` for shared C# entity logic, including nullable type handling.
- Added `IsIntNumeric` and `IsNullableString` helpers to `BasePropertySetting`.
- Added `IsUniqueIndexed` and `StorageColumnSetting` to `SqliteRepositoryPropertySetting`.
- Added `BaseCSharpSqliteRepositoryTemplate.tt` and generated `.cs` companion for reusable SQLite repository T4 helpers.

### Changed
- Bumped `OzzCodeGen` version to `2.2.6`.
- Bumped `OzzCodeGen.Wpf` version to `2.2.6`.
- Refactored `BaseModelClassEntitySetting` and `SqliteRepositoryEntitySetting` to inherit from the new C# entity base class.
- Moved repository property and mapping logic to `BaseCSharpSqliteRepositoryTemplate.part.cs` for better organization.
- Refactored `CSharpSqliteRepositoryTemplate` to generate CRUD and lookup methods, support unique indexes, and improve SQL parameter handling.
- Updated repository interface generation to include all relevant method signatures.
- Updated model and validator templates to clarify partial class usage.
- Updated project files to include new template and generated code entries.
- Applied miscellaneous code cleanups and namespace consistency improvements.
- Kept `OzzLocalization` version at `2.1.6` because there were no changes in this release.
- Kept `OzzLocalization.Wpf` version at `2.1.6` because there were no changes in this release.

## [2.2.5] - 2026-04-02

### Changed
- Bumped `OzzCodeGen` version to `2.2.5`.
- Bumped `OzzCodeGen.Wpf` version to `2.2.5`.
- Updated `DatabaseMetadata.LastUpdateUtc` to allow `NULL` values.
- Improved .NET-to-SQLite type mapping to map more numeric types to `INTEGER` and `datetime` to `TEXT`.
- Generated `ColNrs` structs for named column ordinals and used them in repository mapping expressions for readability and safety.
- Generated `ColumnNames` arrays and used them in `SELECT` statements for maintainability.
- Updated mapping helpers so ordinal values can be passed as named constants where appropriate.
- Improved maintainability, readability, and type-mapping accuracy in generated SQLite repository code.
- Kept `OzzLocalization` version at `2.1.6` because there were no changes in this release.
- Kept `OzzLocalization.Wpf` version at `2.1.6` because there were no changes in this release.

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
