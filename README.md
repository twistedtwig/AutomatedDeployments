Automated Deployments
=====================

The goal of this project is to simplify the process and control of IIS  (7.x) settings and setup / deployments.  Visual studio allows for nice pushing of files and IIS configuration from a development machine, (at least a machine with IIS setup as you want it).  If however you want to do CI / CD (continuous deployment) and do not want to / can't setup your CI server to have all the IIS settings you require for your deployment you are a bit stuck.

One solution to the problem above is to say, "well I am just setting up the web server once, I will do it manually then us MsDeploy or FTP to push my changes up".  This is valid and will work in many situations.  There is (at least) one draw back to this approach, your developers!  They will be developing the system using their own IIS (most likely), they will have to setup their IIS, (or let VS do it for them).  Does this mean that all the developers will set it up the same as each other, and that the setup will be the same for deployments?

Automated Deployments was designed to address this issue.  It has a number of benefits:

1.  Allows IIS Settings to be put into Source Control.
2.  Allows developers to use the same setup and deployment process when ever they build their solution with the same settings as will be used in the live system.
3.  Automates the deployment process.

1) How does it work?
--------------------

The application uses configuration files gained from IIS via "appcmd.exe".  These configuration files can then be put into your source control process.  The application would then be configured to use these configuration files to create IIS app pools, web sites, and applications on an IIS Server, (be that locally or remotely).  It can also be configured to copy all the files to a drop location.

2) What does it need?
---------------------

The application has a few requirements:

1.  .Net V4.0 needs to be installed on the client  / source machine.
2.  MsDeploy needs to be installed on the source machine (if the deployment / destination machine is not the same as the source).
3.  MsDeploy needs to be installed on the destination machine (if the source  machine is not the same as the destination).
4.  Appcmd needs to be installed on the source machine (if this machine is to be used to get IIS configuraiton settings to be put into source control).
5.  Appcmd needs to be installed on the destination machine (if any IIS settings are to be updated).

3) How to install MsDeploy?
---------------------------

You will need MsDeploy installed and setup correctly on your production / live / destination server(s).  Here is a reasonable guide from MS: http://technet.microsoft.com/en-us/library/dd569059(v=ws.10).aspx .  The main thing to be aware of is that it needs a port opened to communicate.  By default it wants to run on port 80.  This is not great if you already have websites on there as it can / could interfere.  I have found it best to install it on port 8172 or 8173 (sometimes there is already a process running on 8172).  To install it so it is not running on port 80 you need to use the command line:

msiexec /I [msi_filename] /passive ADDLOCAL=ALL LISTENURL=http://+:8172/MsDeployAgentService/

This will need to be run on cmd that is running as admin.  If when it installs it rolls back it is pretty possible something is already running on that port, try 8173 instead.  Remember to open the firewall as well to allow traffic in.

4) How to add your IIS configuration to Source Control?
-------------------------------------------------------

There are two ways of getting your IIS settings:

1.  Use the GUI tool to backup your settings (currently still under development and not functioning)
2.  use Appcmd to backup the settings.

4.1) Using the GUI:
-------------------

Not ready yet................

4.2) Using Appcmd:
------------------

Appcmd is a powerful tool designed to help administer IIS (7.x).  Here is a guide to getting started with Appcmd, http://learn.iis.net/page.aspx/114/getting-started-with-appcmdexe/ .  There are a few command we will need to use.  I am going to imagine you have a website on your machine called "stackoverflow", an appPool called "SOAppPool" and an application running within "Default Web Site" called "reddit".

To backup the AppPool you would run the following command:

C:\Windows\System32\inetsrv\appcmd.exe list apppool SOAppPool /config /xml > C:\SOAppPool.config

To backup the website (which will include all the applications within it), run the following command:

C:\Windows\System32\inetsrv\appcmd.exe list site stackoverflow /config /xml > C:\stackoverflowSite.config

To backup the application running under "Default Web Site", run the following command:

C:\Windows\System32\inetsrv\appcmd.exe list app "Default Web site/reddit" /config /xml > C:\redditApp.config

These files can now be added to your source control ready for future deployments.


5) Automated Deployment Fuctions:
---------------------------------

