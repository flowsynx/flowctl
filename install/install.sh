#!/usr/bin/env bash

# FlowCtl location
: ${FLOWCTL_INSTALL_DIR:="/usr/local/bin"}

# sudo is required to copy binary to FLOWCTL_INSTALL_DIR for linux
: ${USE_SUDO:="false"}

# Http request flowctl
HTTP_REQUEST_FLOWCTL=curl

# GitHub Organization and repo name to download release
GITHUB_ORG=flowsynx
GITHUB_REPO=flowctl

# FlowCtl filename
FLOWCTL_FILENAME=flowctl

FLOWCTL_FILE="${FLOWCTL_INSTALL_DIR}/${FLOWCTL_FILENAME}"

getSystemInfo() {
    ARCH=$(uname -m)
    case $ARCH in
        armv7*) ARCH="arm";;
        aarch64) ARCH="arm64";;
        x86_64) ARCH="x64";;
    esac

    OS=$(echo `uname`|tr '[:upper:]' '[:lower:]')

    # Most linux distro needs root permission to copy the file to /usr/local/bin
    if [[ "$OS" == "linux" || "$OS" == "darwin" ]] && [[ "FLOWCTL_INSTALL_DIR" == "/usr/local/bin" ]]; then
        USE_SUDO="true"
    fi
}

verifySupported() {
    releaseTag=$1
    local supported=(darwin-x64 linux-x64 linux-arm linux-arm64)
    local current_osarch="${OS}-${ARCH}"

    for osarch in "${supported[@]}"; do
        if [[ "$osarch" == "$current_osarch" ]]; then
            echo "Your system is ${OS}_${ARCH}"
            return
        fi
    done

    if [[ "$current_osarch" == "darwin-arm64" ]]; then
        if isReleaseAvailable $releaseTag; then
            return
        else
            echo "The darwin_arm64 arch has no native binary for this version of FlowCtl, however you can use the amd64 version so long as you have rosetta installed"
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

    if [[ $EUID -ne 0 -a $USE_SUDO = "true" ]]; then
        CMD="sudo $CMD"
    fi

    $CMD || {
        echo "Please visit https://flowsynx.io/docs/getting-started/flowctl-based-installation/ for instructions on how to install without sudo."
        exit 1
    }
}

checkHttpRequestFlowCtl() {
    if type "curl" > /dev/null; then
        HTTP_REQUEST_FLOWCTL=curl
    elif type "wget" > /dev/null; then
        HTTP_REQUEST_FLOWCTL=wget
    else
        echo "Either curl or wget is required"
        exit 1
    fi
}

checkExistingFlowCtl() {
    if [[ -f "$FLOWCTL_FILE" ]]; then
        echo -e "\nFlowCtl is detected:"
        $FLOWCTL_FILE version
        echo -e "Reinstalling FlowCtl - ${FLOWCTL_FILE}...\n"
    else
        echo -e "Installing FlowCtl...\n"
    fi
}

