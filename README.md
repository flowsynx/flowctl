# FlowSynx CLI

[![License: MIT][mit-badge]][mit-url] [![Build Status][actions-badge]][actions-url] [![FOSSA Status][fossa-badge]][fossa-url]

[mit-badge]: https://img.shields.io/github/license/flowsynx/cli?style=flat&label=License&logo=github
[mit-url]: https://github.com/flowsynx/cli/blob/master/LICENSE
[actions-badge]: https://github.com/flowsynx/cli/actions/workflows/cli-release.yml/badge.svg?event=push&branch=master
[actions-url]: https://github.com/flowsynx/cli/actions?workflow=cli
[fossa-badge]: https://app.fossa.com/api/projects/git%2Bgithub.com%2Fflowsynx%2Fcli.svg?type=shield&issueType=license
[fossa-url]: https://app.fossa.com/projects/git%2Bgithub.com%2Fflowsynx%2Fcli?ref=badge_shield&issueType=license

The FlowSynx CLI allows you to setup FlowSynx on your local dev machine, manage FlowSynx on local or remote instance.

## Getting started
### Prerequisites
On default, during initialization the FlowSynx CLI will install the FlowSynx binaries as well as setup an environment to help you get started easily with FlowSynx.

### Installing FlowSynx CLI
Using script to install the latest release

#### Windows
Install the latest windows FlowSynx CLI to $Env:SystemDrive\flowsynx and add this directory to User PATH environment variable.

```
powershell -Command "iwr -useb https://raw.githubusercontent.com/flowsynx/cli/master/install/install.ps1 | iex"
```

#### Linux
Install the latest linux FlowSynx CLI to /usr/local/bin

```
wget -q https://raw.githubusercontent.com/flowsynx/cli/master/install/install.sh -O - | /bin/bash
```

#### MacOS
Install the latest macOS FlowSynx CLI to /usr/local/bin

```
curl -fsSL https://raw.githubusercontent.com/flowsynx/cli/master/install/install.sh | /bin/bash
```

#### From the Binary Releases
Each release of FlowSynx CLI includes various OSes and architectures. These binary versions can be manually downloaded and installed.

1. Download the [FlowSynx CLI](https://github.com/flowsynx/cli/releases)
2. Unpack it (e.g. flowsynx-linux-amd64.tar.gz, flowsynx-windows-amd64.zip)
3. Move it to your desired location.
	- For Linux/MacOS - /usr/local/bin
	- For Windows, create a directory and add this to your System PATH. For example create a directory called c:\flowsynx and add this directory to your path, by editing your system environment variable.

### Install FlowSynx as standalone mode
In standalone mode, flowsynx can be initialized using the CLI.

#### Initialize Dapr
```
flowsynx init
```

#### Install a specific engine version
You can install or upgrade to a specific version of the FlowSynx engine using `flowsynx init --engine-version`. 
You can find the list of versions in [FlowSynx Release](https://github.com/flowsynx/flowsynx/releases).

```
# Install v0.2.0 engine
flowsynx init --engine-version 0.2.0

# Check the versions of CLI and engine
flowsynx version --full
CLI version: v0.2.0
Engine version: v0.2.0
```

### Uninstall FlowSynx in a standalone mode
Uninstalling will remove flowsynxe binary along with the dashboard.
```
flowsynx uninstall
```
The command above will remove the default flowsynx folder that was created on `flowsynx init`.