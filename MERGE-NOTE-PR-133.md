# Merge Conflict Note – PR #133 (feature/docker-engine vs master)

There is a single conflict when bringing `origin/master` into `feature/docker-engine`:

- **File:** `src/FlowCtl/Commands/Run/RunCommandOptionsHandler.cs`
- **Cause:** `feature/docker-engine` adds the Docker run path and settings persistence; `master` added XML docs/cleanup around `GetArgumentStr` (and nearby helpers). Git could not auto-merge those overlapping edits.

## How to resolve

1. Keep the Docker logic from `feature/docker-engine`:
   - The `RunDockerAsync` method with Docker pull/run/start and settings persistence.
   - The platform/tag resolution helpers and the Docker mode hinting.
2. Keep the upstream doc/comment/formatting improvements from `master`:
   - The XML documentation and tightened argument construction around `GetArgumentStr` (and related small cleanups).
3. Resulting file should have:
   - The full Docker pathway (pull/create/start, attach logs when not background).
   - The `GetArgumentStr` with upstream doc comment.
   - No conflict markers.

## Notes
- Do the merge in a clean worktree to avoid touching unrelated local changes (e.g., ConsoleLogger removals, JsonSerializer tweaks).
- After resolving, rerun `dotnet test` and push the updated branch before re-running the PR checks.
