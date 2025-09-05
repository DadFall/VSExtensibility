// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PluginDeployment;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.VisualStudio.Extensibility.UI;

/// <summary>
/// 环境配置对话框控件
/// Environment Configuration Dialog Control
/// </summary>
internal class EnvironmentConfigurationDialog : RemoteUserControl
{
    private readonly EnvironmentConfigurationViewModel _viewModel;

    /// <summary>
    /// 初始化环境配置对话框的新实例
    /// Initializes a new instance of the Environment Configuration Dialog
    /// </summary>
    /// <param name="environment">要编辑的环境，为null时表示新增</param>
    public EnvironmentConfigurationDialog(CrmEnvironment? environment = null)
        : this(new EnvironmentConfigurationViewModel(environment))
    {
    }

    /// <summary>
    /// 初始化环境配置对话框的新实例，使用指定的视图模型
    /// Initializes a new instance of the Environment Configuration Dialog with specified view model
    /// </summary>
    /// <param name="viewModel">视图模型 (View Model)</param>
    /// <param name="synchronizationContext">同步上下文 (Synchronization Context)</param>
    public EnvironmentConfigurationDialog(EnvironmentConfigurationViewModel viewModel, SynchronizationContext? synchronizationContext = null)
        : base(viewModel, synchronizationContext)
    {
        _viewModel = viewModel;
        
        // 加载 XAML 资源
        // Load XAML resources
        this.ResourceDictionaries.AddEmbeddedResource("PluginDeployment.EnvironmentConfigurationDialog.xaml");
    }

    /// <summary>
    /// 获取对话框结果
    /// Get dialog result
    /// </summary>
    /// <returns>配置的环境，如果取消则返回 null (Configured environment, or null if cancelled)</returns>
    public Task<CrmEnvironment?> GetResultAsync()
    {
        return _viewModel.DialogResultTask;
    }

    /// <summary>
    /// 获取视图模型
    /// Get view model
    /// </summary>
    public EnvironmentConfigurationViewModel ViewModel => _viewModel;
}

/// <summary>
/// 环境配置对话框的视图模型
/// View Model for Environment Configuration Dialog
/// </summary>
public class EnvironmentConfigurationViewModel : INotifyPropertyChanged
{
    private readonly TaskCompletionSource<CrmEnvironment?> _dialogResult;
    private readonly bool _isEditMode;

    public EnvironmentConfigurationViewModel(CrmEnvironment? environment = null)
    {
        _dialogResult = new TaskCompletionSource<CrmEnvironment?>();
        _isEditMode = environment != null;
        
        // 创建环境副本或新环境
        Environment = environment != null ? CloneEnvironment(environment) : new CrmEnvironment
        {
            Name = "default",
            Server = "http://",
            Port = 80,
            OrganizationName = "crmdev",
            Type = CrmEnvironmentType.OnPremise,
            AuthMethod = AuthenticationMethod.ActiveDirectory,
            Domain = "tcdev",
            Username = "administrator",
            GrantType = GrantType.Password,
            UseSSL = false
        };

        // 监听环境属性变化
        Environment.PropertyChanged += (s, e) => 
        {
            OnPropertyChanged(nameof(CanSave));
            OnPropertyChanged(nameof(IsOnPremise));
            OnPropertyChanged(nameof(IsDataverse));
            OnPropertyChanged(nameof(ShowCredentialFields));
        };

        // 初始化命令
        SaveCommand = new RelayCommand(Save, () => CanSave);
        CancelCommand = new RelayCommand(Cancel);

        // 初始化认证方法列表
        AuthMethods = Enum.GetValues<AuthenticationMethod>().ToList();
    }

    /// <summary>
    /// 配置的环境 (Environment being configured)
    /// </summary>
    public CrmEnvironment Environment { get; }

    /// <summary>
    /// 对话框标题 (Dialog Title)
    /// </summary>
    public string Title => _isEditMode ? "编辑环境配置" : "新增环境配置";

    /// <summary>
    /// 保存按钮文本 (Save Button Text)
    /// </summary>
    public string SaveButtonText => _isEditMode ? "保存" : "添加";

    /// <summary>
    /// 是否为本地部署 (Is On-Premise)
    /// </summary>
    public bool IsOnPremise
    {
        get => Environment.Type == CrmEnvironmentType.OnPremise;
        set
        {
            if (value)
            {
                Environment.Type = CrmEnvironmentType.OnPremise;
                Environment.AuthMethod = AuthenticationMethod.ActiveDirectory;
                Environment.Port = 80;
                Environment.UseSSL = false;
            }
        }
    }

    /// <summary>
    /// 是否为 Dataverse (Is Dataverse)
    /// </summary>
    public bool IsDataverse
    {
        get => Environment.Type == CrmEnvironmentType.Dataverse;
        set
        {
            if (value)
            {
                Environment.Type = CrmEnvironmentType.Dataverse;
                Environment.AuthMethod = AuthenticationMethod.OAuth;
                Environment.Port = 443;
                Environment.UseSSL = true;
            }
        }
    }

    /// <summary>
    /// 是否显示凭据字段 (Whether to show credential fields)
    /// </summary>
    public bool ShowCredentialFields => !Environment.SignInAsCurrentUser;

    /// <summary>
    /// 是否可以保存 (Whether can save)
    /// </summary>
    public bool CanSave => 
        !string.IsNullOrWhiteSpace(Environment.Name) &&
        !string.IsNullOrWhiteSpace(Environment.Server) &&
        !string.IsNullOrWhiteSpace(Environment.OrganizationName);

    /// <summary>
    /// 认证方法列表 (Authentication Methods List)
    /// </summary>
    public List<AuthenticationMethod> AuthMethods { get; }

    /// <summary>
    /// 对话框结果任务 (Dialog Result Task)
    /// </summary>
    public Task<CrmEnvironment?> DialogResultTask => _dialogResult.Task;

    // 命令 (Commands)
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    private void Save()
    {
        _dialogResult.SetResult(Environment);
    }

    private void Cancel()
    {
        _dialogResult.SetResult(null);
    }

    private static CrmEnvironment CloneEnvironment(CrmEnvironment source)
    {
        return new CrmEnvironment
        {
            Name = source.Name,
            Server = source.Server,
            Port = source.Port,
            OrganizationName = source.OrganizationName,
            Type = source.Type,
            AuthMethod = source.AuthMethod,
            Domain = source.Domain,
            Username = source.Username,
            Password = source.Password,
            UseSSL = source.UseSSL,
            SignInAsCurrentUser = source.SignInAsCurrentUser,
            DisplayOrganizationList = source.DisplayOrganizationList,
            ClientId = source.ClientId,
            ClientSecret = source.ClientSecret,
            AppId = source.AppId,
            GrantType = source.GrantType
        };
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}