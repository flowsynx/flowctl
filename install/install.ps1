param (
    [string]$Version,
    [string]$FlowSynxRoot = "$Env:SystemDrive\flowsynx",
    [string]$FlowSynxReleaseJsonUrl = "",
    [scriptblock]$CustomAssetFactory = $null
)

Write-Output ""
$ErrorActionPreference = 'stop'

#Escape space of FlowSynxRoot path
$FlowSynxRoot = $FlowSynxRoot -replace ' ', '` '

# Constants
$FlowSynxCliFileName = "synx.exe"
$FlowSynxCliFilePath = "${FlowSynxRoot}\${FlowSynxCliFileName}"

# GitHub Org and repo hosting FlowSynx CLI
$GitHubOrg = "FlowSynx"
$GitHubRepo = "Cli"

# Set Github request authentication for basic authentication.
if ($Env:GITHUB_USER) {
    $basicAuth = [System.Convert]::ToBase64String([System.Text.Encoding]::ASCII.GetBytes($Env:GITHUB_USER + ":" + $Env:GITHUB_TOKEN));
    $githubHeader = @{"Authorization" = "Basic $basicAuth" }
}
else {
    $githubHeader = @{}
}

if ((Get-ExecutionPolicy) -gt 'RemoteSigned' -or (Get-ExecutionPolicy) -eq 'ByPass') {
    Write-Output "PowerShell requires an execution policy of 'RemoteSigned'."
    Write-Output "To make this change please run:"
    Write-Output "'Set-ExecutionPolicy RemoteSigned -scope CurrentUser'"
    break
}

# Change security protocol to support TLS 1.2 / 1.1 / 1.0 - old powershell uses TLS 1.0 as a default protocol
[Net.ServicePointManager]::SecurityProtocol = "tls12, tls11, tls"

# Check if FlowSynx CLI is installed.
if (Test-Path $FlowSynxCliFilePath -PathType Leaf) {
    Write-Warning "FlowSynx is detected - $FlowSynxCliFilePath"
    Invoke-Expression "$FlowSynxCliFilePath version"
    Write-Output "Reinstalling FlowSynx..."
}
else {
    Write-Output "Installing FlowSynx..."
}

# Create FlowSynx Directory
Write-Output "Creating $FlowSynxRoot directory"
New-Item -ErrorAction Ignore -Path $FlowSynxRoot -ItemType "directory"
if (!(Test-Path $FlowSynxRoot -PathType Container)) {
    Write-Warning "Please visit http://flowsynx.io/docs/getting-started/install-flowsynx-cli/ for instructions on how to install without admin rights."
    throw "Cannot create $FlowSynxRoot"
}

# Get the list of release from GitHub
$releaseJsonUrl = $FlowSynxReleaseJsonUrl
if (!$releaseJsonUrl) {
    $releaseJsonUrl = "https://api.github.com/repos/${GitHubOrg}/${GitHubRepo}/releases"
}

$releases = Invoke-RestMethod -Headers $githubHeader -Uri $releaseJsonUrl -Method Get
if ($releases.Count -eq 0) {
    throw "No releases from github.com/FlowSynx/Cli repo"
}

# Get latest or specified version info from releases
function GetVersionInfo {
    param (
        [string]$Version,
        $Releases
    )
    # Filter windows binary and download archive
    if (!$Version) {
        $release = $Releases | Where-Object { $_.tag_name -notlike "*rc*" } | Select-Object -First 1
    }
    else {
        $release = $Releases | Where-Object { $_.tag_name -eq "v$Version" } | Select-Object -First 1
    }

    return $release
}

# Get info about windows asset from release
function GetWindowsAsset {
    param (
        $Release
    )
    if ($CustomAssetFactory) {
        Write-Output "CustomAssetFactory dectected, try to invoke it"
        return $CustomAssetFactory.Invoke($Release)
    }
    else {
        $windowsAsset = $Release | Select-Object -ExpandProperty assets | Where-Object { $_.name -Like "*windows-x64.zip" }
        if (!$windowsAsset) {
            throw "Cannot find the windows FlowSynx CLI binary"
        }
        [hashtable]$return = @{}
        $return.url = $windowsAsset.url
        $return.name = $windowsAsset.name
        return $return
    }`
}

$release = GetVersionInfo -Version $Version -Releases $releases
if (!$release) {
    throw "Cannot find the specified FlowSynx CLI binary version"
}
$asset = GetWindowsAsset -Release $release
$zipFileUrl = $asset.url
$assetName = $asset.name

$zipFilePath = $FlowSynxRoot + "\" + $assetName
Write-Output "Downloading $zipFileUrl ..."

$githubHeader.Accept = "application/octet-stream"
$oldProgressPreference = $progressPreference;
$progressPreference = 'SilentlyContinue';
Invoke-WebRequest -Headers $githubHeader -Uri $zipFileUrl -OutFile $zipFilePath
$progressPreference = $oldProgressPreference;
if (!(Test-Path $zipFilePath -PathType Leaf)) {
    throw "Failed to download FlowSynx Cli binary - $zipFilePath"
}

# Extract FlowSynx CLI to $FlowSynxRoot
Write-Output "Extracting $zipFilePath..."
Microsoft.Powershell.Archive\Expand-Archive -Force -Path $zipFilePath -DestinationPath $FlowSynxRoot
if (!(Test-Path $FlowSynxCliFilePath -PathType Leaf)) {
    throw "Failed to download FlowSynx Cli archive - $zipFilePath"
}

# Check the FlowSynx CLI version
# Invoke-Expression "$FlowSynxCliFilePath version"

# Clean up zipfile
Write-Output "Clean up $zipFilePath..."
Remove-Item $zipFilePath -Force

# Add FlowSynxRoot directory to User Path environment variable
Write-Output "Try to add $FlowSynxRoot to User Path Environment variable..."
$UserPathEnvironmentVar = [Environment]::GetEnvironmentVariable("PATH", "User")
if ($UserPathEnvironmentVar -like '*flowsynx*') {
    Write-Output "Skipping to add $FlowSynxRoot to User Path - $UserPathEnvironmentVar"
}
else {
    [System.Environment]::SetEnvironmentVariable("PATH", $UserPathEnvironmentVar + ";$FlowSynxRoot", "User")
    $UserPathEnvironmentVar = [Environment]::GetEnvironmentVariable("PATH", "User")
    Write-Output "Added $FlowSynxRoot to User Path - $UserPathEnvironmentVar"
}

Write-Output "`r`nFlowSynx CLI is installed successfully."
Write-Output "To get started with FlowSynx, please visit http://flowsynx.io/docs/getting-started/ ."
Write-Output "Ensure that Docker Desktop is set to Linux containers mode when you run FlowSynx in self hosted mode."