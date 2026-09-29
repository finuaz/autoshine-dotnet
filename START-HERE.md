# AutoShine Repo 2 — Start Here

This repository intentionally starts as **documentation first**. Do not implement application code until the documented decisions are reviewed.

## Phase 0 — Local setup

1. Extract this starter pack so that `README.md`, `Directory.Packages.props`, `.gitignore`, and `context/` are at the repository root.
2. Open PowerShell in the repository root.
3. Verify the .NET SDK:

```powershell
dotnet --version
dotnet --list-sdks
```

AutoShine is currently planned for .NET 9. If the machine does not have a suitable .NET 9 SDK, stop here and install/configure it before continuing.

4. Create the empty solution:

```powershell
dotnet new sln -n AutoShine
```

5. Verify the solution exists:

```powershell
dotnet sln list
```

At this point an empty solution is intentional. We are not creating every project or installing every package yet.

## Git checkpoint

Recommended after Phase 0:

```powershell
git init
git add .
git commit -m "docs: initialize AutoShine architecture and context"
```

## Next cycle

Cycle 1 starts with the **basic MVC CRUD foundation**. We will add only the project(s), packages, database setup, and code required for that cycle, then stop at a working checkpoint.

## Important rule

Do not install the full technology stack just because it appears in `Directory.Packages.props`. Package versions are centrally defined, but package references are added to individual projects only when a cycle introduces that technology.
