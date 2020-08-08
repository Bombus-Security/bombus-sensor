# HONEYBEE SENSOR

## What is it?

The Honeybee sensor is the collection of software that is installed on a network device to capture, analyze, and transmit network traffic. 

## Building

The build process.

As of 7-Aug-2020 the build process is still being developed. We are re-designing key parts so theres nothing to build.

## TODO

(7-Aug-2020) Move the docker-compose stuff into the Alpine host.
(7-Aug-2020) Setup message broker

## Architecture

This was last updated on 7-Aug-2020

The sensor uses various software packages including [Zeek](https://zeek.org/), [Wazuh](https://wazuh.com/), and [Filebeat](https://www.elastic.co/beats/filebeat). Each of these components is run in a Docker container. The host OS is Alpine Linux. Various scripts and services are used to control and integrate these components. A frontend hosted on [Nginx](https://www.nginx.com/) is developed in the repo [kropotkin-sensor-frontend](https://github.com/Kropotkin-Security/kropotkin-sensor-frontend).

The sensor is responsible for sending the data to various sinks and performing any transforms needed to do this. Currently (6-Aug-2020) logstash and filebeat are used for collecting and transforming the data.

### Loosely Coupled Sub-systems

The system is composed of sub-systems that communicate via AMQP and a Database. Each sub-system is ideally contained in a docker image. This is not always possible so some exceptions are allowed. 

| Abstract System | Current Implementation | General Description |
| ----------- | ----------- | ----------- |
| NIDS | Zeek | Monitors network traffic with signatures and anomaly detection |
| Database | Elasticsearch and Logstash | Stores, indexes, and makes available the data from other systems. |
| Data Collection | Filebeat on individual sub-systems | Collects and uploads the data from other sub-systems. Slightly breaks loose coupling. |
| Analysis | Scripts running in docker | Takes collected data and performs various analysis. |
| Message Broker | RabbitMQ | Acts as a broker between the other sub-systems. |
| Data Transmission | Logstash on Database | Breaks loose coupling currently. Transmits data to external systems and performs any needed modifications. |
| Firewall | NFTables on sensor host | Controls network access. |
| Routing | NFTables on sensor host | Controls IP assignment, connections, WiFi, etc. |
| Frontend | Custom web app on NGINX | Allows a user to view and manage their network. |
| Subsystem Management | Custom service on sensor host | Manages starting, stopping, and monitoring of other sub-systems. |

Control of the various sub-systems is enacted by sending generic messages to an abstract system. Each abstract system has a specific interface of actions it must implement. The implementation is completely contained within the subsystem.

### Internal Security

Almost all sub-systems are contained within their own docker containers and can only interact externally via AMQP and the Database.

Each sub-system has a whitelist of messages it is allowed to send. For instance: the NIDS should not transmit any active or control messages, it isn't allowed to modify the system. 

Could we sign the docker images in some way and keep the public key in a secure hardware store?

The sensor is built on top of Alpine linux, a hardened distro. 

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
