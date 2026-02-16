# Core Model

This folder contains the lightweight domain model types used by the core policy evaluation logic.

Contents
- `PolicyDecision.cs` — top-level container for policy evaluation results with separate `ServerDetails` and `ClientDetails`.
- `PolicyDetails.cs` — concrete detail types: `ClientPolicyDetails` (Visible, Required, NotAllowed) and `ServerPolicyDetails` (Required, NotAllowed).

Purpose
- These types represent the computed outcome of policy evaluation for a single attribute and are passed between `PolicyEvaluator` and `PolicyScopeEvaluation` during tests and runtime.

Usage
- Construct or inspect `PolicyDecision` to evaluate or apply policy outcomes in code or tests.
- Defaults are chosen to reflect permissive client visibility (`Visible = true`) and non-required/non-blocking server defaults.
