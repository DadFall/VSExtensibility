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
            // 第一步：环境选择 (Step 1: Environment Selection)
            CrmEnvironmentType selectedEnvironment = await shell.ShowPromptAsync(
                "请选择目标环境类型：",
                new PromptOptions<CrmEnvironmentType>
                {
                    Choices =
                    {
                        { "On-Premise (本地部署)", CrmEnvironmentType.OnPremise },
                        { "Dataverse (云端环境)", CrmEnvironmentType.Dataverse },
                    },
                    DismissedReturns = CrmEnvironmentType.OnPremise,
                    Title = DialogTitle,
                    Icon = ImageMoniker.KnownValues.Settings,
                },
                cancellationToken);

            // 第二步：根据环境选择认证方式 (Step 2: Authentication Method Selection based on Environment)
            AuthenticationMethod selectedAuth;
            if (selectedEnvironment == CrmEnvironmentType.OnPremise)
            {
                selectedAuth = await shell.ShowPromptAsync(
                    "请选择 On-Premise 环境的认证方式：",
                    new PromptOptions<AuthenticationMethod>
                    {
                        Choices =
                        {
                            { "Active Directory", AuthenticationMethod.ActiveDirectory },
                            { "IFD (Internet Facing Deployment)", AuthenticationMethod.IFD },
                            { "连接字符串", AuthenticationMethod.ConnectionString },
                        },
                        DismissedReturns = AuthenticationMethod.ActiveDirectory,
                        Title = DialogTitle,
                        Icon = ImageMoniker.KnownValues.StatusSecurityWarning,
                    },
                    cancellationToken);
            }
            else // Dataverse
            {
                selectedAuth = await shell.ShowPromptAsync(
                    "请选择 Dataverse 环境的认证方式：",
                    new PromptOptions<AuthenticationMethod>
                    {
                        Choices =
                        {
                            { "OAuth", AuthenticationMethod.OAuth },
                            { "连接字符串", AuthenticationMethod.ConnectionString },
                        },
                        DismissedReturns = AuthenticationMethod.OAuth,
                        Title = DialogTitle,
                        Icon = ImageMoniker.KnownValues.StatusSecurityWarning,
                    },
                    cancellationToken);
            }

            // 第三步：获取连接详细信息 (Step 3: Get Connection Details)
            string? connectionDetails = null;
            if (selectedAuth == AuthenticationMethod.ConnectionString)
            {
                connectionDetails = await shell.ShowPromptAsync(
                    "请输入连接字符串：",
                    InputPromptOptions.Default with 
                    { 
                        Title = DialogTitle,
                        Icon = ImageMoniker.KnownValues.Database,
                    },
                    cancellationToken);

                if (string.IsNullOrEmpty(connectionDetails))
                {
                    await shell.ShowPromptAsync(
                        "连接字符串不能为空，发布过程已取消。",
                        PromptOptions.ErrorConfirm with { Title = DialogTitle },
                        cancellationToken);
                    return;
                }
            }
            else if (selectedAuth == AuthenticationMethod.OAuth && selectedEnvironment == CrmEnvironmentType.Dataverse)
            {
                connectionDetails = await shell.ShowPromptAsync(
                    "请输入 Dataverse 环境 URL（例如：https://yourorg.crm.dynamics.com）：",
                    InputPromptOptions.Default with 
                    { 
                        Title = DialogTitle,
                        Icon = ImageMoniker.KnownValues.Cloud,
                    },
                    cancellationToken);

                if (string.IsNullOrEmpty(connectionDetails))
                {
                    await shell.ShowPromptAsync(
                        "环境 URL 不能为空，发布过程已取消。",
                        PromptOptions.ErrorConfirm with { Title = DialogTitle },
                        cancellationToken);
                    return;
                }
            }

            // 第四步：确认配置 (Step 4: Confirm Configuration)
            string configSummary = $"环境类型：{(selectedEnvironment == CrmEnvironmentType.OnPremise ? "On-Premise" : "Dataverse")}\n" +
                                 $"认证方式：{GetAuthMethodDisplayName(selectedAuth)}\n" +
                                 $"连接信息：{(string.IsNullOrEmpty(connectionDetails) ? "使用默认配置" : "已配置")}";

            bool confirmDeployment = await shell.ShowPromptAsync(
                $"确认发布配置：\n\n{configSummary}\n\n确定要继续发布插件吗？",
                PromptOptions.OKCancel with
                {
                    Title = DialogTitle,
                    Icon = ImageMoniker.KnownValues.StatusInformation,
                },
                cancellationToken);

            if (!confirmDeployment)
            {
                await shell.ShowPromptAsync(
                    "插件发布已取消。",
                    PromptOptions.AlertConfirm with { Title = DialogTitle },
                    cancellationToken);
                return;
            }

            // 第五步：执行发布 (Step 5: Execute Deployment)
            await shell.ShowPromptAsync(
                $"正在发布插件到 {(selectedEnvironment == CrmEnvironmentType.OnPremise ? "On-Premise" : "Dataverse")} 环境...\n\n" +
                "发布完成！插件已成功部署。",
                PromptOptions.AlertConfirm with { Title = DialogTitle },
                cancellationToken);

            // 在这里可以添加实际的插件发布逻辑
            // Actual plugin deployment logic can be added here
            // 例如：
            // For example:
            // - 根据 selectedEnvironment 和 selectedAuth 建立连接
            //   Establish connection based on selectedEnvironment and selectedAuth
            // - 构建并打包插件程序集
            //   Build and package plugin assembly
            // - 上传到目标环境
            //   Upload to target environment
            // - 注册插件步骤和映像
            //   Register plugin steps and images
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
