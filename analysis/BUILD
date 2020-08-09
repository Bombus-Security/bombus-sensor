load("@org_pubref_rules_protobuf//python:rules.bzl", "py_proto_compile")

py_proto_compile(
  name = "analysis",
  protos = ["rpc.proto"],
  with_grpc = True, # only one file is generated with or without grpc
)