# Racetrack Generator
![Image of the plugin](https://raw.githubusercontent.com/ivankarez/RacetrackGenerator/main/.documentation-images/racetrack-generator.png)

A unity editor tool that generates meshes of a racetrack based on csv files found in the [racetrack-database](https://github.com/TUMFTM/racetrack-database) repository.

## How to use
To use this plugin in your Unity projects, you can just simply download the content of this repository and copy the `Assets\Racetrack Generator` folder into your project's `Assets` folder.

After you copied the content, you can open the _Racetrack Generator_ window by clicking the `Window/Racetrack Generator` menu option in the Unity editor.

## Example output
On the first run the plugin will generate 2 new assets in you asset folder. The first one is the mesh of the racetrack, the second one is an instance of the [TrackData](https://github.com/ivankarez/RacetrackGenerator/blob/main/Assets/Racetrack%20Generator/Scripts/TrackData.cs) scriptable object. You can use the second file to acces the coordinates of the left, right, center and ideal racing lines of your racetrack in other scripts.

On the following image you see part of the output of the plugin. The mesh is dark gray, left line is blue, right line is red, center line is white and the racing line is green. The spheres marks the suggested speeds for the center line and the racing line. Red means slower, green means faster.

You can visualize your track the same way with the [RaceTrackDataVisualizer](https://github.com/ivankarez/RacetrackGenerator/blob/main/Assets/Scripts/RaceTrackDataVisualizer.cs) script. It's not part of the plugin, but you can copy this one also in your project if you need it.

![Example of how the output looks like](https://raw.githubusercontent.com/ivankarez/RacetrackGenerator/main/.documentation-images/example-output.png)

