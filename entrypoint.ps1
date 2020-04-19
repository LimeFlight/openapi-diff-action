#!/usr/bin/pwsh

## Load up some common functionality for interacting
## with the GitHub Actions/Workflow environment
# with the GitHub Actions/Workflow environment
. lib/ActionsCore.ps1

## Pull in some inputs
$NewFile = Get-ActionInput head_spec -Required
$OldFile   = Get-ActionInput base_spec -Required
$GitHubToken =  Get-ActionInput github_token -Required
$GitHubEventPath = Get-EnvironmentInput github_event_path -Required
$GitHubRepository = Get-EnvironmentInput github_repository -Required
$AddCommentStr = Get-ActionInput add_comment

try {
  $AddComment = [System.Convert]::ToBoolean($AddCommentStr) 
} catch [FormatException] {
  $AddComment = $false
}

Write-Output "Starting"

Write-Output "HEAD-SPEC = $($NewFile)"
Write-Output "BASE-SPEC = $($OldFile)"
Write-Output "GITHUB_TOKEN = $($GitHubToken)" 
Write-Output "ADD_COMMENT = $($AddComment)"
Write-Output "GITHUB_REPOSITORY = $($GitHubRepository)"
Write-Output "GITHUB_EVENT_PATH = $($GitHubEventPath)"

$ActionEvent = Get-Content -Raw -Path $GitHubEventPath | ConvertFrom-Json
Write-Output "EVENT JSON = $($ActionEvent)"

$PullRequest = $ActionEvent.pull_request.number
Write-Output "PULL REQUEST ID = $($PullRequest)"

# Install openapi-diff-action from nuget
## TODO: INSTALL dotnet
dotnet tool install --global yaos.OpenAPI.Diff.Action --version 1.0.0-alpha

# Run openapi-diff-action with args from github action
openapi-diff-action $GitHubToken $GitHubRepository $PullRequest $OldFile $NewFile $AddComment