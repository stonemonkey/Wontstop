using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content.PM;
using Android.Support.Design.Widget;
using Android.Views;
using System;

namespace Wontstop.Climb.Ui.Android
{
    [Activity(Label = "@string/app_name", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, Icon = "@drawable/Icon", WindowSoftInputMode = SoftInput.AdjustResize)]
    public class LoginActivity : Activity
    {
        #region UI elements

        private View _rootLayout;
        private Button _loginButton;
        private EditText _usernameInput;
        private EditText _passwordInput;

        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.login);

            LoadElements();

            _loginButton.Click += OnLoginClicked;
        }

        private void OnLoginClicked(object s, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_usernameInput.Text))
            {
                Snackbar.Make(_rootLayout, "Fill in username.", Snackbar.LengthLong)
                    .SetAction("OK", (v) => { })
                    .Show();
            }
            else if (string.Equals(_passwordInput.Text, "monkey", StringComparison.OrdinalIgnoreCase))
            {
                Snackbar.Make(_rootLayout, "Invalid Password", Snackbar.LengthLong)
                    .SetAction("Clear", (v) => { _passwordInput.Text = string.Empty; })
                    .Show();
            }
            else
            {
                Snackbar.Make(_rootLayout, "You are now Logged in!", Snackbar.LengthLong)
                    .Show();
            }
        }

        private void LoadElements()
        {
            _rootLayout = FindViewById(Resource.Id.loginRootLayout);
            _loginButton = FindViewById<Button>(Resource.Id.loginButton);
            _usernameInput = FindViewById<EditText>(Resource.Id.usernameInput);
            _passwordInput = FindViewById<EditText>(Resource.Id.passwordInput);
        }
    }
}

