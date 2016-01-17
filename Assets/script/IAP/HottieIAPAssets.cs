using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
  This class handles the different assets available for in app purchases.
  It is relatively simple as there only exists two products: a small and large
  pack of hints.

  Both products are consumeable.
*/

namespace Soomla.Store{
  public class HottieIAPAssets : IStoreAssets {

    /** IStoreAssets interface methods **/
    public int GetVersion() {
			return 1;
		}

    public VirtualCurrency[] GetCurrencies() {
			return new VirtualCurrency[]{};
		}

    public VirtualGood[] GetGoods() {
			return new VirtualGood[]{HINTS_SMALL_PACK, HINTS_LARGE_PACK};
		}

    public VirtualCurrencyPack[] GetCurrencyPacks() {
			return new VirtualCurrencyPack[]{};
		}

    public VirtualCategory[] GetCategories() {
			return new VirtualCategory[]{};
		}

    /** static final members **/
    public const string HINTS_SMALL_PRODUCT_ID = "com.huusmann.hottiesworld.smallhintpack5hints"; //Must match the InApp purchase ID on GooglePlay/AppStore
    public const string HINTS_LARGE_PRODUCT_ID = "com.huusmann.hottiesworld.largehintpack20hints";

    /** Virtual Goods **/
    public static VirtualGood HINTS_SMALL_PACK = new SingleUseVG(
      "Small hints pack",
      "This gives 5 hints",
      "hints_small_id",
      new PurchaseWithMarket(HINTS_SMALL_PRODUCT_ID, 8)
    );

    public static VirtualGood HINTS_LARGE_PACK = new SingleUseVG(
      "Large hints pack",
      "This gives 20 hints",
      "hints_large_id",
      new PurchaseWithMarket(HINTS_LARGE_PRODUCT_ID, 25)
    );

  }
}
