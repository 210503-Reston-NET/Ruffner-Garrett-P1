using System.ComponentModel.DataAnnotations;
using StoreModels;
namespace WebUI.Models
{
    public class CustomerVM
    {
        public CustomerVM(Customer customer){
            Id = customer.CustomerID;
            Name = customer.Name;
            Address = customer.Address;
            Email = customer.Email;
        }
        public CustomerVM(){

        }
        public  int Id { get; set; }
        
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string Email { get; set; }
    }
}