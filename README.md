<!-- markdownlint-disable MD033 -->
<!-- markdownlint-disable MD010 -->
# Json Power Inspector for Typed Programing Languauge

Json Power Inspector is a JSON editor that offers advanced GUI editing experience based on the serialization information created from a typed language.

## Using the application

1. [Create a `.jsontemplate` file from your data structure.](#create-a-jsontemplate-file-from-your-data-structure)
2. Download and unzip the application to a place where you can remember.
3. Launch the application.
4. Drag and drop your `.jsontemplate` file to the application window.
5. Start editing.

## Create a JsonTemplate file from your data structure

Before start using the application, it is required for the developers to serialize their data structure into a dedicated `.jsontemplate` file that contains type information.

We provide a serializer Nuget package for `C#/dotnet8` developers, you are more than welcome to create your version of the serializer for your language/environment.

### Documentation for `JsonPowerInspector.Template` Nuget Package Users

#### Usage

- Install the [JsonPowerInspector.Template](https://www.nuget.org/packages/JsonPowerInspector.Template) Nuget package into the C# project that contains the data structure you wish to work with, for demonstration purpose, let's use this `MyItem` type as an example.

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

- The serializer collect type info for `instance` `Properties` that are `publicly` avaible and have both `get` and `set` accessor.
- The following types and features are supported by the serializer:

|Type info|Restrictions|Inspector Type|Customizable Display Name|Restrict the number range|Displays a dropdown instead of value editor|
|-|-|-|-|-|-|
|`T[]` or `List<T>`|Nested types are displayed with their corresponding inspector|`Array Inspector`|Annotate the property with `InspectorNameAttribute` to customize the name shown in the editor header|Annotate the property with `NumberRangeAttribute` for the array element type, the type itself should be compatible with `NumberRange`.|Annotate the property with `DropdownAttribute` for the array element type, the type itself should be compatible with `Dropdown`|
|`Dictionary<TKey, TValue>`|`TKey` only support `Numbers` or `Strings` (.Net restrictions)|`Dictionary Inspector`|Annotate the property with `InspectorNameAttribute` to customize the name shown in the editor header|Annotate the property with `KeyNumberRangeAttribute` for `TKey`<br/> and use `ValueNumberRangeAttribute` for `TValue`, <br/>the annotated type should be a `NumberRange` compatible type.|Annotate the property with `KeyDropdownAttribute` for `TKey`<br/> and use `ValueDropdownAttribute` for `TValue`, <br/>the annotated type should be a `Dropdown` compatible type, the correcponding types gets |
|`bool`|N/A|`Boolean Inspector`|Annotate the property with `InspectorNameAttribute` to customize the name shown in the editor name|Not supported|Not supported|
|Primitive number types `byte`, `ushort`, `uint`, `ulong`, `sbyte`, `short`, `int`, `long`, `float`, and `double`|Integer types only support inputing integral values, where Float types support inputing values with decimal|`Number Inspector`|Annotate the property with `InspectorNameAttribute` to customize the name shown in the editor name|Annotate the property with `NumberRangeAttribute` to customize the value range|Use `DropdownAttribute` to customize the dropdown data source and value resolver|
|`string`|N/A|`String Inspector`|Annotate the property with `InspectorNameAttribute` to customize the name shown in the editor name|Not supported|Annotate the property with `DropdownAttribute` to customize the dropdown data source and value resolver|
|`enum`|Enum Flags are not supported curerntly|`Enum Inspector`|Annotate the property with `InspectorNameAttribute` to customize the name shown in the editor name, or annotated the enum values with `InspectorNameAttribute` to customize the names shown in the dropdown|Not supported|Not supported|
|Other Non-Generic Types|Only `instance` `Properties` that are `publicly` avaible and have both `get` and `set` accessor are recorded.|`Object Inspector`|Annotate the property with `InspectorNameAttribute` to customize the name shown in the editor header|Not Supported|Not Supported|

##### Example

- The model use for serialization:

```csharp
public class MyDemoModel
{
    /// <summary>
    /// Displayed as "Int Array" in inspector.
    /// Each array element have an input range clamps to -2 to 2.
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

- And the content for `StringSelection.tsv`, which should be placed int the same with `MyDemoModel.jsontemplate`.

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
