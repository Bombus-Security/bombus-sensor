#!/bin/bash

#The purpose of this script is to package everything needed to build a sensor into one zip

export KRO_HOME=../
export KRO_SENSOR=.
export KRO_ZEEK=$KRO_HOME/zeek
export KRO_ELK=$KRO_HOME/docker-elk
export KRO_HADOOP=$KRO_HOME/hadoop

export BUILD_DIR=$KRO_SENSOR/sensor
export BUILD_ZEEK_DIR=$BUILD_DIR/zeek
export BUILD_ELK_DIR=$BUILD_DIR/docker-elk
export BUILD_HADOOP_DIR=$BUILD_DIR/hadoop

mkdir -p $BUILD_DIR
mkdir -p $BUILD_ZEEK_DIR
mkdir -p $BUILD_ELK_DIR
mkdir -p $BUILD_HADOOP_DIR

#Copy all bro files
cp $KRO_ZEEK/* $BUILD_ZEEK_DIR

cp $KRO_ELK/* $BUILD_ELK_DIR

cp $KRO_HOME/start-all.sh $BUILD_DIR
cp $KRO_HOME/stop-all.sh $BUILD_DIR

cp $KRO_SENSOR/install.sh $BUILD_DIR


###########################################################################
#ZIP it up
###########################################################################
chmod -R +x $BUILD_DIR/*.sh
zip -r sensor.zip $BUILD_DIR/
