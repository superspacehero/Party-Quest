#!/bin/bash

# Get the parent directory path
parent_dir=$(dirname "$0")

# Loop through all the files in the current directory
for file in *; do
    # Check if the file doesn't end with ".import" and contains "-"
    if [[ ! "$file" == *.import* && "$file" == *-* ]]; then
        # Extract the part of the filename before the "-"
        folder_name="${file%%-*}"

        # Create the folder if it doesn't exist
        mkdir -p "$parent_dir/$folder_name"

        # Move the file to the corresponding folder, removing the part before (and including) the "-"
        mv "$file" "$parent_dir/$folder_name/${file#*-}"
    fi
done