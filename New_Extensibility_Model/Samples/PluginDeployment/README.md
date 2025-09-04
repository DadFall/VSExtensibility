# Dynamics CRM 插件发布工具（PluginDeployment）

基于 Visual Studio 新扩展性模型（Non‑VSIX）的示例扩展，目标是简化 Dynamics 365/Dataverse 与 On‑Premise 环境中的插件程序集发布与部署。当前版本已在项目节点添加“插件发布”入口，后续将逐步完善环境选择与自动发布流程。

## 当前进展
- 已实现
  - 解决方案资源管理器项目节点右键菜单：显示“插件发布”命令
  - 异步执行与状态提示（示例提示“插件发布中”）
  - 中文本地化（string-resources.json）
- 进行中 / 计划中
  - 环境选择对话框与多环境配置（On‑Premise 与 Dataverse）
  - 认证方式：OAuth、连接字符串、AD、IFD
  - 发布管道：自动构建、上传插件程序集，解析并同步 `[CrmPluginRegistration]` 步骤与映像
  - 发布选项：目标解决方案唯一名、隔离模式、源类型等
  - 日志与重试机制

## 安装与运行
- 在 Visual Studio 2022 中打开解决方案
- 构建 PluginDeployment 项目，按 F5 启动调试
- 扩展将自动部署到 Visual Studio 实验实例（基于新扩展性模型，非传统 VSIX 安装）

## 使用
1. 在解决方案资源管理器中右键任意项目
2. 选择“插件发布”
3. 当前版本会显示“插件发布中”提示；后续版本将弹出环境选择并执行发布

## 技术概要
- 目标框架：.NET 8.0 for Windows（net8.0-windows8.0）
- 语言版本：C# 12；启用隐式 using 与可空引用类型
- 依赖：
  ```xml
  <PackageReference Include="Microsoft.VisualStudio.Extensibility.Sdk" Version="17.14.40254" />
  <PackageReference Include="Microsoft.VisualStudio.Extensibility.Build" Version="17.14.40254" />
  ```
- 菜单位置（VSCT）：
  ```csharp
  CommandPlacement.VsctParent(new Guid("{d309f791-903f-11d0-9efc-00a0c911004f}"), id: 518, priority: 0)
  ```
  - 518：项目节点右键菜单；常见 ID：537（解决方案），521（项目文件）
- 本地化键：
  ```json
  {
    "PluginDeployment.PluginDeployCommand.DisplayName": "插件发布",
    "PluginDeployment.PluginDeployCommand.ProgressMessage": "插件发布中"
  }
  ```

## 项目结构
```
PluginDeployment/
├─ .vsextension/
│  └─ string-resources.json
├─ ExtensionCommandConfiguration.cs
├─ PluginDeployCommand.cs
├─ PluginDeployment.csproj
├─ PluginDeploymentExtension.cs
└─ README.md
```

## 兼容性
- Visual Studio 2022 及更高版本
- Windows 8.0 及更高版本
- 新扩展性模型（Non‑VSIX）

## 贡献
1. **Fork 本项目**：从 GitHub 上 fork 本项目并进行修改。
2. **提交 PR**：提交 pull request 以贡献代码或修复 bug。
3. **报告问题**：在 Issues 页面提交 bug 或建议。

## 联系方式

如果你有任何问题或建议，请通过以下方式联系：

- **邮件**：[99472773+DadFall@users.noreply.github.com]
- **GitHub**：[https://github.com/DadFall/PluginPublisher](https://github.com/DadFall/PluginPublisher)

## 许可证

此项目使用 MIT 许可证。详情请参阅 LICENSE 文件。