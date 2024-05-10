using Accord.Math;
using Accord.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        var allFavList = new List<List<string>>
        {
            new List<string> { "Cheese", "Butter", "Milk","Papaya", "Yogurt","Orange", "Banana" },
            new List<string> { "Tomato",  "Chicken","Pineapple", "Watermelon", "Fish", "Egg", "Pork" },
            new List<string> { "Fish","Beef", "Canned Fruit", "Canned Soup",   "Mango", "Chocolate", "Candy", "Donuts" },
            new List<string> { "Fish", "Chicken", "Beef", "Quinoa", "Chocolate", "Donuts", "Bread", "Hazelnuts" },
            new List<string> { "Rice", "Noodle", "Pasta", "Bread", "Egg" },
            new List<string> { "Tomato", "Garlic", "Energy Drink" },
        };

        var currentFavList = new List<string> { "Fish", "Beef", "Milk", "Pasta", "Bread", "Egg" };

        // Add current user's fav list to all fav lists
        allFavList.Add(currentFavList);

        // Step 1: Create DataFrame
        var allProducts = allFavList.SelectMany(x => x).Distinct().ToList();
        var df = allFavList.Select(favList => allProducts.Select(product => favList.Contains(product) ? 1 : 0).ToArray()).ToArray();

        // Step 2: Compute cosine similarities
        var currentUser = df.Last();
        var similarities = df.Take(df.Length - 1).Select(user => Distance.Cosine(currentUser, user)).ToArray();

        // Step 3: Compute weighted sums
        var weightedSums = df.Take(df.Length - 1).Select((user, i) => user.Select(x => x * similarities[i]).ToArray()).Aggregate((a, b) => a.Zip(b, (x, y) => x + y).ToArray());

        // Step 4: Recommend products
        var currentFavSet = new HashSet<string>(currentFavList);
        var sortedSums = allProducts.Zip(weightedSums, (product, weight) => new { Product = product, Weight = weight }).OrderByDescending(x => x.Weight).ToList();
        var recommendations = sortedSums.Where(x => !currentFavSet.Contains(x.Product)).Take(3).Select(x => x.Product).ToList();

        Console.WriteLine(string.Join(", ", recommendations));
    }
}