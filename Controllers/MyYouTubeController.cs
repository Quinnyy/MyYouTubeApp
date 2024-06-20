using Google.Apis.YouTube.v3.Data;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;
using YourTubes.Models;
using YourTubes.Services;

namespace YourTubes.Controllers
{
    public class MyYouTubeController : Controller
    {
        private readonly IYouTubeService youTubeService;

        public MyYouTubeController(IYouTubeService youTubeService)
        {
            this.youTubeService = youTubeService;
        }

        /// <summary>
        /// Default controller action that is called when page is first loaded. Passes empty model back to view as there is no data to render.
        /// </summary>
        /// <returns></returns>
        public IActionResult VideoSearchList()
        {
            return View(new MyYouTube());
        }

        /// <summary>
        /// Post method which is invoced by begin form on view. It takes in user input keyword and calls service method to search youtube videos. Returns populated model containing found videos
        /// </summary>
        /// <param name="searchQ"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> VideoSearchList(string searchQ)
        {
            //Google command pattern handler (controller calls command class which builds paras and calls service)
           MyYouTube myYouTube = await youTubeService.RetrieveVideosAsync(searchQ);

            return View(myYouTube);
        }

        /// <summary>
        /// Post method which is invoced by begin form on view. Take selected video and playlist from dropdown box in view and calls service method to add the video to the playlist.
        /// </summary>
        /// <param name="playlistID"></param>
        /// <param name="videoID"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddVideoToPlaylist(string playlistID, string videoID)
        {
            string response =  await youTubeService.InsertVideoToPlaylist(playlistID, videoID);
            return View("VideoSearchList",new MyYouTube());
        }

        /// <summary>
        /// Post method which is invoced by begin form on view. Takes playlist id from selected item in the dropdown and calls service method to delete the playlist.
        /// </summary>
        /// <param name="playlistID"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeletePlaylist(string playlistID)
        {
            string response = await youTubeService.DeleteMyPlaylist(playlistID);
            return View("VideoSearchList", new MyYouTube());       
        }

        /// <summary>
        /// Post method which is invoced by begin form on view. Takes text box input from users and calls service method to create a new playlist
        /// </summary>
        /// <param name="youtube_playlistName"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreatePlaylist(string youtube_playlistName)
        {
            string response = await youTubeService.CreateMyPlaylist(youtube_playlistName);
            return View("VideoSearchList", new MyYouTube());
        }
    }
}