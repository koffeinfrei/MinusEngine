MinusEngine
===========
A  C# library to access [Minus API](http://minus.com/pages/api).
Also includes support for Windows Phone 7

License
-------
The license for this project is [Apache Software License 2.0](http://www.apache.org/licenses/LICENSE-2.0.html).

It means you can pretty much do whatever you want with it IF you give back some credit.

Event handling
--------------
Every operation has two possible outputs: completion or failure.

Thus, for each operation on the class MinusEngine (which in turn represents a call to the actual Minus site) you have to register a success and failure delegate to handle these events.

The asynchronous nature of the library makes it perfect to develop UI-based applications on top of it.

Current State
-------------
Please read the changelog.

Contact
-------
If you have any questions, please email me at cardonfry@gmail.com

Dependencies
------------
This project uses the [Json.NET](http://json.codeplex.com/) library, by James Newton-King.