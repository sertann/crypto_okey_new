using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Firebase.Auth;
using System.Net.Mail;
using Firebase.Extensions;
using Firebase;

public class AuthenticationManager : MonoBehaviour
{

    [SerializeField] GameObject loginPage, signUpPage, forgetPage, warning;
    [SerializeField] TMP_InputField loginEmail, loginPassword, signupNick, signupEmail, signupPassword, signupCPassword, forgetEmail;
    FirebaseAuth auth;
    FirebaseUser user;
    string displayName;

    private void Start()
    {
        InitializeFirebase();
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
                displayName = user.DisplayName ?? "";
            }
        }
    }

    void InitializeFirebase()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    public void OpenSignUpPage()
    {
        loginPage.SetActive(false);
        signUpPage.SetActive(true);
    }

    public void OpenLoginPage()
    {
        loginPage.SetActive(true);
        signUpPage.SetActive(false);
    }

    public void LoginGame()
    {
        if (string.IsNullOrEmpty(loginEmail.text) || string.IsNullOrEmpty(loginPassword.text))
        {
            StopAllCoroutines();
            StartCoroutine(ThrowWarning());
            return;
        }

        // login
        LoginUser(loginEmail.text, loginPassword.text);
    }

    public void SignUp()
    {
        if (string.IsNullOrEmpty(signupEmail.text) || string.IsNullOrEmpty(signupPassword.text) || string.IsNullOrEmpty(signupCPassword.text))
        {
            StopAllCoroutines();
            StartCoroutine(ThrowWarning());
            return;
        }

        //Signup
        SignUpUser(signupEmail.text, signupPassword.text);
    }

    public void ForgetPassword()
    {
        if (string.IsNullOrEmpty(forgetEmail.text))
        {
            StopAllCoroutines();
            StartCoroutine(ThrowWarning());
            return;
        }

        //send password reset
    }

    IEnumerator ThrowWarning()
    {
        warning.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        warning.SetActive(false);
    }

    void LoginUser(string email, string password)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
            user = newUser;
            displayName = newUser.DisplayName;
        });
    }

    void SignUpUser(string email, string password)
    {
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }
            OpenLoginPage();
            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
            
        });
        
    }

    void OnDestroy()
    {
        auth.StateChanged -= AuthStateChanged;
        auth = null;
    }
}
