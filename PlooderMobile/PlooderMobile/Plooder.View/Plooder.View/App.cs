﻿using Xamarin.Forms;

namespace Plooder.View
{
    public class App : Application
    {
        

        public App()
        {
            MainPage = new View.MainPage();

        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }


    }
}