using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using Ensign.Unity;

// Deriving the Purchaser class from IStoreListener enables it to receive messages from Unity Purchasing.
public class IAPManager : Singleton<IAPManager>, IStoreListener
{
    private static IStoreController m_StoreController;          // The Unity Purchasing system.
    private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.
    private IAppleExtensions m_AppleExtensions;

    // Product identifiers for all products capable of being purchased: 
    // "convenience" general identifiers for use with Purchasing, and their store-specific identifier 
    // counterparts for use with and outside of Unity Purchasing. Define store-specific identifiers 
    // also on each platform's publisher dashboard (iTunes Connect, Google Play Developer Console, etc.)

    // General product identifiers for the consumable, non-consumable, and subscription products.
    // Use these handles in the code to reference which product to purchase. Also use these values 
    // when defining the Product Identifiers on the store. Except, for illustration purposes, the 
    // kProductIDSubscription - it has custom Apple and Google identifiers. We declare their store-
    // specific mapping to Unity Purchasing's AddProduct, below.

    // public static string kProductIDSubscription = "woc.remove_ads.monthly";
    [SerializeField] private string[] subscription;
    [SerializeField] private string[] itemIDs;
    [SerializeField] private string[] itemIDsConsumable;
    private string[] iapItems;
    private string[] iapItemsConsumable;
    private string[] subscriptionItem;

