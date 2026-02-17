# Dataverse Policy Enforcement — Client

Lightweight client library used to apply and enforce UI policies on Dataverse forms.

Overview
--------
This package contains the form entry points, small models, and service abstractions used by the client to evaluate policy configuration and apply UI decisions (visibility, required, disabled) at runtime.

Quickstart
----------
Prerequisites: Node.js and npm installed.

Install dev dependencies and run tests/build:

```bash
npm install
npm run build
npm test
```

Scripts
-------
- `npm run build` — bundle the project via rollup.
- `npm test` — run unit tests using `vitest`.
- `npm run clean` — clean build artifacts via `tsc --build --clean`.

Testing
-------
- Tests run with `vitest` (configured in `vitest.config.ts`) in a Node environment.
- The project uses `xrm-mock` for mocking Dynamics/XRM APIs when testing form behavior.

Layout
------
- `src/Form` — form entry points and handlers (see [src/Form/README.md](src/Form/README.md)).
- `src/Models` — types and `FormPolicyConfiguration` (see [src/Models/README.md](src/Models/README.md)).
- `src/Services` — service interfaces and `PolicyService` (see [src/Services/README.md](src/Services/README.md)).
- `tests` — unit tests and testing guidance (see [tests/README.md](tests/README.md)).

Contributing
------------
- Keep model classes focused on validation and shapes; put side-effecting logic in services.
- Unit-test new behavior; prefer injecting mocks for `IPolicyService` and `Xrm` for deterministic tests.

License
-------
See `package.json` (project currently uses ISC in `package.json`).
