namespace TMI_RogueLike_DataEditor.Model;

using System.Collections.Generic;
using System.Text.Json.Serialization;


public class RogueLikeData
{
    [JsonPropertyName("InitialProducts")] public InitialProduct[] InitialProducts { get; set; }

    [JsonPropertyName("RoundSetups")] public RoundSetup[] RoundSetups { get; set; }

    [JsonPropertyName("MapSetups")] public MapSetup[] MapSetups { get; set; }

    [JsonPropertyName("CookerPriceSetups")] public List<CookerPriceSetup> CookerPriceSetups { get; set; }

    [JsonPropertyName("GachaTriggerData")] public List<GachaTriggerDatum> GachaTriggerData { get; set; }

    [JsonPropertyName("GachaRewardPairData")] public List<GachaRewardPairDatum> GachaRewardPairData { get; set; }

    [JsonPropertyName("RoundDuration")] public int RoundDuration { get; set; }

    [JsonPropertyName("GlobalProductMultiplier")] public float GlobalProductMultiplier { get; set; }

    [JsonPropertyName("Level1RecipePriceMultiplier")] public float Level1RecipePriceMultiplier { get; set; }

    [JsonPropertyName("Level2RecipePriceMultiplier")] public float Level2RecipePriceMultiplier { get; set; }

    [JsonPropertyName("Level3RecipePriceMultiplier")] public float Level3RecipePriceMultiplier { get; set; }

    [JsonPropertyName("Level4RecipePriceMultiplier")] public float Level4RecipePriceMultiplier { get; set; }

    [JsonPropertyName("Level5RecipePriceMultiplier")] public float Level5RecipePriceMultiplier { get; set; }

    [JsonPropertyName("PartnerBasePrice")] public int PartnerBasePrice { get; set; }

    [JsonPropertyName("SpecialGuestPrice")] public int SpecialGuestPrice { get; set; }

    [JsonPropertyName("ClothesPrice")] public int ClothesPrice { get; set; }

    [JsonPropertyName("DecorationPrice")] public int DecorationPrice { get; set; }

    [JsonPropertyName("GachaNeedComboNum")] public int GachaNeedComboNum { get; set; }

    [JsonPropertyName("GachaNeedSpellNum")] public int GachaNeedSpellNum { get; set; }

    [JsonPropertyName("GachaMaxCardNum")] public int GachaMaxCardNum { get; set; }

    [JsonPropertyName("BeginToSpawnDangerousCardRoundIndex")] public int BeginToSpawnDangerousCardRoundIndex { get; set; }

    [JsonPropertyName("UnlockAllLevel2SpotRoundIndex")] public int UnlockAllLevel2SpotRoundIndex { get; set; }

    [JsonPropertyName("UnlockAllLevel3SpotRoundIndex")] public int UnlockAllLevel3SpotRoundIndex { get; set; }

    [JsonPropertyName("LuckyLeafExtraMultiplier")] public float LuckyLeafExtraMultiplier { get; set; }
    
    [JsonPropertyName("AyaNewsBeginRoundIndex")] public int AyaNewsBeginRoundIndex { get; set; }

    [JsonPropertyName("PriceToRefreshAyaNews")] public int PriceToRefreshAyaNews { get; set; }

    [JsonPropertyName("AkyuuWashiPrice")] public int AkyuuWashiPrice { get; set; }
        
    [JsonPropertyName("EllenCandyPrice")] public int EllenCandyPrice { get; set; }

}

public class CookerPriceSetup
{
    [JsonPropertyName("Price")] public int Price { get; set; }

    [JsonPropertyName("CookerSeries")] public string CookerSeries { get; set; }
}

public class GachaRewardPairDatum
{
    [JsonPropertyName("WholeCardNum")] public int WholeCardNum { get; set; }

    [JsonPropertyName("CardRewardRates")] public List<CardRewardRate> CardRewardRates { get; set; }
}

public class CardRewardRate
{
    [JsonPropertyName("Rarity")] public Rarity Rarity { get; set; }

    [JsonPropertyName("Rate")] public float Rate { get; set; }
}

public class GachaTriggerDatum
{
    [JsonPropertyName("TriggerCardCondition")] public string TriggerCardCondition { get; set; }

    [JsonPropertyName("InitialCardNum")] public int InitialCardNum { get; set; }

    [JsonPropertyName("AddMaxCardNumWhenTrigger")] public bool AddMaxCardNumWhenTrigger { get; set; }
}

public class InitialProduct
{
    [JsonPropertyName("productType")] public ProductType ProductType { get; set; }

    [JsonPropertyName("productId")] public int ProductId { get; set; }

    [JsonPropertyName("productLabel")] public string ProductLabel { get; set; }

    [JsonPropertyName("productAmount")] public int ProductAmount { get; set; }
}

public class MapSetup
{
    [JsonPropertyName("MapName")] public string MapName { get; set; }

    [JsonPropertyName("Ingredients")] public List<Ingredient> Ingredients { get; set; }

    [JsonPropertyName("PartnerId")] public List<int> PartnerId { get; set; }

    [JsonPropertyName("SpawnedSpecialGuestId")] public List<SpawnedSpecialGuestId> SpawnedSpecialGuestId { get; set; }

    [JsonPropertyName("SpecialGuestId")] public List<int> SpecialGuestId { get; set; }

    [JsonPropertyName("ClothesId")] public List<int> ClothesId { get; set; }

    [JsonPropertyName("DecorationId")] public List<int> DecorationId { get; set; }

    [JsonPropertyName("CookerId")] public List<int> CookerId { get; set; }

    [JsonPropertyName("RecipeId")] public List<int> RecipeId { get; set; }

    [JsonPropertyName("MerchantIngredientDiscount")] public MerchantIngredientDiscount MerchantIngredientDiscount { get; set; }
}

public class Ingredient
{
    [JsonPropertyName("IngredientId")] public int IngredientId { get; set; }

    [JsonPropertyName("Range")] public MerchantIngredientDiscount Range { get; set; }

    [JsonPropertyName("IsBeverage")] public bool IsBeverage { get; set; }
}

public class MerchantIngredientDiscount
{
    [JsonPropertyName("x")] public float X { get; set; }

    [JsonPropertyName("y")] public float Y { get; set; }
}

public class SpawnedSpecialGuestId
{
    [JsonPropertyName("m_SpecialGuestId")] public int MSpecialGuestId { get; set; }

    [JsonPropertyName("m_DependentAreaName")] public List<string> MDependentAreaName { get; set; }
}

public class RoundSetup
{
    [JsonPropertyName("IncomeRequirement")] public int IncomeRequirement { get; set; }

    [JsonPropertyName("GuestSpawnRateMultiplier")] public float GuestSpawnRateMultiplier { get; set; }
}

public enum ProductType
{
    Beverage,
    Cooker,
    Ingredient,
    Recipe
}

public enum Rarity
{
    Green,
    Blue,
    Purple,
    Gold
}
