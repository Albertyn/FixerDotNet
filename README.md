# FixerDotNet
Todo Forex Conversions using ECB Spot Rates.
This application can retreive Forgein Exchange Rates from an External source. http://fixer.io/ 

Data Stored in MongoDB is a replica of data retreived from Fixer. 
The Web Api can be extended to service currency conversion requests.

## Prerequisites
.Net Core
MongoDB

## Usage
The Api accepts 3 GET requests illustrated below.

**Import:** the app retreives ECB Spot rates from Fixer.io and persists the response to MongoDb.
```
GET: api/Rates/Import
```
**GET:** Making requests to the ULR's below retreives the data by a given date from MongoDb (local resource)
```
GET: api/Rates/Date/2017-02-18
GET: api/Rates/Date/2017-02-18/Base/JPY
```
