# AARR-stat
Collect and display anonymous statistics for AARR (AARR RSS reader)

## Setup
* Requires a DynamoDb database available in your aws account
* Enter your Aws details in appsettings.json

First add a appsettings.development.json (and optionally an appsettings.prod.json):
````
>echo {} > appsettings.development.json
>echo {} > appsettings.prod.json
````

Then add your aws IAM key id and secret to the appsettings files:

````javascript
{
    "AWS": {
        "AWSProfileName": "default",
        "AWSRegion": "eu-north-1", 
        "AWSAccessKey": "<your aws IAM access key id>",
        "AWSSecretKey": "<you access key>"
    }
}
````

Note: if no aws config is added the IAM user added to your aws cli/sdk will be used.


## Publish 

Create a zip file for uploading to AWS EB
````
> del /s /q .\publish
> dotnet publish --output .\publish --configuration Release /p:EnvironmentName=Prod
> del .\arrstat*.zip
> dotnet build .\AARR-stat.csproj /target:CreateZip
Zipping directory "C:\...\AARR-stat\Publish" to "C:\...\AARR-stat\aarrstat-2020-10-21-084013.zip"
````
Now copy this file to your S3 bucket used for EB:
````
aws s3 cp aarrstat-2020-10-21-084013.zip s3://<your s3 eb bucket>/
````

Create new application version
````
# Use git sha as version label
> git rev-parse --short HEAD
cd77e8d

# Create application version
> aws elasticbeanstalk create-application-version --application-name aarrstat --version-label version-cd77e8d --source-bundle S3Bucket="<your s3 eb bucket>",S3Key="aarrstat-2020-10-21-084013.zip"
{
    "ApplicationVersion": {
        ...
        "ApplicationName": "aarrstat",
        ...
        ...
        "Status": "UNPROCESSED"
    }
}

# Update EB environment with the new version
> aws elasticbeanstalk update-environment --application-name aarrstat --environment-name some-env --version-label version-cd77e8d
{
    "EnvironmentName": "some-env",
    ...
    "ApplicationName": "aarrstat",
    ...
    ...
    "Status": "Updating",
}
````