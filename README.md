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

If you then want to get a value out of the object, you can simple call one of the [get methods](#get-methods).
This could be done like this:
```c#
Console.WriteLine(bsynobj.GetString("teststring"));
```
> Output: Hello World!

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
