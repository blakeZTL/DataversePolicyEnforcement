DataversePolicyEnforcement.CustomApi

This project contains the Custom API implementation for the DataversePolicyEnforcement solution.

Purpose
- Implement a Custom API (`DPE_GetAttributeDecisions`) that returns client-side policy decisions for attributes based on configured policy rules.
- Provide lightweight helpers and models consumed by plugins and tests.

Target framework
- .NET Framework 4.6.2 (keep this in mind when building or adding package references).

Key files and responsibilities
- `DPE_GetAttributeDecisions.cs` - the Custom API implementation. Accepts a `dpe_GetAttributeDecisionsRequest` and returns a `dpe_GetAttributeDecisionsResponse` containing serialized `AttributeDecisionResult` data for requested attributes.
- `Models/AttributeDecisionResult.cs` - DTO used to marshal policy decisions to callers (clients or UI code).
- `Helpers/MetadataHelper.cs` - helpers for working with `EntityMetadata` and attribute metadata used when converting values and validating trigger attribute types.
- `PluginBase.cs` - a small base class used by plugin/custom API code for shared functionality (e.g., context helpers).
- `dpe.snk` - strong name key used to sign the assembly (do not remove if you rely on strong-name signing in deployment).

How it is used
- The Custom API is invoked by client code (or tests) to get per-attribute UI guidance (visible/required/not-allowed) based on policy rules stored in Dataverse.
- Tests in `DataversePolicyEnforcement.Tests` exercise this Custom API via FakeXrmEasy by registering the implementation with a fake message executor.

Building and testing
- Build with Visual Studio targeting .NET Framework 4.6.2. If you add NuGet packages, ensure compatibility with this target framework.
- Unit tests run in the test project (`DataversePolicyEnforcement.Tests`). The tests use FakeXrmEasy to simulate Dataverse and call the Custom API implementation directly.

Extending
- To add another Custom API, add a class that follows the pattern of `DPE_GetAttributeDecisions` and expose request/response message classes in the Dataverse solution (or in code for tests).
- Add corresponding fake message executor classes in the test project under `DataversePolicyEnforcement.Tests/CustomApi/FakeMessageExecutors` to register the custom API with FakeXrmEasy.

Deployment notes
- When deploying to an environment, register the Custom API definition in Dataverse (via solution or Power Platform tools) so it becomes available to callers.
- Preserve the assembly strong name if the deployment process expects it.

Contact and contribution
- Follow the repository contribution guidelines and ensure new code includes unit tests where appropriate.
