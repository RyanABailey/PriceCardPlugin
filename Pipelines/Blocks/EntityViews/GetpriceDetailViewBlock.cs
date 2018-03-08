namespace Plugin.MyProject.PriceDetail
{
    using System;
    using System.Threading.Tasks;
    using Plugin.MyProject.PriceDetail;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Pricing;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName(Constants.Pipelines.Blocks.GetPriceDetailViewBlock)]
    public class GetPriceDetailViewBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly ViewCommander _viewCommander;

        public GetPriceDetailViewBlock(ViewCommander viewCommander)
        {
            this._viewCommander = viewCommander;
        }

        public override Task<EntityView> Run(EntityView arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull($"{Name}: The argument cannot be null.");

            var request = this._viewCommander.CurrentEntityViewArgument(context.CommerceContext);

            var pricingViewsPolicy = context.GetPolicy<KnownPricingViewsPolicy>();

            var viewsPolicy = context.GetPolicy<KnownPriceDetailViewsPolicy>();
            var actionsPolicy = context.GetPolicy<KnownPriceDetailActionsPolicy>();

            // Make sure that we target the correct views
            if (string.IsNullOrEmpty(request.ViewName) ||
                !request.ViewName.Equals(pricingViewsPolicy.Master, StringComparison.OrdinalIgnoreCase) &&
                !request.ViewName.Equals(pricingViewsPolicy.Details, StringComparison.OrdinalIgnoreCase) &&
                !request.ViewName.Equals(viewsPolicy.PriceDetail, StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(arg);
            }

            // Only proceed if the current entity is a price card
            if (!(request.Entity is Sitecore.Commerce.Plugin.Pricing.PriceCard))
            {
                return Task.FromResult(arg);
            }

            var priceCard = (Sitecore.Commerce.Plugin.Pricing.PriceCard) request.Entity;

            var targetView = arg;

            // Check if the edit action was requested
            var isEditView = !string.IsNullOrEmpty(arg.Action) && arg.Action.Equals(actionsPolicy.EditPriceDetail, StringComparison.OrdinalIgnoreCase);
            if (!isEditView)
            {
                // Create a new view and add it to the current entity view.
                var view = new EntityView
                {
                    Name = context.GetPolicy<KnownPriceDetailViewsPolicy>().PriceDetail,
                    DisplayName = "Pricing Details",
                    EntityId = arg.EntityId
                };

                arg.ChildViews.Add(view);

                targetView = view;
            }

            if (priceCard != null && (priceCard.HasComponent<PriceDetailComponent>() || isEditView))
            {
                var component = priceCard.GetComponent<PriceDetailComponent>();
                AddPropertiesToView(targetView, component, !isEditView);
            }

            return Task.FromResult(arg);
        }

        private void AddPropertiesToView(EntityView entityView, PriceDetailComponent component, bool isReadOnly)
        {
            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = nameof(PriceDetailComponent.PromotionCode),
                    RawValue = component.PromotionCode,
                    IsReadOnly = isReadOnly,
                    IsRequired = false
                });

            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = nameof(PriceDetailComponent.PromotionText),
                    RawValue = component.PromotionText,
                    IsReadOnly = isReadOnly,
                    IsRequired = false
                });
        }
    }
}
