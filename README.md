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
| [Analysis](doc/analysis.md) | Scripts running in docker | Takes collected data and performs various analysis. |
| Message Broker | RabbitMQ | Acts as a broker between the other sub-systems. |
| Data Transmission | Logstash on Database | Breaks loose coupling currently. Transmits data to external systems and performs any needed modifications. |
| Firewall | NFTables on sensor host | Controls network access. |
| HIDS | Wazuh | The HIDS manager for anyagents installed on hosts. |
| Routing | NFTables on sensor host | Controls IP assignment, connections, WiFi, etc. |
| Frontend | Custom web app on NGINX | Allows a user to view and manage their network. |
| Subsystem Management | Custom service on sensor host | Manages starting, stopping, and monitoring of other sub-systems. |

Control of the various sub-systems is enacted by sending generic messages to an abstract system. Each abstract system has a specific interface of actions it must implement. The implementation is completely contained within the subsystem.

### Internal Security

Almost all sub-systems are contained within their own docker containers and can only interact externally via AMQP and the Database.

Each sub-system has a whitelist of messages it is allowed to send. For instance: the NIDS should not transmit any active or control messages, it isn't allowed to modify the system. 

Could we sign the docker images in some way and keep the public key in a secure hardware store?

The sensor is built on top of Alpine linux, a hardened distro. 

### Message Broker

**General Messages**

| Message Name | Purpose |
| ----------- | ----------- |
| Start | Tells the abstract system to start. |
| Graceful Stop | Tells the abstract system to stop gracefully. |
| Forceful Stop | Tells the abstract system to halt immeditely. |
| Status | Tells the system to respond with a status report. |
| Get Implementation | Asks the system to return a string describing its implementation. IE Name and Version |

**NIDS Messages**

| Message Name | Purpose |
| ----------- | ----------- |
| Add Whitelist Address | Ignore traffic associated with this address. |
| Remove Whitelist Address | Remove this address from the whitelist. |
| Update Intel | Update the intelligence file. |

**Database Messages**

Note: Reads/Writes/Queries are not performed with the messaging system but through a generic SQL interface.

| Message Name | Purpose |
| ----------- | ----------- |

**Analysis Messages**

See [here](doc/analysis.md)

**Firewall Messages**

| Message Name | Purpose |
| ----------- | ----------- |
| Add Block | Blocks the address. |
| Remove Block | Unblocks the address. |

**HIDS Messages**

| Message Name | Purpose |
| ----------- | ----------- |

**Routing Messages**

| Message Name | Purpose |
| ----------- | ----------- |
| Host Connected | A new host has connected to the network. |
| Host Disconnected | A host has left the network. |
| Host Address Change | A host has had its address changed. |
| Drop Host | Tells the router to drop a host. |

**Frontend Messages**

| Message Name | Purpose |
| ----------- | ----------- |

### Analysis

See [here](doc/analysis.md)

### Database

The database system must provide a generic SQL interface. A system can also use an interface specific to the DB system. 

The current (as of 8-Aug-2020) implementation is an ELK stack. Filebeat is installed on each sub-system and configured to collect and transmit the data to a Logstash endpoint on the DB container.

What tables/indices/collections does the DB provide?

| Table Name | Purpose |
| ----------- | ----------- |
| Hosts | Tracks what hosts are or have been connected to the network and their address, fingerprints, etc. |
| Alerts | Various important info that needs to be acted upon or acknowledged. I.E. security alerts |
| Services | What services are available on what hosts. |
| Raw * | The various sub-systems can store their raw implementation specific data in these tables. |

This part gets very close to LDAP. Some of this data should be available to external systems.

How do we translate from the implementation specific data to a general form?

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
