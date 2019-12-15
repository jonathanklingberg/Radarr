﻿using System.Collections.Generic;
using Newtonsoft.Json;
using NzbDrone.Common.Extensions;
using NzbDrone.Core.MetadataSource;
using NzbDrone.Core.MetadataSource.SkyHook.Resource;

namespace NzbDrone.Core.NetImport.TMDb.List
{
    public class TMDbListParser : TMDbParser
    {
        private readonly ISearchForNewMovie _skyhookProxy;
        private NetImportResponse _importResponse;

        public TMDbListParser(ISearchForNewMovie skyhookProxy)
            : base(skyhookProxy)
        {
            _skyhookProxy = skyhookProxy;
        }

        public override IList<Movies.Movie> ParseResponse(NetImportResponse importResponse)
        {
            _importResponse = importResponse;

            var movies = new List<Movies.Movie>();

            if (!PreProcess(_importResponse))
            {
                return movies;
            }

            var jsonResponse = JsonConvert.DeserializeObject<ListResponseRoot>(_importResponse.Content);

            // no movies were return
            if (jsonResponse == null)
            {
                return movies;
            }

            foreach (var movie in jsonResponse.items)
            {
                // Movies with no Year Fix
                if (string.IsNullOrWhiteSpace(movie.release_date))
                {
                    continue;
                }

                movies.AddIfNotNull(_skyhookProxy.MapMovie(movie));
            }

            return movies;
        }
    }
}
