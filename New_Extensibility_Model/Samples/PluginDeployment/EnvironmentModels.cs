// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PluginDeployment;

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.VisualStudio.Extensibility.UI;

/// <summary>
/// CRM 环境信息模型类
/// CRM Environment Information Model Class
/// </summary>
public class CrmEnvironment : INotifyPropertyChanged
{
    private string _name = string.Empty;
    private string _server = string.Empty;
    private int _port = 443;
    private string _organizationName = string.Empty;
    private CrmEnvironmentType _type = CrmEnvironmentType.OnPremise;
    private AuthenticationMethod _authMethod = AuthenticationMethod.ActiveDirectory;
    private string _domain = string.Empty;
    private string _username = string.Empty;
    private string _password = string.Empty;
    private bool _useSSL = true;
    private bool _signInAsCurrentUser = false;
    private bool _displayOrganizationList = false;
    private string _clientId = string.Empty;
    private string _clientSecret = string.Empty;
    private string _appId = string.Empty;
    private GrantType _grantType = GrantType.Password;

    /// <summary>
    /// 环境名称 (Environment Name)
    /// </summary>
    public string Name
    {
        get => _name;
        set
        {
            if (_name != value)
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
    }

    /// <summary>
    /// 服务器地址 (Server Address)
    /// </summary>
    public string Server
    {
        get => _server;
        set
        {
            if (_server != value)
            {
                _server = value;
                OnPropertyChanged(nameof(Server));
                OnPropertyChanged(nameof(Url));
            }
        }
    }

    /// <summary>
    /// 端口 (Port)
    /// </summary>
    public int Port
    {
        get => _port;
        set
        {
            if (_port != value)
            {
                _port = value;
                OnPropertyChanged(nameof(Port));
                OnPropertyChanged(nameof(Url));
            }
        }
    }

    /// <summary>
    /// 组织名称 (Organization Name)
    /// </summary>
    public string OrganizationName
    {
        get => _organizationName;
        set
        {
            if (_organizationName != value)
            {
                _organizationName = value;
                OnPropertyChanged(nameof(OrganizationName));
            }
        }
    }

    /// <summary>
    /// 环境类型 (Environment Type)
    /// </summary>
    public CrmEnvironmentType Type
    {
        get => _type;
        set
        {
            if (_type != value)
            {
                _type = value;
                OnPropertyChanged(nameof(Type));
                OnPropertyChanged(nameof(TypeDisplay));
                OnPropertyChanged(nameof(Url));
            }
        }
    }

    /// <summary>
    /// 认证方式 (Authentication Method)
    /// </summary>
    public AuthenticationMethod AuthMethod
    {
        get => _authMethod;
        set
        {
            if (_authMethod != value)
            {
                _authMethod = value;
                OnPropertyChanged(nameof(AuthMethod));
            }
        }
    }

    /// <summary>
    /// 域 (Domain)
    /// </summary>
    public string Domain
    {
        get => _domain;
        set
        {
            if (_domain != value)
            {
                _domain = value;
                OnPropertyChanged(nameof(Domain));
            }
        }
    }

    /// <summary>
    /// 用户名 (Username)
    /// </summary>
    public string Username
    {
        get => _username;
        set
        {
            if (_username != value)
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }
    }

    /// <summary>
    /// 密码 (Password)
    /// </summary>
    public string Password
    {
        get => _password;
        set
        {
            if (_password != value)
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }
    }

    /// <summary>
    /// 使用 SSL (Use SSL)
    /// </summary>
    public bool UseSSL
    {
        get => _useSSL;
        set
        {
            if (_useSSL != value)
            {
                _useSSL = value;
                OnPropertyChanged(nameof(UseSSL));
                OnPropertyChanged(nameof(Url));
            }
        }
    }

    /// <summary>
    /// 以当前用户身份登录 (Sign in as current user)
    /// </summary>
    public bool SignInAsCurrentUser
    {
        get => _signInAsCurrentUser;
        set
        {
            if (_signInAsCurrentUser != value)
            {
                _signInAsCurrentUser = value;
                OnPropertyChanged(nameof(SignInAsCurrentUser));
            }
        }
    }

    /// <summary>
    /// 显示可用组织列表 (Display list of available organizations)
    /// </summary>
    public bool DisplayOrganizationList
    {
        get => _displayOrganizationList;
        set
        {
            if (_displayOrganizationList != value)
            {
                _displayOrganizationList = value;
                OnPropertyChanged(nameof(DisplayOrganizationList));
            }
        }
    }

    /// <summary>
    /// 客户端 ID (Client ID)
    /// </summary>
    public string ClientId
    {
        get => _clientId;
        set
        {
            if (_clientId != value)
            {
                _clientId = value;
                OnPropertyChanged(nameof(ClientId));
            }
        }
    }

    /// <summary>
    /// 客户端密钥 (Client Secret)
    /// </summary>
    public string ClientSecret
    {
        get => _clientSecret;
        set
        {
            if (_clientSecret != value)
            {
                _clientSecret = value;
                OnPropertyChanged(nameof(ClientSecret));
            }
        }
    }

    /// <summary>
    /// 应用程序 ID (Application ID)
    /// </summary>
    public string AppId
    {
        get => _appId;
        set
        {
            if (_appId != value)
            {
                _appId = value;
                OnPropertyChanged(nameof(AppId));
            }
        }
    }

    /// <summary>
    /// 授权类型 (Grant Type)
    /// </summary>
    public GrantType GrantType
    {
        get => _grantType;
        set
        {
            if (_grantType != value)
            {
                _grantType = value;
                OnPropertyChanged(nameof(GrantType));
            }
        }
    }

    /// <summary>
    /// 环境 URL (Environment URL) - 根据配置动态生成
    /// </summary>
    public string Url
    {
        get
        {
            if (string.IsNullOrEmpty(Server))
                return string.Empty;

            var protocol = UseSSL ? "https" : "http";
            var defaultPort = UseSSL ? 443 : 80;
            
            if (Type == CrmEnvironmentType.Dataverse)
            {
                // For Dataverse, typically the server is the full URL
                return Server.StartsWith("http") ? Server : $"https://{Server}";
            }
            else
            {
                // For On-Premise, construct the URL
                var portPart = Port != defaultPort ? $":{Port}" : string.Empty;
                return $"{protocol}://{Server}{portPart}";
            }
        }
    }

    /// <summary>
    /// 环境类型显示名称 (Environment Type Display Name)
    /// </summary>
    public string TypeDisplay => Type == CrmEnvironmentType.OnPremise ? "On-Premise" : "Dataverse";

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

/// <summary>
/// 环境选择对话框的视图模型
/// View Model for Environment Selection Dialog
/// </summary>
public class EnvironmentSelectionViewModel : INotifyPropertyChanged
{
    private CrmEnvironment? _selectedEnvironment;
    private readonly TaskCompletionSource<CrmEnvironment?> _dialogResult;
    private readonly EnvironmentStorageService _storageService;

    public EnvironmentSelectionViewModel()
    {
        _dialogResult = new TaskCompletionSource<CrmEnvironment?>();
        _storageService = new EnvironmentStorageService();
        
        // 初始化环境列表
        Environments = new ObservableCollection<CrmEnvironment>();
        
        // 初始化命令
        AddEnvironmentCommand = new AsyncRelayCommand(AddEnvironmentAsync);
        EditEnvironmentCommand = new AsyncRelayCommand(EditEnvironmentAsync, () => HasSelectedEnvironment);
        DeleteEnvironmentCommand = new AsyncRelayCommand(DeleteEnvironmentAsync, () => HasSelectedEnvironment);
        ConfirmCommand = new RelayCommand(Confirm, () => HasSelectedEnvironment);
        CancelCommand = new RelayCommand(Cancel);

        // 异步加载环境数据
        _ = LoadEnvironmentsAsync();
    }

    /// <summary>
    /// 环境列表 (Environment List)
    /// </summary>
    public ObservableCollection<CrmEnvironment> Environments { get; }

    /// <summary>
    /// 选中的环境 (Selected Environment)
    /// </summary>
    public CrmEnvironment? SelectedEnvironment
    {
        get => _selectedEnvironment;
        set
        {
            if (_selectedEnvironment != value)
            {
                _selectedEnvironment = value;
                OnPropertyChanged(nameof(SelectedEnvironment));
                OnPropertyChanged(nameof(HasSelectedEnvironment));
                
                // 更新命令状态
                // Update command states
                ((AsyncRelayCommand)EditEnvironmentCommand).RaiseCanExecuteChanged();
                ((AsyncRelayCommand)DeleteEnvironmentCommand).RaiseCanExecuteChanged();
                ((RelayCommand)ConfirmCommand).RaiseCanExecuteChanged();
            }
        }
    }

    /// <summary>
    /// 是否有选中的环境 (Whether there is a selected environment)
    /// </summary>
    public bool HasSelectedEnvironment => SelectedEnvironment != null;

    /// <summary>
    /// 对话框结果任务 (Dialog Result Task)
    /// </summary>
    public Task<CrmEnvironment?> DialogResultTask => _dialogResult.Task;

    // 命令 (Commands)
    public ICommand AddEnvironmentCommand { get; }
    public ICommand EditEnvironmentCommand { get; }
    public ICommand DeleteEnvironmentCommand { get; }
    public ICommand ConfirmCommand { get; }
    public ICommand CancelCommand { get; }

    /// <summary>
    /// 加载环境数据
    /// Load environment data
    /// </summary>
    private async Task LoadEnvironmentsAsync()
    {
        try
        {
            var environments = await _storageService.LoadEnvironmentsAsync();
            
            Environments.Clear();
            foreach (var environment in environments)
            {
                Environments.Add(environment);
            }

            // 默认选择第一个环境
            SelectedEnvironment = Environments.FirstOrDefault();
        }
        catch (Exception)
        {
            // 加载失败时可以显示错误消息或使用默认数据
            // Could show error message or use default data when loading fails
        }
    }

    private async Task AddEnvironmentAsync()
    {
        try
        {
            // 创建环境配置对话框
            var configDialog = new EnvironmentConfigurationDialog();
            var result = await configDialog.GetResultAsync();
            
            if (result != null)
            {
                // 保存到存储
                await _storageService.AddEnvironmentAsync(result);
                
                // 添加到列表并选中
                Environments.Add(result);
                SelectedEnvironment = result;
            }
        }
        catch (Exception ex)
        {
            // TODO: 显示错误消息
            // TODO: Show error message
            System.Diagnostics.Debug.WriteLine($"Failed to add environment: {ex.Message}");
        }
    }

    private async Task EditEnvironmentAsync()
    {
        if (SelectedEnvironment == null) return;

        try
        {
            var originalName = SelectedEnvironment.Name;
            
            // 创建环境配置对话框，传入当前环境进行编辑
            var configDialog = new EnvironmentConfigurationDialog(SelectedEnvironment);
            var result = await configDialog.GetResultAsync();
            
            if (result != null)
            {
                // 更新存储
                await _storageService.UpdateEnvironmentAsync(originalName, result);
                
                // 更新列表中的环境
                var index = Environments.IndexOf(SelectedEnvironment);
                if (index >= 0)
                {
                    Environments[index] = result;
                    SelectedEnvironment = result;
                }
            }
        }
        catch (Exception ex)
        {
            // TODO: 显示错误消息
            // TODO: Show error message
            System.Diagnostics.Debug.WriteLine($"Failed to edit environment: {ex.Message}");
        }
    }

    private async Task DeleteEnvironmentAsync()
    {
        if (SelectedEnvironment == null) return;

        try
        {
            var environmentToDelete = SelectedEnvironment;
            var index = Environments.IndexOf(environmentToDelete);
            
            // 从存储中删除
            await _storageService.DeleteEnvironmentAsync(environmentToDelete.Name);
            
            // 从列表中移除
            Environments.Remove(environmentToDelete);
            
            // 选择下一个环境
            if (Environments.Count > 0)
            {
                var newIndex = Math.Min(index, Environments.Count - 1);
                SelectedEnvironment = Environments[newIndex];
            }
            else
            {
                SelectedEnvironment = null;
            }
        }
        catch (Exception ex)
        {
            // TODO: 显示错误消息
            // TODO: Show error message
            System.Diagnostics.Debug.WriteLine($"Failed to delete environment: {ex.Message}");
        }
    }

    private void Confirm()
    {
        _dialogResult.SetResult(SelectedEnvironment);
    }

    private void Cancel()
    {
        _dialogResult.SetResult(null);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

/// <summary>
/// 简单的异步中继命令实现
/// Simple Async Relay Command Implementation
/// </summary>
public class AsyncRelayCommand : ICommand
{
    private readonly Func<Task> _execute;
    private readonly Func<bool>? _canExecute;
    private bool _isExecuting;

    public AsyncRelayCommand(Func<Task> execute, Func<bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter)
    {
        return !_isExecuting && (_canExecute?.Invoke() ?? true);
    }

    public async void Execute(object? parameter)
    {
        if (!CanExecute(parameter)) return;

        _isExecuting = true;
        RaiseCanExecuteChanged();

        try
        {
            await _execute();
        }
        finally
        {
            _isExecuting = false;
            RaiseCanExecuteChanged();
        }
    }

    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}

/// <summary>
/// 简单的中继命令实现
/// Simple Relay Command Implementation
/// </summary>
public class RelayCommand : ICommand
{
    private readonly Action _execute;
    private readonly Func<bool>? _canExecute;

    public RelayCommand(Action execute, Func<bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter)
    {
        return _canExecute?.Invoke() ?? true;
    }

    public void Execute(object? parameter)
    {
        if (CanExecute(parameter))
        {
            _execute();
        }
    }

    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}

/// <summary>
/// CRM 环境类型枚举
/// CRM Environment Type Enumeration
/// </summary>
public enum CrmEnvironmentType
{
    /// <summary>
    /// 本地部署 (On-Premise)
    /// </summary>
    OnPremise,

    /// <summary>
    /// Dataverse (云服务)
    /// </summary>
    Dataverse
}

/// <summary>
/// 认证方式枚举
/// Authentication Method Enumeration
/// </summary>
public enum AuthenticationMethod
{
    /// <summary>
    /// Active Directory 认证
    /// </summary>
    ActiveDirectory,

    /// <summary>
    /// OAuth 认证
    /// </summary>
    OAuth,

    /// <summary>
    /// 密码认证
    /// </summary>
    Password,

    /// <summary>
    /// 客户端凭据
    /// </summary>
    ClientCredentials
}

/// <summary>
/// OAuth 授权类型枚举
/// OAuth Grant Type Enumeration
/// </summary>
public enum GrantType
{
    /// <summary>
    /// 密码授权
    /// </summary>
    Password,

    /// <summary>
    /// 客户端凭据授权
    /// </summary>
    ClientCredentials,

    /// <summary>
    /// 授权码授权
    /// </summary>
    AuthorizationCode
}