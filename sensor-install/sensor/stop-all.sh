#!/bin/bash

if ! [ $(id -u) = 0 ]; then
        echo "Must be run as root."
        exit 1
fi

docker-elk/stop-elk.sh
zeek/stop-zeek.sh
systemctl stop filebeat
