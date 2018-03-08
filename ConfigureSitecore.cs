namespace Plugin.MyProject.PriceDetail
{
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Pricing;
    using Sitecore.Framework.Configuration;
    using Sitecore.Framework.Pipelines.Definitions.Extensions;

    /// <summary>
    /// The configure sitecore class.
    /// </summary>
    public class ConfigureSitecore : IConfigureSitecore
    {
        /// <summary>
        /// The configure services.
        /// </summary>
        /// <param name="services">
        /// The services.
        /// </param>
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.RegisterAllPipelineBlocks(assembly);

            services.Sitecore().Pipelines(config =>
                config
                    .ConfigurePipeline<IGetEntityViewPipeline>(c =>
                    {
                        c.Add<GetPriceDetailViewBlock>().After<GetPriceCardDetailsViewBlock>();
                    })
                    .ConfigurePipeline<IPopulateEntityViewActionsPipeline>(c =>
                    {
                        c.Add<PopulatePriceDetailActionsBlock>().After<InitializeEntityViewActionsBlock>();
                    })
                    .ConfigurePipeline<IDoActionPipeline>(c =>
                    {
                        c.Add<DoActionEditPriceDetail>().After<ValidateEntityVersionBlock>();
                    })
            );
        }
    }
}