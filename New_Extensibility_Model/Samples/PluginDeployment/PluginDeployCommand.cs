// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PluginDeployment;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;
using Microsoft.VisualStudio.Extensibility.Shell;

[VisualStudioContribution]
internal class PluginDeployCommand : Command
{
    /// <inheritdoc />
    public override CommandConfiguration CommandConfiguration => new("%PluginDeployment.PluginDeployCommand.DisplayName%")
    {
        Placements =
        [
            // CommandPlacement.VsctParent(new Guid("{d309f791-903f-11d0-9efc-00a0c911004f}"), id: 537, priority: 0), // Solution context menu (解决方案的右键菜单)
            CommandPlacement.VsctParent(new Guid("{d309f791-903f-11d0-9efc-00a0c911004f}"), id: 518, priority: 0), // Project context menu  (项目的右键菜单)
            // CommandPlacement.VsctParent(new Guid("{d309f791-903f-11d0-9efc-00a0c911004f}"), id: 521, priority: 0), // File in project context menu   (文件的右键菜单)
        ],
    };

    /// <inheritdoc />
    public override async Task ExecuteCommandAsync(IClientContext context, CancellationToken cancellationToken)
    {
        // 显示提示"插件发布中" (Show message "Plugin is being released")
        await this.Extensibility.Shell().ShowPromptAsync(
            "%PluginDeployment.PluginDeployCommand.ProgressMessage%",
            PromptOptions.OK,
            cancellationToken);
    }
}
