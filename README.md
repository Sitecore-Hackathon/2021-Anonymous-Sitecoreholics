![Hackathon Logo](docs/images/hackathon.png?raw=true "Hackathon Logo")

# Sitecore Hackathon 2021

- MUST READ: **[Submission requirements](SUBMISSION_REQUIREMENTS.md)**
- [Entry form template](ENTRYFORM.md)
- [Starter kit instructions](STARTERKIT_INSTRUCTIONS.md)

## Team name

⟹ Anonymous Sitecoreholics

## Category

⟹ Best use of Headless using JSS or .NET

## Description

Our project is called S**pee**do - it's an abbreviation for Sitecore Poor man's Experience Edge. The concept is simple. The CM will scrape and persist layout service responses and media items to a given persistent storage on publish and the rendering host will then read layout service content from here instead of calling the CD and media will be served statically from the rendering host using the File Server Option available. The following drawing summarizes the concept!
![Concept](docs/images/concept.jpg?raw=true "Concept")

### Benefits

- See it as a cache without actually using memory cache. It's blazing fast and reacts as soon as a file is modified
- Many runtimes can share the same persisted content
- You can easily geo replicate the persisted content
- It's easily usable in AKS with file shares or persistent volumes but could also easily be extended with Blob Storage, S3 buckets or other options
- You can cut out the CD role entirely

### Limitations

- The concept works for XM (without session personalization) only
- Images cannot be cropped at this point in time. This would require mimicking the media handler from Sitecore on the rendering host instead of using a File Server or by persisting media in consumed crops
- The Snapshot publisher from CM doesn't clear deleted items at this point. It will require adding a head file to track snapshots and number of snapshots to retain

### Is it faster?

Testing the performance win on a small sample site with few components will not reveal the full potential of uplift but yes, it's faster for sure.

[TODO: Insert performance test results]

## Video link

⟹ Provide a video highlighting your Hackathon module submission and provide a link to the video. You can use any video hosting, file share or even upload the video to this repository. _Just remember to update the link below_

⟹ [Replace this Video link](#video-link)

## Installation instructions

⟹ To preview our work, please follow these steps

1. Make sure to place your license file in `.\license` **AND** `.\docker\License` (The starter kit seems not to handle this correctly)
1. Start docker environment using `.\Start-Hackathon.ps1`
1. Build solution! (normally the Docker build of the solution should handle this but the starter kit structure seems to not work as expected)
1. Run `dotnet tool restore`
1. Run `dotnet sitecore login --authority https://id.speedo.localhost --cm https://cm.speedo.localhost --allow-write true`
1. Run `dotnet sitecore ser push`
1. Run `dotnet sitecore publish -p /sitecore/content/SpeedoDemo`

Now you can visit https://www.speedo.localhost/

### Configuration

⟹ No further configuration needed

## Usage instructions

⟹ Did you want to use S**pee**do in your own solution. Here's what you need to know...

### Setting the File System

In the current implementation, we only implemented a File System provider and a stub of the Blob Storage Provider to show the idea from a rendering host perspective. To setup the file system, you need to follow these steps:

- Create a folder which is accessible to both CM and rendering host! :-)
- For the purpose of local development, add it in the solution directory with a .gitkeep
- If you are using Docker, create volumes the bind the S**pee**do folder to the local drives within CM and rendering host

### Setting up CM

Deploy these files:

- ".\src\Feature\SitecorePublisher\sitecore\bin\Speedo.Feature.SitecorePublisher.dll"
- ".\src\Feature\SitecorePublisher\sitecore\App_Config\Include\Feature\SitecorePublisher\Speedo.Feature.SitecorePublisher.config"

Add a config patch:

```xml
<configuration xmlns:patch="http://www.sitecore.net/xmlconfig/">
    <sitecore>
        <speedo>
            <storage>
                <sources hint="raw:AddSource">
                    <!--
                      site: Sitecore site name, all items with layouts will be saved.
                      media: Path to root media library folder for the site, all media blobs will be saved.
                      apiKey: Key allowing access to headless endpoints.
                      output: Root file path (that is shared with Rendering Host)
                    -->
                    <source site="speedo" media="/sitecore/media library/SpeedoDemo" apiKey="3c22a88c-600a-414b-87ca-2aee4e998fa4" output="C:\speedo\speedo" />
                </sources>
            </storage>
        </speedo>
    </sitecore>
</configuration>
```



### Setting up the rendering host for layout service

To setup the rendering host to use the persisted layout service results, you need to do follow these steps:

- Simply replace the existing HttpHandler with the SpeedoHandler with disk persistency in **Startup.cs**. See before and after below
- Also, you should add configuration in **appSettings.json**
  - Section should be called **Speedo**
  - Add a setting **LayoutServiceContentFilePath** with value such as c:\\speedo\\{sitename}\\content (local container path if Docker is used)

AddSitecoreLayoutService() Before
![Before](docs/images/traditional-setup.jpg?raw=true "Before")

AddSitecoreLayoutService() With S**pee**do
![After](docs/images/speedo-setup.jpg?raw=true "After")

### Setting up the rendering host for serving images

To use static images, you need to configure a File Server in the rendering host. It's done easily in Startup.

- In the ctor initialize options with SpeedoConfiguration = configuration.GetSection(SpeedoOptions.Key).Get<SpeedoOptions>();
- The values for the options are in **appSettings.json**
  - Section should be called **Speedo** (the same section as used for the other configuration)
  - Add a setting **MediaLibraryFilePath** with value such as c:\\speedo\\{{sitename}}\\media\\ (local container path if Docker is used)
  - Add a setting **MediaLibraryPath** with the value /-/jssmedia. This is the virtual folder that media will be served from
- In the Configure method add the File Server with the below lines

  ```csharp
  app.UseFileServer(new FileServerOptions
  {
      FileProvider = new PhysicalFileProvider(SpeedoConfiguration.MediaLibraryFilePath),
      RequestPath = new PathString(SpeedoConfiguration.MediaLibraryPath)
  });
  ```

## Comments

⟹ We have the following remarks

- Design has been adapted from Templated's BINARY design (https://templated.co/binary) to fit the structure of our Sitecore solution structure.
- Experience Editor is working, but without our custom styling.
