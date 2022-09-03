namespace Shopping.Aggregrator.Models
{
    public class BasketModel
    {
        public string UserName { get; set; }
        public List<BasketItemExtendedModel> Items { get; set; }
    }
}
