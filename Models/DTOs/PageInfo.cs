namespace API_Test1
{
    public class PageInfo<T>
    {
        public Pagination Pagination { get; set; }
        public IQueryable<T> Data { get; set; }

        public PageInfo(Pagination pagination, IQueryable<T> data)
        {
            Pagination = pagination;
            Data = data;
        }
        public static IQueryable<T> ToPageInfo(Pagination page, IQueryable<T> data)
        {
            page.PageNumber = page.PageNumber < 1 ? 1 : page.PageNumber;
            data = data.Skip(page.PageSize * (page.PageNumber - 1)).Take(page.PageSize);
            return data;
        }
    }
}
