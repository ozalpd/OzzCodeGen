# Changelog

All notable changes to this project will be documented in this file.
The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

## [2.2.23] - 2026-04-18

### Changed
- Replaced `GetNullableDecimal` with `GetDecimalFromInteger` and `GetDecimalFromText` helpers in `CSharpSqliteExtensionsTemplate` for clearer and more robust decimal mapping.
- Refactored `GetMappingExpression` in repository templates to use the new decimal helpers and accept a `needsComma` parameter for improved formatting.
- Updated SQLite repository templates to use the improved mapping and formatting logic throughout.
- No business logic changes; generated code is clearer and more robust for decimal handling.
- Bumped `OzzCodeGen` version to `2.2.23`.
- Bumped `OzzCodeGen.Wpf` version to `2.2.23`.
- Kept `OzzLocalization` version at `2.1.6` because there were no changes in this release.
- Kept `OzzLocalization.Wpf` version at `2.1.6` because there were no changes in this release.

## [2.2.22] - 2026-04-18

### Added
- Added paged query method generation (`GetPagedAsync`) to C# SQLite repository code generation, leveraging per-entity query parameter classes.
- Added WPF UI toggle per entity to enable/disable paged query generation (`GeneratePaged` property).
- Enhanced repository templates with modular autoloading and parameter handling for improved flexibility in query construction.

### Changed
- Refactored SQLite repository templates to better support modular parameter generation and filtering logic.
- Improved extensibility for future search and filter feature enhancements.
- Bumped `OzzCodeGen` version to `2.2.22`.
- Bumped `OzzCodeGen.Wpf` version to `2.2.22`.
- Kept `OzzLocalization` version at `2.1.6` because there were no changes in this release.
- Kept `OzzLocalization.Wpf` version at `2.1.6` because there were no changes in this release.

## [2.2.21] - 2026-04-17

### Added
- Added `IsDouble` and `IsFloat` properties to `BasePropertySetting` for fine-grained numeric type checks.
- Added `IsInteger` and `IsText` properties to `StorageColumnSetting` for clearer column-type detection.
- Expanded `SqliteExtensions` with overloads for `DateTime`, `bool`, integer, and string parameters (both nullable and non-nullable).
- Enhanced `SqliteRepositoryPropertySetting` with new column type helpers and improved XML documentation.

### Changed
- Renamed numeric type detection methods for clarity: `IsTypeIntNumeric` → `IsTypeIntegerNumeric` and updated all usages across templates and settings.
- Refactored SQLite repository templates to use the new type-safe `SqliteCommand` extension methods throughout.
- Improved handling of decimal-to-integer and decimal-to-text conversions in generated repository code.
- Bumped `OzzCodeGen` version to `2.2.21`.
- Bumped `OzzCodeGen.Wpf` version to `2.2.21`.
- Kept `OzzLocalization` version at `2.1.6` because there were no changes in this release.
- Kept `OzzLocalization.Wpf` version at `2.1.6` because there were no changes in this release.

### Fixed
- Minor code cleanup for maintainability across repository templates and property settings.

## [2.2.20] - 2026-04-16

### Added
- Added `SqliteExtensions` with reusable extension methods for nullable and scaled decimal SQLite parameter handling in generated repositories.
- Added `DecimalToIntegerScale` to SQLite repository property settings and exposed it in the WPF UI.
- Added generation of a `DecimalToIntegerScale` struct for entities that contain decimal columns.

### Changed
- Replaced `AddNullableTextParameter` usage with the new SQLite extension methods throughout repository templates and generated code.
- Renamed `IsDecimalNumeric` to `IsFractionalNumeric` and `IsIntNumeric` to `IsIntegerNumeric` for clarity and consistency.
- Updated T4 templates, generated code, and project files to support the new decimal-scale and SQLite-extension behavior.
- Bumped `OzzCodeGen` version to `2.2.20`.
- Bumped `OzzCodeGen.Wpf` version to `2.2.20`.
- Kept `OzzLocalization` version at `2.1.6` because there were no changes in this release.
- Kept `OzzLocalization.Wpf` version at `2.1.6` because there were no changes in this release.

## [2.2.19] - 2026-04-14

### Added
- Added `HasAnySearchCriteria()` virtual/override methods for more robust and extensible search-criteria detection in generated `QueryParameters` classes.
- Added partial-method support for custom `HasAnySearchCriteria()` logic extensions.

### Changed
- Refactored `QueryParametersTemplate` search generation logic to better distinguish simple searchable properties from min/max (date/numeric) searchable properties.
- Bumped `OzzCodeGen` version to `2.2.19`.
- Bumped `OzzCodeGen.Wpf` version to `2.2.19`.
- Kept `OzzLocalization` version at `2.1.6` because there were no changes in this release.
- Kept `OzzLocalization.Wpf` version at `2.1.6` because there were no changes in this release.

## [2.2.18] - 2026-04-14

### Added
- Added per-entity strongly-typed `QueryParameters` class generation support via the new `GenerateQueryParam` property.