The application has a number of functions, be it a local or remote machine:

1.  Backuping up IIS Settings to file for Source Control.
2.  Copying files to a destination location.
3.  Give Full Permisisons to a given folder and its children for the given username.
4.  Removing files from a destination location.
5.  Setting up App Pools on a destination machine.
6.  Setting up web sites on a destination machine.
7.  Setting up applications on a destination machine.
8.  Removing App Pools from a destination machine.
9.  Removing web sites from a destination machine.
10.  Removing applications from a destination machine.
11. Create project packages (normally web projects)
12. deploy project packages.

5.1) How is Automated Deployments used?
---------------------------------------

To run the app you will need to specify the configuration section you want to use.  There are a number of other parameters you can give that will override any settings in the configuration:

To run the app you would call "MYAPPCONSOLE.EXE /CONFIGSECTION MYCONFIGSECTION"

This would look for a config section with the name of "MYCONFIGSECTION".  A list of the parameters available are given below:

    param				|		value								Description
    __________________________________________________________________________________________________________________________________________________
    /CONFIGSECTION		|		MYCONFIGSECTION					| The deployment configurationgroup name required
    /FORCE				|		true / false					| Weather the application should force each action to happen
    /CONFIGPATH			|		C:\MYPATHTOCONFIGFILE.CONFIG	| The full path to the configuration file
    /BREAKONERROR		|		true / false					| Should the application stop when an error occurs
    /CLEANUP            |       true / false					| Should the application delete any files it creates after running
	/SETEXEPATH			|		true / false					| Should the applicaiton set the executing folder to the same as the exe's file path, (default true)
	/VERBOSE			|		true / false					| should the application log verbosely or normally, (default is none verbose.. i.e. normal logging level)?
	
The above parameters are all optional and would override anything given within the configuration file.

5.2) How is Automated Deployments configured?
---------------------------------------------

To setup a process you will need to make some entries in either the app.config for the EXE or create your own configuration file and pass that in at runtime.  Firstly a configuraitonSection will need to be added:
```xml
<configSections>
    <section name="deployments" type="CustomConfigurations.ConfigurationSectionLoader, CustomConfigurations"/>
  </configSections>
```
This tells .Net to parse a custom section called "deployments" using our application.  The next step is to create our own configuration section to be able to run an automated deployment, (be careful as all elements are case sensitive):
```xml
<deployments>
    <Configs>

    </Configs>
</deployments>
```
Lets start by showing one quick example and then expand that to show each type of Task that can be used:
```xml
<deployments>
    <Configs>
		<ConfigurationGroup name="localtestSetup">
			<ValueItems>
			  <ValueItem key="AppCmdExe" value="C:\Windows\System32\inetsrv\appcmd.exe"/>
			  <ValueItem key="DestinationComputerName" value="localhost"/>          
			</ValueItems>
			<Collections>        
                <Collection name="MsDeployLocations">
                    <ValueItems>
                        <ValueItem key="location1" value="C:\Program Files\IIS\Microsoft Web Deploy V2\msdeploy.exe"/>
                        <ValueItem key="location2" value="C:\Program Files (x86)\IIS\Microsoft Web Deploy V2\msdeploy.exe"/>
                    </ValueItems>    
                </Collection>
			     <Collection name="CopyWebsite">
				    <ValueItems>
				        <ValueItem key="ComponentType" value="FileDeployment"/>
				        <ValueItem key="SourceContentPath" value="C:\temp\deploy\Website"/>
				        <ValueItem key="DestinationContentPath" value="c:\websites\deploytest"/>                        
				    </ValueItems>
			     </Collection>     
			</Collections>
		</ConfigurationGroup>
    </Configs>
</deployments>
```

The above ConfigurationGroup is called "localtestSetup".  This is the section name that would be passed in at the command line to run this (/CONFIGSECTION=localtestSetup).  It then declares a list of items.  These are global values to the deployment and will be inherited to each child element.  The system needs to know certain bits of information:

1.  where msdeploy is installed, (will default to the above if it is not given)
2.  where appcmd is installed, (will default to the above if it is not given)
3.  The destination computer name.

