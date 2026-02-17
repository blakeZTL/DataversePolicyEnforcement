Tests overview
==============

High-level testing notes for this project — focuses on tooling and how tests are organized, not test content.

Core tools
----------
- `vitest` — primary test runner and assertion library used here (configured in `vitest.config.ts`).
- `xrm-mock` — used to mock the Dynamics/XRM APIs when exercising form logic that expects `Xrm` primitives.

Test layout & conventions
------------------------
- Tests live under `tests/` and may also appear next to source files under `src/` using the `.test.ts` suffix.
- Test filename pattern: `**/*.test.ts` (see `vitest.config.ts` include pattern).
- Tests run in a Node environment (no browser DOM by default).

Running tests
-------------
- Run the full test suite:

```bash
npm test
# or
npx vitest
```

Configuration notes
-------------------
- `vitest.config.ts` sets `environment: "node"` and includes both `tests/**/*.test.ts` and `src/**/*.test.ts`.
- Keep tests deterministic: mock `Xrm` and any network calls; prefer dependency injection for services where practical.

Best practices
--------------
- Unit tests should target small units (models, services, form handlers) and avoid manipulating real `Xrm` unless explicitly required.
- Use `xrm-mock` for form-level behavior; inject mock implementations of `IPolicyService` to test `onLoad` logic in isolation.
- Keep tests fast: avoid long artificial timeouts and prefer synchronous mocks where possible.

Troubleshooting
---------------
- If vitest reports tests missing, ensure test files end with `.test.ts` and are located under `tests` or `src`.
- For XRM-related failures in CI, ensure `xrm-mock` is available and that tests run in Node (no browser DOM expected).
