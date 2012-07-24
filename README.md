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

msiexec /I <msi_filename> /passive ADDLOCAL=ALL LISTENURL=http://+:8172/MsDeployAgentService/

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
3.  Removing files from a destination location.
4.  Setting up App Pools on a destination machine.
5.  Setting up web sites on a destination machine.
6.  Setting up applications on a destination machine.
7.  Removing App Pools from a destination machine.
8.  Removing web sites from a destination machine.
9.  Removing applications from a destination machine.

5.1) How is Automated Deployments used?
---------------------------------------

To run the app you will need to specify the configuration section you want to use.  There are a number of other parameters you can give that will override any settings in the configuration:

To run the app you would call "MYAPPCONSOLE.EXE /CONFIGSECTION=MYCONFIGSECTION"

This would look for a config section with the name of "MYCONFIGSECTION".  A list of the parameters available are given below:

    param				|		value
    _________________________________________________________
    /CONFIGSECTION		|		MYCONFIGSECTION
    /FORCE				|		true / false
    /CONFIGPATH			|		C:\MYPATHTOCONFIGFILE.CONFIG
    /BREAKONERROR		|		true / false
    /CLEANUP            |       true / false

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
			  <ValueItem key="MsdeployExe" value="C:\Program Files\IIS\Microsoft Web Deploy V2\msdeploy.exe"/>
			  <ValueItem key="AppCmdExe" value="C:\Windows\System32\inetsrv\appcmd.exe"/>
			  <ValueItem key="DestinationComputerName" value="localhost"/>          
			</ValueItems>
			<Collections>        
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
    MsdeployExe					|		file path			|	false		|	"C:\Program Files\IIS\Microsoft Web Deploy V2\msdeploy.exe"
    AppCmdExe					|		file path			|	false		|	"C:\Windows\System32\inetsrv\appcmd.exe"
    ShouldSortTasks             |       bool                |   true        |   false

If CleanUp is set to true it will delete all files the application generates, (this doen't include files that are used but not generated by the application, such as IIS config xml files that are used by appcmd), once the files have been used.

The secions below will deal with setting up each of the ComponentType's.

List of all ComponentType's:
----------------------------

1.  FileDeployment        
2.  AppPoolCreation
3.  AppPoolRemoval
4.  WebSiteCreation
5.  WebsiteRemoval
6.  AppCreation
7.  AppRemoval
8.  ApplicationExecution

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

If forceInstall is used it will delete all the conents of the destination folder when copying, otherwise all the files from the source location will be appended to the destination folder, (where the files are the same the source file will overwrite the destination file).

N.B. with remote deployments, if you give incorrect authentication details msdeploy will hang but the system will continue and not report the error.  If there is no output from this process you can tell there is an error (issue has been logged in GITHUB).

5.6) Installing an AppPool (Component Type: AppPoolCreation)
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

5.7) Removing an AppPool (Component Type: AppPoolRemoval)
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
    ComponentType               |       AppPoolCreation     |   false
    DestinationComputerName     |       string              |   false
    DestinationUserName         |       string              |   true
    DestinationPassword         |       string              |   true
    ForceInstall                |       bool                |   true
    CleanUp                     |       bool                |   true
    SourceContentPath           |       string              |   false
    DestinationContentPath      |       string              |   false
    PathToConfigFile            |       string              |   false


5.8) Installing an Application into an existing website (Component Type: AppCreation)
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

The above example is a local installation, it gives the component type, source folder location and the relative path (from source) to the config file that has the Application information.  For a remote installation it would require the destination content path and authentication details.  Below is a list of the parameters available for AppPoolCreation:

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


