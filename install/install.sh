#!/usr/bin/env bash

# FlowSynx CLI location
: ${FLOWSYNX_INSTALL_DIR:="/usr/local/bin"}

# sudo is required to copy binary to FLOWSYNX_INSTALL_DIR for linux
: ${USE_SUDO:="false"}

# Http request CLI
FLOWSYNX_HTTP_REQUEST_CLI=curl

# GitHub Organization and repo name to download release
GITHUB_ORG=FlowSynx
GITHUB_REPO=Cli

# FlowSynx CLI filename
FLOWSYNX_CLI_FILENAME=synx

FLOWSYNX_CLI_FILE="${FLOWSYNX_INSTALL_DIR}/${FLOWSYNX_CLI_FILENAME}"

getSystemInfo() {
    ARCH=$(uname -m)
    case $ARCH in
        armv7*) ARCH="arm";;
        aarch64) ARCH="arm64";;
        x86_64) ARCH="x64";;
    esac

    OS=$(echo `uname`|tr '[:upper:]' '[:lower:]')

    # Most linux distro needs root permission to copy the file to /usr/local/bin
    if [[ "$OS" == "linux" || "$OS" == "darwin" ]] && [ "FLOWSYNX_INSTALL_DIR" == "/usr/local/bin" ]; then
        USE_SUDO="true"
    fi
}

verifySupported() {
    releaseTag=$1
    local supported=(darwin-x64 linux-x64 linux-arm linux-arm64)
    local current_osarch="${OS}-${ARCH}"

    for osarch in "${supported[@]}"; do
        if [ "$osarch" == "$current_osarch" ]; then
            echo "Your system is ${OS}_${ARCH}"
            return
        fi
    done

    if [ "$current_osarch" == "darwin-arm64" ]; then
        if isReleaseAvailable $releaseTag; then
            return
        else
            echo "The darwin_arm64 arch has no native binary for this version of FlowSynx, however you can use the amd64 version so long as you have rosetta installed"
            echo "Use 'softwareupdate --install-rosetta' to install rosetta if you don't already have it"
            ARCH="x64"
            return
        fi
    fi

    echo "No prebuilt binary for ${current_osarch}"
    exit 1
}

runAsRoot() {
    local CMD="$*"

    if [ $EUID -ne 0 -a $USE_SUDO = "true" ]; then
        CMD="sudo $CMD"
    fi

    $CMD || {
        echo "Please visit http://flowsynx.io/docs/getting-started/install-flowsynx-cli/ for instructions on how to install without sudo."
        exit 1
    }
}

checkHttpRequestCLI() {
    if type "curl" > /dev/null; then
        FLOWSYNX_HTTP_REQUEST_CLI=curl
    elif type "wget" > /dev/null; then
        FLOWSYNX_HTTP_REQUEST_CLI=wget
    else
        echo "Either curl or wget is required"
        exit 1
    fi
}

checkExistingFlowSynx() {
    if [ -f "$FLOWSYNX_CLI_FILE" ]; then
        echo -e "\nFlowSynx CLI is detected:"
        $FLOWSYNX_CLI_FILE version
        echo -e "Reinstalling FlowSynx CLI - ${FLOWSYNX_CLI_FILE}...\n"
    else
        echo -e "Installing FlowSynx CLI...\n"
    fi
}

