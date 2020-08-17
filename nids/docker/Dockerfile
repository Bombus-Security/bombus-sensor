FROM ubuntu:20.04
MAINTAINER Leena Wilson <lwilson2048@gmail.com>

RUN apt update

RUN apt install apt-transport-https ca-certificates curl gnupg-agent software-properties-common lsb-release gnupg2 wget net-tools -y

#Install Zeek repos
RUN echo 'deb http://download.opensuse.org/repositories/security:/zeek/xUbuntu_20.04/ /' | tee /etc/apt/sources.list.d/security:zeek.list
RUN wget -q https://download.opensuse.org/repositories/security:zeek/xUbuntu_20.04/Release.key 
RUN gpg --dearmor Release.key
RUN apt-key add Release.key.gpg
RUN rm ./Release.key
RUN rm ./Release.key.gpg

#Install ELK repos
RUN wget -q https://artifacts.elastic.co/GPG-KEY-elasticsearch
RUN apt-key add GPG-KEY-elasticsearch
RUN echo "deb https://artifacts.elastic.co/packages/7.x/apt stable main" | tee -a /etc/apt/sources.list.d/elastic-7.x.list
RUN rm ./GPG-KEY-elasticsearch

RUN apt update
RUN apt install zeek zeekctl filebeat -y

#Enable Zeek module
RUN filebeat modules enable zeek

#Create symlink to where Filebeat Zeek module expects Zeek logs
RUN ln -s /opt/zeek/logs /var/log/zeek
RUN ln -s /opt/zeek/logs /var/log/bro

RUN mkdir /etc/service
RUN mkdir /etc/service/filebeat

#Copy config files
COPY ../config/local.zeek /opt/zeek/share/zeek/
COPY ../config/node.cfg /opt/zeek/etc/
COPY ../config/zeek-filebeat.yml /etc/filebeat
COPY ./filebeat.runit.service /etc/service/filebeat/run

#Copy all Zeek scripts
ADD ../zeek-scripts /opt/zeek/share/zeek/site/

RUN chmod +x /etc/service/filebeat/run
COPY ./entry.sh /bin/entry.sh

ENTRYPOINT ["/bin/entry.sh"]
