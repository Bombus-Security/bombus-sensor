# HONEYBEE SENSOR

## What is it?

The Honeybee sensor is the collection of software that is installed on a network device to capture, analyze, and transmit network traffic. 

## Building

The build process.

If you're developing on an Alpine linux host skip making the VM.
Create a VM with Alpine installed on it.
On the Alpine VM install git, alpine-sdk, docker, docker-compose

On your host create an SSH server.
From the Alpine host run:
git clone ssh://<host ip>/<host path to this repo>

On Alpine run:
gradle :make-alpine-package

## TODO

Switch to building stuff in an Alpine docker container instead of needing a VM.

Move the docker-compose stuff into the Alpine host.

## Organization

The sensor uses various software packages including [Zeek](https://zeek.org/), [Wazuh](https://wazuh.com/), and [Filebeat](https://www.elastic.co/beats/filebeat). Each of these components is run in a Docker container. The host OS is Alpine Linux. Various scripts and services are used to control and integrate these components. A frontend hosted on [Nginx](https://www.nginx.com/) is developed in the repo [kropotkin-sensor-frontend](https://github.com/Kropotkin-Security/kropotkin-sensor-frontend).

The sensor is responsible for sending the data to various sinks and performing any transforms needed to do this. Currently (6-Aug-2020) logstash and filebeat are used for collecting and transforming the data.

The system is composed of sub-systems that communicate via AMQP and a Database. 

| Abstract System | Current Implementation | General Description |
| ----------- | ----------- | ----------- |
| NIDS | Zeek | Monitors network traffic with signatures and anomaly detection |
| Database | Elasticsearch and Logstash | Stores, indexes, and makes available the data from other systems. |
| Data Collection | Filebeat on individual sub-systems | Collects and uploads the data from other sub-systems. Slightly breaks loose coupling. |
| Analysis | Scripts running in docker | Takes collected data and performs various analysis. |
| Message Broker | RabbitMQ | Acts as a broker between the other sub-systems. |
| Data Transmission | Logstash on Database | Breaks loose coupling. Transmits data to external systems. |

## Directory Layout

### alpine/

The alpine package and stuff needed to build it.

### doc/

Documentation.

### docker/

### elk/

### gradle/

Stuff created and used by Gradle. Probably shouldn't edit anything in it.

### hadoop/

### wazuh/

### zeek/

## License

The code developed by Kropotkin is licensed under the Apache 2.0 license. All other software is licensed under various licenses.