### Changed
- Refactored model-class templates and related code paths to support generation of both base and derived `QueryParameters` classes.
- Updated search-parameter inclusion logic to use `IsSearchParameter` for controlling which properties participate in generated SQL/LINQ `WHERE` filtering.
- Improved naming consistency and updated project documentation.
- Bumped `OzzCodeGen` version to `2.2.18`.
- Bumped `OzzCodeGen.Wpf` version to `2.2.18`.
- Kept `OzzLocalization` version at `2.1.6` because there were no changes in this release.
- Kept `OzzLocalization.Wpf` version at `2.1.6` because there were no changes in this release.

## [2.2.17] - 2026-04-13

### Added
- Added support in the C# model-class engine to generate a `QueryParameters` helper class with paging/search support.
- Added a new QueryParameters T4 template and related project wiring for generation.
- Added WPF UI options to enable/disable QueryParameters generation and configure its namespace and target folder.
- Added `OnInitialized` partial hooks in generated SQLite repositories for flexible initialization.

### Changed
- Bumped `OzzCodeGen` version to `2.2.17`.
- Bumped `OzzCodeGen.Wpf` version to `2.2.17`.
- Improved code quality by using `nameof(...)` in property-change notifications.
- Hid more technical columns in property-grid views for cleaner UX.
- Refactored validator-template logic for improved maintainability.
- Applied minor UI/layout improvements for better usability.
- Updated project files to include newly added templates.
- Kept `OzzLocalization` version at `2.1.6` because there were no changes in this release.
- Kept `OzzLocalization.Wpf` version at `2.1.6` because there were no changes in this release.

## [2.2.16] - 2026-04-13

### Changed
- Bumped `OzzCodeGen` version to `2.2.16`.
- Bumped `OzzCodeGen.Wpf` version to `2.2.16`.
- Refactored generated repository autoload and dependency logic for improved correctness and maintainability.
- Added caching for autoload and foreign-key properties in `SqliteRepositoryEntitySetting`.
- Introduced `GetRepositoryName` and `HasThisKindOfRepository` helpers for more robust repository-name resolution and dependency checks.
- Updated template logic to inject repository dependencies only when required, improving generated constructor parameters.
- Updated `CSharpSqliteRepositoryTemplate.cs` and `CSharpSqliteRepositoryTemplate.tt` to generate repository interfaces as `partial`, enabling interface extensions across multiple files.
- Kept `OzzLocalization` version at `2.1.6` because there were no changes in this release.
- Kept `OzzLocalization.Wpf` version at `2.1.6` because there were no changes in this release.

## [2.2.15] - 2026-04-12

### Changed
- Bumped `OzzCodeGen` version to `2.2.15`.
- Bumped `OzzCodeGen.Wpf` version to `2.2.15`.
- Improved generated SQLite repository extensibility by adding an `OnLoaded` partial method call after entity loading.
- Improved autoload logic to skip self-referencing properties and `ICollection<>` types, preventing recursive loading paths.
- Enhanced repository-name resolution to handle `Dto` suffixes and `ICollection<>` wrappers more consistently.
- Limited generated autoload behavior to complex/navigation properties only.
- Improved null/empty checks and autoload handling in generated `GetAllAsync` and `GetByPKeyAsync` methods.
- Standardized generation of partial hooks for `OnLoaded`, `OnCreated`, and `OnUpdated`.
- Applied minor formatting and whitespace cleanup for readability.
- Increased correctness, extensibility, and maintainability of generated repository code.
- Kept `OzzLocalization` version at `2.1.6` because there were no changes in this release.
- Kept `OzzLocalization.Wpf` version at `2.1.6` because there were no changes in this release.

## [2.2.14] - 2026-04-12

### Changed
- Bumped `OzzCodeGen` version to `2.2.14`.
- Bumped `OzzCodeGen.Wpf` version to `2.2.14`.
- Improved T-SQL index generation in `CreateTSqlTable.part.cs` to correctly format column names ending with ` Desc` as descending index columns, using case-insensitive matching and proper SQL syntax.
- Kept `OzzLocalization` version at `2.1.6` because there were no changes in this release.
- Kept `OzzLocalization.Wpf` version at `2.1.6` because there were no changes in this release.

## [2.2.13] - 2026-04-11

### Added
- Added `CompositeIndexColumns` and related settings support to `StorageColumnSetting` for multi-column/composite index generation.

### Changed
- Bumped `OzzCodeGen` version to `2.2.13`.
- Bumped `OzzCodeGen.Wpf` version to `2.2.13`.
- Improved property change notifications using `nameof(...)` and expanded XML documentation in storage settings code.
- Refactored storage-setting methods for improved readability and maintainability.
- Updated T-SQL and SQLite storage templates to generate composite indexes and refactored index-generation logic.
- Updated `StorageEntitySetting` inheritance/structure for cleaner, more maintainable code.
- Cleaned up `StorageEngineUI` and `SqliteRepositoryEngineUI` by hiding more technical columns and improving load-state handling (`IsLoadingFromFile`).
- Reduced the `Namespace` column width in `MainWindow.xaml` for better UI balance.
- Applied general code cleanup and documentation improvements across the storage engine area.
- Kept `OzzLocalization` version at `2.1.6` because there were no changes in this release.
- Kept `OzzLocalization.Wpf` version at `2.1.6` because there were no changes in this release.

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
- Introduced `BaseCSharpPropertySetting` to centralize shared C# property type logic.

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
