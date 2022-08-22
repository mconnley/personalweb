# personalweb


For dev purposes only, create a /secrets folder and place the below in /secrets/secrets.json
```
{
  "SiteCountsConnectionString": "Host=postgres;Database=database;Username=user;Password=pass"
}  
```

For dev purposes only, create a /configs folder and place the below in /configs/mainconfig.json
```
  {
      "siteKey": "foo",
      "visitorIdCookieName": "VisitorId",
      "monitorUrl": "healthz",
      "monitorAddedHeader": "someheader-here",
      "headerLogLevel": "Information"
  }
```
- siteKey: The db key name for the site in the webcounters table
- visitorIdCookieName: The name of the cookie for unique visitor tracking
- monitorUrl: The URL that the healthcheck endpoint responds to
- monitorAddedHeader: Requests with headers matching this string will not be counted in the hit counter middleware or logged. Add a header of this value to monitoring requests
