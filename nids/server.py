from concurrent import futures
import logging

import grpc

from ..proto import nids_pb2
from ..proto import nids_pb2_grpc

from . import rpc

def serve():
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
    nids_pb2_grpc.add_NIDSServicer_to_server(rpc.NIDS(), server)
    server.add_insecure_port('[::]:50051')
    server.start()
    server.wait_for_termination()


if __name__ == '__main__':
    logging.basicConfig()
    serve()