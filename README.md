# svcdeploy - Windows Service remote deploy tool & agent

## Install agent

Copy agent files to install directory.

* Service.Deploy.Agent.exe
* appsettings.json
* services.json

```
sc create ServiceDeployAgent binPath=(install directory)\Service.Deploy.Agent.exe start=auto
```

```
sc start ServiceDeployAgent
```

## Setting agent

Edit services.json.

```json
{
  "Service": {
    "Entry": [
      {
        "Name": "WebApp",
        "Display": "Sample Service",
        "Token": "xxxxxxxxxx",
        "Directory": "D:\\WebApplication",
        "BinPath": "D:\\WebApplication\\WebApplication.exe"
      }
    ]
  }
}
```

The target service must be registered before the first deploy. The agent only stops and starts it, so the logon account and start type configured here are kept.

```
sc create WebApp binPath=D:\WebApplication\WebApplication.exe start=auto
```

## Setting certificate

The agent listens on HTTPS. Set the certificate in appsettings.json.

```json
{
  "Kestrel": {
    "Certificates": {
      "Default": {
        "Path": "sample.pfx",
        "Password": "sample"
      }
    }
  }
}
```

## Install client

```
dotnet tool install --global Usa.Tool.ServiceDeploy
```

## Use client

```
svcdeploy deploy -n WebApp -d D:\Project\WebApplication\bin\Release\net5.0\publish\ -u https://server:50443/ -t xxxxxxxxxx
```

```
svcdeploy config update -n WebApp -u https://server:50443/ -t xxxxxxxxxx
```

```
svcdeploy deploy -n WebApp -d D:\Project\WebApplication\bin\Release\net5.0\publish\
```