getLatestRelease() {
    local flowsynxReleaseUrl="https://api.github.com/repos/${GITHUB_ORG}/${GITHUB_REPO}/releases"
    local latest_release=""

    if [ "$FLOWSYNX_HTTP_REQUEST_CLI" == "curl" ]; then
        latest_release=$(curl -s $flowsynxReleaseUrl | grep \"tag_name\" | grep -v rc | awk 'NR==1{print $2}' |  sed -n 's/\"\(.*\)\",/\1/p')
    else
        latest_release=$(wget -q --header="Accept: application/json" -O - $flowsynxReleaseUrl | grep \"tag_name\" | grep -v rc | awk 'NR==1{print $2}' |  sed -n 's/\"\(.*\)\",/\1/p')
    fi

    ret_val=$latest_release
}

downloadFile() {
    LATEST_RELEASE_TAG=$1

    FLOWSYNX_CLI_ARTIFACT="${FLOWSYNX_CLI_FILENAME}-${OS}-${ARCH}.tar.gz"
    DOWNLOAD_BASE="https://github.com/${GITHUB_ORG}/${GITHUB_REPO}/releases/download"
    DOWNLOAD_URL="${DOWNLOAD_BASE}/${LATEST_RELEASE_TAG}/${FLOWSYNX_CLI_ARTIFACT}"

    # Create the temp directory
    FLOWSYNX_TMP_ROOT=$(mktemp -dt flowsynx-install-XXXXXX)
    ARTIFACT_TMP_FILE="$FLOWSYNX_TMP_ROOT/$FLOWSYNX_CLI_ARTIFACT"

    echo "Downloading $DOWNLOAD_URL ..."
    if [ "$FLOWSYNX_HTTP_REQUEST_CLI" == "curl" ]; then
        curl -SsL "$DOWNLOAD_URL" -o "$ARTIFACT_TMP_FILE"
    else
        wget -q -O "$ARTIFACT_TMP_FILE" "$DOWNLOAD_URL"
    fi

    if [ ! -f "$ARTIFACT_TMP_FILE" ]; then
        echo "failed to download $DOWNLOAD_URL ..."
        exit 1
    fi
}

isReleaseAvailable() {
    LATEST_RELEASE_TAG=$1

    FLOWSYNX_CLI_ARTIFACT="${FLOWSYNX_CLI_FILENAME}-${OS}-${ARCH}.tar.gz"
    DOWNLOAD_BASE="https://github.com/${GITHUB_ORG}/${GITHUB_REPO}/releases/download"
    DOWNLOAD_URL="${DOWNLOAD_BASE}/${LATEST_RELEASE_TAG}/${FLOWSYNX_CLI_ARTIFACT}"

    if [ "$FLOWSYNX_HTTP_REQUEST_CLI" == "curl" ]; then
        httpstatus=$(curl -sSLI -o /dev/null -w "%{http_code}" "$DOWNLOAD_URL")
        if [ "$httpstatus" == "200" ]; then
            return 0
        fi
    else
        wget -q --spider "$DOWNLOAD_URL"
        exitstatus=$?
        if [ $exitstatus -eq 0 ]; then
            return 0
        fi
    fi
    return 1
}

installFile() {
    tar xf "$ARTIFACT_TMP_FILE" -C "$FLOWSYNX_TMP_ROOT"
    local tmp_root_flowsynx_cli="$FLOWSYNX_TMP_ROOT/$FLOWSYNX_CLI_FILENAME"

    if [ ! -f "$tmp_root_flowsynx_cli" ]; then
        echo "Failed to unpack FlowSynx CLI executable."
        exit 1
    fi

    if [ -f "$FLOWSYNX_CLI_FILE" ]; then
        runAsRoot rm "$FLOWSYNX_CLI_FILE"
    fi
    chmod o+x $tmp_root_flowsynx_cli
    mkdir -p $FLOWSYNX_INSTALL_DIR
    runAsRoot cp "$tmp_root_flowsynx_cli" "$FLOWSYNX_INSTALL_DIR"

    if [ -f "$FLOWSYNX_CLI_FILE" ]; then
        echo "$FLOWSYNX_CLI_FILENAME installed into $FLOWSYNX_INSTALL_DIR successfully."

        $FLOWSYNX_CLI_FILE version
    else 
        echo "Failed to install $FLOWSYNX_CLI_FILENAME"
        exit 1
    fi
}

fail_trap() {
    result=$?
    if [ "$result" != "0" ]; then
        echo "Failed to install FlowSynx CLI"
        echo "For support, go to http://flowsynx.io"
    fi
    cleanup
    exit $result
}

cleanup() {
    if [[ -d "${FLOWSYNX_TMP_ROOT:-}" ]]; then
        rm -rf "$FLOWSYNX_TMP_ROOT"
    fi
}

installCompleted() {
    echo -e "\nTo get started with FlowSynx, please visit http://flowsynx.io/docs/getting-started/"
}

# -----------------------------------------------------------------------------
# main
# -----------------------------------------------------------------------------
trap "fail_trap" EXIT

getSystemInfo
checkHttpRequestCLI

if [ -z "$1" ]; then
    echo "Getting the latest FlowSynx CLI..."
    getLatestRelease
else
    ret_val=v$1
fi

verifySupported $ret_val
checkExistingFlowSynx

echo "Installing $ret_val FlowSynx CLI..."

downloadFile $ret_val
installFile
cleanup

installCompleted