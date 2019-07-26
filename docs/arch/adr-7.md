## ADR 7: Publishing with ClickOnce
July 26, 2019

### Context
The app needs a convenient way to deploy and publish to multiple devices. This process should be easiest for the user wishing to download the app, so the process of downloading an update is as simple as possible. Ideally, the developer would make any necessary changes to the app, publish those changes to a location, and someone on another end would only need to double click a download link.

### Decision
Older WPF and WinForm applications used a separate setup application as the standard, which was responsible for stepping someone through the process of installing an app on a system. The setup application was downloaded, which would be run to install the main application on a system. Although we went this route early in the development process, it was abandoned due to too many errors that would pop up with the install process.

ClickOnce now seems to be the standard for WPF applications. Visual studio even references ClickOnce under the "Security" tab of the properties window. There is a good bit of documentation on ClickOnce, and most articles concerning publishing mention ClickOnce. Because of this, we will accept ClickOnce as our method of choice when publishing.

### Status
Accepted

### Consequences
There aren't many options to choose from when it comes to deploying a WPF application. Unfortunately, if ClickOnce doesn't suit our needs easily, we'll likely have to spend a good bit of time troubleshooting.

_In Retrospect:_ Unfortunately, it is recommended that ClickOnce deploys to a CD-Rom, a website, or a file share/UNC path. This seems particularly dated with the CD-Rom reference. Also, the Adler Planetarium is deprecating their local file share system in lieu of using Google Drive. This is why we use Google Drive File Stream (see wiki) when publishing, which works just _fine_. As WPF isn't necessarily cutting-edge technology, I don't see the publishing process evolving much in the future. While ClickOnce _usually_ works well, we'll have to deal with any errors in deployment as we have our hands tied with using ClickOnce.
