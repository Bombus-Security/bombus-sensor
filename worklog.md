### 8-Aug-2020

Debating whether to use DBus or RabbitMQ. We need RPC which RabbitMQ doesn't really support. We also need a manageable way to define the protocols such as ProtoBuf. RabbitMQ can work with Protobufs. gRPC is too distributed, I want a single broker.