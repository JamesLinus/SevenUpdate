Seven Update
=============

Seven Update is an open source update platform for windows. It allows an easy way to keep your software updated. Much like windows update, it offers automatic update notification and installation. If your familiar with Linux, think of a package manager for Windows, the difference being Seven Update does not officially support installation of programs. It allows developers to easily use Seven Update for their update distribution needs without needing any code. Seven Update is open source and licensed under the GNU GPL v3.0

SDK
-------

The SDK is intended for developers only and is the main tool to create and manage updates. During the Seven Update installation, you can choose the option to install the SDK.

Getting Started
------------

### Seven Update

At the current time, Seven Update does not ship with any applications in the offical application list. So by default, Seven Update can only check for updates for itself and the SDK.
This will change in the future as publisher add offical support for Seven Update, they can request to be added to the offical list.

On the Change Settings page you can configure automatic updates. It will also list any apps that are installed on your system that can work with Seven Update.

### SDK

The SDK is how you will create and manage updates. You start by creating a project and filling out your app information.
Before you finish the first step you will need to pre-determine a public location where you will store the SUI file and app files.

[View the SDK documentation for further information.][2]

Files Types Explained
------
* **SUI - Seven Update Information** - Contains a list of updates and data for the application updates.
* **SUA - Seven Update Application** - Contains data including the app name, install location, publisher information, and a location where to find the corresponding SUI file.
* **SUL - Seven Update List** - Acts like a reposistory for SUA files. It's an array containing SUA items.


Contributing
------------
VS 2010 or later with .NET 4 is required. Please also install Resharper and Stylecop and use the settings file found in the repo.

1. Fork it.
2. Create a branch (`git checkout -b sevenupdate`)
3. Commit your changes (`git commit -am "My commit message"`)
4. Push to the branch (`git push origin sevenupdate`)
5. Create an [Issue][1] with a link to your branch
6. Enjoy a refreshing Diet Coke and wait

[1]: http://github.com/sevenalive/sevenupdate/issues
[2]: https://github.com/sevenalive/sevenupdate/blob/master/sdk.readme.md