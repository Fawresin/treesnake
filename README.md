# Treesnake

Treesnake is a cross platform tool that can translate certain
[Python 3.0](https://en.wikipedia.org/wiki/Python_\(programming_language\)) scripts
into valid [Linden Scripting Language](http://wiki.secondlife.com/wiki/LSL_Portal)
(LSL) scripts.
It is meant for fun, but could also be used by people who prefer to write their
stuff in Python.

It targets the Microsoft .NET Framework 4.6.1.

Word of warning: expect lots of limits!

## Usage

Input files should be UTF-8 encoded.

```
Usage: treesnake.exe [OPTIONS]+ input
Compile a Python script into a Linden Scripting Language (LSL) script.

Options:
  -o, --output=VALUE         The file to output.
  -l, --log-level=VALUE      Logging level. Choices are: error, warn, info, or
                               debug. Default: error
  -v, --version              Show version information.
  -h, --help                 Show this message and quit.
```

## What's Supported

Nothing yet.

## What's Not Supported

Everything else. This may change with updates.

## What's Planned

- Comments
- Indentation to delimit code blocks.
- State definitions.
- Event handler definitions.
- Function definitions.
- Class definitions.
- Dynamic variable assignment.
- Dictionaries.
- Typecasting.
- Python arithmetic operators.
- Python order of operations.
- Python logical operators.
- Python bitwise operators.
- Python conditional statements.
- `in` operator.
- List slicing and striding.
- Loop statements.
- `break` and `continue` operators.
- Double or single quoted string literals.
- Multi-line string literals.
- String concatenation (`+`) and repeat (`*`) operators.
- String slicing.
- Import file statements.
- Namespaces.
- LSL built-in functions and constants.
- Python built-in functions: `abs`, `exit`, `min`, `max`, `len`, `pow`, `print`, `range`,
`round`, `sum`

## Coding Standards and Naming Conventions

Treesnake's codebase follows the coding standards and naming conventions [listed
here](https://github.com/ktaranov/naming-convention/blob/master/C%23%20Coding%20Standards%20and%20Naming%20Conventions.md).

## License

Treesnake is licensed under the [MIT License](https://en.wikipedia.org/wiki/MIT_License).
