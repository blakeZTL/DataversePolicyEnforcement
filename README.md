# DataversePolicyEnforcement

A solution for enforcing policies against Microsoft Dataverse data and operations. This repository provides core validation and enforcement logic with a client surface and a planned Custom API bridge to integrate with host applications.

Status: WIP — Core functionality is available; the `CustomApi` project is planned as the bridge between the client and core but hasn't been implemented yet.

Contents
 - `src/DataversePolicyEnforcement.Core` — Core libraries implementing validation, policy rules, and enforcement logic.
 - `src/DataversePolicyEnforcement.Client` — Client(s) that call into the core libraries to apply policies from a UI or automated process.
 - `src/DataversePolicyEnforcement.CustomApi` — (Planned) A Dataverse Custom API project that will act as the bridge between Dataverse clients and the `Core` logic.
 - `tests/` — Unit and integration tests for the core and client projects.

Key concepts
 - Policy: A rule or set of rules describing allowed or disallowed operations on Dataverse entities (create/update/delete) or attribute values.
 - Enforcement: The runtime logic that evaluates operations against policies and returns accept/deny responses or corrective actions.
 - Custom API bridge: A Dataverse-hosted endpoint that receives requests from the Dataverse runtime (or external clients) and routes them to the `Core` for evaluation.

Prerequisites
 - Git
 - .NET SDK 6.0 or newer (adjust to your target framework if different)
 - (Optional) Visual Studio 2022 / VS Code for development

Getting started

1. Clone the repository

   `git clone https://github.com/blakeZTL/DataversePolicyEnforcement.git`

2. Open the solution

   - From Visual Studio: open the solution file in `src/` (if present).
   - From the command line: `dotnet restore` in the solution root.

3. Build

   `dotnet build`

4. Run tests

   `dotnet test`

Project guidance

- Core
  - Location: `src/DataversePolicyEnforcement.Core`
  - Responsibilities: define policy models, evaluation engine, utilities for loading and applying policies.
  - Should be framework-agnostic so it can be reused by different hosts (Custom API, background services, CLI, etc.).

- Client
  - Location: `src/DataversePolicyEnforcement.Client`
  - Responsibilities: provide a consuming surface (console, service, or UI) that calls into `Core` to validate operations.

- Custom API (planned)
  - Location: `src/DataversePolicyEnforcement.CustomApi` (planned)
  - Responsibilities: host a Dataverse Custom API that receives requests from Dataverse workflows/plugins or direct clients, forwards them to `Core` for evaluation, and returns results.
  - Note: This project has not been built yet. Recommended approach: implement a lightweight Web API using the same .NET target as `Core`, keep request/response models minimal, and perform authorization and validation before delegating to `Core`.

Development notes
 - Keep `Core` free of Dataverse-hosting dependencies so it remains reusable.
 - Add comprehensive unit tests for each policy and enforcement path.
 - Use dependency injection for pluggable components (policy stores, logging, telemetry).

Roadmap / TODO
 - Implement `CustomApi` project to act as the bridge between Dataverse and `Core`.
 - Create sample Dataverse plugin or Power Automate connector that calls the Custom API.
 - Add more policy examples and test coverage.

Contributing
 - Fork the repo and open a pull request against `main`.
 - Follow existing code style and add tests for new behavior.

Contact
 - Repo: `https://github.com/blakeZTL/DataversePolicyEnforcement`
