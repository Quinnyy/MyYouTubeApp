using YourTubes.Models;

namespace YourTubes.Services
{
    public interface IYouTubeService
    {
        Task<MyYouTube> RetrieveVideosAsync(string searchQ);

        Task<MyYouTube> RetrieveMyPlaylists(MyYouTube myYouTube);

        Task<string> InsertVideoToPlaylist(string playlistID, string videoID);

        Task<string> DeleteMyPlaylist(string playlistID);

        Task<string> CreateMyPlaylist(string playlistName);
    }
}