The minimum that is required he would be the destinationComputerName.  This tells the application where things will be going.  If localhost is used it will consider it to be doing a local deployment so msdeploy would not be needed.  Otherwise it will consider it to be doing a remote deployment and need to invoke msdeploy.  If it is doing a remote deployment there are two other valueItem's that will almost certainly be required, (the destination machine should always be secured, do not allow anonymous access):
```xml
	<ValueItem key="DestinationUserName" value="username"/>
    <ValueItem key="DestinationPassword" value="password"/>
```
After the "ValueItems" collection there is a new element called "Collections".  This holds a list of "Collection" elements.  Each of these elements is a set of values which will result in one or more Tasks being invoked.  The above example shows a File Copy Task being setup.  The first ValueItem is the "ComponentType", this tells the application what it wants to do.  In this case, deploy some files.  The next two ValueItem's give the source and destination location (folders).

When the application is run it will parse the configuration file, look for the section name given as a command line parameter, find that configurationGroup and run those Tasks in order. If BREAKONERROR is set to false, it will run through all the Tasks even if some of them return errors, otherwise it will exit after the first error is encountered.

A point of interest with the above xml snippet.  "AppCmdExe" is a single entry in the global ValueItems section, where as the "MsDeployLocations", (i.e. the path to MsDeploy) has its own section and has multiple entries.  The reason for this is that it depends if you are running a 32bit or 64bit machine.  As it can be in multiple places the application has the ability to check multiple locations till it finds a match then uses that.  It is not actually necessary to enter these as it has internal defaults which will add the two values everytime, but if for some reason your MsDeploy was at a different location you could add that value and it will use the first match it finds, (defaults and yours included).  The keys for each location need to be entered but it doesn't matter what they are, just note that they can not be excluded or left blank.

"AppCmdExe" is also defaulted to the above location so there is no need to enter that unless yours is different to this, (only one path can be given for appCmd, I am not aware of it ever living at multiple locations).

5.3) Global Configuration ValueItem's
-------------------------------------

There are a number of settings that are global and are defined within the ValueItems collection at the beginning of the ConfigurationGroup.

    param						|		value 				|	optional 	|	default
    _________________________________________________________________________________________
    DestinationComputerName		|		string      		|	false		| 	NONE
    DestinationUserName			|		string				|	true		|	NONE
    DestinationPassword			|		string				|	true		|	NONE
    ForceInstall				|		bool				|	true		|	false
    CleanUp                     |       bool                |   true        |   true
    SourceContentPath			|		string		    	|	false		|	NONE
    DestinationContentPath		|		string				|	false		|	NONE
    AppCmdExe					|		file path			|	false		|	"C:\Windows\System32\inetsrv\appcmd.exe"
    ShouldSortTasks             |       bool                |   true        |   false

If CleanUp is set to true it will delete all files the application generates, (this doen't include files that are used but not generated by the application, such as IIS config xml files that are used by appcmd), once the files have been used.

The secions below will deal with setting up each of the ComponentType's.

List of all ComponentType's:
----------------------------

1.  FileDeployment   
2.  FilePermission     
3.  AppPoolCreation
4.  AppPoolRemoval
5.  WebSiteCreation
6.  WebsiteRemoval
7.  AppCreation
8.  AppRemoval
9.  ApplicationExecution
10.  CreatePackage
11. DeployPackage

N.B A quick note on Task ordering.  The application will try and create all tasks in the order they are given in the configuration section.  Most of the time this is logical and the desired behaviour.  However if you are trying to force a task through, such as an App Pool install and uset the "ForceInstall" element, (set to 'true'), then it is generally wise to sort the tasks using the 'ShouldSortTasks' element.  The reason being when a ForceInstall is given it will create any tasks needed to achieve that goal.  For example to force an install for an App Pool and a Web Site you would need to remove the Web Site then the App Pool, but will need to Install the App Pool before installing the Web Site.  You can either explicity create each of these Tasks, or you can use the ForceInstall and ShouldSortTasks elements.  This will create the needed Tasks and order them in the correct manor, so all applications are removed in order, files copied and lastly all installs done in the correct order.

5.4) Backuping up IIS Settings to file for Source Control
---------------------------------------------------------

