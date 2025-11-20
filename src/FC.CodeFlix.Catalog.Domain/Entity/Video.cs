using FC.CodeFlix.Catalog.Domain.Enum;
using FC.CodeFlix.Catalog.Domain.Exceptions;
using FC.CodeFlix.Catalog.Domain.SeedWork;
using FC.CodeFlix.Catalog.Domain.Validation;
using FC.CodeFlix.Catalog.Domain.ValueObject;

namespace FC.CodeFlix.Catalog.Domain.Entity
{
    public class Video : AggregateRoot
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public int YearLaunched { get; set; }

        public bool Opened { get; set; }

        public bool Published { get; set; }

        public int Duration { get; set; }

        public DateTime CreatedAt { get; set; }

        public Rating Rating { get; set; }

        public Image? Thumb { get; private set; }
        public Image? ThumbHalf { get; private set; }
        public Image? Banner { get; private set; }
        public Media? Media { get; set; }
        public Media? Trailer { get; private set; }

        private List<Guid> _categories;
        public IReadOnlyList<Guid> Categories => _categories.AsReadOnly();

        private List<Guid> _genres;
        public IReadOnlyList<Guid> Genres => _genres.AsReadOnly();

        private List<Guid> _castMembers;
        public IReadOnlyList<Guid> CastMembers 
            => _castMembers.AsReadOnly();


        public Video(
            string title,
            string description,
            int yearLaunched,
            bool opened,
            bool published,
            int duration,
            Rating rating
        )
        {
            Title = title;
            Description = description;
            YearLaunched = yearLaunched;
            Opened = opened;
            Published = published;
            Duration = duration;
            Rating = rating;
            CreatedAt = DateTime.Now;
            _categories = new();
            _genres = new();
            _castMembers = new();
        }

        public void Validate(ValidationHandler notificationHandler)
            => (new VideoValidator(this, notificationHandler)).Validate();

        public void Update(
            string title,
            string description,
            int yearLaunched,
            bool opened,
            bool published,
            int duration,
            Rating? rating = null
        )
        {
            Title = title;
            Description = description;
            YearLaunched = yearLaunched;
            Opened = opened;
            Published = published;
            Duration = duration;
            if (rating is not null) Rating = (Rating)rating!;
        }

        public void UpdateThumb(string path) => Thumb = new Image(path);

        public void UpdateThumbHalf(string path) => ThumbHalf = new Image(path);

        public void UpdateBanner(string path) => Banner = new Image(path);

        public void UpdateMedia(string validMediaPath) => Media = new Media(validMediaPath);

        public void UpdateTrailer(string validTrailerPath) => Trailer = new Media(validTrailerPath);

        public void UpdateAsSentToEncode()
        {
            if (Media is null)
                throw new EntityValidationException("There is no media.");

            Media.UpdateAsSentToEncode();
        }

        public void UpdateAsEncoded(string encodedExamplePath)
        {
            if (Media is null)
                throw new EntityValidationException("There is no media.");
            Media.UpdateAsEncoded(encodedExamplePath);
        }

        public void AddCategory(Guid categoryIdExample)
            => _categories.Add(categoryIdExample);

        public void RemoveCategory(Guid categoryIdExample)
            => _categories.Remove(categoryIdExample);

        public void RemoveAllCategories()
            => _categories = new();

        public void AddGenre(Guid genreId)
            => _genres.Add(genreId);

        public void RemoveGenre(Guid genreId)
            => _genres.Remove(genreId);

        public void RemoveAllGenres()
            => _genres = new();

        public void AddCastMember(Guid castMemberId)
            => _castMembers.Add(castMemberId);

        public void RemoveCastMember(Guid castMemberId)
            => _castMembers.Remove(castMemberId);

        public void RemoveAllCastMembers()
            => _castMembers = new();
    }
}
