# Project Setup

The following are instructions on setting up a development environment for the cal-notify project.

## Pre-requisites
These are the pre-requisite packages and frameworks that will be required:
* .NET Core: https://www.microsoft.com/net/core. The SDK is required at a minimum, but you can install the tools as well if you are using Microsoft Visual Studio (highly recommended). Follow the instructions provided on the website for installation and configuration.
* PostgreSQL: https://www.postgresql.org/download/. Follow the instructions provided on the website for installation and configuration.
* PostGIS: http://postgis.net/install/. Follow the instructions provided on the website for installation and configuration.

## RESTful Server

We are leveraging .NET Core as the platform powering our restful API, so make sure you have the .NET Core SDKs installed and the required tooling for Visual Studio. Once that is complete, open the Visual Studio solution file for the project and you should be ready to go. See the Configuration section below on configuring the project for your local development environment.

### Data Persistence

For data storage PostgreSQL is our go to, and as such, it also must be installed. Make sure also to install and enable the PostGIS extension, which is used for performing geospatial queries.

### Configuration

For backend processes and other setup we need to have a set of api keys and paths setup for our specific environment.

An example configuration is listed below

```
{
  "Logging": {
    "IncludeScopes": false,
    "LogLevel": {
      "Default": "Debug",
      "System": "Information",
      "Microsoft": "Information"
    }
  },
  "ConnectionStrings": {
    "Development": "User ID=calnotify;Password=calnotify;Host=localhost;Port=5432;Database=calnotify;Pooling=true;",
    "Testing": "User ID=calnotify;Password=calnotify;Host=localhost;Port=5432;Database=calnotifytest;Pooling=true;"
  },

  "Services": {
  
     "Urls": {
      "Frontend": "http://localhost:3000",
      "Backend": "http://localhost:3002"
    },
    "Email": {
      "Validation": {
        "Name": "No Reply",
        "Address": "noreply@yourdomain.com",
        "Server": "email-smtp.your-server.com",
        "Port": 25,
        "Username": "email user name",
        "Password": "AqTXouLESsfakeoP5"
      }
    },
    "Pages": {
      "HelpPage": "help.html",
      "AdminPage": "admin.html",
      "SetPasswordPage": "validate.html",
      "ResendPage": "resend.html",
      "AccountPage": "account.html"
    },
    "Twillo": {
      "Id": "C39KK34e2bfake87857fae",
      "Number": "+12223334444",
      "SecretKey": "e712d987fake0706ce7a8"
    }
  }

}
```





## Swagger

Once the project is up and running go to the swagger docs which by default is http://localhost:3002/swagger/index.html

To complete the setup run the grunt default task in the `/front-end` folder which starts up the front-end server at http://localhost:3000/



