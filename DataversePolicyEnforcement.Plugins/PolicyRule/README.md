# PolicyRule plugins (high level)

This folder contains the Policy Rule plugin(s) used by the solution to validate policy rule records at runtime.

Overview
- Primary plugin: `ValidatePolicyRulePlugin` — validates incoming Policy Rule records on create/update and enforces metadata/structure constraints.
- Responsibilities:
  - Validate required fields and value formats for policy rule records.
  - Use core helpers (for example `MetadataValidator`) to validate entity metadata or attribute-level constraints.
  - Interact with Dataverse/XRM services via `IOrganizationService` (mocked in unit tests).
  - Throw meaningful plugin exceptions (e.g., `InvalidPluginExecutionException`) when validations fail.

Architecture (high-level)

Flowchart (what happens during execution):

```mermaid
flowchart TD
  A[Dataverse event: Create/Update Policy Rule] --> B[Plugin pipeline invokes ValidatePolicyRulePlugin]
  B --> C{Load context & input parameters}
  C --> D[Validate required fields]
  C --> E[Call MetadataValidator]
  E --> F{Metadata valid?}
  F -->|Yes| G[Proceed / persist changes]
  F -->|No| H[Throw validation exception]
  D -->|Invalid| H
  D -->|Valid| E
  H --> I[Abort operation / return error to caller]
  G --> I
```

Sequence (interaction with helpers):

```mermaid
sequenceDiagram
  participant P as Plugin
  participant M as MetadataValidator
  participant S as IOrganizationService
  P->>M: Request metadata validation
  M-->>P: Validation result
  alt valid
    P->>S: Persist or allow operation
  else invalid
    P-->>Caller: Throw InvalidPluginExecutionException
  end
```

Testing
- Unit tests for this plugin live in `DataversePolicyEnforcement.Tests/Plugins/PolicyRule` (see `ValidatePolicyRulePluginTests.cs`).
- Tests use mocks/fakes for `IOrganizationService`, plugin execution context and the metadata validator to remain deterministic.
- Run tests via Visual Studio Test Explorer or `vstest.console.exe` in CI for .NET Framework 4.6.2 projects.

Guidelines for contributors
- Keep plugin logic small and focused; move reusable validation into core helpers (`MetadataValidator`) so it can be unit tested independently.
- Add unit tests for all new validation paths and edge cases.
- Avoid calling live Dataverse in unit tests — use mocking.

Notes
- This README is intentionally high-level. For implementation details see `ValidatePolicyRulePlugin.cs` and related core helpers in `DataversePolicyEnforcement.Core`.
