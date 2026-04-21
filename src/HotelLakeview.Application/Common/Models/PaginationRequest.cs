namespace HotelLakeview.Application.Common.Models;

/// <summary>
/// Kuvaa paginoidun haun parametrit.
/// </summary>
public class PaginationRequest
{
    private const int MaxPageSize = 100;

    private int _page = 1;
    private int _pageSize = 10;

    /// <summary>
    /// Sivunumero. Oletus on 1.
    /// </summary>
    public int Page
    {
        get => _page;
        set => _page = value < 1 ? 1 : value;
    }

    /// <summary>
    /// Sivukoko. Oletus on 10 ja maksimi 100.
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set
        {
            if (value < 1)
            {
                _pageSize = 10;
                return;
            }

            _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }
    }
}