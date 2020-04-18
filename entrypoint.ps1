#!/usr/bin/pwsh

[CmdletBinding()]
param (
    [Parameter(Mandatory = $true, Position = 0)]
    [string]
    $GitHubToken,
    [Parameter(Mandatory = $true, Position = 1)]
    [int]
    $PrNumber,
    [Parameter(Mandatory = $true, Position = 2)]
    [string]
    $OldFile,
    [Parameter(Mandatory = $true, Position = 2)]
    [string]
    $NewFile,
    [Parameter(Mandatory = $false, Position = 3)]
    [bool]
    $AddComment    
)

# Install openapi-diff-action from nuget
dotnet tool install --global yaos.OpenAPI.Diff.Action --version 1.0.0-alpha

# Run openapi-diff-action with args from github action
openapi-diff-action $GitHubToken $GITHUB_REPOSITORY $PrNumber $OldFile $NewFile $AddComment