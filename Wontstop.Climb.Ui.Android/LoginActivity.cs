using Android.App;
using Android.Widget;
using Android.OS;

namespace Wontstop.Climb.Ui.Android
{
    [Activity(Label = "Wontstop Climbing", MainLauncher = true)]
    public class LoginActivity : Activity
    {
        Button _translateButton;
        EditText _phoneNumberText;
        EditText _translatedPhoneWord;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.LoginPage);

            _translateButton = FindViewById<Button>(Resource.Id.buttonLogin);
            _phoneNumberText = FindViewById<EditText>(Resource.Id.editTextUser);
            _translatedPhoneWord = FindViewById<EditText>(Resource.Id.editTextPassword);
        }
    }
}

