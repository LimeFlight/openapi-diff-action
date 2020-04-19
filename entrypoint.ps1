#!/usr/bin/pwsh

$NewFile =  $env:$INPUT_HEAD_SPEC 
$OldFile = $env:$INPUT_BASE_SPEC 
$GitHubToken = $env:$INPUT_GITHUB_TOKEN 
$AddComment = [boolean] $env:$INPUT_ADD_COMMENT

Write-Host "Starting"

Write-Host $NewFile
Write-Host $OldFile
Write-Host $GitHubToken
Write-Host $AddComment
Write-Host $env:$GITHUB_EVENT_PATH

$ActionEvent = ConvertFrom-Json $env:$GITHUB_EVENT_PATH
Write-Host $ActionEvent 

$PullRequest = $ActionEvent.pull_request.number
Write-Host $PullRequest 

# Install openapi-diff-action from nuget
dotnet tool install --global yaos.OpenAPI.Diff.Action --version 1.0.0-alpha

# Run openapi-diff-action with args from github action
openapi-diff-action $GitHubToken $env:$GITHUB_REPOSITORY $PullRequest $OldFile $NewFile $AddComment