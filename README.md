# ShareXUploadApi
ASP.NET Core API Backend for ShareX(with Docker support)

[Docker Hub](https://hub.docker.com/r/solidprogramming/sharexuploadapi)


![ShareXApiLogScreenshot](https://lucaweidmann.de/cdn/sharexapilog.png)
![ShareXApiSwagger](https://lucaweidmann.de/cdn/sharexapiswagger.png)

## Getting started
### Prerequisites
1. MySQL Database
#### The table needed for uploads is created on the first request if the table doesn't exist.
```
CREATE TABLE IF NOT EXISTS `uploads` (
  `guid` varchar(50) COLLATE utf8mb4_bin NOT NULL,
  `filename` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  PRIMARY KEY (`guid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_bin;
```

3. Connection String in appsettings.json. Should look link this([appsettings.json](https://dl.lucaweidmann.de/wl/?id=yTvXiGnH1Zck94ZiW3DRnt9tB5vwSpCv)):
```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "server=XXXX;port=XXXX;database=XXXX;user=XXXX;password=XXXX;"
  }
}
```
3. settings.json. Located in folder 'ShareXUploadApi\ShareXUploadApi'([settings.json](https://dl.lucaweidmann.de/wl/?id=6SMZHXBRQzDx2S5ZZpGeMJS8F4Dw3uFP)).
```
{
  "pathSettings": {
    "dockerFolder": "/sharex/",
    "desktopFolder": "//127.0.0.1/mysharedfolder"

  }
}
```
### :exclamation::exclamation:The "dockerFolder" setting needs to be the same as the unraid container path variable:exclamation::exclamation:


### Unraid Docker
1. Login to your UnRaid dashboard. 
2. Go to tab 'Docker'.
3. Click on 'Add Container' button.
4. Fill fields.
   - Repository => solidprogramming/sharexuploadapi
   - Docker Hub Url => https://hub.docker.com/r/solidprogramming/sharexuploadapi
   - (optional) Icon URL => https://getsharex.com/favicon.ico
5. Click on 'Add another Path...'
6. Fill fields.
   - Config Type => Path
   - Name => Host Path
   - Container Path => /sharex
   - Host Path => /mnt/user/... (use your own share/folder)
   - Access Mode => Read/Write
7. Cick on 'Apply' button.

   
