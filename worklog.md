### 08-Aug-2020

Debating whether to use DBus or RabbitMQ. We need RPC which RabbitMQ doesn't really support. We also need a manageable way to define the protocols such as ProtoBuf. RabbitMQ can work with Protobufs. gRPC is too distributed, I want a single broker.

Going ahead with protobuf and RabbitMQ. 

Will need to build a simple RPC library. It will setup the connection to RabbitMQ and associate routing keys to handler functions.

Further examination of gRPC shows it should work. Each sub-system has a port assigned to it that is exposed through docker. External callers can call it via localhost:port. This doesnt break loose coupling because a caller must know the interface already.

I don't think Gradle fits my needs. Its too based around Java. The primary language is Python and shell scripts. We also need to call a lot of external stuff like docker.

### 09-Aug-2020

https://www.digitalocean.com/community/tutorials/how-to-set-up-continuous-integration-with-buildbot-on-ubuntu-16-04
https://www.digitalocean.com/community/tutorials/how-to-install-buildbot-on-ubuntu-16-04

BuildBot has everything we need but is too heavy. It will be great to set it up now since it will be useful down the road. I still need actual build scripts.

Will also need a secure file store system for uploading/downloading files.

Working on the NIDS system. Integrating with CSIRT

@load policy/integration/collective-intel

Also figuring out Sphinx/docs

Need to seperate the grpc server so we don't duplicate that code everywhere. Store a copy of the script with each sub-system. The only thing that probably varies is the port.

### 10-Aug-2020

Work on building the Docker images.

Each docker image will contain an RPC server script customized for it.

Each Docker image also needs a service that manages the sub-system and RPC server.

Alpine uses OpenRC. https://wiki.alpinelinux.org/wiki/Writing_Init_Scripts

Researching service meshs and microservices.

A true service mesh seems to be over kill. The microservices are all on the same device. 

We don't even need a proxy system. Just hardcode the ports for each service. The host will always be localhost

Build steps:
	protoc
	build python
	build docker images:
		create container
		copy files into container (dont need to start container)
		docker commit container -m "version"
		
		OR
		
		make build dir for each Docker image
		copy relevant files to each Dockerfile folder
		build from docker file
		
	make alpine package:
		move relevant stuff into package dir
		start docker image
		copy over relevant files
		docker exec
		copy package out of image
		
python -m grpc_tools.protoc -I=. --python_out=. --grpc_python_out=. .

### 11-Aug-2020

Zeek dockerfile now copies the relevant config files

Need to add a makefile rule.

### 17-Aug-2020

Worked a lot on funding and business stuff.

Realized we need to focus on the local cloud aspect. We could store data in a distributed manner accross all the devices and use it like a highly localized CDN.

You'll be able to develop apps for the device as if it was a traditional cloud environment.

Also, moving towards getting a simple prototype running on a VM ASAP.

Probably going to move away from ES and towards a trad database to ease integration.

### 31-Aug-2020

C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe C:\Users\leena\honeybee-sensor\windows\service\HoneybeeService\HoneybeeService\bin\Debug\HoneybeeService.exe

Need to properly parse the output from docker-compose

checking configurations ...
zeek scripts failed.
error in /opt/zeek/share/zeek/site/local.zeek, line 4: unrecognized character -
fatal error in /opt/zeek/share/zeek/site/local.zeek, line 6: can't find misc/loaded-scripts

Exiting: error loading config file: config file ("/etc/filebeat/filebeat.yml") can only be writable by the owner but the permissions are "-rwxrwxrwx" (to fix the permissions use: 'chmod go-w /etc/filebeat/filebeat.yml')
   ...fail!
2020-07-12T22:12:33.498Z        INFO    instance/beat.go:647    Home path: [/usr/share/filebeat] Config path: [/etc/filebeat] Data path: [/var/lib/filebeat] Logs path: [/var/log/filebeat]
2020-07-12T22:12:33.595Z        INFO    instance/beat.go:655    Beat ID: 7f7aac7d-bc9e-4952-9887-14279065ddd0

