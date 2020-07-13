# bsyn
Bsyn is a tool for c#, that lets you encode and decode json code.

# How it works
## Decoding
If you want to decode a json file or just a string that contains json code, you can make an **BsynObject**
This can be done like this:
```c#
string json = "{\"teststring\": \"Hello World!\"}";
BsynObject bsynobj = new BsynObject(json);
```

If you then want to get a value out of the object, you can simply call one of the [get methods](#get-methods).
This could be done like this:
```c#
Console.WriteLine(bsynobj.GetString("teststring"));
```
> Output: Hello World!

You can also do nested BsynObjects. This means that if you have a BsynObject that looks a little like this:
```c#
string json = "{\"testobject\": {\"teststring1\": \"Hello 1\", \"stringstring2\": \"Hello 2\"}}";
BsynObject bsyn = new BsynObject(json);
```
You can store one of the objects in a seperate variable. In this example the object is "testobject". We can store this object in another variable, like this:
```c#
BsynObject testobj = bsyn.GetObject("testobject");
```
If we then want to write the first string of the object to the console, we can just do it like this:
```c#
Console.WriteLine(testobj.GetString("teststring1"));
```
> Output: Hello 1

### Get methods
```c#
GetString(string path); // String
```
```c#
GetInt(string path); // Int
```
```c#
GetFloat(string path); // Float
```
```c#
GetObject(string path); // BsynObject
```
```c#
GetStringArray(string path); // String[]
```
```c#
GetIntArray(string path); // Int[]
```
```c#
GetFloatArray(string path); // Float[]
```
```c#
GetObjectArray(string path); // BsynObject[]
```