    private System.Action onSuccess;
    private System.Action onFail;
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        // If we haven't set up the Unity Purchasing reference
        if (m_StoreController == null)
        {
            // Begin to configure our connection to Purchasing
            InitializePurchasing(itemIDs, itemIDsConsumable, subscription);
        }
    }
    private Action onInitSuccess;
    public void InitializePurchasing(string[] iapItems,string[] itemIDsConsumable,string[] subscription, Action onInit = null)
    {
        onInitSuccess = onInit;
        // If we have already connected to Purchasing ...
        // if (IsInitialized() || Application.isEditor)
        // {
        //     onInitSuccess?.Invoke();
        //     Debug.Log("Paass");
        //     // ... we are done here.
        //     return;
        // }

        this.iapItems = iapItems;
        this.iapItemsConsumable = itemIDsConsumable;
        this.subscriptionItem = subscription;

        // Create a builder, first passing in a suite of Unity provided stores.
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        foreach (var item in subscriptionItem)
        {
            builder.AddProduct(item, ProductType.Subscription);
        }
        if (iapItems != null && itemIDsConsumable != null)
        {
            foreach (var item in iapItems)
            {
                builder.AddProduct(item, ProductType.NonConsumable);
            }
            foreach (var item in itemIDsConsumable)
            {
                builder.AddProduct(item, ProductType.Consumable);
            }
        }
        // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
        // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
        UnityPurchasing.Initialize(this, builder);

    }


    private bool IsInitialized()
    {
        // Only say we are initialized if both the Purchasing references are set.
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

    public void BuyProduct(string id, System.Action onSuccess = null, System.Action onCancel = null)
    {
        this.onSuccess = onSuccess;
        this.onFail = onCancel;
        BuyProductID(id);
    }

    public void BuySubscription()
    {
        // Buy the subscription product using its the general identifier. Expect a response either 
        // through ProcessPurchase or OnPurchaseFailed asynchronously.
        // Notice how we use the general product identifier in spite of this ID being mapped to
        // custom store-specific identifiers above.
        // BuyProductID(kProductIDSubscription);
    }

    void BuyProductID(string productId)
    {
        Debug.Log($"BuyProductID {productId} + IsInitialized { IsInitialized()}");
        // If Purchasing has been initialized ...
        //if (IsInitialized())
        // {
        // ... look up the Product reference with the general product identifier and the Purchasing 
        // system's products collection.
        Product product = m_StoreController.products.WithID(productId);

        // If the look up found a product for this device's store and that product is ready to be sold ... 
        if (product != null && product.availableToPurchase)
        {
            Debug.Log(string.Format("Purchasing product asychronously: '{0}', {1}", product.definition.id, product.metadata.localizedPriceString));
            // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
            // asynchronously.
            m_StoreController.InitiatePurchase(product);
        }
        // Otherwise ...
        else
        {
            // ... report the product look-up failure situation  
            Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase " + productId);
        }
        // }
        // Otherwise ...
        // else
        // {
        //     // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
        //     // retrying initiailization.
        //     Debug.Log("BuyProductID FAIL. Not initialized.");
        // }
    }


    // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
    // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
    public void RestorePurchases()
    {
        // If Purchasing has not yet been set up ...
        if (!IsInitialized())
        {
            // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }

        // If we are running on an Apple device ... 
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            // ... begin restoring purchases
            Debug.Log("RestorePurchases started ...");

            // Fetch the Apple store-specific subsystem.
            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
            // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
            apple.RestoreTransactions((result) =>
            {
                // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                // no purchases are available to be restored.
                Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            });
        }
        // Otherwise ...
        else
        {
            // We are not running on an Apple device. No work is necessary to restore purchases.
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }

    //  
    // --- IStoreListener
    //

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        // Purchasing has succeeded initializing. Collect our Purchasing references.
        Debug.Log("OnInitialized: PASS");
        // Overall Purchasing system, configured with products for this application.
        m_StoreController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        m_StoreExtensionProvider = extensions;

        m_AppleExtensions = extensions.GetExtension<IAppleExtensions>();

        Debug.Log("Available items:");
        Dictionary<string, string> introductory_info_dict = m_AppleExtensions.GetIntroductoryPriceDictionary();
        foreach (var item in controller.products.all)
        {
            if (item.availableToPurchase)
            {
                Debug.Log(string.Join(" - ",
                    new[]
                    {
                        item.metadata.localizedTitle,
                        item.metadata.localizedDescription,
                        item.metadata.isoCurrencyCode,
                        item.metadata.localizedPrice.ToString(),
                        item.metadata.localizedPriceString,
                        item.transactionID,
                        item.receipt
                    }));


                // this is the usage of SubscriptionManager class
                if (item.receipt != null)
                {
                    if (item.definition.type == ProductType.Subscription)
                    {
                        if (checkIfProductIsAvailableForSubscriptionManager(item.receipt))
                        {
                            string intro_json = (introductory_info_dict == null || !introductory_info_dict.ContainsKey(item.definition.storeSpecificId)) ? null : introductory_info_dict[item.definition.storeSpecificId];
                            SubscriptionManager p = new SubscriptionManager(item, intro_json);
                            SubscriptionInfo info = p.getSubscriptionInfo();
                            Debug.Log("product id is: " + info.getProductId());
                            Debug.Log("purchase date is: " + info.getPurchaseDate());
                            Debug.Log("subscription next billing date is: " + info.getExpireDate());
                            Debug.Log("is subscribed? " + info.isSubscribed().ToString());
                            Debug.Log("is expired? " + info.isExpired().ToString());
                            Debug.Log("is cancelled? " + info.isCancelled());
                            Debug.Log("product is in free trial peroid? " + info.isFreeTrial());
                            Debug.Log("product is auto renewing? " + info.isAutoRenewing());
                            Debug.Log("subscription remaining valid time until next billing date is: " + info.getRemainingTime());
                            Debug.Log("is this product in introductory price period? " + info.isIntroductoryPricePeriod());
                            Debug.Log("the product introductory localized price is: " + info.getIntroductoryPrice());
                            Debug.Log("the product introductory price period is: " + info.getIntroductoryPricePeriod());
                            Debug.Log("the number of product introductory price period cycles is: " + info.getIntroductoryPricePeriodCycles());
                            if (info.isSubscribed() == Result.True && info.isExpired() == Result.False)
                            {
                                //Removed ads
                            }
                            else
                            {
                                //Not remove ads
                            }
                        }
                        else
                        {
                            Debug.Log("This product is not available for SubscriptionManager class, only products that are purchase by 1.19+ SDK can use this class.");
                        }
                    }
                    else
                    {
                        Debug.Log("the product is not a subscription product");
                    }
                }
                else
                {
                    Debug.Log("the product should have a valid receipt");
                }

            }
        }
        onInitSuccess?.Invoke();

    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        foreach (var i in iapItems)
        {
            if (String.Equals(args.purchasedProduct.definition.id, i, StringComparison.Ordinal))
            {
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
                onSuccess?.Invoke();
                break;
            }
        }
        foreach (var i in iapItemsConsumable)
        {
            if (String.Equals(args.purchasedProduct.definition.id, i, StringComparison.Ordinal))
            {
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
                onSuccess?.Invoke();
                break;
            }
        }
        /*
        // A consumable product has been purchased by this user.
        if (item.productType == ProductType.Consumable)
        {
            
        }
        // Or ... a non-consumable product has been purchased by this user.
        else if (item.productType == ProductType.NonConsumable)
        {
            // TODO: The non-consumable item has been successfully purchased, grant this item to the player.
        }
        // Or ... a subscription product has been purchased by this user.
        else if (item.productType == ProductType.Subscription)
        {
            // TODO: The subscription item has been successfully purchased, grant this to the player.
        }
        */

        // Return a flag indicating whether this product has completely been received, or if the application needs 
        // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
        // saving purchased products to the cloud, and when that save is delayed. 
        return PurchaseProcessingResult.Complete;
    }


    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
        // this reason with the user to guide their troubleshooting actions.
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
        onFail?.Invoke();
    }

    private bool checkIfProductIsAvailableForSubscriptionManager(string receipt)
    {
        var receipt_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(receipt);
        if (!receipt_wrapper.ContainsKey("Store") || !receipt_wrapper.ContainsKey("Payload"))
        {
            Debug.Log("The product receipt does not contain enough information");
            return false;
        }
        var store = (string)receipt_wrapper["Store"];
        var payload = (string)receipt_wrapper["Payload"];

        if (payload != null)
        {
            switch (store)
            {
                case GooglePlay.Name:
                    {
                        var payload_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(payload);
                        if (!payload_wrapper.ContainsKey("json"))
                        {
                            Debug.Log("The product receipt does not contain enough information, the 'json' field is missing");
                            return false;
                        }
                        var original_json_payload_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode((string)payload_wrapper["json"]);
                        if (original_json_payload_wrapper == null || !original_json_payload_wrapper.ContainsKey("developerPayload"))
                        {
                            Debug.Log("The product receipt does not contain enough information, the 'developerPayload' field is missing");
                            return false;
                        }
                        var developerPayloadJSON = (string)original_json_payload_wrapper["developerPayload"];
                        var developerPayload_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(developerPayloadJSON);
                        if (developerPayload_wrapper == null || !developerPayload_wrapper.ContainsKey("is_free_trial") || !developerPayload_wrapper.ContainsKey("has_introductory_price_trial"))
                        {
                            Debug.Log("The product receipt does not contain enough information, the product is not purchased using 1.19 or later");
                            return false;
                        }
                        return true;
                    }
                case AppleAppStore.Name:
                case AmazonApps.Name:
                case MacAppStore.Name:
                    {
                        return true;
                    }
                default:
                    {
                        return false;
                    }
            }
        }
        return false;
    }

    public string GetPrice(string productId)
    {
        return m_StoreController.products.WithID(productId).metadata.localizedPriceString;
    }

    public bool CheckProduct(string productId)
    {
        var product = m_StoreController?.products?.WithID(productId);
        return product != null && product.availableToPurchase;
    }

    public bool HasReceipt(string productId)
    {
        var product = m_StoreController?.products?.WithID(productId);
        return product != null && product.hasReceipt;
    }
}