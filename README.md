# ShareXUploadApi
ASP.NET Core API Backend for ShareX(with Docker support)

## [Docker Hub](https://hub.docker.com/r/solidprogramming/sharexuploadapi)
## [ShareX Custom Uploader Config](https://dl.lucaweidmann.de/wl/?id=yxKbrNgBYilPXO4bZijA03wdtThDiJPY)

![ShareXApiLogScreenshot](https://img.lucaweidmann.de/s/sharexapilog.png)
![ShareXApiSwagger](https://img.lucaweidmann.de/s/sharexapiswagger.png)
![ShareXCustomUploader](https://img.lucaweidmann.de/s/sharexcustomuploader.png)

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
#### The table needed for authentication users is created on the first request if the table doesn't exist(the default user is admin:admin).
```
CREATE TABLE IF NOT EXISTS `users` (
  `id` int NOT NULL AUTO_INCREMENT,
  `username` varchar(50) COLLATE utf8mb4_bin NOT NULL,
  `password` varchar(50) COLLATE utf8mb4_bin NOT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_bin;
INSERT INTO `users` (`id`, `username`, `password`) VALUES
	(1, 'admin', 'admin');
```

![ShareXCustomUploader](https://img.lucaweidmann.de/s/sharexauthwithinsomnia.png)

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

   
