# ShareXUploadApi
ASP.NET Core API Backend for ShareX(with Docker support)

## [Docker Hub](https://hub.docker.com/r/solidprogramming/sharexuploadapi)
## [ShareX Custom Uploader Config](https://dl.lucaweidmann.de/wl/?id=jAhdwBAgpcHIIC8RBujkmZkJ49Ai1vFy)

![ShareXApiLogScreenshot](https://lucaweidmann.de/cdn/sharexapilog.png)
![ShareXApiSwagger](https://lucaweidmann.de/cdn/sharexapiswagger.png)
![ShareXCustomUploader](https://lucaweidmann.de/cdn/sharexcustomuploader.png)

## Getting started
### Prerequisites
## 1. MySQL Database
#### The table needed for uploads is created on the first request if the table doesn't exist.
```
CREATE TABLE IF NOT EXISTS `uploads` (
  `guid` varchar(50) COLLATE utf8mb4_bin NOT NULL,
  `filename` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  PRIMARY KEY (`guid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_bin;
```

## 2. Settings file [settings.json](https://dl.lucaweidmann.de/wl/?id=6SMZHXBRQzDx2S5ZZpGeMJS8F4Dw3uFP). Copy this file to the unraid appdata folder '/mnt/user/appdata/sharexuploadapi/'
```
{
  "pathSettings": {
    "dockerFolder": "/sharex/",
    "desktopFolder": "//myshare/somefolder/"
  },
  "dbSettings": {
    "ip": "ipaddressofdb",
    "database": "dbname",
    "username": "username",
    "password": "password"
  }
}
```

### :exclamation::exclamation:The "dockerFolder" setting needs to be the same as the unraid container path variable:exclamation::exclamation:


## 3. Proxy/Tunnel. You need a reverse proxy or cloudflare tunnel to access the api on your unraid system from the internet.


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
7. Click on 'Add another Path...' again
   - Config Type => Path
   - Name => Appdata
   - Container Path => /app/appdata
   - Host Path => /mnt/user/appdata/sharexuploadapi/
   - Access Mode => Read/Write
7. Cick on 'Apply' button.

   
