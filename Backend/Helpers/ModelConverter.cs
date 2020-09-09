using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Helpers.Extension
{
    public static class ModelConverter
    {
        public static void UpdateDBUser(this Models.DBUser dbUser, ViewModels.User viewUser)
        {
            dbUser.IIA = viewUser.IIA;
            dbUser.Money = viewUser.Money;
            dbUser.Risk = viewUser.Risk;
            dbUser.MonthlyInvestment = viewUser.MonthlyInvestment;
            dbUser.Reinvestment = viewUser.Reinvestment;

        }

        public static void UpdateDBBond(this Models.DBBond dbBond, ViewModels.Bond viewBond)
        {
            dbBond.Company = viewBond.Company;
            dbBond.PurchasePrice = viewBond.PurchasePrice;
            dbBond.PurchaseDate = viewBond.PurchaseDate;
            dbBond.SellingPrice = viewBond.SellingPrice;
            dbBond.SellingDate = viewBond.SellingDate;
            dbBond.CurrentPurchasePrice = viewBond.CurrentPurchasePrice;
            dbBond.CurrentSellingPrice = viewBond.CurrentSellingPrice;
            dbBond.Risk = viewBond.Risk;
            dbBond.InterestRate = viewBond.InterestRate;
            dbBond.CouponFrequency = viewBond.CouponFrequency;
        }

        public static ViewModels.Bond ToViewBond(this Models.DBBond dbUser)
        {
            return new ViewModels.Bond
            {
                Id = dbUser.Id,
                Company = dbUser.Company,
                CouponFrequency = dbUser.CouponFrequency,
                CurrentPurchasePrice = dbUser.CurrentPurchasePrice,
                CurrentSellingPrice = dbUser.CurrentSellingPrice,
                InterestRate = dbUser.InterestRate,
                PurchaseDate = dbUser.PurchaseDate,
                PurchasePrice = dbUser.PurchasePrice,
                Risk = dbUser.Risk,
                SellingDate = dbUser.SellingDate,
                SellingPrice = dbUser.SellingPrice
            };
        }
        public static ViewModels.Bond ToViewBond(this Models.DBUserBond dbUserBond)
        {
            var bond = dbUserBond.Bond.ToViewBond();
            bond.Count = dbUserBond.Count;
            return bond;
        }
        public static ViewModels.User ToViewUser(this Models.DBUser dbUser)
        {
            return new ViewModels.User
            {
                Id = dbUser.Id,
                IIA = dbUser.IIA,
                Money = dbUser.Money,
                Risk = dbUser.Risk,
                MonthlyInvestment = dbUser.MonthlyInvestment,
                Reinvestment = dbUser.Reinvestment,
                Bonds = dbUser.Bonds.Select(bond => bond.ToViewBond())
            };
        }
    }
}
