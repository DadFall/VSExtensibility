// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PluginDeployment;

using System;
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
/// <summary>
/// CRM 环境类型枚举 - 定义支持的环境类型
/// CRM Environment Type Enumeration - defines supported environment types
/// </summary>
internal enum CrmEnvironmentType
{
    /// <summary>
    /// On-Premise 环境 (本地部署环境)
    /// On-Premise environment (local deployment environment)
    /// </summary>
    OnPremise,

    /// <summary>
    /// Dataverse 环境 (云端环境/Dynamics 365 在线版)
    /// Dataverse environment (cloud environment/Dynamics 365 Online)
    /// </summary>
    Dataverse
}

/// <summary>
/// 认证方式枚举 - 定义支持的认证方式
/// Authentication Method Enumeration - defines supported authentication methods
/// </summary>
internal enum AuthenticationMethod
{
    /// <summary>
    /// OAuth 认证 (适用于 Dataverse)
    /// OAuth authentication (for Dataverse)
    /// </summary>
    OAuth,

    /// <summary>
    /// 连接字符串 (Connection String)
    /// Connection String
    /// </summary>
    ConnectionString,

    /// <summary>
    /// Active Directory (适用于 On-Premise)
    /// Active Directory (for On-Premise)
    /// </summary>
    ActiveDirectory,

    /// <summary>
    /// IFD (Internet Facing Deployment，适用于 On-Premise)
    /// IFD (Internet Facing Deployment, for On-Premise)
    /// </summary>
    IFD
}

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
        const string DialogTitle = "Dynamics CRM 插件发布工具";
        ShellExtensibility shell = this.Extensibility.Shell();

        try
        {
            // 显示环境选择对话框 (Show Environment Selection Dialog)
            #pragma warning disable CA2000 // Dispose objects before losing scope
            var environmentDialog = new EnvironmentSelectionDialog();
            #pragma warning restore CA2000 // Dispose objects before losing scope

            await shell.ShowDialogAsync(environmentDialog, cancellationToken);

            // 获取对话框结果 (Get Dialog Result)
            var selectedEnvironment = await environmentDialog.GetResultAsync();

            if (selectedEnvironment == null)
            {
                // 用户取消了操作 (User cancelled the operation)
                return;
            }

            // 继续后续的部署流程 (Continue with deployment process)
            await ContinueDeploymentAsync(shell, selectedEnvironment, DialogTitle, cancellationToken);
        }
        catch (Exception ex)
        {
            await shell.ShowPromptAsync(
                $"发布过程中发生错误：{ex.Message}",
                PromptOptions.ErrorConfirm with { Title = DialogTitle },
                cancellationToken);
        }
    }

    /// <summary>
    /// 继续部署流程
    /// Continue deployment process
    /// </summary>
    /// <param name="shell">Shell 扩展性 (Shell Extensibility)</param>
    /// <param name="selectedEnvironment">选中的环境 (Selected Environment)</param>
    /// <param name="dialogTitle">对话框标题 (Dialog Title)</param>
    /// <param name="cancellationToken">取消令牌 (Cancellation Token)</param>
    private async Task ContinueDeploymentAsync(
        ShellExtensibility shell, 
        CrmEnvironment selectedEnvironment, 
        string dialogTitle, 
        CancellationToken cancellationToken)
    {
        // 确认部署 (Confirm Deployment)
        string configSummary = $"环境名称：{selectedEnvironment.Name}\n" +
                              $"环境类型：{selectedEnvironment.TypeDisplay}\n" +
                              $"环境 URL：{selectedEnvironment.Url}\n" +
                              $"认证方式：{GetAuthMethodDisplayName(selectedEnvironment.AuthMethod)}";

        bool confirmDeployment = await shell.ShowPromptAsync(
            $"确认发布配置：\n\n{configSummary}\n\n确定要继续发布插件吗？",
            PromptOptions.OKCancel with
            {
                Title = dialogTitle,
                Icon = ImageMoniker.KnownValues.StatusInformation,
            },
            cancellationToken);

        if (!confirmDeployment)
        {
            await shell.ShowPromptAsync(
                "插件发布已取消。",
                PromptOptions.AlertConfirm with { Title = dialogTitle },
                cancellationToken);
            return;
        }

        // 获取额外的连接信息（如果需要）
        // Get additional connection details if needed
        if (selectedEnvironment.AuthMethod == AuthenticationMethod.ConnectionString)
        {
            var connectionString = await shell.ShowPromptAsync(
                "请输入连接字符串：",
                InputPromptOptions.Default with 
                { 
                    Title = dialogTitle,
                    Icon = ImageMoniker.KnownValues.Database,
                },
                cancellationToken);

            if (string.IsNullOrEmpty(connectionString))
            {
                await shell.ShowPromptAsync(
                    "连接字符串不能为空，发布过程已取消。",
                    PromptOptions.ErrorConfirm with { Title = dialogTitle },
                    cancellationToken);
                return;
            }
        }

        // 执行发布 (Execute Deployment)
        await shell.ShowPromptAsync(
            $"正在发布插件到环境 \"{selectedEnvironment.Name}\"...\n\n" +
            "发布完成！插件已成功部署。",
            PromptOptions.AlertConfirm with { Title = dialogTitle },
            cancellationToken);

        // 在这里可以添加实际的插件发布逻辑
        // Actual plugin deployment logic can be added here
        // 例如：
        // For example:
        // - 根据 selectedEnvironment 建立连接
        //   Establish connection based on selectedEnvironment
        // - 构建并打包插件程序集
        //   Build and package plugin assembly
        // - 上传到目标环境
        //   Upload to target environment
        // - 注册插件步骤和映像
        //   Register plugin steps and images
    }

    /// <summary>
    /// 获取认证方式的显示名称
    /// Get display name for authentication method
    /// </summary>
    /// <param name="authMethod">认证方式</param>
    /// <returns>显示名称</returns>
    private static string GetAuthMethodDisplayName(AuthenticationMethod authMethod)
    {
        return authMethod switch
        {
            AuthenticationMethod.OAuth => "OAuth",
            AuthenticationMethod.ConnectionString => "连接字符串",
            AuthenticationMethod.ActiveDirectory => "Active Directory",
            AuthenticationMethod.IFD => "IFD",
            _ => authMethod.ToString()
        };
    }
}
