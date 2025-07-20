namespace Application.Lib.Models;

public record KendoGridModel
{
    public int take { get; set; }
    public int skip { get; set; }
    public int page { get; set; }
    public int pageSize { get; set; }
    public List<sortModel> sort { get; set; } = [];


    public record sortModel
    {
        public string field { get; set; }
        public string dir { get; set; }
    }
}
