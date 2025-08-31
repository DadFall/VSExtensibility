// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PluginDeployment;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Extensibility;

/// <summary>
/// Extension entry point for the PluginDeployment.
/// </summary>
[VisualStudioContribution]
public class PluginDeploymentExtension : Extension
{
    /// <inheritdoc/>
    public override ExtensionConfiguration ExtensionConfiguration => new()
    {
        Metadata = new(
                id: "PluginDeployment.d40c8971-86cd-5fg7-a4f9-26c0b65d0f3c",
                version: this.ExtensionAssemblyVersion,
                publisherName: "Microsoft",
                displayName: "Plugin Deployment Extension",
                description: "Extension for deploying plugins from project items"),
    };

    /// <inheritdoc/>
    protected override void InitializeServices(IServiceCollection serviceCollection)
    {
        base.InitializeServices(serviceCollection);
    }
}
