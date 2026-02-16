# PolicyRule plugin tests (high level)

This folder contains unit tests that exercise the `ValidatePolicyRulePlugin` and related behavior for the Policy Rule plugin.

Purpose
- Verify the plugin enforces expected validation rules and reacts correctly to supported and malformed inputs.
- Ensure interactions with core components (for example `MetadataValidator`) behave as expected under test scenarios.

What is tested
- Successful validation paths (valid policy rule data leads to expected behavior).
- Failure cases (missing or invalid required fields, malformed metadata, or other rule violations).
- Integration points with core helpers such as the metadata validator (mocked where appropriate).

Test structure and conventions
- Tests are organized by the target class (`ValidatePolicyRulePlugin`).
- Each test focuses on a single behavior and follows Arrange/Act/Assert.
- Mocks and fakes are used for Dataverse/XRM services and external dependencies so tests remain fast and deterministic.

Running the tests
- Run tests from Visual Studio Test Explorer (recommended for .NET Framework 4.6.2 projects).
- Or use `vstest.console.exe` / your CI test task to run tests from the command line.

Contributing
- Add small, focused, deterministic tests.
- Put shared fixtures and helpers in the common test helpers area so they can be reused.

Notes
- These are unit tests and should avoid contacting live Dataverse instances; mock `IOrganizationService`, plugin context, and metadata interactions.
- Keep tests readable and explicit about the scenario being validated.
