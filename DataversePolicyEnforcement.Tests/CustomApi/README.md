CustomApi tests

This folder contains unit tests for the Custom API surface of the DataversePolicyEnforcement solution.

Purpose
- Validate Custom API request/response behavior for `DPE_GetAttributeDecisions`.
- Exercise the code paths that convert trigger values, query metadata, and build client policy decisions.

Key files
- `GetAttributeDecisions/DPE_GetAttributeDecisionsTests.cs` - main test class covering valid/invalid requests and different trigger attribute types (String, Integer, OptionSet, Lookup).
- `FakeMessageExecutors/DPE_GetAttributeDecisionFakeMessageExecutor.cs` - lightweight fake message executor used with FakeXrmEasy to register the custom API implementation for tests.

Test helpers and fixtures
- Tests rely on the shared `FakeXrmEasyTestBase` in the test project root which sets up the fake XRM context and `IOrganizationService`.
- `ConditionHelpers` and test metadata initialization are used to create policy conditions and entity attribute metadata required by the tests.
- `DataversePolicyEnforcement.CustomApi.Models.AttributeDecisionResult` is used to deserialize the Custom API response payload.

Running the tests
- Use Visual Studio Test Explorer or the `dotnet test` command from the solution root (the test project targets .NET Framework so use the appropriate test runner in Visual Studio).

Extending tests
- Add more scenarios to `DPE_GetAttributeDecisionsTests.cs` for additional attribute types or edge cases.
- If you add a new Custom API, create a corresponding fake message executor class under `FakeMessageExecutors` to register it with FakeXrmEasy.

Notes
- Keep tests focused on behavior of the Custom API surface; integration with the plugin/engine is covered in other test folders under `DataversePolicyEnforcement.Tests`.
