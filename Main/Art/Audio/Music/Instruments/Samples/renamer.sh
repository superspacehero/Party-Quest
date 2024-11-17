#!/bin/bash

# Set the parent directory path
parent_dir=$(dirname "$0")

# Loop through all the child folders
for folder in "$parent_dir"/*; do
    # Check if the current item is a directory
    if [[ -d "$folder" ]]; then
        # Loop through all the files in the current folder
        for file in "$folder"/*; do
            # Check if the current item is a file
            if [[ -f "$file" ]]; then
                # Get the filename without the path
                filename=$(basename "$file")
                
                # Get the part of the filename that comes after the "-"
                new_filename="${filename#*-}"
                
                # Rename the file
                mv "$file" "$folder/$new_filename"
            fi
        done
    fi
done