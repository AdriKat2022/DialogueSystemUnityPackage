# Dialogue System Editor Unity Package

This package offers a Unity editor window to create and manage dialogue systems.  
It is designed to be quickly design simple dialogue systems for games inside Unity itself.

# Installation
1. Open the Unity Package Manager `(Window -> Package Manager)`
5. Click on the "+" button and select `"Add package from git URL..."`
6. Paste the following URL: `git+https://github.com/AdriKat2022/DialogueSystemUnityPackage.git`
7. Click on "Install"
8. Enjoy!


# Usage
1. Open the Dialogue System Editor window `(Window -> Dialogue System Editor)`
2. Create nodes by right-clicking
3. Make dialogues!

Save with the "Save" button and load with the "Load" button.
Warning: There is no confirmation dialog yet when saving, loading, overwritting or quitting without saving so be careful to save each time!

# Features
- Single Nodes
- Multiple Nodes (nodes with choices)
- Conditional Nodes (nodes with conditions)

## Single Nodes
Single nodes are the most basic nodes.  
They contain a single text with no possibility whatsoever and either end or continue to another node.

## Multiple Nodes
Multiple nodes are branch nodes according to the player's choice.  
They contain a text and a variable list of options whose length is adjustable.  
Each option bears a name and can lead to a specific node.

## Conditional Nodes
Conditional nodes are branch nodes according to a condition or list of conditions.  
This node doesn't appear at any time in the dialogue and is purely logical, as the story automatically continues to the appropriate node. 
They only have two outputs: one for when the condition is true and one for when it is false.  

### Condition mode
- **All**: All conditions of the list must be true to go to the true output.
- **Any**: At least one condition of the list must be true to go to the true output.

### Condition types
Conditions are currently supported for the following types and their respective operators:
- **Boolean**: Is, And, Or, Xor
- **Integer**: Equal, NotEqual, Greater, GreaterOrEqual, Less, LesserOrEqual
- **String**: Equal, NotEqual, Contains, StartsWith, EndsWith

### Condition Variables
Conditions are great, but they need values to be compared to!  
For this, the dynamic variables used for the conditions are stored in a static dictionary at runtime, using strings to localize them.  

> **STRINGS? REALLY? Yes, strings.**

However **DOT NOT FEAR**, as the editor provides a very useful way to manage these values in a user-friendly way!  
I decided to make it **TYPE-SAFE**, meaning you only have to write the name of the variable once and then you can select it from a dropdown list everywhere else.  
Pretty neat, huh?

This is how it works:
1. **Create a new DialogueVariableNames scriptable object `(Assets -> Create -> Dialogue System -> Variable Names Condition Container)`**
2. **Define your variables in there**  

Yep that's it! Just don't forget to use a Condition Initializer `(GameObject->Dialogue System->Variable Condition Initializer)` in your scene (or add the Condition Initializer component directly in any game object active at the beginning, you choose!).

You only need one at the very beginning of your game. It will initialize the dictionary with the values you set in the scriptable object that you give it. Then you can even tell it to disappear once it has done its job!

> **Note**: Variables can only be tested against **hard coded values** for now.

#### Condition variables in CODE
Of course, the variables you declared in the scriptable object can be accessed in your code! Yeah otherwise *static* variables would be pretty useless, right?

Okay remember when I said it was type-safe? Well, I kinda lied.  
You can't access the variables directly in code, but you can access them through the `DialogueVariable` static class with the proper variable key.

```csharp
using AdriKat.DialogueSystem.Variables;
using UnityEngine;

public class MyScript : MonoBehaviour
{
    private void Start()
    {
        // Get the value of a variable
        int myInt = (int)DialogueVariable.GetInt("MyInt");
        string myString = (string)DialogueVariable.GetString("MyString");
        bool myBool = (bool)DialogueVariable.GetBool("MyBool");

        // Set the value of a variable
        DialogueVariable.SetInt("MyInt", 42);
        DialogueVariable.SetString("MyString", "Hello World!");
        DialogueVariable.SetBool("MyBool", true);

        // Returns NULL is the variable doesn't exist
        int? myInt2 = DialogueVariable.GetInt("MyInt2");

        // Creates the variable if it doesn't exist
        DialogueVariable.SetInt("MyInt2", 42);
    }
}
```

I'm sorry for the inconvenience, I'm working on it.  
Maybe generating at compilation or on prompt a enum with the variable names for instance?  
I don't know yet, but I'll find a way to make it better!  

# Future Features
- [ ] Confirmation dialog when saving, loading, overwritting or quitting without saving

# Known Issues
I'm not aware of any major issue, but I'm sure there are some!