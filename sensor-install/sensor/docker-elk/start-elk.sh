#!/bin/bash
if ! [ $(id -u) = 0 ]; then
	echo "Must be run as root."
	exit 1
fi

echo "Make sure you started the VM because LS needs Kafka to be up."

docker-compose up &> docker-log.log &
systemctl start filebeat
