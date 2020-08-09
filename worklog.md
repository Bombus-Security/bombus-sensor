### 8-Aug-2020

Debating whether to use DBus or RabbitMQ. We need RPC which RabbitMQ doesn't really support. We also need a manageable way to define the protocols such as ProtoBuf. RabbitMQ can work with Protobufs. gRPC is too distributed, I want a single broker.

Going ahead with protobuf and RabbitMQ. 

Will need to build a simple RPC library. It will setup the connection to RabbitMQ and associate routing keys to handler functions.

Further examination of gRPC shows it should work. Each sub-system has a port assigned to it that is exposed through docker. External callers can call it via localhost:port. This doesnt break loose coupling because a caller must know the interface already.

I don't think Gradle fits my needs. Its too based around Java. The primary language is Python and shell scripts. We also need to call a lot of external stuff like docker.