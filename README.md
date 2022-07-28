# ShareXUploadApi
ASP.NET Core API Backend for ShareX(with Docker support)

[Docker Hub](https://hub.docker.com/r/solidprogramming/sharexuploadapi)


![ShareXApiLogScreenshot](https://lucaweidmann.de/cdn/sharexapilog.png)
![ShareXApiSwagger](https://lucaweidmann.de/cdn/sharexapiswagger.png)

## Getting started



### Unraid Docker
1. Login to your UnRaid dashboard. 
2. Go to tab 'Docker'.
3. Click on 'Add Container' button.
4. Fill fields
   - Repository => solidprogramming/sharexuploadapi
   - Docker Hub Url => https://hub.docker.com/r/solidprogramming/sharexuploadapi
   - (optional) Icon URL => https://getsharex.com/favicon.ico
5. Click on 'Add another Path...'
6. Fill fields
   - Config Type => Path
   - Name => Host Path
   - Container Path => /sharex
   - Host Path => /mnt/user/... (use your own share/folder)
   - Access Mode => Read/Write

   
