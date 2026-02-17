# Models folder

## Overview

The `src/Models` folder contains TypeScript types and a small configuration class used by the form and service layers to represent policy rules and decisions.

## Files

- `AttributePolicyDecision.ts` — `AttributePolicyDecision` interface returned from the `PolicyService` describing the computed policy for a specific attribute.
- `ClientPolicyDetails.ts` — `ClientPolicyDetails` interface describing UI-affecting flags (`visible`, `required`, `notAllowed`).
- `FormPolicyConfiguration.ts` — `FormPolicyConfiguration` class that parses and validates the JSON configuration provided to `onLoad` and groups rules into trigger groups.
- `index.ts` — re-exports from the folder.

## FormPolicyConfiguration behavior

- Constructor accepts JSON string or object and validates shape.
- Requires a top-level `rules` array; optionally accepts `triggerGroups`.
- If `triggerGroups` is missing, the class builds them by grouping `rules` by `triggerAttributeLogicalName`.
- Performs strict validation (types, non-empty strings, duplicate detection) and throws descriptive `TypeError`/`SyntaxError` on invalid input.

## Usage

The `onLoad` handler constructs a `FormPolicyConfiguration` from the provided `config` and uses `triggerGroups` to wire attribute change handlers that call the `PolicyService`.

## Testing

- Unit tests for configuration parsing and validation should target `FormPolicyConfiguration` behavior.

## Notes

- Keep models minimal and focused on data shape and validation — business logic belongs in services.
