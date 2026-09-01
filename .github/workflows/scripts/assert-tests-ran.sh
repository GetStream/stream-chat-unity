#!/usr/bin/env bash
# Fails the job when the test runner reported success without executing any test.
#
# Unity exits 0 when it cannot activate a license, and game-ci then prints
# "Run succeeded, no failures occurred" with an empty result file. Without this guard a
# job that ran zero tests is indistinguishable from a job where everything passed.
set -euo pipefail

results_file="${1:?path to the results xml is required}"

if [ ! -f "${results_file}" ]; then
  echo "::error::${results_file} is missing - the test runner never produced results."
  exit 1
fi

test_case_count="$(sed -n 's/.*<test-run [^>]*testcasecount="\([0-9]*\)".*/\1/p' "${results_file}" | head -n 1)"

if [ -z "${test_case_count}" ]; then
  echo "::error::Could not read testcasecount from ${results_file}."
  exit 1
fi

if [ "${test_case_count}" -eq 0 ]; then
  echo "::error::The test runner executed 0 tests. Treating this as a failure - see the log for license or compilation errors."
  exit 1
fi

echo "Test runner executed ${test_case_count} tests."
