# AARR-stat
Collect and display anonymous statistics for AARR (AARR RSS reader)

## Setup
* Requires a DynamoDb database available in your aws account
* Enter your Aws details in appsettings.json

````javascript
  "AWSProfileName": "default",
  "AWSRegion": "eu-north-1", 
  
````



## Publish 
First production credentials need to be added in *appsettings.prod.json*:


Then create a bundle
````
dotnet publish --output .\publish --configuration Release /p:EnvironmentName=Prod
````

Create a zip file for uploading to AWS EB
````
del /s .\publish
del .\publish.zip
7z a -r -aoa publish.zip .\publish\*
````