﻿namespace MvcBoilerplate.Services
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel.Syndication;
    using System.Web.Mvc;
    using MvcBoilerplate.Constants;
    using MvcBoilerplate.Framework;

    /// <summary>
    /// Builds <see cref="SyndicationFeed"/>'s containing meta data about the feed and the feed entries.
    /// Note: We are targeting Atom 1.0 over RSS 2.0 because Atom 1.0 is a newer and more well defined format. Atom 1.0 is a standard and RSS is not. 
    /// (See http://www.intertwingly.net/wiki/pie/Rss20AndAtom10Compared).
    /// (See http://atomenabled.org/developers/syndication/ for more information about Atom 1.0).
    /// (See https://tools.ietf.org/html/rfc4287 for the official Atom Syndication Format specifications).
    /// (See http://feedvalidator.org/ to validate your feed).
    /// (See http://stackoverflow.com/questions/1301392/pagination-in-feeds-like-atom-and-rss to see how you can add paging to your feed).
    /// </summary>
    public sealed class FeedService : IFeedService
    {
        /// <summary>
        /// The feed universally unique identifier. Do not use the URL of your feed as some recommend as this can change.
        /// A much better ID is to use a GUID which you can generate from Tools->Create GUID in Visual Studio.
        /// </summary>
        private const string FeedId = "[INSERT GUID HERE]";
        private readonly UrlHelper urlHelper;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedService"/> class.
        /// </summary>
        /// <param name="urlHelper">The URL helper.</param>
        public FeedService(UrlHelper urlHelper)
        {
            this.urlHelper = urlHelper;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the feed containing meta data about the feed and the feed entries.
        /// </summary>
        /// <returns>A <see cref="SyndicationFeed"/>.</returns>
        public SyndicationFeed GetFeed()
        {
            SyndicationFeed feed = new SyndicationFeed()
            {
                // id (Required) - The feed universally unique identifier.
                Id = FeedId,
                // title (Required) - Contains a human readable title for the feed. Often the same as the title of the associated website. This value should not be blank.
                Title = SyndicationContent.CreatePlaintextContent("ASP.NET MVC Boilerplate"),
                // items (Required) - The items to add to the feed.
                Items = this.GetItems(),
                // subtitle (Recommended) - Contains a human-readable description or subtitle for the feed.
                Description = SyndicationContent.CreatePlaintextContent("This is the ASP.NET MVC Boilerplate feed description."),
                // updated (Optional) - Indicates the last time the feed was modified in a significant way.
                LastUpdatedTime = DateTimeOffset.Now,
                // logo (Optional) - Identifies a larger image which provides visual identification for the feed. Images should be twice as wide as they are tall.
                ImageUrl = new Uri(this.urlHelper.AbsoluteContent("~/content/icons/atom-logo-96x48.png")),
                // rights (Optional) - Conveys information about rights, e.g. copyrights, held in and over the feed.
                Copyright = new TextSyndicationContent(string.Format("© {0} - {0}", DateTime.Now.Year, Application.Name)),
                // lang (Optional) - The language of the feed.
                Language = "en-GB",
                // generator (Optional) - Identifies the software used to generate the feed, for debugging and other purposes. Do not put in anything that identifies the technology you are using.
                // Generator = "Sample Code",
                // base (Buggy) - Add the full base URL to the site so that all other links can be relative. This is great, except some feed readers are buggy with it, INCLUDING FIREFOX!!! (See https://bugzilla.mozilla.org/show_bug.cgi?id=480600).
                // BaseUri = new Uri(this.urlHelper.AbsoluteRouteUrl(HomeControllerRoute.GetIndex))
            };

            // self link (Required) - The URL for the syndication feed.
            feed.Links.Add(SyndicationLink.CreateSelfLink(new Uri(this.urlHelper.AbsoluteRouteUrl(HomeControllerRoute.GetFeed)), ContentType.Atom));

            // alternate link (Recommended) - The URL for the web page showing the same data as the syndication feed.
            feed.Links.Add(SyndicationLink.CreateAlternateLink(new Uri(this.urlHelper.AbsoluteRouteUrl(HomeControllerRoute.GetIndex)), ContentType.Html));

            // author (Recommended) - Names one author of the feed. A feed may have multiple author elements. A feed must contain at least one author element unless all of the entry elements contain at least one author element.
            feed.Authors.Add(this.GetPerson());

            // category (Optional) - Specifies a category that the feed belongs to. A feed may have multiple category elements.
            feed.Categories.Add(new SyndicationCategory("CategoryName"));

            // contributor (Optional) - Names one contributor to the feed. An feed may have multiple contributor elements.
            feed.Contributors.Add(this.GetPerson());

            // icon (Optional) - Identifies a small image which provides iconic visual identification for the feed. Icons should be square.
            feed.SetIcon(this.urlHelper.AbsoluteContent("~/content/icons/atom-icon-48x48.png"));

            // Add the Yahoo Media namespace (xmlns:media="http://search.yahoo.com/mrss/") to the Atom feed. 
            // This gives us extra abilities, like the ability to give thumbnail images to entries. See http://www.rssboard.org/media-rss for more information.
            feed.AddYahooMediaNamespace();

            return feed;
        }

        #endregion

        #region Private Methods

        private SyndicationPerson GetPerson()
        {
            return new SyndicationPerson()
            {
                // name (Required) - conveys a human-readable name for the person.
                Name = "Rehan Saeed",
                // uri (Optional) - contains a home page for the person.
                Uri = "http://rehansaeed.co.uk",
                // email (Optional) - contains an email address for the person.
                Email = "example@email.com"
            };
        }

        /// <summary>
        /// Gets the collection of <see cref="SyndicationItem"/>'s that represent the atom entries.
        /// </summary>
        /// <returns>A collection of <see cref="SyndicationItem"/>'s.</returns>
        private List<SyndicationItem> GetItems()
        {
            List<SyndicationItem> items = new List<SyndicationItem>();

            for (int i = 1; i < 4; ++i)
            {
                SyndicationItem item = new SyndicationItem()
                {
                    // id (Required) - Identifies the entry using a universally unique and permanent URI. Two entries in a feed can have the same value for id if they represent the same entry at different points in time.
                    Id = FeedId + i,
                    // title (Required) - Contains a human readable title for the entry. This value should not be blank.
                    Title = SyndicationContent.CreatePlaintextContent("Item " + i),
                    // description (Reccomended) - A summary of the entry.
                    Summary = SyndicationContent.CreatePlaintextContent("A summary of item " + i),
                    // updated (Optional) - Indicates the last time the entry was modified in a significant way. This value need not change after a typo is fixed, only after a substantial modification. Generally, different entries in a feed will have different updated timestamps.
                    LastUpdatedTime = DateTimeOffset.Now,
                    // published (Optional) - Contains the time of the initial creation or first availability of the entry.
                    PublishDate = DateTimeOffset.Now,
                    // rights (Optional) - Conveys information about rights, e.g. copyrights, held in and over the entry.
                    Copyright = new TextSyndicationContent(string.Format("© {0} - {0}", DateTime.Now.Year, Application.Name)),
                };

                // link (Reccomended) - Identifies a related Web page. An entry must contain an alternate link if there is no content element.
                item.Links.Add(SyndicationLink.CreateAlternateLink(new Uri(this.urlHelper.AbsoluteRouteUrl(HomeControllerRoute.GetIndex)), ContentType.Html));
                // AND/OR
                // Text content  (Optional) - Contains or links to the complete content of the entry. Content must be provided if there is no alternate link.
                // item.Content = SyndicationContent.CreatePlaintextContent("The actual plain text content of the entry");
                // HTML content (Optional) - Content can be plain text or HTML. Here is a HTML example.
                // item.Content = SyndicationContent.CreateHtmlContent("The actual HTML content of the entry");

                // author (Optional) - Names one author of the entry. An entry may have multiple authors. An entry must contain at least one author element unless there is an author element in the enclosing feed, or there is an author element in the enclosed source element.
                item.Authors.Add(this.GetPerson());

                // contributor (Optional) - Names one contributor to the entry. An entry may have multiple contributor elements.
                item.Contributors.Add(this.GetPerson());

                // category (Optional) - Specifies a category that the entry belongs to. A entry may have multiple category elements.
                item.Categories.Add(new SyndicationCategory("CategoryName"));

                // link - Add additional links to related images, audio or video like so.
                item.Links.Add(SyndicationLink.CreateMediaEnclosureLink(new Uri(this.urlHelper.AbsoluteContent("~/content/icons/atom-icon-48x48.png")), ContentType.Png, 0));

                // media:thumbnail - Add a Yahoo Media thumbnail for the entry. See http://www.rssboard.org/media-rss for more information.
                item.SetThumbnail(this.urlHelper.AbsoluteContent("~/content/icons/atom-icon-48x48.png"), 48, 48);

                items.Add(item);
            }

            return items;
        }

        #endregion
    }
}