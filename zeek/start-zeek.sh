#!/bin/bash

if ! [ $(id -u) = 0 ]; then
        echo "Must be run as root."
        exit 1
fi

/opt/zeek/bin/zeekctl deploy
