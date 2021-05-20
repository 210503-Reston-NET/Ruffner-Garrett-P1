using System.Net.Mail;
using System;
using System.Collections.Generic;
namespace StoreModels
{
    /// <summary>
    /// This class should contain necessary properties and fields for customer info.
    /// </summary>
    public class Customer
    {
        public Customer(string name, string Address, string Email)
        {
            this.Name = name;
            this.Address = Address;
            this.Email = Email;
        }
        // public Customer(string name, string Address, MailAddress Email)
        // {
        //     this.Name = name;
        //     this.Address = Address;
        //     this.Email = Email;
        // }
        // public Customer(string name, string Address, MailAddress Email, int id) : this(name,Address,Email)
        // {
        //    this.ID = id;
        // }
        public Customer(string name, string Address, string Email, int id) : this(name,Address,Email)
        {
           this.CustomerID = id;
        }

        public override string ToString()
        {
            return String.Format("{0}, {1}, {2}", this.Name, this.Address, this.Email);
        }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public int CustomerID { get; set; }
        
    }
}