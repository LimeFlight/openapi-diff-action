using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using yaos.OpenAPI.Diff.Output.Markdown;

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
                .BuildServiceProvider();
        }
    }
}
