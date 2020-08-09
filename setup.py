import setuptools

with open("README.md", "r") as fh:
    long_description = fh.read()

setuptools.setup(
    name="honeybee-sesnor", # Replace with your own username
    version="0.0.1",
    author="Leena Wilson",
    author_email="lwilson2048@gmail.com",
    description="The backend software for the Honeybee sensor.",
    long_description=long_description,
    long_description_content_type="text/markdown",
    url="https://github.com/Honeybee-Security/honeybee-sensor",
    packages=setuptools.find_packages(),
    classifiers=[
        "Development Status :: 3 - Alpha",
        "Programming Language :: Python :: 3",
        "License :: OSI Approved :: Apache Software License",
        "Operating System :: POSIX :: Linux",
    ],
    python_requires='>=3.6',
)