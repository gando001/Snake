using System;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;

// Example script showing how to invoke the Google Mobile Ads Unity plugin.
public class GoogleMobileAdsScript : MonoBehaviour
{
    private BannerView bannerView;

    void Start()
    {
		#if UNITY_EDITOR
		string adUnitId = "unused";
		#elif UNITY_ANDROID
		string adUnitId = "ca-app-pub-6634653121418342/2036883916";
		#elif UNITY_IPHONE
		string adUnitId = "INSERT_IOS_BANNER_AD_UNIT_ID_HERE";
		#else
		string adUnitId = "unexpected_platform";
		#endif
		
		// Create a 320x50 banner at the bottom of the screen.
		bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Bottom);
/*		// Register for ad events.
		bannerView.AdLoaded += HandleAdLoaded;
		bannerView.AdFailedToLoad += HandleAdFailedToLoad;
		bannerView.AdOpened += HandleAdOpened;
		bannerView.AdClosing += HandleAdClosing;
		bannerView.AdClosed += HandleAdClosed;
		bannerView.AdLeftApplication += HandleAdLeftApplication;
		*/
		// Load a banner ad.
		bannerView.LoadAd(createAdRequest());

		HideBanner();
    }

    public void ShowBanner()
    {
		if (bannerView != null)
			bannerView.Show();
    }

	public void HideBanner()
	{
		if (bannerView != null)
			bannerView.Hide();
	}

    // Returns an ad request with custom ad targeting.
    private AdRequest createAdRequest()
    {
        return new AdRequest.Builder()
				//.AddTestDevice("FE28D26C6EC35DC2E2F340CE4ECE7165") // remove this for real ads
				.AddKeyword("game")
                .TagForChildDirectedTreatment(true)
                .Build();

    }

    #region Banner callback handlers

    public void HandleAdLoaded(object sender, EventArgs args)
    {
        print("HandleAdLoaded event received.");
    }

    public void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        print("HandleFailedToReceiveAd event received with message: " + args.Message);
    }

    public void HandleAdOpened(object sender, EventArgs args)
    {
        print("HandleAdOpened event received");
    }

    void HandleAdClosing(object sender, EventArgs args)
    {
        print("HandleAdClosing event received");
    }

    public void HandleAdClosed(object sender, EventArgs args)
    {
        print("HandleAdClosed event received");
    }

    public void HandleAdLeftApplication(object sender, EventArgs args)
    {
        print("HandleAdLeftApplication event received");
    }

    #endregion
}
