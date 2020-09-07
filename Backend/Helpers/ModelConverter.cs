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
        public static ViewModels.Bond ToViewUser(this Models.DBUserBond dbUser)
        {
            return new ViewModels.Bond
            {
                Id = dbUser.Bond.Id,
                Count = dbUser.Count,
                Company = dbUser.Bond.Company,
                CouponFrequency = dbUser.Bond.CouponFrequency,
                CurrentPurchasePrice = dbUser.Bond.CurrentPurchasePrice,
                CurrentSellingPrice = dbUser.Bond.CurrentSellingPrice,
                InterestRate = dbUser.Bond.InterestRate,
                PurchaseDate = dbUser.Bond.PurchaseDate,
                PurchasePrice = dbUser.Bond.PurchasePrice,
                Risk = dbUser.Bond.Risk,
                SellingDate = dbUser.Bond.SellingDate,
                SellingPrice = dbUser.Bond.SellingPrice
            };
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
                Bonds = dbUser.Bonds.Select(bond => bond.ToViewUser())
            };
        }
    }
}