This has been covered above, (section 4, 4.1 and 4.2).

5.5) Copying files to a destination location (Component Type: FileDeployment)
-----------------------------------------------------------------------------

There are two types of file copy, local or remote, depending on the DestinationComputerName given.  As stated above if the DestinationComputerName is "localhost" it will do a local copy, otherwise it will use msdeploy to sync the files to the remote machine.  Here is a list of the parameters available to FileDeployment:

    param						|		value 				|	optional
    ________________________________________________________________________
    ComponentType				|		FileDeployment		|	false
    DestinationComputerName		|		string      		|	false 
    DestinationUserName			|		string				|	true
    DestinationPassword			|		string				|	true
    ForceInstall				|		bool				|	true
    CleanUp                     |       bool                |   true
    SourceContentPath			|		string		    	|	false
    DestinationContentPath		|		string				|	false
    
Each parameter given above would need its own ValueItem.  Here is an example FileDeployment section:

```xml
<Collection name="CopyWebsite">
	<ValueItems>
	  <ValueItem key="ComponentType" value="FileDeployment"/>
	  <ValueItem key="SourceContentPath" value="C:\temp\deploy\Website"/>
	  <ValueItem key="DestinationContentPath" value="c:\websites\deploytest"/>                        
	</ValueItems>
</Collection>  
```


5.6)  Giving Modify Permissions to a given user for a folder and all its children (Component Type: FilePermission)
------------------------------------------------------------------------------------------------------------------

At this point the system only allows you to give modify permisisons to a folder and all its children.  There is a future task to allow this to delete permisisons ang assign different permission levels.  There is another task to allow lists of either users or paths so that there isn't repeated configuration.  The parameters required are:

    param                       |       value               |   optional
    ________________________________________________________________________
    ComponentType               |       FilePermission      |   false
    Folder                      |       string              |   false 
    UserName                    |       string              |   false

Each parameter given above would need its own ValueItem.  Here is an example FileDeployment section:

```xml
<Collection name="SetPermissions">
    <ValueItems>
      <ValueItem key="ComponentType" value="FilePermission" />
      <ValueItem key="Folder" value="C:\inetpub\Website" />
      <ValueItem key="UserName" value="domain\username" />                        
    </ValueItems>
</Collection>  
```


If forceInstall is used it will delete all the conents of the destination folder when copying, otherwise all the files from the source location will be appended to the destination folder, (where the files are the same the source file will overwrite the destination file).

N.B. with remote deployments, if you give incorrect authentication details msdeploy will hang but the system will continue and not report the error.  If there is no output from this process you can tell there is an error (issue has been logged in GITHUB).

5.7) Installing an AppPool (Component Type: AppPoolCreation)
-------------------------------------------------------------

To Install an AppPool either locally or remotely requires a Collection configuration section, such as:

```xml
<Collection name="InstallAppPool">
    <ValueItems>
      <ValueItem key="ComponentType" value="AppPoolCreation"/>
      <ValueItem key="SourceContentPath" value="C:\temp\deploy\installer"/>
      <ValueItem key="PathToConfigFile" value="pool.config"/>              
    </ValueItems>
</Collection>
```

The above example is a local installation, it gives the component type, source folder location and the relative path (from source) to the config file that has the AppPool information.  For a remote installation it would require the destination content path and authentication details.  Below is a list of the parameters available for AppPoolCreation:

    param                       |       value               |   optional
    ________________________________________________________________________
    ComponentType               |       AppPoolCreation     |   false
    DestinationComputerName     |       string              |   false
    DestinationUserName         |       string              |   true
    DestinationPassword         |       string              |   true
    ForceInstall                |       bool                |   true
    CleanUp                     |       bool                |   true
    SourceContentPath           |       string              |   false
    DestinationContentPath      |       string              |   false
    PathToConfigFile            |       string              |   false

5.8) Removing an AppPool (Component Type: AppPoolRemoval)
---------------------------------------------------------

To Remove an AppPool either locally or remotely requires a Collection configuration section, such as:

