using WebVisitsMobile.Domain.EntitiesCustom;

namespace WebVisitsMobile.Services.Responses
{
    public class ApiResponse<T>
    {
        public bool Respuesta { get; set; }

        public string Mensaje { get; set; }

        public int Codigo { get; set; }

        public T Data { get; set; }

        public MetaData Meta { get; set; }

        public ApiResponse(bool respuesta, string mensaje, int codigo, T data)
        {
            Respuesta = respuesta;
            Mensaje = mensaje;
            Codigo = codigo;
            Data = data;
        }

        public void CargarMetaData(int totalCount, int pageSize, int currentPage, int totalPages, bool hasNextPage,
                                    bool hasPreviousPage, string urlNextPage, string urlPreviousPage)
        {
            try
            {
                Meta = new MetaData();
                Meta.TotalCount = totalCount;
                Meta.PageSize = pageSize;
                Meta.CurrentPage = currentPage;
                Meta.TotalPages = totalPages;
                Meta.HasNextPage = hasNextPage;
                Meta.HasPreviousPage = hasPreviousPage;
                Meta.NextPageUrl = urlNextPage;
                Meta.PreviousPageUrl = urlPreviousPage;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}