#!/bin/bash
#
# USE THIS SCRIPT TO RUN THE PROGRAM!
#
if [ "$1" != "" ] ; then
  CURRENT_DIR=$(pwd)
  FILE_PATH="$CURRENT_DIR/$1"
  echo "$CURRENT_DIR"
  # This should always be the same
  INTERPRETER_LOC="/Users/paavohemmo/projects/Paavo++/Interpreter/Interpreter.csproj"
  if test -f "$FILE_PATH" ; then
    dotnet run --project $INTERPRETER_LOC $FILE_PATH
  else
    echo "File '$1' was not found"
  fi
else
  echo "No file specified"
  INTERPRETER_LOC="/Users/paavohemmo/projects/Paavo++/Interpreter/Interpreter.csproj"
  dotnet run --project $INTERPRETER_LOC
fi