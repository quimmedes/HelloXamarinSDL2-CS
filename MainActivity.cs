using Android.Content.Res;
using Android.Content.Res.Loader;
using Android.OS;
using Android.Util;
using Android.Views;
using Java.Lang;
using Java.Util;
using Microsoft.VisualBasic.FileIO;
using Org.Libsdl.App;
using SDL2;
using System.Reflection;
using System.Runtime.InteropServices;
using static SDL2.SDL;
using static SDL2.SDL_image;
using Exception = System.Exception;


namespace HelloXamarinSDL_CS
{


    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : SDLActivity
    {
        public static MainActivity Instance { get ; protected set; }
        public static bool Fullscreen = true;
        static nint window;
        static nint renderer;

        //Return the libraries you want to load that are inside lib folder this is only for the SDL and Java comunication/
        //you also have to make sure the C# DllImport are set up correctly since you want to use C# too.
        protected override string[] GetLibraries()
        {

            return new string[] { "SDL2", "SDL2_image", "main", "SDL2_mixer", "SDL2_ttf", "SDL2_net" };
            
        }

        protected override string MainFunction
        {
            get
            {
                Init();
                return "SDL_main";

            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Window.AddFlags(WindowManagerFlags.Fullscreen);
            Window.RequestFeature(WindowFeatures.NoTitle);

            if(Build.VERSION.SdkInt >= BuildVersionCodes.P)
                Window.Attributes.LayoutInDisplayCutoutMode = LayoutInDisplayCutoutMode.ShortEdges;

            Instance = this;
            Fullscreen = true;
            //Don't forget to call the base class
            base.OnCreate(savedInstanceState);

          
         
        }


        public static void Init()
        {


            SDL_Init(SDL_INIT_EVERYTHING);
            IMG_Init(IMG_InitFlags.IMG_INIT_PNG | IMG_InitFlags.IMG_INIT_JPG);
            

            DisplayMetrics dm = new DisplayMetrics();
            Instance.WindowManager.DefaultDisplay.GetMetrics(dm);

            window = SDL_CreateWindow(
                "Hello World",
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


            byte[] bitmap = ReadAssetFileToByteArray("helloworld.jpeg");

            
            //LoadTextureFromByteArray can also be used with Properties.Resources
            //if you want to load using the resources properties instead of the assets folder
            nint texture = LoadTextureFromByteArray(renderer, bitmap);


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

        public static nint LoadTextureFromByteArray(nint renderer, byte[] imageData)
        {
            GCHandle handle = GCHandle.Alloc(imageData, GCHandleType.Pinned);
            nint imagePtr = handle.AddrOfPinnedObject();
            // Create SDL_RWops from byte array
            nint rwops = SDL_RWFromMem(imagePtr, imageData.Length);
            if (rwops == nint.Zero)
                throw new Exception("Failed to create RWops: " + SDL_GetError());

            // Load texture from RWops
            nint texture = IMG_LoadTexture_RW(renderer, rwops, 1);
            if (texture == nint.Zero)
                throw new Exception("Failed to load texture from memory: " + IMG_GetError());

            handle.Free();
            return texture;
        }

        // Method to read a file from the assets folder into a byte array
        public static byte[] ReadAssetFileToByteArray(string fileName)
        {
            AssetManager assets = Application.Context.Assets;

            using (Stream stream = assets.Open(fileName)) 
            using (MemoryStream memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }




        public override void OnWindowFocusChanged(bool hasFocus)
        {
            base.OnWindowFocusChanged(hasFocus);
            if (hasFocus && Fullscreen)
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
