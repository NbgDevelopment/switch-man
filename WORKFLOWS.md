# GitHub Actions Workflows

This repository uses GitHub Actions for Continuous Integration (CI) and Continuous Deployment (CD).

## Workflows

### CI Workflow (`ci.yml`)

The CI workflow builds and tests the application on every push.

**Triggers:**
- Automatically on push to **any branch**
- Manually via workflow_dispatch (see "Running Workflows Manually" below)

**What it does:**
1. Checks out the code
2. Sets up .NET 10
3. Restores dependencies
4. Builds the solution in Release mode with warnings treated as errors
5. Runs all tests (if any exist)

**Permissions Required:**
- `contents: read` - Already available by default

### CD Workflow (`cd.yml`)

The CD workflow builds a Docker image and publishes it to GitHub Container Registry.

**Triggers:**
- Automatically on push to the `main` branch
- Manually via workflow_dispatch from **any branch** (see "Running Workflows Manually" below)

**What it does:**
1. Checks out the code with full git history
2. Sets up .NET 10
3. Installs Nerdbank.GitVersioning tool (nbgv)
4. Generates a semantic version number based on git history
5. Logs into GitHub Container Registry (ghcr.io)
6. Builds the Docker image
7. Pushes the image with two tags:
   - Semantic version (e.g., `1.0.3`)
   - `latest`

**Permissions Required:**
- `contents: read` - Already available by default
- `packages: write` - Already available by default for GITHUB_TOKEN

## Running Workflows Manually

Both workflows can be started manually from any branch, including branches starting with `copilot/`.

### Steps to manually run a workflow:

1. Navigate to the **Actions** tab in the GitHub repository
2. Select the workflow you want to run (CI or CD) from the left sidebar
3. Click the **"Run workflow"** button on the right side
4. Select the branch you want to run the workflow on (defaults to current branch)
5. Click the green **"Run workflow"** button

**Note:** You need write access to the repository to manually trigger workflows.

## GitVersioning

This project uses [Nerdbank.GitVersioning](https://github.com/dotnet/Nerdbank.GitVersioning) to automatically generate version numbers based on git history.

### Configuration

Version configuration is stored in `version.json` at the repository root:

```json
{
  "version": "1.0",
  "publicReleaseRefSpec": [
    "^refs/heads/main$"
  ]
}
```

### Version Number Format

- **On main branch:** Produces stable versions like `1.0.3`
- **On other branches:** Produces pre-release versions like `1.0.3-alpha.45+g1234567`

The version number is automatically used to tag Docker images in the CD workflow.

### Changing the Version

To bump the major or minor version:

1. Update the `version` field in `version.json`
2. Commit and push the change
3. The next build will use the new version base

## Package Registry Setup

Docker images are published to GitHub Container Registry (ghcr.io). No additional setup is required as the workflow uses the built-in `GITHUB_TOKEN`.

### Accessing Published Images

Published images are available at:
```
ghcr.io/nbgdevelopment/switch-man:latest
ghcr.io/nbgdevelopment/switch-man:<version>
```

To pull and run a published image:

```bash
# Login to GitHub Container Registry (if the repository is private)
echo $GITHUB_TOKEN | docker login ghcr.io -u USERNAME --password-stdin

# Pull the latest image
docker pull ghcr.io/nbgdevelopment/switch-man:latest

# Run the container
docker run -d -p 8080:8080 ghcr.io/nbgdevelopment/switch-man:latest
```

**Note:** For public repositories, the images are publicly accessible and don't require authentication to pull.

## Troubleshooting

### CI Workflow Fails

- **Build errors:** Check the build logs for compilation errors. All warnings are treated as errors.
- **Test failures:** Check the test logs for failing tests.

### CD Workflow Fails

- **GitVersioning errors:** Ensure `version.json` exists and is valid JSON
- **Docker build errors:** Check the Dockerfile and build context
- **Push permission errors:** Ensure the workflow has `packages: write` permission (already configured)

### Manual Workflow Doesn't Appear

- Ensure you have write access to the repository
- Refresh the Actions page
- Ensure the workflow files are in the `.github/workflows/` directory on the branch you're viewing

## No Additional Setup Required

Both workflows are fully configured and ready to use. No manual setup, secrets, or configuration is required:

βœ… .NET 10 is installed automatically by the workflow  
βœ… Nerdbank.GitVersioning is installed automatically  
βœ… Docker Buildx is configured automatically  
βœ… GitHub Container Registry authentication uses GITHUB_TOKEN (automatic)  
βœ… All required permissions are specified in the workflow files  

Simply push code or manually trigger the workflows to use them.
