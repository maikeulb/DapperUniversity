#!/bin/bash

set -e
run_cmd="dotnet run --server.urls http://*:80"

>&2 echo "running application"
exec $run_cmd
