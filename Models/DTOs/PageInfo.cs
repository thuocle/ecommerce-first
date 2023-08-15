namespace API_Test1
{
    public class PageInfo<T>
    {
        public Pagination Pagination { get; set; }
        public IEnumerable<T> Data { get; set; }

        public PageInfo(Pagination pagination, IEnumerable<T> data)
        {
            Pagination = pagination;
            Data = data;
        }
        public static IEnumerable<T> ToPageInfo(Pagination page, IEnumerable<T> data)
        {
            page.PageNumber = page.PageNumber < 1 ? 1 : page.PageNumber;
            data = data.Skip(page.PageSize * (page.PageNumber - 1)).Take(page.PageSize).AsQueryable();
            return data;
        }
    }
}
