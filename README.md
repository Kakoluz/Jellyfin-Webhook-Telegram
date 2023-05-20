# Jellyfin Webhook listener for Telegram
This is a simple webhook listener for Jellyfin webhook plugin that will send messages to Telegram uisng the bots API.

Its built on the ASP.NET 7.0 Core platform.

---------

## Installation
1. Download the latest release from the [releases](https://github.com/Kakoluz/Jellyfin-Webhook-Telegram/releases) tab.
1. Setup your bot using [BotFather](https://telegram.me/BotFather) and get the **API Key** or **Token**.
1. Place your API key and your (or your administrator's) **ChatID** in `appsettings.json`
1. Run the application.
    1. If you want to modify the port or listening addresses you can add **--urls** "\<urls\>" (comma separated and protocol included)
1. Using your browser navigate to `http://<your-ip>:<port>/notifications/healthcheck` to see if its running.

	
## Usage
1. Once setup, go to your bot's chat and type `/start` it will notify you that your chat/group is in the update list.
1. Set your **Jellyfin Webhook plugin** to send the notifications to the following endpoints
    1. For notifying every user on the list `http://<your-ip>:<port>/notifications/general`
	1. For notifying only the administrator `http://<your-ip>:<port>/notifications/admin`
1. Messages should be formated as a **JSON** with the following structure:
    1. ```JSON
       {
           "photo": "<URL to picture>",
    	   "message": "<Text to display>"
       }
       ```
       For pictures it will generally be `"photo":"{{ServerUrl}}/Items/{{ItemId}}/Images/Primary"`.
	
	   If there is no image or fails to fecth, the message wil be sent as text only message. The text can be given style using **HTML** syntax.
1. Enjoy!
    1. If you want to disable notifications, just type `/leave`.

## Building
1. Clone the repository
1. Install .NET Core 7 or newer.
1. Run `dotnet publish -c Release` on the root folder.
1. Follow Usage guides!

## License
This project is licensed under the GNU GPL License - see the [LICENSE](https://github.com/Kakoluz/Jellyfin-Webhook-Telegram/blob/main/LICENSE) file for details.