#!/usr/bin/pwsh

$NewFile = $INPUT_HEAD-SPEC 
$OldFile =$INPUT_BASE-SPEC 
$GitHubToken = $INPUT_GITHUB-TOKEN 
$AddComment = [boolean]$INPUT_ADD-COMMENT

Write-Host "Starting"

Write-Host $NewFile
Write-Host $OldFile
Write-Host $GitHubToken
Write-Host $AddComment
Write-Host $GITHUB_EVENT_PATH

$ActionEvent = ConvertFrom-Json $GITHUB_EVENT_PATH
Write-Host $ActionEvent 

$PullRequest = $ActionEvent.pull_request.number
Write-Host $PullRequest 

# Install openapi-diff-action from nuget
dotnet tool install --global yaos.OpenAPI.Diff.Action --version 1.0.0-alpha

# Run openapi-diff-action with args from github action
openapi-diff-action $GitHubToken $GITHUB_REPOSITORY $PullRequest $OldFile $NewFile $AddComment