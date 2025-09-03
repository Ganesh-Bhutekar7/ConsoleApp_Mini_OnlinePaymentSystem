using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AdvancedOnlinePaymentSystem
{
    // ------------------ Enums ------------------
    enum PaymentStatus { Pending, Success, Failed }

    // ------------------ User Class ------------------
    class User
    {
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string BankName { get; set; }
        public string BankAccountNumber { get; set; }
        public string IFSC { get; set; }
        public List<Payment> PaymentHistory { get; set; } = new List<Payment>();
    }

    // ------------------ Base Payment Class ------------------
    class Payment
    {
        public string PaymentId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        public Payment()
        {
            PaymentId = Guid.NewGuid().ToString();
            PaymentDate = DateTime.Now;
        }

        public virtual void ProcessPayment()
        {
            Console.WriteLine("Processing generic payment...");
        }

        public void LogTransaction(User user)
        {
            string log = $"{DateTime.Now} | {user.PhoneNumber} | {GetType().Name} | Amount: ₹{Amount} | Status: {Status} | ID: {PaymentId}\n";
            File.AppendAllText("PaymentLog.txt", log);
        }
    }

    // ------------------ Interface ------------------
    interface IReceiptGenerator
    {
        void GenerateReceipt();
    }

    // ------------------ Abstract Transaction ------------------
    abstract class OnlineTransaction
    {
        public abstract bool ValidateTransaction();
    }

    // ------------------ Derived Payment Classes ------------------
    class CreditCardPayment : Payment, IReceiptGenerator
    {
        public string CardNumber { get; set; }

        public override void ProcessPayment()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nProcessing Credit Card Payment: ₹{Amount}");
            Console.WriteLine($"Card Number: **** **** **** {CardNumber.Substring(CardNumber.Length - 4)}");
            Status = PaymentStatus.Success;
            Console.ResetColor();
        }

        public void GenerateReceipt()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n===== CREDIT CARD RECEIPT =====");
            Console.WriteLine($"Payment ID: {PaymentId}");
            Console.WriteLine($"Date: {PaymentDate}");
            Console.WriteLine($"Amount: ₹{Amount}");
            Console.WriteLine($"Card Number: **** **** **** {CardNumber.Substring(CardNumber.Length - 4)}");
            Console.WriteLine($"Status: {Status}");
            Console.WriteLine("===============================\n");
            Console.ResetColor();
        }
    }

    class PayPalPayment : Payment, IReceiptGenerator
    {
        public string Email { get; set; }

        public override void ProcessPayment()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nProcessing PayPal Payment: ₹{Amount}");
            Console.WriteLine($"PayPal Account: {Email}");
            Status = PaymentStatus.Success;
            Console.ResetColor();
        }

        public void GenerateReceipt()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n===== PAYPAL RECEIPT =====");
            Console.WriteLine($"Payment ID: {PaymentId}");
            Console.WriteLine($"Date: {PaymentDate}");
            Console.WriteLine($"Amount: ₹{Amount}");
            Console.WriteLine($"PayPal Email: {Email}");
            Console.WriteLine($"Status: {Status}");
            Console.WriteLine("==========================\n");
            Console.ResetColor();
        }
    }

    class UPIPayment : Payment, IReceiptGenerator
    {
        public string UPIId { get; set; }

        public override void ProcessPayment()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nProcessing UPI Payment: ₹{Amount}");
            Console.WriteLine($"UPI ID: {UPIId}");
            Status = PaymentStatus.Success;
            Console.ResetColor();
        }

        public void GenerateReceipt()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n===== UPI RECEIPT =====");
            Console.WriteLine($"Payment ID: {PaymentId}");
            Console.WriteLine($"Date: {PaymentDate}");
            Console.WriteLine($"Amount: ₹{Amount}");
            Console.WriteLine($"UPI ID: {UPIId}");
            Console.WriteLine($"Status: {Status}");
            Console.WriteLine("=======================\n");
            Console.ResetColor();
        }
    }

    // ------------------ Transaction Validation ------------------
    class CreditCardTransaction : OnlineTransaction
    {
        public string CardNumber { get; set; }
        public override bool ValidateTransaction() => CardNumber.Length == 16 && CardNumber.All(Char.IsDigit);
    }

    class PayPalTransaction : OnlineTransaction
    {
        public string Email { get; set; }
        public override bool ValidateTransaction() => Email.Contains("@") && Email.Contains(".");
    }

    class UPITransaction : OnlineTransaction
    {
        public string UPIId { get; set; }
        public override bool ValidateTransaction() => UPIId.Contains("@");
    }

    // ------------------ Program ------------------
    class Program
    {
        static List<User> users = new List<User>();
        static User loggedInUser = null;

        static void Main(string[] args)
        {
            while (true)
            {
                if (loggedInUser == null)
                    ShowAuthMenu();
                else
                    ShowUserMenu();
            }
        }

        // ------------------ Auth Menu ------------------
        static void ShowAuthMenu()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n=== Online Payment System ===");
            Console.WriteLine("1. Register");
            Console.WriteLine("2. Login");
            Console.WriteLine("3. Exit");
            Console.ResetColor();
            Console.Write("Choose option: ");
            string option = Console.ReadLine();

            switch (option)
            {
                case "1": Register(); break;
                case "2": Login(); break;
                case "3": Environment.Exit(0); break;
                default: Console.WriteLine("Invalid option!"); break;
            }
        }

        static void Register()
        {
            Console.Write("Enter Phone Number: ");
            string phone = Console.ReadLine();
            Console.Write("Enter Email: ");
            string email = Console.ReadLine();
            Console.Write("Enter Password: ");
            string password = ReadPassword();
            Console.Write("Bank Name: ");
            string bankName = Console.ReadLine();
            Console.Write("Bank Account Number: ");
            string accNo = Console.ReadLine();
            Console.Write("IFSC Code: ");
            string ifsc = Console.ReadLine();

            var user = new User
            {
                PhoneNumber = phone,
                Email = email,
                Password = password,
                BankName = bankName,
                BankAccountNumber = accNo,
                IFSC = ifsc
            };
            users.Add(user);
            Console.WriteLine("✅ Registration successful! You can login now.");
        }

        static void Login()
        {
            Console.Write("Enter Phone Number: ");
            string phone = Console.ReadLine();
            Console.Write("Enter Password: ");
            string password = ReadPassword();

            var user = users.FirstOrDefault(u => u.PhoneNumber == phone && u.Password == password);
            if (user != null)
            {
                loggedInUser = user;
                Console.WriteLine($"✅ Welcome {loggedInUser.PhoneNumber}!");
            }
            else Console.WriteLine("❌ Invalid credentials!");
        }

        static string ReadPassword()
        {
            StringBuilder sb = new StringBuilder();
            while (true)
            {
                ConsoleKeyInfo c = Console.ReadKey(true);
                if (c.Key == ConsoleKey.Enter) break;
                if (c.Key == ConsoleKey.Backspace && sb.Length > 0)
                {
                    sb.Remove(sb.Length - 1, 1);
                    Console.Write("\b \b");
                }
                else if (!char.IsControl(c.KeyChar))
                {
                    sb.Append(c.KeyChar);
                    Console.Write("*");
                }
            }
            Console.WriteLine();
            return sb.ToString();
        }

        // ------------------ User Menu ------------------
        static void ShowUserMenu()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n=== Welcome {loggedInUser.PhoneNumber} ===");
            Console.WriteLine("1. Credit Card Payment");
            Console.WriteLine("2. PayPal Payment");
            Console.WriteLine("3. UPI Payment");
            Console.WriteLine("4. View Payment History");
            Console.WriteLine("5. View Profile");
            Console.WriteLine("6. Logout");
            Console.ResetColor();
            Console.Write("Choose option: ");
            string option = Console.ReadLine();

            Payment payment = null;
            switch (option)
            {
                case "1":
                    payment = new CreditCardPayment();
                    MakePayment(payment);
                    break;
                case "2":
                    payment = new PayPalPayment();
                    MakePayment(payment);
                    break;
                case "3":
                    payment = new UPIPayment();
                    MakePayment(payment);
                    break;
                case "4":
                    ShowPaymentHistory();
                    break;
                case "5":
                    ShowProfile();
                    break;
                case "6":
                    loggedInUser = null;
                    Console.WriteLine("Logged out successfully.");
                    break;
                default: Console.WriteLine("Invalid option!"); break;
            }
        }

        static void MakePayment(Payment payment)
        {
            Console.Write("Enter amount (max ₹5000): ");
            payment.Amount = SafeDecimalInput();
            if (payment.Amount > 5000)
            {
                Console.WriteLine("❌ Amount exceeds ₹5000 limit!");
                return;
            }

            if (payment is CreditCardPayment cc)
            {
                Console.Write("Enter Card Number: "); cc.CardNumber = Console.ReadLine();
                if (!new CreditCardTransaction { CardNumber = cc.CardNumber }.ValidateTransaction())
                { Console.WriteLine("Invalid card number!"); return; }
            }
            else if (payment is PayPalPayment pp)
            {
                Console.Write("Enter PayPal Email: "); pp.Email = Console.ReadLine();
                if (!new PayPalTransaction { Email = pp.Email }.ValidateTransaction())
                { Console.WriteLine("Invalid PayPal email!"); return; }
            }
            else if (payment is UPIPayment upi)
            {
                Console.Write("Enter UPI ID: "); upi.UPIId = Console.ReadLine();
                if (!new UPITransaction { UPIId = upi.UPIId }.ValidateTransaction())
                { Console.WriteLine("Invalid UPI ID!"); return; }
            }

            ConfirmAndProcess(payment);
            loggedInUser.PaymentHistory.Add(payment);
        }

        static void ConfirmAndProcess(Payment payment)
        {
            Console.Write("Confirm payment? (Y/N): ");
            if (Console.ReadLine().ToUpper() == "Y")
            {
                payment.ProcessPayment();
                if (payment is IReceiptGenerator receipt) receipt.GenerateReceipt();
                payment.LogTransaction(loggedInUser);
            }
            else
            {
                Console.WriteLine("Payment cancelled.");
                payment.Status = PaymentStatus.Failed;
            }
        }

        static void ShowProfile()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n=== User Profile ===");
            Console.WriteLine($"Phone Number: {loggedInUser.PhoneNumber}");
            Console.WriteLine($"Email: {loggedInUser.Email}");
            Console.WriteLine($"Bank Name: {loggedInUser.BankName}");
            Console.WriteLine($"Account Number: {loggedInUser.BankAccountNumber}");
            Console.WriteLine($"IFSC: {loggedInUser.IFSC}");
            Console.WriteLine($"Total Transactions: {loggedInUser.PaymentHistory.Count}");
            decimal total = loggedInUser.PaymentHistory.Sum(p => p.Amount);
            Console.WriteLine($"Total Spent: ₹{total}");
            Console.ResetColor();
        }

        static void ShowPaymentHistory()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\n--- Payment History ---");
            if (loggedInUser.PaymentHistory.Count == 0) Console.WriteLine("No transactions yet.");
            else
            {
                foreach (var p in loggedInUser.PaymentHistory)
                    Console.WriteLine($"ID: {p.PaymentId}, Amount: ₹{p.Amount}, Date: {p.PaymentDate}, Type: {p.GetType().Name}, Status: {p.Status}");
            }
            Console.ResetColor();
        }

        static decimal SafeDecimalInput()
        {
            decimal value;
            while (!decimal.TryParse(Console.ReadLine(), out value) || value <= 0)
                Console.Write("Invalid amount! Enter again: ");
            return value;
        }
    }
}
