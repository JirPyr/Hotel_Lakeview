namespace HotelLakeview.Application.Common.Models;

/// <summary>
/// Kuvaa paginoidun vastauksen.
/// </summary>
/// <typeparam name="T">Listan item-tyyppi.</typeparam>
public class PagedResult<T>
{
    /// <summary>
    /// Luo uuden paginoidun vastauksen.
    /// </summary>
    /// <param name="items">Sivun data.</param>
    /// <param name="page">Nykyinen sivu.</param>
    /// <param name="pageSize">Sivukoko.</param>
    /// <param name="totalCount">Koko tulosjoukon määrä.</param>
    public PagedResult(
        IReadOnlyCollection<T> items,
        int page,
        int pageSize,
        int totalCount)
    {
        Items = items;
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    /// <summary>
    /// Sivun rivit.
    /// </summary>
    public IReadOnlyCollection<T> Items { get; }

    /// <summary>
    /// Nykyinen sivunumero.
    /// </summary>
    public int Page { get; }

    /// <summary>
    /// Käytetty sivukoko.
    /// </summary>
    public int PageSize { get; }

    /// <summary>
    /// Koko tulosjoukon määrä.
    /// </summary>
    public int TotalCount { get; }

    /// <summary>
    /// Kokonaissivujen määrä.
    /// </summary>
    public int TotalPages => PageSize <= 0
        ? 0
        : (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// Kertoo, onko edellinen sivu olemassa.
    /// </summary>
    public bool HasPreviousPage => Page > 1;

    /// <summary>
    /// Kertoo, onko seuraava sivu olemassa.
    /// </summary>
    public bool HasNextPage => Page < TotalPages;
}