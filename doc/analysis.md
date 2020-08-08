# Honeybee Analysis System

Analysis is somewhat distributed and not cleanly seperated along sub-system lines. Each sub-system naturally performs some analysis, such as the NIDS. The analysis sub-system seeks to centralize this as much as possible and offer a place for loose scripts to be run. 

## Architecture

Analysis can be performed by the following triggers:
- On a schedule
- In response to a message
- In response to a write to a table
- As a RPC

Other sub-systems can upload scripts to the analysis sub-system. This should be preferred to performing analysis within a sub-system.

### Messages {#message-types}


