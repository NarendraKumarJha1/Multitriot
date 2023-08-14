// using System.Collections;
using UnityEngine;
// using VoxelBusters.NativePlugins;

public class GSF_InAppController : MonoBehaviour
{

    // 	public static GSF_InAppController Instance { get; private set; }

    // 	private BillingProduct[] m_products;
    // 	private bool m_productRequestFinished = false;

    // 	[System.Serializable]
    // 	public class StoreItem {
    // 		public string ItemIdentifier;
    // 		public int CoinsAmount;
    // 	}

    // 	[Header ("Product Ids")]
    // 	public string RemoveAdsIdentifier;
    // 	public string unlockAllLevelsIdentifier;
    // 	public string unlockEverythingIdentifier;

    // 	// public string[] vehiclesIds;

    // 	[Header ("Store Items")]
    // 	public StoreItem[] StoreItems;

    // 	public System.Action onPurchaseComplete;

    // 	void Awake () {

    // 		if (Instance != null) {
    // 			DestroyImmediate (gameObject);
    // 			return;
    // 		}
    // 		Instance = this;
    // 		DontDestroyOnLoad (gameObject);

    // 		// Intialise
    // #if USES_BILLING
    // 		m_products = NPSettings.Billing.Products;
    // 		m_productRequestFinished = false;
    // 		RequestBillingProducts (m_products);
    // #endif
    // 	}

    // #if USES_BILLING
    // 	void OnEnable () {
    // 		// Register for callbacks
    // 		Billing.DidFinishRequestForBillingProductsEvent += OnDidFinishRequestForBillingProducts;
    // 		Billing.DidFinishProductPurchaseEvent += OnDidFinishProductPurchase;
    // 		Billing.DidFinishRestoringPurchasesEvent += OnDidFinishRestoringPurchases;
    // 	}

    // 	void OnDisable () {
    // 		// Deregister for callbacks
    // 		Billing.DidFinishRequestForBillingProductsEvent -= OnDidFinishRequestForBillingProducts;
    // 		Billing.DidFinishProductPurchaseEvent -= OnDidFinishProductPurchase;
    // 		Billing.DidFinishRestoringPurchasesEvent -= OnDidFinishRestoringPurchases;
    // 	}
    // #endif

    // 	private void OnDidFinishRequestForBillingProducts (BillingProduct[] _products, string _error) {
    // 		if (_products != null) {
    // 			m_productRequestFinished = true;
    // 		}
    // 	}

    // 	private void OnDidFinishProductPurchase (BillingTransaction _transaction) {
    // 		if (_transaction.VerificationState == eBillingTransactionVerificationState.SUCCESS) {
    // 			if (_transaction.TransactionState == eBillingTransactionState.PURCHASED) {

    // 				if (_transaction.ProductIdentifier.Equals (RemoveAdsIdentifier)) {
    // 					SaveData.Instance.RemoveAds = true;
    // 					GSF_SaveLoad.SaveProgress ();
    // 					GSF_AdsManager.RemoveAdvertisements ();
    // 					NPBinding.UI.ShowAlertDialogWithSingleButton ("Congratulations", "All advertisemetns have been removed !", "Ok", null);
    // 				} else {
    // 					if (StoreItems.Length > 0) {
    // 						for (int i = 0; i < StoreItems.Length; i++) {
    // 							if (_transaction.ProductIdentifier.Equals (StoreItems[i].ItemIdentifier)) {
    // 								SaveData.Instance.Coins += StoreItems[i].CoinsAmount;
    // 								GSF_SaveLoad.SaveProgress ();
    // 								NPBinding.UI.ShowAlertDialogWithSingleButton ("Congratulations", StoreItems[i].CoinsAmount + " Coins Added to Your Inventory !", "Ok", null);
    // 							}
    // 						}
    // 					} else {
    // 						Debug.LogError ("Product identifier" + _transaction.ProductIdentifier + " does not exist in InAppController !");
    // 					}
    // 				}
    // 				if (onPurchaseComplete != null) {
    // 					onPurchaseComplete.Invoke ();
    // 					onPurchaseComplete = null;
    // 				}
    // 			} else if (_transaction.TransactionState == eBillingTransactionState.FAILED) {
    // 				NPBinding.UI.ShowAlertDialogWithSingleButton ("Error", "Failed to purchase item ! Please try again later", "Ok", null);
    // 			}
    // 		}
    // 	}

    // 	private void OnDidFinishRestoringPurchases (BillingTransaction[] _transactions, string _error) {
    // 		bool isSomethingRestored = false;
    // 		if (_error == null) {
    // 			if (_transactions.Length > 0) {
    // 				foreach (BillingTransaction _eachTransaction in _transactions) {
    // 					if (_eachTransaction.VerificationState == eBillingTransactionVerificationState.SUCCESS) {
    // 						if (_eachTransaction.ProductIdentifier.Equals (RemoveAdsIdentifier)) {
    // 							SaveData.Instance.RemoveAds = true;
    // 							GSF_SaveLoad.SaveProgress ();
    // 							GSF_AdsManager.RemoveAdvertisements ();
    // 							isSomethingRestored = true;
    // 						} else if (_eachTransaction.ProductIdentifier.Equals (unlockAllLevelsIdentifier)) {
    // 							Helper.UnlockAllLevels ();
    // 							isSomethingRestored = true;
    // 						} else if (_eachTransaction.ProductIdentifier.Equals (unlockEverythingIdentifier)) {
    // 							Helper.UnlockEverything ();
    // 							isSomethingRestored = true;
    // 						} 
    // 						// else {
    // 						// 	for (int i = 0; i < vehiclesIds.Length; i++) {
    // 						// 		if (_eachTransaction.ProductIdentifier.Equals (vehiclesIds[i])) {
    // 						// 			SaveData.Instance.players[i + 1].Locked = false; // 1st vehicle is always unlocked
    // 						// 		}
    // 						// 	}
    // 						// 	GSF_SaveLoad.SaveProgress ();
    // 						// 	isSomethingRestored = true;
    // 						// }
    // 					}
    // 				}

    // 				if (isSomethingRestored)
    // 					NPBinding.UI.ShowAlertDialogWithSingleButton ("Restore Successful", "All of your purchases have been restored!", "Ok", null);

    // 			} else {
    // 				NPBinding.UI.ShowAlertDialogWithSingleButton ("Alert", "No Restoreable Items Found !", "Ok", null);
    // 			}
    // 		}
    // 	}

    // 	private BillingProduct GetCurrentProduct (int id) {
    // 		return m_products[id];
    // 	}

    // 	private void BuyProduct (BillingProduct _product) {
    // #if USES_BILLING
    // 		NPBinding.Billing.BuyProduct (_product);
    // #endif
    // 	}

    // 	private void RequestBillingProducts (BillingProduct[] _products) {
    // #if USES_BILLING
    // 		NPBinding.Billing.RequestForBillingProducts (_products);
    // #endif
    // 	}

    // #if USES_BILLING
    // 	private bool IsProductPurchased (BillingProduct _product) {
    // 		return NPBinding.Billing.IsProductPurchased (_product);
    // 	}
    // #endif

    // 	public void BuyInAppProduct (int item_id) {
    // 		BuyProduct (GetCurrentProduct (item_id));
    // 	}

    // 	public void RestorePurchases () {
    // #if USES_BILLING
    // 		NPBinding.Billing.RestorePurchases ();
    // #endif
    // 	}
}