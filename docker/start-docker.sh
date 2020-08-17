#!/bin/bash

# This script moves all the necessary config files into a temp folder

if ! [ $(id -u) = 0 ]; then
        echo "Must be run as root."
        exit 1
fi

export DATABASE_DIR=../database
export NIDS_DIR=../nids
export HIDS_DIR=../hids
export TEMP_DIR=./temp

#Move all needed files here
cp $DATABASE_DIR/logstash.yml $TEMP_DIR/logstash.yml
cp $DATABASE_DIR/logstash-pipeline.conf $TEMP_DIR/logstash-pipeline.conf

cp $NIDS_DIR/config/local.zeek $TEMP_DIR/local.zeek
cp $NIDS_DIR/config/zeek-filebeat.yml $TEMP_DIR/zeek-filebeat.yml
cp $NIDS_DIR/config/node.cfg $TEMP_DIR/node.cfg

cp $HIDS_DIR/wazuh-filebeat.yml $TEMP_DIR/wazuh-filebeat.yml
cp $HIDS_DIR/wazuh-template.json $TEMP_DIR/wazuh-template.json

docker-compose up

rm temp/*
