namespace HandMade_Store_api.DTOs.Color
{
    public class ColorRequest
    {
        public string Name { get; set; }
    }

    public class ColorResponse
    {
        public int ColorId { get; set; }
        public string Name { get; set; }
    }

    public class ColorUpdateRequest
    {
        public string Name { get; set; }
    }
}
