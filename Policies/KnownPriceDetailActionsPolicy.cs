namespace Plugin.MyProject.PriceDetail
{
    using Sitecore.Commerce.Core;

    public class KnownPriceDetailActionsPolicy : Policy
    {
        public string EditPriceDetail { get; set; } = nameof(EditPriceDetail);
    }
}