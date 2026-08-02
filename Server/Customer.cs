using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EncryptedDbAtRest.Server;

public class Customer : Instance
{
    [NotMapped]
    public string FirstName { get; private set; }

    [NotMapped]
    public string LastName { get; private set; }

    [NotMapped]
    public string Address { get; private set; }
    
    [NotMapped]
    public string ZipCode { get; private set; }

    public Customer(Tenant tenant, string firstName, string lastName, string address, string zipCode) : base(tenant)
    {
        FirstName = firstName;
        LastName = lastName;
        Address = address;
        ZipCode = zipCode;
    }
}