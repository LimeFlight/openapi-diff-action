#!/usr/bin/pwsh

## Load up some common functionality for interacting
## with the GitHub Actions/Workflow environment
# . lib/ActionsCore.ps1 or ./lib/ActionsCore.ps1 -- not working for some reason

## Used to identify inputs from env vars in Action/Workflow context
if (-not (Get-Variable -Scope Script -Name INPUT_PREFIX -ErrorAction SilentlyContinue)) {
    Set-Variable -Scope Script -Option Constant -Name INPUT_PREFIX -Value 'INPUT_'
}

<#
.SYNOPSIS
Gets the value of an input.  The value is also trimmed.
.PARAMETER Name
Name of the input to get
.PARAMETER Required
Whether the input is required. If required and not present, will throw.
#>
function Get-ActionInput {
    param(
        [Parameter(Position = 0, Mandatory)]
        [string]$Name,
        [switch]$Required
    )
    
    $cleanName = ($Name -replace ' ', '_').ToUpper()
    $inputValue = Get-ChildItem "Env:$($INPUT_PREFIX)$($cleanName)" -ErrorAction SilentlyContinue
    if ($Required -and (-not $inputValue)) {
        throw "Input required and not supplied: $($Name)"
    }

    return "$($inputValue.Value)".Trim()
}

<#
.SYNOPSIS
Gets the value of an input.  The value is also trimmed.
.PARAMETER Name
Name of the input to get
.PARAMETER Required
Whether the input is required. If required and not present, will throw.
#>
function Get-EnvironmentInput {
    param(
        [Parameter(Position = 0, Mandatory)]
        [string]$Name,
        [switch]$Required
    )
    
    $cleanName = ($Name -replace ' ', '_').ToUpper()
    $inputValue = Get-ChildItem "Env:$($cleanName)" -ErrorAction SilentlyContinue
    if ($Required -and (-not $inputValue)) {
        throw "Env Input required and not supplied: $($Name)"
    }

    return "$($inputValue.Value)".Trim()
}

## Pull in some inputs
$NewFile = Get-ActionInput head_spec -Required
$OldFile = Get-ActionInput base_spec -Required
$GitHubToken = Get-ActionInput github_token -Required
$GitHubEventPath = Get-EnvironmentInput github_event_path -Required
$GitHubRepository = Get-EnvironmentInput github_repository -Required
$AddCommentStr = Get-ActionInput add_comment
$exludeLabelsStr = Get-ActionInput exlude_labels

try {
    $AddComment = [System.Convert]::ToBoolean($AddCommentStr) 
}
catch [FormatException] {
    $AddComment = $false
}
try {
    $exludeLabels = [System.Convert]::ToBoolean($exludeLabelsStr) 
}
catch [FormatException] {
    $exludeLabels = $false
}

Write-Output "Starting"

Write-Output "HEAD-SPEC = $($NewFile)"
Write-Output "BASE-SPEC = $($OldFile)"
Write-Output "GITHUB_TOKEN = $($GitHubToken)" 
Write-Output "ADD_COMMENT = $($AddComment)"
Write-Output "EXCLUDE_LABELS = $($exludeLabels)"
Write-Output "GITHUB_REPOSITORY = $($GitHubRepository)"
Write-Output "GITHUB_EVENT_PATH = $($GitHubEventPath)"

$ActionEvent = Get-Content -Raw -Path $GitHubEventPath | ConvertFrom-Json
Write-Output "EVENT JSON = $($ActionEvent)"

$PullRequest = $ActionEvent.pull_request.number
Write-Output "PULL REQUEST ID = $($PullRequest)"

# Install openapi-diff-action from nuget
dotnet new tool-manifest
dotnet tool install yaos.OpenAPI.Diff.Action --version 1.1.0

# Run openapi-diff-action with args from github action
dotnet tool run openapi-diff-action $GitHubToken $GitHubRepository $PullRequest $OldFile $NewFile $AddComment $exludeLabels
if ($LastExitCode -eq 0) {
    Write-Host "Execution succeeded"
}
else {
    Write-Host "Execution Failed"
    exit $LastExitCode
}