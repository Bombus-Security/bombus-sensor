# Minimal makefile 
#

PROTOCBIN = python3 -m grpc_tools.protoc
PROTOCDIRS = -Ianalysis \
-Inids \
-Ihids \
-Idatabase

PROTOCFILES = analysis/analysis.proto \
nids/nids.proto

PROTOCPYOUT = proto
PROTOCGRPCOUT = proto

# Put it first so that "make" without argument is like "make help".
help:
	@$(PROTOCBIN) --help

protoc:
	@$(PROTOCBIN) $(PROTOCDIRS) --python_out="$(PROTOCPYOUT)" --grpc_python_out="$(PROTOCGRPCOUT)" $(PROTOCFILES)

.PHONY: help Makefile