```xml
<Collection name="RemoveAppPool">
    <ValueItems>
      <ValueItem key="ComponentType" value="AppPoolRemoval"/>
      <ValueItem key="SourceContentPath" value="C:\temp\deploy\installer"/>
      <ValueItem key="PathToConfigFile" value="pool.config"/>              
    </ValueItems>
</Collection>
```

The above example is a local removal, it gives the component type, source folder location and the relative path (from source) to the config file that has the AppPool information, it will search the config file for all AppPool names matching APPPOOL.NAME="".  All matches will be removed.  For a remote removal it would require the destination content path and authentication details.  Below is a list of the parameters available for AppPoolRemoval:

    param                       |       value               |   optional
    ________________________________________________________________________
    ComponentType               |       AppPoolRemoval      |   false
    DestinationComputerName     |       string              |   false
    DestinationUserName         |       string              |   true
    DestinationPassword         |       string              |   true
    ForceInstall                |       bool                |   true
    CleanUp                     |       bool                |   true
    SourceContentPath           |       string              |   false
    DestinationContentPath      |       string              |   false
    PathToConfigFile            |       string              |   false


5.9) Installing an Application into an existing website (Component Type: AppCreation)
-------------------------------------------------------------------------------------

To Install an applicaiton (what used to be referred to as a virtual directory) either locally or remotely requires a Collection configuration section, such as:

```xml
<Collection name="installApplication">
    <ValueItems>
      <ValueItem key="ComponentType" value="AppCreation"/>
      <ValueItem key="SourceContentPath" value="C:\temp\deploy\installer"/>
      <ValueItem key="PathToConfigFile" value="application.config"/>
    </ValueItems>
</Collection>
```

The above example is a local installation, it gives the component type, source folder location and the relative path (from source) to the config file that has the Application information.  For a remote installation it would require the destination content path and authentication details.  Below is a list of the parameters available for AppCreation:

    param                       |       value               |   optional
    ________________________________________________________________________
    ComponentType               |       AppPoolCreation     |   false
    DestinationComputerName     |       string              |   false
    DestinationUserName         |       string              |   true
    DestinationPassword         |       string              |   true
    ForceInstall                |       bool                |   true
    CleanUp                     |       bool                |   true
    SourceContentPath           |       string              |   false
    DestinationContentPath      |       string              |   false
    PathToConfigFile            |       string              |   false


5.10) Removing an Applicaiton (Component Type: AppRemoval)
---------------------------------------------------------

To Remove an Application (what used to be referred to as a virtual directory) either locally or remotely requires a Collection configuration section, such as:

```xml
<Collection name="RemoveApplication">
    <ValueItems>
      <ValueItem key="ComponentType" value="AppRemoval"/>
      <ValueItem key="SourceContentPath" value="C:\temp\deploy\installer"/>
      <ValueItem key="PathToConfigFile" value="application.config"/>              
    </ValueItems>
</Collection>
```

The above example is a local removal, it gives the component type, source folder location and the relative path (from source) to the config file that has the Application information, it will search the config file for all Application names matching APP.NAME="".  All matches will be removed.  For a remote removal it would require the destination content path and authentication details.  Below is a list of the parameters available for AppRemoval:

    param                       |       value               |   optional
    ________________________________________________________________________
    ComponentType               |       AppPoolCreation     |   false
    DestinationComputerName     |       string              |   false
    DestinationUserName         |       string              |   true
    DestinationPassword         |       string              |   true
    ForceInstall                |       bool                |   true
    CleanUp                     |       bool                |   true
    SourceContentPath           |       string              |   false
    DestinationContentPath      |       string              |   false
    PathToConfigFile            |       string              |   false


5.11) Installing a Web Site (Component Type: WebSiteCreation)
-------------------------------------------------------------------------------------

To Install a Web Site either locally or remotely requires a Collection configuration section, such as:

```xml
<Collection name="installWebSite">
    <ValueItems>
      <ValueItem key="ComponentType" value="WebSiteCreation"/>
      <ValueItem key="SourceContentPath" value="C:\temp\deploy\installer"/>
      <ValueItem key="PathToConfigFile" value="site.config"/>
    </ValueItems>
</Collection>
```

