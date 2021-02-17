# OpenAPI Diff Action

This GitHub Action compares two OpenAPI (3.x) specs to determine if the newer (HEAD) spec introduces breaking or non-breaking changes.

When running on `pull_request` events, a comment will be added (or updated if exists) to the PR with a backward compatibility report and human-readable diff, giving PR authors and reviewers greater insight into the implications if merged.

When running on `pull_request` events, a label will also be added to the PR with the _classification_ (`major`, `minor`, or `patch`) of the diff.

## Usage

This action needs two OpenAPI spec files to compare in order to run. Your workflow may need to check out multiple branches of a repo or run additional steps to ensure that these files exist.

### Inputs:

- `head-spec` _(required)_: Local path or http URL to the new (HEAD) OpenAPI spec file. An error will be thrown if the file can't be found.
- `base-spec` _(required)_: Local path or http URL to the old (BASE) OpenAPI spec file. An error will be thrown if the file can't be found.
- `github-token` _(required)_: Must be in form `${{ github.token }}` or `${{ secrets.GITHUB_TOKEN }}`; This token is used to add labels and comments to pull requests. It is built into Github Actions and does not need to be manually specified in your secrets store. [More Info](https://help.github.com/en/actions/reference/context-and-expression-syntax-for-github-actions#github-context)

### Outputs:

- `classification`: A string representing the type of change detected between the two OpenAPI specs. Possible values: 
  - `major`: Indicates that the HEAD spec introduces at least one _incompatible_ or "breaking" changes
  - `minor`: Indicates that the HEAD spec introduces only _compatible_ or "non-breaking" changes
  - `patch`: Indicates that the HEAD spec does not introduce any changes at all

### Example:

This following example assumes that your repository contains a valid OpenAPI spec file called `openapi.yaml` in the repository root. 

```yaml
on: [pull_request]

name: openapi-diff

jobs:

  check:
    runs-on: ubuntu-latest
    steps:

    - name: Check out HEAD revision
      uses: actions/checkout@v2
      with:
        ref: ${{ github.head_ref }}
        path: head
    - name: Check out BASE revision
      uses: actions/checkout@v2
      with:
        ref: ${{ github.base_ref }}
        path: base
    - name: Run OpenAPI Diff (from HEAD revision)
      uses: evereepay/openapi-diff-action@v1.0.0-beta.1
      with:
        head-spec: head/openapi.yaml
        base-spec: base/openapi.yaml
        github-token: ${{ github.token }}
```
