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
    private readonly EnvironmentSelectionViewModel _viewModel;

    /// <summary>
    /// 初始化环境选择对话框的新实例
    /// Initializes a new instance of the Environment Selection Dialog
    /// </summary>
    public EnvironmentSelectionDialog()
        : this(new EnvironmentSelectionViewModel())
    {
    }

    /// <summary>
    /// 初始化环境选择对话框的新实例，使用指定的视图模型
    /// Initializes a new instance of the Environment Selection Dialog with specified view model
    /// </summary>
    /// <param name="viewModel">视图模型 (View Model)</param>
    /// <param name="synchronizationContext">同步上下文 (Synchronization Context)</param>
    public EnvironmentSelectionDialog(EnvironmentSelectionViewModel viewModel, SynchronizationContext? synchronizationContext = null)
        : base(viewModel, synchronizationContext)
    {
        _viewModel = viewModel;
        
        // 加载 XAML 资源
        // Load XAML resources
        this.ResourceDictionaries.AddEmbeddedResource("PluginDeployment.EnvironmentSelectionDialog.xaml");
    }

    /// <summary>
    /// 获取对话框结果
    /// Get dialog result
    /// </summary>
    /// <returns>选中的环境，如果取消则返回 null (Selected environment, or null if cancelled)</returns>
    public Task<CrmEnvironment?> GetResultAsync()
    {
        return _viewModel.DialogResultTask;
    }

    /// <summary>
    /// 获取视图模型
    /// Get view model
    /// </summary>
    public EnvironmentSelectionViewModel ViewModel => _viewModel;
}