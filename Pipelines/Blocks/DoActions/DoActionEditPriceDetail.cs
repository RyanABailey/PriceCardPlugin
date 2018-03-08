using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Pricing;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Plugin.MyProject.PriceDetail
{
    [PipelineDisplayName(Constants.Pipelines.Blocks.DoActionEditPriceDetail)]
    public class DoActionEditPriceDetail : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        public DoActionEditPriceDetail(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override Task<EntityView> Run(EntityView arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull($"{Name}: The argument cannot be null.");

            var actionsPolicy = context.GetPolicy<KnownPriceDetailActionsPolicy>();

            // Only proceed if the right action was invoked
            if (string.IsNullOrEmpty(arg.Action) || !arg.Action.Equals(actionsPolicy.EditPriceDetail, StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(arg);
            }

            // Get the price card from the context
            var entity = context.CommerceContext.GetObject<Sitecore.Commerce.Plugin.Pricing.PriceCard>(x => x.Id.Equals(arg.EntityId));
            if (entity == null)
            {
                return Task.FromResult(arg);
            }

            // Get the component from the price card
            var component = entity.GetComponent<PriceDetailComponent>();

            // Map entity view properties to component
            component.PromotionCode =
               arg.Properties.FirstOrDefault(x =>
                       x.Name.Equals(nameof(PriceDetailComponent.PromotionCode), StringComparison.OrdinalIgnoreCase))?.Value;

            component.PromotionText =
               arg.Properties.FirstOrDefault(x =>
                       x.Name.Equals(nameof(PriceDetailComponent.PromotionText), StringComparison.OrdinalIgnoreCase))?.Value;

            // Persist changes
            this._commerceCommander.Pipeline<IPersistEntityPipeline>().Run(new PersistEntityArgument(entity), context);

            return Task.FromResult(arg);
        }
    }
}
