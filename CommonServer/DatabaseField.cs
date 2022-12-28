namespace BankServer
{

    public class CompanyDatabase
    {
        public const string TABLE_NAME = "Company";
        public const string ID_FIELD = "Id";
        public const string NAME_FIELD = "Name_";
        public const string ACRONYM_FIELD = "Acronym";
        public const string PRICEATOPENING_FIELD = "PriceAtOpening";
        public const string CURRENTPRICE_FIELD = "CurrentPrice";
    }

    public class CompanySectorDatabase
    {
        public const string TABLE_NAME = "CompanySector";
        public const string ID_FIELD = "Id";
        public const string COMPANYID_FIELD = "CompanyId";
        public const string SECTORID_FIELD = "SectorId";
    }

    public class PlayerMoneyDatabase
    {
        public const string TABLE_NAME = "PlayerMoney";
        public const string ID_FIELD = "Id";
        public const string CASH_FIELD = "Cash";
        public const string CURRENTACCOUNT_FIELD = "CurrentAccount";
        public const string SAVING_FIELD = "Savings";
        public const string LIFEINSURANCE_FIELD = "LifeInsurance";
    }

    public class StockDatabase
    {
        public const string TABLE_NAME = "Stock";
        public const string ID_FIELD = "Id";
        public const string PLAYERID_FIELD = "PlayerId";
        public const string COMPANYID_FIELD = "CompanyId";
        public const string TYPE_FIELD = "Type_";
        public const string AMOUNT_FIELD = "Amount";
        public const string SIMULATEDAMOUNT_FIELD = "SimulatedAmount";
        public const string AVERAGEBYVALUE_FIELD = "AverageByValue";
    }

    public class TransactionDatabase
    {
        public const string TABLE_NAME = "Transaction_";
        public const string ID_FIELD = "Id";
        public const string SOURCE_FIELD = "Source";
        public const string TARGET_FIELD = "Target";
        public const string TYPE_FIELD = "Type_";
        public const string AMOUNT_FIELD = "Amount";
        public const string DATE_FIELD = "Date_";
        public const string SOURCEACCOUNT_FIELD = "SourceAccount";
        public const string TARGETACCOUNT_FIELD = "TargetAccount";
    }

    public class TrendDatabase
    {
        public const string TABLE_NAME = "Trend";
        public const string ID_FIELD = "Id";
        public const string START_FIELD = "Start_";
        public const string DURATION_FIELD = "Duration";
        public const string IMPORTANCE_FIELD = "Importance";
        public const string SECTOR_FIELD = "Sector";
    }

    public class PlayerInfoDatabase
    {
        public const string TABLE_NAME = "PlayerInfo";
        public const string ID_FIELD = "Id";
        public const string STEAMID_FIELD = "SteamId";
    }
}