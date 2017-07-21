using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using WebMarkupMin.AspNet.Common.Compressors;
using WebMarkupMin.AspNetCore1;
using WebMarkupMin.NUglify;

namespace Apex.Websites.AppStart
{
    public static class WebMarkupMinExtensions
    {
        public static IServiceCollection AddCustomWebMarkupMin(this IServiceCollection services)
        {
            services
                .AddWebMarkupMin(opts =>
                {
                    opts.AllowMinificationInDevelopmentEnvironment = true;
                    opts.AllowCompressionInDevelopmentEnvironment = true;
                })
                .AddHtmlMinification(opts =>
                {
                    // opts.ExcludedPages = new List<IUrlMatcher>
                    // {
                    //     new WildcardUrlMatcher("/minifiers/x*ml-minifier"),
                    //     new ExactUrlMatcher("/contact")
                    // };

                    var minificationSettings = opts.MinificationSettings;
                    minificationSettings.RemoveRedundantAttributes = true;
                    minificationSettings.RemoveHttpProtocolFromAttributes = true;
                    minificationSettings.RemoveHttpsProtocolFromAttributes = true;

                    opts.CssMinifierFactory = new NUglifyCssMinifierFactory();
                    opts.JsMinifierFactory = new NUglifyJsMinifierFactory();
                })
                .AddHttpCompression(opts =>
                {
                    opts.CompressorFactories = new List<ICompressorFactory>
                    {
                        new DeflateCompressorFactory(),
                        new GZipCompressorFactory()
                    };
                });

            return services;
        }

        public static IApplicationBuilder UseCustomWebMarkupMin(this IApplicationBuilder app)
        {
            app.UseWebMarkupMin();

            return app;
        }
    }
}
