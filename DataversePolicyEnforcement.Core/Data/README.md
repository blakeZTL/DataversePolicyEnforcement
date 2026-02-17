# Core Data

This folder contains data access helpers used by the policy engine to read `dpe_PolicyRule` and `dpe_PolicyCondition` records from Dataverse.

Key components
- `PolicyCollection` — implementation of `IPolicyCollection` that queries Dataverse for rules and conditions.
- `IPolicyCollection` — interface used to decouple data access from evaluation logic and to make testing easier.
 - `IPolicyCollection` — interface used to decouple data access from evaluation logic and to make testing easier.
 - `IMetadataValidator` — interface used to validate that entities and attributes exist in Dataverse metadata. Implementations (for example `MetadataValidator`) call the Dataverse metadata messages and return a boolean indicating whether the target entity/attribute(s) exist. Implementations also perform argument validation and handle service exceptions by returning false.

Policy Collection behavior
- `GetRules(service, entityLogicalName, attributeLogicalName)` validates arguments, returns only active `dpe_PolicyRule` records that match the entity/attribute, ordered by `dpe_Sequence` ascending.
- `GetConditions(service, ruleId)` validates arguments, returns only active `dpe_PolicyCondition` records for the provided rule ordered by `dpe_Sequence` ascending.
- `GetGovernedAttributes(service, entityLogicalName)` returns distinct attribute logical names that have active rules for the specified entity.

Metadata validation
- `ValidateEntity(service, logicalName)` verifies the entity exists in metadata and returns true when found; returns false for null/whitespace arguments or on errors.
- `ValidateAttribute(service, entityLogicalName, attributeLogicalName)` verifies a single attribute exists for the given entity; returns false for invalid arguments or on errors.
- `ValidateAllAttributes(service, entityLogicalName, attributeLogicalNames)` verifies that all provided attribute logical names exist on the entity. Returns true for a null/empty attribute list (nothing to validate), false for invalid arguments or when any attribute is missing.

Testing
- Tests in `DataversePolicyEnforcement.Tests/Core/Data/PolicyCollectionTests.cs` use `FakeXrmEasy` to create in-memory entities and validate filtering, active-state, and ordering behavior.
 - Tests in `DataversePolicyEnforcement.Tests/Core/Data/PolicyCollectionTests.cs` use `FakeXrmEasy` to create in-memory entities and validate filtering, active-state, and ordering behavior.
 - Metadata validation tests are in `DataversePolicyEnforcement.Tests/Core/Data/MetadataValidatorTests.cs` and exercise argument validation, success paths and error handling for the `IMetadataValidator` implementation.
