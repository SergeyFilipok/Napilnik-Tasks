﻿using System;
using System.Collections.Generic;
using System.Linq;

public class Store {
    public void Main() {
        Good iPhone12 = new Good("IPhone 12");
        Good iPhone11 = new Good("IPhone 11");

        Warehouse warehouse = new Warehouse();

        Shop shop = new Shop(warehouse);

        warehouse.Delive(iPhone12, 10);
        warehouse.Delive(iPhone11, 1);

        //Вывод всех товаров на складе с их остатком
        DisplayGoods(warehouse.GetType().Name, warehouse.Goods);

        Cart cart = shop.Cart();
        cart.Add(iPhone12, 4);
        cart.Add(iPhone11, 3); //при такой ситуации возникает ошибка так, как нет нужного количества товара на складе

        //Вывод всех товаров в корзине
        DisplayGoods(cart.GetType().Name, cart.Goods);

        Console.WriteLine(cart.Order().Paylink);
        Console.ReadLine();
    }

    private void DisplayGoods(string sourceName, IEnumerable<(Good good, int count)> goods) {
        Console.WriteLine(sourceName);
        foreach (var pair in goods) {
            Console.WriteLine($"Name: {pair.good.Name} - Count: {pair.count}");
        }
    }
}

public class Good {
    public readonly string Name;

    public Good(string name) {
        if (string.IsNullOrEmpty(name)) {
            throw new ArgumentNullException("Name is NULL or EMPTY");
        }
        Name = name;
    }
}

public class Warehouse {
    private readonly Dictionary<Good, int> _goods = new Dictionary<Good, int>();

    public IEnumerable<(Good good, int count)> Goods => _goods.Select(pair => (pair.Key, pair.Value));

    public void Delive(Good good, int count) {
        _goods.AddCount((good, count), (pair) => true);
    }

    public bool HasGoods(Good good, int count) {
        if (count <= 0 || good == null) {
            throw new InvalidOperationException();
        }

        var hasGood = _goods.TryGetValue(good, out int currentCount);
        var isValidCount = hasGood && currentCount >= count;

        return hasGood && isValidCount;
    }

    public void ShippingOut(IEnumerable<(Good good, int count)> goods) {
        foreach (var pair in goods) {
            if (HasGoods(pair.good, pair.count)) {
                var currentCount = _goods[pair.good];
                var passedCount = currentCount - pair.count;
                if (passedCount <= 0) {
                    _goods.Remove(pair.good);
                }
                else {
                    _goods[pair.good] = passedCount;
                }
            }
            else {
                throw new InvalidOperationException("Not enough goods in warehouse");
            }
        }
    }
}

public class Shop {
    private readonly Warehouse _warehouse;

    public Shop(Warehouse warehouse) {
        if (warehouse == null) {
            throw new ArgumentNullException("Warehouse is NULL");
        }
        _warehouse = warehouse;
    }

    public Warehouse Warehouse => _warehouse;

    public Cart Cart() {
        return new Cart(this);
    }
}

public class Cart {
    private readonly Shop _shop;
    private readonly Dictionary<Good, int> _goods = new Dictionary<Good, int>();
    private readonly Predicate<(Good, int)> addCondition;

    private Order _order;

    public Cart(Shop shop) {
        if (shop == null) {
            throw new ArgumentNullException("Shop is NULL");
        }
        _shop = shop;
        addCondition = (pair) => _shop.Warehouse.HasGoods(pair.Item1, pair.Item2);
    }

    public IEnumerable<(Good good, int count)> Goods => _goods.Select(pair => (pair.Key, pair.Value));

    public void Add(Good good, int count) {
        _goods.AddCount((good, count), addCondition);

        if (_order != null) {
            _order = null;
        }
    }

    public Order Order() {
        if (_order == null) {
            _order = new Order();
            _shop.Warehouse.ShippingOut(Goods);
            _goods.Clear();
        }
        return _order;
    }
}

public class Order {
    public readonly string Paylink;

    public Order() {
        Paylink = GeneratePaylink();
    }

    private string GeneratePaylink() {
        return Guid.NewGuid().ToString();
    }
}

public static class DictionatyExtension {
    public static void AddCount<T>(this Dictionary<T, int> dictionary, (T item, int count) pair, Predicate<(T, int)> condition) where T : class {
        if (pair.count <= 0 || pair.item == null) {
            throw new InvalidOperationException();
        }

        dictionary.TryGetValue(pair.item, out int currentCount);
        var totalCount = currentCount + pair.count;
        if (condition((pair.item, totalCount))) {
            dictionary[pair.item] = totalCount;
        }
        else {
            throw new ArgumentOutOfRangeException("Conditon is false");
        }
    }
}