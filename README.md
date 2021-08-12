# OpenAPI Diff Action

Based on [LimeFlight/openapi-diff](https://github.com/LimeFlight/openapi-diff).

This GitHub Action compares two OpenAPI (3.x) specs to determine if the newer (HEAD) spec introduces breaking or non-breaking changes.

When running on `pull_request` events, a comment will be added (or updated if exists) to the PR with a backward compatibility report and human-readable diff, giving PR authors and reviewers greater insight into the implications if merged.

When running on `pull_request` events, a label will also be added to the PR with the _classification_ (`major`, `minor`, or `patch`) of the diff.

## Usage

This action needs two OpenAPI spec files to compare in order to run. Your workflow may need to check out multiple branches of a repo or run additional steps to ensure that these files exist.

### Inputs:

- `head-spec` _(required)_: Local path to the new (HEAD) OpenAPI spec file. An error will be thrown if the file can't be found.
- `base-spec` _(required)_: Local path to the old (BASE) OpenAPI spec file. An error will be thrown if the file can't be found.
- `output-path` _(required)_: Local path to store the output file.
- `github-token` _(required)_: Must be in form `${{ github.token }}` or `${{ secrets.GITHUB_TOKEN }}`; This token is used to add labels and comments to pull requests. It is built into Github Actions and does not need to be manually specified in your secrets store. [More Info](https://help.github.com/en/actions/reference/context-and-expression-syntax-for-github-actions#github-context)

### Example:

This following example assumes that your repository contains a valid OpenAPI spec file called `openapi.json` in the repository root.

```yaml
on: [pull_request]

name: openapi-diff

jobs:
  openapi-compatiable:
    strategy:
      max-parallel: 1
      fail-fast: false
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
        uses: LimeFlight/openapi-diff-action@master
        with:
          head-spec: head/openapi.json
          base-spec: base/openapi.json
          output-path: ./output
          github-token: ${{ github.token }}
      - uses: actions/upload-artifact@v2
        with:
          name: diff-reports
          path: ./output
```
