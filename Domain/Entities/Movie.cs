using Cinema_MGMT.Domain.Common;

namespace Cinema_MGMT.Domain.Entities;

/// <summary>
/// Represents a movie.
/// Encapsulates movie business rules and state.
/// </summary>
public class Movie : BaseEntity
{
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public int DurationMinutes { get; private set; }
    public string Genre { get; private set; } = string.Empty;
    public string Director { get; private set; } = string.Empty;
    public DateTime ReleaseDate { get; private set; }
    public string? PosterUrl { get; private set; }
    public decimal TicketPrice { get; private set; }
    public bool IsActive { get; private set; }

    // Navigation properties
    public virtual ICollection<Screening> Screenings { get; private set; } = new List<Screening>();

    // Private constructor for EF Core
    private Movie() { }

    /// <summary>
    /// Creates a new movie.
    /// </summary>
    /// <param name="title">The movie title</param>
    /// <param name="description">The movie description</param>
    /// <param name="durationMinutes">The movie duration in minutes</param>
    /// <param name="genre">The movie genre</param>
    /// <param name="director">The movie director</param>
    /// <param name="releaseDate">The movie release date</param>
    /// <param name="ticketPrice">The default ticket price</param>
    /// <param name="posterUrl">Optional poster URL</param>
    public Movie(
        string title,
        string description,
        int durationMinutes,
        string genre,
        string director,
        DateTime releaseDate,
        decimal ticketPrice,
        string? posterUrl = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be null or empty", nameof(title));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be null or empty", nameof(description));

        if (durationMinutes <= 0)
            throw new ArgumentException("Duration must be greater than zero", nameof(durationMinutes));

        if (string.IsNullOrWhiteSpace(genre))
            throw new ArgumentException("Genre cannot be null or empty", nameof(genre));

        if (string.IsNullOrWhiteSpace(director))
            throw new ArgumentException("Director cannot be null or empty", nameof(director));

        if (ticketPrice <= 0)
            throw new ArgumentException("Ticket price must be greater than zero", nameof(ticketPrice));

        Title = title.Trim();
        Description = description.Trim();
        DurationMinutes = durationMinutes;
        Genre = genre.Trim();
        Director = director.Trim();
        ReleaseDate = releaseDate;
        TicketPrice = ticketPrice;
        PosterUrl = posterUrl;
        IsActive = true;
    }

    /// <summary>
    /// Updates the movie details.
    /// </summary>
    public void Update(
        string? title = null,
        string? description = null,
        int? durationMinutes = null,
        string? genre = null,
        string? director = null,
        DateTime? releaseDate = null,
        decimal? ticketPrice = null,
        string? posterUrl = null)
    {
        if (!string.IsNullOrWhiteSpace(title))
            Title = title.Trim();

        if (!string.IsNullOrWhiteSpace(description))
            Description = description.Trim();

        if (durationMinutes.HasValue && durationMinutes.Value > 0)
            DurationMinutes = durationMinutes.Value;

        if (!string.IsNullOrWhiteSpace(genre))
            Genre = genre.Trim();

        if (!string.IsNullOrWhiteSpace(director))
            Director = director.Trim();

        if (releaseDate.HasValue)
            ReleaseDate = releaseDate.Value;

        if (ticketPrice.HasValue && ticketPrice.Value > 0)
            TicketPrice = ticketPrice.Value;

        if (posterUrl != null)
            PosterUrl = posterUrl;

        MarkAsUpdated();
    }

    /// <summary>
    /// Activates the movie.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        MarkAsUpdated();
    }

    /// <summary>
    /// Deactivates the movie.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        MarkAsUpdated();
    }
}

