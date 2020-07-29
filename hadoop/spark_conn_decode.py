#./bin/spark-submit --packages org.apache.spark:spark-sql-kafka-0-10_2.12:3.0.0 spark_test.py 

from pyspark import SparkContext
from pyspark.sql.session import SparkSession
from pyspark.sql.functions import col

spark = SparkSession.builder.master("local").appName("Test PY App").getOrCreate()

from pyspark.sql.functions import UserDefinedFunction
from pyspark.sql.types import StringType
from pyspark.sql.functions import from_json
from pyspark.sql.types import StructType
import json

df1 = spark.readStream.format("kafka").option("kafka.bootstrap.servers", "172.16.3.129:9092").option("subscribePattern", "connection").load()
udf = UserDefinedFunction(lambda x: x.decode("utf-8"), StringType())
df2 = df1.withColumn("value", udf(df1.value))
schema_file = open("kafka_conn_schema.json")
new_schema = StructType.fromJson(json.load(schema_file))

#Remove the top level object "from_json"
schemadf = df2.select(from_json(col("value"), new_schema).alias("tmp")).select("tmp.*")

schemadf.printSchema()

#query = schemadf.writeStream.format("console").start()
#query.awaitTermination()