Still having issues with dos and unix new lines and the permissions on the filebeat.yml

Updated gitattributes to deal with the line endings

Drop Zeek for now. Probably ELK too.

What do we need for a minimal viable prototype?
	Needs to send logs to us
	Needs to accept updates remotely
	logstash setup to sned all data to us

Update:
	Shutdown docker
	Unzip update.zip into the docker directory, force overwrite

Add to Windows registry on install: dockerlocation: where the docker-compose folder is 

Install osquery alongside

Going to rip out nids, es, and kibana

Need UUID for agent that is used to determine the ELK index 

MAKE SURE REMOTE ES ENDPOINT ONLY ALLOWS WHITELISTED ADDRESSES!
We don't have the ES security expack so we can't do encrypted stuff

### 01-Sep-2020

A reverse shell isn't going to work on Windows. It's too risky. 

Have each agent check in via ssh/scp every thirty minutes.

Agent will scp a list of osquery queries to run. It will perform each one and then send the response to HQ

Going to re-write the service in Powershell if possible.

### 02-Sep-2020

Services can be written in Powershell

Windows 10 has an OpenSSH server

Need to create a service/system account on Windows

### 09-Sep-2020

Got the physical prototype. All work is now focused on getting it working.

We're using Docker Swarm now. It has caused one difficult issue: we can no longer add NET_ADMIN so Zeek won't work in Docker now. I remember previously trying to mirror traffic to it from the host. That might be possible now. It will also probably be necessary. Other apps are going to want net traffic visibility, its a huge selling point of the device.

Just thought of something. We should clusterize Zeek onto the local cluster. Zeek will only run on a Honeybee device.

The most flexible method for packet capture is probably something like pf_ring on host that sends to containers. pf_ring can run in a docker container but it requires host networking or net_admin. We could run it seperately from the swarm.

The Honeybee device will have both swarm and a local.

Looks like pf_ring will need to be installed directly on host. This makes sense since its a kernel module. There are no pre-built packages for Alpine. 

"PF_RING™ kernel module and drivers are distributed under the GNU GPLv2 license, LGPLv2.1 for the user-space PF_RING library, and are available in source code format."

Building pf_ring on Alpine is a huge pain. 

Should probably move back to Ubuntu server. We want the platform to be open and easy to expand. Alpine is not widely supported and includes its own package manager.

Maybe Debian? Smaller, more stable, but with basically the same packages available.

Moved to debian. pf_ring installs easily

Unclear where we need to have pf_ring listen. If it listens on eno1 (to internet) it sees all traffic as its local IP. Probably need to listen on wlan0 and eno2

Still need to figure out how to send traffic from pf_ring to docker containers. Looks like the zbounce example does this.

As long as pf_ring is installed on the host it can be accessed from docker containers. These containers require --network=host though

### 10-Sep-2020

ntopng accounts for %15 of memory usage. 

Setting up software agents and PKI

1599704268.059068       Reporter::ERROR Zeek was not configured for GeoIP support (lookup_location(SSH::lookup_ip))     /opt/zeek/share/zeek/policy/protocols/ssh/geo-data.zeek, line 30

1599703341.836045       Reporter::WARNING       Your interface is likely receiving invalid TCP checksums, most likely from NIC checksum offloading.  By default, packets with invalid checksums are discarded by Zeek unless using the -C command-line option or toggling the 'ignore_checksums' variable.  Alternatively, disable checksum offloading by the network adapter to ensure Zeek analyzes the actual checksums that are transmitted.        /opt/zeek/share/zeek/base/misc/find-checksum-offloading.zeek, line 54
1599703375.370201       Reporter::INFO  received termination signal     (empty)
1599703375.370201       Reporter::INFO  375 packets received on interface eno2, 0 (0.00%) dropped       (empty)

postdrop: warning: unable to look up public/pickup: No such file or directory

Got basic PKI set up.