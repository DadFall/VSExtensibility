// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PluginDeployment;

using Microsoft.VisualStudio.Extensibility;

/// <summary>
/// 扩展命令配置类 - 用于配置扩展的命令相关设置
/// Extension Command Configuration Class - Used to configure extension command-related settings
/// 
/// 设计说明：
/// Design notes:
/// 在这个扩展中，我们采用了简化的架构设计：
/// In this extension, we adopt a simplified architectural design:
/// 
/// 1. 命令直接在 PluginDeployCommand.cs 中配置
///    Commands are configured directly in PluginDeployCommand.cs
/// 2. 不需要额外的工具栏或复杂的 UI 配置
///    No additional toolbars or complex UI configuration needed
/// 3. 所有命令配置都通过 CommandConfiguration 属性完成
///    All command configurations are completed through CommandConfiguration property
/// 
/// 这种设计的优点：
/// Advantages of this design:
/// - 简单明了，易于理解和维护
///   Simple and clear, easy to understand and maintain
/// - 减少了不必要的配置复杂性
///   Reduces unnecessary configuration complexity
/// - 适合单一功能的扩展
///   Suitable for single-function extensions
/// 
/// 如果需要更复杂的命令配置（如工具栏、子菜单等），可以在这里添加：
/// If more complex command configurations are needed (such as toolbars, submenus, etc.), they can be added here:
/// - 工具栏配置 (Toolbar configuration)
/// - 子菜单配置 (Submenu configuration) 
/// - 快捷键配置 (Keyboard shortcut configuration)
/// - 命令分组配置 (Command grouping configuration)
/// </summary>
internal static class ExtensionCommandConfiguration
{
    // 当前扩展只使用上下文菜单命令，无需自定义工具栏或其他 UI 元素
    // Current extension only uses context menu commands, no custom toolbars or other UI elements needed
    
    // 如果将来需要添加工具栏配置，可以在这里定义：
    // If toolbar configuration needs to be added in the future, it can be defined here:
    // 
    // 示例：工具栏配置 (Example: Toolbar configuration)
    // public static readonly ToolbarConfiguration PluginDeploymentToolbar = new()
    // {
    //     Name = "PluginDeploymentToolbar",
    //     DisplayName = "插件发布工具栏",
    //     Commands = new[] { typeof(PluginDeployCommand) }
    // };
    
    // 示例：快捷键配置 (Example: Keyboard shortcut configuration)
    // public static readonly KeyboardShortcut PluginDeployShortcut = new()
    // {
    //     Key = "Ctrl+Shift+P",
    //     Command = typeof(PluginDeployCommand)
    // };
    
    // 示例：命令分组配置 (Example: Command group configuration)
    // public static readonly CommandGroup PluginCommands = new()
    // {
    //     Name = "PluginCommands",
    //     DisplayName = "插件命令",
    //     Commands = new[] { typeof(PluginDeployCommand) }
    // };
}
