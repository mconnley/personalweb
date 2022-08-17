# personalweb


Requires a secret similar to:
```
{
  "ConnectionStrings": {
    "SiteCountsContext": "Host=postgres;Database=database;Username=user;Password=pass"
  }
}
```

For dev purposes only, create a /secrets folder and place the above in /secrets/secrets.json

For running inside k8s, create a generic secret similar to:
```
apiVersion: v1
kind: Secret
metadata:
  name: personalweb-appsettings-prod
  namespace: default
type: Opaque
data:
  secrets.json:|
{
  "ConnectionStrings": {
    "SiteCountsContext": "Host=postgres;Database=database;Username=user;Password=pass"
  }
}  
```