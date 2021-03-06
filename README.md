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
  - The concept works for XM only
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
⟹ Write a short clear step-wise instruction on how to install your module.  

> _A simple well-described installation process is required to win the Hackathon._  
> Feel free to use any of the following tools/formats as part of the installation:
> - Sitecore Package files
> - Docker image builds
> - Sitecore CLI
> - msbuild
> - npm / yarn
> 
> _Do not use_
> - TDS
> - Unicorn
 
f. ex. 

1. Start docker environment using `.\Start-Hackathon.ps1`
2. Open solution in Visual Studio and run build
3. Use the Sitecore Installation wizard to install the [package](#link-to-package)
4. ...
5. profit

### Configuration
⟹ If there are any custom configuration that has to be set manually then remember to add all details here.

_Remove this subsection if your entry does not require any configuration that is not fully covered in the installation instructions already_

## Usage instructions
⟹ Provide documentation about your module, how do the users use your module, where are things located, what do the icons mean, are there any secret shortcuts etc.

Include screenshots where necessary. You can add images to the `./images` folder and then link to them from your documentation:

![Hackathon Logo](docs/images/hackathon.png?raw=true "Hackathon Logo")

You can embed images of different formats too:

![Deal With It](docs/images/deal-with-it.gif?raw=true "Deal With It")

And you can embed external images too:

![Random](https://thiscatdoesnotexist.com/)

## Comments
⟹ Design has been adapted from Templated's BINARY design (https://templated.co/binary) to fit the structure of our Sitecore solution structure