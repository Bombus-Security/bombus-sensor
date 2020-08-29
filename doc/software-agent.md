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