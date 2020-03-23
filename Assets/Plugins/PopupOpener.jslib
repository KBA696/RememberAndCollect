var LibraryPageTool = {
    GoFullscreen: function()
    {
        var viewFullScreen = document.getElementById("unityContainer");

       /* var ActivateFullscreen = function()
        {*/
			var isInFullScreen = (document.fullscreenElement && document.fullscreenElement !== null) ||
				(document.webkitFullscreenElement && document.webkitFullscreenElement !== null) ||
				(document.mozFullScreenElement && document.mozFullScreenElement !== null) ||
				(document.msFullscreenElement && document.msFullscreenElement !== null);

			if (!isInFullScreen) {
				if (viewFullScreen.requestFullscreen) {
					viewFullScreen.requestFullscreen();
				} else if (viewFullScreen.mozRequestFullScreen) {
					viewFullScreen.mozRequestFullScreen();
				} else if (viewFullScreen.webkitRequestFullScreen) {
					viewFullScreen.webkitRequestFullScreen();
				} else if (viewFullScreen.msRequestFullscreen) {
					viewFullScreen.msRequestFullscreen();
				}
			} else {
				if (document.exitFullscreen) {
					document.exitFullscreen();
				} else if (document.webkitExitFullscreen) {
					document.webkitExitFullscreen();
				} else if (document.mozCancelFullScreen) {
					document.mozCancelFullScreen();
				} else if (document.msExitFullscreen) {
					document.msExitFullscreen();
				}
			}
		/*	
            viewFullScreen.removeEventListener('click', ActivateFullscreen);//touchend, click, mouseup
        }
 
        viewFullScreen.addEventListener('click', ActivateFullscreen, false);//touchend, click, mouseup*/
    }
};
mergeInto(LibraryManager.library, LibraryPageTool);