using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace FruitCalculator
{
   class Program
   {
      public enum TJCApplications
      {
         FruitCalculator
      }

      static void Main(string[] args)
      {
         Console.WriteLine("Welcome to the Fruit Calculator!  This app calculates the cost of a fruit basket given fruit type, price, quantity and any promotions.");
         Console.WriteLine();

         try
         {
            // Get the Fruit, Promotions and Basket from a file so that if we want to change these inputs, we don't need to recompile
            Dictionary<string, double> fruit = GetFruitAndPrices();
            Dictionary<string, double> promotions = GetPromotions();
            Dictionary<string, int> basket = GetBasket();

            // if any of these lists are null, there is definitly a problem, so abort and give a message to the user.
            if (basket == null)
            {
               Logger.GetLogger().WriteEntry(TJCApplications.FruitCalculator,"There was a problem reading in the fruit in the basket.  Canceling order...",Logger.MessageType.Error);
            }
            else if (fruit == null)
            {
               Logger.GetLogger().WriteEntry(TJCApplications.FruitCalculator, "There was a problem reading in the available fruit for sale.  Canceling order...", Logger.MessageType.Error);
            }
            else if (promotions == null)
            {
               Logger.GetLogger().WriteEntry(TJCApplications.FruitCalculator, "There was a problem reading in the promotions.  Canceling order...", Logger.MessageType.Error);
            }
            else
            {
               double totalPrice = ProcessFruitBasket(fruit, promotions, basket);

               System.Text.StringBuilder sb = new System.Text.StringBuilder();
               foreach (string key in basket.Keys)
               {
                  sb.Append(basket[key] + " " + key + ", ");
               }

               Console.WriteLine();
               Logger.GetLogger().WriteEntry(TJCApplications.FruitCalculator, "This basket includes the following fruit: " + sb.ToString().Remove(sb.ToString().Length - 1).TrimEnd(), Logger.MessageType.Info);
               Console.WriteLine();
               Logger.GetLogger().WriteEntry(TJCApplications.FruitCalculator, "The total price for the basket is: $" + totalPrice.ToString("F"), Logger.MessageType.Mandatory);

               // ALL IS GOOD IN THE JUNGLE!  THE LION SLEEPS TONIGHT!
            }
         }
         catch (Exception ex)
         {
            Logger.GetLogger().WriteEntry(TJCApplications.FruitCalculator, ex.Message, Logger.MessageType.Error);
         }
      }

      /// <summary>
      /// This method will check the inputs and if all is good, it will attempt to calculate the total price and return that result
      /// </summary>
      /// <param name="fruit"></param>
      /// <param name="promotions"></param>
      /// <param name="basket"></param>
      /// <returns>double totalPrice</returns>
      private static double ProcessFruitBasket(Dictionary<string, double> fruit, Dictionary<string, double> promotions, Dictionary<string, int> basket )
      {
         double totalPrice = 0.00;

         try
         {
            // Check the inputs to make sure they are valid before proceding.  Log a message if something is amiss!
            if ( basket.Count > 0 )
            {
               if ( fruit.Count < 1)
               {
                  Logger.GetLogger().WriteEntry(TJCApplications.FruitCalculator, "There was no fruit defined.", Logger.MessageType.Error);
               }
               else
               {
                  if (promotions.Count < 1)
                  {
                     Logger.GetLogger().WriteEntry(TJCApplications.FruitCalculator, "There are no promotions for this sale.", Logger.MessageType.Info);
                  }
                  // We are good to go ahead and attempt to calculate the total price!
                  totalPrice = CalculateTotalPrice(fruit, promotions, basket);
               }
            }
            else
            {
               Logger.GetLogger().WriteEntry(TJCApplications.FruitCalculator, "There was no fruit in the basket list.", Logger.MessageType.Error);
            }
         }
         catch(Exception e)
         {
            Logger.GetLogger().WriteEntry(TJCApplications.FruitCalculator, e.Message, Logger.MessageType.Error);
         }

         return totalPrice;
      }

      /// <summary>
      /// This is the method that will actually calculate the price of the basket of fruit.  The parameters have already been checked for validity by the calling method,
      /// but we will need to check the validity of the content.
      /// </summary>
      /// <param name="fruits"></param>
      /// <param name="promotions"></param>
      /// <param name="basket"></param>
      /// <returns>The total price of the basket based on the price, quantity and deduction if there is a promotion</returns>
      private static double CalculateTotalPrice(Dictionary<string, double> fruit, Dictionary<string, double> promotions, Dictionary<string, int> basket)
      {
         double totalPrice = 0.00;

         try
         {

            // For efficency, start with the basket.  If only 3 fruit are required, don't need to go over up to 2 million different fruits with prices!
            foreach( var item in basket.Keys)
            {
               int quantity = basket[item];
               string fruitName = item;
               double discount = -1.0;
               double promotion = -1.0;
               bool hasDiscount = false;

               // ASSUMPTION:  THIS IS NOT STATED IN THE PROBLEM, BUT I AM ASSUMING THAT THE PROMOTION VALUE IS A PERCENT TAKEN OFF THE PRICE.  For example, a value of .25 means that 25% is taken
               // off of the price.  Likewise, .75 means 75% taken off the price.  Therefore, the value must be 0 <= promotion <= 1 where a promotion of 1 means the item is free.
               // A null promotion is valid here, so only worry about promotions if the promotion value is valid
               //if (promotions.Count > 0)
               //{
               try
               {
                  // This will throw an exception if we try to reference a key that doesn't exist, so we must handle that case here.
                  try
                  {
                     promotion = promotions[fruitName];

                     if ((promotion < 0.00) || (promotion > 1.00))
                     {
                        Logger.GetLogger().WriteEntry(TJCApplications.FruitCalculator, "The promotion for " + fruitName + " was not between 0 and 1.  Canceling order...", Logger.MessageType.Error);
                        totalPrice = 0.00;
                        break;
                     }
                     else
                     {
                        hasDiscount = true;
                        discount = 1 - promotion;
                        Logger.GetLogger().WriteEntry(TJCApplications.FruitCalculator, "There is a promotion for " + fruitName + " of " + (discount * 100).ToString("F") + "% off.", Logger.MessageType.Info);
                     }
                  }
                  catch(KeyNotFoundException ex)
                  {
                     Logger.GetLogger().WriteEntry(TJCApplications.FruitCalculator, "No promotion found for " + item, Logger.MessageType.Info);
                     hasDiscount = false;
                  }
               }
               catch (Exception e)
               {
                  Logger.GetLogger().WriteEntry(TJCApplications.FruitCalculator, "Error occurred getting promotion for " + fruit[fruitName] + " The error was " + e.Message, Logger.MessageType.Error);
                  totalPrice = 0.00;
                  break;
               }

               try
               {
                  // Now work with the fruit - we only need to get prices of fruit that are in the basket, a big reason I used a Dictionary
                  try
                  {
                     if (hasDiscount == true)
                     {
                        totalPrice += fruit[fruitName] * Convert.ToDouble(quantity) * discount;
                     }
                     else
                     {
                        totalPrice += fruit[fruitName] * Convert.ToDouble(quantity);
                     }
                  }
                  catch (KeyNotFoundException ex)
                  {
                     Logger.GetLogger().WriteEntry(TJCApplications.FruitCalculator, fruitName + " called for in the basket does not exist in the list of fruit.  Canceling order...", Logger.MessageType.Error);
                     totalPrice = 0.00;
                     break;
                  }
               }
               catch (Exception ex)
               {
                  Logger.GetLogger().WriteEntry(TJCApplications.FruitCalculator, "Error occurred getting price for " + fruitName + " The error was " + ex.Message, Logger.MessageType.Error);
                  totalPrice = 0.00;
                  break;
               }
            }
         }
         catch(Exception ex)
         {
            Logger.GetLogger().WriteEntry(TJCApplications.FruitCalculator, ex.Message, Logger.MessageType.Error);
         }

         return totalPrice;
      }

      private static Dictionary<string, double> GetFruitAndPrices()
      {
         Dictionary<string, double> fruit = new Dictionary<string, double>();

         try
         {
            // Read in the promotions data.  Note that I'm using "using" blocks which will clean up closing and freeing resources
            using (FileStream fs = File.OpenRead("./inputs/Fruit.txt"))
            {
               using (StreamReader sr = new StreamReader(fs, System.Text.Encoding.UTF8, true, 128))
               {
                  string line;
                  while ((line = sr.ReadLine()) != null)
                  {
                     // Beware of possible empty lines.  Further, each valid line must contain a comma
                     if (line.Contains(','))
                     {
                        string[] lineArray = line.Split(',');

                        // Handle potentially adding a duplicate key.  In this case, we'll continue on and take the one that's already there.
                        try
                        {
                           fruit.Add(lineArray[0].Trim(), Convert.ToDouble(lineArray[1].Trim()));
                        }
                        catch (ArgumentException e)
                        {
                           Logger.GetLogger().WriteEntry(TJCApplications.FruitCalculator, "A key for " + lineArray[0] + " in the list of fruit already exists.  The duplicate has been ignored and processing will continue.", Logger.MessageType.Warning);
                        }
                     }
                  }
               }
            }
         }
         catch (Exception e)
         {
            Logger.GetLogger().WriteEntry(TJCApplications.FruitCalculator, e.Message, Logger.MessageType.Error);
            fruit = null;
         }

         return fruit;
      }

      /// <summary>
      /// This will read the Promotions from a file and put them in a dictionary of the form Fruit, Discount
      /// </summary>
      /// <returns>A Dictionary of Fruit,Discount pairs</returns>
      private static Dictionary<string, double> GetPromotions()
      {
         Dictionary<string, double> promotions = new Dictionary<string, double>();

         try
         {
            // Read in the promotions data.  Note that I'm using "using" blocks which will clean up closing and freeing resources
            using (FileStream fs = File.OpenRead("./inputs/Promotions.txt"))
            {
               using (StreamReader sr = new StreamReader(fs, System.Text.Encoding.UTF8, true, 128))
               {
                  string line;
                  while ((line = sr.ReadLine()) != null)
                  {
                     string[] lineArray = line.Split(',');

                     // Handle potentially adding a duplicate key.  In this case, we'll continue on and take the one that's already there.
                     try
                     {
                        promotions.Add(lineArray[0].Trim(), Convert.ToDouble(lineArray[1].Trim()));
                     }
                     catch (ArgumentException e)
                     {
                        Logger.GetLogger().WriteEntry(TJCApplications.FruitCalculator, "A key for " + lineArray[0] + " in the promotions list already exists.  The duplicate has been ignored and processing will continue.", Logger.MessageType.Warning);
                     }
                  }
               }
            }
         }
         catch (Exception e)
         {
            Logger.GetLogger().WriteEntry(TJCApplications.FruitCalculator, e.Message, Logger.MessageType.Error);
            promotions = null;
         }

         return promotions;
      }

      /// <summary>
      /// This will read the Basket from a file and put them in a dictionary of the form Fruit, Quantity
      /// </summary>
      /// <returns>A Dictionary of Fruit, Quantity pairs</returns>
      private static Dictionary<string, int> GetBasket()
      {
         Dictionary<string, int> basket = new Dictionary<string, int>();

         try
         {
            using (FileStream fs = File.OpenRead("./inputs/Basket.txt"))
            {
               using (StreamReader sr = new StreamReader(fs, System.Text.Encoding.UTF8, true, 128))
               {
                  string line;
                  while ((line = sr.ReadLine()) != null)
                  {
                     string[] lineArray = line.Split(',');

                     // Handle potentially adding a duplicate key.  In this case, we'll continue on and take the one that's already there.
                     try
                     {
                        basket.Add(lineArray[0].Trim(), Convert.ToInt32(lineArray[1].Trim()));
                     }
                     catch (ArgumentException e)
                     {
                        Logger.GetLogger().WriteEntry(TJCApplications.FruitCalculator, "A key for " + lineArray[0] + " in the basket already exists.  The duplicate has been ignored and processing will continue.", Logger.MessageType.Warning);
                     }
                  }
               }
            }
         }
         catch(Exception ex)
         {
            Logger.GetLogger().WriteEntry(TJCApplications.FruitCalculator, ex.Message, Logger.MessageType.Error);
            basket = null;
         }

         return basket;
      }
   }
}
