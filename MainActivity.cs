using Android.Nfc;
using Android.OS;
using Android.Util;
using Java.Lang;
using Org.Libsdl.App;
using SDL2;
using static SDL2.SDL;
using static SDL2.SDL_image;


namespace HelloXamarinSDL_CS
{

    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : SDLActivity
    {
        static nint window;
        static nint renderer;
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            //base.OnCreate(savedInstanceState);

            Log.Verbose("Device: ", Build.Device);
            Log.Verbose("Model: " , Build.Model);
            Log.Verbose("","OnCreate()");
            base.OnCreate(savedInstanceState);

            try
            {

                Java.Lang.Thread.CurrentThread().Name = "SDLActivity";

            }
            catch (System.Exception e)
            {
                Log.Error( "modify thread properties failed: " , e.Message);
            }

            // Load shared libraries
            string errorMsgBrokenLib = "";


            try
            {
               foreach(string lib in GetLibraries())
                {
                    Org.Libsdl.App.SDL.LoadLibrary(lib);
                }
                MBrokenLibraries = false; /* success */
            }
            catch (UnsatisfiedLinkError e)
            {
                e.PrintStackTrace();
                MBrokenLibraries = true;
                errorMsgBrokenLib = e.Message;
            }
            catch (System.Exception e)
            {

                Log.Error("", e.Message);
                MBrokenLibraries = true;
                errorMsgBrokenLib = e.Message;
            }

           

            if (MBrokenLibraries)
            {
                MSingleton = this;
                AlertDialog.Builder dlgAlert = new AlertDialog.Builder(this);
                dlgAlert.SetMessage("An error occurred while trying to start the application. Please try again and/or reinstall."
                      + Java.Lang.JavaSystem.GetProperty("line.separator")
                      + Java.Lang.JavaSystem.GetProperty("line.separator")
                      + "Error: " + errorMsgBrokenLib);
                dlgAlert.SetTitle("SDL Error");

                dlgAlert.SetPositiveButton("Exit", (sender, args) =>
                {
                    MSingleton.Finish();
                });

                dlgAlert.SetCancelable(false);
                dlgAlert.Create().Show();

                return;
            }


            MSingleton = this;
            Org.Libsdl.App.SDL.Context = this;

          //  MClipboardHandler = new SDLClipboardHandler(this);
           // MHIDDeviceManager = new SDLHIDDeviceManager();

            Org.Libsdl.App.SDL.SetupJNI();
            
            MSurface = CreateSDLSurface(this);
            MLayout = new RelativeLayout(this);
            MLayout.AddView(MSurface);
            MCurrentOrientation = SDLActivity.MCurrentOrientation;
            OnNativeOrientationChanged(MCurrentOrientation);

            SetContentView(MLayout);
            SetWindowStyle(false);
            this.Window.DecorView.SetOnSystemUiVisibilityChangeListener(this);


            // Set our view from the "main" layout resource
            //   SetContentView(Resource.Layout.activity_main);

            if (SDL_Init(SDL_INIT_EVERYTHING)<0)
            {
                Toast.MakeText(this, "SDL_Init failed", ToastLength.Long).Show();
            }
            else
            {
                Toast.MakeText(this, "SDL_Init success "+ NativeGetVersion(), ToastLength.Long).Show();
            }



            if (IMG_Init(IMG_InitFlags.IMG_INIT_PNG | IMG_InitFlags.IMG_INIT_JPG) < 0)
            {
                Toast.MakeText(this, "SDL_image failed", ToastLength.Long).Show();
            }
            else
            {
                Toast.MakeText(this, "SDL_image success " , ToastLength.Long).Show();
            }


             window = SDL_CreateWindow("An SDL2 window!", SDL_WINDOWPOS_UNDEFINED, 0, 640, 480, SDL_WindowFlags.SDL_WINDOW_OPENGL);

             renderer = SDL_CreateRenderer(window, -1, SDL_RendererFlags.SDL_RENDERER_ACCELERATED);

            nint texture = IMG_LoadTexture(renderer, Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "player.png")); 

            if(texture > 0)
                Toast.MakeText(this, "Texture loaded", ToastLength.Long).Show();



            int i = 0;
            while(i<300)
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
                SDL_SetRenderDrawColor(renderer, 0, 128, 0, 255);


                blit(texture, 100, 100);

                SDL_RenderPresent(renderer);

                SDL_Delay(16);
            }


            SDL_DestroyRenderer(renderer);
            SDL_DestroyWindow(window);
            SDL_Quit();



        }

        protected override string[]? GetLibraries()
        {
            return new string[] { "SDL2", "main","SDL2_image"  };
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