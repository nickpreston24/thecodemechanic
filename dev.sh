#!/bin/bash
echo "foo"

# Define your scripts
SCRIPT1="tailwindcss-extra -i wwwroot/css/site.css -o wwwroot/css/site.min.css --watch"
SCRIPT2="dotnet watch run web  --no-build --non-interactive | grep --invert-match warning --line-buffered" 
#  SCRIPT3="sh tail.sh"

# Define your tmux session name
SESSION_NAME="my_parallel_scripts"

# Check if a tmux session with the given name already exists
tmux has-session -t "$SESSION_NAME" 2>/dev/null

if [ $? != 0 ]; then
    # Create a new tmux session and run the first script
    tmux new-session -d -s "$SESSION_NAME" "$SCRIPT1"

    # Split the window horizontally and run the second script
    tmux split-window -h -t "$SESSION_NAME" "$SCRIPT2"

    # run tail
#      tmux split-window -h -t "$SESSION_NAME" "$SCRIPT3"

else
    echo "Tmux session '$SESSION_NAME' already exists. Attaching to it."
fi

# Attach to the tmux session
tmux attach-session -t "$SESSION_NAME"