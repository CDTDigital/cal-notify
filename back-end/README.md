# Project Setup

The following are instructions on setting up a development environment for the cal-notify project.

## Pre-requisites
These are the pre-requisite packages and frameworks that will be required:
* .NET Core: https://www.microsoft.com/net/core. The SDK is required at a minimum, but you can install the tools as well if you are using Microsoft Visual Studio (highly recommended). Follow the instructions provided on the website for installation and configuration.
* PostgreSQL: https://www.postgresql.org/download/. Follow the instructions provided on the website for installation and configuration.
* PostGIS: http://postgis.net/install/. Follow the instructions provided on the website for installation and configuration.
* Node.js: https://nodejs.org/en/download/. Follow the instructions provided on the website for installation and configuration.
* Grunt: https://gruntjs.com/getting-started. Requires Node.js first. Follow the instructions provided on the website for installation and configuration.

## Back End (RESTful Server)

We are leveraging .NET Core as the platform powering our restful API, so make sure you have the .NET Core SDKs installed and the required tooling for Visual Studio. Once that is complete, open the Visual Studio solution file for the project and you should be ready to go. See the Configuration section below on configuring the project for your local development environment.

### Data Persistence

For data storage PostgreSQL is our go to, and as such, it also must be installed. Make sure also to install and enable the PostGIS extension, which is used for performing geospatial queries.

### Configuration

Configuration for the ASP.NET core backend happens through the "appsettings.json" file. An example configuration is listed below.

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

The items above will need to be adjusted in terms of configuration for local development as follows:
* Logging - for the appropriate level of logging needed.
* Connection Strings - so that you can connect to the appropriate Postgres instances on your local development machine.
* Service API Keys - for geocoding and Google Maps access.
* Email Settings - for the system to send notification e-mails.
* Twillo - configurations for the Twillo API that sends SMS.

### Swagger

Once the project is up and running, go to the swagger documentation URL, which is http://localhost:3002/swagger/index.html by default. This is usually the first test that ensures all is running as expected for the backend. You can also hit some of the API endpoints to ensure functionality, but at this point, you can start development.

## Front End (Public and Admin UI)

Open a command prompt at the front-end folder of the project (where you will see a "project.json" file). Run "npm install" at the command prompt, which will download and install the necessary packages needed for front-end development. This is required for compilation of less files and JavaScript files via the Grunt task runner.

Once installation is complete, run "grunt" at the command prompt, which will compile the front-end project, launch a BrowserSync server, and automatically invoke a web browser for the front-end project (usually navigating to http://localhost:3000/). The browser will automatically refresh as changes are made to files in the front-end project.
