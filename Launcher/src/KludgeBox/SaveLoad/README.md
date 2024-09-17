# KludgeBox.SaveLoad

What a cursed way to save and load data!

## How to use

If you use only primitive types, you can use this just out-of-box.

To save data you need to define custom class that implements the `IExposable` interface.

`IExposable` contains only one method - `void ExposeData()`. All Save/Load logic must be contained here.

There is 5 default SaveLoad classes, that is enough in most cases:
- **SaveLoad_Value** - uses `Parser` class to serialize and deserialize value types
- **SaveLoad_Exposable** - processes all classes that implement `IExposable` interface
- **SaveLoad_Reference** - processes all classes that implement `IReferencable` interface.
Saves only one copy of the object as `IExposable`. Restores targets by reference.
- **SaveLoad_Array** - processes List<TValue>, where TValue can be ValueType, IExposable or IReferencable
- **SaveLoad_Dictionary** - processes Dictionary<string, TValue>, same as Array.

Here is example how to use this:
```csharp
var exposable = new Exposable()
{
    number = 100,
    someText = "SAMPLE_TEXT",
    real = 3.14,
    exp = new()
    {
        
    }
}
string text = SaveLoad.Save(exposable);
Exposable restoredExposable = SaveLoad.Load<Exposable>(text);

public class Exposable : IExposable
{
    int number = 0;
    string someText = "";
    double real = 0;
    AnotherExposable exp;
    
    public void ExposeData()
    {
        SaveLoad_Value.Link(ref number, "num");
        SaveLoad_Value.Link(ref someText, "txt");
        SaveLoad_Value.Link(ref real, "real");
        SaveLoad_Exposable.Link(ref exp, "thing");
    }
}

public class AnotherExposable : IExposable
{
    int anotherNum = 0;
    string anotherText = "";
    double anotherReal = 0;
    
    public void ExposeData()
    {
        SaveLoad_Value.Link(ref anotherNum, "num");
        SaveLoad_Value.Link(ref anotherText, "txt");
        SaveLoad_Value.Link(ref anotherReal, "real");
    }
}
```

## Custom parsers
You can process your custom types with `SaveLoad_Value`, if `Parser` can parse it from it's `ToString()` result.
So you need 2 things to do that:
1. override `ToString()` method for your type
2. register parser, that can transform `ToString()` result back to your type instance

Here is a parser registration example:
```csharp
public YourType ParseMyType(string text)
{
    // Some parsing logic here
    return result;
}

Parser.Parsers<YourType>.Register(ParseMyType);
```