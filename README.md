## What
This is the repository for the back-end of the site located at https://image-serializer.kthsalar.se

## Who
This code is written by Adam at Sakur Ab.

## How
I recommend opening the solution file (.sln) with Visual Studio

The code for the image serialization/deserialization can be found in the Image class in /Models/Image.cs

In short it works by setting the bit values of a byte to control the on/off state of 8 pixels at a time.
One byte: 0000 0000, will hold the information of 8 pixels that can be either turned on or off. The bytes are then saved in a comma separated array.
