		
Readme file
===========

Build the App
=============
	1. Start Microsoft Visual Studio 2013 Update 2 and select File > Open > Project/Solution. 
	2. Go to the directory to which you unzipped the app. Then go to the subdirectory 
	   named for the app and double-click the Visual Studio 2013 Update 2 Solution (.sln) file.
	   
	- To build the Windows Phone app:
		1. Select FileAccess.WindowsPhone in Solution Explorer. 
		2. Press Ctrl+Shift+B or use Build > Build Solution or use Build > Build FileAccess.WindowsPhone. 
	
Run the app
===========
	- To deploy and run the Windows Phone app:
		1. Right-click FileAccess.WindowsPhone in Solution Explorer and select Set as StartUp Project.
		2. To debug the app and then run it, press F5 or use Debug > Start Debugging. 
		   To run the app without debugging, press Ctrl+F5 or use Debug > Start Without Debugging. 
		3. To deploy on Emulator, click on BUILD in Visual Studio 2012 or 2013 and click on Deploy Solution
		
To make a app launch Tile on Emulator:
	1. Click on run Emulator icon.
	2. On Emulator,  click on right arrow button in the buttom of the start screen to go to app listing page.
	3. On app listing page, click and hold on to app's name (BkWinApp).
	4. Select pin to start menu
	5. The live tile icon should be listed on the start screen now.
	
Change the device settings to accept speech:

	1. Go to device (Emulator) settings.
	2. Select Speech from the list.
	3. Check Enable Speech Recognition Service box.

Launch the app:
	1. Click on the BkWinApp Tile icon from the start screent to launch the app.
	2. Log in using Microsoft's account id.
	3. Click on the microphone icon from the app' bar.
	4. Speak some reminder to record.
	5. Click OK to Save the speech-to-text data on Azure.
	6. Repeat 1-5 to record more reminders.
	7. Check the box from the reminder list to mark the things that are already done.
	
		
	