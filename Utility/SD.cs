using System;
using System.Collections.Generic;
using System.Text;

namespace Utility
{
    public static class SD
    {
        public const string Usp_CoverType_GetAll = "usp_GetCoverTypes";
        public const string Usp_CoverType_Get = "usp_GetCoverType";
        public const string Usp_CoverType_Update = "usp_UpdateCoverType";
        public const string Usp_CoverType_Delete = "usp_DeleteCoverType";
        public const string Usp_CoverType_Create = "usp_CreateCoverType";


        public const string Role_User_Indiv = "Individual Customer";
        public const string Role_User_Comp = "Company Customer";
        public const string Role_Admin = "Admin";
        public const string Role_Employee = "Employee";

        public const string Session_Cart_count = "Shopping Cart Count";


        public const string StatusPending = "Pending";
        public const string StatusApproved = "Approved";
        public const string StatusInProcess = "Processing";
        public const string StatusShipped = "Shipped";
        public const string StatusCancelled = "Cancelled";
        public const string StatusRefunded = "Refunded";

        public const string PaymentStatusPending = "Pending";
        public const string PaymentStatusApproved = "Approved";
        public const string PaymentStatusDelayedPayment = "ApprovedForDelayedPayment";
        public const string PaymentStatusRejected = "Rejected";



        public static double GetPriceDependingOnQuantity(int quantity, double price, double price50, double price100)
        {
            if (quantity < 49)
                return price; 
            else if (quantity < 99)
                return price50;
            else
                return price100;
        }

        public static string ConvertToRawHtml(string source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }


    }
} 
