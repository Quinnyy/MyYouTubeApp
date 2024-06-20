using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Services;
using Google.Apis.Util;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System.Net;
using System.Net.Http;
using System.Text;
using YourTubes.Models;
using static Google.Apis.Requests.BatchRequest;
using static System.Net.WebRequestMethods;

namespace YourTubes.Services
{
    public class YouTubeService : IYouTubeService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IConfiguration configuration;
        private const string YouTubeBaseURL = @"https://www.googleapis.com/youtube/v3/";
        private const string Snippet = "snippet";
        private const string Type = "video";
        private const int Max = 12;
        private const bool trueValue = true;
        private const int number = 0;

        public YouTubeService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            this.configuration = configuration;
            this.httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Method that gets and stores a list of the user's playlist ids and names
        /// </summary>
        /// <param name="myYouTube"></param>
        /// <returns></returns>
        public async Task<MyYouTube> RetrieveMyPlaylists(MyYouTube myYouTube)
        {
            var credentials = await RetrieveCredentialsToken();
            if (credentials.Token.IsExpired(SystemClock.Default))
            {
                credentials.RefreshTokenAsync(CancellationToken.None).Wait();
            }

            var youtubeService = CreateYouTubeService(credentials);
            try
            {
                var request = youtubeService.Playlists.List("snippet");
                request.Mine = trueValue;
                var playLists = request.Execute();
                foreach (var item in playLists.Items)
                {
                    myYouTube.playlistID.Add(item.Id);
                    myYouTube.playlistName.Add(item.Snippet.Title);
                }
            }
            catch (Exception ex)
            {
                //log
                //return error message
            }
            finally
            {
                youtubeService.Dispose();
            }
            return myYouTube;
        }

        /// <summary>
        /// Method that deletes specified playlist via the passed in id
        /// </summary>
        /// <param name="playlistID"></param>
        /// <returns></returns>
        public async Task<string> DeleteMyPlaylist(string playlistID)
        {
            string response = "";
            var credentials = await GetUserCredential();
            var youtubeService = CreateYouTubeService(credentials);
            try
            {
                var request = youtubeService.Playlists.Delete(playlistID);
                var e = request.Execute();
                response = "Success";
            }
            catch (Exception ex)
            {
                response = "Error: " + ex.ToString();
            }
            finally
            {
                youtubeService.Dispose();
            }
            return response;
        }
        /// <summary>
        /// Method that creates a new playlist of the user's channel given the name of the passed in value
        /// </summary>
        /// <param name="playlistName"></param>
        /// <returns></returns>
        public async Task<string> CreateMyPlaylist(string playlistName)
        {
            string response = "";
            var credentials = await GetUserCredential();
            var youtubeService = CreateYouTubeService(credentials);
            try
            {
                PlaylistSnippet snippet = new PlaylistSnippet();
                snippet.Title = playlistName;
                Playlist playlist = new Playlist();
                playlist.Snippet = snippet;
                var request = youtubeService.Playlists.Insert(playlist, "snippet");
                var e = request.Execute();
                response = "Success";
            }
            catch (Exception ex)
            {
                response = "Error: " + ex.ToString();
            }
            finally
            {
                youtubeService.Dispose();
            }

            return response;
        }
        /// <summary>
        /// Method that inserts a video into a existing playlist. 
        /// </summary>
        /// <param name="playlistID"></param>
        /// <param name="videoID"></param>
        /// <returns></returns>
        public async Task<string>InsertVideoToPlaylist(string playlistID, string videoID)
        {
            string response = "";
            var credentials =  await GetUserCredential();
            var youtubeService = CreateYouTubeService(credentials);

            try
            {
                PlaylistItem playlistItem = new PlaylistItem();
                PlaylistItemSnippet snippet = new PlaylistItemSnippet();
                snippet.PlaylistId =playlistID;
                ResourceId resourceId = new ResourceId();
                resourceId.Kind="youtube#video";
                resourceId.VideoId="a";
                snippet.ResourceId=resourceId;
                playlistItem.Snippet=snippet;
                var request = youtubeService.PlaylistItems.Insert(playlistItem, "snippet");
                response = "Success";
            }
            catch (Exception ex)
            {
                response = "Error: " + ex.ToString();
            }
            finally
            {
                youtubeService.Dispose();
            }
            return response;
        }
        /// <summary>
        /// Method that searches and returns specified number of youtube videos based on input keyword
        /// </summary>
        /// <param name="searchQ"></param>
        /// <returns></returns>
        public async Task<MyYouTube> RetrieveVideosAsync(string searchQ)
        {
            MyYouTube myYouTube = new MyYouTube();
            HttpClient httpClient = this.httpClientFactory.CreateClient();
            try
            {
                string url = YouTubeBaseURL + "search?";
                var result = await httpClient.GetAsync(BuildSearchParams(searchQ, Type, url));
                if (result.StatusCode == (HttpStatusCode)200)
                {
                    string response = await result.Content.ReadAsStringAsync();
                    myYouTube = Newtonsoft.Json.JsonConvert.DeserializeObject<MyYouTube>(response);
                    myYouTube = await RetrieveMyPlaylists(myYouTube);
                }
            }
            catch (Exception ex)
            {
                //log
                //return error message
            }
            finally
            {
                httpClient.Dispose();
            }
            return myYouTube;
        }
        /// <summary>
        /// Method that builds params and their values for the Get request
        /// </summary>
        /// <param name="q"></param>
        /// <param name="type"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        private string BuildSearchParams(string q, string type, string url)
        {
            StringBuilder buildURL = new StringBuilder(url);
            buildURL.Append("q=" + q + "&");
            buildURL.Append("part=" + Snippet + "&");
            buildURL.Append("type=" + Type + "&");
            buildURL.Append("key=" + configuration.GetValue<string>("YouTubeAPIKey") + "&");
            buildURL.Append("maxResults=" + Max + "&");

            return buildURL.ToString();
        }
        /// <summary>
        /// Method that returns refreshed/updated OAuth 2.0 credentials 
        /// </summary>
        /// <returns></returns>
        private async Task<UserCredential> GetUserCredential()
        {
            var credentials = await RetrieveCredentialsToken();
            if (credentials.Token.IsExpired(SystemClock.Default))
            {
                credentials.RefreshTokenAsync(CancellationToken.None).Wait();
            }
            return credentials;
        }
        /// <summary>
        /// Method that creates a youtube service object. This is repeating code so put into it's own method
        /// </summary>
        /// <param name="credentials"></param>
        /// <returns></returns>
        private Google.Apis.YouTube.v3.YouTubeService CreateYouTubeService(UserCredential credentials)
        {
            return new Google.Apis.YouTube.v3.YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credentials
            });
        }
        /// <summary>
        /// Method that gets OAuth 2.0 credentials
        /// </summary>
        /// <returns></returns>
        private async Task<UserCredential> RetrieveCredentialsToken()
        {

            string[] scopes = { "https://www.googleapis.com/auth/youtube" };

            return await GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets
                {
                    ClientId = configuration.GetValue<string>("ClientID"),
                    ClientSecret = configuration.GetValue<string>("ClientSecret")
                },
                scopes, "user", CancellationToken.None);              
        }
    }
}
