Seven Update SDK
=============

The SDK is intended for developers only and is the main tool to create and manage updates. During the Seven Update installation, you can choose the option to install the SDK.

Getting Started
---------------
The SDK is how you will create and manage updates. You start by creating a project and filling out your app information.
Before you finish the first step you will need to pre-determine a public location where you will store the SUI file.

The update file location refers to the SUI file generated when you are finished creating an update. That file contains the list of updates and information needed. It needs to be a public readable location. It can be a network, filesystem or web server location.

You will also need to determine a location where to store the files needed for your update. It will also need to be a public directory.


Creating a new project
------------
After you determine where you are going to store the SUI and updated files you can create the project.

Start by filling out the application and publisher information.

###Application Install Location
If your application uses a registry path to determine it's installation location, you need to specify the key and value where to find the path. Otherwise if it's in a fix location you can use standard windows environment variables.

###Update File Location
The SUI file generated when you are finished creating an update. That file contains the list of updates and information needed. It needs to be a public readable location. It can be a network, filesystem or web server location.

###Application Type
Specificy if your application is 32 bit, 64 bit or a hybrid. Most .NET programs are hybrid, they will run 64 bit on 64 bit systems, and 32 bit on 32 bit systems. This setting is mainly used to help determine the install location.

###Localization
You can localize the strings by switching the current language by using the combobox in the top right corner.

Creating an update
------
Once you have your project information filled out it will auto create your first update.

###Update Name
You should have a short descriptive name for your update and it's recommended that it ends with your application name.

	Security update for MyAppUpdate
	
###Update Details
Provide a short overview of what the update entails. Use a paragraph format instead of a changelog. Keep it short and to the point.
You can include a link to a page that goes into detail.

###Software License URL
A URI to a public readable plain text file containg your software license. It will be displayed before a user can install your update. This field only applies to that specific update and is not application wide. So you can choose if you want to display your license for a specific update if you wish.

###Source Download Location
This should be a URI to a directory containing where the update files are stored. This should be the parent directory. Files can be located anywhere, but this field allows you to specify the default download location.

###Update Importance
Determine the importance of the update. Use Locale for updates that only consist of language translations.

Add Files
---------
You start by right clicking the ListBox and add file(s) or a folder.
By default the SDK will set the download location to your SourceDownloadLocation/Filename.exe
If the files you added are located in your installation directory you set in the project page, it will set the variable %INSTALLDIR%, otherwise it will use Windows Environment Variables when it can.

###Download Location
By default this is set to your source download location. You can change this to anything as long as it a URI that points directly to the file.

###Install Location
The SDK will generate the location based off the location of the files on your machine, you can make changes if needed.

### File action
* **Update** = Updates or adds the file
* **Update if the file exists** = Only updates the file if the client machine has the file installed. This is useful for optional files like plugins, language files or stuff that can be delected at installation time.
* **Update & Execute** = Updates or adds the file, then executes it and waits until it exits to move on.
* **Execute** = Executes a file. This is a special action. The file can already be on the system or it can be downloaded. You can use this action for advanced senarios. For example, you can download a update helper application to the tmp directory and execute it
* **Compare Only** = Compares a files hash against the clients hash, does not update or execute the file. This can be used for advanced senarios.  For example, you can use a file to determine if you need to run an update helper. This is often used along with the execute action. You can also compare your main exe file and then execute a seperate installer to do the actual updating.
* **Delete** = Deletes a file if it exists on the system
* **ExecuteThenDelete** = Executes a file, then deletes it. You would normally have the file download to the tmp folder in this case. This can be used for updates that just run a standalone installer package.

The execute commands set the window style to none, so no UI will be shown.

###Command args
Arguments used for the execute actions. Leave blank if you don't need to execute a file with arguments.

###File Hashes
When you add a file, the SDK calculates an SHA256 hash for the file along with the file size. Seven Update uses the hash and compares it to the client machines hash to determine if that file needed updated. Seven Update only downloads the file it needs.

Adding Registry Items
---------------------
Right click the listbox and add an item. You can import a .reg file to use or add the information manually.

This page should be self explainitory so no need to go into any detail here.

Adding Shortcuts
----------------
Right click the listbox andimport a .lnk file to use. You can modify the information presented.

###Update Only
Only updates if the shortcut already exists on the client machine, does not add it if it does not exist.

Saving Your Project
-------------------
When you press the save button, the files are stored in %appdata%\Seven Update SDK. You can also export your SUA and SUI files for deployment.

Deploying Updates
-----------------
Once your project is created, export the files on the review page or right clicking the list box on the start page.

You should upload your SUA, SUI, and update files to your webserver or other location and make sure the links you specified are working. If any errors occur during your update, they will be stored in the Error log found in

	%appdata%\Seven Update\error.log

###How can my clients use Seven Update to update my app?
There are a ways to have client machines update your application.

The first option is to install Seven Update when your application is installed and call some commandline arguments to add your application as default.

You could also offer a special link on your website that tells Seven Update to download your SUA file and prompt the user to allow Seven Update to manage updates for your app.

    sevenupdate://pathtomysuafile.sua
	
The third option is double clicking on the SUA file which does the same thing as the 2nd option.

Other Notes
-----------
Currently Seven Update uses SHA256 hashing to determine file updates. It also downloads the raw uncompressed files, which can be a waste of bandwidth. This will soon change to use binary patching with a easier deployment method. More on this can be found on the development blog at http://sevenupdate.com.