The above example is a local installation, it gives the component type, source folder location and the relative path (from source) to the config file that has the Web Site information.  For a remote installation it would require the destination content path and authentication details.  Below is a list of the parameters available for WebSiteCreation:

    param                       |       value               |   optional
    ________________________________________________________________________
    ComponentType               |       WebSiteCreation     |   false
    DestinationComputerName     |       string              |   false
    DestinationUserName         |       string              |   true
    DestinationPassword         |       string              |   true
    ForceInstall                |       bool                |   true
    CleanUp                     |       bool                |   true
    SourceContentPath           |       string              |   false
    DestinationContentPath      |       string              |   false
    PathToConfigFile            |       string              |   false


5.12) Removing a WebSite (Component Type: WebSiteRemoval)
---------------------------------------------------------

To Remove a Web Site either locally or remotely requires a Collection configuration section, such as:

```xml
<Collection name="RemoveWebSite">
    <ValueItems>
      <ValueItem key="ComponentType" value="WebSiteRemoval"/>
      <ValueItem key="SourceContentPath" value="C:\temp\deploy\installer"/>
      <ValueItem key="PathToConfigFile" value="site.config"/>              
    </ValueItems>
</Collection>
```

The above example is a local removal, it gives the component type, source folder location and the relative path (from source) to the config file that has the Web Site information, it will search the config file for all site names matching SITE.NAME="".  All matches will be removed.  For a remote removal it would require the destination content path and authentication details.  Below is a list of the parameters available for WebSiteRemoval:

    param                       |       value               |   optional
    ________________________________________________________________________
    ComponentType               |       WebSiteRemoval      |   false
    DestinationComputerName     |       string              |   false
    DestinationUserName         |       string              |   true
    DestinationPassword         |       string              |   true
    ForceInstall                |       bool                |   true
    CleanUp                     |       bool                |   true
    SourceContentPath           |       string              |   false
    DestinationContentPath      |       string              |   false
    PathToConfigFile            |       string              |   false


5.13) Packaging an Application or folder (Component Type: CreatePackage)
------------------------------------------------------------------------

This component gives you the ability to package a project (or a folder), primary use would probably be with web apps where you want to create the deployment package.  Apart from creating web deployment packages this will also allow you to zip a folder in the same way, (it will not remove any files or folders, unlike the web deployment package).

5.13.1) Packaging a web project:
--------------------------------

When packaging and project, (such as web app or a web deployment project), either locally or remotely the following Collection configuration section is required, (example is for local, more information about parameters below):

```xml
<Collection name="PackageSite">
    <ValueItems>
        <ValueItem key="ComponentType" value="CreatePackage"/>
        <ValueItem key="SourceContentPath" value="C:\temp\deploy\mvctestsite\mvctestsite\mvctestsite.csproj" />        
    </ValueItems>
</Collection>
```

The above example will take the project file given in the "SourceContentPath" and create a package for it.  The internals of this uses MSBuild to create the package.  By default it will build the project with the Debug configuration and with all the default paths.  The example below shows some level of customization:


```xml
<Collection name="PackageSite">
    <ValueItems>
        <ValueItem key="ComponentType" value="CreatePackage"/>
        <ValueItem key="SourceContentPath" value="C:\temp\deploy\mvctestsite\mvctestsite\mvctestsite.csproj" />
        <ValueItem key="InternalPath" value="C:\websites\myMVCSite" />
        <ValueItem key="ConfigurationType" value="Release" />
        <ValueItem key="OutputLocation" value="C:\installer\testPackage.zip" />
        <ValueItem key="ZipFileOnly" value="true" />
    </ValueItems>
</Collection>
```

This example changes the path that is created inside the zip package file.  I.E, it is no longer "C_C\temp\deploy\mvctestsite\mvctestsite" (with the extra folders it normally adds), it will now be "C:\websites\myMVCSite".  This is done by setting the "InternalPath" element.  This is useful for deploying to servers as normally the location of your project file is not the same path as your IIS folders.  The configuration is changed to "Release" using the "ConfigurationType" element.  The output location of the packaged zip is changed (normally it is in the "obj" folder under the project file folder then the configuration type, i.e. "Debug" or "Release" and "package" folder).  Lastly it tells the application if it should delete all the other files it generates (I.E. the xml and cmd files), and only leave the zip file, (this is defaulted to false).

