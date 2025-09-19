#!/bin/bash
cd /home/kavia/workspace/code-generation/event-planning-suite-12832-12841/event_planner_backend
dotnet build --no-restore -v quiet -nologo -consoleloggerparameters:NoSummary /p:TreatWarningsAsErrors=false
LINT_EXIT_CODE=$?
if [ $LINT_EXIT_CODE -ne 0 ]; then
  exit 1
fi

