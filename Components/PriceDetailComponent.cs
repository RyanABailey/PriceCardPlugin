namespace Plugin.MyProject.PriceDetail
{
    using Sitecore.Commerce.Core;

    public class PriceDetailComponent : Component
    {
        public string PromotionText { get; set; } = string.Empty;
        public string PromotionCode { get; set; } = string.Empty;
    }
}