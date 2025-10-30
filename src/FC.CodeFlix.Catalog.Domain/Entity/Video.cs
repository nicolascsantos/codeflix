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
            Rating = Rating;
            CreatedAt = DateTime.Now;
            _categories = new();
        }

        public void Validate(ValidationHandler notificationHandler)
            => (new VideoValidator(this, notificationHandler)).Validate();

        public void Update(
            string title,
            string description,
            int yearLaunched,
            bool opened,
            bool published,
            int duration
        )
        {
            Title = title;
            Description = description;
            YearLaunched = yearLaunched;
            Opened = opened;
            Published = published;
            Duration = duration;
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
    }
}
