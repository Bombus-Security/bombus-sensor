from concurrent import futures
import logging

import json

import grpc

import rpc_pb2
import rpc_pb2_grpc

class NIDS(rpc_pb2_grpc.NIDSServicer):
    def StartNIDS(self, request, context):
        return rpc_pb2.StartNIDSReply()

    def StopNIDS(self, request, context):
        return rpc_pb2.StopNIDSReply()

    def _upload_intel_return_fail(self, message):
        return rpc_pb2.UploadIntelReply(success=False)

    def _test_intel_validity(self, intel):
        lines = intel.split("\n")
        fields = lines[0].split("\t")
        #Test the first and last lines of the fields
        assert(len(fields) == len(lines[1].split("\t")))
        assert(len(fields) == len(lines[:-1].split("\t")))

    def UploadIntel(self, request, context):
        """Attempts to upload some intel into the NIDS.
        """
        filename = request.filename
        intel = request.intel

        # Test validity of data
        try:
            self._test_intel_validity(intel)
        except:
            return rpc_pb2.UploadIntelReply(success=False, message="Intel failed validity")

        # Test the validity of the filename

        return rpc_pb2.UploadIntelReply(success=True)

def serve():
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
    rpc_pb2_grpc.add_NIDSServicer_to_server(NIDS(), server)
    server.add_insecure_port('[::]:50051')
    server.start()
    server.wait_for_termination()


if __name__ == '__main__':
    logging.basicConfig()
    serve()