Below is a list of the parameters available for CreatePackage:

    param                                           |       value               |   optional
    _______________________________________________________________________________________________________
    ComponentType                                   |       CreatePackage       |   false
    DestinationComputerName                         |       string              |   false
    DestinationUserName                             |       string              |   true
    DestinationPassword                             |       string              |   true
    ForceInstall                                    |       bool                |   true
    CleanUp                                         |       bool                |   true
    SourceContentPath                               |       string              |   false
    DestinationContentPath                          |       string              |   false
    InternalPath                                    |       string              |   true
    OutputLocation                                  |       string              |   true
    ConfigurationType                               |       string              |   true
    ZipFileOnly                                     |       string              |   true (default is false)
    AutoParameterizationWebConfigConnectionStrings  |       bool                |   true (default is false)
    ShouldPushPackageToRemoteMachine                |       bool                |   true (default is false)
    MsBuildExe                                      |       string              |   true (defaults to 64bit V4.0 version)

N.B.  AutoParameterizationWebConfigConnectionStrings is defaulted to false and is optional, it tells MSBuild to not change connection strings in the web config, if this is set to true it will put the connection strings into the {PACKAGENAME}.SetParameters.xml file outside of the zip file, (where {PACKAGENAME} is the name of the zip file).  If this is set to true then it is not recommended to use the package deployment as it doesn't use msdeploy directly, (msdeploy has issues when IIS is not directly involved).

5.13.2) Packaging a folder:
---------------------------

It is also possible to package a folder so that it follows the same idea as a web project, (is a zip file that has the file path embedded in it). There is currently no way to override the InternalPath with this process.  An example configuration collection is below:

```xml
  <Collection name="PackageFolder">
    <ValueItems>
      <ValueItem key="ComponentType" value="CreatePackage"/>
      <ValueItem key="SourceContentPath" value="C:\temp\deploy\mvctestsite\mvctestsite" />
      <ValueItem key="OutputLocation" value="C:\websites\testPackage.zip" />
    </ValueItems>
  </Collection>
```

The above is an example of a local folder packaging task.  It will take the "C:\temp\deploy\mvctestsite\mvctestsite" folder and create a zip file from that.  The zip file will the archive.xml and systeminfo.xml files and a Content folder, (same process as web projects above).  Under the Content folder it will have the drive and full path, "C_C\temp\deploy\mvctestsite\mvctestsite" in this case.

To deploy remotely simply requires the "DestinationContentPath" element, (as well as the connection credentials as always).  Below is a list of parameters available for CreatePackage for folders:

    param                       |       value               |   optional
    ________________________________________________________________________
    ComponentType               |       CreatePackage       |   false
    DestinationComputerName     |       string              |   false
    DestinationUserName         |       string              |   true
    DestinationPassword         |       string              |   true
    ForceInstall                |       bool                |   true
    CleanUp                     |       bool                |   true
    SourceContentPath           |       string              |   false
    DestinationContentPath      |       string              |   false
    OutputLocation              |       string              |   true
    MsDeploy                    |       string              |   true (has internal default)



5.14)  Deploying a Package
--------------------------

When deploying a package (zip file) either locally or remotely the following collection configuration section is required, (example is for local deployment):

```xml
<Collection name="DeployPackage">
    <ValueItems>
      <ValueItem key="ComponentType" value="DeployPackage"/>
      <ValueItem key="SourceContentPath" value="C:\temp\deploy\installer\test\testPackage.zip" />
    </ValueItems>
</Collection>
```

The above example shows using the "DeploymentPackage" ComponentType.  For local install the minimum information required is the "SourceContentPath", which is the path the zip file.  For remote deployments the deployment details such as server name and credientials would be required.  A list of the other parameters is listed below:

    param                       |       value               |   optional
    ________________________________________________________________________
    ComponentType               |       DeployPackage       |   false
    DestinationComputerName     |       string              |   false
    DestinationUserName         |       string              |   true
    DestinationPassword         |       string              |   true
    ForceInstall                |       bool                |   true
    SourceContentPath           |       string              |   false
    DestinationContentPath      |       string              |   true
    AllowUseOfDestinationFolder |       bool                |   true (default value is false)
    
