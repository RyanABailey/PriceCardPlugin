namespace Plugin.MyProject.PriceDetail
{
    using Sitecore.Commerce.Core;

    public class KnownPriceDetailViewsPolicy : Policy
    {
        public string PriceDetail { get; set; } = "PriceCardPriceDetail";
    }
}
