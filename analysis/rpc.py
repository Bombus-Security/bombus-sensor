from concurrent import futures
import logging

import grpc

import rpc_pb2
import rpc_pb2_grpc

class Analysis(rpc_pb2_grpc.AnalysisServicer):
    def UploadScript(self, request, context):
        return rpc_pb2.UploadScriptReply(upload_success=True, script_uuid="b")

def serve():
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
    rpc_pb2_grpc.add_AnalysisServicer_to_server(Analysis(), server)
    server.add_insecure_port('[::]:50051')
    server.start()
    server.wait_for_termination()


if __name__ == '__main__':
    logging.basicConfig()
    serve()