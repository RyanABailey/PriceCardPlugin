namespace Plugin.MyProject.PriceDetail
{
    using System;
    using System.Threading.Tasks;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName(Constants.Pipelines.Blocks.PopulatePriceDetailActionsBlock)]
    public class PopulatePriceDetailActionsBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        public override Task<EntityView> Run(EntityView arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull($"{Name}: The argument cannot be null.");

            var viewsPolicy = context.GetPolicy<KnownPriceDetailViewsPolicy>();

            if (string.IsNullOrEmpty(arg?.Name) || 
                !arg.Name.Equals(viewsPolicy.PriceDetail, StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(arg);
            }

            var actionPolicy = arg.GetPolicy<ActionsPolicy>();

            actionPolicy.Actions.Add(
                new EntityActionView
                {
                    Name = context.GetPolicy<KnownPriceDetailActionsPolicy>().EditPriceDetail,
                    DisplayName = "Edit Price Card data",
                    Description = "Edits the Price Card data",
                    IsEnabled = true,
                    EntityView = arg.Name,
                    Icon = "edit"
                });

            return Task.FromResult(arg);
        }
    }
}
