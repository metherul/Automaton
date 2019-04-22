## Usage documentation for Hephaestus
Welcome to a quick-and-dirty overview of how to use Hephaestus, the tool for creating Automaton pack files. 

At a most basic level this tool will scan a Mod Organizer 2 install, and create a mod pack that as closely
as possible matches the configuration of a MO2 profile. This is done without copying any files directly, instead
the modpack will include instructions on how to build a copy of the MO2 profile from scratch.

### One-time config settings
First of all, you must configure a JSON file that tells MO2 how to access the Nexus. Head to https://www.nexusmods.com/users/myaccount?tab=api%20access
and generate a personal API key (the section at the bottom of the page in that link). This key will allow Hephaestus
to make Nexus requests on your behaf. Now head to your Mod Organizer 2 folder and create a file called `automaton.perfs`. 
Open this `.perf` file and add the following JSON information, including the personal API key you got from the link above.

```json
{
  "api_key": "<personal api key goes here>"
}

```

Save and close this file. This is a one-time configuration and these settings are assumed to be the same for all mods you create;


### Define a mod pack
Now we need to define the input parameters for the Hephaestus process. Create another text file in your MO2 folder and
name it whatever you want followed by a `.auto_definition`. For example: `my_awesome_pack.auto_definition`. Open this file
in a text editor (Notepad++ is highly recommended), and copy-paste-and modify the following JSON information:


```JSON
{
  "pack_name" : "Automaton Test 2",
  "author" : "halgari",
  "mo2_path" : "C:\\Mod Organizer 2",
  "mo2_profile" : "Automaton Test 2",
  "alternate_archive_locations" : ["C:\\Skyrim\\mods"],
  "scan_game_directory": true
}
```

* pack_name - The full name of the pack
* author - Your handle, user-name, or superhero alter-ego alias
* mo2_path - The full path to your MO2 folder, *notice the double backslashes, these are required because this is JSON*
* mo2_profile - The name of the MO2 profile you want to use to generate the pack
* alternative_archive_locations - An optional list of folders that can be scanned for downloaded folders. By default only the `<mo2_path>/downloads` folder is scanned. If you don't have additional search locations, remove this line from the config, or use the empty list `[]`.
* scan_game_directory - If true, Hephaestus will attempt to find files in the downloaded archives that match files in your base game folder. This should 
detect SKSE base files, ENB configurations, or anything else that is copied from a downloaded archive directly into the game folder.

### Running Hephaestus
Now run Hephaestus. It will prompt you to locate a `.auto_definition` file. Now begins the compilation process, this consists 
of several phases:

#### Archive Indexing
The first step is that Hephaestus will index all the archives of which it is aware. This involves hashing every archive, and
then reading the contents of each and hashing the contents. The output of this process is cached and so you'll only have to 
wait for the archives to be indexed a single time. If you download new archives in the future only the new ones will be hashed.
This process is *heavily* parallel. The more disk bandwidth and the more CPU cores you can throw at it, the faster it will finish.

During this process you may see warnings that instruct you to update the `direct_url` fields for a given archive. This will happen
when Hephaestus can't determine the origin of a given mod. After the indexing process go and find these files in the download folder
and open the corrisponding `.SOURCE_ARCHIVE` file. In there you will see a field called `direct_url`. Fill out this field with the
direct URL to the archive you downloaded. Now re-compile your mod pack.

#### Mod Compilation
Now Hephaestus will scan all the mods in the given MO2 profile and link them to files it's aware of in the archives indexed above.
This process is fairly fast, and is parallel, but since we must hash every file in every installed mod, it may take a little bit of 
time. 

#### Game directory Scanning
If enabled, Hephaestus will now scan the game folder looking for ENB, SKSE or other such files that you've copied from archives
directly into the game folder. This normally doesn't take long, but depending on the game it may take some time.

#### Mod Exporting
Finally Hephaestus will export the mod. This involves writing a `.json` file for each mod installed, copying in the `.ini` files
for each mod, and zipping everything up into the final `.auto` file. Once this process completes you should have a `.auto` file 
in the same folder as Hephaestus. 

**Note:** A complete log of the compilation process is stored in the same folder as Hephaestus (and is overwritten with every)
subsequent compilation process. Be sure to review the log for any warnings or errors you may have missed.
