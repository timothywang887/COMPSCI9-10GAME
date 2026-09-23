# This file is to standardize all conventions for the project. Please follow these conventions when creating new files or editing existing ones.

## Variable, Method, Class, and Argument Naming Conventions
* All variables should be in camelCase. No matter if they are public or private, they should be in camelCase.
* All methods should be in PascalCase. No matter if they are public or private, they should be in PascalCase.
* All classes should be in PascalCase. No matter if they are public or private, they should be in PascalCase.
* All arguments made in a method should be in _underscoreCamelCase.

* All variables should be private. If you need to access a variable from another class, create public getter and setter methods for it. Do not make variables public.
* If a variable needs to be accessed by Unity from the inspector, use the [SerializeField] attribute to make it visible in the inspector while keeping it private.

## File Naming Conventions
* Non C# files should be named in snake_case.
* Folders should be named in PascalCase.
* C# files should be named in PascalCase. The name of the file should match the name of the class inside it. If there are multiple classes in a file, the file name should match the name of the main class in it.

## Commit Names
* Commits should be very descriptive. At least 3 words.
* Just be smart about it please.