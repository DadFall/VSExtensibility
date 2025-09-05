// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PluginDeployment;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

/// <summary>
/// 环境存储服务 - 负责在本地持久化环境配置
/// Environment Storage Service - Responsible for persisting environment configurations locally
/// </summary>
public class EnvironmentStorageService
{
    private static readonly string ConfigDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "VSExtensions",
        "PluginDeployment");
    
    private static readonly string ConfigFilePath = Path.Combine(ConfigDirectory, "environments.json");

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// 加载所有环境配置
    /// Load all environment configurations
    /// </summary>
    /// <returns>环境配置列表</returns>
    public async Task<List<CrmEnvironment>> LoadEnvironmentsAsync()
    {
        try
        {
            if (!File.Exists(ConfigFilePath))
            {
                // 返回默认环境配置
                return GetDefaultEnvironments();
            }

            var json = await File.ReadAllTextAsync(ConfigFilePath);
            var environments = JsonSerializer.Deserialize<List<CrmEnvironment>>(json, JsonOptions);
            
            return environments ?? GetDefaultEnvironments();
        }
        catch (Exception)
        {
            // 如果加载失败，返回默认配置
            return GetDefaultEnvironments();
        }
    }

    /// <summary>
    /// 保存所有环境配置
    /// Save all environment configurations
    /// </summary>
    /// <param name="environments">要保存的环境配置列表</param>
    public async Task SaveEnvironmentsAsync(IEnumerable<CrmEnvironment> environments)
    {
        try
        {
            // 确保配置目录存在
            Directory.CreateDirectory(ConfigDirectory);

            var json = JsonSerializer.Serialize(environments.ToList(), JsonOptions);
            await File.WriteAllTextAsync(ConfigFilePath, json);
        }
        catch (Exception ex)
        {
            // 可以在这里记录日志或显示错误消息
            throw new InvalidOperationException($"Failed to save environments: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// 添加新环境配置
    /// Add new environment configuration
    /// </summary>
    /// <param name="environment">要添加的环境</param>
    public async Task AddEnvironmentAsync(CrmEnvironment environment)
    {
        var environments = await LoadEnvironmentsAsync();
        
        // 检查是否存在同名环境
        if (environments.Any(e => e.Name.Equals(environment.Name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"Environment with name '{environment.Name}' already exists.");
        }

        environments.Add(environment);
        await SaveEnvironmentsAsync(environments);
    }

    /// <summary>
    /// 更新现有环境配置
    /// Update existing environment configuration
    /// </summary>
    /// <param name="originalName">原始环境名称</param>
    /// <param name="updatedEnvironment">更新后的环境配置</param>
    public async Task UpdateEnvironmentAsync(string originalName, CrmEnvironment updatedEnvironment)
    {
        var environments = await LoadEnvironmentsAsync();
        
        var existingIndex = environments.FindIndex(e => e.Name.Equals(originalName, StringComparison.OrdinalIgnoreCase));
        if (existingIndex == -1)
        {
            throw new InvalidOperationException($"Environment with name '{originalName}' not found.");
        }

        // 如果名称发生变化，检查新名称是否与其他环境冲突
        if (!originalName.Equals(updatedEnvironment.Name, StringComparison.OrdinalIgnoreCase))
        {
            if (environments.Any(e => e.Name.Equals(updatedEnvironment.Name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException($"Environment with name '{updatedEnvironment.Name}' already exists.");
            }
        }

        environments[existingIndex] = updatedEnvironment;
        await SaveEnvironmentsAsync(environments);
    }

    /// <summary>
    /// 删除环境配置
    /// Delete environment configuration
    /// </summary>
    /// <param name="environmentName">要删除的环境名称</param>
    public async Task DeleteEnvironmentAsync(string environmentName)
    {
        var environments = await LoadEnvironmentsAsync();
        
        var existingIndex = environments.FindIndex(e => e.Name.Equals(environmentName, StringComparison.OrdinalIgnoreCase));
        if (existingIndex == -1)
        {
            throw new InvalidOperationException($"Environment with name '{environmentName}' not found.");
        }

        environments.RemoveAt(existingIndex);
        await SaveEnvironmentsAsync(environments);
    }

    /// <summary>
    /// 获取默认环境配置
    /// Get default environment configurations
    /// </summary>
    private static List<CrmEnvironment> GetDefaultEnvironments()
    {
        return new List<CrmEnvironment>
        {
            new CrmEnvironment
            {
                Name = "XCMG-DEV-YW",
                Server = "xcmg-dev-yw.crm.dynamics.com",
                Port = 443,
                Type = CrmEnvironmentType.Dataverse,
                AuthMethod = AuthenticationMethod.OAuth,
                UseSSL = true,
                OrganizationName = "xcmgdevyw"
            },
            new CrmEnvironment
            {
                Name = "XCMG-TEST",
                Server = "xcmg-test.crm.dynamics.com",
                Port = 443,
                Type = CrmEnvironmentType.Dataverse,
                AuthMethod = AuthenticationMethod.OAuth,
                UseSSL = true,
                OrganizationName = "xcmgtest"
            },
            new CrmEnvironment
            {
                Name = "本地开发环境",
                Server = "localhost",
                Port = 5555,
                Type = CrmEnvironmentType.OnPremise,
                AuthMethod = AuthenticationMethod.ActiveDirectory,
                UseSSL = false,
                OrganizationName = "crmdev",
                Domain = "tcdev",
                Username = "administrator"
            }
        };
    }
}