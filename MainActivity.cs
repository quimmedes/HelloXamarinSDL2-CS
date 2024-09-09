using Android.Nfc;
using Android.OS;
using Android.Util;
using Android.Views;
using Java.Lang;
using Org.Libsdl.App;
using SDL2;
using System.Runtime.InteropServices;
using static SDL2.SDL;
using static SDL2.SDL_image;


namespace HelloXamarinSDL_CS
{


    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : SDLActivity
    {
        public static MainActivity Instance { get ; protected set; }
        public static bool SDL2DCS_Fullscreen = true;
        static nint window;
        static nint renderer;

        [DllImport("main")]
        static extern void SetMain(Main main);


        public static void Init()
        {


            SDL_Init(SDL_INIT_EVERYTHING);
            IMG_Init(IMG_InitFlags.IMG_INIT_PNG | IMG_InitFlags.IMG_INIT_JPG);
            

            DisplayMetrics dm = new DisplayMetrics();
            Instance.WindowManager.DefaultDisplay.GetMetrics(dm);

            window = SDL_CreateWindow(
                "HEY, LISTEN!",
                SDL_WINDOWPOS_CENTERED,
                SDL_WINDOWPOS_CENTERED,
                dm.WidthPixels,
                dm.HeightPixels,
                SDL_WindowFlags.SDL_WINDOW_OPENGL |
                SDL_WindowFlags.SDL_WINDOW_INPUT_FOCUS |
                SDL_WindowFlags.SDL_WINDOW_MOUSE_FOCUS | 
                SDL_WindowFlags.SDL_WINDOW_FULLSCREEN
            );

             renderer = SDL_CreateRenderer(window, -1, SDL_RendererFlags.SDL_RENDERER_ACCELERATED);

            nint texture = IMG_LoadTexture(renderer, Path.Combine("file:///android_asset", "player.png")); 

           

            int i = 0;
            while(i<600)
            {
                SDL_Event e;
                while (SDL_PollEvent(out e) != 0)
                {
                    if (e.type == SDL_EventType.SDL_QUIT)
                    {
                        return;
                    }
                }
                i++;

                SDL_RenderClear(renderer);
                SDL_SetRenderDrawColor(renderer, (byte) (i % 255), 128, 0, 255);


                blit(texture, 100, 100);

                SDL_RenderPresent(renderer);

                SDL_Delay(16);
            }


            SDL_DestroyRenderer(renderer);
            SDL_DestroyWindow(window);
            SDL_Quit();



        }
        
        

        public override void LoadLibraries()
        {
            base.LoadLibraries();
            Instance = this;
            Bootstrap.SetMain(Init);
            // Bootstrap.SetupMain();
        }

        public override void OnWindowFocusChanged(bool hasFocus)
        {
            base.OnWindowFocusChanged(hasFocus);
            if (hasFocus && SDL2DCS_Fullscreen)
            {
                Window.DecorView.SystemUiFlags = (SystemUiFlags)(
                    SystemUiFlags.LayoutStable |
                    SystemUiFlags.LayoutHideNavigation |
                    SystemUiFlags.LayoutFullscreen |
                    SystemUiFlags.HideNavigation |
                    SystemUiFlags.Fullscreen |
                    SystemUiFlags.ImmersiveSticky
                );
            }
        }

        
        

        protected override string[]? GetLibraries()
        {

            return new string[] { "SDL2", "SDL2_image", "main"  };
        }

        public static void blit(IntPtr texutre, float x, float y)
        {
            SDL_Rect dest = new SDL_Rect();
            dest.x = (int)x;
            dest.y = (int)y;

            SDL_QueryTexture(texutre, out uint format, out int access, out dest.w, out dest.h);
            SDL_RenderCopy(renderer, texutre, IntPtr.Zero, ref dest);

        }


    }

   
}