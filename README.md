# Discord-BooliBot
Quick Discord bot service to handle a Windows game server
## What it is
BooliBot is a Discord Bot built on [Discord.NET](https://github.com/discord-net/Discord.Net) that will run as a Windows service. The bot was quickly created to manage a few game servers running on a Windows server in order to not have to remote in to restart and update them. The name comes from the Discord name, which if you know you know.

This bot basically allows me to be a bit lazy and allows the entire community to manage the game servers.
## Why Windows Server?
Because Palworld had a memory leak in the Linux build on launch and we wanted to play.
## How it works
BooliBot uses the appsettings to know which game servers are installed and where, as well as provides connectivity information for friends that wants to connect.

The bot assumes that you got something that starts the servers again after it has been killed, in this case I have a custom Scheduled Task that reads the event logs to find if the game server has died. Which looks something like this:
```xml
<QueryList>
	<Query Id="0" Path="Security">
		<Select Path="Security"> *[System[Provider[@Name='Microsoft-Windows-Security-Auditing'] and (band(Keywords,9007199254740992)) and (EventID=4689)]] and *[EventData[(Data='D:\Games\Palworld\PalServer.exe')]] </Select>
	</Query>
</QueryList>
```