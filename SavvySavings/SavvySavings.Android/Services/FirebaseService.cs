using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SavvySavings.Services;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using SavvySavings.Models;
using Firebase;
using Firebase.Database;
using System.Threading.Tasks;
using Java.Util;

[assembly: Xamarin.Forms.Dependency(typeof(SavvySavings.Droid.Services.FirebaseService))]
namespace SavvySavings.Droid.Services
{
    public class FirebaseService : Java.Lang.Object, ILocalFirebase<Sale>
    {

        public static List<Sale> sales;

        public static DatabaseReference currentSales;
        public static List<Sale> currentSalesList;

        public FirebaseService()
        {
            sales = new List<Sale>();
            var db = FirebaseDatabase.GetInstance(FirebaseApp.Instance);
            currentSales = db.GetReference("currentSales");

            currentSales.AddListenerForSingleValueEvent(new Listener());
        }

        /*
        public Task<bool> UpdateItemAsync(Sale sale)
        {
            var _item = items.Where((Item arg) => arg.Id == item.Id).FirstOrDefault();
            items.Remove(_item);
            items.Add(item);

            return await Task.FromResult(true);
        }
        */

        public async Task<IEnumerable<Sale>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(sales);
        }

        public void Init()
        {
            var options = new FirebaseOptions.Builder()
                 .SetApplicationId("1:1076235234949:android:82c6328d968066e7")
                 .SetApiKey("AIzaSyCu1hjwWIeuF3TTFwrqjIb2m1ghs_duDcs")
                 .SetDatabaseUrl("https://savvy-savings.firebaseio.com/")
                 .SetProjectId("savvy-savings")
                 .SetStorageBucket("savvy-savings.appspot.com")
                 .SetGcmSenderId("1076235234949")
                 .Build();

            var app = FirebaseApp.InitializeApp(Application.Context.ApplicationContext, options);
            var db = FirebaseDatabase.GetInstance(app);

            currentSales = db.GetReference("currentSales");
            Console.WriteLine(currentSales);
        }


    }
    
    public class Listener : Java.Lang.Object, IValueEventListener
    {
        //public new IntPtr Handle => throw new NotImplementedException();

        public new void Dispose()
        {
            //throw new NotImplementedException();
        }

        public void OnCancelled(DatabaseError error)
        {
            //throw new NotImplementedException();
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
           foreach (DataSnapshot child in snapshot.Children.ToEnumerable())
            {
                if ( child.HasChild("begin") && child.HasChild("desc")
                    && child.HasChild("end") && child.HasChild("lat")
                    && child.HasChild("lng") && child.HasChild("name")
                    && child.HasChild("title"))
                {
                    SaleObj childSale = (SaleObj)child;
                    if(childSale.Begin != "false")
                        FirebaseService.sales.Add(childSale);
                } else
                {
                    //TODO: log this
                }                
            }            
        }
    }

    class SaleObj : Sale
    {
        public static explicit operator SaleObj(DataSnapshot snapshot)
        {
            SaleObj retSale;
            try
            {
                retSale = new SaleObj
                {
                    Begin = snapshot.Child("begin").Value.ToString(),
                    Desc = snapshot.Child("desc").Value.ToString(),
                    End = snapshot.Child("end").Value.ToString(),
                    Lat = (Double)snapshot.Child("lat").Value,
                    Lng = (Double)snapshot.Child("lng").Value,
                    Name = snapshot.Child("name").Value.ToString(),
                    Title = snapshot.Child("title").Value.ToString()
                };
                return retSale;
            } catch (NullReferenceException e)
            {
                //TODO: log these
                Console.WriteLine(e);
                return new SaleObj { Begin = "false" };
            }
            catch (InvalidCastException e)
            {
                Console.WriteLine(e);
                return new SaleObj { Begin = "false" };
            }            
        }
    }
}