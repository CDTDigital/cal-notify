# Project Setup

We require a few packages to be installed for running the backend system.



## RESTful Server

We are leveraging .net core as the platform powering our Restful API, so make sure you have the sdk's installed and the required tooling. A quick google search of `.net core install` will take you to what you needed but for completeness here are the direct links as of late:

* [Visual studio tooling](https://www.microsoft.com/net/core#windowsvs2015)
* [sdk and runtime](https://www.microsoft.com/net/download/core#/current)




### Data Persistence

For data storage PostgreSQL is our go to, and as such, it also must be installed. Visit the [download page](https://www.postgresql.org/download/)  for more detailed instructions to set it up. 





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
   
    "Google": {
      "MapsKey": "fakeg3hw_azxR8u9fakegrQU"
    },
    "Urls": {
      "Frontend": "http://yourdomain.com",
      "Backend": "http://api-yourdomain.com"
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



