#!/bin/bash

if ! [ $(id -u) = 0 ]; then
        echo "Must be run as root."
        exit 1
fi

###########################################################################
#Environment Variables
###########################################################################

export KRO_HOME=.
export KRO_ZEEK_DIR=$KRO_HOME/zeek
export KRO_ELK_DIR=$KRO_HOME/docker-elk
export KRO_HADOOP_DIR=$KRO_HOME/hadoop

export ZEEK_INSTALL_DIR=/opt/zeek

#The purpose of this script is to turn an Ubuntu 20.04 install into a sensor.

###########################################################################
#Install Pre-reqs
###########################################################################

#install pre-reqs
apt install apt-transport-https ca-certificates curl gnupg-agent software-properties-common lsb-release gnupg2 -y

###########################################################################
#Add Repos
###########################################################################

#Install Zeek repos
echo 'deb http://download.opensuse.org/repositories/security:/zeek/xUbuntu_20.04/ /' | tee /etc/apt/sources.list.d/security:zeek.list
wget -q https://download.opensuse.org/repositories/security:zeek/xUbuntu_20.04/Release.key 
gpg --dearmor Release.key
apt-key add Release.key.gpg
rm ./Release.key
rm ./Release.key.gpg

#Install Docker repos
wget -q https://download.docker.com/linux/ubuntu/gpg 
apt-key add gpg
add-apt-repository "deb [arch=amd64] https://download.docker.com/linux/ubuntu $(lsb_release -cs) stable"
rm ./gpg

#Install ELK repos
wget -q https://artifacts.elastic.co/GPG-KEY-elasticsearch
apt-key add GPG-KEY-elasticsearch
echo "deb https://artifacts.elastic.co/packages/7.x/apt stable main" | tee -a /etc/apt/sources.list.d/elastic-7.x.list
rm ./GPG-KEY-elasticsearch

#Install Wazuh repos
wget -q https://packages.wazuh.com/key/GPG-KEY-WAZUH
apt-key add GPG-KEY-WAZUH
echo "deb https://packages.wazuh.com/3.x/apt/ stable main" | tee -a /etc/apt/sources.list.d/wazuh.list
rm ./GPG-KEY-WAZUH

#Install Virtualbox
wget -q https://download.virtualbox.org/virtualbox/6.1.10/virtualbox-6.1_6.1.10-138449~Ubuntu~eoan_amd64.deb
apt install ./virtualbox-6.1_6.1.10-138449~Ubuntu~eoan_amd64.deb -y

###########################################################################
#Install APT Packages
###########################################################################

apt update
apt install docker-ce docker-ce-cli containerd.io zeek zeekctl filebeat wazuh-manager wazuh-api -y

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
ln -s /opt/zeek/logs /var/log/zeek
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
