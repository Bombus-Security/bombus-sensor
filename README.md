# KROPOTKIN SENSOR

## What is it?

The Kropotkin sensor is the collection of software that is installed on a network device to capture, analyze, and transmit network traffic. 

## Organization

The sensor uses various software packages including [Zeek](https://zeek.org/), [Wazuh](https://wazuh.com/), and [Filebeat](https://www.elastic.co/beats/filebeat). Each of these components is run in a Docker container. The host OS is Alpine Linux. Various scripts and services are used to control and integrate these components. A frontend hosted on [Nginx](https://www.nginx.com/) is developed in the repo [kropotkin-sensor-frontend](https://github.com/Kropotkin-Security/kropotkin-sensor-frontend).

## License

The code developed by Kropotkin is licensed under the Apache 2.0 license. All other software is licensed under various licenses.
