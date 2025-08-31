# Plugin Deployment Extension

This extension adds a "插件发布" (Plugin Release) command to the right-click context menu of project items in Visual Studio.

## Features

- **Right-click context menu**: Adds "插件发布" command to project items
- **User feedback**: Shows "插件发布中" (Plugin is being released) message when clicked

## Usage

1. Right-click on any file in a project within Solution Explorer
2. Select "插件发布" from the context menu
3. A message dialog will appear showing "插件发布中"

## Technical Details

This extension uses the Visual Studio Extensibility model with:
- Command placement on project item context menu (ID: 521)
- Localized string resources for Chinese UI
- Simple message prompt for user feedback

Based on the CommandParentingSample from the VSExtensibility repository.
