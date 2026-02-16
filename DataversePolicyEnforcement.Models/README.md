# DataversePolicyEnforcement.Models

This project contains the generated early-bound model classes and option-set enums representing Dataverse entities used by the policy engine.

Contents
- `Entities/` — generated entity classes (for example `dpe_PolicyRule`, `dpe_PolicyCondition`) produced by Dataverse Model Builder.
- `OptionSets/` — generated enums for option set values used across the models.
- `OrgContext.cs` — helper context for creating queries or initializing the SDK context.
- `generate-models.ps1` and `builderSettings.json` — artifacts and script used to regenerate the models.

Notes
- The entity classes are auto-generated. Do not edit generated files directly because changes will be lost when models are regenerated.
- The project targets .NET Framework 4.6.2 and is consumed by other projects in the solution.
- To regenerate the models, run the provided `generate-models.ps1` script with appropriate credentials/environment for your Dataverse org.

Usage
- Reference this assembly from other projects to get strongly-typed access to Dataverse entities and option sets in code and tests.

Files under test
- Tests reference the generated classes (for example `dpe_PolicyRule` and `dpe_PolicyCondition`) and their option sets to create in-memory entities in `FakeXrmEasy`.
