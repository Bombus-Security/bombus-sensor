#!/bin/bash

if ! [ $(id -u) = 0 ]; then
        echo "Must be run as root."
        exit 1
fi

cd docker-elk
./start-elk.sh
cd ..

zeek/start-zeek.sh
