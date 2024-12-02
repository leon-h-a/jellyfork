## JellyFork

<p align="center">
    <img src="Jellyfin.Plugin.JellyFork/assets/jellyfork.jpg" alt="jellyfork_logo" width="400"/>
</p>

<br>
<p align="center">
    Quickly rename and move your movies and subtitles from downloads to media.
    
</p>
<p align="center">
    All available from inside Jellyfin application.
</p>

### Installation

##### 1. Clone this repository
````
git clone git@github.com:leon-h-a/jellyfork.git
````

##### 2. Create new plugin directory and set ownership to ``jellyfin`` user
````
sudo mkdir /var/lib/jellyfin/plugins/JellyFork
sudo chown jellyfin /var/lib/jellyfin/plugins/JellyFork
````

##### 3. Move .dll to newly created directory
````
sudo mv jellyfork/Jellyfin.Plugin.JellyFork/bin/Release/net8.0/JellyFork.dll \
/var/lib/jellyfin/plugins/JellyFork
````

##### 4. (optional) Restart Jellyfin server
````
sudo systemctl restart jellyfin
````

### Usage

1. Define your (input) download directory path and (output) movie directory path
2. Click "Scan for media" to get a list of files inside downloads directory
3. Expand directories and select files that you want to rename and move to movies directory
4. Click on "Rename and move" to complete the action
5. (optionaly) Delete directory from which you moved your files

<p align="center">
    <img src="Jellyfin.Plugin.JellyFork/assets/user_guide.gif" alt="user_guide"/>
</p>

### Contributing

All contributions are welcome!
