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

If you then want to get a value out of the object, you can simple call one of the [get methods](# Get methods)

### Get methods
```c#
GetString(path);
```
