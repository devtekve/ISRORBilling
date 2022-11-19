# NationPingService (by F3rre)
- [NationPingService (by F3rre)](#nationpingservice--by-f3rre-)
    * [Settings](#settings)
    * [Here is how to make TCP work.](#here-is-how-to-make-tcp-work)
    * [Credits](#credits)

<small><i><a href='http://ecotrust-canada.github.io/markdown-toc/'>Table of contents generated with markdown-toc</a></i></small>

This is a service used to show the ping of the given server from the ingame login screen.
![App Screenshot](https://i.imgur.com/iOMPFBL.png)


## Settings

You can customize the binding IP and binding port for the PingService on `appsettings.json`
````json
{
...
"NationPingService": {
    "ListenAddress": "0.0.0.0",
    "ListenPort": 12989
  },
...
}
````

## Here is how to make TCP work.

1-Open General Server.cfg from the serverfiles.

2-Go to Gateway Section to "**TcpPingServerCount**".

3-Change "TcpPingServerCount" from 3 to 4.

4-Add

```	// TcpPing01_Nation : Europe
	TcpPing03_Nation		4			// Second (Zero Base) Bing Server Country Code
	TcpPing03_URL			"YourServerIP"	// Second Ping Server URL
	TcpPing03_Port			12989			// Second Ping Server Port
```

![App Screenshot](https://i.imgur.com/T5laM3T.png)

## Credits
* F3rre for initial implementation and reverse engineering
* Devtekve & Daxtersoul for code improvements
