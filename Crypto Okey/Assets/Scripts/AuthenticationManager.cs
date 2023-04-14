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

    [SerializeField] GameObject loginPage, signUpPage, forgetPage, warning, gamePage;
    [SerializeField] TMP_InputField loginEmail, loginPassword, signupEmail, signupPassword, signupCPassword, forgetEmail;
    FirebaseAuth auth;
    FirebaseUser user;
    string displayName;

    public static AuthenticationManager instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            return;
        }

        Destroy(gameObject);
    }

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

    void CloseLoginPage()
    {
        loginPage.SetActive(false);
    }

    void CloseSignUpPage()
    {
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
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task => {
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
            if (!user.IsEmailVerified)
            {
                SendVerificationEmail();
            }
            else
            {
                ConnectToGameScene();
            }
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
            
            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
            if (!user.IsEmailVerified)
            {
                SendVerificationEmail();
            }

            OpenLoginPage();

        });
        
    }

    void ConnectToGameScene()
    {
        CloseLoginPage();
        gamePage.SetActive(true);
    }

    void SendVerificationEmail()
    {
        
        if (user != null)
        {
            user.SendEmailVerificationAsync().ContinueWithOnMainThread(task => {
                if (task.IsCanceled)
                {
                    Debug.LogError("SendEmailVerificationAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SendEmailVerificationAsync encountered an error: " + task.Exception);
                    return;
                }
                UIManager.instance.EmailSentNotification(user.Email);
                Debug.Log("Email sent successfully.");
            });
        }
    }

    void OnDestroy()
    {
        auth.StateChanged -= AuthStateChanged;
        auth = null;
    }

    public FirebaseUser GetCurrentUser()
    {
        return user;
    }
}