If "ForceInstall" install is used it will ensure the deployment folder is removed before unpackaging the zip file, otherwise it will merge the two folders and files together, where conflicts occur the package files will take precedence.  The DestinationContentPath is not required, it will use the path given in the archive.xml file, (iisapp path element), this can be specified (customized) when creating the zip, (either with this application or natively with MsBuild).

The "AllowUseOfDestinationFolder" property is defaulted to false.  This defines weather the "DestinationContentPath" property can be used within MsDeploy to override the path that the package will be installed at, i.e. the path in archive.xml.  If false it will use the archive.xml value, if true it will check to see if there is a value in DestinationContentPath, if so use that otherwise fall back on the archive.xml value.

6) Gotcha's and helpful hints
-----------------------------

6.1) Any appcmd (working with IIS) tasks will require admin permission.  It is best to always run the installer as admin to avoid these permission issues.

6.2) Some appcmd tasks may not directly fail, (at least they do not report an unusual exit code), if the results are unexpected check the output of the installer, (pumping the installer to a log file if being used automatically is recommended).

6.3) When installing a website they are given an ID such as the excert below:

```xml
    <SITE SITE.NAME="mywebsite" SITE.ID="2" bindings="http/*:99:" state="Started">
        <site name="mywebsite" id="2">
```

The above example has an ID of "2".  This can be an issue when installing on a different machine.  IIS will not allow an install that already has that ID.  Unfortunately the error it reports doesn't lead directly to that result:

```xml
message:Failed to add duplicate collection element "mywebsite"
```

The only solution I have at this point is when you create your configuration file give it a large number to try avoid conflicts, (see road map).

6.4) Error while using msdeploy, 'The application pool that you are trying to use has the 'managedRuntimeVersion' property set to 'v2.0'. This application requires 'v4.0'.':

This normally is due to IIS not having .Net V4.0 registered.  using aspnet_regiis.exe /i normally fixes this (http://msdn.microsoft.com/en-us/library/ee942158.aspx).

6.5) If when executing the application it doesn't find a file that has a relative file path given, you might need to set the SETEXEPATH flag to true at the command line.

6.6) If when trying to copy files to a remote server with msdeploy and no errors are returned, and no real output, check authentication credentials.

6.7) If you get the error:

Error: Unable to cast object of type 'Microsoft.Web.Deployment.DeploymentProviderOptions' to type 'Microsoft.Web.Deployment.DeploymentProviderOptions'.

This is often due to the fact that you have both v1 and v2 installed in which case change

"C:\Program Files\IIS\Microsoft Web Deploy\msdeploy.exe" in the command line to

"C:\Program Files\IIS\Microsoft Web Deploy v2\msdeploy.exe"

6.8) If you get the error:

Error: The application pool that you are trying to use has the 'managedRuntimeVersion' property set to 'v2.0'. This application requires 'v4.0'.

This can be, (not always) related to IIS and .Net V4 not being registered.  Registering it with "aspnet_regiis" should help, (http://msdn.microsoft.com/en-us/library/k6h9cz8h(v=vs.100).aspx)

cd %windir%\Microsoft.NET\Framework\v4.0.30319
aspnet_regiis -i



7) Examples:
------------

There are a number of examples showing how an application can be setup.

7.1) Examples\MVC4-site - Shows taking a web project (MVC4 in this case) file (.csproj), building a deployment package and installing that on a remote machine.  Whilst also using two configuration files to setup the AppPool and IIS Website on the remote server.



8) Road Map
-----------

There are a number of tasks I would like the application to do that it currently can't:

1) Have a slightly more intelligent install
1.1) When installing an element, (for example an app pool), check if the same name is already there.  If so, check the values, if they are identicial then no install required.
1.2) When force is given only do the force if there is something to remove
1.3) When installing web sites change the ID to be unique to avoid conflicts

2) Have a client GUI (wpf) to allow the creation of the scripts and configuration files to be able to run the process and not need to open a text editor.  As well as allowing the whole process to be run from the UI if wanted, (not sure this last requirement is very needed as Visual Studio already does this pretty well).