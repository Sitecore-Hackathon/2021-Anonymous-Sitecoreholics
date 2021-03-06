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
Our project is called S**pee**do - it's an abbreviation for Sitecore Poor man's Experience Edge. The concept is simple. The CM will persist layout service responses (using ILayoutService) and media items to a given persistent storage on publish and the rendering host will then read layout service content from here instead of calling the CD and media will be served statically from the rendering host using the File Server Option available. The following drawing summarizes the concept!
![Concept](docs/images/concept.jpg?raw=true "Concept")

### Benefits
  - See it as a cache without actually using memory cache. It's blazing fast and reacts as soon as a file is modified
  - Many runtimes can share the same persisted content
  - You can easily geo replicate the persisted content
  - It's easily usable in AKS with file shares or persistent volumes but could also easily be extended with Blob Storage, S3 buckets or other options
  - You can cut out the CD role entirely

### Limitations
  - The concept works for XM (without session personalization) only
  - Images cannot be cropped at this point in time

### Is it faster?
Testing the performance win on a small sample site with few components will not reveal the full potential of uplift but yes, it's faster for sure.

[TODO: Insert performance test results]

## Video link
⟹ Provide a video highlighing your Hackathon module submission and provide a link to the video. You can use any video hosting, file share or even upload the video to this repository. _Just remember to update the link below_

⟹ [Replace this Video link](#video-link)

## Pre-requisites and Dependencies

⟹ Does your module rely on other Sitecore modules or frameworks?

- List any dependencies
- Or other modules that must be installed
- Or services that must be enabled/configured

_Remove this subsection if your entry does not have any prerequisites other than Sitecore_

## Installation instructions
⟹ To preview our work, please follow these steps

1. Start docker environment using `.\Start-Hackathon.ps1`
2. Run dotnet tool restore
3. Run dotnet sitecore ser push
4. Run dotnet sitecore ser publish

#TODO add more if needed

### Configuration
⟹ No further configuration needed

## Usage instructions
⟹ Did you want't to use Speedo in your own solution. Here's what you need to know...

### Setting the File System
In the current implementation, we only implemented a File System provider and a stub of the Blob Storage Provider to show the idea from a rendering host perspective. To setup the file system, you need to follow these steps:
  - Create a folder which is accessible to both CM and rendering host! :-)
  - For the purpose of local development, add it in the solution directory with a .gitkeep: [see here](/tree/main/docker/data/speedo)
  - Add We would add a gitkeep 

### Setting up CM

### Setting up the rendering host for layout service

### Setting up the rendering host for serving images

Include screenshots where necessary. You can add images to the `./images` folder and then link to them from your documentation:

![Hackathon Logo](docs/images/hackathon.png?raw=true "Hackathon Logo")

You can embed images of different formats too:

![Deal With It](docs/images/deal-with-it.gif?raw=true "Deal With It")

And you can embed external images too:

![Random](https://thiscatdoesnotexist.com/)

## Comments
⟹ We have the following remarks
  - Design has been adapted from Templated's BINARY design (https://templated.co/binary) to fit the structure of our Sitecore solution structure
