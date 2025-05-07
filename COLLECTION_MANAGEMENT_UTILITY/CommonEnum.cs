using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_UTILITY
{
    public static class CommonEnum
    {
        public enum ResponseCodes
        {
            [Description("Success")]
            Success = 200,
            InvalidRequest = 400,
            Unauthorized = 401,
            UnprocessableEntity = 422,
            InternalServerError = 500,
            AlreadyExists = 6003,
            NotFound = 6004,
            FailedToCreate = 6006,
            FailedToUpdate = 6007,
            FailedToDelete = 6008,
            UserFoundForThisRole = 6009,
            IncorrectUsernameOrPassword = 6010,
            UserNotFound = 6011,
            MenuFoundUnderThisModule = 6013,
            ModuleNotFound = 6014,
            MenuNotFound = 6015,
            RoleNotFound = 6016,
            CompanyTypeNotFound = 6017,
            CategoryNotFound = 6018,
            CompanyNotFound = 6019,
            PickupRequestFound = 6020,
            UniqueReferenceGenerateFailed = 6021,
            ProductListEmpty = 6022,
            ItemSoldForThisCheckin = 6023,
            NoRepairProductFound = 6024,
            WarehouseNotFound = 6025,
            AlreadyRepaired = 6026,
            NetPriceMismatched = 6027,
            NotAClientCompany = 6028,
            ResetPasswordTokenEmpty = 5018,
            FailedToReset = 6029,
            TwoFactorRequired = 6030,
            InvalidCode = 6031,
            EmailIsEmpty = 6002,
            DependencyFound = 6032,
            AlreadyProcessed = 6033,
            NoCheckinFound = 6034,
            UndefinedValue = 6035
        }

        public enum ContextName
        {
            Identity,
            Default
        }

        public enum CompanyType
        {
            CONTRACT_OWNER = 1,
            CLIENT = 2,
            TRANSPORTER = 3,
            SELLER = 4
        }

        public enum Status
        {
            ACTIVE = 1,
            INACTIVE = 0
        }

        public enum PrefixType
        {
            PROJECT = 1,
            WAREHOUSE = 2,
            INVOICE = 3,
            RMA = 4,
            ASSET = 5,
        }

        public enum PartnerLocation
        {
            UK = 1,
            NL = 2,
        }

        public enum CheckInStatus
        {
            [Description("OK")]
            Ok = 1,
            [Description("Somewhat OK")]
            SomeWhatOk = 2,
            [Description("Not OK")]
            NotOk = 3,
            [Description("Need to Repair")]
            NeedToRepair = 4,
            [Description("Dismantle")]
            Dismantle = 5
        }

        public enum RepairStatus
        {
            [Description("Pending")]
            Pending = 1,
            [Description("In Progress")]
            InProgress = 2,
            [Description("Dismantled")]
            Dismantled = 3,
            [Description("Repaired")]
            Repaired = 4
        }

        public enum CertificationStatus
        {
            [Description("Certified")]
            Certified = 1
        }

        public enum DismantleStatus
        {
            [Description("Dismantle")]
            Dismantle = 1,
            [Description("To Be Scrapped")]
            ToBeScrapped = 2
        }

        public enum ScrapStatus
        {
            [Description("Srapped")]
            Scrapped = 1
        }

        //also used as product_status
        public enum ProjectStatus
        {
            [Description("Pick Up")]
            PickUp = 1,
            [Description("Check In")]
            CheckIn = 2,
            [Description("Repair")]
            Repair = 3,
            [Description("Ready To Sell")]
            ReadyToSell = 4,
            [Description("Sold")]
            Sold = 5,
            [Description("Dismantled")]
            Dismantled = 6
        }

        public enum StockStatus
        {
            [Description("In Stock")]
            InStock = 1,
            [Description("Sold")]
            Sold = 2,
            [Description("Damage")]
            Damage = 3,
            [Description("Return")]
            Return = 4
        }

        public enum Gender
        {
            Male = 1,
            Female = 2,
            Others = 3
        }

        public static string GetDescription(this Enum enumValue)
        {
            return enumValue.GetType()
                       .GetMember(enumValue.ToString())
                       .First()
                       .GetCustomAttribute<DescriptionAttribute>()?
                       .Description ?? string.Empty;
        }
    }
}
