<!-- markdownlint-disable MD033 -->
<!-- markdownlint-disable MD010 -->
# Json Power Inspector for Typed Programming Language

[![GitHub Release](https://img.shields.io/github/v/release/Delsin-Yu/Json-Power-Inspector)](https://github.com/Delsin-Yu/Json-Power-Inspector/releases/latest) [![Stars](https://img.shields.io/github/stars/Delsin-Yu/Json-Power-Inspector?color=brightgreen)](https://github.com/Delsin-Yu/Json-Power-Inspector/stargazers) [![License](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/Delsin-Yu/Json-Power-Inspector/blob/main/LICENSE)

Json Power Inspector is a JSON editor that offers advanced GUI editing experience based on the serialization information created from a typed language.

## Using the application

1. [Create a `.jsontemplate` file from your data structure.](#create-a-jsontemplate-file-from-your-data-structure)
2. Download and unzip the application to a place where you can remember.
3. Launch the application.
4. Drag and drop your `.jsontemplate` file to the application window.
5. Start editing.

## Create a JsonTemplate file from your data structure

Before using the application, developers must serialize their data structure into a dedicated `.jsontemplate` file that contains type information.

We provide a serializer Nuget package for `C#/dotnet8` developers, so you are more than welcome to create your own version of the serializer for your language/environment.

### Documentation for `JsonPowerInspector.Template` Nuget Package Users

[![NuGet Version](https://img.shields.io/nuget/v/JsonPowerInspector.Template)](https://www.nuget.org/packages/JsonPowerInspector.Template) ![NuGet Downloads](https://img.shields.io/nuget/dt/JsonPowerInspector.Template)

#### Usage

- Install the [JsonPowerInspector.Template](https://www.nuget.org/packages/JsonPowerInspector.Template) Nuget package into the C# project that contains the data structure you wish to work with. For demonstration purposes, let's use this `MyItem` type as an example.

```csharp
public struct MyItem
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int Price { get; set; }
}
```

- Use the following line to serialize the model and save it into a jsontemlpate file.

```csharp
var definition = TemplateSerializer.CollectTypeDefinition<MyItem>();
var jsonTemplateString = TemplateSerializer.Serialize(definition);
File.WriteAllText("MyItem.jsontemplate", jsonTemplateString);
```

#### Supported features and restrictions

- The serializer collects type info for `instance` `Properties` that are `publicly` available and have both `get` and `set` accessors.
- The following types and features are supported by the serializer:

|Type info ______________|Restrictions ______________|Inspector type __________________________________|Customizable Display Name ___________________________________|Restrict the number range ______________________________|Dropdown support ___________________________|
|-|-|:-:|-|-|-|
|`T[]` or `List<T>`|Nested types are displayed with their corresponding inspector|`Array Inspector`<br/><br/>![image](https://github.com/Delsin-Yu/Json-Power-Inspector/assets/71481700/559687f7-3084-4c9d-b55d-c729a686af55)<br/>|Annotate the property with `InspectorNameAttribute` to customize the name shown in the editor header|Annotate the property with `NumberRangeAttribute` for the array element type, the type itself should be compatible with `NumberRange`.|Annotate the property with `DropdownAttribute` for the array element type, the type itself should be compatible with `Dropdown`|
|`Dictionary<TKey, TValue>`|`TKey` only supports `Numbers` or `Strings` (.Net restrictions)|`Dictionary Inspector`<br/><br/>![image](https://github.com/Delsin-Yu/Json-Power-Inspector/assets/71481700/4b78b962-8e34-47ab-b439-12e6091c7e87)<br/>|Annotate the property with `InspectorNameAttribute` to customize the name shown in the editor header|Annotate the property with `KeyNumberRangeAttribute` for `TKey`<br/> and use `ValueNumberRangeAttribute` for `TValue`, <br/>the annotated type should be a `NumberRange` compatible type.|Annotate the property with `KeyDropdownAttribute` for `TKey`<br/> and use `ValueDropdownAttribute` for `TValue`, <br/>the annotated type should be a `Dropdown` compatible type, the corresponding types gets |
|`bool`|N/A|`Boolean Inspector`<br/><br/>![image](https://github.com/Delsin-Yu/Json-Power-Inspector/assets/71481700/5ab478be-39c1-43f7-b7e3-e2fe6e0bd419)<br/>|Annotate the property with `InspectorNameAttribute` to customize the name shown in the editor name|Not supported|Not supported|
|Primitive number types `byte`, `ushort`, `uint`, `ulong`, `sbyte`, `short`, `int`, `long`, `float`, and `double`|Integer types only support inputting integral values, where Float types support inputting values with decimal|`Number Inspector`<br/><br/>![image](https://github.com/Delsin-Yu/Json-Power-Inspector/assets/71481700/85e035dc-845e-467f-89b2-8d7c7a8f0433)<br/>|Annotate the property with `InspectorNameAttribute` to customize the name shown in the editor name|Annotate the property with `NumberRangeAttribute` to customize the value range|Use `DropdownAttribute` to customize the dropdown data source and value resolver|
|`string`|N/A|`String Inspector`<br/><br/>![image](https://github.com/Delsin-Yu/Json-Power-Inspector/assets/71481700/50b37079-a32b-410f-abc2-0544054684aa)<br/>|Annotate the property with `InspectorNameAttribute` to customize the name shown in the editor name|Not supported|Annotate the property with `DropdownAttribute` to customize the dropdown data source and value resolver|
|`enum`|Enum Flags are not supported currently |`Enum Inspector`<br/><br/>![image](https://github.com/Delsin-Yu/Json-Power-Inspector/assets/71481700/7070dfe9-ed68-4813-b61e-86c1b866dbd5)<br/>|Annotate the property with `InspectorNameAttribute` to customize the name shown in the editor name, or annotate the enum values with `InspectorNameAttribute` to customize the names shown in the dropdown|Not supported|Not supported|
|Other Non-Generic Types|Only `instance` `Properties` that are `publicly` available and have both `get` and `set` accessors are recorded.|`Object Inspector`<br/><br/>![image](https://github.com/Delsin-Yu/Json-Power-Inspector/assets/71481700/af703fe5-a952-49ac-b859-615ea7750beb)<br/>|Annotate the property with `InspectorNameAttribute` to customize the name shown in the editor header|Not Supported|Not Supported|

##### Example

- The model used for serialization:

```csharp
public class MyDemoModel
{
    /// <summary>
    /// Displayed as "Int Array" in inspector.
    /// Each array element has an input range clamps to -2 to 2.
    /// </summary>
    [InspectorName("Int Array"), NumberRange(-2, 2)] 
    public int[] MyIntArrayProperty { get; set; }
    
    /// <summary>
    /// Displayed as "Dictionary" in inspector.
    /// When adding a dictionary element,
    /// the input range for the key clamps to 0 to 10.
    /// </summary>
    [InspectorName("Dictionary"), KeyNumberRange(0, 10)] 
    public Dictionary<int, string> MyDictionaryProperty { get; set; }
    
    /// <summary>
    /// Displayed as "MyBool" in inspector.
    /// </summary>
    public bool MyBool { get; set; }
    
    /// <summary>
    /// Displayed as "Number Value" in inspector.
    /// Have an input range clamps to -10 to 10.
    /// </summary>
    [InspectorName("Number Value"), NumberRange(-10, 10)] 
    public float MyFloat { get; set; }
    
    /// <summary>
    /// Displayed as "Number Value" in inspector.
    /// Use a dropdown for selecting the values.
    /// </summary>
    [InspectorName("String Value"), Dropdown("StringSelection.tsv")] 
    public string MyString { get; set; }
    
    /// <summary>
    /// Displayed as "Time Type" in inspector.
    /// Use a dropdown for selecting the enum values.
    /// </summary>
    [InspectorName("Time Type")]
    public DateTimeKind MyDateTimeKind { get; set; }
    
    /// <summary>
    /// Displayed as "Nested Model" in inspector.
    /// </summary>
    [InspectorName("Nested Model")]
    public MyDemoModel Nested { get; set; }
}
```

- The serialization code:

```csharp
var definition = TemplateSerializer.CollectTypeDefinition<MyDemoModel>();
var jsonText = TemplateSerializer.Serialize(definition);
File.WriteAllText("MyDemoModel.jsontemplate", jsonText, Encoding.UTF8);
```

- And the content for `StringSelection.tsv`, which should be placed int the same as `MyDemoModel.jsontemplate`.

```text
Value	Display
Lorem	String Value: Lorem
ipsum	String Value: ipsum
dolor	String Value: dolor
sit	String Value: sit
amet	String Value: amet
consectetur	String Value: consectetur
adipiscing	String Value: adipiscing
elit	String Value: elit
```

- Here is a screenshot of the inspector after loading the `MyDemoModel.jsontemplate`.

### Documentation for creating your serializer and `jsontemlpate` file specification

WIP
