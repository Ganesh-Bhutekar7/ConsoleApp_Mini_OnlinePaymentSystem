# ConsoleApp_Mini_OnlinePaymentSystem
A fully-featured console-based online payment system built in C# demonstrating Object-Oriented Programming (OOP) concepts, polymorphism, inheritance, interfaces, and abstract classes. This project simulates a real-world payment app with multiple users, secure login, bank integration, and transaction management.

Features :

User Authentication :
Register with Phone Number, Email, Password
Secure login with masked password input

Bank Account Integration :
Mandatory Bank Name, Account Number, IFSC during registration

Payment Methods :
Credit Card Payment (16-digit validation)
PayPal Payment (email validation)
UPI Payment (UPI ID validation)
Each payment type implements receipt generation

Transaction Management :
Maximum transaction limit: ₹5000
Transaction status: Pending, Success, Failed
Logs stored in PaymentLog.txt

User Profile :
View personal info, linked bank details, and payment history
Summary of total transactions and total spent

Polymorphism & OOP :
Single Payment reference used to process multiple payment types
Uses interfaces, abstract classes, and inheritance

Console Enhancements :
Color-coded menus and receipts
ar, attractive layout for better user experience


Technologies :
C#
.NET Console Application
File I/O for transaction logging
