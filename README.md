# Elmish.WPF Starter

A tempalte for [Elmish.WPF](https://github.com/elmish/Elmish.WPF) applications.

## Usage:

1. Install the template if it's not installed yet:
    ```
    git clone https://github.com/fuchen/Elmish.WPF.Starter
    dotnet new -i Elmish.WPF.Starter/src/content
    ```
2. Create project:
    ```
    dotnet new elmish-wpf -n <YourPorjectName>
    ```

## Features:

* MVU pattern for WPF application, powered by [Elmish.WPF](https://github.com/elmish/Elmish.WPF).
* Single F# project. C# view project is not needed.
* Hot reload view window when source xaml file is changed.
