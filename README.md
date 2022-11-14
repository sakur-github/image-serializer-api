## What
This is the repository for the back-end of the site located at https://image-serializer.kthsalar.se

## Who
This code is written by Adam at Sakur Ab.

## How
I recommend opening the solution file (.sln) with Visual Studio

The code for the image serialization/deserialization can be found in the Image class in /Models/Image.cs

In short it works by setting the bit values of a byte to control the on/off state of 8 pixels at a time.
One byte: 0000 0000, will hold the information of 8 pixels that can be either turned on or off. The bytes are then saved in a comma separated array.

## Why

Because I want to help people learn and do stuff. If you're scratching your head because the code is confusing and you still don't understand how the screen works, don't worry, me too.<br/><br/>I would recommend trying to make a 128x32 pixels large image that is pure white and upload it to the site at https://image-serializer.kthsalar.se<br/><br/>
Then add one black pixel at the top let corner of your picure and upload it again. Look at how the byte values have changed. It should now be a 1 at the top left corner.<br/><br/>Add another black pixel below the one you just added. Upload the picture again. Now it says 3.
Why?
Because the first byte holds the information of the 8 pixels in a vertical row down from the top left corner. Now there are two white pixels so the value is a bunch of 0's and two 11. In binary 11 is 3.
