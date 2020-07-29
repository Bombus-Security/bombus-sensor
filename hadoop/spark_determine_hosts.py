from pyspark import SparkContext
from pyspark.sql.session import SparkSession
from pyspark.sql.functions import col
from pyspark.sql.functions import UserDefinedFunction
from pyspark.sql.types import StringType
from pyspark.sql.functions import from_json
from pyspark.sql.types import StructType
import json

spark = SparkSession.builder.master("local").appName("Determine Hosts").getOrCreate()


