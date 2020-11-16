# bsyn
Bsyn is a tool for c#, that lets you encode and decode json code.

# How it works
## Decoding
If you want to decode a json file or just a string that contains json code, you can make a **BsynObject**.
This can be done like this:
```c#
string json = "{\"teststring\": \"Hello World!\"}";
BsynObject bsynobj = new BsynObject(json);
```

If you then want to get a value out of the object, you can simply call one of the [get methods](#get-methods).
This can be done like this:
```c#
Console.WriteLine(bsynobj.GetString("teststring"));
```
> Output: Hello World!

You can also do nested BsynObjects. This means that if you have a BsynObject with json code, that looks a little like this:
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

But we can do this even simpler!
If we want to get the value from `teststring1` we can simply just do this:
```c#
Console.WriteLine(bsyn.GetString("testobject.teststring1"));
```
> Output: Hello 1

## Encoding
If you want to encode any object or array into json code, you can do it easely with a BsynObject.
If we for example want to encode these values:
```c#
string firstName = "Elon";
string lastName = "Musk";
int age = 49;
dobule bankAmountInMil = 3.9;
```
We can do it like so:
```c#
Bsyn bsynEncode = new Bysn();
     bsynEncode.append("firstName", firstName);
     bsynEncode.append("lastName", lastName);
     bsynEncode.append("age", age);
     bsynEncode.append("bankAmountInMil", bankAmountInMil);
string json = bsynEncode.stringified;
Console.WriteLine(json);
```
> Output: {\"firstName\":"Elon",\"lastName\":"Musk",\"age\":49,\"bankAmountInMil\":3.9}

But you can also nest the values:
```c#
Bsyn elonMuskObj = new Bsyn();
     elonMuskObj.append("firstName", firstName);
     elonMuskObj.append("lastName", lastName);
     elonMuskObj.append("age", age);
     elonMuskObj.append("bankAmountInMil", bankAmountInMil);
     
Bsyn bsynEncode = new Bysn();
     bsynEncode.append("elonMusk", elonMuskObj);
     
string json = bsynEncode.stringified;
Console.WriteLine(json);
```
> Output: {\"elonMusk\":{\"firstName\":"Elon",\"lastName\":"Musk",\"age\":49,\"bankAmountInMil\":3.9}}

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
