namespace WiseWMS.Application.DTOs
{
    public class UpdateProductDto
    {
        public string Name { get; set; } = string.Empty;

        public string Spec { get; set; } = string.Empty;

        public string Unit { get; set; } = string.Empty;

        public int CategoryId { get; set; }

        public decimal Price { get; set; }

        public int MinStock { get; set; }

        public string Description { get; set; } = string.Empty;
    }
}
