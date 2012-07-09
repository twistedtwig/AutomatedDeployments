Automated Deployments
=====================

The goal of this project is to simplify the process and control of IIS  (7.x) settings and setup / deployments.  Visual studio allows for nice pushing of files and IIS configuration from a development machine, (at least a machine with IIS setup as you want it).  If however you want to do CI / CD (continuous deployment) and do not want to / can't setup your CI server to have all the IIS settings you require for your deployment you are a bit stuck.

One solution to the problem above is to say, "well I am just setting up the web server once, I will do it manually then us MsDeploy or FTP to push my changes up".  This is valid and will work in many situations.  There is (at least) one draw back to this approach, your developers!  They will be developing the system using their own IIS (most likely), they will have to setup their IIS, (or let VS do it for them).  Does this mean that all the developers will set it up the same as each other, and that the setup will be the same for deployments?

Automated Deployments was designed to address this issue.  It has a number of benefits:

1.  Allows IIS Settings to be put into Source Control.
2.  Allows developers to use the same setup and deployment process when ever they build their solution with the same settings as will be used in the live system.
3.  Automates the deployment process.

How does it work?
-----------------

The application uses configuration files gained from IIS via "appcmd.exe".  These configuration files can then be put into your source control process.  The application would then be configured to use these configuration files to create IIS app pools, web sites, and applications on an IIS Server, (be that locally or remotely).  It can also be configured to copy all the files to a drop location.

What does it need?
------------------

The application has a few requirements:

1.  .Net V4.0 needs to be installed on the client  / source machine.
2.  MsDeploy needs to be installed on the source machine (if the deployment / destination machine is not the same as the source).
3.  MsDeploy needs to be installed on the destination machine (if the source  machine is not the same as the destination).
4.  Appcmd needs to be installed on the source machine (if this machine is to be used to get IIS configuraiton settings to be put into source control).
5.  Appcmd needs to be installed on the destination machine (if any IIS settings are to be updated).

How to install MsDeploy?
------------------------

You will need MsDeploy installed and setup correctly on your production / live / destination server(s).  Here is a reasonable guide from MS: http://technet.microsoft.com/en-us/library/dd569059(v=ws.10).aspx .  The main thing to be aware of is that it needs a port opened to communicate.  By default it wants to run on port 80.  This is not great if you already have websites on there as it can / could interfere.  I have found it best to install it on port 8172 or 8173 (sometimes there is already a process running on 8172).  To install it so it is not running on port 80 you need to use the command line:

msiexec /I <msi_filename> /passive ADDLOCAL=ALL LISTENURL=http://+:8172/MsDeployAgentService/

This will need to be run on cmd that is running as admin.  If when it installs it rolls back it is pretty possible something is already running on that port, try 8173 instead.  Remember to open the firewall as well to allow traffic in.

How to add your IIS configuration to Source Control?
----------------------------------------------------

There are two ways of getting your IIS settings:

1.  Use the GUI tool to backup your settings (currently still under development and not functioning)
2.  use Appcmd to backup the settings.

Using the GUI:
--------------

Not ready yet................

Using Appcmd:
-------------

Appcmd is a powerful tool designed to help administer IIS (7.x).  Here is a guide to getting started with Appcmd, http://learn.iis.net/page.aspx/114/getting-started-with-appcmdexe/ .  There are a few command we will need to use.  I am going to imagine you have a website on your machine called "stackoverflow", an appPool called "SOAppPool" and an application running within "Default Web Site" called "reddit".

To backup the AppPool you would run the following command:

C:\Windows\System32\inetsrv\appcmd.exe list apppool SOAppPool /config /xml > C:\SOAppPool.config

To backup the website (which will include all the applications within it), run the following command:

C:\Windows\System32\inetsrv\appcmd.exe list site stackoverflow /config /xml > C:\stackoverflowSite.config

To backup the application running under "Default Web Site", run the following command:

C:\Windows\System32\inetsrv\appcmd.exe list app "Default Web site/reddit" /config /xml > C:\redditApp.config

These files can now be added to your source control ready for future deployments.


Automated Deployment Fuctions:
------------------------------

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


Backuping up IIS Settings to file for Source Control
----------------------------------------------------

This has been covered above

Copying files to a destination location
---------------------------------------











<code>some test code block</code>