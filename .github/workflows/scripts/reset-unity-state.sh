#!/usr/bin/env bash
# Resets everything a killed Unity attempt leaves behind, so the next attempt can start.
#
# When a "Run Tests" step hits its step timeout, GitHub kills the action but not the
# docker container it started. Without this cleanup the next attempt dies immediately
# with either "Multiple Unity instances cannot open the same project" (the orphan still
# holds the project) or "Machine identification is invalid for current license" - and in
# the licensing case Unity exits 0 after running zero tests, which silently turns the
# job green.
set -uo pipefail

running_containers="$(docker ps -q)"
if [ -n "${running_containers}" ]; then
  echo "Stopping leftover containers: ${running_containers}"
  # shellcheck disable=SC2086
  docker stop --time 10 ${running_containers} || true
fi

echo "Removing Unity project lock file"
sudo rm -f Temp/UnityLockfile || true

HOME_DIR="${RUNNER_TEMP}/_github_home"
echo "Removing Unity license state under ${HOME_DIR}"
sudo find "${HOME_DIR}" -iname '*.ulf' -delete -print || true
sudo rm -rf "${HOME_DIR}/.config/unity3d/Unity/licenses" \
  "${HOME_DIR}/.local/share/unity3d/Unity" || true
