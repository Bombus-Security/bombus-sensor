#!/bin/sh

export BUILD_DIR="/build"

cd /build/honeybee/
abuild checksum
abuild -r