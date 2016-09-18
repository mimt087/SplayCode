# SplayCode
Part IV Project 2016 - SplayCode: Spatial Layout on Large Display
Zuohao (Jonny) Lu and Seongwoo (Andrew) Jeong

About
------
SplayCode is an extension to Visual Studio which is developed in order to aid programmers' understanding of a project. It allows its users to view, manipulate and edit multiple program files by easily adding them to the spatial layout. Users can save this spatial layout as an XML file for later use and load it again whenever necessary.

Installation
------------
Install the Visual Studio 2015 SDK in addition to Visual Studio 2015 in order to run this project.
You can install the Visual Studio 2015 SDK as part of regular setup, or you can install it later on.
For more information about installing the Visual Studio SDK, see [Visual Studio SDK](https://msdn.microsoft.com/en-us/library/bb166441.aspx).

After installing the Visual Studio SDK, download or clone this repository.

Running SplayCode
-----------------
1. Open Visual Studio 2015
2. Open this project by selecting ```Open Project...``` option on the Start Page, then selecting ```SplayCode.sln``` file under SplayCode project directory
3. Run the loaded project. Doing this will start an Experimental Instance of Visual Studio 2015
4. Open a project, which you will use SplayCode for
5. Go to ```View > Other Windows > SplayCode``` to open SplayCode extension

Using SplayCode
---------------
It is recommended that you use SplayCode on a large touch display with high resolution. SplayCode provides its users with helpful features that assist users' navigation and the construction of their own mental model of the project. Users can use these features by clicking the corresponding buttons in the toolbar. The toolbar is located next to the Visual Studio's default toolbar. If it is not visible, right click on the empty space beside the default toolbar and select ```SplayCode Toolbar```.

###Adding Editors
####Using 'Add file...' dialog
1. From the toolbar, select ```Add file...``` button
2. Navigate to the target directory and select the files you wish to open
3. Click Open

####Using Drag&Drop
#####From the Solution Explorer
To use this feature, it is recommended that you disable the Preview Tab feature of Visual Studio to prevent the preview tabs from opening when trying to drag and drop from the Solution Explorer.
This can be done by going to ```Tools > Options > Environment > Tabs and Windows``` and untick ```Allow new files to be opened in the preview tab``` option.
	
1. Select a file from the Solution Explorer
2. Drag the selected file from the Solution Explorer and drop into the SplayCode window

#####From the Windows Explorer
1. Navigate to the target directory
2. Select a file of files that you wish to open
3. Drag and drop into the SplayCode window


###Manipulate the Files
####Resizing
The Editor blocks can be resized by pressing down on the borders or corners of the Editor blocks and dragging.

####Repositioning
The Editor blocks can be repositioned by pressing down on the title bar of an Editor block and dragging.

####Editing
Users can edit the program files adding or removing lines on the Editor blocks

###Closing Editors
####Closing One by One
Individual Editor blocks have close buttons in the top right corner. Clicking this button will remove the Editor from the virtual space

####Clear
Users can remove every Editor block open in the virtual space by simply clicking the ```Clear``` button in the toolbar

###Layout Persistence
####Saving Layout
1. Click on the ```Save Layout``` button in the toolbar
2. Navigate to the directory to save the layout file in
3. Name the layout file
4. Click ```Save``` button

####Loading Layout
1. Click on the ```Load Layout``` button in the toolbar
2. Navigate to the directory with the layout file to load
3. Select the layout file
4. Click ```Open``` button
