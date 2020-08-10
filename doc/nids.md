# Honeybee Network Intrusion Detection System

## Architecture

### Messages

| Message Name | Description |
| ----------- | ----------- |
| Start | Starts or restarts the NIDS. |
| Stop | Stops the NIDS |
| Upload Intel | Causes the NIDS to upload the given intel file. Intel is passed as a tab seperated string. The intel file determines what file to put it in. |
| Add Whitelists | Causes the NIDS to ignore traffic related to the addresses. Addresses are passed as a comma seperated string. An address can be a IPv4, IPv6, MAC, or domain name. |
| Remove Whitelists | Causes the NIDS to ignore traffic related to the address. |
