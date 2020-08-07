#!/bin/sh

# This script is executed by gradle

IMAGE_VERSION = "3.12.0"
IMAGE_NAME = "apk-builder-${IMAGE_VERSION}"

# Check if image already exists
docker create alpine:3.12.0 --name $IMAGE_NAME

# Copy all neccessary files
docker cp ./to-docker/* $IMAGE_NAME:/build

docker exec sh -c "/build/build-apk.sh"