getLatestRelease() {
    local flowctlReleaseUrl="https://api.github.com/repos/${GITHUB_ORG}/${GITHUB_REPO}/releases"
    local latest_release=""

    if [[ "$HTTP_REQUEST_FLOWCTL" == "curl" ]]; then
        latest_release=$(curl -s $flowctlReleaseUrl | grep \"tag_name\" | grep -v rc | awk 'NR==1{print $2}' |  sed -n 's/\"\(.*\)\",/\1/p')
    else
        latest_release=$(wget -q --header="Accept: application/json" -O - $flowctlReleaseUrl | grep \"tag_name\" | grep -v rc | awk 'NR==1{print $2}' |  sed -n 's/\"\(.*\)\",/\1/p')
    fi

    ret_val=$latest_release
}

downloadFile() {
    LATEST_RELEASE_TAG=$1

    FLOWCTL_ARTIFACT="${FLOWCTL_FILENAME}-${OS}-${ARCH}.tar.gz"
    DOWNLOAD_BASE="https://github.com/${GITHUB_ORG}/${GITHUB_REPO}/releases/download"
    DOWNLOAD_URL="${DOWNLOAD_BASE}/${LATEST_RELEASE_TAG}/${FLOWCTL_ARTIFACT}"

    # Create the temp directory
    FLOWCTL_TMP_ROOT=$(mktemp -dt flowctl-install-XXXXXX)
    ARTIFACT_TMP_FILE="$FLOWCTL_TMP_ROOT/$FLOWCTL_ARTIFACT"

    echo "Downloading $DOWNLOAD_URL ..."
    if [[ "$HTTP_REQUEST_FLOWCTL" == "curl" ]]; then
        curl -SsL "$DOWNLOAD_URL" -o "$ARTIFACT_TMP_FILE"
    else
        wget -q -O "$ARTIFACT_TMP_FILE" "$DOWNLOAD_URL"
    fi

    if [[ ! -f "$ARTIFACT_TMP_FILE" ]]; then
        echo "failed to download $DOWNLOAD_URL ..."
        exit 1
    fi
}

isReleaseAvailable() {
    LATEST_RELEASE_TAG=$1

    FLOWCTL_ARTIFACT="${FLOWCTL_FILENAME}-${OS}-${ARCH}.tar.gz"
    DOWNLOAD_BASE="https://github.com/${GITHUB_ORG}/${GITHUB_REPO}/releases/download"
    DOWNLOAD_URL="${DOWNLOAD_BASE}/${LATEST_RELEASE_TAG}/${FLOWCTL_ARTIFACT}"

    if [[ "$HTTP_REQUEST_FLOWCTL" == "curl" ]]; then
        httpstatus=$(curl -sSLI -o /dev/null -w "%{http_code}" "$DOWNLOAD_URL")
        if [[ "$httpstatus" == "200" ]]; then
            return 0
        fi
    else
        wget -q --spider "$DOWNLOAD_URL"
        exitstatus=$?
        if [[ $exitstatus -eq 0 ]]; then
            return 0
        fi
    fi
    return 1
}

installFile() {
    tar xf "$ARTIFACT_TMP_FILE" -C "$FLOWCTL_TMP_ROOT"
    local tmp_root_flowctl="$FLOWCTL_TMP_ROOT/$FLOWCTL_FILENAME"

    if [[ ! -f "$tmp_root_flowctl" ]]; then
        echo "Failed to unpack FlowCtl executable."
        exit 1
    fi

    if [[ -f "$FLOWCTL_FILE" ]]; then
        runAsRoot rm "$FLOWCTL_FILE"
    fi
    chmod o+x $tmp_root_flowctl
    mkdir -p $FLOWCTL_INSTALL_DIR
    runAsRoot cp "$tmp_root_flowctl" "$FLOWCTL_INSTALL_DIR"

    if [[ -f "$FLOWCTL_FILE" ]]; then
        echo "$FLOWCTL_FILENAME installed into $FLOWCTL_INSTALL_DIR successfully."

        $FLOWCTL_FILE version
    else 
        echo "Failed to install $FLOWCTL_FILENAME"
        exit 1
    fi
}

fail_trap() {
    result=$?
    if [[ "$result" != "0" ]]; then
        echo "Failed to install FlowCtl"
        echo "For support, go to https://flowsynx.io"
    fi
    cleanup
    exit $result
}

cleanup() {
    if [[ -d "${FLOWCTL_TMP_ROOT:-}" ]]; then
        rm -rf "$FLOWCTL_TMP_ROOT"
    fi
}

installCompleted() {
    echo -e "\nTo get started with FlowCtl, please visit https://flowsynx.io/docs/getting-started"
}

# -----------------------------------------------------------------------------
# main
# -----------------------------------------------------------------------------
trap "fail_trap" EXIT

getSystemInfo
checkHttpRequestFlowCtl

if [[ -z "$1" ]]; then
    echo "Getting the latest FlowCtl..."
    getLatestRelease
else
    ret_val=v$1
fi

verifySupported $ret_val
checkExistingFlowCtl

echo "Installing $ret_val FlowCtl..."

downloadFile $ret_val
installFile
cleanup

installCompleted