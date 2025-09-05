// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PluginDeployment;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Extensibility;

/// <summary>
/// PluginDeployment 扩展的入口点类
/// Extension entry point for the PluginDeployment.
/// 
/// 这个类是 Visual Studio 扩展的主要入口点，负责：
/// This class is the main entry point for the Visual Studio extension, responsible for:
/// 1. 定义扩展的元数据信息（ID、版本、发布者等）
///    Defining extension metadata (ID, version, publisher, etc.)
/// 2. 配置扩展的服务依赖注入
///    Configuring service dependency injection for the extension
/// 3. 作为整个扩展生命周期的管理者
///    Acting as the manager for the entire extension lifecycle
/// </summary>
[VisualStudioContribution] // 标记这个类为 Visual Studio 扩展贡献者 (Marks this class as a Visual Studio extension contributor)
public class PluginDeploymentExtension : Extension
{
    /// <summary>
    /// 扩展配置属性，定义了扩展的基本信息
    /// Extension configuration property that defines basic extension information
    /// 
    /// 包含的信息：
    /// Information included:
    /// - 唯一标识符：用于在 Visual Studio 中识别此扩展
    ///   Unique identifier: Used to identify this extension in Visual Studio
    /// - 版本号：从程序集版本自动获取
    ///   Version: Automatically obtained from assembly version
    /// - 发布者名称：显示在扩展管理器中的发布者信息
    ///   Publisher name: Publisher information displayed in extension manager
    /// - 显示名称：用户看到的扩展名称
    ///   Display name: Extension name visible to users
    /// - 描述：扩展功能的简要说明
    ///   Description: Brief description of extension functionality
    /// </summary>
    /// <inheritdoc/>
    public override ExtensionConfiguration ExtensionConfiguration => new()
    {
        Metadata = new(
                id: "PluginDeployment.d40c8971-86cd-5fg7-a4f9-26c0b65d0f3c", // 扩展的唯一 GUID 标识符 (Unique GUID identifier for the extension)
                version: this.ExtensionAssemblyVersion, // 扩展版本，自动从程序集获取 (Extension version, automatically obtained from assembly)
                publisherName: "Microsoft", // 发布者名称 (Publisher name)
                displayName: "插件部署工具", // 扩展显示名称 (Extension display name)
                description: "Dynamics 365 插件程序集和步骤的发布与部署"), // 扩展描述 (Extension description)
    };

    /// <summary>
    /// 初始化服务的方法，用于配置依赖注入容器
    /// Method to initialize services, used to configure the dependency injection container
    /// 
    /// 这个方法在扩展加载时被调用，可以用来：
    /// This method is called when the extension loads and can be used to:
    /// 1. 注册自定义服务到依赖注入容器
    ///    Register custom services to the dependency injection container
    /// 2. 配置扩展所需的各种服务
    ///    Configure various services required by the extension
    /// 3. 设置扩展的初始化逻辑
    ///    Set up initialization logic for the extension
    /// 
    /// 目前这个扩展只使用基础功能，所以只调用了基类的实现
    /// Currently this extension only uses basic functionality, so only calls the base class implementation
    /// </summary>
    /// <param name="serviceCollection">依赖注入服务集合 (Dependency injection service collection)</param>
    /// <inheritdoc/>
    protected override void InitializeServices(IServiceCollection serviceCollection)
    {
        // 调用基类的服务初始化方法，确保扩展的基础服务正确设置
        // Call the base class service initialization method to ensure basic extension services are properly set up
        base.InitializeServices(serviceCollection);

        // 在这里可以添加额外的服务注册
        // Additional service registrations can be added here
        // 例如：serviceCollection.AddSingleton<IMyService, MyService>();
        // Example: serviceCollection.AddSingleton<IMyService, MyService>();
    }
}
