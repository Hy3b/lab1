#!/bin/bash

FILE="/home/theone1/Documents/CODE/lab1/is_done.txt"

while true; do
    if [ -f "$FILE" ]; then
        CONTENT=$(cat "$FILE" | tr -d '\n\r ')
        if [ "$CONTENT" == "ok" ]; then
            shutdown 0
        fi
    fi
    
    # Tạm dừng 600 giây (10 phút) trước khi lặp lại vòng kiểm tra tiếp theo
    sleep 600
done