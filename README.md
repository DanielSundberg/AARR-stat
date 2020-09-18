# AARR-stat
Collect and display anonymous statistics for AARR (AARR RSS reader)

## Setup
* Requires MariaDb or MySQL on standard port
* Requires a db user/pwd root/root

Upon first run a new db (aarrusage) and a corresponding user (aarr/aarr) will be created.


## Publish 
First production credentials need to be added in *appsettings.prod.json*:

````javascript
{
  "ConnectionStrings": {
    "AdminConnection": "server=prod-url;uid=admin;pwd=****;database=aarrstat", 
    "AARRStatConnection": "server=prod-url;uid=aarr;pwd=aarr;database=aarrstat"
  }
}
````

Then create a bundle
````
dotnet publish --output .\publish --configuration Release /p:EnvironmentName=Prod
````