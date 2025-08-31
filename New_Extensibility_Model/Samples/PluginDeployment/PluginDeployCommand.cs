// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PluginDeployment;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;
using Microsoft.VisualStudio.Extensibility.Shell;

/// <summary>
/// 插件发布命令类 - 实现项目项右键菜单的"插件发布"功能
/// Plugin Deployment Command Class - Implements "Plugin Release" functionality for project item right-click menu
/// 
/// 这个类是整个扩展的核心功能实现，负责：
/// This class is the core functionality implementation of the entire extension, responsible for:
/// 1. 定义命令在 Visual Studio 界面中的位置（右键菜单）
///    Defining the command's position in Visual Studio interface (right-click menu)
/// 2. 配置命令的显示名称和本地化
///    Configuring the command's display name and localization
/// 3. 实现命令被点击时的具体行为
///    Implementing the specific behavior when the command is clicked
/// 
/// 技术实现说明：
/// Technical implementation notes:
/// - 继承自 Command 基类，这是 VS 扩展性模型的标准做法
///   Inherits from Command base class, which is the standard practice for VS extensibility model
/// - 使用 CommandPlacement 来定位菜单位置
///   Uses CommandPlacement to locate menu position
/// - 通过 VSCT GUID 和 ID 来精确定位右键菜单
///   Uses VSCT GUID and ID to precisely locate right-click menu
/// </summary>
[VisualStudioContribution] // 标记为 Visual Studio 扩展贡献 (Mark as Visual Studio extension contribution)
internal class PluginDeployCommand : Command
{
    /// <summary>
    /// 命令配置属性 - 定义命令的显示名称和菜单位置
    /// Command configuration property - defines command display name and menu placement
    /// 
    /// 菜单位置配置说明：
    /// Menu placement configuration explanation:
    /// 
    /// VSCT GUID "{d309f791-903f-11d0-9efc-00a0c911004f}" 是 Visual Studio 标准菜单组的 GUID
    /// VSCT GUID "{d309f791-903f-11d0-9efc-00a0c911004f}" is the GUID for Visual Studio standard menu groups
    /// 
    /// 常用的 ID 值说明：
    /// Common ID value explanations:
    /// - ID 537: 解决方案节点的右键菜单 (Solution node right-click menu)
    /// - ID 518: 项目节点的右键菜单 (Project node right-click menu)  
    /// - ID 521: 项目文件/项目项的右键菜单 (Project file/project item right-click menu)
    /// 
    /// priority 参数控制菜单项在菜单中的显示顺序，数值越小越靠前
    /// priority parameter controls the display order of menu items, smaller values appear first
    /// </summary>
    /// <inheritdoc />
    public override CommandConfiguration CommandConfiguration => new("%PluginDeployment.PluginDeployCommand.DisplayName%")
    {
        Placements =
        [
            // 以下是不同菜单位置的配置选项，目前启用项目节点菜单
            // The following are configuration options for different menu positions, currently enabling project node menu
            
            // CommandPlacement.VsctParent(new Guid("{d309f791-903f-11d0-9efc-00a0c911004f}"), id: 537, priority: 0), 
            // ↑ 解决方案的右键菜单 - 在解决方案资源管理器中右键点击解决方案名称时显示
            // ↑ Solution right-click menu - displayed when right-clicking solution name in Solution Explorer
            
            CommandPlacement.VsctParent(new Guid("{d309f791-903f-11d0-9efc-00a0c911004f}"), id: 518, priority: 0), 
            // ↑ 项目的右键菜单 - 在解决方案资源管理器中右键点击项目名称时显示  
            // ↑ Project right-click menu - displayed when right-clicking project name in Solution Explorer
            
            // CommandPlacement.VsctParent(new Guid("{d309f791-903f-11d0-9efc-00a0c911004f}"), id: 521, priority: 0), 
            // ↑ 项目文件的右键菜单 - 在解决方案资源管理器中右键点击文件时显示
            // ↑ Project file right-click menu - displayed when right-clicking files in Solution Explorer
        ],
    };

    /// <summary>
    /// 执行命令的异步方法 - 当用户点击"插件发布"菜单项时调用
    /// Asynchronous method to execute command - called when user clicks "Plugin Release" menu item
    /// 
    /// 方法执行流程：
    /// Method execution flow:
    /// 1. 接收用户的点击事件
    ///    Receive user click event
    /// 2. 获取当前的客户端上下文（包含当前选中的项目/文件信息）
    ///    Get current client context (contains currently selected project/file information)
    /// 3. 显示用户反馈消息
    ///    Display user feedback message
    /// 4. 可以在此处添加实际的插件发布逻辑
    ///    Actual plugin deployment logic can be added here
    /// 
    /// 参数说明：
    /// Parameter explanations:
    /// - context: 客户端上下文，包含当前 VS 环境的状态信息
    ///   context: Client context containing current VS environment state information
    /// - cancellationToken: 取消令牌，用于支持操作取消
    ///   cancellationToken: Cancellation token for supporting operation cancellation
    /// </summary>
    /// <param name="context">客户端上下文 (Client context)</param>
    /// <param name="cancellationToken">取消令牌 (Cancellation token)</param>
    /// <returns>异步任务 (Asynchronous task)</returns>
    /// <inheritdoc />
    public override async Task ExecuteCommandAsync(IClientContext context, CancellationToken cancellationToken)
    {
        // 显示提示消息"插件发布中" - 使用本地化字符串资源
        // Display progress message "Plugin is being released" - using localized string resources
        // 
        // ShowPromptAsync 方法的功能：
        // ShowPromptAsync method functionality:
        // - 在 Visual Studio 中显示模态对话框
        //   Display modal dialog in Visual Studio
        // - 支持本地化消息（通过 %key% 语法引用字符串资源）
        //   Support localized messages (reference string resources via %key% syntax)
        // - 提供用户交互选项（这里使用 PromptOptions.OK 只显示确定按钮）
        //   Provide user interaction options (here using PromptOptions.OK to show only OK button)
        await this.Extensibility.Shell().ShowPromptAsync(
            "%PluginDeployment.PluginDeployCommand.ProgressMessage%", // 引用本地化字符串 (Reference localized string)
            PromptOptions.OK, // 仅显示确定按钮 (Show only OK button)
            cancellationToken); // 传入取消令牌以支持操作取消 (Pass cancellation token to support operation cancellation)
        
        // 在这里可以添加实际的插件发布逻辑
        // Actual plugin deployment logic can be added here
        // 例如：
        // For example:
        // - 获取当前选中的项目或文件信息
        //   Get information about currently selected project or file
        // - 执行构建、打包操作
        //   Execute build and packaging operations
        // - 上传到插件仓库或部署到目标环境
        //   Upload to plugin repository or deploy to target environment
        // - 显示部署结果和日志
        //   Display deployment results and logs
    }
}
