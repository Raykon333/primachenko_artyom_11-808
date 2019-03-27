using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlLinq
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<Tuple<TItem, TItem>> Task2<TItem>(this IEnumerable<TItem> sequence1,
            IEnumerable<TItem> sequence2, Func<TItem, int> function)
        {
            TItem[] array = sequence2.ToArray();
            return sequence1
                .Select(x =>
                {
                    int index = function(x);
                    if (index >= array.Length)
                        return Tuple.Create(x, default(TItem));
                    return Tuple.Create(x, array[index]);
                });
        }
    }

    //классы для третьего задания
    #region Task3Classes
    class Product //товар
    {
        public readonly string Code; //артикул
        public readonly string Category;
        public readonly string MfrCountry; //страна-производитель
    }

    class Discount
    {
        public readonly int CustomerCode;
        public readonly string Shop;
        public readonly int Value; //сама скидка
    }

    class Price
    {
        public readonly string ProductCode; //артикул товара
        public readonly string Shop;
        public readonly int Value;
    }

    class Purchase
    {
        public readonly int CustomerCode;
        public readonly string ProductCode;
        public readonly string Shop;
    }
    #endregion

    class Program
    {
        static IEnumerable<string> Task1(IEnumerable<string> A)
        {
            int i = 1;
            return A
                .Select(x => x + i++)
                .Where(x => char.IsLetter(x[0]))
                .OrderBy(x => x);
        }

        static Dictionary<Tuple<string, string>, int> Task3(IEnumerable<Product> B, IEnumerable<Discount> C,
            IEnumerable<Price> D, IEnumerable<Purchase> E)
        {
            Dictionary<Tuple<string, string>, int> result =
                new Dictionary<Tuple<string, string>, int>();
            //словарь: ключ - имя категории, значение - список кодов продукта
            Dictionary<string, List<string>> categories = B
                .GroupBy(product => product.Category)
                .ToDictionary(group => group.Key, group => group
                    .Select(product => product.Code).ToList());
            //словарь: ключ - магазин, значение - список покупок в нём
            Dictionary<string, List<Purchase>> shops = E
                .GroupBy(purchase => purchase.Shop)
                .ToDictionary(group => group.Key, group => group.ToList());
            //словарь: ключ - имя магазина и код покупателя, значение - скидка
            Dictionary<Tuple<string, int>, int> discounts = C
                .GroupBy(discount => Tuple.Create(discount.Shop, discount.CustomerCode))
                .ToDictionary(group => group.Key, group => group.First().Value);
            //словарь: ключ - имя магазина и имя продукта, значение - цена
            Dictionary<Tuple<string, string>, int> prices = D
                .GroupBy(price => Tuple.Create(price.Shop, price.ProductCode))
                .ToDictionary(group => group.Key, group => group.First().Value);
            foreach (var category in categories)
                foreach(var shop in shops)
                {
                    Tuple<string, string> id = Tuple.Create(category.Key, shop.Key);
                    result[id] = 0;
                    var purchasesByID = shop.Value
                        .Where(x => category.Value.Contains(x.ProductCode));
                    if (!purchasesByID.Any())
                        result[id] = -1;
                    else foreach (var purchase in purchasesByID)
                        result[id] += discounts[Tuple.Create(shop.Key, purchase.CustomerCode)]
                            * prices[Tuple.Create(shop.Key, purchase.ProductCode)] / 100;
                }
            return result;
        }

        static void Main(string[] args)
        {
        }
    }
}
