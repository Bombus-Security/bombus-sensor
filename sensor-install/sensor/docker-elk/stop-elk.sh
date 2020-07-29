#!/bin/bash

if ! [ $(id -u) = 0 ]; then
	echo "You must be root."
	exit 1
fi

docker kill es01 eshq01 ls01 kib01
