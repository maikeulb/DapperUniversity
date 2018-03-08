#!/bin/bash
set -e
run_cmd="flyway migrate"

>&2 echo "Start migration"
exec $run_cmd

