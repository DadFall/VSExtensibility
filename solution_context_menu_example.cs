// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;

namespace SolutionContextMenuExample;

/// <summary>
/// Example command that demonstrates how to add a menu item to the solution's right-click context menu.
/// 演示如何在解决方案右键上下文菜单中添加菜单项的示例命令。
/// </summary>
[VisualStudioContribution]
internal class SolutionContextMenuCommand : Command
{
    /// <inheritdoc />
    public override CommandConfiguration CommandConfiguration => new("My Solution Command")
    {
        Placements =
        [
            // Add command to solution context menu (解决方案上下文菜单)
            // GUID: {d309f791-903f-11d0-9efc-00a0c911004f} - Standard Visual Studio command groups
            // ID: 537 - Solution context menu identifier
            CommandPlacement.VsctParent(new Guid("{d309f791-903f-11d0-9efc-00a0c911004f}"), id: 537, priority: 0),
        ],
    };

    /// <inheritdoc />
    public override async Task ExecuteCommandAsync(IClientContext context, CancellationToken cancellationToken)
    {
        // Show a simple message when the command is executed
        // 当命令执行时显示一个简单的消息
        await this.Extensibility.Shell().ShowPromptAsync(
            "Hello from Solution Context Menu! 来自解决方案上下文菜单的问候！", 
            PromptOptions.OK, 
            cancellationToken);
    }
}