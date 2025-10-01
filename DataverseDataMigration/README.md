# Project Structure

This project is designed to move data between two Microsoft Dynamics environments, either to restore accidentally deleted data or to migrate data between environments. It leverages the XRMFramework for connecting to Dynamics and performing data operations.

## Core Concepts

The data restoration process is organized around three main component types: **Fetchers**, **Mappers**, and **Creators**. Each plays a distinct role in the data migration workflow:

### 1. Fetchers

- **Purpose:** Retrieve data from the source Dynamics environment.
- **How:** Each fetcher is responsible for querying a specific entity along with related entities, constructing a hierarchical data structure that represents the entity and its relationships.
- **Example:** `AccountFetcher`, `AnnotationFetcher`
- **Output:** Returns the entity, including its children, and related links to reestablish links that were lost during deletion.

### 2. Mappers

- **Purpose:** Transform and map the raw data fetched from the source environment into a format suitable for saving on the local computer in JSON format. Transforms the saved JSON data back into a format ready for creation in the target environment.
- **How:** Mappers handle field mapping, data transformation, and any necessary adjustments to ensure compatibility between environments.
- **Example:** `AccountMapper`, `AnnotationMapper`
- **Output:** Returns mapped data, either as JSON for local storage or as entities ready for creation in the target system.

### 3. Creators

- **Purpose:** Insert or update the mapped data into the target Dynamics environment.
- **How:** Creators take the mapped data and use the XRMFramework to create or update records in the target system.
- **Example:** `AccountCreater`, `AnnotationCreater`
- **Output:** Confirms successful creation or update of records in the target environment.

## Workflow Overview

1. **Fetchers** extract data from the source environment.
2. **Mappers** process and transform the fetched data.
3. **Creators** import the mapped data into the target environment.

This modular approach ensures that each step of the data migration process is clearly separated, making the system easier to maintain, extend, and debug.

### 4. Orchestrators

- **Purpose:** Coordinate the overall data restoration or migration process for a specific entity or data set. Handles tracking of already imported/exported data to allow resume part way through the job in case of errors. Genereally contain a method for importing, exporting and generating a list of data to be exported.
- **How:** Orchestrators manage the workflow by invoking the appropriate fetchers, mappers, and creators in the correct sequence. They handle the end-to-end logic, including error handling, logging, and ensuring data dependencies are respected.
- **Example:** `OrchestratorAccounts`, `OrchestratorNotes`
- **Output:** Provides a high-level operation that ensures data is fetched, mapped, and created in the correct order, enabling a smooth and reliable data transfer process.


## Helpers

Helpers provide utility functions and shared logic to support the main components of the data restoration process. They encapsulate common operations to promote code reuse and maintainability.

### 1. XRMHelper

- **Purpose:** Simplifies and centralizes common operations, provides configuration allowing to do a dry run, or to skip business logic during data creation. Provides a summary of created entities to allow for easy verification of the migration process.
- **How:** Provides utility methods for tasks such as entity retrieval, relationship handling, and data formatting, reducing code duplication across fetchers, mappers, and creators.
- **Usage:** Used throughout the project to abstract away repetitive or complex XRMFramework interactions.

### 2. ReadDataFromDisk

- **Purpose:** Handles reading and deserializing data stored locally, in JSON format.
- **How:** Provides methods to load previously exported data from disk, making it available for mapping and creation in the target environment.
- **Usage:** Used during the import process to retrieve data that was exported and saved locally during the export process.


## Nice to have expansions
Clean up of all FF specific code and mappings.
General exception handling of outermost exception. Log exception and log current state of summary of created entities + time etc.
Refactoring of reused code, fx. HandleBrokenLinks in AccountCreator and other places.
General cleanup, and examples of how to use the scaffolding. 
Expanded readme with examples of how to use the project.