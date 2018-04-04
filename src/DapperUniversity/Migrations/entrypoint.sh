while true; do
    flyway migrate
    if [[ "$?" == "0" ]]; then
        break
    fi
    sleep 5
done
