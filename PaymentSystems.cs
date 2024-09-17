using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;

namespace Napilnik.PaymentSystemsTask {
    public static class PaymentSystems {
        public static void Run() {
            //Выведите платёжные ссылки для трёх разных систем платежа: 
            //pay.system1.ru/order?amount=12000RUB&hash={MD5 хеш ID заказа}
            //order.system2.ru/pay?hash={MD5 хеш ID заказа + сумма заказа}
            //system3.com/pay?amount=12000&curency=RUB&hash={SHA-1 хеш сумма заказа + ID заказа + секретный ключ от системы}

            Order order = new Order(1212, 12000);

            PaymentSystem1 paymentSystem1 = new PaymentSystem1();
            PaymentSystem2 paymentSystem2 = new PaymentSystem2();
            PaymentSystem3 paymentSystem3 = new PaymentSystem3();

            var systems = new IPaymentSystem[] { paymentSystem1, paymentSystem2, paymentSystem3 };
            var paymenstLinks = GetPaymenstLinks(order, systems);
            PrintLines(paymenstLinks);
        }

        public static string[] GetPaymenstLinks(Order order, params IPaymentSystem[] systems) {
            string[] result = new string[systems.Length];
            for (int i = 0; i < systems.Length; i++) {
                result[i] = systems[i].GetPayingLink(order);
            }
            return result;
        }

        public static void PrintLines(params string[] strings) {
            foreach (var str in strings) {
                Console.WriteLine(str);
            }
        }
    }

    public class PaymentSystem1 : IPaymentSystem {
        private const string _url = "pay.system1.ru";
        private const string _currencyName = "RUB";
        private string _paylinkFormat = "{0}/order?amount={1}{2}&hash={3}";
        private Func<int, string> _getHashID = id => BitConverter.ToInt32(MD5.Create().ComputeHash(BitConverter.GetBytes(id)), 0).ToString();

        public string GetPayingLink(Order order) {
            return string.Format(_paylinkFormat, _url, order.Amount, _currencyName, _getHashID(order.Id));
        }
    }

    public class PaymentSystem2 : IPaymentSystem {
        private const string _url = "order.system2.ru";
        private string _paylinkFormat = "{0}/pay?hash={1}";
        private Func<Order, int> _getOrderHash = order => BitConverter.ToInt32(MD5.Create().ComputeHash(BitConverter.GetBytes(order.Id)), 0);

        public string GetPayingLink(Order order) {
            return string.Format(_paylinkFormat, _url, (_getOrderHash(order) + order.Amount).ToString());
        }
    }

    public class PaymentSystem3 : IPaymentSystem {
        private const string _url = "system3.com";
        private const string _currencyName = "RUB";

        private readonly int _secretKye;

        private string _paylinkFormat = "{0}/pay?amount={1}&curency={2}&hash={3}";
        private Func<Order, int> _getOrderHash;

        public PaymentSystem3() {
            _secretKye = new Random().Next(100000, 999999);
            _getOrderHash = order => {
                return BitConverter.ToInt32(SHA1.Create().ComputeHash(ObjectToByteArray(order)), 0);
            };
        }

        public string GetPayingLink(Order order) {
            return string.Format(_paylinkFormat, _url, order.Amount, _currencyName, _getOrderHash(order) + order.Id + _secretKye);
        }

        private static byte[] ObjectToByteArray(object obj) {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream()) {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
    }

    [Serializable]
    public class Order {
        public readonly int Id;
        public readonly int Amount;

        public Order(int id, int amount) => (Id, Amount) = (id, amount);
    }

    public interface IPaymentSystem {
        string GetPayingLink(Order order);
    }
}
