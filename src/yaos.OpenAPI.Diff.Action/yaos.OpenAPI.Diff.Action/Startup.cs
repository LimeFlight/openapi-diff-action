using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Logging;
using yaos.OpenAPI.Diff.Output;

namespace yaos.OpenAPI.Diff.Action
{
    public static class Startup
    {
        public static IServiceProvider Build()
        {
            return new ServiceCollection()
                .AddLogging(builder =>
                {
                    builder.AddFilter("", LogLevel.Trace);
                    builder.AddConsole();
                })
                .AddSingleton<IOpenAPICompare, OpenAPICompare>()
                .AddSingleton<IMarkdownRender, MarkdownRender>()
                //.AddTransient(x => (IExtensionDiff)x.GetService(typeof(ExtensionDiff)))
                .BuildServiceProvider();
        }
    }
}
