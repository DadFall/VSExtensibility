// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PluginDeployment;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Extensibility.UI;

/// <summary>
/// 环境选择对话框控件
/// Environment Selection Dialog Control
/// </summary>
internal class EnvironmentSelectionDialog : RemoteUserControl
{
    /// <summary>
    /// 初始化环境选择对话框的新实例
    /// Initializes a new instance of the Environment Selection Dialog
    /// </summary>
    /// <param name="dataContext">
    /// Data context of the remote control which can be referenced from xaml through data binding.
    /// </param>
    /// <param name="synchronizationContext">
    /// Optional synchronizationContext that the extender can provide to ensure that <see cref="IAsyncCommand"/>
    /// are executed and properties are read and updated from the extension main thread.
    /// </param>
    public EnvironmentSelectionDialog(object? dataContext = null, SynchronizationContext? synchronizationContext = null)
        : base(dataContext ?? new SimpleDialogData(), synchronizationContext)
    {
    }

    /// <summary>
    /// 获取对话框结果
    /// Get dialog result
    /// </summary>
    /// <returns>选中的环境，如果取消则返回 null (Selected environment, or null if cancelled)</returns>
    public Task<CrmEnvironment?> GetResultAsync()
    {
        // 暂时返回一个示例环境
        // Temporarily return a sample environment
        var sampleEnvironment = new CrmEnvironment 
        { 
            Name = "XCMG-DEV-YW", 
            Type = CrmEnvironmentType.Dataverse,
            Server = "xcmgdev.crm.dynamics.com",
            Port = 443,
            UseSSL = true
        };
        
        return Task.FromResult<CrmEnvironment?>(sampleEnvironment);
    }
}

/// <summary>
/// 简单的对话框数据类（模仿 ModalDialogData 的模式）
/// Simple dialog data class (mimicking ModalDialogData pattern)
/// </summary>
public class SimpleDialogData
{
    public string Title { get; } = "选择部署环境";
    public string[] EnvironmentOptions { get; } = 
    {
        "XCMG-DEV-YW (Dataverse)",
        "XCMG-TEST (Dataverse)", 
        "本地开发环境 (On-Premise)"
    };
}