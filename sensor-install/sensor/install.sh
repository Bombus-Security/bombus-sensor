#!/bin/bash

if ! [ $(id -u) = 0 ]; then
        echo "Must be run as root."
        exit 1
fi

###########################################################################
#Environment Variables
###########################################################################

export KRO_HOME=.
export KRO_ZEEK_DIR=$INSTALL_HOME/zeek
export KRO_ELK_DIR=$INSTALL_HOME/docker-elk
export KRO_HADOOP_DIR=$INSTALL_HOME/hadoop

export ZEEK_INSTALL_DIR=/opt/zeek

#The purpose of this script is to turn an Ubuntu 20.04 install into a sensor.

###########################################################################
#Install Pre-reqs
###########################################################################

#install pre-reqs
apt install apt-transport-https ca-certificates curl gnupg-agent software-properties-common lsb-release gnupg2

###########################################################################
#Add Repos
###########################################################################

#Install Zeek repos
echo 'deb http://download.opensuse.org/repositories/security:/zeek/xUbuntu_20.04/ /' | tee /etc/apt/sources.list.d/security:zeek.list
curl -fsSL https://download.opensuse.org/repositories/security:zeek/xUbuntu_20.04/Release.key | gpg --dearmor | tee /etc/apt/trusted.gpg.d/security:zeek.gpg > /dev/null

#Install Docker repos
curl -fsSL https://download.docker.com/linux/ubuntu/gpg | apt-key add -
add-apt-repository "deb [arch=amd64] https://download.docker.com/linux/ubuntu $(lsb_release -cs) stable"

#Install ELK repos
wget -qO - https://artifacts.elastic.co/GPG-KEY-elasticsearch | apt-key add -
echo "deb https://artifacts.elastic.co/packages/7.x/apt stable main" | tee -a /etc/apt/sources.list.d/elastic-7.x.list

#Install Wazuh repos
curl -s https://packages.wazuh.com/key/GPG-KEY-WAZUH | apt-key add -
echo "deb https://packages.wazuh.com/3.x/apt/ stable main" | tee -a /etc/apt/sources.list.d/wazuh.list

#Install VMWare

###########################################################################
#Install APT Packages
###########################################################################

apt update
apt install docker-ce docker-ce-cli containerd.io zeek zeekctl filebeat wazuh-manager wazuh-api

###########################################################################
#Wazuh Configuration
###########################################################################
curl -so /etc/filebeat/wazuh-template.json https://raw.githubusercontent.com/wazuh/wazuh/v3.13.0/extensions/elasticsearch/7.x/wazuh-template.json
curl -s https://packages.wazuh.com/3.x/filebeat/wazuh-filebeat-0.1.tar.gz | tar -xvz -C /usr/share/filebeat/module

###########################################################################
#Filebeat Configuration
###########################################################################
#Enable Zeek module
filebeat modules enable zeek

#Create symlink to where Filebeat Zeek module expects Zeek logs
ln -s /var/log/zeek /opt/zeek/logs
cp $KRO_ELK_DIR/filebeat.yml /etc/filebeat/

###########################################################################
#Zeek Configuration
###########################################################################

cp $KRO_ZEEK_DIR/node.cfg $ZEEK_INSTALL_DIR/etc
cp $KRO_ZEEK_DIR/local.cfg $ZEEK_INSTALL_DIR/share/zeek/site/
cp $KRO_ZEEK_DIR/sumstats-kropotkin.zeek $ZEEK_INSTALL_DIR/share/zeek/site/

###########################################################################
#Docker Configuration
###########################################################################
