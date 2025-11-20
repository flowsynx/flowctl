# Implementation Plan – Issue #110 (Dockerize FlowSynx Engine)

**Scope & Answers Incorporated**
- **Component scope:** Engine only for now. Console support is deferred; add later once config needs are clear.
- **Registry & tags:** Public images on Docker Hub (`docker.io/flowsynx/flowsynx`). Tags follow semantic versioning (e.g., `1.2.4-linux-amd64`, `1.2.4-windows-ltsc2022-amd64`).
- **Ports:** Engine listens on **6262** (Console would be 6264 when added).
- **Config/storage:** Persist install mode and Docker metadata alongside `flowctl` in an `appsettings.json` file (same directory as the CLI).
- **Persistence (recommended):** Default to a host bind mount `~/.flowsynx/data -> /app/data` so workflows, logs/telemetry, and plugins survive container restarts. Allow overrides for power users (named volumes or custom paths).
- **Testing expectation:** Unit tests only for the Docker pathway.

---

## Goals
- Add Docker-based initialization and run support for the FlowSynx engine via an explicit flag.
- Keep binary flow as the default and fully functional.
- Provide friendly UX when Docker is absent or not running and offer fallback guidance to binary mode.

---

## Architecture & Key Decisions
- **Docker client approach:** Use the existing process wrapper to shell out to the Docker CLI (simpler dependency story). Consider Docker.DotNet later if needed.
- **Configuration:** `appsettings.json` (sibling to `flowctl`) stores deployment mode and Docker artifact metadata.
- **Persistence:** Single host bind by default: mount `~/.flowsynx/data` to `/app/data`. Inside `/app/data`, keep subfolders for state/db, logs/telemetry, and plugins. This keeps user data portable and inspectable. CLI flags allow overriding to a named volume or a different host path.
- **Mode selection:** Binary remains default. Docker requires `--docker` (or `--use-docker`); we record the last successful mode in config to provide actionable hints.

---

## Workplan

### 1) Docker Service Abstraction
- Add `IDockerService` in Core and a CLI-based implementation in Infrastructure that can:
  - Check availability of Docker daemon.
  - Pull images with progress feedback.
  - Create/run/stop/remove containers.
  - Inspect container status and fetch logs (tail).
- Keep surface area focused on engine needs (no console operations yet).

### 2) Configuration & State
- Add/load `appsettings.json` beside the `flowctl` binary to track:
  - `deploymentMode`: `binary` or `docker`.
  - Docker details: image name, tag, container name/id, mapped host port, and host mount path used.
  - Last successful run mode to shape UX hints.
- Provide a small config service to read/update this file safely.

### 3) Init Command (`flowctl init --docker`)
- Add `--docker` flag (explicit opt-in).
- Options:
  - `--flowsynx-version` (tag), defaults to latest tag lookup if omitted.
  - `--container-name` (default: `flowsynx-engine`).
  - `--port` (host) default: 6262.
  - `--mount` (hostPath:containerPath) with default bind `~/.flowsynx/data:/app/data`.
  - `--platform` auto-detected to choose the right arch tag (linux-amd64, windows-ltsc2022-amd64).
- Flow:
  1. Check Docker availability; emit friendly guidance if missing/stopped and suggest binary fallback.
  2. Resolve tag (use provided tag or fetch latest), pick platform-specific image.
  3. Pull image.
  4. Create container with port mapping 6262 and bind mount `~/.flowsynx/data:/app/data` (unless overridden).
  5. Start container and wait for the engine to report healthy/up.
  6. Persist Docker metadata + mode in `appsettings.json`.
  7. Output connection info and how to run/stop.

### 4) Run Command (`flowctl run --docker`)
- Detect mode from `appsettings.json`; require `--docker` to enter Docker path.
- Behavior:
  - If container exists, start it (background by default).
  - If missing, recreate using stored image/tag/port/mount.
  - `--background` keeps container detached; without it, attach logs and handle Ctrl+C gracefully.
- Friendly errors when Docker is unavailable; suggest binary run when appropriate.

### 5) Stop/Remove Helpers (Scoped to Engine)
- `flowctl stop --docker`: stop the engine container if running.
- `flowctl uninstall --docker`: remove container; prompt before removing the host data directory if requested (default is to leave `~/.flowsynx/data` intact).

---

## Persistence Details (Best Course)
- **Default:** Bind mount `~/.flowsynx/data` to `/app/data` to persist workflow state/db, logs/telemetry, and plugins in one place that users can inspect and back up.
- **Why:** Easy to reason about, works across platforms, no hidden Docker volumes, and aligns with existing FlowSynx data expectations.
- **Overrides:** Allow users to:
  - Supply a different host path via `--mount hostPath:/app/data`.
  - Opt into a named volume via `--mount flowsynx-data:/app/data` if they prefer Docker-managed storage.
- Document permissions considerations for Linux/macOS when binding host paths.

---

## Testing
- Unit tests for Docker service (availability, pull/create/run/stop flows mocked).
- Unit tests for init/run option parsing and config persistence.
- No integration tests required for this scope.

---

## Risks & Mitigations
- Docker not installed or daemon down → clear error + link to install; suggest binary fallback.
- Port 6262 conflict → detect before run/create and allow `--port` override.
- Permission issues on host binds → document chmod/chown guidance; allow named volume override.
- Tag/platform mismatch → auto-detect OS/arch to choose the correct tag suffix, surface a clear error if unsupported.

---

## Future (Out of Scope Here)
- Add Console container support (port 6264) once its config story is defined.
- Broader Docker commands (logs/status/exec) and integration tests if needed.
- Registry auth/PAT handling if images ever become private.

