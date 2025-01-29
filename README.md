# personalweb

v0.5.0

For dev purposes only, create a /secrets folder and place the below in /secrets/secrets.json
```
{
  "SiteCountsConnectionString": "Host=postgres;Database=database;Username=user;Password=pass",
  "latestBlogUrl": "https://blog.mywordpressblog.net/wp-json/wp/v2/posts?per_page=1",
  "googleAnalyticsTagId": "G-FOO0BAR",
  "redisHostname": "cache"
}  
```

For dev purposes only, create an appsettings.json file, that looks like this. It is in gitignore and is instead mapped to a ConfigMap when running in k8s
```
{
  "Logging": {
    "LogLevel": {
      "Default": "None"
    },
    "NLog": {
      "IncludeScopes": true
    }
  },
  "AllowedHosts": "*",
  "siteKey": "mc",
  "visitorIdCookieName": "VisitorId",
  "monitorUrl": "healthz",
  "monitorAddedHeader": "personalweb-donotincrement",
  "NLog": {
    "autoReload": true,
    "internalLogLevel": "Info",
    "internalLogToConsole": true,
    "extensions": [
      {"assembly": "NLog.Web.AspNetCore"}
    ],
    "targets": {
      "consoleLog": {
        "type": "console",
        "layout": "${longdate}|${event-properties:item=EventId_Id}|${level:uppercase=true}|${logger}|${message} ${exception:format=tostring}"
      }
    },
    "rules":[
      {
        "logger": "Microsoft.*",
        "finalMinLevel":"Warn",
        "writeTo": "consoleLog"
      },
      {
        "logger": "*",
        "minLevel":"Debug",
        "writeTo": "consoleLog"
      }
    ]
  }
}
```
- siteKey: The db key name for the site in the webcounters table
- visitorIdCookieName: The name of the cookie for unique visitor tracking
- monitorUrl: The URL that the healthcheck endpoint responds to
- monitorAddedHeader: Requests with headers matching this string will not be counted in the hit counter middleware or logged. Add a header of this value to monitoring requests
