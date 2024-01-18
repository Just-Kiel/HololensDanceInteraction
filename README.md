# Hololens Move Interaction
This project was realised during a exchange semester in FUN in Hakodate, Japan. It was made by Aurore Lafaurie in the laboratory of Kaoru Sumi during 4 months from September 2023 to January 2024.
The aim of this project is to show an experience to people through the Microsoft Hololens 2 headset. It is based on movement recognition and allows to interact with environment and virtual agent.

Report is included in this repository [here](https://gitlab.com/Just-Kiel/hololensdanceinteraction/-/blob/main/final_report.pdf?ref_type=heads).

N.B. : The second part of this project is available [here](https://gitlab.com/Just-Kiel/hololensdanceinteraction) because of Git LFS issues.

## How to install and run
Using Unity 2022.3.11f and if possible Hololens 2 and Azure Kinect DK (optional).
No need to build an application, you can directly play in the editor in the Scene "AdvancedScene" located in Assets/My Scenes.

If using Hololens 2, Holographic Remoting is needed and for that need to be on the same WIFI and share the address of the device.

Some packages could not be pushed on the repository but all the names are in the packages.json file.

## How to use
When the project is played several interactions are possible :
- Talk to the virtual agent,
- Do one of the movements hard coded to interact with the virtual agent,
- Switch to custom movements and record positions with right click of mouse and delete them with click on wheel. Those custom movements will trigger effects on the environment.
- Switch of detection device.

### Simple moves
- One hand upper than head
- Both hands and elbows upper than head
- Having both hands pointing on a side
- Do angry pose

## Credits
This project was made thanks to the help of https://github.com/janwww/motion-instructor.
