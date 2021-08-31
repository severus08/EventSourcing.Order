# DDD+CQRS+Event-Sourcing Example ![Hits](https://hitcounter.pythonanywhere.com/count/tag.svg?url=https://github.com/MrBugra/EventSourcing.Order)

### Dependencies
* Docker
* [EventStore](https://eventstore.org/) WriteDB
* [MongoDb](https://www.mongodb.com/) ReadDB

### Libraries
* EasyCaching
* EventStore.Client
* MediatR
* MongoDB.Driver
* AutoMapper
* Bogus (for testing)

### How To Build
`docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d`
You can run tests afterwards.

### Domain Models
* OrderAggregateRoot
 
# Read Models
* OrderReadModel
* TransactionCountReportReadModel

### Notes
* Eventstore and Mongo db must be running with default configrations for integration tests.