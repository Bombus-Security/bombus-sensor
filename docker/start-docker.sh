#!/bin/bash

# This script moves all the necessary config files into a temp folder

if ! [ $(id -u) = 0 ]; then
        echo "Must be run as root."
        exit 1
fi

export ELK_DIR=../elk
export ZEEK_DIR=../zeek
export WAZUH_DIR=../wazuh
export TEMP_DIR=./temp

#Move all needed files here
cp $ELK_DIR/logstash.yml $TEMP_DIR/logstash.yml
cp $ELK_DIR/logstash-pipeline.conf $TEMP_DIR/logstash-pipeline.conf

cp $ZEEK_DIR/local.zeek $TEMP_DIR/local.zeek
cp $ZEEK_DIR/filebeat.yml $TEMP_DIR/zeek-filebeat.yml
cp $ZEEK_DIR/node.cfg $TEMP_DIR/node.cfg
cp $ZEEK_DIR/sumstats-kropotkin.zeek $TEMP_DIR/sumstats-kropotkin.zeek

cp $WAZUH_DIR/filebeat.yml $TEMP_DIR/wazuh-filebeat.yml
cp $WAZUH_DIR/wazuh-template.json $TEMP_DIR/wazuh-template.json

docker-compose up

rm temp/*
