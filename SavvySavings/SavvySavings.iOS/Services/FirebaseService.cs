using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using SavvySavings.Services;
using UIKit;
using Firebase.CloudMessaging;
using Firebase;
using Firebase.Core;
using Firebase.Database;
using UserNotifications;

using SavvySavings.Models;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(SavvySavings.iOS.Services.FirebaseService))]
namespace SavvySavings.iOS.Services
{
    public class FirebaseService : ILocalFirebase<Sale>
    { 
        public static List<Sale> sales;

        public FirebaseService()
        {
            sales = new List<Sale>();

            Database db = Database.From(Firebase.Core.App.DefaultInstance);
            dataRef = db.GetRootReference();

            DatabaseReference testDir = dataRef.GetChild("currentSales");
            nuint handleReference = testDir.ObserveEvent(DataEventType.Value, (snapshot) => {
                NSEnumerator children = snapshot.Children;

                var child = children.NextObject() as DataSnapshot;

                while (child != null)
                {
                    var data = (SaleObj)child.GetValue<NSDictionary>();
                    if (data.Begin != "false")
                        sales.Add(data);

                    child = children.NextObject() as DataSnapshot;
                }
            }, (error) =>
            {
                Console.WriteLine(error);
            });
        }

        public static Firebase.Core.App app;
        public static DatabaseReference dataRef;
        public static Dictionary<String,String> _salesList;

        public async Task<IEnumerable<Sale>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(sales);
        }

        public void Init()
        {
            Console.WriteLine("Init");
        }
    }

    class SaleObj : Sale
    {
        public static explicit operator SaleObj(NSDictionary dictionary)
        {
            try
            {
                var retSale = new SaleObj
                {
                    Begin = (NSString)dictionary["begin"].ToString(),
                    Desc = (NSString)dictionary["desc"].ToString(),
                    End = (NSString)dictionary["end"].ToString(),
                    Lat = (double)(NSNumber)dictionary["lat"],
                    Lng = (double)(NSNumber)dictionary["lng"],
                    Name = (NSString)dictionary["name"].ToString(),
                    Title = (NSString)dictionary["title"].ToString()
                };
                return retSale;
            }
            catch (NullReferenceException e)
            {
                //TODO: firebase log these
                Console.WriteLine($"MY ERROR: {e}");
                return new SaleObj { Begin = "false" };
            }
            catch (InvalidCastException e)
            {
                Console.WriteLine($"MY ERROR: {e}");
                return new SaleObj { Begin = "false" };
            }           
        }
    }
}