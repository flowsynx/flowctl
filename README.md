# FlowCtl

[![dotnet][dotnet-budge]][dotnet-url] [![License: MIT][mit-badge]][mit-url] [![Build Status][actions-badge]][actions-url] [![FOSSA Status][fossa-badge]][fossa-url]

[mit-badge]: https://img.shields.io/github/license/flowsynx/flowctl?style=flat&label=License&logo=github
[mit-url]: https://github.com/flowsynx/flowctl/blob/master/LICENSE
[actions-badge]: https://github.com/flowsynx/flowctl/actions/workflows/flowctl-release.yml/badge.svg?branch=master
[actions-url]: https://github.com/flowsynx/flowctl/actions?workflow=flowctl
[fossa-badge]: https://app.fossa.com/api/projects/git%2Bgithub.com%2Fflowsynx%2Fcli.svg?type=shield&issueType=license
[fossa-url]: https://app.fossa.com/projects/git%2Bgithub.com%2Fflowsynx%2Fcli?ref=badge_shield&issueType=license
[dotnet-budge]: https://img.shields.io/badge/.NET-9.0-blue
[dotnet-url]: https://dotnet.microsoft.com/en-us/download/dotnet/9.0

**flowctl** is a powerful command-line interface (CLI) tool written in C# for controlling and managing a FlowSynx workflow automation system. 
Designed for developers and operations teams, flowctl enables seamless integration, orchestration, and lifecycle management of workflows 
whether you're working in the cloud, on-premises, or in hybrid environments.

---

## ✨ Features
- 🔧 **Workflow Lifecycle Management**: Create, update, validate, delete, and execute workflows.
- 📦 **Plugin Support**: Manage plugins and integrations for workflow tasks.
- 📊 **Monitoring & Status**: Query real-time execution status and audit logs.
- 🔐 **Authentication**: Supports Basic and JWT tokens-based authentication.
- ⚡ **Cross-Platform**: Runs on Windows, Linux, and macOS (.NET 9+ required).

---

## 📦 Installation

### 🚀 Installing FlowCtl
You can install FlowCtl, the CLI for FlowSynx workflow automation system, using a platform-specific script or by downloading binaries manually from the Releases page.

#### 🪟 Windows
Use the following PowerShell script to install FlowCtl to $Env:SystemDrive\flowctl and automatically add this directory to your User PATH:

```
powershell -Command "iwr -useb https://raw.githubusercontent.com/flowsynx/flowctl/master/install/install.ps1 | iex"
```

After installation, restart your terminal or run refreshenv (if using tools like Chocolatey) to ensure FlowCtl is available in your PATH.

#### 🐧 Linux
Install FlowCtl to /usr/local/bin, making it accessible system-wide:

```
wget -q https://raw.githubusercontent.com/flowsynx/flowctl/master/install/install.sh -O - | /bin/bash
```

This script automatically detects your architecture and places the flowctl binary in /usr/local/bin.

#### 🍎 macOS
Install FlowCtl to /usr/local/bin using curl:

```
curl -fsSL https://raw.githubusercontent.com/flowsynx/flowctl/master/install/install.sh | /bin/bash
```

Ensure /usr/local/bin is in your shell's PATH. You can check this by running:
```
echo $PATH
```

#### 📦 Manual Installation from Binary Releases
If you prefer not to use the install scripts, you can manually download and set up FlowCtl:

1. Visit the [Releases page](https://github.com/flowsynx/flowctl/releases).
2. Download the appropriate archive for your OS and architecture (e.g., flowsynx-linux-amd64.tar.gz, flowsynx-windows-amd64.zip, etc.).
3. Unpack the archive.
4. Move the flowctl binary to a directory in your PATH:
	- Linux/macOS: Move to /usr/local/bin
	```
	sudo mv flowctl /usr/local/bin/
	chmod +x /usr/local/bin/flowctl
	```
	- Windows
		- Create a directory like C:\flowctl
		- Move flowctl.exe into it
		- Add C:\flowctl to the System or User PATH via Environment Variables settings

	- Confirm installation by running:
	```
	flowctl --version
	```

### 🧰 Initialize FlowSynx in Standalone Mode
In standalone mode, FlowSynx can operate locally with minimal dependencies. You can initialize FlowSynx using FlowCtl with a simple command:
```
flowctl init
```

This command bootstraps a local workflow environment, creating necessary configuration files and directory structures.

#### Install a specific engine version
You can install or upgrade to a specific version of the **FlowSynx engine** by using the `--flowsynx-version` flag with the `flowctl init` command.
Available versions can be found on the [FlowSynx Releases](https://github.com/flowsynx/flowsynx/releases) page.

Example command to initialize a specific FlowSynx version:

```bash
flowctl init --flowsynx-version 1.2.3
```

> Replace `1.2.3` with the desired version number.
> This command ensures that the specified version of the FlowSynx engine is downloaded, installed, and properly configured for use.

#### Check the versions of flowctl and flowsynx engine
To verify the installed versions of both FlowCtl and the FlowSynx engine, run the following command:
```
flowctl version
```

This will output version information in JSON format, similar to the example below:
```
{
  "FlowCtl": "1.0.0",
  "FlowSynx": "1.0.0.0"
}
```

> This command is useful for confirming compatibility and ensuring you're using the intended versions.

### 🔄 Uninstalling FlowSynx (Standalone Mode)
To uninstall FlowSynx in standalone mode, run the following command:
```
flowctl uninstall
```
This will remove the FlowSynx engine binary, and the default installation directory that was created during flowctl init.
> ⚠️ This operation is irreversible and will delete all local FlowSynx files associated with the standalone setup.