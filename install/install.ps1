param (
    [string]$Version,
    [string]$FlowCtlRootPath = "$Env:SystemDrive\flowctl"
)

Write-Output ""
$ErrorActionPreference = 'stop'

#Escape space of FlowSynxRoot path
$FlowCtlRootPath = $FlowCtlRootPath -replace ' ', '` '

# Constants
$FlowCtlFileName = "flowctl.exe"
$FlowCtlFilePath = "${FlowCtlRootPath}\${$FlowCtlFileName}"

# GitHub Org and repo hosting FlowCtl
$GitHubOrg = "flowsynx"
$GitHubRepo = "flowctl"

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

# Check if FlowCtl is installed.
if (Test-Path $FlowCtlFilePath -PathType Leaf) {
    Write-Warning "FlowCtl is detected - $FlowCtlFilePath"
    Invoke-Expression "$FlowCtlFilePath version"
    Write-Output "Reinstalling FlowCtl..."
}
else {
    Write-Output "Installing FlowCtl..."
}

# Create FlowCtl Directory
Write-Output "Creating $FlowCtlRootPath directory"
New-Item -ErrorAction Ignore -Path $FlowCtlRootPath -ItemType "directory"
if (!(Test-Path $FlowCtlRootPath -PathType Container)) {
    Write-Warning "Please visit https://flowsynx.io/docs/getting-started/install-flowctl/ for instructions on how to install without admin rights."
    throw "Cannot create $FlowCtlRootPath"
}

# Get the list of release from GitHub
$releaseJsonUrl = "https://api.github.com/repos/${GitHubOrg}/${GitHubRepo}/releases"

$releases = Invoke-RestMethod -Headers $githubHeader -Uri $releaseJsonUrl -Method Get
if ($releases.Count -eq 0) {
    throw "No releases from github.com/flowsynx/flowctl repo"
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
    $windowsAsset = $Release | Select-Object -ExpandProperty assets | Where-Object { $_.name -Like "*windows-x64.zip" }
    if (!$windowsAsset) {
        throw "Cannot find the windows FlowCtl binary"
    }
    [hashtable]$return = @{}
    $return.url = $windowsAsset.url
    $return.name = $windowsAsset.name
    return $return

}

$release = GetVersionInfo -Version $Version -Releases $releases
if (!$release) {
    throw "Cannot find the specified FlowCtl binary version"
}
$asset = GetWindowsAsset -Release $release
$zipFileUrl = $asset.url
$assetName = $asset.name

$zipFilePath = $FlowCtlRootPath + "\" + $assetName
Write-Output "Downloading $zipFileUrl ..."

$githubHeader.Accept = "application/octet-stream"
$oldProgressPreference = $progressPreference;
$progressPreference = 'SilentlyContinue';
Invoke-WebRequest -Headers $githubHeader -Uri $zipFileUrl -OutFile $zipFilePath
$progressPreference = $oldProgressPreference;
if (!(Test-Path $zipFilePath -PathType Leaf)) {
    throw "Failed to download FlowCtl binary - $zipFilePath"
}

# Extract FlowCtl to $FlowCtlRootPath
Write-Output "Extracting $zipFilePath..."
Microsoft.Powershell.Archive\Expand-Archive -Force -Path $zipFilePath -DestinationPath $FlowCtlRootPath
if (!(Test-Path $FlowCtlFilePath -PathType Leaf)) {
    throw "Failed to download FlowCtl archive - $zipFilePath"
}

# Check the FlowCtl version
# Invoke-Expression "$FlowCtlFilePath version"

# Clean up zipfile
Write-Output "Clean up $zipFilePath..."
Remove-Item $zipFilePath -Force

# Add FlowCtlRootPath directory to User Path environment variable
Write-Output "Try to add $FlowCtlRootPath to User Path Environment variable..."
$UserPathEnvironmentVar = [Environment]::GetEnvironmentVariable("PATH", "User")
if ($UserPathEnvironmentVar -like '*flowctl*') {
    Write-Output "Skipping to add $FlowCtlRootPath to User Path - $UserPathEnvironmentVar"
}
else {
    [System.Environment]::SetEnvironmentVariable("PATH", $UserPathEnvironmentVar + ";$FlowCtlRootPath", "User")
    $UserPathEnvironmentVar = [Environment]::GetEnvironmentVariable("PATH", "User")
    Write-Output "Added $FlowCtlRootPath to User Path - $UserPathEnvironmentVar"
}

Write-Output "`r`nFlowCtl is installed successfully."
Write-Output "To get started with FlowCtl, please visit https://flowsynx.io/docs/category/getting-started ."