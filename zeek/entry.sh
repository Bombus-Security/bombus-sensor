#!/bin/bash

chmod go-w /etc/filebeat/filebeat.yml

/opt/zeek/bin/zeekctl deploy
service filebeat start

tail -f /var/log/filebeat/filebeat
