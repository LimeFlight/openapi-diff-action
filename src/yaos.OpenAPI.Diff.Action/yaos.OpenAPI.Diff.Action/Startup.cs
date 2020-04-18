using Microsoft.Extensions.DependencyInjection;
using System;
using yaos.OpenAPI.Diff.Output;

namespace yaos.OpenAPI.Diff.Action
{
    public static class Startup
    {
        public static IServiceProvider Build()
        {
            return new ServiceCollection()
                .AddSingleton<IOpenAPICompare, OpenAPICompare>()
                .AddSingleton<IMarkdownRender, MarkdownRender>()
                //.AddTransient(x => (IExtensionDiff)x.GetService(typeof(ExtensionDiff)))
                .BuildServiceProvider();
        }
    }
}
