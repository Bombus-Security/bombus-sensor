### 8-Aug-2020

Debating whether to use DBus or RabbitMQ. We need RPC which RabbitMQ doesn't really support. We also need a manageable way to define the protocols such as ProtoBuf. RabbitMQ can work with Protobufs. gRPC is too distributed, I want a single broker.

Going ahead with protobuf and RabbitMQ. 

Will need to build a simple RPC library. It will setup the connection to RabbitMQ and associate routing keys to handler functions.

Further examination of gRPC shows it should work. Each sub-system has a port assigned to it that is exposed through docker. External callers can call it via localhost:port. This doesnt break loose coupling because a caller must know the interface already.

I don't think Gradle fits my needs. Its too based around Java. The primary language is Python and shell scripts. We also need to call a lot of external stuff like docker.

### 9-Aug-2020

https://www.digitalocean.com/community/tutorials/how-to-set-up-continuous-integration-with-buildbot-on-ubuntu-16-04
https://www.digitalocean.com/community/tutorials/how-to-install-buildbot-on-ubuntu-16-04

BuildBot has everything we need but is too heavy. It will be great to set it up now since it will be useful down the road. I still need actual build scripts.

Will also need a secure file store system for uploading/downloading files.

Working on the NIDS system. Integrating with CSIRT

@load policy/integration/collective-intel