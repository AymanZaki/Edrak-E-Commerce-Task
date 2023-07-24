
namespace Edrak.Order.Models
{
    public class PageModel<T> where T : class
    {
        public int TotalCount { get; set; }
        public int PagesCount { get; set; }
        public int PageNumber { get; set; }
        public int Limit { get; set; }
        public IEnumerable<T> Results { get; set; }
    }
}
