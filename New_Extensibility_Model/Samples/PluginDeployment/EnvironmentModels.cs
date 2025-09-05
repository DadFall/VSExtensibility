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
    private string _url = string.Empty;
    private CrmEnvironmentType _type;
    private AuthenticationMethod _authMethod;

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
    /// 环境 URL (Environment URL)
    /// </summary>
    public string Url
    {
        get => _url;
        set
        {
            if (_url != value)
            {
                _url = value;
                OnPropertyChanged(nameof(Url));
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

    public EnvironmentSelectionViewModel()
    {
        _dialogResult = new TaskCompletionSource<CrmEnvironment?>();
        
        // 初始化示例环境数据
        // Initialize sample environment data
        Environments = new ObservableCollection<CrmEnvironment>
        {
            new CrmEnvironment
            {
                Name = "XCMG-DEV-YW",
                Url = "https://xcmg-dev-yw.crm.dynamics.com",
                Type = CrmEnvironmentType.Dataverse,
                AuthMethod = AuthenticationMethod.OAuth
            },
            new CrmEnvironment
            {
                Name = "XCMG-TEST",
                Url = "https://xcmg-test.crm.dynamics.com",
                Type = CrmEnvironmentType.Dataverse,
                AuthMethod = AuthenticationMethod.OAuth
            },
            new CrmEnvironment
            {
                Name = "本地开发环境",
                Url = "http://localhost:5555",
                Type = CrmEnvironmentType.OnPremise,
                AuthMethod = AuthenticationMethod.ActiveDirectory
            }
        };

        // 默认选择第一个环境
        // Select first environment by default
        SelectedEnvironment = Environments.FirstOrDefault();

        // 初始化命令
        // Initialize commands
        AddEnvironmentCommand = new AsyncRelayCommand(AddEnvironmentAsync);
        EditEnvironmentCommand = new AsyncRelayCommand(EditEnvironmentAsync, () => HasSelectedEnvironment);
        DeleteEnvironmentCommand = new AsyncRelayCommand(DeleteEnvironmentAsync, () => HasSelectedEnvironment);
        ConfirmCommand = new RelayCommand(Confirm, () => HasSelectedEnvironment);
        CancelCommand = new RelayCommand(Cancel);
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

    private async Task AddEnvironmentAsync()
    {
        // TODO: 实现添加环境对话框
        // TODO: Implement add environment dialog
        var newEnvironment = new CrmEnvironment
        {
            Name = $"新环境 {Environments.Count + 1}",
            Url = "https://example.crm.dynamics.com",
            Type = CrmEnvironmentType.Dataverse,
            AuthMethod = AuthenticationMethod.OAuth
        };

        Environments.Add(newEnvironment);
        SelectedEnvironment = newEnvironment;
    }

    private async Task EditEnvironmentAsync()
    {
        if (SelectedEnvironment == null) return;

        // TODO: 实现编辑环境对话框
        // TODO: Implement edit environment dialog
        
        // 临时实现：修改名称
        // Temporary implementation: modify name
        SelectedEnvironment.Name += " (已编辑)";
    }

    private async Task DeleteEnvironmentAsync()
    {
        if (SelectedEnvironment == null) return;

        var environmentToDelete = SelectedEnvironment;
        var index = Environments.IndexOf(environmentToDelete);
        
        Environments.Remove(environmentToDelete);
        
        // 选择下一个环境
        // Select next environment
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